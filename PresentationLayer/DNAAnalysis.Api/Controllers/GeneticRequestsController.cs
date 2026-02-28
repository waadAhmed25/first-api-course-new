using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.GeneticRequestDtos;
using DNAAnalysis.Api.Requests;

namespace DNAAnalysis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneticRequestsController : ControllerBase
{
    private readonly IGeneticRequestService _service;

    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private readonly string[] AllowedExtensions = { ".txt", ".csv", ".vcf" };

    public GeneticRequestsController(IGeneticRequestService service)
    {
        _service = service;
    }

    // ================= USER =================

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateGeneticRequestFormDto form)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        string? fatherPath = null;
        string? motherPath = null;
        string? childPath = null;

        if (form.CombinedFile != null)
        {
            if (!ValidateFile(form.CombinedFile))
                return BadRequest("Invalid file (type or size)");

            fatherPath = await SaveFileAsync(form.CombinedFile);
        }
        else if (form.FatherFile != null && form.MotherFile != null)
        {
            if (!ValidateFile(form.FatherFile) || !ValidateFile(form.MotherFile))
                return BadRequest("Invalid file (type or size)");

            fatherPath = await SaveFileAsync(form.FatherFile);
            motherPath = await SaveFileAsync(form.MotherFile);

            if (form.ChildFile != null)
            {
                if (!ValidateFile(form.ChildFile))
                    return BadRequest("Invalid child file");

                childPath = await SaveFileAsync(form.ChildFile);
            }
        }
        else
        {
            return BadRequest("Invalid file input");
        }

        var dto = new CreateGeneticRequestDto
        {
            FatherFilePath = fatherPath!,
            MotherFilePath = motherPath!,
            ChildFilePath = childPath
        };

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

    // ================= PRIVATE =================

    private bool ValidateFile(IFormFile file)
    {
        if (file.Length == 0 || file.Length > MaxFileSize)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            return false;

        return true;
    }

    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Path.Combine("uploads", uniqueFileName);
    }
}