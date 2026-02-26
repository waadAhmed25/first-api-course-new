using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAAnalysis.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusColumnToGeneticRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "GeneticRequests");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GeneticRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "GeneticRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "GeneticRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
