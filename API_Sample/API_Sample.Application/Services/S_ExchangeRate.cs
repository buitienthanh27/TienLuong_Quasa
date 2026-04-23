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
    public interface IS_ExchangeRate
    {
        Task<ResponseData<MRes_ExchangeRate>> Create(MReq_ExchangeRate request);
        Task<ResponseData<MRes_ExchangeRate>> Update(MReq_ExchangeRate request);
        Task<ResponseData<MRes_ExchangeRate>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_ExchangeRate>> GetById(int id);
        Task<ResponseData<List<MRes_ExchangeRate>>> GetListByPaging(MReq_ExchangeRate_FullParam request);
        Task<ResponseData<List<MRes_ExchangeRate>>> GetListByFullParam(MReq_ExchangeRate_FullParam request);
        Task<ResponseData<MRes_ExchangeRate>> GetRateByMonth(string yearMonth, string fromCurrency, string toCurrency);
        Task<ResponseData<MRes_ExchangeRate>> Approve(int id, int approvedBy);
    }

    /// <summary>
    /// Quản lý tỷ giá hàng tháng (Vietinbank, P.Tổ chức xác nhận)
    /// </summary>
    public class S_ExchangeRate : BaseService<S_ExchangeRate>, IS_ExchangeRate
    {
        private readonly IMapper _mapper;

        public S_ExchangeRate(MainDbContext context, IMapper mapper, ILogger<S_ExchangeRate> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới tỷ giá tháng
        /// </summary>
        public async Task<ResponseData<MRes_ExchangeRate>> Create(MReq_ExchangeRate request)
        {
            try
            {
                request.FromCurrency = request.FromCurrency?.Trim().ToUpper();
                request.ToCurrency = request.ToCurrency?.Trim().ToUpper();

                var isExists = await _context.ExchangeRates.AnyAsync(x =>
                    x.YearMonth == request.YearMonth &&
                    x.FromCurrency == request.FromCurrency &&
                    x.ToCurrency == request.ToCurrency &&
                    x.Status != -1);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Tỷ giá đã tồn tại cho tháng/cặp tiền tệ này!");

                var data = _mapper.Map<ExchangeRate>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.ExchangeRates.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_ExchangeRate>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_ExchangeRate>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật tỷ giá
        /// </summary>
        public async Task<ResponseData<MRes_ExchangeRate>> Update(MReq_ExchangeRate request)
        {
            try
            {
                request.FromCurrency = request.FromCurrency?.Trim().ToUpper();
                request.ToCurrency = request.ToCurrency?.Trim().ToUpper();

                var isExists = await _context.ExchangeRates.AnyAsync(x =>
                    x.YearMonth == request.YearMonth &&
                    x.FromCurrency == request.FromCurrency &&
                    x.ToCurrency == request.ToCurrency &&
                    x.Status != -1 &&
                    x.Id != request.Id);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Tỷ giá đã tồn tại cho tháng/cặp tiền tệ này!");

                var data = await _context.ExchangeRates.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_ExchangeRate>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_ExchangeRate>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// P.Tổ chức xác nhận tỷ giá
        /// </summary>
        public async Task<ResponseData<MRes_ExchangeRate>> Approve(int id, int approvedBy)
        {
            try
            {
                var data = await _context.ExchangeRates.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.ApprovedBy = approvedBy;
                data.ApprovedAt = DateTime.UtcNow;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = approvedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_ExchangeRate>(1, (int)HttpStatusCode.OK, "Xác nhận tỷ giá thành công!")
                {
                    data = _mapper.Map<MRes_ExchangeRate>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Approve), new { id, approvedBy });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái tỷ giá (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_ExchangeRate>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.ExchangeRates.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_ExchangeRate>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_ExchangeRate>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm tỷ giá (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.ExchangeRates
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
        /// Lấy chi tiết tỷ giá theo ID
        /// </summary>
        public async Task<ResponseData<MRes_ExchangeRate>> GetById(int id)
        {
            try
            {
                var data = await _context.ExchangeRates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_ExchangeRate>
                {
                    data = _mapper.Map<MRes_ExchangeRate>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy tỷ giá theo tháng và cặp tiền tệ
        /// </summary>
        public async Task<ResponseData<MRes_ExchangeRate>> GetRateByMonth(string yearMonth, string fromCurrency, string toCurrency)
        {
            try
            {
                var data = await _context.ExchangeRates
                    .AsNoTracking()
                    .Where(x => x.YearMonth == yearMonth &&
                                x.FromCurrency == fromCurrency.ToUpper() &&
                                x.ToCurrency == toCurrency.ToUpper() &&
                                x.Status != -1)
                    .FirstOrDefaultAsync();

                if (data == null)
                    return Error(HttpStatusCode.NotFound, $"Không tìm thấy tỷ giá {fromCurrency}/{toCurrency} cho tháng {yearMonth}!");

                return new ResponseData<MRes_ExchangeRate>
                {
                    data = _mapper.Map<MRes_ExchangeRate>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetRateByMonth), new { yearMonth, fromCurrency, toCurrency });
            }
        }

        /// <summary>
        /// Lấy danh sách tỷ giá có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_ExchangeRate>>> GetListByPaging(MReq_ExchangeRate_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_ExchangeRate> data = new List<MRes_ExchangeRate>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderByDescending(x => x.YearMonth)
                        .ThenBy(x => x.FromCurrency)
                        .ProjectTo<MRes_ExchangeRate>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_ExchangeRate>>
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
        /// Lấy danh sách tỷ giá theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_ExchangeRate>>> GetListByFullParam(MReq_ExchangeRate_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderByDescending(x => x.YearMonth).ThenBy(x => x.FromCurrency)
                    .ProjectTo<MRes_ExchangeRate>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_ExchangeRate>>
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
        private IQueryable<ExchangeRate> BuildFilterQuery(MReq_ExchangeRate_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.ExchangeRates.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (!string.IsNullOrWhiteSpace(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            if (!string.IsNullOrWhiteSpace(request.FromCurrency))
                query = query.Where(x => x.FromCurrency == request.FromCurrency.ToUpper());

            if (!string.IsNullOrWhiteSpace(request.ToCurrency))
                query = query.Where(x => x.ToCurrency == request.ToCurrency.ToUpper());

            return query;
        }
        #endregion
    }
}
