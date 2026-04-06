using Microsoft.EntityFrameworkCore;
using DNAAnalysis.Persistence.IdentityData.DbContext;
using DNAAnalysis.Domain.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Persistence.IdentityData.DataSeed;
using DNAAnalysis.Services;
using DNAAnalysis.Services.Abstraction;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DNAAnalysis.Persistence;
using DNAAnalysis.Persistence.Data.DBContexts;
using DNAAnalysis.Persistence.Repository;
using AutoMapper;
using DNAAnalysis.Services.MappingProfiles;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using DNAAnalysis.ServiceAbstraction;
using Microsoft.AspNetCore.Mvc;
using DNAAnalysis.API.Responses;
using System.Text.Json;

// ================= SERILOG CONFIG =================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// ✅ Activate Serilog
builder.Host.UseSerilog();

// ================= Swagger =================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token like this: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);

// ================= Controllers + Validation =================
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = new ApiResponse<string>(
                errors,
                "Bad Request"
            );

            return new BadRequestObjectResult(response);
        };
    });

// ================= Database =================
builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("IdentityConnection"));
});

builder.Services.AddDbContext<DNAAnalysisDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("IdentityConnection"));
});

// ================= Services =================
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<StoreIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IOtpRepository, OtpRepository>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGeneticAnalysisClient, FakeGeneticAnalysisClient>();

// ================= FluentValidation =================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ===== Generic Repository + Unit Of Work =====
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ===== Genetic Module Services =====
builder.Services.AddScoped<IGeneticRequestService, GeneticRequestService>();
builder.Services.AddScoped<IGeneticResultService, GeneticResultService>();

// ===== AutoMapper =====
builder.Services.AddAutoMapper(typeof(DrugProfile).Assembly);

// ===== Drug Module Service =====
builder.Services.AddScoped<IDrugService, DrugService>();
builder.Services.AddScoped<IDrugInteractionClient, FakeDrugInteractionClient>();

// ===== Nutrition Module Service =====
builder.Services.AddScoped<INutritionService, NutritionService>();

// ===== Alarm Module Service =====
builder.Services.AddScoped<IReminderService, ReminderService>();

// ================= JWT Authentication (🔥 المهم هنا) =================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
        ValidAudience = builder.Configuration["JwtOptions:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]!))
    };

    // 🔥 هنا بقى السحر
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<string>(
                new List<string> { "You are not logged in." },
                "Unauthorized"
            );

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        },

        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<string>(
                new List<string> { "You are not authorized. Admin access only." },
                "Forbidden"
            );

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    };
});

var app = builder.Build();

// ================= Data Seed =================
await app.MigrateIdentityDatabaseAsync();
await app.SeedIdentityDatabaseAsync();

// ================= Middleware =================
app.UseMiddleware<DNAAnalysis.API.Middleware.ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();