using Invoicing.Application.DTOs;
using Invoicing.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoicing.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService service) : ControllerBase
{
    [HttpGet("allUsers")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));
}