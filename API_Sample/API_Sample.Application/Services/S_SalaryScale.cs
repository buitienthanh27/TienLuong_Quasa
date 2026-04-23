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
    public interface IS_SalaryScale
    {
        Task<ResponseData<MRes_SalaryScale>> Create(MReq_SalaryScale request);
        Task<ResponseData<MRes_SalaryScale>> Update(MReq_SalaryScale request);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_SalaryScale>> GetById(int id);
        Task<ResponseData<List<MRes_SalaryScale>>> GetListByPaging(MReq_SalaryScale_FullParam request);
        Task<ResponseData<List<MRes_SalaryScale>>> GetListByFullParam(MReq_SalaryScale_FullParam request);
    }

    /// <summary>
    /// Quản lý bảng lương theo trạm và hạng kỹ thuật
    /// </summary>
    public class S_SalaryScale : BaseService<S_SalaryScale>, IS_SalaryScale
    {
        private readonly IMapper _mapper;

        public S_SalaryScale(MainDbContext context, IMapper mapper, ILogger<S_SalaryScale> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới bảng lương theo trạm, hạng và ngày hiệu lực
        /// </summary>
        public async Task<ResponseData<MRes_SalaryScale>> Create(MReq_SalaryScale request)
        {
            try
            {
                request.Grade = request.Grade?.Trim().ToUpper();
                var isExists = await _context.SalaryScales.AnyAsync(x =>
                    x.TramId == request.TramId &&
                    x.Grade == request.Grade &&
                    x.EffectiveDate == request.EffectiveDate &&
                    x.Status != -1);
                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Bảng lương này đã tồn tại!");

                var data = _mapper.Map<SalaryScale>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.SalaryScales.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                await _context.Entry(data).Reference(x => x.Tram).LoadAsync();

                return new ResponseData<MRes_SalaryScale>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_SalaryScale>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật bảng lương
        /// </summary>
        public async Task<ResponseData<MRes_SalaryScale>> Update(MReq_SalaryScale request)
        {
            try
            {
                var data = await _context.SalaryScales.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_SalaryScale>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_SalaryScale>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Xóa cứng bảng lương
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.SalaryScales.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        /// Lấy chi tiết bảng lương theo ID
        /// </summary>
        public async Task<ResponseData<MRes_SalaryScale>> GetById(int id)
        {
            try
            {
                var data = await _context.SalaryScales.AsNoTracking()
                    .Include(x => x.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_SalaryScale>
                {
                    data = _mapper.Map<MRes_SalaryScale>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách bảng lương có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_SalaryScale>>> GetListByPaging(MReq_SalaryScale_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_SalaryScale> data = new List<MRes_SalaryScale>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.TramId)
                        .ThenBy(x => x.Grade)
                        .ThenByDescending(x => x.EffectiveDate)
                        .ProjectTo<MRes_SalaryScale>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_SalaryScale>>
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
        /// Lấy danh sách bảng lương theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_SalaryScale>>> GetListByFullParam(MReq_SalaryScale_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.TramId).ThenBy(x => x.Grade).ThenByDescending(x => x.EffectiveDate)
                    .ProjectTo<MRes_SalaryScale>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_SalaryScale>>
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
        private IQueryable<SalaryScale> BuildFilterQuery(MReq_SalaryScale_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.SalaryScales.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (request.TramId.HasValue)
                query = query.Where(x => x.TramId == request.TramId.Value);

            if (!string.IsNullOrWhiteSpace(request.Grade))
                query = query.Where(x => x.Grade == request.Grade);

            if (request.EffectiveDate.HasValue)
                query = query.Where(x => x.EffectiveDate <= request.EffectiveDate.Value);

            return query;
        }
        #endregion
    }
}
