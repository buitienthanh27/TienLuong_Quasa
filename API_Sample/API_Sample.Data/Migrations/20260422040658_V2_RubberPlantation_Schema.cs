using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Sample.Data.Migrations
{
    /// <inheritdoc />
    public partial class V2_RubberPlantation_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "working_days",
                table: "attendance",
                newName: "young_tree_days");

            migrationBuilder.AddColumn<decimal>(
                name: "care_days",
                table: "attendance",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "double_cut_days",
                table: "attendance",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "double_cut_sunday",
                table: "attendance",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "hardship_days",
                table: "attendance",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "regular_days",
                table: "attendance",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_days",
                table: "attendance",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "drc_rate",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    tram_id = table.Column<int>(type: "int", nullable: true),
                    drc_raw_latex = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    drc_reference = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    drc_serum = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    drc_rope = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drc_rate", x => x.id);
                    table.ForeignKey(
                        name: "FK_drc_rate_tram_tram_id",
                        column: x => x.tram_id,
                        principalTable: "tram",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "production",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    raw_latex_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    rope_latex_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    serum_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    carry_over_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    drc_raw = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    drc_serum = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    dry_latex_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    carry_dry_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_pay_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    tech_grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production", x => x.id);
                    table.ForeignKey(
                        name: "FK_production_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_drc_rate_tram_id",
                table: "drc_rate",
                column: "tram_id");

            migrationBuilder.CreateIndex(
                name: "IX_drc_rate_year_month_tram_id",
                table: "drc_rate",
                columns: new[] { "year_month", "tram_id" },
                unique: true,
                filter: "[tram_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_production_employee_id_year_month",
                table: "production",
                columns: new[] { "employee_id", "year_month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "drc_rate");

            migrationBuilder.DropTable(
                name: "production");

            migrationBuilder.DropColumn(
                name: "care_days",
                table: "attendance");

            migrationBuilder.DropColumn(
                name: "double_cut_days",
                table: "attendance");

            migrationBuilder.DropColumn(
                name: "double_cut_sunday",
                table: "attendance");

            migrationBuilder.DropColumn(
                name: "hardship_days",
                table: "attendance");

            migrationBuilder.DropColumn(
                name: "regular_days",
                table: "attendance");

            migrationBuilder.DropColumn(
                name: "total_days",
                table: "attendance");

            migrationBuilder.RenameColumn(
                name: "young_tree_days",
                table: "attendance",
                newName: "working_days");
        }
    }
}
