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
    public interface IS_TechnicalEvaluation
    {
        Task<ResponseData<MRes_TechnicalEvaluation>> Create(MReq_TechnicalEvaluation request);
        Task<ResponseData<MRes_TechnicalEvaluation>> Update(MReq_TechnicalEvaluation request);
        Task<ResponseData<MRes_TechnicalEvaluation>> Review(int id, string reviewedGrade, decimal? reviewedScore, int reviewedBy, string? note);
        Task<ResponseData<MRes_TechnicalEvaluation>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_TechnicalEvaluation>> GetById(int id);
        Task<ResponseData<List<MRes_TechnicalEvaluation>>> GetListByPaging(MReq_TechnicalEvaluation_FullParam request);
        Task<ResponseData<List<MRes_TechnicalEvaluation>>> GetListByFullParam(MReq_TechnicalEvaluation_FullParam request);
        Task<ResponseData<MRes_TechnicalEvaluation>> GetByEmployeeMonth(int employeeId, string yearMonth);
    }

    /// <summary>
    /// Quản lý đánh giá hạng kỹ thuật hàng tháng (QLKT đánh giá + phúc tra)
    /// </summary>
    public class S_TechnicalEvaluation : BaseService<S_TechnicalEvaluation>, IS_TechnicalEvaluation
    {
        private readonly IMapper _mapper;

        public S_TechnicalEvaluation(MainDbContext context, IMapper mapper, ILogger<S_TechnicalEvaluation> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo đánh giá hạng kỹ thuật (QLKT đánh giá cuối tháng)
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalEvaluation>> Create(MReq_TechnicalEvaluation request)
        {
            try
            {
                request.EvaluatedGrade = request.EvaluatedGrade?.Trim().ToUpper();
                request.FinalGrade = request.EvaluatedGrade;

                var isExists = await _context.TechnicalEvaluations.AnyAsync(x =>
                    x.EmployeeId == request.EmployeeId &&
                    x.YearMonth == request.YearMonth &&
                    x.Status != -1);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Đánh giá hạng KT đã tồn tại cho nhân viên/tháng này!");

                var employee = await _context.Employees.FindAsync(request.EmployeeId);
                if (employee == null)
                    return Error(HttpStatusCode.NotFound, "Không tìm thấy nhân viên!");

                var data = _mapper.Map<TechnicalEvaluation>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;
                data.EvaluatedAt = DateTime.UtcNow;

                _context.TechnicalEvaluations.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.TechnicalEvaluations
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_TechnicalEvaluation>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TechnicalEvaluation>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật đánh giá
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalEvaluation>> Update(MReq_TechnicalEvaluation request)
        {
            try
            {
                request.EvaluatedGrade = request.EvaluatedGrade?.Trim().ToUpper();

                var data = await _context.TechnicalEvaluations.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                if (!data.IsReviewed)
                    data.FinalGrade = data.EvaluatedGrade;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.TechnicalEvaluations
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_TechnicalEvaluation>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TechnicalEvaluation>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Phúc tra đánh giá (cập nhật hạng sau khi phúc tra)
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalEvaluation>> Review(int id, string reviewedGrade, decimal? reviewedScore, int reviewedBy, string? note)
        {
            try
            {
                var data = await _context.TechnicalEvaluations.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.IsReviewed = true;
                data.ReviewedGrade = reviewedGrade?.Trim().ToUpper();
                data.ReviewedScore = reviewedScore;
                data.ReviewedBy = reviewedBy;
                data.ReviewedAt = DateTime.UtcNow;
                data.FinalGrade = data.ReviewedGrade;
                data.Note = note;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = reviewedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.TechnicalEvaluations
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_TechnicalEvaluation>(1, (int)HttpStatusCode.OK, "Phúc tra thành công!")
                {
                    data = _mapper.Map<MRes_TechnicalEvaluation>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Review), new { id, reviewedGrade, reviewedScore, reviewedBy, note });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái đánh giá KT (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalEvaluation>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.TechnicalEvaluations.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_TechnicalEvaluation>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TechnicalEvaluation>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm đánh giá KT (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.TechnicalEvaluations
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
        /// Lấy chi tiết đánh giá KT theo ID
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalEvaluation>> GetById(int id)
        {
            try
            {
                var data = await _context.TechnicalEvaluations
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_TechnicalEvaluation>
                {
                    data = _mapper.Map<MRes_TechnicalEvaluation>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy đánh giá theo nhân viên và tháng
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalEvaluation>> GetByEmployeeMonth(int employeeId, string yearMonth)
        {
            try
            {
                var data = await _context.TechnicalEvaluations
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .Where(x => x.EmployeeId == employeeId &&
                                x.YearMonth == yearMonth &&
                                x.Status != -1)
                    .FirstOrDefaultAsync();

                if (data == null)
                    return Error(HttpStatusCode.NotFound, "Không tìm thấy đánh giá hạng KT!");

                return new ResponseData<MRes_TechnicalEvaluation>
                {
                    data = _mapper.Map<MRes_TechnicalEvaluation>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByEmployeeMonth), new { employeeId, yearMonth });
            }
        }

        /// <summary>
        /// Lấy danh sách đánh giá KT có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_TechnicalEvaluation>>> GetListByPaging(MReq_TechnicalEvaluation_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_TechnicalEvaluation> data = new List<MRes_TechnicalEvaluation>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Employee.Msnv)
                        .ProjectTo<MRes_TechnicalEvaluation>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_TechnicalEvaluation>>
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
        /// Lấy danh sách đánh giá KT theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_TechnicalEvaluation>>> GetListByFullParam(MReq_TechnicalEvaluation_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_TechnicalEvaluation>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_TechnicalEvaluation>>
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
        private IQueryable<TechnicalEvaluation> BuildFilterQuery(MReq_TechnicalEvaluation_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.TechnicalEvaluations.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId);

            if (!string.IsNullOrWhiteSpace(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            if (!string.IsNullOrWhiteSpace(request.FinalGrade))
                query = query.Where(x => x.FinalGrade == request.FinalGrade.ToUpper());

            if (request.IsReviewed.HasValue)
                query = query.Where(x => x.IsReviewed == request.IsReviewed);

            if (request.TramId.HasValue)
                query = query.Where(x => x.Employee.TramId == request.TramId);

            return query;
        }
        #endregion
    }
}
