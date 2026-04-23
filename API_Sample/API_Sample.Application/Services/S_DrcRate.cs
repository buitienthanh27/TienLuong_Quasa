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
    public interface IS_DrcRate
    {
        Task<ResponseData<MRes_DrcRate>> Create(MReq_DrcRate request);
        Task<ResponseData<MRes_DrcRate>> Update(MReq_DrcRate request);
        Task<ResponseData<int>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id, int updatedBy);
        Task<ResponseData<MRes_DrcRate>> GetById(int id);
        Task<ResponseData<List<MRes_DrcRate>>> GetListByPaging(MReq_DrcRate_FullParam request);
        Task<ResponseData<List<MRes_DrcRate>>> GetListByFullParam(MReq_DrcRate_FullParam request);
        Task<ResponseData<MRes_DrcRate>> GetByTramAndMonth(int? tramId, string yearMonth);
    }

    /// <summary>
    /// Service quản lý tỷ lệ DRC (Dry Rubber Content) theo tháng và trạm
    /// </summary>
    public class S_DrcRate : BaseService<S_DrcRate>, IS_DrcRate
    {
        private readonly IMapper _mapper;

        public S_DrcRate(MainDbContext context, IMapper mapper, ILogger<S_DrcRate> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới tỷ lệ DRC cho trạm theo tháng
        /// </summary>
        public async Task<ResponseData<MRes_DrcRate>> Create(MReq_DrcRate request)
        {
            try
            {
                if (await _context.DrcRates.AnyAsync(x =>
                    x.TramId == request.TramId &&
                    x.YearMonth == request.YearMonth &&
                    x.Status != -1))
                    return Error(HttpStatusCode.Conflict, "Tỷ lệ DRC tháng này đã tồn tại cho trạm!");

                var data = _mapper.Map<DrcRate>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.DrcRates.Add(data);
                if (await _context.SaveChangesAsync() == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.DrcRates
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_DrcRate>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_DrcRate>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật tỷ lệ DRC
        /// </summary>
        public async Task<ResponseData<MRes_DrcRate>> Update(MReq_DrcRate request)
        {
            try
            {
                var data = await _context.DrcRates.FindAsync(request.Id);
                if (data == null || data.Status == -1)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.DrcRawLatex = request.DrcRawLatex;
                data.DrcReference = request.DrcReference;
                data.DrcSerum = request.DrcSerum;
                data.DrcRope = request.DrcRope;
                data.Note = request.Note;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                if (await _context.SaveChangesAsync() == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.DrcRates
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_DrcRate>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_DrcRate>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái tỷ lệ DRC (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<int>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var updated = await _context.DrcRates
                    .Where(x => x.Id == id && x.Status != -1)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.Status, status)
                        .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                        .SetProperty(x => x.UpdatedBy, updatedBy));

                if (updated == 0)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<int>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS) { data = updated };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm tỷ lệ DRC (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id, int updatedBy)
        {
            return await UpdateStatus(id, -1, updatedBy);
        }

        /// <summary>
        /// Lấy chi tiết tỷ lệ DRC theo ID
        /// </summary>
        public async Task<ResponseData<MRes_DrcRate>> GetById(int id)
        {
            try
            {
                var data = await _context.DrcRates
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status != -1);

                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<MRes_DrcRate>(1, (int)HttpStatusCode.OK, "")
                {
                    data = _mapper.Map<MRes_DrcRate>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), id);
            }
        }

        /// <summary>
        /// Lấy tỷ lệ DRC theo trạm và tháng
        /// </summary>
        public async Task<ResponseData<MRes_DrcRate>> GetByTramAndMonth(int? tramId, string yearMonth)
        {
            try
            {
                var data = await _context.DrcRates
                    .AsNoTracking()
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.TramId == tramId && x.YearMonth == yearMonth && x.Status != -1);

                if (data == null)
                    return Error(HttpStatusCode.NotFound, "Chưa có tỷ lệ DRC cho trạm và tháng này!");

                return new ResponseData<MRes_DrcRate>(1, (int)HttpStatusCode.OK, "")
                {
                    data = _mapper.Map<MRes_DrcRate>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByTramAndMonth), new { tramId, yearMonth });
            }
        }

        /// <summary>
        /// Lấy danh sách tỷ lệ DRC có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_DrcRate>>> GetListByPaging(MReq_DrcRate_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);
                var total = await query.CountAsync();

                int page = request.Page > 0 ? request.Page : 1;
                int record = request.Record > 0 ? request.Record : 10;

                // SQL Server 2008 R2: fetch all rồi paging in-memory
                var allData = await query
                    .OrderByDescending(x => x.YearMonth)
                    .ThenBy(x => x.TramId)
                    .ProjectTo<MRes_DrcRate>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                var list = allData.Skip((page - 1) * record).Take(record).ToList();

                return new ResponseData<List<MRes_DrcRate>>(1, (int)HttpStatusCode.OK, "")
                {
                    data = list,
                    data2nd = total
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByPaging), request);
            }
        }

        /// <summary>
        /// Lấy danh sách tỷ lệ DRC theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_DrcRate>>> GetListByFullParam(MReq_DrcRate_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);
                var list = await query
                    .OrderByDescending(x => x.YearMonth)
                    .ThenBy(x => x.TramId)
                    .ProjectTo<MRes_DrcRate>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_DrcRate>>(1, (int)HttpStatusCode.OK, "")
                {
                    data = list,
                    data2nd = list.Count
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByFullParam), request);
            }
        }

        #region Common functions
        private IQueryable<DrcRate> BuildFilterQuery(MReq_DrcRate_FullParam request)
        {
            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.DrcRates
                .AsNoTracking()
                .Where(x => x.Status != -1);

            if (!string.IsNullOrEmpty(request.SequenceStatus))
            {
                var statusList = request.SequenceStatus.Split(',').Select(short.Parse).ToArray();
                query = query.Where(x => statusList.Contains(x.Status));
            }

            if (request.TramId.HasValue)
                query = query.Where(x => x.TramId == request.TramId.Value);

            if (!string.IsNullOrEmpty(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            return query;
        }
        #endregion
    }
}
