using API_Sample.Data.EF;
using API_Sample.Data.Entities;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API_Sample.Data.Seed
{
    public class PayrollSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            var logger = scope.ServiceProvider.GetService<ILogger<PayrollSeeder>>();

            try
            {
                await SeedAccounts(context);
                await SeedTrams(context);
                await SeedPositions(context);
                await SeedSystemParameters(context);
                await SeedSalaryScales(context);
                await SeedCostCenters(context);
                await SeedEmployees(context);
                await SeedAttendances(context);
                await SeedDrcRates(context);
                await SeedProductions(context);
                await SeedTaxBrackets(context);
                await SeedPayrollPolicies(context);

                logger?.LogInformation("PayrollSeeder: Seed data completed successfully!");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "PayrollSeeder: Error seeding data");
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
                    FirstName = "Admin",
                    LastName = "System",
                    AccountType = "ADMIN",
                    Email = "admin@quasa.com",
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
                    FirstName = "Kế",
                    LastName = "Toán",
                    AccountType = "ACCOUNTANT",
                    Email = "ketoan@quasa.com",
                    Phone = "0123456790",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new Account
                {
                    Id = 3,
                    UserName = "hr",
                    Password = BCrypt.Net.BCrypt.HashPassword("Hr@123456"),
                    FirstName = "Nhân",
                    LastName = "Sự",
                    AccountType = "HR",
                    Email = "hr@quasa.com",
                    Phone = "0123456791",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedTrams(MainDbContext context)
        {
            if (await context.Trams.AnyAsync()) return;

            var now = DateTime.UtcNow;
            context.Trams.AddRange(
                new Tram { Code = "T1", Name = "TRẠM 1", Description = "Trạm sản xuất 1", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Tram { Code = "T2", Name = "TRẠM 2", Description = "Trạm sản xuất 2", Status = 1, CreatedAt = now, CreatedBy = 1 }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedPositions(MainDbContext context)
        {
            if (await context.Positions.AnyAsync()) return;

            var now = DateTime.UtcNow;
            context.Positions.AddRange(
                new Position { Code = "CNKT", Name = "Công nhân kỹ thuật", Type = "CNKT", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "NVKT", Name = "Nhân viên kỹ thuật", Type = "NVKT", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "BV", Name = "Bảo vệ", Type = "BV", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "TV", Name = "Tạp vụ", Type = "TV", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new Position { Code = "CB", Name = "Cán bộ", Type = "CB", Status = 1, CreatedAt = now, CreatedBy = 1 }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedSystemParameters(MainDbContext context)
        {
            if (await context.SystemParameters.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 11, 1);

            context.SystemParameters.AddRange(
                new SystemParameter { ParamCode = "P7", ParamName = "Hằng số chia P7", ParamValue = 27.0m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "Dùng trong công thức tính lương chính", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "DRC_T1", ParamName = "Định mức riêng TRẠM 1", ParamValue = 0.3851m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "DRC_T2", ParamName = "Định mức riêng TRẠM 2", ParamValue = 0.3770m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "RATE_THB", ParamName = "Tỷ giá THB", ParamValue = 600m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "RATE_USD", ParamName = "Tỷ giá USD", ParamValue = 640m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "BHXH_RATE", ParamName = "Tỷ lệ BHXH", ParamValue = 0.08m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "8%", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "BHYT_RATE", ParamName = "Tỷ lệ BHYT", ParamValue = 0.015m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "1.5%", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_TH_1", ParamName = "Ngưỡng thuế bậc 1", ParamValue = 5000000m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_TH_2", ParamName = "Ngưỡng thuế bậc 2", ParamValue = 10000000m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_TH_3", ParamName = "Ngưỡng thuế bậc 3", ParamValue = 18000000m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_TH_4", ParamName = "Ngưỡng thuế bậc 4", ParamValue = 32000000m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_TH_5", ParamName = "Ngưỡng thuế bậc 5", ParamValue = 52000000m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_RATE_1", ParamName = "Thuế suất bậc 1", ParamValue = 5m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "5%", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_RATE_2", ParamName = "Thuế suất bậc 2", ParamValue = 10m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "10%", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_RATE_3", ParamName = "Thuế suất bậc 3", ParamValue = 15m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "15%", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_RATE_4", ParamName = "Thuế suất bậc 4", ParamValue = 20m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "20%", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_RATE_5", ParamName = "Thuế suất bậc 5", ParamValue = 25m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "25%", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SystemParameter { ParamCode = "TAX_RATE_6", ParamName = "Thuế suất bậc 6", ParamValue = 35m, DataType = "DECIMAL", EffectiveDate = effectiveDate, Description = "35%", Status = 1, CreatedAt = now, CreatedBy = 1 }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedSalaryScales(MainDbContext context)
        {
            if (await context.SalaryScales.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 11, 1);
            var trams = await context.Trams.ToListAsync();
            var t1 = trams.First(t => t.Code == "T1");
            var t2 = trams.First(t => t.Code == "T2");

            context.SalaryScales.AddRange(
                new SalaryScale { TramId = t1.Id, Grade = "A", Coefficient = 1.0m, BaseRate = 9.2m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t1.Id, Grade = "B", Coefficient = 0.95m, BaseRate = 8.9m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t1.Id, Grade = "C", Coefficient = 0.90m, BaseRate = 8.6m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t1.Id, Grade = "D", Coefficient = 0.85m, BaseRate = 8.3m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t1.Id, Grade = "E", Coefficient = 0.80m, BaseRate = 8.0m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t2.Id, Grade = "A", Coefficient = 1.0m, BaseRate = 7.7m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t2.Id, Grade = "B", Coefficient = 0.95m, BaseRate = 7.4m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t2.Id, Grade = "C", Coefficient = 0.90m, BaseRate = 7.1m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t2.Id, Grade = "D", Coefficient = 0.85m, BaseRate = 6.8m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 },
                new SalaryScale { TramId = t2.Id, Grade = "E", Coefficient = 0.80m, BaseRate = 6.5m, EffectiveDate = effectiveDate, Status = 1, CreatedAt = now, CreatedBy = 1 }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedCostCenters(MainDbContext context)
        {
            if (await context.CostCenters.AnyAsync()) return;

            var now = DateTime.UtcNow;
            context.CostCenters.AddRange(
                new CostCenter { Code = "CC01", Name = "Chi phí nhân công trực tiếp", AllocationRate = 0.40m, AccountingCode = "6221", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC02", Name = "Chi phí quản lý phân xưởng", AllocationRate = 0.15m, AccountingCode = "6271", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC03", Name = "Chi phí bảo trì máy móc", AllocationRate = 0.10m, AccountingCode = "6273", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC04", Name = "Chi phí vật tư tiêu hao", AllocationRate = 0.08m, AccountingCode = "6222", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC05", Name = "Chi phí điện nước", AllocationRate = 0.07m, AccountingCode = "6277", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC06", Name = "Chi phí khấu hao TSCĐ", AllocationRate = 0.05m, AccountingCode = "6274", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC07", Name = "Chi phí bảo hiểm", AllocationRate = 0.05m, AccountingCode = "6278", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC08", Name = "Chi phí đào tạo", AllocationRate = 0.03m, AccountingCode = "6279", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC09", Name = "Chi phí an toàn lao động", AllocationRate = 0.02m, AccountingCode = "6275", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC10", Name = "Chi phí phúc lợi", AllocationRate = 0.02m, AccountingCode = "6276", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC11", Name = "Chi phí văn phòng phẩm", AllocationRate = 0.01m, AccountingCode = "6422", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC12", Name = "Chi phí tiếp khách", AllocationRate = 0.01m, AccountingCode = "6423", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC13", Name = "Chi phí vận chuyển", AllocationRate = 0.005m, AccountingCode = "6417", Status = 1, CreatedAt = now, CreatedBy = 1 },
                new CostCenter { Code = "CC14", Name = "Chi phí khác", AllocationRate = 0.005m, AccountingCode = "6428", Status = 1, CreatedAt = now, CreatedBy = 1 }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedEmployees(MainDbContext context)
        {
            if (await context.Employees.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var trams = await context.Trams.ToListAsync();
            var positions = await context.Positions.ToListAsync();
            var t1 = trams.First(t => t.Code == "T1");
            var t2 = trams.First(t => t.Code == "T2");
            var cnkt = positions.First(p => p.Code == "CNKT");
            var cb = positions.First(p => p.Code == "CB");

            var employees = new List<Employee>();
            var random = new Random(42);
            string[] grades = { "A", "B", "C", "D", "E" };

            for (int i = 1; i <= 39; i++)
            {
                employees.Add(new Employee
                {
                    Msnv = $"T1-{i:D3}",
                    FullName = $"Nhân viên Trạm 1 - {i:D2}",
                    TramId = t1.Id,
                    PositionId = cnkt.Id,
                    TechnicalGrade = grades[random.Next(grades.Length)],
                    HireDate = new DateTime(2020 + random.Next(5), random.Next(1, 13), random.Next(1, 28)),
                    InsuranceIncluded = true,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }

            for (int i = 1; i <= 53; i++)
            {
                employees.Add(new Employee
                {
                    Msnv = $"T2-{i:D3}",
                    FullName = $"Nhân viên Trạm 2 - {i:D2}",
                    TramId = t2.Id,
                    PositionId = cnkt.Id,
                    TechnicalGrade = grades[random.Next(grades.Length)],
                    HireDate = new DateTime(2020 + random.Next(5), random.Next(1, 13), random.Next(1, 28)),
                    InsuranceIncluded = true,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }

            for (int i = 1; i <= 4; i++)
            {
                employees.Add(new Employee
                {
                    Msnv = $"CB-{i:D3}",
                    FullName = $"Cán bộ {i:D2}",
                    TramId = t1.Id,
                    PositionId = cb.Id,
                    TechnicalGrade = "A",
                    HireDate = new DateTime(2018 + random.Next(3), random.Next(1, 13), random.Next(1, 28)),
                    InsuranceIncluded = true,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAttendances(MainDbContext context)
        {
            if (await context.Attendances.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var yearMonth = "2025-11";
            var employees = await context.Employees.ToListAsync();
            var random = new Random(42);

            var attendances = employees.Select(emp => new Attendance
            {
                EmployeeId = emp.Id,
                YearMonth = yearMonth,
                RegularDays = 18 + (decimal)(random.NextDouble() * 6),
                SundayDays = random.Next(0, 5),
                YoungTreeDays = random.Next(0, 4),
                HardshipDays = random.Next(0, 3),
                DoubleCutDays = random.Next(0, 2),
                DoubleCutSunday = random.Next(0, 2),
                CareDays = random.Next(2, 8),
                AbsentDays = random.Next(0, 3),
                SickDays = random.Next(0, 2),
                LeaveDays = random.Next(0, 2),
                TotalDays = 0,
                AttendanceStatus = "APPROVED",
                Status = 1,
                CreatedAt = now,
                CreatedBy = 1
            }).ToList();

            foreach (var att in attendances)
            {
                att.TotalDays = att.RegularDays + att.SundayDays + att.YoungTreeDays + att.HardshipDays
                              + att.DoubleCutDays + att.DoubleCutSunday + att.CareDays;
            }

            context.Attendances.AddRange(attendances);
            await context.SaveChangesAsync();
        }

        private static async Task SeedDrcRates(MainDbContext context)
        {
            if (await context.DrcRates.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var trams = await context.Trams.ToListAsync();

            var drcRates = new List<DrcRate>();

            foreach (var tram in trams)
            {
                drcRates.Add(new DrcRate
                {
                    YearMonth = "2025-11",
                    TramId = tram.Id,
                    DrcRawLatex = tram.Code == "T1" ? 0.3851m : 0.3770m,
                    DrcReference = 0.40m,
                    DrcSerum = 0.45m,
                    DrcRope = 0.42m,
                    Note = $"DRC tháng 11/2025 - {tram.Name}",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                });
            }

            context.DrcRates.AddRange(drcRates);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductions(MainDbContext context)
        {
            if (await context.Productions.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var yearMonth = "2025-11";
            var employees = await context.Employees.Include(e => e.Tram).ToListAsync();
            var drcRates = await context.DrcRates.Where(d => d.YearMonth == yearMonth).ToListAsync();
            var random = new Random(42);

            var grades = new[] { "A", "A", "B", "B", "B", "C", "C", "D" };

            var productions = employees.Select(emp =>
            {
                var drc = drcRates.FirstOrDefault(d => d.TramId == emp.TramId);
                var rawLatex = 50 + (decimal)(random.NextDouble() * 150);
                var ropeLatex = (decimal)(random.NextDouble() * 30);
                var serum = (decimal)(random.NextDouble() * 20);
                var carryOver = random.Next(0, 10) > 7 ? (decimal)(random.NextDouble() * 30) : 0;

                var dryLatex = rawLatex * (drc?.DrcRawLatex ?? 0.38m);
                var carryDry = carryOver * (drc?.DrcReference ?? 0.40m);

                return new Production
                {
                    EmployeeId = emp.Id,
                    YearMonth = yearMonth,
                    RawLatexKg = Math.Round(rawLatex, 2),
                    RopeLatexKg = Math.Round(ropeLatex, 2),
                    SerumKg = Math.Round(serum, 2),
                    CarryOverKg = Math.Round(carryOver, 2),
                    DrcRaw = drc?.DrcRawLatex,
                    DrcSerum = drc?.DrcSerum,
                    DryLatexKg = Math.Round(dryLatex, 2),
                    CarryDryKg = Math.Round(carryDry, 2),
                    TotalPayKg = Math.Round(dryLatex + carryDry, 2),
                    TechGrade = grades[random.Next(grades.Length)],
                    Note = null,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                };
            }).ToList();

            context.Productions.AddRange(productions);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed biểu thuế TNCN theo bậc lũy tiến (Lào)
        /// Đây là dữ liệu tham khảo, công ty có thể chỉnh sửa theo nhu cầu
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
                    ToAmount = 1300000,
                    TaxRate = 0,
                    AdditionalAmount = 0,
                    SortOrder = 1,
                    EffectiveDate = effectiveDate,
                    Description = "Bậc 1: Thu nhập từ 0 - 1,300,000 KIP không chịu thuế",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 1300000,
                    ToAmount = 5000000,
                    TaxRate = 0.05m,
                    AdditionalAmount = 0,
                    SortOrder = 2,
                    EffectiveDate = effectiveDate,
                    Description = "Bậc 2: 5% cho phần từ 1,300,000 - 5,000,000 KIP",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 5000000,
                    ToAmount = 15000000,
                    TaxRate = 0.10m,
                    AdditionalAmount = 185000,
                    SortOrder = 3,
                    EffectiveDate = effectiveDate,
                    Description = "Bậc 3: 10% cho phần từ 5,000,000 - 15,000,000 KIP + 185,000",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 15000000,
                    ToAmount = 25000000,
                    TaxRate = 0.15m,
                    AdditionalAmount = 1185000,
                    SortOrder = 4,
                    EffectiveDate = effectiveDate,
                    Description = "Bậc 4: 15% cho phần từ 15,000,000 - 25,000,000 KIP + 1,185,000",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 25000000,
                    ToAmount = 65000000,
                    TaxRate = 0.20m,
                    AdditionalAmount = 2685000,
                    SortOrder = 5,
                    EffectiveDate = effectiveDate,
                    Description = "Bậc 5: 20% cho phần từ 25,000,000 - 65,000,000 KIP + 2,685,000",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new TaxBracket
                {
                    FromAmount = 65000000,
                    ToAmount = null, // Bậc cuối không giới hạn
                    TaxRate = 0.25m,
                    AdditionalAmount = 10685000,
                    SortOrder = 6,
                    EffectiveDate = effectiveDate,
                    Description = "Bậc 6: 25% cho phần trên 65,000,000 KIP + 10,685,000",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                }
            );
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed chính sách tính lương cho các nhóm FIXED (CB.CNV, Bảo vệ, Tạp vụ)
        /// Đây là dữ liệu tham khảo, công ty có thể chỉnh sửa theo nhu cầu
        /// </summary>
        private static async Task SeedPayrollPolicies(MainDbContext context)
        {
            if (await context.PayrollPolicies.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var effectiveDate = new DateTime(2025, 1, 1);

            // Lấy EmployeeType FIXED
            var fixedEmployeeType = await context.EmployeeTypes
                .FirstOrDefaultAsync(e => e.CalculationMethod == "FIXED" && e.Status == 1);

            if (fixedEmployeeType == null) return;

            context.PayrollPolicies.AddRange(
                new PayrollPolicy
                {
                    Code = "CB_CNV",
                    Name = "Cán bộ công nhân viên người Lào",
                    EmployeeTypeId = fixedEmployeeType.Id,
                    TramId = null,
                    PositionId = null,
                    DivisorValue = null,
                    DivisorParamCode = "P7",
                    IncludeAllowance = true,
                    RoundingRule = "ROUND_DOWN_1000",
                    EffectiveDate = effectiveDate,
                    EndDate = null,
                    Priority = 10,
                    Description = "CB.CNV: (BaseSalary + Allowance) / P7 * WorkingDays. Chịu thuế TNCN.",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new PayrollPolicy
                {
                    Code = "BAO_VE",
                    Name = "Bảo vệ",
                    EmployeeTypeId = fixedEmployeeType.Id,
                    TramId = null,
                    PositionId = null,
                    DivisorValue = 30,
                    DivisorParamCode = null,
                    IncludeAllowance = true,
                    RoundingRule = "ROUND_DOWN_1000",
                    EffectiveDate = effectiveDate,
                    EndDate = null,
                    Priority = 20,
                    Description = "Bảo vệ: (BaseSalary + Allowance) / 30 * WorkingDays. Không chịu thuế.",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                },
                new PayrollPolicy
                {
                    Code = "TAP_VU",
                    Name = "Tạp vụ",
                    EmployeeTypeId = fixedEmployeeType.Id,
                    TramId = null,
                    PositionId = null,
                    DivisorValue = 30,
                    DivisorParamCode = null,
                    IncludeAllowance = false,
                    RoundingRule = "ROUND_DOWN_1000",
                    EffectiveDate = effectiveDate,
                    EndDate = null,
                    Priority = 30,
                    Description = "Tạp vụ: BaseSalary / 30 * WorkingDays. Không phụ cấp, không thuế.",
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = 1
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
