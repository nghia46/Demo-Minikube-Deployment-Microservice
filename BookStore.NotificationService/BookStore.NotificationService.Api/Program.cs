using BookStore.NotificationService.Api.Consumers;
using BookStore.NotificationService.Commands;
using BookStore.NotificationService.Queries;
using MassTransit;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

#region addedService
builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
{
    var uri = s.GetRequiredService<IConfiguration>()["MongoDb:LocalConnectionString"];
    return new MongoClient(uri);
});
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddQueries();
builder.Services.AddCommands();
#endregion

#region defaultService
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region AMQP
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationServiceConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMq:Host"] ?? throw new NullReferenceException()), h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? throw new NullReferenceException());
            h.Password(builder.Configuration["RabbitMq:Password"] ?? throw new NullReferenceException());
        });

        cfg.ReceiveEndpoint(builder.Configuration["RabbitMq:Queues:Borrowing"] ?? throw new NullReferenceException(), c =>
        {
            c.ConfigureConsumer<NotificationServiceConsumer>(context);
        });
        // Thiết lập Retry
        cfg.UseRetry(retryConfig =>
        {
            retryConfig.Interval(5, TimeSpan.FromSeconds(10)); // Thử lại 5 lần, mỗi lần cách nhau 10 giây
        });

        // Tùy chọn khác như Timeout, CircuitBreaker nếu cần
        cfg.UseCircuitBreaker(cbConfig =>
        {
            cbConfig.TrackingPeriod = TimeSpan.FromMinutes(1);
            cbConfig.ActiveThreshold = 5;
            cbConfig.ResetInterval = TimeSpan.FromMinutes(5);
        });
    });
});

// MassTransit hosted service is added automatically, no need to manually start the bus
#endregion

builder.Configuration.AddEnvironmentVariables();
var app = builder.Build();

#region Middleware Configuration
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
#endregion

app.Run();
