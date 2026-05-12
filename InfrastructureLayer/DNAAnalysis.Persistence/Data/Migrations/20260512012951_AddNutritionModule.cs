using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAAnalysis.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCalories",
                table: "NutritionPlans");

            migrationBuilder.DropColumn(
                name: "FoodName",
                table: "MealSuggestions");

            migrationBuilder.DropColumn(
                name: "Grams",
                table: "MealSuggestions");

            migrationBuilder.RenameColumn(
                name: "ProteinPercentage",
                table: "NutritionPlans",
                newName: "Tdee");

            migrationBuilder.RenameColumn(
                name: "FatPercentage",
                table: "NutritionPlans",
                newName: "FinalCaloriesGoal");

            migrationBuilder.RenameColumn(
                name: "CarbsPercentage",
                table: "NutritionPlans",
                newName: "Bmr");

            migrationBuilder.AddColumn<string>(
                name: "AiRawResponse",
                table: "NutritionPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CarbsGrams",
                table: "MealSuggestions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FatGrams",
                table: "MealSuggestions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ProteinGrams",
                table: "MealSuggestions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "MealOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MealSuggestionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealOptions_MealSuggestions_MealSuggestionId",
                        column: x => x.MealSuggestionId,
                        principalTable: "MealSuggestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealOptions_MealSuggestionId",
                table: "MealOptions",
                column: "MealSuggestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealOptions");

            migrationBuilder.DropColumn(
                name: "AiRawResponse",
                table: "NutritionPlans");

            migrationBuilder.DropColumn(
                name: "CarbsGrams",
                table: "MealSuggestions");

            migrationBuilder.DropColumn(
                name: "FatGrams",
                table: "MealSuggestions");

            migrationBuilder.DropColumn(
                name: "ProteinGrams",
                table: "MealSuggestions");

            migrationBuilder.RenameColumn(
                name: "Tdee",
                table: "NutritionPlans",
                newName: "ProteinPercentage");

            migrationBuilder.RenameColumn(
                name: "FinalCaloriesGoal",
                table: "NutritionPlans",
                newName: "FatPercentage");

            migrationBuilder.RenameColumn(
                name: "Bmr",
                table: "NutritionPlans",
                newName: "CarbsPercentage");

            migrationBuilder.AddColumn<int>(
                name: "TotalCalories",
                table: "NutritionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FoodName",
                table: "MealSuggestions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Grams",
                table: "MealSuggestions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
