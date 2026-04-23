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
    public interface IS_ZoneSupport
    {
        Task<ResponseData<MRes_ZoneSupport>> Create(MReq_ZoneSupport request);
        Task<ResponseData<MRes_ZoneSupport>> Update(MReq_ZoneSupport request);
        Task<ResponseData<MRes_ZoneSupport>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_ZoneSupport>> GetById(int id);
        Task<ResponseData<List<MRes_ZoneSupport>>> GetListByPaging(MReq_ZoneSupport_FullParam request);
        Task<ResponseData<List<MRes_ZoneSupport>>> GetByTramId(int tramId);
        Task<ResponseData<MRes_ZoneSupport>> GetActiveSupportByTram(int tramId, DateTime date);
    }

    /// <summary>
    /// Quản lý hỗ trợ vùng (vùng khó khăn, vùng sâu vùng xa)
    /// </summary>
    public class S_ZoneSupport : BaseService<S_ZoneSupport>, IS_ZoneSupport
    {
        private readonly IMapper _mapper;

        public S_ZoneSupport(MainDbContext context, IMapper mapper, ILogger<S_ZoneSupport> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới hỗ trợ vùng
        /// </summary>
        public async Task<ResponseData<MRes_ZoneSupport>> Create(MReq_ZoneSupport request)
        {
            try
            {
                var tram = await _context.Trams.FindAsync(request.TramId);
                if (tram == null)
                    return Error(HttpStatusCode.NotFound, "Không tìm thấy trạm!");

                // Kiểm tra trùng loại hỗ trợ trong cùng thời gian
                var isOverlap = await _context.ZoneSupports.AnyAsync(x =>
                    x.TramId == request.TramId &&
                    x.SupportType == request.SupportType &&
                    x.Status != -1 &&
                    x.EffectiveDate <= (request.EndDate ?? DateTime.MaxValue) &&
                    (x.EndDate == null || x.EndDate >= request.EffectiveDate));

                if (isOverlap)
                    return Error(HttpStatusCode.Conflict, "Đã có hỗ trợ vùng cùng loại trong khoảng thời gian này!");

                var data = _mapper.Map<ZoneSupport>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.ZoneSupports.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.ZoneSupports
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_ZoneSupport>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_ZoneSupport>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật hỗ trợ vùng
        /// </summary>
        public async Task<ResponseData<MRes_ZoneSupport>> Update(MReq_ZoneSupport request)
        {
            try
            {
                var data = await _context.ZoneSupports.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                // Kiểm tra trùng loại hỗ trợ trong cùng thời gian (ngoại trừ bản ghi hiện tại)
                var isOverlap = await _context.ZoneSupports.AnyAsync(x =>
                    x.TramId == request.TramId &&
                    x.SupportType == request.SupportType &&
                    x.Status != -1 &&
                    x.Id != request.Id &&
                    x.EffectiveDate <= (request.EndDate ?? DateTime.MaxValue) &&
                    (x.EndDate == null || x.EndDate >= request.EffectiveDate));

                if (isOverlap)
                    return Error(HttpStatusCode.Conflict, "Đã có hỗ trợ vùng cùng loại trong khoảng thời gian này!");

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.ZoneSupports
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_ZoneSupport>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_ZoneSupport>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái hỗ trợ vùng (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_ZoneSupport>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.ZoneSupports.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_ZoneSupport>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_ZoneSupport>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm hỗ trợ vùng (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.ZoneSupports
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
        /// Lấy chi tiết hỗ trợ vùng theo ID
        /// </summary>
        public async Task<ResponseData<MRes_ZoneSupport>> GetById(int id)
        {
            try
            {
                var data = await _context.ZoneSupports
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_ZoneSupport>
                {
                    data = _mapper.Map<MRes_ZoneSupport>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách hỗ trợ vùng có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_ZoneSupport>>> GetListByPaging(MReq_ZoneSupport_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_ZoneSupport> data = new List<MRes_ZoneSupport>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Tram.Code)
                        .ThenByDescending(x => x.EffectiveDate)
                        .ProjectTo<MRes_ZoneSupport>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_ZoneSupport>>
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
        /// Lấy tất cả hỗ trợ vùng của một trạm
        /// </summary>
        public async Task<ResponseData<List<MRes_ZoneSupport>>> GetByTramId(int tramId)
        {
            try
            {
                var data = await _context.ZoneSupports
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .Where(x => x.TramId == tramId && x.Status != -1)
                    .OrderByDescending(x => x.EffectiveDate)
                    .ProjectTo<MRes_ZoneSupport>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_ZoneSupport>>
                {
                    data = data,
                    data2nd = data.Count,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByTramId), new { tramId });
            }
        }

        /// <summary>
        /// Lấy hỗ trợ vùng đang có hiệu lực của trạm tại một thời điểm
        /// </summary>
        public async Task<ResponseData<MRes_ZoneSupport>> GetActiveSupportByTram(int tramId, DateTime date)
        {
            try
            {
                var data = await _context.ZoneSupports
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .Where(x => x.TramId == tramId &&
                               x.Status != -1 &&
                               x.EffectiveDate <= date &&
                               (x.EndDate == null || x.EndDate >= date))
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefaultAsync();

                return new ResponseData<MRes_ZoneSupport>
                {
                    data = _mapper.Map<MRes_ZoneSupport>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetActiveSupportByTram), new { tramId, date });
            }
        }

        #region Common functions
        private IQueryable<ZoneSupport> BuildFilterQuery(MReq_ZoneSupport_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.ZoneSupports.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.TramId.HasValue)
                query = query.Where(x => x.TramId == request.TramId);

            if (!string.IsNullOrWhiteSpace(request.SupportType))
                query = query.Where(x => x.SupportType == request.SupportType);

            if (request.EffectiveDate.HasValue)
                query = query.Where(x => x.EffectiveDate <= request.EffectiveDate &&
                                        (x.EndDate == null || x.EndDate >= request.EffectiveDate));

            if (request.IsActive.HasValue && request.IsActive.Value)
            {
                var today = DateTime.Today;
                query = query.Where(x => x.EffectiveDate <= today &&
                                        (x.EndDate == null || x.EndDate >= today));
            }

            return query;
        }
        #endregion
    }
}
