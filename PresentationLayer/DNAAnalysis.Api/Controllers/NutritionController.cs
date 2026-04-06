using DNAAnalysis.API.Responses;
using DNAAnalysis.ServiceAbstraction;
using DNAAnalysis.Shared.NutritionDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DNAAnalysis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NutritionController : ControllerBase
{
    private readonly INutritionService _nutritionService;

    public NutritionController(INutritionService nutritionService)
    {
        _nutritionService = nutritionService;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    [HttpPost("profile")]
    public async Task<IActionResult> CreateProfile(CreateNutritionProfileDto dto)
    {
        var userId = GetUserId();

        await _nutritionService.CreateProfileAsync(userId, dto);

        return Ok(new ApiResponse<string>(
            "Profile saved successfully",
            "Nutrition profile created"
        ));
    }

    [HttpPost("generate-plan")]
    public async Task<IActionResult> GeneratePlan()
    {
        var userId = GetUserId();

        var plan = await _nutritionService.GeneratePlanAsync(userId);

        if (plan == null)
        {
            return BadRequest(new ApiResponse<string>(
                new[] { "Plan generation failed" },
                "Unable to generate plan"
            ));
        }

        return Ok(new ApiResponse<NutritionPlanDto>(
            plan,
            "Nutrition plan generated successfully"
        ));
    }

    [HttpGet("my-plan")]
    public async Task<IActionResult> GetMyPlan()
    {
        var userId = GetUserId();

        var plan = await _nutritionService.GetUserPlanAsync(userId);

        if (plan == null)
        {
            return NotFound(new ApiResponse<string>(
                new[] { "No nutrition plan found for this user" },
                "Plan not found"
            ));
        }

        return Ok(new ApiResponse<NutritionPlanDto>(
            plan,
            "Nutrition plan retrieved successfully"
        ));
    }

    // ✅ FIXED
    [HttpPost("select-meal/{mealId}")]
    public async Task<IActionResult> SelectMeal([FromRoute] int mealId)
    {
        var userId = GetUserId();

        await _nutritionService.SelectMealAsync(userId, mealId);

        return Ok(new ApiResponse<string>(
            "Meal selected",
            "Success"
        ));
    }

    // ✅ FIXED
    [HttpDelete("select-meal/{mealId}")]
    public async Task<IActionResult> UnselectMeal([FromRoute] int mealId)
    {
        var userId = GetUserId();

        await _nutritionService.UnselectMealAsync(userId, mealId);

        return Ok(new ApiResponse<string>(
            "Meal unselected",
            "Success"
        ));
    }
}