using AutoMapper;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities.NutritionModule;
using DNAAnalysis.ServiceAbstraction;
using DNAAnalysis.Shared.Enums;
using DNAAnalysis.Shared.NutritionDtos;

namespace DNAAnalysis.Services;


public class NutritionService : INutritionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NutritionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task CreateProfileAsync(string userId, CreateNutritionProfileDto dto)
    {
        var repo = _unitOfWork.GetRepository<NutritionProfile, int>();

        var existingProfile = await repo.GetAsync(x => x.UserId == userId);

        if (existingProfile != null)
        {
            existingProfile.Weight = dto.Weight;
            existingProfile.Height = dto.Height;
            existingProfile.Age = dto.Age;
            existingProfile.Gender = dto.Gender;
            existingProfile.ActivityLevel = dto.ActivityLevel;
            existingProfile.PatientStatus = dto.PatientStatus;

            repo.Update(existingProfile);
        }
        else
        {
            var profile = _mapper.Map<NutritionProfile>(dto);
            profile.UserId = userId;

            await repo.AddAsync(profile);
        }

        await _unitOfWork.SaveChangeAsync();
    }

   public async Task<NutritionPlanDto?> GetUserPlanAsync(string userId)
{
    var profileRepo = _unitOfWork.GetRepository<NutritionProfile, int>();
    var planRepo = _unitOfWork.GetRepository<NutritionPlan, int>();
    var mealRepo = _unitOfWork.GetRepository<MealSuggestion, int>();
    var selectionRepo = _unitOfWork.GetRepository<UserMealSelection, int>();

    var profile = await profileRepo.GetAsync(x => x.UserId == userId);

    if (profile == null)
        return null;

    var plan = await planRepo.GetAsync(x => x.NutritionProfileId == profile.Id);

    if (plan == null)
        return null;

    var meals = await mealRepo.GetAllAsync(x => x.NutritionPlanId == plan.Id);

    var selectedMeals = await selectionRepo.GetAllAsync(x => x.UserId == userId);

    var selectedMealIds = selectedMeals.Select(x => x.MealSuggestionId).ToList();

    var eatenCalories = meals
        .Where(x => selectedMealIds.Contains(x.Id))
        .Sum(x => x.Calories);

    return new NutritionPlanDto
    {
        TotalCalories = plan.TotalCalories,
        ProteinPercentage = plan.ProteinPercentage,
        CarbsPercentage = plan.CarbsPercentage,
        FatPercentage = plan.FatPercentage,
        EatenCalories = eatenCalories,
        RemainingCalories = plan.TotalCalories - eatenCalories,
        Meals = meals.Select(x => new MealSuggestionDto
        {
             Id = x.Id,
            MealType = x.MealType,
            FoodName = x.FoodName,
            Calories = x.Calories,
            Grams = x.Grams
        })
    };
}

    public async Task<NutritionPlanDto?> GeneratePlanAsync(string userId)
    {
        var profileRepo = _unitOfWork.GetRepository<NutritionProfile, int>();
        var planRepo = _unitOfWork.GetRepository<NutritionPlan, int>();
        var mealRepo = _unitOfWork.GetRepository<MealSuggestion, int>();

        var profile = await profileRepo.GetAsync(x => x.UserId == userId);

        if (profile == null)
            throw new Exception("Nutrition profile not found");

        var existingPlan = await planRepo.GetAsync(x => x.NutritionProfileId == profile.Id);

        if (existingPlan != null)
        {
            return await GetUserPlanAsync(userId);
        }

        // Fake AI result for now
        var plan = new NutritionPlan
        {
            NutritionProfileId = profile.Id,
            TotalCalories = 2560,
            ProteinPercentage = 20,
            CarbsPercentage = 45,
            FatPercentage = 35
        };

        await planRepo.AddAsync(plan);
        await _unitOfWork.SaveChangeAsync();

        var meals = new List<MealSuggestion>
        {
            new MealSuggestion
            {
                NutritionPlanId = plan.Id,
                MealType = MealType.Breakfast,
                FoodName = "Scrambled Eggs",
                Calories = 302,
                Grams = 150
            },
            new MealSuggestion
            {
                NutritionPlanId = plan.Id,
                MealType = MealType.Lunch,
                FoodName = "Chicken Soup",
                Calories = 320,
                Grams = 200
            },
            new MealSuggestion
            {
                NutritionPlanId = plan.Id,
                MealType = MealType.Dinner,
                FoodName = "Soft Pasta",
                Calories = 400,
                Grams = 220
            },
            new MealSuggestion
            {
                NutritionPlanId = plan.Id,
                MealType = MealType.Snack,
                FoodName = "Yogurt",
                Calories = 150,
                Grams = 120
            }
        };

        foreach (var meal in meals)
        {
            await mealRepo.AddAsync(meal);
        }

        await _unitOfWork.SaveChangeAsync();

        return await GetUserPlanAsync(userId);
    }

   public async Task SelectMealAsync(string userId, int mealId)
{
    var selectionRepo = _unitOfWork.GetRepository<UserMealSelection, int>();
    var mealRepo = _unitOfWork.GetRepository<MealSuggestion, int>();
    var profileRepo = _unitOfWork.GetRepository<NutritionProfile, int>();
    var planRepo = _unitOfWork.GetRepository<NutritionPlan, int>();

    // ✅ 1. هات اليوزر بروفايل
    var profile = await profileRepo.GetAsync(x => x.UserId == userId);
    if (profile == null)
        throw new Exception("Profile not found");

    // ✅ 2. هات الخطة
    var plan = await planRepo.GetAsync(x => x.NutritionProfileId == profile.Id);
    if (plan == null)
        throw new Exception("Plan not found");

    // ✅ 3. اتأكد إن الوجبة بتاعته
    var meal = await mealRepo.GetAsync(x => x.Id == mealId && x.NutritionPlanId == plan.Id);
    if (meal == null)
    throw new ArgumentException("Meal not found or not belongs to this user");

    // ✅ 4. منع التكرار
    var existing = await selectionRepo.GetAsync(x => x.UserId == userId && x.MealSuggestionId == mealId);
    if (existing != null)
        return;

    // ✅ 5. حفظ الاختيار
    var selection = new UserMealSelection
    {
        UserId = userId,
        MealSuggestionId = mealId
    };

    await selectionRepo.AddAsync(selection);
    await _unitOfWork.SaveChangeAsync();
}

public async Task UnselectMealAsync(string userId, int mealId)
{
    var repo = _unitOfWork.GetRepository<UserMealSelection, int>();

    var existing = await repo.GetAsync(x => x.UserId == userId && x.MealSuggestionId == mealId);

    if (existing == null)
    throw new ArgumentException("Meal selection not found");

    repo.Remove(existing);
    await _unitOfWork.SaveChangeAsync();
}
}