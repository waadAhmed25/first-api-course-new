using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAAnalysis.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvertGeneticResultToRawJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Advice",
                table: "GeneticResults");

            migrationBuilder.DropColumn(
                name: "Probabilities",
                table: "GeneticResults");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "GeneticResults");

            migrationBuilder.RenameColumn(
                name: "Explanation",
                table: "GeneticResults",
                newName: "RawAiResponse");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RawAiResponse",
                table: "GeneticResults",
                newName: "Explanation");

            migrationBuilder.AddColumn<string>(
                name: "Advice",
                table: "GeneticResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Probabilities",
                table: "GeneticResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "GeneticResults",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
