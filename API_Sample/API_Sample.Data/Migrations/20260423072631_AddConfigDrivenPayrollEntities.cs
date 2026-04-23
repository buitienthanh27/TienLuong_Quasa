using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Sample.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigDrivenPayrollEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "employee_type_id",
                table: "salary_scale",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "position_id",
                table: "salary_scale",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "priority",
                table: "salary_scale",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_taxable",
                table: "employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "payroll_policy",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    employee_type_id = table.Column<int>(type: "int", nullable: true),
                    tram_id = table.Column<int>(type: "int", nullable: true),
                    position_id = table.Column<int>(type: "int", nullable: true),
                    divisor_value = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    divisor_param_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    include_allowance = table.Column<bool>(type: "bit", nullable: false),
                    rounding_rule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    priority = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_policy", x => x.id);
                    table.ForeignKey(
                        name: "FK_payroll_policy_employee_type_employee_type_id",
                        column: x => x.employee_type_id,
                        principalTable: "employee_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_payroll_policy_position_position_id",
                        column: x => x.position_id,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_payroll_policy_tram_tram_id",
                        column: x => x.tram_id,
                        principalTable: "tram",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tax_bracket",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    from_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    to_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    tax_rate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    additional_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_bracket", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_salary_scale_employee_type_id",
                table: "salary_scale",
                column: "employee_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_salary_scale_position_id",
                table: "salary_scale",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_policy_code",
                table: "payroll_policy",
                column: "code",
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_policy_employee_type_id_tram_id_position_id_effective_date_priority",
                table: "payroll_policy",
                columns: new[] { "employee_type_id", "tram_id", "position_id", "effective_date", "priority" },
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_policy_position_id",
                table: "payroll_policy",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_policy_tram_id",
                table: "payroll_policy",
                column: "tram_id");

            migrationBuilder.CreateIndex(
                name: "IX_tax_bracket_sort_order_effective_date",
                table: "tax_bracket",
                columns: new[] { "sort_order", "effective_date" },
                filter: "[status] != -1");

            migrationBuilder.AddForeignKey(
                name: "FK_salary_scale_employee_type_employee_type_id",
                table: "salary_scale",
                column: "employee_type_id",
                principalTable: "employee_type",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_salary_scale_position_position_id",
                table: "salary_scale",
                column: "position_id",
                principalTable: "position",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salary_scale_employee_type_employee_type_id",
                table: "salary_scale");

            migrationBuilder.DropForeignKey(
                name: "FK_salary_scale_position_position_id",
                table: "salary_scale");

            migrationBuilder.DropTable(
                name: "payroll_policy");

            migrationBuilder.DropTable(
                name: "tax_bracket");

            migrationBuilder.DropIndex(
                name: "IX_salary_scale_employee_type_id",
                table: "salary_scale");

            migrationBuilder.DropIndex(
                name: "IX_salary_scale_position_id",
                table: "salary_scale");

            migrationBuilder.DropColumn(
                name: "employee_type_id",
                table: "salary_scale");

            migrationBuilder.DropColumn(
                name: "position_id",
                table: "salary_scale");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "salary_scale");

            migrationBuilder.DropColumn(
                name: "is_taxable",
                table: "employee");
        }
    }
}
