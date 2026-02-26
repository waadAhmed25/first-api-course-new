using Microsoft.AspNetCore.Identity;
using DNAAnalysis.Domain.Entities.IdentityModule;
using DNAAnalysis.Domain.Contracts;

namespace DNAAnalysis.Persistence.IdentityData.DataSeed;

public class IdentityDataInitializer : IDataInitializer
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityDataInitializer(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // 1️⃣ Seed Roles (Safe Way)
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }

            // 2️⃣ Seed Real Admin
            var existingAdmin = await _userManager.FindByEmailAsync("waadodeana@outlook.com");

            if (existingAdmin == null)
            {
                var admin = new ApplicationUser()
                {
                    DisplayName = "waad Ahmed",
                    UserName = "waadAhmed27",
                    Email = "waadodeana@outlook.com", 
                    PhoneNumber = "01000000000",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, "Shahdwaad26#");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while seeding identity data: {ex}");
        }
    }
}