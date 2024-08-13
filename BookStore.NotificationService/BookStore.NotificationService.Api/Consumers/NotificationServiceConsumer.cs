using BookStore.Domain.Entities;
using MassTransit;
using MongoDB.Bson;

namespace BookStore.NotificationService.Api.Consumers;

public class NotificationServiceConsumer : IConsumer<BorrowingRequest>
{
    public async Task Consume(ConsumeContext<BorrowingRequest> context)
    {
        var book = context.Message;
        await Console.Out.WriteLineAsync($"Borrowing book event received: {book.ToJson()}");
    }
}