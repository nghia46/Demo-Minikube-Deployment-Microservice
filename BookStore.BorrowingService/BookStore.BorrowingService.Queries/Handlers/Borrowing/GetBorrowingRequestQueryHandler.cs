using BookStore.BorrowingService.Domain.Interface;
using BookStore.BorrowingService.Queries.Queries.Borrowing;
using BookStore.Domain.Entities;
using MediatR;

namespace BookStore.BorrowingService.Queries.Handlers.Borrowing;

public class GetBorrowingRequestQueryHandler(IBorrowingRequestRepository borrowingRequestRepository)
    : IRequestHandler<GetBorrowingRequestQuery, IEnumerable<BorrowingRequest>>
{
    public async Task<IEnumerable<BorrowingRequest>> Handle(GetBorrowingRequestQuery request,
        CancellationToken cancellationToken)
    {
        return await borrowingRequestRepository.GetBorrowingRequestsAsync();
    }
}