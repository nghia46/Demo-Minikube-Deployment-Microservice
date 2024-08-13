using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.NotificationService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController(ISender sender) : ControllerBase
{
}