using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.DrugDtos;
using DNAAnalysis.API.Responses;

namespace DNAAnalysis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DrugController : ControllerBase
{
    private readonly IDrugService _drugService;

    public DrugController(IDrugService drugService)
    {
        _drugService = drugService;
    }

    // ================= ADMIN ONLY =================
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DrugInteractionDto>>>> GetAll()
    {
        var result = await _drugService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<DrugInteractionDto>>(result, "All interactions retrieved successfully"));
    }

    // ================= GET BY ID =================
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DrugInteractionDto>>> GetById(int id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var result = await _drugService.GetByIdAsync(id, userId, isAdmin);

        if (result is null)
            return NotFound(new ApiResponse<string>(new List<string> { "Interaction not found" }, "Not Found"));

        return Ok(new ApiResponse<DrugInteractionDto>(result, "Interaction retrieved successfully"));
    }

    // ================= USER HISTORY =================
    [HttpGet("my-history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DrugInteractionDto>>>> GetMyHistory()
    {
        var userId = GetUserId();
        var result = await _drugService.GetUserDrugInteractionsAsync(userId);
        return Ok(new ApiResponse<IEnumerable<DrugInteractionDto>>(result, "User history retrieved successfully"));
    }

    // ================= DELETE =================
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var deleted = await _drugService.DeleteInteractionAsync(id, userId, isAdmin);

        if (!deleted)
            return NotFound(new ApiResponse<string>(new List<string> { "Interaction not found" }, "Not Found"));

        return Ok(new ApiResponse<string>("Deleted Successfully", "Delete completed"));
    }

    // ================= CHECK INTERACTION =================
    [HttpPost("check-interaction")]
    public async Task<ActionResult<ApiResponse<DrugInteractionDto>>> CheckInteraction(
        CheckDrugInteractionRequest request)
    {
        var userId = GetUserId();
        var result = await _drugService.CheckInteractionAsync(request, userId);
        return Ok(new ApiResponse<DrugInteractionDto>(result, "Interaction checked successfully"));
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}