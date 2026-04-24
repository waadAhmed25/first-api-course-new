using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.GeneticRequestDtos;
using DNAAnalysis.Api.Requests;
using DNAAnalysis.API.Responses;
using DNAAnalysis.Shared.Enums;
using DNAAnalysis.API.Validators;

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

    // ================= CREATE =================

   [Authorize]
[HttpPost]
public async Task<ActionResult<ApiResponse<object>>> Create([FromForm] CreateGeneticRequestFormDto form)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (userId is null)
        return Unauthorized(new ApiResponse<string>(
            new List<string> { "Unauthorized" },
            "User not authenticated"));

    // ✅ VALIDATION (NEW)
    var errors = GeneticRequestValidator.Validate(
        form.FatherFile,
        form.MotherFile,
        form.IndividualFile,
        form.TestType);

    if (errors.Any())
        return BadRequest(new ApiResponse<string>(errors, "Validation error"));

    string? fatherPath = null;
    string? motherPath = null;
    string? individualPath = null;

    // ✅ SAVE FILES AFTER VALIDATION
    if (form.TestType == TestType.MoreThanOne)
    {
        fatherPath = await SaveFileAsync(form.FatherFile!);
        motherPath = await SaveFileAsync(form.MotherFile!);
    }
    else
    {
        individualPath = await SaveFileAsync(form.IndividualFile!);
    }

    var dto = new CreateGeneticRequestDto
    {
        FatherFilePath = fatherPath,
        MotherFilePath = motherPath,
        IndividualFilePath = individualPath,
        TestType = form.TestType
    };

    var requestId = await _service.CreateRequestAsync(userId, dto);

    return Ok(new ApiResponse<object>(
        new { Id = requestId },
        "Genetic request created successfully"));
}

    // ================= USER =================

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<object>>> GetMyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var data = await _service.GetUserRequestsAsync(userId!);

        return Ok(new ApiResponse<object>(data));
    }

    // ================= ADMIN =================

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll()
    {
        var data = await _service.GetAllRequestsAsync();

        return Ok(new ApiResponse<object>(data));
    }

    // ================= DETAILS =================

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _service.GetByIdForUserAsync(id, userId!, isAdmin);

        if (result == null)
            return NotFound(new ApiResponse<string>(
                new List<string> { "Request not found" }, "Not Found"));

        return Ok(new ApiResponse<object>(result));
    }

    // ================= UPDATE STATUS =================

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(
        int id,
      [FromBody] UpdateRequestStatusDto dto)
{
    if (dto == null)
    {
        return BadRequest(new ApiResponse<string>(
            new List<string> { "Request body is required." },
            "Validation Error"));
    }

    await _service.UpdateStatusAsync(id, dto.Status);

    return Ok(new ApiResponse<EmptyResponse>(
        new EmptyResponse(),
        "Status updated successfully"));
}

    // ================= PRIVATE =================

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