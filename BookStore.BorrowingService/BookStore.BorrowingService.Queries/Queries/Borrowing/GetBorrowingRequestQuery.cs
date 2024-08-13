using BookStore.Domain.Entities;
using MediatR;

namespace BookStore.BorrowingService.Queries.Queries.Borrowing;

public record GetBorrowingRequestQuery : IRequest<IEnumerable<BorrowingRequest>>;
