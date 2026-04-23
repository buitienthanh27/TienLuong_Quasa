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
    public interface IS_Tram
    {
        Task<ResponseData<MRes_Tram>> Create(MReq_Tram request);
        Task<ResponseData<MRes_Tram>> Update(MReq_Tram request);
        Task<ResponseData<MRes_Tram>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<List<MRes_Tram>>> UpdateStatusList(string sequenceIds, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Tram>> GetById(int id);
        Task<ResponseData<List<MRes_Tram>>> GetListByPaging(MReq_Tram_FullParam request);
        Task<ResponseData<List<MRes_Tram>>> GetListByFullParam(MReq_Tram_FullParam request);
    }

    /// <summary>
    /// Quản lý trạm cao su (đơn vị thu mua mủ)
    /// </summary>
    public class S_Tram : BaseService<S_Tram>, IS_Tram
    {
        private readonly IMapper _mapper;

        public S_Tram(MainDbContext context, IMapper mapper, ILogger<S_Tram> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới trạm cao su
        /// </summary>
        public async Task<ResponseData<MRes_Tram>> Create(MReq_Tram request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();
                var isExistsCode = await _context.Trams.AnyAsync(x => x.Code == request.Code && x.Status != -1);
                if (isExistsCode)
                    return Error(HttpStatusCode.Conflict, "Mã trạm đã tồn tại!");

                var data = _mapper.Map<Tram>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.Trams.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_Tram>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Tram>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật thông tin trạm cao su
        /// </summary>
        public async Task<ResponseData<MRes_Tram>> Update(MReq_Tram request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();
                var isExistsCode = await _context.Trams.AnyAsync(x => x.Code == request.Code && x.Status != -1 && x.Id != request.Id);
                if (isExistsCode)
                    return Error(HttpStatusCode.Conflict, "Mã trạm đã tồn tại!");

                var data = await _context.Trams.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Tram>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Tram>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái trạm (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_Tram>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.Trams.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Tram>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Tram>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái nhiều trạm cùng lúc
        /// </summary>
        public async Task<ResponseData<List<MRes_Tram>>> UpdateStatusList(string sequenceIds, short status, int updatedBy)
        {
            try
            {
                List<int> ids = JsonConvert.DeserializeObject<List<int>>(sequenceIds);
                if (ids == null || !ids.Any())
                    return Error(HttpStatusCode.BadRequest, MessageErrorConstants.DO_NOT_FIND_DATA);

                var now = DateTime.UtcNow;
                var updatedCount = await _context.Trams
                    .Where(x => ids.Contains(x.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Status, status)
                        .SetProperty(p => p.UpdatedBy, updatedBy)
                        .SetProperty(p => p.UpdatedAt, now)
                    );

                if (updatedCount == 0)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                var datas = await _context.Trams.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();

                return new ResponseData<List<MRes_Tram>>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<List<MRes_Tram>>(datas)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatusList), new { sequenceIds, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa cứng trạm (chỉ khi không có nhân viên liên kết)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var hasEmployees = await _context.Employees.AnyAsync(x => x.TramId == id && x.Status != -1);
                if (hasEmployees)
                    return Error(HttpStatusCode.BadRequest, "Không thể xoá trạm đang có nhân viên!");

                var deletedCount = await _context.Trams.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        /// Lấy chi tiết trạm theo ID
        /// </summary>
        public async Task<ResponseData<MRes_Tram>> GetById(int id)
        {
            try
            {
                var data = await _context.Trams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_Tram>
                {
                    data = _mapper.Map<MRes_Tram>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách trạm có phân trang (tương thích SQL Server 2008 R2)
        /// </summary>
        public async Task<ResponseData<List<MRes_Tram>>> GetListByPaging(MReq_Tram_FullParam request)
        {
            try
            {
                int page = request.Page > 0 ? request.Page : 1;
                int record = request.Record > 0 ? request.Record : 10;

                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Tram> data = new List<MRes_Tram>();

                if (count > 0)
                {
                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Code)
                        .ProjectTo<MRes_Tram>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_Tram>>
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
        /// Lấy danh sách trạm theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_Tram>>> GetListByFullParam(MReq_Tram_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Code)
                    .ProjectTo<MRes_Tram>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Tram>>
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
        private IQueryable<Tram> BuildFilterQuery(MReq_Tram_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.Trams.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchTextUpper = request.SearchText.ToUpper().Trim();
                query = query.Where(x => x.Code.Contains(searchTextUpper) || x.Name.Contains(request.SearchText));
            }

            return query;
        }
        #endregion
    }
}
