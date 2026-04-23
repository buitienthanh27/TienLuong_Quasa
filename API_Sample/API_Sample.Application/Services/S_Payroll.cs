using API_Sample.Application.Ultilities;
using API_Sample.Data.EF;
using API_Sample.Data.Entities;
using API_Sample.Models.Common;
using API_Sample.Models.Request;
using API_Sample.Models.Response;
using API_Sample.Utilities;
using API_Sample.Utilities.Constants;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace API_Sample.Application.Services
{
    public interface IS_Payroll
    {
        Task<ResponseData<MRes_PayrollCalculateSummary>> Calculate(MReq_PayrollCalculate request);
        Task<ResponseData<MRes_PayrollResult>> Recalculate(int payrollId, int calculatedBy);
        Task<ResponseData<MRes_PayrollResult>> GetDetailById(int payrollId);
        Task<ResponseData<List<MRes_Payroll>>> GetListByPaging(MReq_Payroll_FullParam request);
        Task<ResponseData<List<MRes_Payroll>>> GetListByYearMonth(string yearMonth);
        Task<ResponseData<MRes_Payroll>> UpdatePayrollStatus(int id, string payrollStatus, int updatedBy);
        Task<ResponseData<int>> UpdatePayrollStatusBulk(List<int> ids, string payrollStatus, int updatedBy);
    }

    /// <summary>
    /// Service tính lương công nhân cao su
    /// Công thức CNKT: Lương = Sản lượng quy khô (kg) × Đơn giá theo hạng (KIP/kg)
    /// </summary>
    public class S_Payroll : BaseService<S_Payroll>, IS_Payroll
    {
        private readonly IMapper _mapper;

        public S_Payroll(MainDbContext context, IMapper mapper, ILogger<S_Payroll> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tính lương cho tất cả nhân viên hoặc danh sách chỉ định trong tháng
        /// Công thức: Lương = Sản lượng quy khô × Đơn giá theo hạng + Phụ cấp vùng khó
        /// </summary>
        public async Task<ResponseData<MRes_PayrollCalculateSummary>> Calculate(MReq_PayrollCalculate request)
        {
            try
            {
                var summary = new MRes_PayrollCalculateSummary
                {
                    YearMonth = request.YearMonth,
                    Errors = new List<string>()
                };

                var monthEnd = GetMonthEndDate(request.YearMonth);

                // 1. Lấy danh sách nhân viên
                var employeesQuery = _context.Employees
                    .Include(e => e.Tram)
                    .Include(e => e.EmployeeType)
                    .Where(e => e.Status != -1);

                if (request.EmployeeIds != null && request.EmployeeIds.Any())
                    employeesQuery = employeesQuery.Where(e => request.EmployeeIds.Contains(e.Id));

                var employees = await employeesQuery.ToListAsync();
                summary.TotalEmployees = employees.Count;

                // 2. Lấy sản lượng tháng này
                var productions = await _context.Productions.AsNoTracking()
                    .Where(p => p.YearMonth == request.YearMonth && p.Status != -1)
                    .ToListAsync();

                // 3. Lấy đơn giá mủ theo hạng (đã convert sang KIP)
                var unitPrices = await _context.RubberUnitPrices.AsNoTracking()
                    .Where(r => r.Status != -1 && r.EffectiveDate <= monthEnd)
                    .OrderByDescending(r => r.EffectiveDate)
                    .ToListAsync();

                // 4. Lấy hệ số hạng kỹ thuật từ DB
                var techGrades = await _context.TechnicalGrades.AsNoTracking()
                    .Where(t => t.Status != -1)
                    .ToDictionaryAsync(t => t.Grade, t => t.PointCoefficient);

                // 5. Lấy WorkTypes cho lương chăm sóc
                var workTypes = await _context.WorkTypes.AsNoTracking()
                    .Where(w => w.Status != -1)
                    .ToDictionaryAsync(w => w.Code, w => w.UnitPrice);

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var now = DateTime.UtcNow;

                    foreach (var emp in employees)
                    {
                        try
                        {
                            var result = await CalculateEmployeePayroll(
                                emp, request.YearMonth, productions, unitPrices,
                                techGrades, workTypes, request.CalculatedBy, now);

                            if (result != null)
                            {
                                summary.SuccessCount++;
                                summary.TotalGrossSalary += result.GrossSalary;
                                summary.TotalNetSalary += result.NetSalary;
                                summary.TotalDeductions += result.TotalDeductions;
                                summary.TotalAllowances += result.TotalAllowances;
                            }
                            else
                            {
                                summary.FailedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            summary.Errors.Add($"NV {emp.Msnv}: {ex.Message}");
                            summary.FailedCount++;
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Payroll.Calculate: {YearMonth} - Success={Success}, Failed={Failed}, TotalNet={TotalNet:N0}",
                        request.YearMonth, summary.SuccessCount, summary.FailedCount, summary.TotalNetSalary);

                    return new ResponseData<MRes_PayrollCalculateSummary>(1, (int)HttpStatusCode.OK, MessageErrorConstants.PAYROLL_CALCULATE_SUCCESS)
                    {
                        data = summary,
                        data2nd = summary.SuccessCount
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Calculate), request);
            }
        }

        /// <summary>
        /// Tính lương cho 1 nhân viên dựa trên sản lượng mủ
        /// </summary>
        private async Task<Payroll> CalculateEmployeePayroll(
            Employee emp,
            string yearMonth,
            List<Production> productions,
            List<RubberUnitPrice> unitPrices,
            Dictionary<string, decimal> techGrades,
            Dictionary<string, decimal> workTypes,
            int calculatedBy,
            DateTime now)
        {
            var details = new List<PayrollDetail>();

            // Kiểm tra payroll đã tồn tại
            var existingPayroll = await _context.Payrolls
                .Include(p => p.PayrollDetails)
                .FirstOrDefaultAsync(p => p.EmployeeId == emp.Id && p.YearMonth == yearMonth && p.Status != -1);

            if (existingPayroll != null && existingPayroll.PayrollStatus == "LOCKED")
                return null;

            if (existingPayroll != null)
            {
                _context.PayrollDetails.RemoveRange(existingPayroll.PayrollDetails);
            }

            var employeeType = emp.EmployeeType?.CalculationMethod ?? "PRODUCTION";
            decimal grossSalary = 0;
            decimal baseSalary = 0;
            decimal perfCoef = 1.0m;
            decimal workingDays = 0;

            // Phase 1: Xác định loại tính lương
            details.Add(new PayrollDetail
            {
                Phase = 1,
                ItemCode = "EMP_TYPE",
                Description = $"Loại lao động: {emp.EmployeeType?.Name ?? "CNKT"} ({employeeType})",
                Amount = 0,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            switch (employeeType)
            {
                case "PRODUCTION":
                    // Công nhân kỹ thuật - tính theo sản lượng mủ
                    (grossSalary, baseSalary, perfCoef) = CalculateProductionSalary(
                        emp, yearMonth, productions, unitPrices, techGrades, details, now, calculatedBy);
                    break;

                case "FIXED":
                    // BV/TV/CB - lương cố định (config-driven)
                    var (fixedSalary, fixedError) = await CalculateFixedSalaryAsync(emp, yearMonth, details, now, calculatedBy);
                    if (fixedError != null)
                    {
                        _logger.LogWarning("CalculateFixedSalary failed for {Msnv}: {Error}", emp.Msnv, fixedError);
                        return null;
                    }
                    grossSalary = fixedSalary;
                    baseSalary = grossSalary;
                    break;

                case "DAILY":
                    // Chăm sóc - tính theo ngày công (config-driven)
                    var (dailySalary, careDays, dailyError) = await CalculateDailySalaryAsync(emp, yearMonth, details, now, calculatedBy);
                    if (dailyError != null)
                    {
                        _logger.LogWarning("CalculateDailySalary failed for {Msnv}: {Error}", emp.Msnv, dailyError);
                        return null;
                    }
                    grossSalary = dailySalary;
                    workingDays = careDays;
                    baseSalary = grossSalary;
                    break;
            }

            // Phase 4: Khấu trừ (BHXH, BHYT, Thuế TNCN - config-driven)
            var (bhxh, bhyt, tax, totalDeductions, deductionError) = await CalculateDeductionsAsync(emp, grossSalary, yearMonth, details, now, calculatedBy);
            if (deductionError != null)
            {
                _logger.LogWarning("CalculateDeductions failed for {Msnv}: {Error}", emp.Msnv, deductionError);
                return null;
            }

            // Phụ cấp (nếu có)
            var allowances = await _context.Allowances.AsNoTracking()
                .Where(a => a.EmployeeId == emp.Id && a.YearMonth == yearMonth && a.Status != -1)
                .ToListAsync();
            var totalAllowances = allowances.Sum(a => a.CalculatedAmount);

            // Phase 5: Thực lĩnh - làm tròn xuống tới nghìn
            var netSalary = Utilities.Utilities.RoundDownToThousand(grossSalary + totalAllowances - totalDeductions);

            details.Add(new PayrollDetail
            {
                Phase = 5,
                ItemCode = "NET_SALARY",
                Description = $"Thực lĩnh: {grossSalary:N0} + {totalAllowances:N0} - {totalDeductions:N0}",
                Amount = netSalary,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // Lưu hoặc cập nhật Payroll
            if (existingPayroll != null)
            {
                existingPayroll.BaseSalary = baseSalary;
                existingPayroll.PerformanceCoef = perfCoef;
                existingPayroll.WorkingDays = workingDays;
                existingPayroll.GrossSalary = grossSalary;
                existingPayroll.Bhxh = bhxh;
                existingPayroll.Bhyt = bhyt;
                existingPayroll.IncomeTax = tax;
                existingPayroll.TotalDeductions = totalDeductions;
                existingPayroll.TotalAllowances = totalAllowances;
                existingPayroll.NetSalary = netSalary;
                existingPayroll.PayrollStatus = "DRAFT";
                existingPayroll.CalculatedAt = now;
                existingPayroll.CalculatedBy = calculatedBy;
                existingPayroll.UpdatedAt = now;
                existingPayroll.UpdatedBy = calculatedBy;

                foreach (var detail in details)
                {
                    detail.PayrollId = existingPayroll.Id;
                    _context.PayrollDetails.Add(detail);
                }

                return existingPayroll;
            }
            else
            {
                var payroll = new Payroll
                {
                    EmployeeId = emp.Id,
                    YearMonth = yearMonth,
                    BaseSalary = baseSalary,
                    PerformanceCoef = perfCoef,
                    WorkingDays = workingDays,
                    GrossSalary = grossSalary,
                    Bhxh = bhxh,
                    Bhyt = bhyt,
                    IncomeTax = tax,
                    TotalDeductions = totalDeductions,
                    TotalAllowances = totalAllowances,
                    NetSalary = netSalary,
                    PayrollStatus = "DRAFT",
                    CalculatedAt = now,
                    CalculatedBy = calculatedBy,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                };

                _context.Payrolls.Add(payroll);
                await _context.SaveChangesAsync();

                foreach (var detail in details)
                {
                    detail.PayrollId = payroll.Id;
                    _context.PayrollDetails.Add(detail);
                }

                return payroll;
            }
        }

        /// <summary>
        /// Tính lương theo sản lượng (CNKT)
        /// Công thức: Lương = Sản lượng quy khô × Đơn giá theo hạng + Bonus vùng khó
        /// </summary>
        private (decimal grossSalary, decimal baseSalary, decimal perfCoef) CalculateProductionSalary(
            Employee emp,
            string yearMonth,
            List<Production> productions,
            List<RubberUnitPrice> unitPrices,
            Dictionary<string, decimal> techGrades,
            List<PayrollDetail> details,
            DateTime now,
            int calculatedBy)
        {
            // Lấy sản lượng
            var production = productions.FirstOrDefault(p => p.EmployeeId == emp.Id);
            if (production == null)
            {
                details.Add(new PayrollDetail
                {
                    Phase = 2,
                    ItemCode = "NO_PRODUCTION",
                    Description = "Không có sản lượng trong tháng",
                    Amount = 0,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
                return (0, 0, 1.0m);
            }

            // Sản lượng quy khô - cắt 2 số thập phân (không làm tròn)
            var dryLatexKg = Utilities.Utilities.Truncate2Decimals(production.TotalPayKg);
            // Mủ tươi không có số lẻ
            var rawLatexKg = Utilities.Utilities.TruncateRawLatex(production.RawLatexKg);
            var grade = production.TechGrade ?? emp.TechnicalGrade ?? "B";

            details.Add(new PayrollDetail
            {
                Phase = 2,
                ItemCode = "PRODUCTION_KG",
                Description = $"Sản lượng quy khô: {dryLatexKg:F2} kg (Mủ tạp: {rawLatexKg:F0} kg)",
                Amount = dryLatexKg,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // Hệ số hạng từ DB
            var gradeCoef = techGrades.GetValueOrDefault(grade, 1.0m);

            details.Add(new PayrollDetail
            {
                Phase = 2,
                ItemCode = "GRADE_COEF",
                Description = $"Hạng kỹ thuật: {grade} (hệ số {gradeCoef:N2})",
                Amount = gradeCoef,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // Lấy đơn giá theo trạm + hạng
            var unitPrice = unitPrices
                .Where(u => u.TramId == emp.TramId && u.Grade == grade)
                .OrderByDescending(u => u.EffectiveDate)
                .FirstOrDefault();

            if (unitPrice == null)
            {
                details.Add(new PayrollDetail
                {
                    Phase = 3,
                    ItemCode = "NO_UNIT_PRICE",
                    Description = $"Không tìm thấy đơn giá cho Trạm {emp.Tram?.Code}, Hạng {grade}",
                    Amount = 0,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
                return (0, 0, gradeCoef);
            }

            // Đơn giá (KIP/kg)
            var pricePerKg = unitPrice.UnitPrice;

            details.Add(new PayrollDetail
            {
                Phase = 3,
                ItemCode = "UNIT_PRICE",
                Description = $"Đơn giá: {pricePerKg:N2} {unitPrice.Currency}/kg",
                Amount = pricePerKg,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // Tính lương cơ bản = Sản lượng × Đơn giá (cắt 2 số thập phân)
            var baseSalary = Utilities.Utilities.Truncate2Decimals(dryLatexKg * pricePerKg);

            details.Add(new PayrollDetail
            {
                Phase = 3,
                ItemCode = "BASE_SALARY",
                Description = $"Lương sản lượng: {dryLatexKg:F2} × {pricePerKg:F2}",
                Amount = baseSalary,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // Bonus vùng khó khăn (đã tích hợp trong đơn giá từ seed)
            var difficultBonus = 0m;
            if (emp.IsDifficultArea && unitPrice.DifficultAreaBonus.HasValue && unitPrice.DifficultAreaBonus.Value > 0)
            {
                difficultBonus = Utilities.Utilities.Truncate2Decimals(unitPrice.DifficultAreaBonus.Value * dryLatexKg);
                details.Add(new PayrollDetail
                {
                    Phase = 3,
                    ItemCode = "DIFFICULT_BONUS",
                    Description = $"Phụ cấp vùng khó: {unitPrice.DifficultAreaBonus:F0} × {dryLatexKg:F2} kg",
                    Amount = difficultBonus,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
            }

            // Tổng lương làm tròn xuống tới nghìn (hàng trăm, chục, đơn vị = 000)
            var grossSalary = Utilities.Utilities.RoundDownToThousand(baseSalary + difficultBonus);

            details.Add(new PayrollDetail
            {
                Phase = 3,
                ItemCode = "GROSS_SALARY",
                Description = "Tổng lương trước khấu trừ",
                Amount = grossSalary,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            return (grossSalary, baseSalary, gradeCoef);
        }

        /// <summary>
        /// Tính lương cố định (BV/TV/CB) - Config-driven từ PayrollPolicy và SalaryScale
        /// Công thức: (BaseSalary + Allowance?) / Divisor * WorkingDays
        /// </summary>
        private async Task<(decimal grossSalary, string? error)> CalculateFixedSalaryAsync(
            Employee emp,
            string yearMonth,
            List<PayrollDetail> details,
            DateTime now,
            int calculatedBy)
        {
            var payrollDate = GetMonthEndDate(yearMonth);

            // 1. Lấy PayrollPolicy
            var policy = await GetPayrollPolicy(emp, payrollDate);
            if (policy == null)
            {
                var errorMsg = $"Không tìm thấy PayrollPolicy cho EmployeeType={emp.EmployeeTypeId}, Tram={emp.TramId}";
                details.Add(new PayrollDetail
                {
                    Phase = 2,
                    ItemCode = "POLICY_ERROR",
                    Description = errorMsg,
                    Amount = 0,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
                return (0, errorMsg);
            }

            details.Add(new PayrollDetail
            {
                Phase = 2,
                ItemCode = "POLICY",
                Description = $"Áp dụng policy: {policy.Code} - {policy.Name}",
                Amount = 0,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // 2. Lấy Divisor
            var (divisor, divisorError) = await GetDivisor(policy, payrollDate);
            if (divisorError != null)
            {
                details.Add(new PayrollDetail
                {
                    Phase = 2,
                    ItemCode = "DIVISOR_ERROR",
                    Description = divisorError,
                    Amount = 0,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
                return (0, divisorError);
            }

            details.Add(new PayrollDetail
            {
                Phase = 2,
                ItemCode = "DIVISOR",
                Description = $"Mẫu số ngày công: {divisor.Value}",
                Amount = divisor.Value,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // 3. Lấy Base Salary
            var (baseSalary, salaryError) = await GetBaseSalary(emp, payrollDate);
            if (salaryError != null)
            {
                details.Add(new PayrollDetail
                {
                    Phase = 2,
                    ItemCode = "SALARY_ERROR",
                    Description = salaryError,
                    Amount = 0,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
                return (0, salaryError);
            }

            details.Add(new PayrollDetail
            {
                Phase = 2,
                ItemCode = "BASE_SALARY",
                Description = $"Lương cơ bản: {baseSalary.Value:N0} KIP",
                Amount = baseSalary.Value,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // 4. Lấy phụ cấp (nếu policy cho phép)
            decimal allowance = 0;
            if (policy.IncludeAllowance)
            {
                allowance = await _context.Allowances
                    .AsNoTracking()
                    .Where(x => x.EmployeeId == emp.Id
                                && x.YearMonth == yearMonth
                                && x.Status == 1)
                    .SumAsync(x => x.CalculatedAmount);

                if (allowance > 0)
                {
                    details.Add(new PayrollDetail
                    {
                        Phase = 2,
                        ItemCode = "ALLOWANCE",
                        Description = $"Phụ cấp tháng: {allowance:N0} KIP",
                        Amount = allowance,
                        Status = 1,
                        CreatedAt = now,
                        CreatedBy = calculatedBy
                    });
                }
            }

            // 5. Lấy ngày công từ Attendance
            var attendance = await _context.Attendances.AsNoTracking()
                .Where(a => a.EmployeeId == emp.Id && a.YearMonth == yearMonth && a.Status != -1)
                .FirstOrDefaultAsync();

            decimal workingDays = (attendance?.RegularDays ?? 0) + (attendance?.SundayDays ?? 0);

            details.Add(new PayrollDetail
            {
                Phase = 2,
                ItemCode = "WORKING_DAYS",
                Description = $"Ngày công: {workingDays} (Regular={attendance?.RegularDays ?? 0}, Sunday={attendance?.SundayDays ?? 0})",
                Amount = workingDays,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            // 6. Tính lương: (BaseSalary + Allowance) / Divisor * WorkingDays
            decimal grossSalary = (baseSalary.Value + allowance) / divisor.Value * workingDays;

            // 7. Làm tròn theo RoundingRule
            grossSalary = ApplyRounding(grossSalary, policy.RoundingRule);

            details.Add(new PayrollDetail
            {
                Phase = 3,
                ItemCode = "FIXED_SALARY",
                Description = $"Lương FIXED: ({baseSalary.Value:N0} + {allowance:N0}) / {divisor.Value} × {workingDays}",
                Amount = grossSalary,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            return (grossSalary, null);
        }

        /// <summary>
        /// Áp dụng quy tắc làm tròn theo RoundingRule từ PayrollPolicy
        /// </summary>
        private decimal ApplyRounding(decimal value, string roundingRule)
        {
            return roundingRule switch
            {
                "ROUND_DOWN_1000" => Math.Floor(value / 1000) * 1000,
                "ROUND_UP_1000" => Math.Ceiling(value / 1000) * 1000,
                "ROUND_NEAREST_1000" => Math.Round(value / 1000) * 1000,
                "NO_ROUNDING" => value,
                _ => Math.Floor(value / 1000) * 1000
            };
        }

        /// <summary>
        /// Tính lương theo ngày công (Chăm sóc) - Config-driven từ WorkType
        /// </summary>
        private async Task<(decimal grossSalary, decimal workingDays, string? error)> CalculateDailySalaryAsync(
            Employee emp,
            string yearMonth,
            List<PayrollDetail> details,
            DateTime now,
            int calculatedBy)
        {
            var payrollDate = GetMonthEndDate(yearMonth);

            // Lấy đơn giá chăm sóc từ WorkType (không dùng fallback)
            var workType = await _context.WorkTypes
                .AsNoTracking()
                .Where(x => x.Code == "CHAM_SOC"
                            && x.Status == 1
                            && x.EffectiveDate <= payrollDate)
                .OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefaultAsync();

            if (workType == null)
            {
                var errorMsg = "Không tìm thấy WorkType 'CHAM_SOC' hiệu lực";
                details.Add(new PayrollDetail
                {
                    Phase = 2,
                    ItemCode = "WORKTYPE_ERROR",
                    Description = errorMsg,
                    Amount = 0,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
                return (0, 0, errorMsg);
            }

            var careRate = workType.UnitPrice;

            var attendance = await _context.Attendances.AsNoTracking()
                .Where(a => a.EmployeeId == emp.Id && a.YearMonth == yearMonth && a.Status != -1)
                .FirstOrDefaultAsync();

            var careDays = attendance?.CareDays ?? 0;
            var dailySalary = Utilities.Utilities.RoundDownToThousand(careDays * careRate);

            details.Add(new PayrollDetail
            {
                Phase = 2,
                ItemCode = "DAILY_SALARY",
                Description = $"Lương chăm sóc: {careDays} ngày × {careRate:N0} KIP (từ WorkType)",
                Amount = dailySalary,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            return (dailySalary, careDays, null);
        }

        /// <summary>
        /// Tính khấu trừ BHXH, BHYT, Thuế TNCN - Config-driven từ SystemParameter và TaxBracket
        /// </summary>
        private async Task<(decimal bhxh, decimal bhyt, decimal tax, decimal totalDeductions, string? error)> CalculateDeductionsAsync(
            Employee emp,
            decimal grossSalary,
            string yearMonth,
            List<PayrollDetail> details,
            DateTime now,
            int calculatedBy)
        {
            decimal bhxh = 0, bhyt = 0, tax = 0;
            var payrollDate = GetMonthEndDate(yearMonth);

            // 1. BHXH/BHYT - đọc từ SystemParameter
            if (emp.InsuranceIncluded)
            {
                var (bhxhRate, bhxhFound) = await GetSystemParameter("BHXH_RATE", payrollDate);
                var (bhytRate, bhytFound) = await GetSystemParameter("BHYT_RATE", payrollDate);

                if (!bhxhFound)
                {
                    var errorMsg = "Thiếu config BHXH_RATE trong SystemParameter";
                    details.Add(new PayrollDetail
                    {
                        Phase = 4,
                        ItemCode = "CONFIG_ERROR",
                        Description = errorMsg,
                        Amount = 0,
                        Status = 1,
                        CreatedAt = now,
                        CreatedBy = calculatedBy
                    });
                    return (0, 0, 0, 0, errorMsg);
                }

                if (!bhytFound)
                {
                    var errorMsg = "Thiếu config BHYT_RATE trong SystemParameter";
                    details.Add(new PayrollDetail
                    {
                        Phase = 4,
                        ItemCode = "CONFIG_ERROR",
                        Description = errorMsg,
                        Amount = 0,
                        Status = 1,
                        CreatedAt = now,
                        CreatedBy = calculatedBy
                    });
                    return (0, 0, 0, 0, errorMsg);
                }

                bhxh = Utilities.Utilities.RoundDownToThousand(grossSalary * bhxhRate);
                bhyt = Utilities.Utilities.RoundDownToThousand(grossSalary * bhytRate);

                details.Add(new PayrollDetail
                {
                    Phase = 4,
                    ItemCode = "BHXH",
                    Description = $"BHXH {bhxhRate * 100:N1}%",
                    Amount = bhxh,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });

                details.Add(new PayrollDetail
                {
                    Phase = 4,
                    ItemCode = "BHYT",
                    Description = $"BHYT {bhytRate * 100:N1}%",
                    Amount = bhyt,
                    Status = 1,
                    CreatedAt = now,
                    CreatedBy = calculatedBy
                });
            }

            // 2. Thuế TNCN - chỉ tính nếu Employee.IsTaxable = true
            if (emp.IsTaxable)
            {
                var (taxValue, taxError) = await CalculateIncomeTax(grossSalary, payrollDate);
                if (taxError != null)
                {
                    details.Add(new PayrollDetail
                    {
                        Phase = 4,
                        ItemCode = "TAX_ERROR",
                        Description = taxError,
                        Amount = 0,
                        Status = 1,
                        CreatedAt = now,
                        CreatedBy = calculatedBy
                    });
                    return (0, 0, 0, 0, taxError);
                }

                tax = Utilities.Utilities.RoundDownToThousand(taxValue);

                if (tax > 0)
                {
                    details.Add(new PayrollDetail
                    {
                        Phase = 4,
                        ItemCode = "INCOME_TAX",
                        Description = $"Thuế TNCN lũy tiến",
                        Amount = tax,
                        Status = 1,
                        CreatedAt = now,
                        CreatedBy = calculatedBy
                    });
                }
            }

            var totalDeductions = Utilities.Utilities.RoundDownToThousand(bhxh + bhyt + tax);

            details.Add(new PayrollDetail
            {
                Phase = 4,
                ItemCode = "TOTAL_DEDUCTIONS",
                Description = $"Tổng khấu trừ: BHXH={bhxh:N0} + BHYT={bhyt:N0} + Tax={tax:N0}",
                Amount = totalDeductions,
                Status = 1,
                CreatedAt = now,
                CreatedBy = calculatedBy
            });

            return (bhxh, bhyt, tax, totalDeductions, null);
        }

        /// <summary>
        /// Tính lại lương cho 1 nhân viên
        /// </summary>
        public async Task<ResponseData<MRes_PayrollResult>> Recalculate(int payrollId, int calculatedBy)
        {
            try
            {
                var payroll = await _context.Payrolls
                    .Include(p => p.Employee).ThenInclude(e => e.Tram)
                    .Include(p => p.Employee).ThenInclude(e => e.EmployeeType)
                    .FirstOrDefaultAsync(p => p.Id == payrollId);

                if (payroll == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (payroll.PayrollStatus == "LOCKED")
                    return Error(HttpStatusCode.BadRequest, MessageErrorConstants.PAYROLL_ALREADY_LOCKED);

                // Gọi Calculate cho 1 nhân viên
                var request = new MReq_PayrollCalculate
                {
                    YearMonth = payroll.YearMonth,
                    EmployeeIds = new List<int> { payroll.EmployeeId },
                    CalculatedBy = calculatedBy
                };

                var calcResult = await Calculate(request);

                if (calcResult.result != 1)
                    return Error(HttpStatusCode.BadRequest, calcResult.error?.message ?? "Lỗi tính lương");

                // Lấy payroll mới (có thể đã đổi id nếu bị xóa tạo lại)
                var newPayroll = await _context.Payrolls
                    .Where(p => p.EmployeeId == payroll.EmployeeId && p.YearMonth == payroll.YearMonth && p.Status != -1)
                    .FirstOrDefaultAsync();

                return await GetDetailById(newPayroll?.Id ?? payrollId);
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Recalculate), new { payrollId, calculatedBy });
            }
        }

        /// <summary>
        /// Lấy chi tiết bảng lương theo ID bao gồm các bước tính và phân bổ chi phí
        /// </summary>
        public async Task<ResponseData<MRes_PayrollResult>> GetDetailById(int payrollId)
        {
            try
            {
                var payroll = await _context.Payrolls.AsNoTracking()
                    .Include(p => p.Employee).ThenInclude(e => e.Tram)
                    .Include(p => p.PayrollDetails)
                    .Include(p => p.CostAllocations).ThenInclude(c => c.CostCenter)
                    .FirstOrDefaultAsync(p => p.Id == payrollId);

                if (payroll == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                var result = new MRes_PayrollResult
                {
                    Payroll = _mapper.Map<MRes_Payroll>(payroll),
                    Details = payroll.PayrollDetails.OrderBy(d => d.Phase).ThenBy(d => d.Id)
                        .Select(d => _mapper.Map<MRes_PayrollDetail>(d)).ToList(),
                    CostAllocations = payroll.CostAllocations
                        .Select(c => _mapper.Map<MRes_CostAllocation>(c)).ToList()
                };

                return new ResponseData<MRes_PayrollResult>
                {
                    data = result,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetDetailById), new { payrollId });
            }
        }

        /// <summary>
        /// Lấy danh sách bảng lương có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_Payroll>>> GetListByPaging(MReq_Payroll_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Payroll> data = new List<MRes_Payroll>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Employee.Msnv)
                        .ProjectTo<MRes_Payroll>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_Payroll>>
                {
                    data = data,
                    data2nd = count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByPaging), request);
            }
        }

        /// <summary>
        /// Lấy danh sách bảng lương theo tháng
        /// </summary>
        public async Task<ResponseData<List<MRes_Payroll>>> GetListByYearMonth(string yearMonth)
        {
            try
            {
                // KHÔNG dùng Include() với ProjectTo - gây lỗi SQL Server CTE
                var data = await _context.Payrolls.AsNoTracking()
                    .Where(p => p.YearMonth == yearMonth && p.Status != -1)
                    .OrderBy(p => p.Employee.Msnv)
                    .ProjectTo<MRes_Payroll>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Payroll>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByYearMonth), new { yearMonth });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái payroll (DRAFT -> SUBMITTED -> APPROVED -> LOCKED)
        /// </summary>
        public async Task<ResponseData<MRes_Payroll>> UpdatePayrollStatus(int id, string payrollStatus, int updatedBy)
        {
            try
            {
                var validStatuses = new[] { "DRAFT", "SUBMITTED", "APPROVED", "LOCKED" };
                if (!validStatuses.Contains(payrollStatus))
                    return Error(HttpStatusCode.BadRequest, $"Trạng thái không hợp lệ. Cho phép: {string.Join(", ", validStatuses)}");

                var data = await _context.Payrolls.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.PayrollStatus == "LOCKED" && payrollStatus != "LOCKED")
                    return Error(HttpStatusCode.BadRequest, MessageErrorConstants.PAYROLL_ALREADY_LOCKED);

                data.PayrollStatus = payrollStatus;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                await _context.SaveChangesAsync();

                return new ResponseData<MRes_Payroll>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Payroll>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdatePayrollStatus), new { id, payrollStatus, updatedBy });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái payroll hàng loạt (DRAFT -> SUBMITTED -> APPROVED -> LOCKED)
        /// </summary>
        public async Task<ResponseData<int>> UpdatePayrollStatusBulk(List<int> ids, string payrollStatus, int updatedBy)
        {
            try
            {
                var validStatuses = new[] { "DRAFT", "SUBMITTED", "APPROVED", "LOCKED" };
                if (!validStatuses.Contains(payrollStatus))
                    return Error(HttpStatusCode.BadRequest, $"Trạng thái không hợp lệ. Cho phép: {string.Join(", ", validStatuses)}");

                if (ids == null || ids.Count == 0)
                    return Error(HttpStatusCode.BadRequest, "Danh sách ID không hợp lệ");

                var now = DateTime.UtcNow;
                var idsArray = EF.Constant(ids);

                // Kiểm tra payroll đã LOCKED
                var lockedCount = await _context.Payrolls
                    .Where(p => ids.Contains(p.Id) && p.PayrollStatus == "LOCKED")
                    .CountAsync();

                if (lockedCount > 0 && payrollStatus != "LOCKED")
                    return Error(HttpStatusCode.BadRequest, $"Có {lockedCount} bảng lương đã chốt, không thể thay đổi trạng thái");

                var updatedCount = await _context.Payrolls
                    .Where(p => idsArray.Contains(p.Id) && p.Status != -1)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.PayrollStatus, payrollStatus)
                        .SetProperty(p => p.UpdatedAt, now)
                        .SetProperty(p => p.UpdatedBy, updatedBy));

                _logger.LogInformation("UpdatePayrollStatusBulk: Updated {Count} payrolls to status {Status}", updatedCount, payrollStatus);

                return new ResponseData<int>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = updatedCount
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdatePayrollStatusBulk), new { ids, payrollStatus, updatedBy });
            }
        }

        #region Helper methods

        private DateTime GetMonthEndDate(string yearMonth)
        {
            var parts = yearMonth.Split('-');
            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);
            return new DateTime(year, month, DateTime.DaysInMonth(year, month));
        }

        /// <summary>
        /// Lấy giá trị SystemParameter theo code và ngày hiệu lực
        /// Trả về (value, found) để phân biệt giá trị 0 vs không tìm thấy
        /// </summary>
        private async Task<(decimal value, bool found)> GetSystemParameter(string paramCode, DateTime payrollDate)
        {
            var param = await _context.SystemParameters
                .AsNoTracking()
                .Where(x => x.ParamCode == paramCode
                            && x.Status == 1
                            && x.EffectiveDate <= payrollDate)
                .OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefaultAsync();

            if (param == null)
                return (0, false);

            return (param.ParamValue, true);
        }

        /// <summary>
        /// Lấy PayrollPolicy phù hợp nhất cho nhân viên theo Priority
        /// </summary>
        private async Task<PayrollPolicy?> GetPayrollPolicy(Employee employee, DateTime payrollDate)
        {
            return await _context.PayrollPolicies
                .AsNoTracking()
                .Where(x => x.Status == 1
                            && x.EffectiveDate <= payrollDate
                            && (x.EndDate == null || x.EndDate >= payrollDate)
                            && (x.EmployeeTypeId == null || x.EmployeeTypeId == employee.EmployeeTypeId)
                            && (x.TramId == null || x.TramId == employee.TramId)
                            && (x.PositionId == null || x.PositionId == employee.PositionId))
                .OrderByDescending(x => x.Priority)
                .ThenByDescending(x => x.EmployeeTypeId.HasValue)
                .ThenByDescending(x => x.TramId.HasValue)
                .ThenByDescending(x => x.PositionId.HasValue)
                .ThenByDescending(x => x.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy divisor từ PayrollPolicy (XOR logic: DivisorValue hoặc DivisorParamCode)
        /// </summary>
        private async Task<(decimal? value, string? error)> GetDivisor(PayrollPolicy policy, DateTime payrollDate)
        {
            if (policy.DivisorValue.HasValue && policy.DivisorValue.Value > 0)
                return (policy.DivisorValue.Value, null);

            if (!string.IsNullOrEmpty(policy.DivisorParamCode))
            {
                var (value, found) = await GetSystemParameter(policy.DivisorParamCode, payrollDate);
                if (!found)
                    return (null, $"SystemParameter '{policy.DivisorParamCode}' không tồn tại hoặc chưa hiệu lực");
                if (value <= 0)
                    return (null, $"SystemParameter '{policy.DivisorParamCode}' có giá trị không hợp lệ: {value}");
                return (value, null);
            }

            return (null, "PayrollPolicy thiếu cả DivisorValue và DivisorParamCode");
        }

        /// <summary>
        /// Lấy base salary từ SalaryScale theo Priority
        /// </summary>
        private async Task<(decimal? value, string? error)> GetBaseSalary(Employee employee, DateTime payrollDate)
        {
            var salaryScale = await _context.SalaryScales
                .AsNoTracking()
                .Where(x => x.Status == 1
                            && x.TramId == employee.TramId
                            && x.EffectiveDate <= payrollDate
                            && (x.EmployeeTypeId == null || x.EmployeeTypeId == employee.EmployeeTypeId)
                            && (x.PositionId == null || x.PositionId == employee.PositionId)
                            && (x.Grade == null || x.Grade == employee.TechnicalGrade))
                .OrderByDescending(x => x.Priority)
                .ThenByDescending(x => x.EmployeeTypeId.HasValue)
                .ThenByDescending(x => x.PositionId.HasValue)
                .ThenByDescending(x => x.Grade != null)
                .ThenByDescending(x => x.EffectiveDate)
                .FirstOrDefaultAsync();

            if (salaryScale == null)
                return (null, $"Không tìm thấy SalaryScale cho Tram={employee.TramId}, EmployeeType={employee.EmployeeTypeId}");

            return (salaryScale.BaseRate, null);
        }

        /// <summary>
        /// Tính thuế TNCN lũy tiến từ TaxBracket
        /// </summary>
        private async Task<(decimal tax, string? error)> CalculateIncomeTax(decimal grossSalary, DateTime payrollDate)
        {
            var brackets = await _context.TaxBrackets
                .AsNoTracking()
                .Where(x => x.Status == 1
                            && x.EffectiveDate <= payrollDate
                            && (x.EndDate == null || x.EndDate >= payrollDate))
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            if (!brackets.Any())
                return (0, "Không tìm thấy TaxBracket hiệu lực");

            if (grossSalary <= brackets.First().FromAmount)
                return (0, null);

            var bracket = brackets.FirstOrDefault(x =>
                grossSalary > x.FromAmount
                && (x.ToAmount == null || grossSalary <= x.ToAmount));

            if (bracket == null)
                return (0, $"Không tìm thấy bậc thuế phù hợp cho Gross={grossSalary:N0}");

            decimal tax = (grossSalary - bracket.FromAmount) * bracket.TaxRate + bracket.AdditionalAmount;
            return (Math.Max(0, tax), null);
        }

        private IQueryable<Payroll> BuildFilterQuery(MReq_Payroll_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.Payrolls.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (!string.IsNullOrWhiteSpace(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

            if (request.TramId.HasValue)
                query = query.Where(x => x.Employee.TramId == request.TramId.Value);

            if (!string.IsNullOrWhiteSpace(request.PayrollStatus))
                query = query.Where(x => x.PayrollStatus == request.PayrollStatus);

            return query;
        }
        #endregion
    }
}
