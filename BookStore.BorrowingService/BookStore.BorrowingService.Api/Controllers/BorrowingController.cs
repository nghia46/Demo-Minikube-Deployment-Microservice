using BookStore.BorrowingService.Commands.Commands.BorrowingRequests;
using BookStore.BorrowingService.Queries.Queries.Borrowing;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.BorrowingService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BorrowingController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBorrowingRequestAsync(BorrowingRequestCommand command)
    {
        var result = await sender.Send(command);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetsAsync()
    {
        var result = await sender.Send(new GetBorrowingRequestQuery());
        return Ok(result);
    }
}