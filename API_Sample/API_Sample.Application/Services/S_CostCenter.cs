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
    public interface IS_CostCenter
    {
        Task<ResponseData<MRes_CostCenter>> Create(MReq_CostCenter request);
        Task<ResponseData<MRes_CostCenter>> Update(MReq_CostCenter request);
        Task<ResponseData<MRes_CostCenter>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_CostCenter>> GetById(int id);
        Task<ResponseData<List<MRes_CostCenter>>> GetListByPaging(MReq_CostCenter_FullParam request);
        Task<ResponseData<List<MRes_CostCenter>>> GetListByFullParam(MReq_CostCenter_FullParam request);
    }

    /// <summary>
    /// Quản lý trung tâm chi phí (phân bổ lương theo dự án/bộ phận)
    /// </summary>
    public class S_CostCenter : BaseService<S_CostCenter>, IS_CostCenter
    {
        private readonly IMapper _mapper;

        public S_CostCenter(MainDbContext context, IMapper mapper, ILogger<S_CostCenter> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới trung tâm chi phí
        /// </summary>
        public async Task<ResponseData<MRes_CostCenter>> Create(MReq_CostCenter request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();
                var isExists = await _context.CostCenters.AnyAsync(x => x.Code == request.Code && x.Status != -1);
                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Mã trung tâm chi phí đã tồn tại!");

                var data = _mapper.Map<CostCenter>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.CostCenters.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_CostCenter>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_CostCenter>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật trung tâm chi phí
        /// </summary>
        public async Task<ResponseData<MRes_CostCenter>> Update(MReq_CostCenter request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();
                var isExists = await _context.CostCenters.AnyAsync(x => x.Code == request.Code && x.Status != -1 && x.Id != request.Id);
                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Mã trung tâm chi phí đã tồn tại!");

                var data = await _context.CostCenters.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_CostCenter>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_CostCenter>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái trung tâm chi phí (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_CostCenter>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.CostCenters.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_CostCenter>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_CostCenter>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa cứng trung tâm chi phí (chỉ khi không có phân bổ liên kết)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var hasAllocations = await _context.CostAllocations.AnyAsync(x => x.CostCenterId == id);
                if (hasAllocations)
                    return Error(HttpStatusCode.BadRequest, "Không thể xoá trung tâm chi phí đang có phân bổ!");

                var deletedCount = await _context.CostCenters.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        /// Lấy chi tiết trung tâm chi phí theo ID
        /// </summary>
        public async Task<ResponseData<MRes_CostCenter>> GetById(int id)
        {
            try
            {
                var data = await _context.CostCenters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_CostCenter>
                {
                    data = _mapper.Map<MRes_CostCenter>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách trung tâm chi phí có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_CostCenter>>> GetListByPaging(MReq_CostCenter_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_CostCenter> data = new List<MRes_CostCenter>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Code)
                        .ProjectTo<MRes_CostCenter>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_CostCenter>>
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
        /// Lấy danh sách trung tâm chi phí theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_CostCenter>>> GetListByFullParam(MReq_CostCenter_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Code)
                    .ProjectTo<MRes_CostCenter>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_CostCenter>>
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
        private IQueryable<CostCenter> BuildFilterQuery(MReq_CostCenter_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.CostCenters.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchText = request.SearchText.Trim();
                query = query.Where(x => x.Code.Contains(searchText) || x.Name.Contains(searchText));
            }

            return query;
        }
        #endregion
    }
}
