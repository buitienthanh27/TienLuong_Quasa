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
    public interface IS_SystemParameter
    {
        Task<ResponseData<MRes_SystemParameter>> Create(MReq_SystemParameter request);
        Task<ResponseData<MRes_SystemParameter>> Update(MReq_SystemParameter request);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_SystemParameter>> GetById(int id);
        Task<ResponseData<List<MRes_SystemParameter>>> GetListByPaging(MReq_SystemParameter_FullParam request);
        Task<ResponseData<List<MRes_SystemParameter>>> GetListByFullParam(MReq_SystemParameter_FullParam request);
        Task<decimal> GetParamValue(string paramCode, DateTime effectiveDate);
    }

    /// <summary>
    /// Quản lý tham số hệ thống (P7, DRC tạm ứng, hệ số công...)
    /// </summary>
    public class S_SystemParameter : BaseService<S_SystemParameter>, IS_SystemParameter
    {
        private readonly IMapper _mapper;

        public S_SystemParameter(MainDbContext context, IMapper mapper, ILogger<S_SystemParameter> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới tham số hệ thống với ngày hiệu lực
        /// </summary>
        public async Task<ResponseData<MRes_SystemParameter>> Create(MReq_SystemParameter request)
        {
            try
            {
                request.ParamCode = request.ParamCode?.Trim().ToUpper();
                var isExists = await _context.SystemParameters.AnyAsync(x =>
                    x.ParamCode == request.ParamCode &&
                    x.EffectiveDate == request.EffectiveDate &&
                    x.Status != -1);
                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Tham số với ngày hiệu lực này đã tồn tại!");

                var data = _mapper.Map<SystemParameter>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.SystemParameters.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_SystemParameter>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_SystemParameter>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật tham số hệ thống
        /// </summary>
        public async Task<ResponseData<MRes_SystemParameter>> Update(MReq_SystemParameter request)
        {
            try
            {
                request.ParamCode = request.ParamCode?.Trim().ToUpper();
                var data = await _context.SystemParameters.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_SystemParameter>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_SystemParameter>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Xóa cứng tham số hệ thống
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.SystemParameters.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        /// Lấy chi tiết tham số hệ thống theo ID
        /// </summary>
        public async Task<ResponseData<MRes_SystemParameter>> GetById(int id)
        {
            try
            {
                var data = await _context.SystemParameters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_SystemParameter>
                {
                    data = _mapper.Map<MRes_SystemParameter>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách tham số hệ thống có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_SystemParameter>>> GetListByPaging(MReq_SystemParameter_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_SystemParameter> data = new List<MRes_SystemParameter>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.ParamCode)
                        .ThenByDescending(x => x.EffectiveDate)
                        .ProjectTo<MRes_SystemParameter>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_SystemParameter>>
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
        /// Lấy danh sách tham số hệ thống theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_SystemParameter>>> GetListByFullParam(MReq_SystemParameter_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.ParamCode).ThenByDescending(x => x.EffectiveDate)
                    .ProjectTo<MRes_SystemParameter>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_SystemParameter>>
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
        /// Lấy giá trị tham số theo mã và ngày hiệu lực
        /// </summary>
        public async Task<decimal> GetParamValue(string paramCode, DateTime effectiveDate)
        {
            var param = await _context.SystemParameters.AsNoTracking()
                .Where(x => x.ParamCode == paramCode && x.EffectiveDate <= effectiveDate && x.Status != -1)
                .OrderByDescending(x => x.EffectiveDate)
                .FirstOrDefaultAsync();

            return param?.ParamValue ?? 0;
        }

        #region Common functions
        private IQueryable<SystemParameter> BuildFilterQuery(MReq_SystemParameter_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.SystemParameters.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchText = request.SearchText.Trim();
                query = query.Where(x => x.ParamCode.Contains(searchText) || x.ParamName.Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(request.ParamCode))
                query = query.Where(x => x.ParamCode == request.ParamCode);

            if (request.EffectiveDate.HasValue)
                query = query.Where(x => x.EffectiveDate <= request.EffectiveDate.Value);

            return query;
        }
        #endregion
    }
}
