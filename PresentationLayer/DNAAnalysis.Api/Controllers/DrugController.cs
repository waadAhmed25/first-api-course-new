using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.DrugDtos;

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
    public async Task<ActionResult<IEnumerable<DrugInteractionDto>>> GetAll()
    {
        var result = await _drugService.GetAllAsync();
        return Ok(result);
    }

    // ================= GET BY ID =================
    [HttpGet("{id}")]
    public async Task<ActionResult<DrugInteractionDto>> GetById(int id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var result = await _drugService.GetByIdAsync(id, userId, isAdmin);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // ================= USER HISTORY =================
    [HttpGet("my-history")]
    public async Task<ActionResult<IEnumerable<DrugInteractionDto>>> GetMyHistory()
    {
        var userId = GetUserId();
        var result = await _drugService.GetUserDrugInteractionsAsync(userId);
        return Ok(result);
    }

    // ================= DELETE =================
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var deleted = await _drugService.DeleteInteractionAsync(id, userId, isAdmin);

        if (!deleted)
            return NotFound();

        return Ok("Deleted Successfully");
    }

    // ================= CHECK INTERACTION =================
    [HttpPost("check-interaction")]
    public async Task<ActionResult<DrugInteractionDto>> CheckInteraction(
        CheckDrugInteractionRequest request)
    {
        var userId = GetUserId();
        var result = await _drugService.CheckInteractionAsync(request, userId);
        return Ok(result);
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}
