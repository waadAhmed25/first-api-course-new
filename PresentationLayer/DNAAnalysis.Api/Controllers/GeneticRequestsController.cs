using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.GeneticRequestDtos;

namespace DNAAnalysis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneticRequestsController : ControllerBase
{
    private readonly IGeneticRequestService _service;

    public GeneticRequestsController(IGeneticRequestService service)
    {
        _service = service;
    }

    // ================= USER =================

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateGeneticRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var requestId = await _service.CreateRequestAsync(userId, dto);

        return Ok(new { Id = requestId });
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var requests = await _service.GetUserRequestsAsync(userId);

        return Ok(requests);
    }

    [Authorize]
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (userId is null)
        return Unauthorized();

    var request = await _service.GetByIdForUserAsync(id, userId, User.IsInRole("Admin"));

    if (request is null) 
        return NotFound();

    return Ok(request);
}

    // ================= ADMIN =================

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var requests = await _service.GetAllRequestsAsync();

        return Ok(requests);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateRequestStatusDto dto)
    {
        await _service.UpdateStatusAsync(id, dto.Status);

        return NoContent();
    }
}