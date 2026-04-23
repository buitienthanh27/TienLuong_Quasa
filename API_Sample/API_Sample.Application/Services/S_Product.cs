using API_Sample.Application.Ultilities;
using API_Sample.Data.EF;
using API_Sample.Data.Entities;
using API_Sample.Models.Common;
using API_Sample.Models.Request;
using API_Sample.Models.Response;
using API_Sample.Utilities;
using API_Sample.Utilities.Constants;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace API_Sample.Application.Services
{
    public interface IS_Product
    {
        Task<ResponseData<MRes_Product>> Create(MReq_Product request);
        Task<ResponseData<MRes_Product>> Update(MReq_Product request);
        Task<ResponseData<MRes_Product>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<List<MRes_Product>>> UpdateStatusList(string sequenceIds, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Product>> GetById(int id);
        Task<ResponseData<List<MRes_Product>>> GetListByPaging(MReq_Product_FullParam request);
        Task<ResponseData<List<MRes_Product>>> GetListByFullParam(MReq_Product_FullParam request);
        Task<ResponseData<List<MRes_Product>>> GetListBySpFullParam(MReq_Product_FullParam request);
    }

    public class S_Product : BaseService<S_Product>, IS_Product
    {
        private readonly IMapper _mapper;

        public S_Product(MainDbContext context, IMapper mapper, ILogger<S_Product> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Hàm tạo mới bản ghi, thường dùng cho các trường hợp thêm mới dữ liệu, cần quan tâm đến việc kiểm tra dữ liệu đầu vào
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Product>> Create(MReq_Product request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();
                var isExistsCode = await _context.Products.AnyAsync(x => x.Code == request.Code && x.Status != -1);
                if (isExistsCode)
                    return Error(HttpStatusCode.Conflict, "Mã trùng lặp!");

                var data = _mapper.Map<Product>(request);
                var maxId = await _context.Products.MaxAsync(x => (int?)x.Id) ?? 0;
                data.Id = maxId + 1;
                data.NameSlug = StringHelper.ToUrlClean(request.Name);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.Products.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_Product>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Product>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Hàm cập nhật dữ liệu cho 1 bản ghi
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Product>> Update(MReq_Product request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();
                var isExistsCode = await _context.Products.AnyAsync(x => x.Code == request.Code && x.Status != -1 && x.Id != request.Id);
                if (isExistsCode)
                    return Error(HttpStatusCode.Conflict, "Mã trùng lặp!");

                var data = await _context.Products.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.NameSlug = StringHelper.ToUrlClean(request.Name);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Product>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Product>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái cho 1 bản ghi, thường dùng để thay đổi trạng thái kích hoạt, trạng thái xóa mềm,... mà không cần quan tâm đến các trường dữ liệu khác, hoặc khi chỉ cần thay đổi trạng thái mà không cần lấy lại dữ liệu sau khi cập nhật
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Product>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.Products.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                {
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);
                }

                return new ResponseData<MRes_Product>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Product>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái cho nhiều bản ghi cùng lúc
        /// </summary>
        /// <param name="sequenceIds"></param>
        /// <param name="status"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Product>>> UpdateStatusList(string sequenceIds, short status, int updatedBy)
        {
            try
            {
                List<int> ids = JsonConvert.DeserializeObject<List<int>>(sequenceIds);
                if (ids == null || !ids.Any())
                {
                    return Error(HttpStatusCode.BadRequest, MessageErrorConstants.DO_NOT_FIND_DATA);
                }

                var now = DateTime.UtcNow;
                var updatedCount = await _context.Products
                    .Where(x => ids.Contains(x.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Status, status)
                        .SetProperty(p => p.UpdatedBy, updatedBy)
                        .SetProperty(p => p.UpdatedAt, now)
                    );

                if (updatedCount == 0)
                {
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);
                }

                //Trả về danh sách đã được cập nhật trạng thái, nếu không cần thiết có thể bỏ qua bước này để tối ưu hiệu năng
                var datas = await _context.Products.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();

                return new ResponseData<List<MRes_Product>>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<List<MRes_Product>>(datas)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatusList), new { sequenceIds, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa cứng, chỉ xóa khi không còn ràng buộc dữ liệu với bảng khác (thường hiếm khi sử dụng)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.Products.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        /// Lấy chỉ tiết bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Product>> GetById(int id)
        {
            try
            {
                var data = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_Product>
                {
                    data = _mapper.Map<MRes_Product>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách theo tham số đầy đủ, có phân trang, dùng cho các trường hợp cần lấy dữ liệu theo điều kiện lọc và cần quan tâm đến phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Product>>> GetListByPaging(MReq_Product_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Product> data = new List<MRes_Product>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Sort)
                        .ProjectTo<MRes_Product>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_Product>>
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
        /// Lấy danh sách theo tham số đầy đủ, không phân trang, dùng cho các trường hợp cần lấy hết dữ liệu theo điều kiện lọc, hoặc chỉ lấy 1 vài bản ghi theo điều kiện lọc mà không cần quan tâm đến phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Product>>> GetListByFullParam(MReq_Product_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Sort)
                    .ProjectTo<MRes_Product>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Product>>
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
        /// Lấy danh sách theo tham số đầy đủ bằng stored procedure
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Product>>> GetListBySpFullParam(MReq_Product_FullParam request)
        {
            try
            {
                string[] arrParams = {
                    "@SequenceStatus",
                    "@SearchText",
                    "@Page",
                    "@Record"
                };
                object[] arrValues = {
                    request.SequenceStatus,
                    request.SearchText,
                    request.Page,
                    request.Record
                };
                var callSp = await StoreProcedure.GetListAsync<MRes_Product>(_context.Database.GetConnectionString(), "sp_product_getlist_by_fullparam", arrParams, arrValues);
                return new ResponseData<List<MRes_Product>>
                {
                    data = callSp,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListBySpFullParam), request);
            }
        }

        #region Common functions
        // Hàm dùng chung để Build điều kiện Filter (cần thiết đưa riêng khi muốn tái sử dụng logic lọc)
        private IQueryable<Product> BuildFilterQuery(MReq_Product_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.Products.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchTextClean = StringHelper.ToUrlClean(request.SearchText);
                var searchTextUpper = request.SearchText.ToUpper().Trim();
                query = query.Where(x => x.NameSlug.StartsWith(searchTextClean) || x.Code.StartsWith(searchTextUpper));
            }

            return query;
        }
        #endregion
    }
}