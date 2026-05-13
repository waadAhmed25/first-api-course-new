using AutoMapper;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities.NutritionModule;
using DNAAnalysis.ServiceAbstraction;
using DNAAnalysis.Shared.NutritionDtos;
using System.Text.Json;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.NutritionDtos.AI;
using DNAAnalysis.Shared.Enums;


namespace DNAAnalysis.Services;

public class NutritionService : INutritionService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IAiNutritionClient _aiNutritionClient;

    public NutritionService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAiNutritionClient aiNutritionClient)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;

        _aiNutritionClient = aiNutritionClient;
    }

    public async Task CreateProfileAsync(
        string userId,
        CreateNutritionProfileDto dto)
    {
        var repo =
            _unitOfWork.GetRepository<NutritionProfile, int>();

        var existingProfile =
            await repo.GetAsync(x => x.UserId == userId);

        if (existingProfile != null)
        {
            existingProfile.Weight = dto.Weight;

            existingProfile.Height = dto.Height;

            existingProfile.Age = dto.Age;

            existingProfile.Gender = dto.Gender;

            existingProfile.Activity = dto.Activity;

            existingProfile.Status = dto.Status;

            existingProfile.IncludeNightSnack =
                dto.IncludeNightSnack;

            repo.Update(existingProfile);
        }
        else
        {
            var profile =
                _mapper.Map<NutritionProfile>(dto);

            profile.UserId = userId;

            await repo.AddAsync(profile);
        }

        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<NutritionPlanDto?> GetUserPlanAsync(
        string userId)
    {
        var profileRepo =
            _unitOfWork.GetRepository<NutritionProfile, int>();

        var planRepo =
            _unitOfWork.GetRepository<NutritionPlan, int>();

        var mealRepo =
            _unitOfWork.GetRepository<MealSuggestion, int>();

        var selectionRepo =
            _unitOfWork.GetRepository<UserMealSelection, int>();

        var profile =
            await profileRepo.GetAsync(x => x.UserId == userId);

        if (profile == null)
            return null;

        var plan =
            await planRepo.GetAsync(
                x => x.NutritionProfileId == profile.Id);

        if (plan == null)
            return null;

        var meals =
            await mealRepo.GetAllAsync(
                x => x.NutritionPlanId == plan.Id);

        var selectedMeals =
            await selectionRepo.GetAllAsync(
                x => x.UserId == userId);

        var selectedMealIds =
            selectedMeals
                .Select(x => x.MealSuggestionId)
                .ToList();

        var eatenCalories = meals
            .Where(x => selectedMealIds.Contains(x.Id))
            .Sum(x => x.Calories);

        return new NutritionPlanDto
        {
            Bmr = plan.Bmr,

            Tdee = plan.Tdee,

            FinalCaloriesGoal =
                plan.FinalCaloriesGoal,

            EatenCalories = eatenCalories,

            RemainingCalories =
                plan.FinalCaloriesGoal - eatenCalories,

            Meals = meals.Select(x => new MealSuggestionDto
            {
                Id = x.Id,

                MealType = x.MealType,

                Calories = x.Calories,

                ProteinGrams = x.ProteinGrams,

                CarbsGrams = x.CarbsGrams,

                FatGrams = x.FatGrams,

                Options =
                    x.Options.Select(o => o.Name)
            })
        };
    }

    public async Task<NutritionPlanDto?> GeneratePlanAsync(
        string userId)
    {
        var profileRepo =
            _unitOfWork.GetRepository<NutritionProfile, int>();

        var planRepo =
            _unitOfWork.GetRepository<NutritionPlan, int>();

        var mealRepo =
            _unitOfWork.GetRepository<MealSuggestion, int>();

        var profile =
            await profileRepo.GetAsync(
                x => x.UserId == userId);

        if (profile == null)
            throw new ArgumentException(
                "Nutrition profile not found");

        var existingPlan =
            await planRepo.GetAsync(
                x => x.NutritionProfileId == profile.Id);

        if (existingPlan != null)
            return await GetUserPlanAsync(userId);

        var aiRequest = new AiNutritionRequestDto
        {
            Weight = profile.Weight,

            Height = profile.Height,

            Age = profile.Age,

            Gender = profile.Gender,

            Activity = profile.Activity,

            Status = profile.Status,

            IncludeNightSnack =
                profile.IncludeNightSnack
        };
AiNutritionResponseDto aiResponse;

try
{
    aiResponse =
        await _aiNutritionClient
            .GeneratePlanAsync(aiRequest);
}
catch (Exception)
{
    throw new Exception(
        "AI nutrition service unavailable");
}

var meals = aiResponse.MealPlan
    .Select((meal, index) => new MealSuggestionDto
    {
        Id = index + 1,

        MealType = Enum.TryParse<MealType>(
            meal.Key.Replace(" ", ""),
            true,
            out var mealType)
            ? mealType
            : MealType.Snack,

        Calories = meal.Value.Calories,

        ProteinGrams =
            meal.Value.Macros.Protein.Grams,

        CarbsGrams =
            meal.Value.Macros.Carbs.Grams,

        FatGrams =
            meal.Value.Macros.Fat.Grams,

        Options = meal.Value.Options
    })
    .ToList();

        var plan = new NutritionPlan
        {
            NutritionProfileId = profile.Id,

            Bmr = aiResponse.Bmr,

            Tdee = aiResponse.Tdee,

            FinalCaloriesGoal =
                aiResponse.FinalCaloriesGoal,

            AiRawResponse =
                JsonSerializer.Serialize(aiResponse)
        };

        await planRepo.AddAsync(plan);

        await _unitOfWork.SaveChangeAsync();
foreach (var aiMeal in meals)
{
    var meal = new MealSuggestion
    {
        NutritionPlanId = plan.Id,

        MealType = aiMeal.MealType,

        Calories = aiMeal.Calories,

        ProteinGrams = aiMeal.ProteinGrams,

        CarbsGrams = aiMeal.CarbsGrams,

        FatGrams = aiMeal.FatGrams,

        Options = aiMeal.Options
            .Select(option => new MealOption
            {
                Name = option
            })
            .ToList()
    };

    await mealRepo.AddAsync(meal);
}

        await _unitOfWork.SaveChangeAsync();

        return await GetUserPlanAsync(userId);
    }

    public async Task SelectMealAsync(
        string userId,
        int mealId)
    {
        if (mealId <= 0)
            throw new ArgumentException("Invalid meal id");

        var selectionRepo =
            _unitOfWork.GetRepository<UserMealSelection, int>();

        var mealRepo =
            _unitOfWork.GetRepository<MealSuggestion, int>();

        var profileRepo =
            _unitOfWork.GetRepository<NutritionProfile, int>();

        var planRepo =
            _unitOfWork.GetRepository<NutritionPlan, int>();

        var profile =
            await profileRepo.GetAsync(
                x => x.UserId == userId);

        if (profile == null)
            throw new ArgumentException("Profile not found");

        var plan =
            await planRepo.GetAsync(
                x => x.NutritionProfileId == profile.Id);

        if (plan == null)
            throw new ArgumentException("Plan not found");

        var meal =
            await mealRepo.GetAsync(x =>
                x.Id == mealId &&
                x.NutritionPlanId == plan.Id);

        if (meal == null)
        {
            throw new ArgumentException(
                "Meal not found or not belongs to this user");
        }

        var existing =
            await selectionRepo.GetAsync(x =>
                x.UserId == userId &&
                x.MealSuggestionId == mealId);

        if (existing != null)
            throw new ArgumentException(
                "Meal already selected");

        var selection =
            new UserMealSelection
            {
                UserId = userId,

                MealSuggestionId = mealId
            };

        await selectionRepo.AddAsync(selection);

        await _unitOfWork.SaveChangeAsync();
    }

    public async Task UnselectMealAsync(
        string userId,
        int mealId)
    {
        if (mealId <= 0)
            throw new ArgumentException("Invalid meal id");

        var repo =
            _unitOfWork.GetRepository<UserMealSelection, int>();

        var existing =
            await repo.GetAsync(x =>
                x.UserId == userId &&
                x.MealSuggestionId == mealId);

        if (existing == null)
        {
            throw new ArgumentException(
                "Meal selection not found");
        }

        repo.Remove(existing);

        await _unitOfWork.SaveChangeAsync();
    }
}