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
    public interface IS_Holiday
    {
        Task<ResponseData<MRes_Holiday>> Create(MReq_Holiday request);
        Task<ResponseData<MRes_Holiday>> Update(MReq_Holiday request);
        Task<ResponseData<MRes_Holiday>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Holiday>> GetById(int id);
        Task<ResponseData<List<MRes_Holiday>>> GetListByPaging(MReq_Holiday_FullParam request);
        Task<ResponseData<List<MRes_Holiday>>> GetListByFullParam(MReq_Holiday_FullParam request);
        Task<ResponseData<List<MRes_Holiday>>> GetListByYear(int year);
        Task<ResponseData<List<MRes_Holiday>>> GetHolidaysInMonth(string yearMonth);
        Task<ResponseData<bool>> IsHoliday(DateTime date);
        Task<ResponseData<List<MRes_Holiday>>> GetHolidaysByYear(int year);
        Task<ResponseData<MRes_Holiday>> CheckHoliday(DateTime date);
    }

    /// <summary>
    /// Quản lý ngày lễ VN + Lào và hệ số thưởng lễ
    /// </summary>
    public class S_Holiday : BaseService<S_Holiday>, IS_Holiday
    {
        private readonly IMapper _mapper;

        public S_Holiday(MainDbContext context, IMapper mapper, ILogger<S_Holiday> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới ngày lễ
        /// </summary>
        public async Task<ResponseData<MRes_Holiday>> Create(MReq_Holiday request)
        {
            try
            {
                var isExists = await _context.Holidays.AnyAsync(x =>
                    x.HolidayDate == request.HolidayDate && x.Status != -1);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, $"Ngày lễ {request.HolidayDate:dd/MM/yyyy} đã tồn tại!");

                var data = _mapper.Map<Holiday>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.Holidays.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_Holiday>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Holiday>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật ngày lễ
        /// </summary>
        public async Task<ResponseData<MRes_Holiday>> Update(MReq_Holiday request)
        {
            try
            {
                var isExists = await _context.Holidays.AnyAsync(x =>
                    x.HolidayDate == request.HolidayDate && x.Status != -1 && x.Id != request.Id);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, $"Ngày lễ {request.HolidayDate:dd/MM/yyyy} đã tồn tại!");

                var data = await _context.Holidays.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Holiday>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Holiday>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái ngày lễ (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_Holiday>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.Holidays.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Holiday>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Holiday>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm ngày lễ (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.Holidays
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
        /// Lấy chi tiết ngày lễ theo ID
        /// </summary>
        public async Task<ResponseData<MRes_Holiday>> GetById(int id)
        {
            try
            {
                var data = await _context.Holidays.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_Holiday>
                {
                    data = _mapper.Map<MRes_Holiday>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách ngày lễ theo năm
        /// </summary>
        public async Task<ResponseData<List<MRes_Holiday>>> GetHolidaysByYear(int year)
        {
            try
            {
                var data = await _context.Holidays
                    .AsNoTracking()
                    .Where(x => x.HolidayDate.Year == year && x.Status != -1)
                    .OrderBy(x => x.HolidayDate)
                    .ProjectTo<MRes_Holiday>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Holiday>>
                {
                    data = data,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetHolidaysByYear), new { year });
            }
        }

        /// <summary>
        /// Kiểm tra một ngày có phải ngày lễ không
        /// </summary>
        public async Task<ResponseData<MRes_Holiday>> CheckHoliday(DateTime date)
        {
            try
            {
                var data = await _context.Holidays
                    .AsNoTracking()
                    .Where(x => x.HolidayDate.Date == date.Date && x.Status != -1)
                    .FirstOrDefaultAsync();

                return new ResponseData<MRes_Holiday>
                {
                    data = _mapper.Map<MRes_Holiday>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(CheckHoliday), new { date });
            }
        }

        /// <summary>
        /// Lấy danh sách ngày lễ theo năm (alias cho GetHolidaysByYear)
        /// </summary>
        public async Task<ResponseData<List<MRes_Holiday>>> GetListByYear(int year)
        {
            return await GetHolidaysByYear(year);
        }

        /// <summary>
        /// Lấy danh sách ngày lễ trong một tháng cụ thể
        /// </summary>
        public async Task<ResponseData<List<MRes_Holiday>>> GetHolidaysInMonth(string yearMonth)
        {
            try
            {
                var parts = yearMonth.Split('-');
                var year = int.Parse(parts[0]);
                var month = int.Parse(parts[1]);

                var data = await _context.Holidays
                    .AsNoTracking()
                    .Where(x => x.HolidayDate.Year == year && x.HolidayDate.Month == month && x.Status != -1)
                    .OrderBy(x => x.HolidayDate)
                    .ProjectTo<MRes_Holiday>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Holiday>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetHolidaysInMonth), new { yearMonth });
            }
        }

        /// <summary>
        /// Kiểm tra một ngày có phải ngày lễ không (trả về boolean)
        /// </summary>
        public async Task<ResponseData<bool>> IsHoliday(DateTime date)
        {
            try
            {
                var isHoliday = await _context.Holidays
                    .AsNoTracking()
                    .AnyAsync(x => x.HolidayDate.Date == date.Date && x.Status != -1);

                return new ResponseData<bool>
                {
                    data = isHoliday,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(IsHoliday), new { date });
            }
        }

        /// <summary>
        /// Lấy danh sách ngày lễ có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_Holiday>>> GetListByPaging(MReq_Holiday_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Holiday> data = new List<MRes_Holiday>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderByDescending(x => x.HolidayDate)
                        .ProjectTo<MRes_Holiday>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_Holiday>>
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
        /// Lấy danh sách ngày lễ theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_Holiday>>> GetListByFullParam(MReq_Holiday_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderByDescending(x => x.HolidayDate)
                    .ProjectTo<MRes_Holiday>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Holiday>>
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

        #region Common functions
        private IQueryable<Holiday> BuildFilterQuery(MReq_Holiday_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.Holidays.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.Year.HasValue)
                query = query.Where(x => x.HolidayDate.Year == request.Year);

            if (!string.IsNullOrWhiteSpace(request.Country))
                query = query.Where(x => x.Country == request.Country);

            if (request.IsPaid.HasValue)
                query = query.Where(x => x.IsPaid == request.IsPaid);

            return query;
        }
        #endregion
    }
}
