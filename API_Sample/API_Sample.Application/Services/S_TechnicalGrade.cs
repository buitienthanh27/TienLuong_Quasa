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
    public interface IS_TechnicalGrade
    {
        Task<ResponseData<MRes_TechnicalGrade>> Create(MReq_TechnicalGrade request);
        Task<ResponseData<MRes_TechnicalGrade>> Update(MReq_TechnicalGrade request);
        Task<ResponseData<MRes_TechnicalGrade>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_TechnicalGrade>> GetById(int id);
        Task<ResponseData<List<MRes_TechnicalGrade>>> GetListByPaging(MReq_TechnicalGrade_FullParam request);
        Task<ResponseData<List<MRes_TechnicalGrade>>> GetListByFullParam(MReq_TechnicalGrade_FullParam request);
        Task<ResponseData<MRes_TechnicalGrade>> GetByGrade(string grade);
    }

    /// <summary>
    /// Quản lý hạng kỹ thuật (A/B/C/D) và hệ số điểm tương ứng
    /// </summary>
    public class S_TechnicalGrade : BaseService<S_TechnicalGrade>, IS_TechnicalGrade
    {
        private readonly IMapper _mapper;

        public S_TechnicalGrade(MainDbContext context, IMapper mapper, ILogger<S_TechnicalGrade> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới hạng kỹ thuật
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalGrade>> Create(MReq_TechnicalGrade request)
        {
            try
            {
                request.Grade = request.Grade?.Trim().ToUpper();

                var isExists = await _context.TechnicalGrades.AnyAsync(x =>
                    x.Grade == request.Grade && x.Status != -1);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Hạng kỹ thuật đã tồn tại!");

                var data = _mapper.Map<TechnicalGrade>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.TechnicalGrades.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_TechnicalGrade>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TechnicalGrade>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật hạng kỹ thuật
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalGrade>> Update(MReq_TechnicalGrade request)
        {
            try
            {
                request.Grade = request.Grade?.Trim().ToUpper();

                var isExists = await _context.TechnicalGrades.AnyAsync(x =>
                    x.Grade == request.Grade && x.Status != -1 && x.Id != request.Id);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Hạng kỹ thuật đã tồn tại!");

                var data = await _context.TechnicalGrades.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_TechnicalGrade>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TechnicalGrade>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái hạng kỹ thuật (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalGrade>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.TechnicalGrades.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_TechnicalGrade>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_TechnicalGrade>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm hạng kỹ thuật (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.TechnicalGrades
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
        /// Lấy chi tiết hạng kỹ thuật theo ID
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalGrade>> GetById(int id)
        {
            try
            {
                var data = await _context.TechnicalGrades.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_TechnicalGrade>
                {
                    data = _mapper.Map<MRes_TechnicalGrade>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy hạng kỹ thuật theo mã hạng (A/B/C/D)
        /// </summary>
        public async Task<ResponseData<MRes_TechnicalGrade>> GetByGrade(string grade)
        {
            try
            {
                var data = await _context.TechnicalGrades
                    .AsNoTracking()
                    .Where(x => x.Grade == grade.ToUpper() && x.Status != -1)
                    .FirstOrDefaultAsync();

                if (data == null)
                    return Error(HttpStatusCode.NotFound, $"Không tìm thấy hạng kỹ thuật {grade}!");

                return new ResponseData<MRes_TechnicalGrade>
                {
                    data = _mapper.Map<MRes_TechnicalGrade>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByGrade), new { grade });
            }
        }

        /// <summary>
        /// Lấy danh sách hạng kỹ thuật có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_TechnicalGrade>>> GetListByPaging(MReq_TechnicalGrade_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_TechnicalGrade> data = new List<MRes_TechnicalGrade>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.Grade)
                        .ProjectTo<MRes_TechnicalGrade>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_TechnicalGrade>>
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
        /// Lấy danh sách hạng kỹ thuật theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_TechnicalGrade>>> GetListByFullParam(MReq_TechnicalGrade_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.SortOrder).ThenBy(x => x.Grade)
                    .ProjectTo<MRes_TechnicalGrade>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_TechnicalGrade>>
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
        private IQueryable<TechnicalGrade> BuildFilterQuery(MReq_TechnicalGrade_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.TechnicalGrades.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (!string.IsNullOrWhiteSpace(request.Grade))
                query = query.Where(x => x.Grade == request.Grade.ToUpper());

            return query;
        }
        #endregion
    }
}
