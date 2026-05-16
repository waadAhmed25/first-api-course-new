using AutoMapper;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities.NutritionModule;
using DNAAnalysis.ServiceAbstraction;
using DNAAnalysis.Shared.NutritionDtos;
using System.Text.Json;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.NutritionDtos.AI;
using DNAAnalysis.Shared.Enums;
using DNAAnalysis.Domain.Entities;



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
    await mealRepo.GetAllIncludingAsync(
        x => x.NutritionPlanId == plan.Id,
        x => x.Options);

        var selectedMeals =
            await selectionRepo.GetAllAsync(
                x => x.UserId == userId);

        var selectedOptionIds =
    selectedMeals
        .Select(x => x.MealOptionId)
        .ToList();

        var eatenCalories = meals
    .Where(meal => meal.Options
        .Any(option => selectedOptionIds.Contains(option.Id)))
    .Sum(meal => meal.Calories);

        return new NutritionPlanDto
        {
            Bmr = plan.Bmr,

            Tdee = plan.Tdee,

            FinalCaloriesGoal =
                plan.FinalCaloriesGoal,

            EatenCalories = eatenCalories,

            RemainingCalories =
                plan.FinalCaloriesGoal - eatenCalories,

      MealPlan = meals.ToDictionary(
    x => x.MealType switch
    {
        MealType.Breakfast => "Breakfast",
        MealType.Lunch => "Lunch",
        MealType.Dinner => "Dinner",
        MealType.NightSnack => "Night Snack",
        _ => "Snack"
    },
    x => new MealResponseDto
    {
        Calories = x.Calories,

        Macros = new AiMacrosDto
        {
            Protein = new MacroValue
            {
                Grams = x.ProteinGrams
            },
            Carbs = new MacroValue
            {
                Grams = x.CarbsGrams
            },
            Fat = new MacroValue
            {
                Grams = x.FatGrams
            }
        },

        Options = x.Options
            .Select(o => new MealOptionDto
            {
                Id = o.Id,
                Name = o.Name
            })
            .ToList()
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
{
    var oldMeals =
        await mealRepo.GetAllIncludingAsync(
            x => x.NutritionPlanId == existingPlan.Id,
            x => x.Options);

   foreach (var meal in oldMeals)
{
    meal.Options.Clear();

    mealRepo.Remove(meal);
}

    planRepo.Remove(existingPlan);

    await _unitOfWork.SaveChangeAsync();
}

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
    Console.WriteLine(
    JsonSerializer.Serialize(
        aiRequest,
        new JsonSerializerOptions
        {
            WriteIndented = true
        }));
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
    .Select(meal => new MealSuggestionDto
    {
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

    public async Task SelectMealAsync(string userId, int optionId)
{
    if (optionId <= 0)
        throw new ArgumentException("Invalid option id");

    var selectionRepo =
        _unitOfWork.GetRepository<UserMealSelection, int>();

    var optionRepo =
        _unitOfWork.GetRepository<MealOption, int>();

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

    var option =
        await optionRepo.GetAsync(x =>
            x.Id == optionId &&
            x.MealSuggestion.NutritionPlanId == plan.Id);

    if (option == null)
    {
        throw new ArgumentException(
            "Meal option not found or not belongs to this user");
    }

    var existing =
        await selectionRepo.GetAsync(x =>
            x.UserId == userId &&
            x.MealOptionId == optionId);

    if (existing != null)
        throw new ArgumentException(
            "Meal already selected");

    var selection =
        new UserMealSelection
        {
            UserId = userId,
            MealOptionId = optionId
        };

    await selectionRepo.AddAsync(selection);

    await _unitOfWork.SaveChangeAsync();
}

    public async Task UnselectMealAsync(
    string userId,
    int optionId)
{
    if (optionId <= 0)
        throw new ArgumentException("Invalid option id");

    var repo =
        _unitOfWork.GetRepository<UserMealSelection, int>();

    var existing =
        await repo.GetAsync(x =>
            x.UserId == userId &&
            x.MealOptionId == optionId);

    if (existing == null)
    {
        throw new ArgumentException(
            "Meal selection not found");
    }

    repo.Remove(existing);

    await _unitOfWork.SaveChangeAsync();
}
}