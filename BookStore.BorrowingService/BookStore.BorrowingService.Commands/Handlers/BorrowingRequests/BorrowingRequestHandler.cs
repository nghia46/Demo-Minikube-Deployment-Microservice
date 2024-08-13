using BookStore.BorrowingService.Commands.Commands.BorrowingRequests;
using BookStore.Domain.Entities;
using BookStore.BorrowingService.Domain.Interface;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace BookStore.BorrowingService.Commands.Handlers.BorrowingRequests;

public class BorrowingRequestHandler(
    IConfiguration configuration,
    IBorrowingRequestRepository borrowingRequestRepository,
    ISendEndpointProvider provider) : IRequestHandler<BorrowingRequestCommand, BorrowingRequest>
{
    public async Task<BorrowingRequest> Handle(BorrowingRequestCommand request, CancellationToken cancellationToken)
    {
        var borrowingRequest = await borrowingRequestRepository.CreateBorrowingRequestAsync(request.BorrowingRequest);
        var queueName = configuration["RabbitMq:Queues:Borrowing"] ??
                        throw new NullReferenceException("Queue name configuration is missing.");

        var endpoint = await provider.GetSendEndpoint(new Uri($"queue:{queueName}") ??
                                                      throw new NullReferenceException(typeof(BorrowingRequest).Name));
        await endpoint.Send(borrowingRequest, cancellationToken);

        return borrowingRequest;
    }
}