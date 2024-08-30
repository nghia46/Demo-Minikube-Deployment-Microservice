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
builder.Services.AddSingleton(builder.Configuration);
builder.Configuration.AddJsonFile("/app/config/appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "BookStore Notification Service",
            Description = "Book store Notification Service",
            Version = "v1",
        });
});

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
        // Đăng ký consumer
        cfg.ReceiveEndpoint(builder.Configuration["RabbitMq:Queues:Borrowing"] ?? throw new NullReferenceException(), c =>
        {
            c.ConfigureConsumer<NotificationServiceConsumer>(context);
        });
        // Thiết lập Retry
        cfg.UseRetry(retryConfig =>
        {
            retryConfig.Interval(5, TimeSpan.FromSeconds(5)); // Thử lại 5 lần, mỗi lần cách nhau 5 giây
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
// Add MassTransit hosted service
builder.Services.AddHostedService<MassTransitHostedService>();

#endregion

builder.Configuration.AddEnvironmentVariables();
var app = builder.Build();
#region MassTransitHostedService

// Configure MassTransit bus control
var busControl = app.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

#endregion
#region Middleware Configuration
app.UseSwagger();
#region SwaggerUI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore Notification Service");
    c.RoutePrefix = "";
});

#endregion
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();
