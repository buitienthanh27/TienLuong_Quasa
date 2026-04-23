using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Sample.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "employee_type_id",
                table: "employee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "insurance_note",
                table: "employee",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_difficult_area",
                table: "employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "advance_payment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    payment_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    payment_date = table.Column<DateTime>(type: "date", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_deducted = table.Column<bool>(type: "bit", nullable: false),
                    deducted_in_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    deducted_in_payroll_id = table.Column<int>(type: "int", nullable: true),
                    deducted_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    approved_by = table.Column<int>(type: "int", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advance_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_advance_payment_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employee_code_rule",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year = table.Column<int>(type: "int", nullable: false),
                    prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    digit_count = table.Column<int>(type: "int", nullable: false),
                    current_sequence = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_code_rule", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employee_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    salary_currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    payment_currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    has_insurance = table.Column<bool>(type: "bit", nullable: false),
                    calculation_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exchange_rate",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    from_currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    to_currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    rate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    approved_by = table.Column<int>(type: "int", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_rate", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "holiday",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year = table.Column<int>(type: "int", nullable: false),
                    holiday_date = table.Column<DateTime>(type: "date", nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    country = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    is_paid = table.Column<bool>(type: "bit", nullable: false),
                    bonus_rate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rubber_unit_price",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tram_id = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    difficult_area_bonus = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rubber_unit_price", x => x.id);
                    table.ForeignKey(
                        name: "FK_rubber_unit_price_tram_tram_id",
                        column: x => x.tram_id,
                        principalTable: "tram",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "technical_evaluation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    evaluated_grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    evaluation_score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    evaluated_by = table.Column<int>(type: "int", nullable: false),
                    evaluated_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    is_reviewed = table.Column<bool>(type: "bit", nullable: false),
                    reviewed_grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    reviewed_score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    reviewed_by = table.Column<int>(type: "int", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    final_grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technical_evaluation", x => x.id);
                    table.ForeignKey(
                        name: "FK_technical_evaluation_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "technical_grade",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    point_coefficient = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    min_score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    max_score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technical_grade", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    calculation_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    multiplier = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    applies_to_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_type", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_employee_type_id",
                table: "employee",
                column: "employee_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_advance_payment_employee_id_year_month",
                table: "advance_payment",
                columns: new[] { "employee_id", "year_month" });

            migrationBuilder.CreateIndex(
                name: "IX_employee_code_rule_year",
                table: "employee_code_rule",
                column: "year",
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_employee_type_code",
                table: "employee_type",
                column: "code",
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_exchange_rate_year_month_from_currency_to_currency",
                table: "exchange_rate",
                columns: new[] { "year_month", "from_currency", "to_currency" },
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_holiday_holiday_date",
                table: "holiday",
                column: "holiday_date",
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_rubber_unit_price_tram_id_grade_effective_date",
                table: "rubber_unit_price",
                columns: new[] { "tram_id", "grade", "effective_date" },
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_technical_evaluation_employee_id_year_month",
                table: "technical_evaluation",
                columns: new[] { "employee_id", "year_month" },
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_technical_grade_grade_effective_date",
                table: "technical_grade",
                columns: new[] { "grade", "effective_date" },
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_work_type_code_effective_date",
                table: "work_type",
                columns: new[] { "code", "effective_date" },
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.AddForeignKey(
                name: "FK_employee_employee_type_employee_type_id",
                table: "employee",
                column: "employee_type_id",
                principalTable: "employee_type",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_employee_type_employee_type_id",
                table: "employee");

            migrationBuilder.DropTable(
                name: "advance_payment");

            migrationBuilder.DropTable(
                name: "employee_code_rule");

            migrationBuilder.DropTable(
                name: "employee_type");

            migrationBuilder.DropTable(
                name: "exchange_rate");

            migrationBuilder.DropTable(
                name: "holiday");

            migrationBuilder.DropTable(
                name: "rubber_unit_price");

            migrationBuilder.DropTable(
                name: "technical_evaluation");

            migrationBuilder.DropTable(
                name: "technical_grade");

            migrationBuilder.DropTable(
                name: "work_type");

            migrationBuilder.DropIndex(
                name: "IX_employee_employee_type_id",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "employee_type_id",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "insurance_note",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "is_difficult_area",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "employee");
        }
    }
}
