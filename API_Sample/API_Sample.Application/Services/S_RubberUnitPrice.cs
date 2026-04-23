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
    public interface IS_RubberUnitPrice
    {
        Task<ResponseData<MRes_RubberUnitPrice>> Create(MReq_RubberUnitPrice request);
        Task<ResponseData<MRes_RubberUnitPrice>> Update(MReq_RubberUnitPrice request);
        Task<ResponseData<MRes_RubberUnitPrice>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_RubberUnitPrice>> GetById(int id);
        Task<ResponseData<List<MRes_RubberUnitPrice>>> GetListByPaging(MReq_RubberUnitPrice_FullParam request);
        Task<ResponseData<List<MRes_RubberUnitPrice>>> GetListByFullParam(MReq_RubberUnitPrice_FullParam request);
        Task<ResponseData<MRes_RubberUnitPrice>> GetCurrentPrice(int tramId, string grade, DateTime? effectiveDate = null);
    }

    /// <summary>
    /// Quản lý đơn giá mủ theo hạng kỹ thuật (Bath/kg)
    /// </summary>
    public class S_RubberUnitPrice : BaseService<S_RubberUnitPrice>, IS_RubberUnitPrice
    {
        private readonly IMapper _mapper;

        public S_RubberUnitPrice(MainDbContext context, IMapper mapper, ILogger<S_RubberUnitPrice> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới đơn giá mủ
        /// </summary>
        public async Task<ResponseData<MRes_RubberUnitPrice>> Create(MReq_RubberUnitPrice request)
        {
            try
            {
                request.Grade = request.Grade?.Trim().ToUpper();

                var isExists = await _context.RubberUnitPrices.AnyAsync(x =>
                    x.TramId == request.TramId &&
                    x.Grade == request.Grade &&
                    x.EffectiveDate == request.EffectiveDate &&
                    x.Status != -1);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Đơn giá đã tồn tại cho trạm/hạng/ngày hiệu lực này!");

                var data = _mapper.Map<RubberUnitPrice>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.RubberUnitPrices.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.RubberUnitPrices
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_RubberUnitPrice>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_RubberUnitPrice>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật đơn giá mủ
        /// </summary>
        public async Task<ResponseData<MRes_RubberUnitPrice>> Update(MReq_RubberUnitPrice request)
        {
            try
            {
                request.Grade = request.Grade?.Trim().ToUpper();

                var isExists = await _context.RubberUnitPrices.AnyAsync(x =>
                    x.TramId == request.TramId &&
                    x.Grade == request.Grade &&
                    x.EffectiveDate == request.EffectiveDate &&
                    x.Status != -1 &&
                    x.Id != request.Id);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Đơn giá đã tồn tại cho trạm/hạng/ngày hiệu lực này!");

                var data = await _context.RubberUnitPrices.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.RubberUnitPrices
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_RubberUnitPrice>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_RubberUnitPrice>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái đơn giá mủ (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_RubberUnitPrice>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.RubberUnitPrices.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_RubberUnitPrice>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_RubberUnitPrice>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm đơn giá mủ (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.RubberUnitPrices
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
        /// Lấy chi tiết đơn giá mủ theo ID
        /// </summary>
        public async Task<ResponseData<MRes_RubberUnitPrice>> GetById(int id)
        {
            try
            {
                var data = await _context.RubberUnitPrices
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_RubberUnitPrice>
                {
                    data = _mapper.Map<MRes_RubberUnitPrice>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy đơn giá hiện hành theo trạm và hạng
        /// </summary>
        public async Task<ResponseData<MRes_RubberUnitPrice>> GetCurrentPrice(int tramId, string grade, DateTime? effectiveDate = null)
        {
            try
            {
                var date = effectiveDate ?? DateTime.UtcNow;

                var data = await _context.RubberUnitPrices
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .Where(x => x.TramId == tramId &&
                                x.Grade == grade.ToUpper() &&
                                x.EffectiveDate <= date &&
                                x.Status != -1)
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (data == null)
                    return Error(HttpStatusCode.NotFound, $"Không tìm thấy đơn giá cho trạm {tramId}, hạng {grade}!");

                return new ResponseData<MRes_RubberUnitPrice>
                {
                    data = _mapper.Map<MRes_RubberUnitPrice>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetCurrentPrice), new { tramId, grade, effectiveDate });
            }
        }

        /// <summary>
        /// Lấy danh sách đơn giá mủ có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_RubberUnitPrice>>> GetListByPaging(MReq_RubberUnitPrice_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_RubberUnitPrice> data = new List<MRes_RubberUnitPrice>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.TramId)
                        .ThenBy(x => x.Grade)
                        .ThenByDescending(x => x.EffectiveDate)
                        .ProjectTo<MRes_RubberUnitPrice>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_RubberUnitPrice>>
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
        /// Lấy danh sách đơn giá mủ theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_RubberUnitPrice>>> GetListByFullParam(MReq_RubberUnitPrice_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.TramId).ThenBy(x => x.Grade).ThenByDescending(x => x.EffectiveDate)
                    .ProjectTo<MRes_RubberUnitPrice>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_RubberUnitPrice>>
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
        private IQueryable<RubberUnitPrice> BuildFilterQuery(MReq_RubberUnitPrice_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.RubberUnitPrices.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.TramId.HasValue)
                query = query.Where(x => x.TramId == request.TramId);

            if (!string.IsNullOrWhiteSpace(request.Grade))
                query = query.Where(x => x.Grade == request.Grade.ToUpper());

            if (request.EffectiveDate.HasValue)
                query = query.Where(x => x.EffectiveDate == request.EffectiveDate);

            return query;
        }
        #endregion
    }
}
