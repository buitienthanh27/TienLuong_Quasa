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
    public interface IS_CareAdjustment
    {
        Task<ResponseData<MRes_CareAdjustment>> Create(MReq_CareAdjustment request);
        Task<ResponseData<MRes_CareAdjustment>> Update(MReq_CareAdjustment request);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_CareAdjustment>> GetById(int id);
        Task<ResponseData<List<MRes_CareAdjustment>>> GetListByPaging(MReq_CareAdjustment_FullParam request);
        Task<ResponseData<List<MRes_CareAdjustment>>> GetListByYearMonth(string yearMonth);
        Task<ResponseData<MRes_CareAdjustment>> Approve(int id, int approvedBy);
        Task<ResponseData<MRes_CareAdjustment>> Reject(int id, int rejectedBy, string reason);
        Task<ResponseData<List<MRes_CareAdjustment>>> GetPendingApprovals();
    }

    /// <summary>
    /// Quản lý bổ công chăm sóc (điều chỉnh ngày công chăm sóc cây cao su)
    /// </summary>
    public class S_CareAdjustment : BaseService<S_CareAdjustment>, IS_CareAdjustment
    {
        private readonly IMapper _mapper;

        public S_CareAdjustment(MainDbContext context, IMapper mapper, ILogger<S_CareAdjustment> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới yêu cầu bổ công chăm sóc
        /// </summary>
        public async Task<ResponseData<MRes_CareAdjustment>> Create(MReq_CareAdjustment request)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(request.EmployeeId);
                if (employee == null)
                    return Error(HttpStatusCode.NotFound, "Không tìm thấy nhân viên!");

                var data = _mapper.Map<CareAdjustment>(request);
                data.ApprovalStatus = "PENDING";
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                // Tính số tiền bổ công (25,000 KIP/công)
                var careRate = await _context.WorkTypes.AsNoTracking()
                    .Where(w => w.Code == "CHAM_SOC" && w.Status != -1)
                    .Select(w => w.UnitPrice)
                    .FirstOrDefaultAsync();
                data.CareAmount = data.CareDays * (careRate > 0 ? careRate : 25000m);

                _context.CareAdjustments.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.CareAdjustments
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_CareAdjustment>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_CareAdjustment>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật yêu cầu bổ công chăm sóc (chỉ khi chưa duyệt)
        /// </summary>
        public async Task<ResponseData<MRes_CareAdjustment>> Update(MReq_CareAdjustment request)
        {
            try
            {
                var data = await _context.CareAdjustments.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ApprovalStatus != "PENDING")
                    return Error(HttpStatusCode.BadRequest, "Không thể sửa yêu cầu đã được duyệt/từ chối!");

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                // Tính lại số tiền bổ công
                var careRate = await _context.WorkTypes.AsNoTracking()
                    .Where(w => w.Code == "CHAM_SOC" && w.Status != -1)
                    .Select(w => w.UnitPrice)
                    .FirstOrDefaultAsync();
                data.CareAmount = data.CareDays * (careRate > 0 ? careRate : 25000m);

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.CareAdjustments
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_CareAdjustment>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_CareAdjustment>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Xóa mềm yêu cầu bổ công (chỉ khi chưa duyệt)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var data = await _context.CareAdjustments.FindAsync(id);
                if (data != null && data.ApprovalStatus != "PENDING")
                    return Error(HttpStatusCode.BadRequest, "Không thể xoá yêu cầu đã được duyệt/từ chối!");

                var deletedCount = await _context.CareAdjustments
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
        /// Lấy chi tiết yêu cầu bổ công theo ID
        /// </summary>
        public async Task<ResponseData<MRes_CareAdjustment>> GetById(int id)
        {
            try
            {
                var data = await _context.CareAdjustments
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_CareAdjustment>
                {
                    data = _mapper.Map<MRes_CareAdjustment>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách yêu cầu bổ công có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_CareAdjustment>>> GetListByPaging(MReq_CareAdjustment_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_CareAdjustment> data = new List<MRes_CareAdjustment>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderByDescending(x => x.CreatedAt)
                        .ProjectTo<MRes_CareAdjustment>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_CareAdjustment>>
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
        /// Lấy danh sách bổ công theo tháng
        /// </summary>
        public async Task<ResponseData<List<MRes_CareAdjustment>>> GetListByYearMonth(string yearMonth)
        {
            try
            {
                var data = await _context.CareAdjustments
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => x.YearMonth == yearMonth && x.Status != -1)
                    .OrderBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_CareAdjustment>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_CareAdjustment>>
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
        /// Duyệt yêu cầu bổ công
        /// </summary>
        public async Task<ResponseData<MRes_CareAdjustment>> Approve(int id, int approvedBy)
        {
            try
            {
                var data = await _context.CareAdjustments.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ApprovalStatus != "PENDING")
                    return Error(HttpStatusCode.BadRequest, "Yêu cầu này đã được xử lý!");

                data.ApprovalStatus = "APPROVED";
                data.ApprovedBy = approvedBy;
                data.ApprovedAt = DateTime.UtcNow;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = approvedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                _logger.LogInformation("CareAdjustment.Approve: Id={Id}, ApprovedBy={ApprovedBy}", id, approvedBy);

                return new ResponseData<MRes_CareAdjustment>(1, (int)HttpStatusCode.OK, "Đã duyệt yêu cầu bổ công!")
                {
                    data = _mapper.Map<MRes_CareAdjustment>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Approve), new { id, approvedBy });
            }
        }

        /// <summary>
        /// Từ chối yêu cầu bổ công
        /// </summary>
        public async Task<ResponseData<MRes_CareAdjustment>> Reject(int id, int rejectedBy, string reason)
        {
            try
            {
                var data = await _context.CareAdjustments.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (data.ApprovalStatus != "PENDING")
                    return Error(HttpStatusCode.BadRequest, "Yêu cầu này đã được xử lý!");

                data.ApprovalStatus = "REJECTED";
                data.Reason = reason;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = rejectedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                _logger.LogInformation("CareAdjustment.Reject: Id={Id}, RejectedBy={RejectedBy}, Reason={Reason}", id, rejectedBy, reason);

                return new ResponseData<MRes_CareAdjustment>(1, (int)HttpStatusCode.OK, "Đã từ chối yêu cầu bổ công!")
                {
                    data = _mapper.Map<MRes_CareAdjustment>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Reject), new { id, rejectedBy, reason });
            }
        }

        /// <summary>
        /// Lấy danh sách yêu cầu bổ công đang chờ duyệt
        /// </summary>
        public async Task<ResponseData<List<MRes_CareAdjustment>>> GetPendingApprovals()
        {
            try
            {
                var data = await _context.CareAdjustments
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => x.ApprovalStatus == "PENDING" && x.Status != -1)
                    .OrderBy(x => x.CreatedAt)
                    .ProjectTo<MRes_CareAdjustment>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_CareAdjustment>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetPendingApprovals), null);
            }
        }

        #region Common functions
        private IQueryable<CareAdjustment> BuildFilterQuery(MReq_CareAdjustment_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.CareAdjustments.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId);

            if (!string.IsNullOrWhiteSpace(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            if (!string.IsNullOrWhiteSpace(request.AdjustmentType))
                query = query.Where(x => x.AdjustmentType == request.AdjustmentType);

            if (!string.IsNullOrWhiteSpace(request.ApprovalStatus))
                query = query.Where(x => x.ApprovalStatus == request.ApprovalStatus);

            if (request.TramId.HasValue)
                query = query.Where(x => x.Employee.TramId == request.TramId);

            return query;
        }
        #endregion
    }
}
