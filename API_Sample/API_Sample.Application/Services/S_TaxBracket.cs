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
    public interface IS_TaxBracket
    {
        Task<ResponseData<MRes_TaxBracket>> Create(MReq_TaxBracket request);
        Task<ResponseData<MRes_TaxBracket>> Update(MReq_TaxBracket request);
        Task<ResponseData<MRes_TaxBracket>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_TaxBracket>> GetById(int id);
        Task<ResponseData<List<MRes_TaxBracket>>> GetListByPaging(MReq_TaxBracket_FullParam request);
        Task<ResponseData<List<MRes_TaxBracket>>> GetListByFullParam(MReq_TaxBracket_FullParam request);
    }

    /// <summary>
    /// Quản lý biểu thuế TNCN theo bậc lũy tiến
    /// </summary>
    public class S_TaxBracket : BaseService<S_TaxBracket>, IS_TaxBracket
    {
        private readonly IMapper _mapper;

        public S_TaxBracket(MainDbContext context, IMapper mapper, ILogger<S_TaxBracket> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới bậc thuế
        /// </summary>
        public async Task<ResponseData<MRes_TaxBracket>> Create(MReq_TaxBracket request)
        {
            try
            {
                var validationError = await ValidateTaxBracket(request);
                if (validationError != null)
                    return Error(HttpStatusCode.BadRequest, validationError);

                var data = _mapper.Map<TaxBracket>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.TaxBrackets.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_TaxBracket>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TaxBracket>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật bậc thuế
        /// </summary>
        public async Task<ResponseData<MRes_TaxBracket>> Update(MReq_TaxBracket request)
        {
            try
            {
                var data = await _context.TaxBrackets.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                var validationError = await ValidateTaxBracket(request, request.Id);
                if (validationError != null)
                    return Error(HttpStatusCode.BadRequest, validationError);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_TaxBracket>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TaxBracket>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái bậc thuế
        /// </summary>
        public async Task<ResponseData<MRes_TaxBracket>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.TaxBrackets.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_TaxBracket>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TaxBracket>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm bậc thuế
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.TaxBrackets
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
        /// Lấy chi tiết bậc thuế theo ID
        /// </summary>
        public async Task<ResponseData<MRes_TaxBracket>> GetById(int id)
        {
            try
            {
                var data = await _context.TaxBrackets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_TaxBracket>
                {
                    data = _mapper.Map<MRes_TaxBracket>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách bậc thuế có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_TaxBracket>>> GetListByPaging(MReq_TaxBracket_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_TaxBracket> data = new List<MRes_TaxBracket>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.SortOrder)
                        .ProjectTo<MRes_TaxBracket>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_TaxBracket>>
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
        /// Lấy danh sách bậc thuế theo bộ lọc
        /// </summary>
        public async Task<ResponseData<List<MRes_TaxBracket>>> GetListByFullParam(MReq_TaxBracket_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.SortOrder)
                    .ProjectTo<MRes_TaxBracket>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_TaxBracket>>
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
        /// <summary>
        /// Validate bậc thuế trước khi Create/Update
        /// </summary>
        private async Task<string?> ValidateTaxBracket(MReq_TaxBracket request, int? excludeId = null)
        {
            // 1. FromAmount < ToAmount (nếu ToAmount có giá trị)
            if (request.ToAmount.HasValue && request.FromAmount >= request.ToAmount.Value)
                return "Ngưỡng từ (FromAmount) phải nhỏ hơn ngưỡng đến (ToAmount)";

            // 2. Không overlap với bracket khác trong cùng khoảng thời gian hiệu lực
            var overlapping = await _context.TaxBrackets
                .AsNoTracking()
                .Where(x => x.Status == 1
                            && (excludeId == null || x.Id != excludeId)
                            // Overlap thời gian
                            && x.EffectiveDate <= (request.EndDate ?? DateTime.MaxValue)
                            && (x.EndDate == null || x.EndDate >= request.EffectiveDate)
                            // Overlap khoảng tiền
                            && x.FromAmount < (request.ToAmount ?? decimal.MaxValue)
                            && (x.ToAmount == null || x.ToAmount > request.FromAmount))
                .AnyAsync();

            if (overlapping)
                return "Khoảng thuế bị trùng với bậc thuế khác trong cùng thời gian hiệu lực";

            return null;
        }

        private IQueryable<TaxBracket> BuildFilterQuery(MReq_TaxBracket_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.TaxBrackets.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.EffectiveDate.HasValue)
                query = query.Where(x => x.EffectiveDate <= request.EffectiveDate.Value
                                         && (x.EndDate == null || x.EndDate >= request.EffectiveDate.Value));

            return query;
        }
        #endregion
    }
}
