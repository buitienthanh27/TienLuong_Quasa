using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Sample.Data.Migrations
{
    /// <inheritdoc />
    public partial class PayrollEntities_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounting_code",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    account_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: true),
                    cost_center_id = table.Column<int>(type: "int", nullable: true),
                    level = table.Column<int>(type: "int", nullable: false),
                    is_detail = table.Column<bool>(type: "bit", nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounting_code", x => x.id);
                    table.ForeignKey(
                        name: "FK_accounting_code_accounting_code_parent_id",
                        column: x => x.parent_id,
                        principalTable: "accounting_code",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_accounting_code_cost_center_cost_center_id",
                        column: x => x.cost_center_id,
                        principalTable: "cost_center",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "care_adjustment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    care_days = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    care_amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    adjustment_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    approved_by = table.Column<int>(type: "int", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    approval_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_care_adjustment", x => x.id);
                    table.ForeignKey(
                        name: "FK_care_adjustment_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employee_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    change_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    old_value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    new_value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    change_date = table.Column<DateTime>(type: "date", nullable: false),
                    changed_by = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    decision_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    decision_date = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_history_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payroll_audit",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payroll_id = table.Column<int>(type: "int", nullable: false),
                    audit_action = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    audit_data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    audited_by = table.Column<int>(type: "int", nullable: false),
                    audited_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    ip_address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_audit", x => x.id);
                    table.ForeignKey(
                        name: "FK_payroll_audit_payroll_payroll_id",
                        column: x => x.payroll_id,
                        principalTable: "payroll",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payroll_reconciliation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    tram_id = table.Column<int>(type: "int", nullable: true),
                    total_employees = table.Column<int>(type: "int", nullable: false),
                    total_gross_salary = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_deductions = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_net_salary = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_bhxh = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_bhyt = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_bhxh_company = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_bhyt_company = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_tax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_allowances = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    reconciliation_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    balanced_by = table.Column<int>(type: "int", nullable: true),
                    balanced_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    locked_by = table.Column<int>(type: "int", nullable: true),
                    locked_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_reconciliation", x => x.id);
                    table.ForeignKey(
                        name: "FK_payroll_reconciliation_tram_tram_id",
                        column: x => x.tram_id,
                        principalTable: "tram",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "zone_support",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tram_id = table.Column<int>(type: "int", nullable: false),
                    support_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    support_amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    support_rate = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zone_support", x => x.id);
                    table.ForeignKey(
                        name: "FK_zone_support_tram_tram_id",
                        column: x => x.tram_id,
                        principalTable: "tram",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accounting_code_code",
                table: "accounting_code",
                column: "code",
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_code_cost_center_id",
                table: "accounting_code",
                column: "cost_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_code_parent_id",
                table: "accounting_code",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_care_adjustment_employee_id_year_month",
                table: "care_adjustment",
                columns: new[] { "employee_id", "year_month" });

            migrationBuilder.CreateIndex(
                name: "IX_employee_history_employee_id_change_type_change_date",
                table: "employee_history",
                columns: new[] { "employee_id", "change_type", "change_date" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_audit_payroll_id_audited_at",
                table: "payroll_audit",
                columns: new[] { "payroll_id", "audited_at" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_reconciliation_tram_id",
                table: "payroll_reconciliation",
                column: "tram_id");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_reconciliation_year_month_tram_id",
                table: "payroll_reconciliation",
                columns: new[] { "year_month", "tram_id" },
                unique: true,
                filter: "[status] != -1");

            migrationBuilder.CreateIndex(
                name: "IX_zone_support_tram_id_support_type_effective_date",
                table: "zone_support",
                columns: new[] { "tram_id", "support_type", "effective_date" },
                unique: true,
                filter: "[status] != -1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounting_code");

            migrationBuilder.DropTable(
                name: "care_adjustment");

            migrationBuilder.DropTable(
                name: "employee_history");

            migrationBuilder.DropTable(
                name: "payroll_audit");

            migrationBuilder.DropTable(
                name: "payroll_reconciliation");

            migrationBuilder.DropTable(
                name: "zone_support");
        }
    }
}
