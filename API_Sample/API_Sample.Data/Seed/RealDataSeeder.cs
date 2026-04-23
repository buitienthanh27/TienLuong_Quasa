using API_Sample.Data.EF;
using API_Sample.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace API_Sample.Data.Seed
{
    /// <summary>
    /// Seeder sử dụng dữ liệu thực từ Excel tháng 11/2025
    /// </summary>
    public class RealDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            var logger = scope.ServiceProvider.GetService<ILogger<RealDataSeeder>>();

            try
            {
                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", "SeedData_Nov2025.json");

                // Fallback to source path during development
                if (!File.Exists(jsonPath))
                {
                    jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "API_Sample.Data", "Seed", "SeedData_Nov2025.json");
                }

                if (!File.Exists(jsonPath))
                {
                    logger?.LogWarning("RealDataSeeder: JSON file not found at {Path}", jsonPath);
                    return;
                }

                var json = await File.ReadAllTextAsync(jsonPath);
                var seedData = JsonSerializer.Deserialize<SeedDataModel>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (seedData == null)
                {
                    logger?.LogWarning("RealDataSeeder: Failed to deserialize JSON");
                    return;
                }

                logger?.LogInformation("RealDataSeeder: Loading data from {YearMonth}", seedData.YearMonth);

                // Seed accounts first (for authentication)
                await SeedAccounts(context);

                await SeedTrams(context, seedData);
                await SeedEmployeeTypes(context, seedData);
                await SeedTechnicalGrades(context, seedData);
                await SeedDrcRates(context, seedData);
                await SeedRubberUnitPrices(context, seedData);
                await SeedExchangeRates(context, seedData);
                await SeedWorkTypes(context, seedData);
                await SeedSystemParameters(context, seedData);
                await SeedPositions(context);
                await SeedEmployeesFromExcel(context, seedData);
                await SeedProductionsFromExcel(context, seedData);
                await SeedTaxBrackets(context);
                await SeedPayrollPolicies(context);

                logger?.LogInformation("RealDataSeeder: Completed! Employees={Emp}, Productions={Prod}",
                    await context.Employees.CountAsync(),
                    await context.Productions.CountAsync());
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "RealDataSeeder: Error seeding data");
                throw;
            }
        }

        private static async Task SeedAccounts(MainDbContext context)
        {
            if (await context.Accounts.AnyAsync()) return;

            var now = DateTime.UtcNow;
            context.Accounts.AddRange(
                new Account
                {
                    Id = 1,
                    UserName = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Email = "admin@quasa.com",
                    FirstName = "Admin",
                    LastName = "System",
                    AccountType = "ADMIN",
                    Phone = "0123456789",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new Account
                {
                    Id = 2,
                    UserName = "ketoan",
                    Password = BCrypt.Net.BCrypt.HashPassword("Ketoan@123"),
                    Email = "ketoan@quasa.com",
                    FirstName = "Ke",
                    LastName = "Toan",
                    AccountType = "STAFF",
                    Phone = "0123456780",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new Account
                {
                    Id = 3,
                    UserName = "hr",
                    Password = BCrypt.Net.BCrypt.HashPassword("Hr@123456"),
                    Email = "hr@quasa.com",
                    FirstName = "Nhan",
                    LastName = "Su",
                    AccountType = "STAFF",
                    Phone = "0123456781",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedTrams(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.Trams.AnyAsync()) return;

            var now = DateTime.UtcNow;
            foreach (var t in seedData.Trams)
            {
                context.Trams.Add(new Tram
                {
                    Code = t.Code,
                    Name = t.Name,
                    Description = t.Description,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedEmployeeTypes(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.EmployeeTypes.AnyAsync()) return;

            var now = DateTime.UtcNow;
            foreach (var et in seedData.EmployeeTypes)
            {
                context.EmployeeTypes.Add(new EmployeeType
                {
                    Code = et.Code,
                    Name = et.Name,
                    SalaryCurrency = et.SalaryCurrency,
                    PaymentCurrency = et.PaymentCurrency,
                    CalculationMethod = et.CalculationMethod,
                    HasInsurance = et.HasInsurance,
                    SortOrder = et.SortOrder,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedTechnicalGrades(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.TechnicalGrades.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 11, 1);

            foreach (var tg in seedData.TechnicalGrades)
            {
                context.TechnicalGrades.Add(new TechnicalGrade
                {
                    Grade = tg.Grade,
                    Name = tg.Name,
                    PointCoefficient = tg.PointCoefficient,
                    SortOrder = tg.SortOrder,
                    EffectiveDate = effectiveDate,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedDrcRates(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.DrcRates.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var trams = await context.Trams.ToListAsync();

            foreach (var drc in seedData.DrcRates)
            {
                var tram = trams.FirstOrDefault(t => t.Code == drc.TramCode);
                if (tram == null) continue;

                context.DrcRates.Add(new DrcRate
                {
                    TramId = tram.Id,
                    YearMonth = drc.YearMonth,
                    DrcRawLatex = drc.DrcRate,
                    Note = drc.Source,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedRubberUnitPrices(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.RubberUnitPrices.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 11, 1);
            var trams = await context.Trams.ToListAsync();

            foreach (var rup in seedData.RubberUnitPrices)
            {
                var tram = trams.FirstOrDefault(t => t.Code == rup.TramCode);
                if (tram == null) continue;

                context.RubberUnitPrices.Add(new RubberUnitPrice
                {
                    TramId = tram.Id,
                    Grade = rup.Grade,
                    UnitPrice = rup.UnitPriceKip,
                    Currency = "LAK",
                    DifficultAreaBonus = rup.IsDifficultArea ? 1000m : 0m,
                    EffectiveDate = effectiveDate,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedExchangeRates(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.ExchangeRates.AnyAsync()) return;

            var now = DateTime.UtcNow;

            foreach (var er in seedData.ExchangeRates)
            {
                context.ExchangeRates.Add(new ExchangeRate
                {
                    YearMonth = er.YearMonth,
                    FromCurrency = er.FromCurrency,
                    ToCurrency = er.ToCurrency,
                    Rate = er.Rate,
                    Source = er.Source,
                    ApprovedBy = 1,
                    ApprovedAt = now,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedWorkTypes(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.WorkTypes.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 11, 1);

            foreach (var wt in seedData.WorkTypes)
            {
                context.WorkTypes.Add(new WorkType
                {
                    Code = wt.Code,
                    Name = wt.Name,
                    UnitPrice = wt.UnitPrice,
                    Currency = wt.Currency,
                    CalculationType = "PER_DAY",
                    EffectiveDate = effectiveDate,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSystemParameters(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.SystemParameters.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 11, 1);

            foreach (var sp in seedData.SystemParameters)
            {
                context.SystemParameters.Add(new SystemParameter
                {
                    ParamCode = sp.ParamCode,
                    ParamName = sp.Description,
                    ParamValue = sp.ParamValue,
                    DataType = sp.ParamValue < 1 ? "RATE" : "NUMBER",
                    EffectiveDate = effectiveDate,
                    Description = sp.Description,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedPositions(MainDbContext context)
        {
            if (await context.Positions.AnyAsync()) return;

            var now = DateTime.UtcNow;
            context.Positions.AddRange(
                new Position { Code = "CNKT", Name = "Công nhân kỹ thuật", Type = "CNKT", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "BV", Name = "Bảo vệ", Type = "BV", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "TV", Name = "Tạp vụ", Type = "TV", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "CB", Name = "Cán bộ", Type = "CB", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "CS", Name = "Chăm sóc", Type = "CS", Status = 1, CreatedAt = now, CreatedBy = 1 }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedEmployeesFromExcel(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.Employees.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var trams = await context.Trams.ToListAsync();
            var positions = await context.Positions.ToListAsync();
            var employeeTypes = await context.EmployeeTypes.ToListAsync();

            var cnktPosition = positions.First(p => p.Code == "CNKT");
            var cnktType = employeeTypes.First(et => et.Code == "CNKT");

            foreach (var emp in seedData.Employees)
            {
                var tram = trams.FirstOrDefault(t => t.Code == emp.TramCode);
                var position = positions.FirstOrDefault(p => p.Code == emp.Position) ?? cnktPosition;
                var employeeType = employeeTypes.FirstOrDefault(et => et.Code == emp.Position) ?? cnktType;

                if (tram == null) continue;

                context.Employees.Add(new Employee
                {
                    Msnv = emp.Msnv,
                    FullName = emp.FullName,
                    TramId = tram.Id,
                    PositionId = position.Id,
                    EmployeeTypeId = employeeType.Id,
                    TechnicalGrade = emp.TechnicalGrade,
                    IsDifficultArea = emp.TramCode == "T1",
                    Nationality = "LAO",
                    HireDate = new DateTime(2020, 1, 1),
                    InsuranceIncluded = employeeType.HasInsurance,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }

            // Thêm BV/TV
            var bvPosition = positions.First(p => p.Code == "BV");
            var tvPosition = positions.First(p => p.Code == "TV");
            var bvType = employeeTypes.First(et => et.Code == "BV");
            var tvType = employeeTypes.First(et => et.Code == "TV");
            var t1 = trams.First(t => t.Code == "T1");

            context.Employees.AddRange(
                new Employee { Msnv = "BV001", FullName = "San", TramId = t1.Id, PositionId = bvPosition.Id, EmployeeTypeId = bvType.Id, Nationality = "LAO", HireDate = new DateTime(2020, 1, 1), InsuranceIncluded = false, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Employee { Msnv = "BV002", FullName = "Thảo Nọi", TramId = t1.Id, PositionId = bvPosition.Id, EmployeeTypeId = bvType.Id, Nationality = "LAO", HireDate = new DateTime(2020, 1, 1), InsuranceIncluded = false, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Employee { Msnv = "TV001", FullName = "Nàng Ta", TramId = t1.Id, PositionId = tvPosition.Id, EmployeeTypeId = tvType.Id, Nationality = "LAO", HireDate = new DateTime(2020, 1, 1), InsuranceIncluded = false, Status = 1, CreatedAt = now, CreatedBy = 1 }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedProductionsFromExcel(MainDbContext context, SeedDataModel seedData)
        {
            if (await context.Productions.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var employees = await context.Employees.ToListAsync();

            foreach (var prod in seedData.Productions)
            {
                var emp = employees.FirstOrDefault(e => e.Msnv == prod.EmployeeMsnv);
                if (emp == null) continue;

                context.Productions.Add(new Production
                {
                    EmployeeId = emp.Id,
                    YearMonth = prod.YearMonth,
                    RawLatexKg = prod.RawLatexKg,
                    DryLatexKg = prod.DryLatexKg,
                    TotalPayKg = prod.DryLatexKg,
                    TechGrade = prod.Grade,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed bậc thuế thu nhập theo Luật thuế Lào
        /// </summary>
        private static async Task SeedTaxBrackets(MainDbContext context)
        {
            if (await context.TaxBrackets.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 1, 1);

            context.TaxBrackets.AddRange(
                new TaxBracket
                {
                    FromAmount = 0,
                    ToAmount = 5000000,
                    TaxRate = 0,
                    AdditionalAmount = 0,
                    EffectiveDate = effectiveDate,
                    SortOrder = 1,
                    Description = "Thu nhập ≤ 5,000,000 LAK: 0%",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 5000000,
                    ToAmount = 15000000,
                    TaxRate = 0.05m,
                    AdditionalAmount = 0,
                    EffectiveDate = effectiveDate,
                    SortOrder = 2,
                    Description = "Thu nhập 5,000,001 - 15,000,000 LAK: 5%",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 15000000,
                    ToAmount = 25000000,
                    TaxRate = 0.10m,
                    AdditionalAmount = 500000,
                    EffectiveDate = effectiveDate,
                    SortOrder = 3,
                    Description = "Thu nhập 15,000,001 - 25,000,000 LAK: 10%",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 25000000,
                    ToAmount = 45000000,
                    TaxRate = 0.15m,
                    AdditionalAmount = 1500000,
                    EffectiveDate = effectiveDate,
                    SortOrder = 4,
                    Description = "Thu nhập 25,000,001 - 45,000,000 LAK: 15%",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 45000000,
                    ToAmount = 65000000,
                    TaxRate = 0.20m,
                    AdditionalAmount = 4500000,
                    EffectiveDate = effectiveDate,
                    SortOrder = 5,
                    Description = "Thu nhập 45,000,001 - 65,000,000 LAK: 20%",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 65000000,
                    ToAmount = null,
                    TaxRate = 0.25m,
                    AdditionalAmount = 8500000,
                    EffectiveDate = effectiveDate,
                    SortOrder = 6,
                    Description = "Thu nhập > 65,000,000 LAK: 25%",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                }
            );
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed chính sách tính lương FIXED cho CB.CNV, Bảo vệ, Tạp vụ
        /// </summary>
        private static async Task SeedPayrollPolicies(MainDbContext context)
        {
            if (await context.PayrollPolicies.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 1, 1);

            var cbCnvType = await context.EmployeeTypes.FirstOrDefaultAsync(e => e.Code == "CB.CNV");
            var baoVeType = await context.EmployeeTypes.FirstOrDefaultAsync(e => e.Code == "BV" || e.Name.Contains("Bảo vệ"));
            var tapVuType = await context.EmployeeTypes.FirstOrDefaultAsync(e => e.Code == "TV" || e.Name.Contains("Tạp vụ"));

            context.PayrollPolicies.AddRange(
                new PayrollPolicy
                {
                    Code = "CB_CNV",
                    Name = "Chính sách lương CB.CNV",
                    EmployeeTypeId = cbCnvType?.Id,
                    DivisorParamCode = "NGAY_CONG_CHUAN",
                    IncludeAllowance = true,
                    RoundingRule = "ROUND_DOWN_1000",
                    EffectiveDate = effectiveDate,
                    Priority = 100,
                    Description = "Công thức: (Lương CB + Phụ cấp) / Ngày công chuẩn × Ngày công thực tế",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new PayrollPolicy
                {
                    Code = "BAO_VE",
                    Name = "Chính sách lương Bảo vệ",
                    EmployeeTypeId = baoVeType?.Id,
                    DivisorValue = 26,
                    IncludeAllowance = false,
                    RoundingRule = "ROUND_DOWN_1000",
                    EffectiveDate = effectiveDate,
                    Priority = 90,
                    Description = "Công thức: Lương cơ bản / 26 × Ngày công thực tế",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new PayrollPolicy
                {
                    Code = "TAP_VU",
                    Name = "Chính sách lương Tạp vụ",
                    EmployeeTypeId = tapVuType?.Id,
                    DivisorValue = 26,
                    IncludeAllowance = false,
                    RoundingRule = "ROUND_DOWN_1000",
                    EffectiveDate = effectiveDate,
                    Priority = 80,
                    Description = "Công thức: Lương cơ bản / 26 × Ngày công thực tế",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                }
            );
            await context.SaveChangesAsync();
        }

        // ======== DATA MODELS ========
        private class SeedDataModel
        {
            public string YearMonth { get; set; } = "";
            public string ExtractedAt { get; set; } = "";
            public List<TramDataModel> Trams { get; set; } = new();
            public List<EmployeeTypeDataModel> EmployeeTypes { get; set; } = new();
            public List<TechnicalGradeDataModel> TechnicalGrades { get; set; } = new();
            public List<EmployeeDataModel> Employees { get; set; } = new();
            public List<ProductionDataModel> Productions { get; set; } = new();
            public List<DrcRateDataModel> DrcRates { get; set; } = new();
            public List<RubberUnitPriceDataModel> RubberUnitPrices { get; set; } = new();
            public List<ExchangeRateDataModel> ExchangeRates { get; set; } = new();
            public List<WorkTypeDataModel> WorkTypes { get; set; } = new();
            public List<SystemParameterDataModel> SystemParameters { get; set; } = new();
        }

        private class TramDataModel { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public string Description { get; set; } = ""; }
        private class EmployeeTypeDataModel { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public string SalaryCurrency { get; set; } = "LAK"; public string PaymentCurrency { get; set; } = "LAK"; public string CalculationMethod { get; set; } = "PRODUCTION"; public bool HasInsurance { get; set; } public int SortOrder { get; set; } }
        private class TechnicalGradeDataModel { public string Grade { get; set; } = ""; public string Name { get; set; } = ""; public decimal PointCoefficient { get; set; } public int SortOrder { get; set; } }
        private class EmployeeDataModel { public string Msnv { get; set; } = ""; public string FullName { get; set; } = ""; public string TramCode { get; set; } = ""; public string Position { get; set; } = ""; public string TechnicalGrade { get; set; } = ""; }
        private class ProductionDataModel { public string EmployeeMsnv { get; set; } = ""; public string YearMonth { get; set; } = ""; public decimal RawLatexKg { get; set; } public decimal DryLatexKg { get; set; } public string Grade { get; set; } = ""; public decimal CalculatedSalary { get; set; } }
        private class RubberUnitPriceDataModel { public string TramCode { get; set; } = ""; public string Grade { get; set; } = ""; public decimal UnitPriceKip { get; set; } public bool IsDifficultArea { get; set; } }
        private class ExchangeRateDataModel { public string YearMonth { get; set; } = ""; public string FromCurrency { get; set; } = ""; public string ToCurrency { get; set; } = ""; public decimal Rate { get; set; } public string Source { get; set; } = ""; }
        private class WorkTypeDataModel { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public decimal UnitPrice { get; set; } public string Currency { get; set; } = ""; }
        private class SystemParameterDataModel { public string ParamCode { get; set; } = ""; public decimal ParamValue { get; set; } public string Description { get; set; } = ""; }
        private class DrcRateDataModel { public string TramCode { get; set; } = ""; public string YearMonth { get; set; } = ""; public decimal DrcRate { get; set; } public string Source { get; set; } = ""; }
    }
}
