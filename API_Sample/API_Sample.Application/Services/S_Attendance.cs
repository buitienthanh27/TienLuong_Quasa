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
    public interface IS_Attendance
    {
        Task<ResponseData<MRes_Attendance>> Create(MReq_Attendance request);
        Task<ResponseData<MRes_Attendance>> Update(MReq_Attendance request);
        Task<ResponseData<int>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Attendance>> GetById(int id);
        Task<ResponseData<List<MRes_Attendance>>> GetListByPaging(MReq_Attendance_FullParam request);
        Task<ResponseData<List<MRes_Attendance>>> GetListByFullParam(MReq_Attendance_FullParam request);
        Task<ResponseData<int>> BulkImport(MReq_Attendance_BulkImport request);
    }

    /// <summary>
    /// Quản lý chấm công hàng tháng của nhân viên
    /// </summary>
    public class S_Attendance : BaseService<S_Attendance>, IS_Attendance
    {
        private readonly IMapper _mapper;

        public S_Attendance(MainDbContext context, IMapper mapper, ILogger<S_Attendance> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới bản ghi chấm công cho nhân viên theo tháng
        /// </summary>
        public async Task<ResponseData<MRes_Attendance>> Create(MReq_Attendance request)
        {
            try
            {
                var isExists = await _context.Attendances.AnyAsync(x =>
                    x.EmployeeId == request.EmployeeId &&
                    x.YearMonth == request.YearMonth &&
                    x.Status != -1);
                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Chấm công tháng này đã tồn tại cho nhân viên!");

                var data = _mapper.Map<Attendance>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.Attendances.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                await _context.Entry(data).Reference(x => x.Employee).LoadAsync();

                return new ResponseData<MRes_Attendance>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Attendance>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật thông tin chấm công
        /// </summary>
        public async Task<ResponseData<MRes_Attendance>> Update(MReq_Attendance request)
        {
            try
            {
                var data = await _context.Attendances.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Attendance>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Attendance>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái bản ghi chấm công
        /// </summary>
        public async Task<ResponseData<int>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var updatedCount = await _context.Attendances
                    .Where(x => x.Id == id)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.Status, status)
                        .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                        .SetProperty(x => x.UpdatedBy, updatedBy));

                if (updatedCount == 0)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<int>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = updatedCount
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm bản ghi chấm công (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            return await UpdateStatus(id, -1, 0);
        }

        /// <summary>
        /// Lấy chi tiết chấm công theo ID
        /// </summary>
        public async Task<ResponseData<MRes_Attendance>> GetById(int id)
        {
            try
            {
                var data = await _context.Attendances.AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status != -1);

                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<MRes_Attendance>(1, (int)HttpStatusCode.OK, "")
                {
                    data = _mapper.Map<MRes_Attendance>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách chấm công có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_Attendance>>> GetListByPaging(MReq_Attendance_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Attendance> data = new List<MRes_Attendance>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Employee.Msnv)
                        .ProjectTo<MRes_Attendance>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_Attendance>>
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
        /// Lấy danh sách chấm công theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_Attendance>>> GetListByFullParam(MReq_Attendance_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_Attendance>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Attendance>>
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
        /// Import hàng loạt chấm công, cập nhật nếu đã tồn tại
        /// </summary>
        public async Task<ResponseData<int>> BulkImport(MReq_Attendance_BulkImport request)
        {
            try
            {
                if (request.Items == null || !request.Items.Any())
                    return Error(HttpStatusCode.BadRequest, "Không có dữ liệu để import!");

                var now = DateTime.UtcNow;
                int insertedCount = 0;
                int updatedCount = 0;

                foreach (var item in request.Items)
                {
                    var existing = await _context.Attendances.FirstOrDefaultAsync(x =>
                        x.EmployeeId == item.EmployeeId &&
                        x.YearMonth == request.YearMonth &&
                        x.Status != -1);

                    if (existing != null)
                    {
                        existing.RegularDays = item.RegularDays;
                        existing.SundayDays = item.SundayDays;
                        existing.YoungTreeDays = item.YoungTreeDays;
                        existing.HardshipDays = item.HardshipDays;
                        existing.DoubleCutDays = item.DoubleCutDays;
                        existing.DoubleCutSunday = item.DoubleCutSunday;
                        existing.CareDays = item.CareDays;
                        existing.AbsentDays = item.AbsentDays;
                        existing.SickDays = item.SickDays;
                        existing.LeaveDays = item.LeaveDays;
                        existing.TotalDays = item.RegularDays + item.SundayDays + item.YoungTreeDays
                                           + item.HardshipDays + item.DoubleCutDays + item.DoubleCutSunday + item.CareDays;
                        existing.UpdatedAt = now;
                        existing.UpdatedBy = request.CreatedBy;
                        updatedCount++;
                    }
                    else
                    {
                        var totalDays = item.RegularDays + item.SundayDays + item.YoungTreeDays
                                      + item.HardshipDays + item.DoubleCutDays + item.DoubleCutSunday + item.CareDays;
                        _context.Attendances.Add(new Attendance
                        {
                            EmployeeId = item.EmployeeId,
                            YearMonth = request.YearMonth,
                            RegularDays = item.RegularDays,
                            SundayDays = item.SundayDays,
                            YoungTreeDays = item.YoungTreeDays,
                            HardshipDays = item.HardshipDays,
                            DoubleCutDays = item.DoubleCutDays,
                            DoubleCutSunday = item.DoubleCutSunday,
                            CareDays = item.CareDays,
                            AbsentDays = item.AbsentDays,
                            SickDays = item.SickDays,
                            LeaveDays = item.LeaveDays,
                            TotalDays = totalDays,
                            AttendanceStatus = "DRAFT",
                            Status = 1,
                            CreatedAt = now,
                            CreatedBy = request.CreatedBy
                        });
                        insertedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                return new ResponseData<int>(1, (int)HttpStatusCode.OK, $"Import thành công: {insertedCount} mới, {updatedCount} cập nhật")
                {
                    data = insertedCount + updatedCount
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(BulkImport), request);
            }
        }

        #region Common functions
        private IQueryable<Attendance> BuildFilterQuery(MReq_Attendance_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.Attendances.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

            if (!string.IsNullOrWhiteSpace(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            if (!string.IsNullOrWhiteSpace(request.AttendanceStatus))
                query = query.Where(x => x.AttendanceStatus == request.AttendanceStatus);

            return query;
        }
        #endregion
    }
}
