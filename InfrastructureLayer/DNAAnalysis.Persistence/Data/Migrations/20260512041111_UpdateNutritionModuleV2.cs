using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAAnalysis.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNutritionModuleV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityLevel",
                table: "NutritionProfiles");

            migrationBuilder.DropColumn(
                name: "PatientStatus",
                table: "NutritionProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "NutritionProfiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Activity",
                table: "NutritionProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IncludeNightSnack",
                table: "NutritionProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "NutritionProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                table: "NutritionProfiles");

            migrationBuilder.DropColumn(
                name: "IncludeNightSnack",
                table: "NutritionProfiles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "NutritionProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "NutritionProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ActivityLevel",
                table: "NutritionProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientStatus",
                table: "NutritionProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
