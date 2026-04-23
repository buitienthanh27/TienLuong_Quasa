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
    public interface IS_AdvancePayment
    {
        Task<ResponseData<MRes_AdvancePayment>> Create(MReq_AdvancePayment request);
        Task<ResponseData<MRes_AdvancePayment>> Update(MReq_AdvancePayment request);
        Task<ResponseData<MRes_AdvancePayment>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_AdvancePayment>> GetById(int id);
        Task<ResponseData<List<MRes_AdvancePayment>>> GetListByPaging(MReq_AdvancePayment_FullParam request);
        Task<ResponseData<List<MRes_AdvancePayment>>> GetListByFullParam(MReq_AdvancePayment_FullParam request);
        Task<ResponseData<List<MRes_AdvancePayment>>> GetPendingByEmployee(int employeeId);
        Task<ResponseData<MRes_AdvancePayment>> MarkAsDeducted(int id, int payrollId, int updatedBy);
        Task<ResponseData<List<MRes_AdvancePayment>>> GetByEmployeeYearMonth(int employeeId, string yearMonth);
        Task<ResponseData<MRes_AdvancePayment>> Approve(int id, int approvedBy);
        Task<ResponseData<decimal>> CalculateInterest(int id);
        Task<ResponseData<List<MRes_AdvancePayment>>> GetPendingRepayments(string yearMonth);
    }

    /// <summary>
    /// Quản lý tạm ứng/truy thu (ứng lương lễ, truy thu...)
    /// </summary>
    public class S_AdvancePayment : BaseService<S_AdvancePayment>, IS_AdvancePayment
    {
        private readonly IMapper _mapper;

        public S_AdvancePayment(MainDbContext context, IMapper mapper, ILogger<S_AdvancePayment> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới khoản tạm ứng/truy thu
        /// </summary>
        public async Task<ResponseData<MRes_AdvancePayment>> Create(MReq_AdvancePayment request)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(request.EmployeeId);
                if (employee == null)
                    return Error(HttpStatusCode.NotFound, "Không tìm thấy nhân viên!");

                var data = _mapper.Map<AdvancePayment>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;
                data.IsDeducted = false;

                _context.AdvancePayments.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.AdvancePayments
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_AdvancePayment>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_AdvancePayment>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật khoản tạm ứng/truy thu
        /// </summary>
        public async Task<ResponseData<MRes_AdvancePayment>> Update(MReq_AdvancePayment request)
        {
            try
            {
                var data = await _context.AdvancePayments.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.IsDeducted)
                    return Error(HttpStatusCode.BadRequest, "Không thể sửa khoản đã khấu trừ!");

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.AdvancePayments
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_AdvancePayment>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_AdvancePayment>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Đánh dấu đã khấu trừ vào lương
        /// </summary>
        public async Task<ResponseData<MRes_AdvancePayment>> MarkAsDeducted(int id, int payrollId, int updatedBy)
        {
            try
            {
                var data = await _context.AdvancePayments.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.IsDeducted = true;
                data.DeductedInPayrollId = payrollId;
                data.DeductedAt = DateTime.UtcNow;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_AdvancePayment>(1, (int)HttpStatusCode.OK, "Đã đánh dấu khấu trừ!")
                {
                    data = _mapper.Map<MRes_AdvancePayment>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(MarkAsDeducted), new { id, payrollId, updatedBy });
            }
        }

        /// <summary>
        /// Lấy các khoản chưa khấu trừ của nhân viên
        /// </summary>
        public async Task<ResponseData<List<MRes_AdvancePayment>>> GetPendingByEmployee(int employeeId)
        {
            try
            {
                var data = await _context.AdvancePayments
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => x.EmployeeId == employeeId &&
                                !x.IsDeducted &&
                                x.Status != -1)
                    .OrderBy(x => x.PaymentDate)
                    .ProjectTo<MRes_AdvancePayment>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_AdvancePayment>>
                {
                    data = data,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetPendingByEmployee), new { employeeId });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái khoản tạm ứng (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_AdvancePayment>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.AdvancePayments.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_AdvancePayment>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_AdvancePayment>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm khoản tạm ứng (set Status = -1), không xóa được khoản đã khấu trừ
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var data = await _context.AdvancePayments.FindAsync(id);
                if (data != null && data.IsDeducted)
                    return Error(HttpStatusCode.BadRequest, "Không thể xoá khoản đã khấu trừ!");

                var deletedCount = await _context.AdvancePayments
                    .Where(x => x.Id == id)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, (short)-1));

                if (deletedCount == 0)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<int>(1, (int)HttpStatusCode.OK, MessageErrorConstants.DELETE_SUCCESS)
                {
                    data = deletedCount
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Delete), new { id });
            }
        }

        /// <summary>
        /// Lấy chi tiết khoản tạm ứng theo ID
        /// </summary>
        public async Task<ResponseData<MRes_AdvancePayment>> GetById(int id)
        {
            try
            {
                var data = await _context.AdvancePayments
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_AdvancePayment>
                {
                    data = _mapper.Map<MRes_AdvancePayment>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách khoản tạm ứng có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_AdvancePayment>>> GetListByPaging(MReq_AdvancePayment_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_AdvancePayment> data = new List<MRes_AdvancePayment>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderByDescending(x => x.PaymentDate)
                        .ThenBy(x => x.Employee.Msnv)
                        .ProjectTo<MRes_AdvancePayment>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_AdvancePayment>>
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
        /// Lấy danh sách khoản tạm ứng theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_AdvancePayment>>> GetListByFullParam(MReq_AdvancePayment_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderByDescending(x => x.PaymentDate).ThenBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_AdvancePayment>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_AdvancePayment>>
                {
                    data = data,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByFullParam), request);
            }
        }

        /// <summary>
        /// Lấy danh sách tạm ứng của nhân viên trong tháng
        /// </summary>
        public async Task<ResponseData<List<MRes_AdvancePayment>>> GetByEmployeeYearMonth(int employeeId, string yearMonth)
        {
            try
            {
                var data = await _context.AdvancePayments
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => x.EmployeeId == employeeId &&
                               x.YearMonth == yearMonth &&
                               x.Status != -1)
                    .OrderByDescending(x => x.PaymentDate)
                    .ProjectTo<MRes_AdvancePayment>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_AdvancePayment>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByEmployeeYearMonth), new { employeeId, yearMonth });
            }
        }

        /// <summary>
        /// Duyệt khoản tạm ứng
        /// </summary>
        public async Task<ResponseData<MRes_AdvancePayment>> Approve(int id, int approvedBy)
        {
            try
            {
                var data = await _context.AdvancePayments.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ApprovedBy.HasValue)
                    return Error(HttpStatusCode.BadRequest, "Khoản tạm ứng này đã được duyệt!");

                data.ApprovedBy = approvedBy;
                data.ApprovedAt = DateTime.UtcNow;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = approvedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                _logger.LogInformation("AdvancePayment.Approve: Id={Id}, ApprovedBy={ApprovedBy}", id, approvedBy);

                var result = await _context.AdvancePayments
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_AdvancePayment>(1, (int)HttpStatusCode.OK, "Đã duyệt khoản tạm ứng!")
                {
                    data = _mapper.Map<MRes_AdvancePayment>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Approve), new { id, approvedBy });
            }
        }

        /// <summary>
        /// Tính lãi cho khoản tạm ứng (nếu có chính sách tính lãi)
        /// </summary>
        public async Task<ResponseData<decimal>> CalculateInterest(int id)
        {
            try
            {
                var data = await _context.AdvancePayments.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                // Lấy tham số lãi suất từ SystemParameter (mặc định 0% cho tạm ứng lương)
                var rate = await _context.SystemParameters
                    .AsNoTracking()
                    .Where(p => p.ParamCode == "ADVANCE_INTEREST_RATE" && p.Status != -1)
                    .OrderByDescending(p => p.EffectiveDate)
                    .Select(p => p.ParamValue)
                    .FirstOrDefaultAsync();

                if (rate == 0)
                {
                    return new ResponseData<decimal>
                    {
                        data = 0,
                        result = 1
                    };
                }

                // Tính số ngày từ ngày ứng đến hiện tại
                var paymentDate = data.PaymentDate ?? data.CreatedAt;
                var daysElapsed = (DateTime.UtcNow - paymentDate).Days;

                // Lãi = Số tiền × Lãi suất (năm) × Số ngày / 365
                var interest = data.Amount * (rate / 100) * daysElapsed / 365;

                return new ResponseData<decimal>
                {
                    data = Math.Round(interest, 0), // Làm tròn đến đơn vị
                    data2nd = daysElapsed, // Số ngày
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(CalculateInterest), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách tạm ứng chưa trừ (cần truy thu) trong tháng
        /// </summary>
        public async Task<ResponseData<List<MRes_AdvancePayment>>> GetPendingRepayments(string yearMonth)
        {
            try
            {
                // Lấy các khoản tạm ứng chưa khấu trừ VÀ đã đến hạn trừ
                var data = await _context.AdvancePayments
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => !x.IsDeducted &&
                               x.Status != -1 &&
                               (x.DeductedInMonth == null || x.DeductedInMonth == yearMonth))
                    .OrderBy(x => x.PaymentDate).ThenBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_AdvancePayment>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_AdvancePayment>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetPendingRepayments), new { yearMonth });
            }
        }

        #region Common functions
        private IQueryable<AdvancePayment> BuildFilterQuery(MReq_AdvancePayment_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.AdvancePayments.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId);

            if (!string.IsNullOrWhiteSpace(request.PaymentType))
                query = query.Where(x => x.PaymentType == request.PaymentType);

            if (request.IsDeducted.HasValue)
                query = query.Where(x => x.IsDeducted == request.IsDeducted);

            if (request.TramId.HasValue)
                query = query.Where(x => x.Employee.TramId == request.TramId);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.PaymentDate >= request.FromDate);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.PaymentDate <= request.ToDate);

            return query;
        }
        #endregion
    }
}
