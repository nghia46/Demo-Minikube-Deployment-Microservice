using BookStore.Domain.Entities;
using MediatR;

namespace BookStore.BorrowingService.Commands.Commands.BorrowingRequests;

public record BorrowingRequestCommand(BorrowingRequest BorrowingRequest) : IRequest<BorrowingRequest>;