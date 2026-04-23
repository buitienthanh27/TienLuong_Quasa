using API_Sample.Application.Ultilities;
using API_Sample.Data.EF;
using API_Sample.Data.Entities;
using API_Sample.Models.Common;
using API_Sample.Models.Request;
using API_Sample.Models.Response;
using API_Sample.Utilities.Constants;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace API_Sample.Application.Services
{
    public interface IS_PayrollReconciliation
    {
        Task<ResponseData<MRes_PayrollReconciliation>> GenerateReconciliation(string yearMonth, int? tramId, int createdBy);
        Task<ResponseData<MRes_PayrollReconciliation>> GetByYearMonth(string yearMonth, int? tramId);
        Task<ResponseData<MRes_PayrollReconciliation>> Balance(int id, int balancedBy);
        Task<ResponseData<MRes_PayrollReconciliation>> Lock(int id, int lockedBy);
        Task<ResponseData<MRes_PayrollReconciliation>> Unlock(int id, int unlockedBy);
        Task<ResponseData<List<MRes_PayrollReconciliation>>> GetListByPaging(MReq_PayrollReconciliation_FullParam request);
        Task<ResponseData<MRes_PayrollReconciliation>> GetById(int id);
        Task<ResponseData<MRes_PayrollReconciliation>> Update(MReq_PayrollReconciliation request);
    }

    /// <summary>
    /// Quản lý cân đối quỹ lương - Tổng hợp và chốt chi phí lương theo tháng
    /// </summary>
    public class S_PayrollReconciliation : BaseService<S_PayrollReconciliation>, IS_PayrollReconciliation
    {
        private readonly IMapper _mapper;

        public S_PayrollReconciliation(MainDbContext context, IMapper mapper, ILogger<S_PayrollReconciliation> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo/cập nhật bảng cân đối quỹ lương từ dữ liệu Payroll
        /// </summary>
        public async Task<ResponseData<MRes_PayrollReconciliation>> GenerateReconciliation(string yearMonth, int? tramId, int createdBy)
        {
            try
            {
                // Kiểm tra đã có reconciliation chưa
                var existing = await _context.PayrollReconciliations
                    .FirstOrDefaultAsync(x => x.YearMonth == yearMonth && x.TramId == tramId && x.Status != -1);

                if (existing != null && existing.ReconciliationStatus == "LOCKED")
                    return Error(HttpStatusCode.BadRequest, "Kỳ lương này đã bị khóa, không thể tạo lại cân đối!");

                // Tổng hợp từ Payroll
                var payrollQuery = _context.Payrolls.AsNoTracking()
                    .Include(p => p.Employee)
                    .Where(p => p.YearMonth == yearMonth && p.Status != -1);

                if (tramId.HasValue)
                    payrollQuery = payrollQuery.Where(p => p.Employee.TramId == tramId);

                var payrolls = await payrollQuery.ToListAsync();

                if (!payrolls.Any())
                    return Error(HttpStatusCode.NotFound, $"Không có dữ liệu lương tháng {yearMonth}!");

                var now = DateTime.UtcNow;

                if (existing != null)
                {
                    // Cập nhật
                    existing.TotalEmployees = payrolls.Count;
                    existing.TotalGrossSalary = payrolls.Sum(p => p.GrossSalary);
                    existing.TotalDeductions = payrolls.Sum(p => p.TotalDeductions);
                    existing.TotalNetSalary = payrolls.Sum(p => p.NetSalary);
                    existing.TotalBhxh = payrolls.Sum(p => p.Bhxh);
                    existing.TotalBhyt = payrolls.Sum(p => p.Bhyt);
                    existing.TotalBhxhCompany = payrolls.Sum(p => p.Bhxh * 0.175m); // 17.5% phần công ty
                    existing.TotalBhytCompany = payrolls.Sum(p => p.Bhyt * 2m); // 3% phần công ty (1.5% NV, 3% CTY)
                    existing.TotalTax = payrolls.Sum(p => p.IncomeTax);
                    existing.TotalAllowances = payrolls.Sum(p => p.TotalAllowances);
                    existing.ReconciliationStatus = "DRAFT";
                    existing.UpdatedAt = now;
                    existing.UpdatedBy = createdBy;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("PayrollReconciliation.Generate: Updated YearMonth={YearMonth}, TramId={TramId}, TotalNet={TotalNet:N0}",
                        yearMonth, tramId, existing.TotalNetSalary);

                    return new ResponseData<MRes_PayrollReconciliation>(1, (int)HttpStatusCode.OK, "Đã cập nhật bảng cân đối!")
                    {
                        data = _mapper.Map<MRes_PayrollReconciliation>(existing)
                    };
                }
                else
                {
                    // Tạo mới
                    var data = new PayrollReconciliation
                    {
                        YearMonth = yearMonth,
                        TramId = tramId,
                        TotalEmployees = payrolls.Count,
                        TotalGrossSalary = payrolls.Sum(p => p.GrossSalary),
                        TotalDeductions = payrolls.Sum(p => p.TotalDeductions),
                        TotalNetSalary = payrolls.Sum(p => p.NetSalary),
                        TotalBhxh = payrolls.Sum(p => p.Bhxh),
                        TotalBhyt = payrolls.Sum(p => p.Bhyt),
                        TotalBhxhCompany = payrolls.Sum(p => p.Bhxh * 0.175m),
                        TotalBhytCompany = payrolls.Sum(p => p.Bhyt * 2m),
                        TotalTax = payrolls.Sum(p => p.IncomeTax),
                        TotalAllowances = payrolls.Sum(p => p.TotalAllowances),
                        ReconciliationStatus = "DRAFT",
                        Status = 1,
                        CreatedAt = now,
                        CreatedBy = createdBy
                    };

                    _context.PayrollReconciliations.Add(data);
                    var save = await _context.SaveChangesAsync();
                    if (save == 0)
                        return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                    _logger.LogInformation("PayrollReconciliation.Generate: Created YearMonth={YearMonth}, TramId={TramId}, TotalNet={TotalNet:N0}",
                        yearMonth, tramId, data.TotalNetSalary);

                    return new ResponseData<MRes_PayrollReconciliation>(1, (int)HttpStatusCode.Created, "Đã tạo bảng cân đối!")
                    {
                        data = _mapper.Map<MRes_PayrollReconciliation>(data)
                    };
                }
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GenerateReconciliation), new { yearMonth, tramId, createdBy });
            }
        }

        /// <summary>
        /// Lấy bảng cân đối theo tháng/trạm
        /// </summary>
        public async Task<ResponseData<MRes_PayrollReconciliation>> GetByYearMonth(string yearMonth, int? tramId)
        {
            try
            {
                var data = await _context.PayrollReconciliations
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.YearMonth == yearMonth && x.TramId == tramId && x.Status != -1);

                return new ResponseData<MRes_PayrollReconciliation>
                {
                    data = _mapper.Map<MRes_PayrollReconciliation>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByYearMonth), new { yearMonth, tramId });
            }
        }

        /// <summary>
        /// Xác nhận cân bằng quỹ lương
        /// </summary>
        public async Task<ResponseData<MRes_PayrollReconciliation>> Balance(int id, int balancedBy)
        {
            try
            {
                var data = await _context.PayrollReconciliations.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ReconciliationStatus == "LOCKED")
                    return Error(HttpStatusCode.BadRequest, "Kỳ lương này đã bị khóa!");

                data.ReconciliationStatus = "BALANCED";
                data.BalancedBy = balancedBy;
                data.BalancedAt = DateTime.UtcNow;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = balancedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                _logger.LogInformation("PayrollReconciliation.Balance: Id={Id}, BalancedBy={BalancedBy}", id, balancedBy);

                return new ResponseData<MRes_PayrollReconciliation>(1, (int)HttpStatusCode.OK, "Đã xác nhận cân bằng quỹ lương!")
                {
                    data = _mapper.Map<MRes_PayrollReconciliation>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Balance), new { id, balancedBy });
            }
        }

        /// <summary>
        /// Khóa kỳ lương - Không cho phép sửa đổi
        /// </summary>
        public async Task<ResponseData<MRes_PayrollReconciliation>> Lock(int id, int lockedBy)
        {
            try
            {
                var data = await _context.PayrollReconciliations.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ReconciliationStatus != "BALANCED")
                    return Error(HttpStatusCode.BadRequest, "Chỉ có thể khóa kỳ lương đã cân bằng!");

                data.ReconciliationStatus = "LOCKED";
                data.LockedBy = lockedBy;
                data.LockedAt = DateTime.UtcNow;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = lockedBy;

                // Khóa tất cả payroll trong kỳ
                await _context.Payrolls
                    .Where(p => p.YearMonth == data.YearMonth &&
                               (data.TramId == null || p.Employee.TramId == data.TramId))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.PayrollStatus, "LOCKED")
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)
                        .SetProperty(p => p.UpdatedBy, lockedBy));

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                _logger.LogInformation("PayrollReconciliation.Lock: Id={Id}, LockedBy={LockedBy}, YearMonth={YearMonth}",
                    id, lockedBy, data.YearMonth);

                return new ResponseData<MRes_PayrollReconciliation>(1, (int)HttpStatusCode.OK, "Đã khóa kỳ lương!")
                {
                    data = _mapper.Map<MRes_PayrollReconciliation>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Lock), new { id, lockedBy });
            }
        }

        /// <summary>
        /// Mở khóa kỳ lương (chỉ admin)
        /// </summary>
        public async Task<ResponseData<MRes_PayrollReconciliation>> Unlock(int id, int unlockedBy)
        {
            try
            {
                var data = await _context.PayrollReconciliations.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ReconciliationStatus != "LOCKED")
                    return Error(HttpStatusCode.BadRequest, "Kỳ lương này chưa bị khóa!");

                data.ReconciliationStatus = "BALANCED";
                data.LockedBy = null;
                data.LockedAt = null;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = unlockedBy;

                // Mở khóa tất cả payroll trong kỳ
                await _context.Payrolls
                    .Where(p => p.YearMonth == data.YearMonth &&
                               (data.TramId == null || p.Employee.TramId == data.TramId) &&
                               p.PayrollStatus == "LOCKED")
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.PayrollStatus, "APPROVED")
                        .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)
                        .SetProperty(p => p.UpdatedBy, unlockedBy));

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                _logger.LogWarning("PayrollReconciliation.Unlock: Id={Id}, UnlockedBy={UnlockedBy}, YearMonth={YearMonth}",
                    id, unlockedBy, data.YearMonth);

                return new ResponseData<MRes_PayrollReconciliation>(1, (int)HttpStatusCode.OK, "Đã mở khóa kỳ lương!")
                {
                    data = _mapper.Map<MRes_PayrollReconciliation>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Unlock), new { id, unlockedBy });
            }
        }

        /// <summary>
        /// Lấy danh sách cân đối có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_PayrollReconciliation>>> GetListByPaging(MReq_PayrollReconciliation_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_PayrollReconciliation> data = new List<MRes_PayrollReconciliation>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderByDescending(x => x.YearMonth)
                        .ThenBy(x => x.Tram.Code)
                        .ProjectTo<MRes_PayrollReconciliation>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_PayrollReconciliation>>
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
        /// Lấy chi tiết cân đối theo ID
        /// </summary>
        public async Task<ResponseData<MRes_PayrollReconciliation>> GetById(int id)
        {
            try
            {
                var data = await _context.PayrollReconciliations
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_PayrollReconciliation>
                {
                    data = _mapper.Map<MRes_PayrollReconciliation>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Cập nhật ghi chú cân đối (chỉ khi chưa khóa)
        /// </summary>
        public async Task<ResponseData<MRes_PayrollReconciliation>> Update(MReq_PayrollReconciliation request)
        {
            try
            {
                var data = await _context.PayrollReconciliations.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ReconciliationStatus == "LOCKED")
                    return Error(HttpStatusCode.BadRequest, "Không thể sửa kỳ lương đã khóa!");

                data.Notes = request.Notes;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_PayrollReconciliation>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_PayrollReconciliation>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        #region Common functions
        private IQueryable<PayrollReconciliation> BuildFilterQuery(MReq_PayrollReconciliation_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.PayrollReconciliations.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (!string.IsNullOrWhiteSpace(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            if (request.TramId.HasValue)
                query = query.Where(x => x.TramId == request.TramId);

            if (!string.IsNullOrWhiteSpace(request.ReconciliationStatus))
                query = query.Where(x => x.ReconciliationStatus == request.ReconciliationStatus);

            return query;
        }
        #endregion
    }
}
