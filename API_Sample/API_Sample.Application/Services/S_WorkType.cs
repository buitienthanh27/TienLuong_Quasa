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
    public interface IS_WorkType
    {
        Task<ResponseData<MRes_WorkType>> Create(MReq_WorkType request);
        Task<ResponseData<MRes_WorkType>> Update(MReq_WorkType request);
        Task<ResponseData<MRes_WorkType>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_WorkType>> GetById(int id);
        Task<ResponseData<List<MRes_WorkType>>> GetListByPaging(MReq_WorkType_FullParam request);
        Task<ResponseData<List<MRes_WorkType>>> GetListByFullParam(MReq_WorkType_FullParam request);
        Task<ResponseData<MRes_WorkType>> GetByCode(string code);
    }

    /// <summary>
    /// Quản lý loại công việc (CARE, SUNDAY, HARDSHIP...) và đơn giá
    /// </summary>
    public class S_WorkType : BaseService<S_WorkType>, IS_WorkType
    {
        private readonly IMapper _mapper;

        public S_WorkType(MainDbContext context, IMapper mapper, ILogger<S_WorkType> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới loại công việc
        /// </summary>
        public async Task<ResponseData<MRes_WorkType>> Create(MReq_WorkType request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();

                var isExists = await _context.WorkTypes.AnyAsync(x =>
                    x.Code == request.Code && x.Status != -1);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Mã loại công việc đã tồn tại!");

                var data = _mapper.Map<WorkType>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.WorkTypes.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_WorkType>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_WorkType>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật loại công việc
        /// </summary>
        public async Task<ResponseData<MRes_WorkType>> Update(MReq_WorkType request)
        {
            try
            {
                request.Code = request.Code?.Trim().ToUpper();

                var isExists = await _context.WorkTypes.AnyAsync(x =>
                    x.Code == request.Code && x.Status != -1 && x.Id != request.Id);

                if (isExists)
                    return Error(HttpStatusCode.Conflict, "Mã loại công việc đã tồn tại!");

                var data = await _context.WorkTypes.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_WorkType>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_WorkType>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái loại công việc (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_WorkType>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.WorkTypes.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_WorkType>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_WorkType>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm loại công việc (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.WorkTypes
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
        /// Lấy chi tiết loại công việc theo ID
        /// </summary>
        public async Task<ResponseData<MRes_WorkType>> GetById(int id)
        {
            try
            {
                var data = await _context.WorkTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return new ResponseData<MRes_WorkType>
                {
                    data = _mapper.Map<MRes_WorkType>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy loại công việc theo mã
        /// </summary>
        public async Task<ResponseData<MRes_WorkType>> GetByCode(string code)
        {
            try
            {
                var data = await _context.WorkTypes
                    .AsNoTracking()
                    .Where(x => x.Code == code.ToUpper() && x.Status != -1)
                    .FirstOrDefaultAsync();

                if (data == null)
                    return Error(HttpStatusCode.NotFound, $"Không tìm thấy loại công việc {code}!");

                return new ResponseData<MRes_WorkType>
                {
                    data = _mapper.Map<MRes_WorkType>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByCode), new { code });
            }
        }

        /// <summary>
        /// Lấy danh sách loại công việc có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_WorkType>>> GetListByPaging(MReq_WorkType_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_WorkType> data = new List<MRes_WorkType>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.Code)
                        .ProjectTo<MRes_WorkType>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_WorkType>>
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
        /// Lấy danh sách loại công việc theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_WorkType>>> GetListByFullParam(MReq_WorkType_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Code)
                    .ProjectTo<MRes_WorkType>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_WorkType>>
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
        private IQueryable<WorkType> BuildFilterQuery(MReq_WorkType_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.WorkTypes.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (!string.IsNullOrWhiteSpace(request.Code))
                query = query.Where(x => x.Code.StartsWith(request.Code.ToUpper()));

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(x => x.Name.Contains(request.Name));

            return query;
        }
        #endregion
    }
}
