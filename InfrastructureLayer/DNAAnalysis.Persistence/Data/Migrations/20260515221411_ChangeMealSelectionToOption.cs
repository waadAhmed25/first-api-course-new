using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAAnalysis.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMealSelectionToOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMealSelections_MealSuggestions_MealSuggestionId",
                table: "UserMealSelections");

            migrationBuilder.RenameColumn(
                name: "MealSuggestionId",
                table: "UserMealSelections",
                newName: "MealOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMealSelections_MealSuggestionId",
                table: "UserMealSelections",
                newName: "IX_UserMealSelections_MealOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMealSelections_MealOptions_MealOptionId",
                table: "UserMealSelections",
                column: "MealOptionId",
                principalTable: "MealOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMealSelections_MealOptions_MealOptionId",
                table: "UserMealSelections");

            migrationBuilder.RenameColumn(
                name: "MealOptionId",
                table: "UserMealSelections",
                newName: "MealSuggestionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMealSelections_MealOptionId",
                table: "UserMealSelections",
                newName: "IX_UserMealSelections_MealSuggestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMealSelections_MealSuggestions_MealSuggestionId",
                table: "UserMealSelections",
                column: "MealSuggestionId",
                principalTable: "MealSuggestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
