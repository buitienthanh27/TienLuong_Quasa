using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Sample.Data.Migrations
{
    /// <inheritdoc />
    public partial class V1_Payroll_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    use_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    account_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.id);
                },
                comment: "fruit-to-seed conversion rate");

            migrationBuilder.CreateTable(
                name: "allowance_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    default_rate = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allowance_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    table_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    record_id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    old_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    new_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    changed_by = table.Column<int>(type: "int", nullable: false),
                    changed_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cost_center",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    allocation_rate = table.Column<decimal>(type: "decimal(5,4)", nullable: true),
                    accounting_code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cost_center", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.id);
                    table.ForeignKey(
                        name: "FK_department_department_parent_id",
                        column: x => x.parent_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    serial_id = table.Column<int>(type: "int", nullable: true),
                    ref_id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    relative_url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    small_url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    medium_url = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    timer = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Images__3213E83F47534C37", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name_slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    sort = table.Column<int>(type: "int", nullable: true),
                    ratio_transfer = table.Column<double>(type: "float", nullable: true, comment: "Tỉ lệ chuyển đổi"),
                    remark = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.id);
                },
                comment: "fruit-to-seed conversion rate");

            migrationBuilder.CreateTable(
                name: "system_parameter",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    param_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    param_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    param_value = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    data_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_parameter", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tram",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tram", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    msnv = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    full_name_local = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    tram_id = table.Column<int>(type: "int", nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    position_id = table.Column<int>(type: "int", nullable: true),
                    technical_grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    hire_date = table.Column<DateTime>(type: "date", nullable: true),
                    birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    insurance_included = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_department_department_id",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_employee_position_position_id",
                        column: x => x.position_id,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_employee_tram_tram_id",
                        column: x => x.tram_id,
                        principalTable: "tram",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "salary_scale",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tram_id = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    coefficient = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    base_rate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    effective_date = table.Column<DateTime>(type: "date", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_scale", x => x.id);
                    table.ForeignKey(
                        name: "FK_salary_scale_tram_tram_id",
                        column: x => x.tram_id,
                        principalTable: "tram",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "allowance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    allowance_type_id = table.Column<int>(type: "int", nullable: false),
                    days_or_amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    calculated_amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allowance", x => x.id);
                    table.ForeignKey(
                        name: "FK_allowance_allowance_type_allowance_type_id",
                        column: x => x.allowance_type_id,
                        principalTable: "allowance_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_allowance_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attendance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    working_days = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    sunday_days = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    absent_days = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    sick_days = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    leave_days = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    attendance_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendance_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payroll",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    base_salary = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    performance_coef = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    working_days = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    gross_salary = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    bhxh = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    bhyt = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    income_tax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_deductions = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    total_allowances = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    net_salary = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    payroll_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    calculated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    calculated_by = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll", x => x.id);
                    table.ForeignKey(
                        name: "FK_payroll_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "performance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    year_month = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    grade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    output_kg = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_performance", x => x.id);
                    table.ForeignKey(
                        name: "FK_performance_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cost_allocation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payroll_id = table.Column<int>(type: "int", nullable: false),
                    cost_center_id = table.Column<int>(type: "int", nullable: false),
                    allocated_amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cost_allocation", x => x.id);
                    table.ForeignKey(
                        name: "FK_cost_allocation_cost_center_cost_center_id",
                        column: x => x.cost_center_id,
                        principalTable: "cost_center",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cost_allocation_payroll_payroll_id",
                        column: x => x.payroll_id,
                        principalTable: "payroll",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payroll_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payroll_id = table.Column<int>(type: "int", nullable: false),
                    phase = table.Column<int>(type: "int", nullable: false),
                    item_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_payroll_detail_payroll_payroll_id",
                        column: x => x.payroll_id,
                        principalTable: "payroll",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_allowance_allowance_type_id",
                table: "allowance",
                column: "allowance_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_allowance_employee_id",
                table: "allowance",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_allowance_type_code",
                table: "allowance_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_attendance_employee_id_year_month",
                table: "attendance",
                columns: new[] { "employee_id", "year_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_changed_at",
                table: "audit_log",
                column: "changed_at");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_table_name_record_id",
                table: "audit_log",
                columns: new[] { "table_name", "record_id" });

            migrationBuilder.CreateIndex(
                name: "IX_cost_allocation_cost_center_id",
                table: "cost_allocation",
                column: "cost_center_id");

            migrationBuilder.CreateIndex(
                name: "IX_cost_allocation_payroll_id",
                table: "cost_allocation",
                column: "payroll_id");

            migrationBuilder.CreateIndex(
                name: "IX_cost_center_code",
                table: "cost_center",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_department_code",
                table: "department",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_department_parent_id",
                table: "department",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_department_id",
                table: "employee",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_msnv",
                table: "employee",
                column: "msnv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_position_id",
                table: "employee",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_tram_id",
                table: "employee",
                column: "tram_id");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_employee_id_year_month",
                table: "payroll",
                columns: new[] { "employee_id", "year_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payroll_detail_payroll_id",
                table: "payroll_detail",
                column: "payroll_id");

            migrationBuilder.CreateIndex(
                name: "IX_performance_employee_id_year_month",
                table: "performance",
                columns: new[] { "employee_id", "year_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_position_code",
                table: "position",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salary_scale_tram_id_grade_effective_date",
                table: "salary_scale",
                columns: new[] { "tram_id", "grade", "effective_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_system_parameter_param_code_effective_date",
                table: "system_parameter",
                columns: new[] { "param_code", "effective_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tram_code",
                table: "tram",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "allowance");

            migrationBuilder.DropTable(
                name: "attendance");

            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "cost_allocation");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "payroll_detail");

            migrationBuilder.DropTable(
                name: "performance");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "salary_scale");

            migrationBuilder.DropTable(
                name: "system_parameter");

            migrationBuilder.DropTable(
                name: "allowance_type");

            migrationBuilder.DropTable(
                name: "cost_center");

            migrationBuilder.DropTable(
                name: "payroll");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "position");

            migrationBuilder.DropTable(
                name: "tram");
        }
    }
}
