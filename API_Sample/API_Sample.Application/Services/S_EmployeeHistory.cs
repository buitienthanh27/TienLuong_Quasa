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
using Newtonsoft.Json;
using System.Net;

namespace API_Sample.Application.Services
{
    public interface IS_EmployeeHistory
    {
        Task<ResponseData<MRes_EmployeeHistory>> Create(MReq_EmployeeHistory request);
        Task<ResponseData<List<MRes_EmployeeHistory>>> GetByEmployeeId(int employeeId);
        Task<ResponseData<List<MRes_EmployeeHistory>>> GetListByPaging(MReq_EmployeeHistory_FullParam request);
        Task<ResponseData<List<MRes_EmployeeHistory>>> GetChangesByDateRange(DateTime from, DateTime to);
        Task<ResponseData<MRes_EmployeeHistory>> GetById(int id);
        Task<ResponseData<MRes_EmployeeHistory>> RecordChange(int employeeId, string changeType, string oldValue, string newValue, int changedBy, string reason, string decisionNumber);
    }

    /// <summary>
    /// Quản lý lịch sử thay đổi thông tin nhân viên (audit trail)
    /// </summary>
    public class S_EmployeeHistory : BaseService<S_EmployeeHistory>, IS_EmployeeHistory
    {
        private readonly IMapper _mapper;

        public S_EmployeeHistory(MainDbContext context, IMapper mapper, ILogger<S_EmployeeHistory> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới bản ghi lịch sử (thủ công)
        /// </summary>
        public async Task<ResponseData<MRes_EmployeeHistory>> Create(MReq_EmployeeHistory request)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(request.EmployeeId);
                if (employee == null)
                    return Error(HttpStatusCode.NotFound, "Không tìm thấy nhân viên!");

                var data = _mapper.Map<EmployeeHistory>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.EmployeeHistories.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.EmployeeHistories
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                _logger.LogInformation("EmployeeHistory.Create: EmployeeId={EmployeeId}, ChangeType={ChangeType}, OldValue={OldValue}, NewValue={NewValue}",
                    request.EmployeeId, request.ChangeType, request.OldValue, request.NewValue);

                return new ResponseData<MRes_EmployeeHistory>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_EmployeeHistory>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Ghi nhận thay đổi nhân viên (auto-create từ service khác)
        /// </summary>
        public async Task<ResponseData<MRes_EmployeeHistory>> RecordChange(int employeeId, string changeType, string oldValue, string newValue, int changedBy, string reason, string decisionNumber)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null)
                    return Error(HttpStatusCode.NotFound, "Không tìm thấy nhân viên!");

                var data = new EmployeeHistory
                {
                    EmployeeId = employeeId,
                    ChangeType = changeType,
                    OldValue = oldValue,
                    NewValue = newValue,
                    ChangeDate = DateTime.UtcNow.Date,
                    ChangedBy = changedBy,
                    Reason = reason,
                    DecisionNumber = decisionNumber,
                    Status = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = changedBy
                };

                _context.EmployeeHistories.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                _logger.LogInformation("EmployeeHistory.RecordChange: EmployeeId={EmployeeId}, ChangeType={ChangeType}, ChangedBy={ChangedBy}",
                    employeeId, changeType, changedBy);

                return new ResponseData<MRes_EmployeeHistory>(1, (int)HttpStatusCode.OK, "Đã ghi nhận thay đổi!")
                {
                    data = _mapper.Map<MRes_EmployeeHistory>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(RecordChange), new { employeeId, changeType, oldValue, newValue, changedBy });
            }
        }

        /// <summary>
        /// Lấy lịch sử thay đổi của một nhân viên
        /// </summary>
        public async Task<ResponseData<List<MRes_EmployeeHistory>>> GetByEmployeeId(int employeeId)
        {
            try
            {
                var data = await _context.EmployeeHistories
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => x.EmployeeId == employeeId && x.Status != -1)
                    .OrderByDescending(x => x.ChangeDate).ThenByDescending(x => x.CreatedAt)
                    .ProjectTo<MRes_EmployeeHistory>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_EmployeeHistory>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByEmployeeId), new { employeeId });
            }
        }

        /// <summary>
        /// Lấy danh sách lịch sử có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_EmployeeHistory>>> GetListByPaging(MReq_EmployeeHistory_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_EmployeeHistory> data = new List<MRes_EmployeeHistory>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderByDescending(x => x.ChangeDate)
                        .ThenByDescending(x => x.CreatedAt)
                        .ProjectTo<MRes_EmployeeHistory>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_EmployeeHistory>>
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
        /// Lấy các thay đổi trong khoảng thời gian
        /// </summary>
        public async Task<ResponseData<List<MRes_EmployeeHistory>>> GetChangesByDateRange(DateTime from, DateTime to)
        {
            try
            {
                var data = await _context.EmployeeHistories
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => x.ChangeDate >= from && x.ChangeDate <= to && x.Status != -1)
                    .OrderByDescending(x => x.ChangeDate).ThenBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_EmployeeHistory>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_EmployeeHistory>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetChangesByDateRange), new { from, to });
            }
        }

        /// <summary>
        /// Lấy chi tiết lịch sử theo ID
        /// </summary>
        public async Task<ResponseData<MRes_EmployeeHistory>> GetById(int id)
        {
            try
            {
                var data = await _context.EmployeeHistories
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_EmployeeHistory>
                {
                    data = _mapper.Map<MRes_EmployeeHistory>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        #region Common functions
        private IQueryable<EmployeeHistory> BuildFilterQuery(MReq_EmployeeHistory_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.EmployeeHistories.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId);

            if (!string.IsNullOrWhiteSpace(request.ChangeType))
                query = query.Where(x => x.ChangeType == request.ChangeType);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.ChangeDate >= request.FromDate);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.ChangeDate <= request.ToDate);

            if (request.TramId.HasValue)
                query = query.Where(x => x.Employee.TramId == request.TramId);

            return query;
        }
        #endregion
    }
}
