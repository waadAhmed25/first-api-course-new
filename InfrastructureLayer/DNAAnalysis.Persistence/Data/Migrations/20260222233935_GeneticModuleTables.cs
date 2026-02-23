using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAAnalysis.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class GeneticModuleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneticRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FatherFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotherFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChildFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneticRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneticResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FatherStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MotherStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChildStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageToPatient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Advice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneticRequestId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneticResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneticResults_GeneticRequests_GeneticRequestId",
                        column: x => x.GeneticRequestId,
                        principalTable: "GeneticRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneticResults_GeneticRequestId",
                table: "GeneticResults",
                column: "GeneticRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneticResults");

            migrationBuilder.DropTable(
                name: "GeneticRequests");
        }
    }
}
