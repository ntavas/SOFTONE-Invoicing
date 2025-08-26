using Invoicing.Api.Extensions;
using Invoicing.Api.Responses;
using Invoicing.Application.DTOs;
using Invoicing.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoicing.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService service, ILogger<UsersController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    [HttpGet("getAllUsers")]
    public async Task<IActionResult> GetAll(CancellationToken ct) {
        var result = await _service.GetAllAsync(ct);
        return this.ToActionResult(result, _logger);
    }
}