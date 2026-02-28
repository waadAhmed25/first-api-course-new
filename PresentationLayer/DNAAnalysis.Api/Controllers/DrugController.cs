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

    // ================= GET ALL (Admin / Testing) =================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DrugInteractionDto>>> GetAll()
    {
        var result = await _drugService.GetAllAsync();
        return Ok(result);
    }

    // ================= GET BY ID =================
    [HttpGet("{id}")]
    public async Task<ActionResult<DrugInteractionDto>> GetById(int id)
    {
        var result = await _drugService.GetByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // ================= ADD NEW INTERACTION =================
    [HttpPost]
    public async Task<ActionResult> Add(DrugInteractionDto dto)
    {
        // نجيب اليوزر الحالي من التوكن
        dto.UserId = GetUserId();

        await _drugService.AddAsync(dto);

        return Ok("Drug Interaction Added Successfully");
    }

    // ================= GET USER HISTORY =================
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

        var deleted = await _drugService.DeleteInteractionAsync(id, userId);

        if (!deleted)
            return NotFound();

        return Ok("Deleted Successfully");
    }

    // ================= HELPER =================
    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }
}