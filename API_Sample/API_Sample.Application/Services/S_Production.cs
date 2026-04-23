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
    public interface IS_Production
    {
        Task<ResponseData<MRes_Production>> Create(MReq_Production request);
        Task<ResponseData<MRes_Production>> Update(MReq_Production request);
        Task<ResponseData<int>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id, int updatedBy);
        Task<ResponseData<MRes_Production>> GetById(int id);
        Task<ResponseData<List<MRes_Production>>> GetListByPaging(MReq_Production_FullParam request);
        Task<ResponseData<List<MRes_Production>>> GetListByFullParam(MReq_Production_FullParam request);
    }

    /// <summary>
    /// Service quản lý sản lượng mủ cao su
    /// </summary>
    public class S_Production : BaseService<S_Production>, IS_Production
    {
        private readonly IMapper _mapper;

        public S_Production(MainDbContext context, IMapper mapper, ILogger<S_Production> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới sản lượng mủ cho nhân viên theo tháng
        /// </summary>
        public async Task<ResponseData<MRes_Production>> Create(MReq_Production request)
        {
            try
            {
                if (await _context.Productions.AnyAsync(x =>
                    x.EmployeeId == request.EmployeeId &&
                    x.YearMonth == request.YearMonth &&
                    x.Status != -1))
                    return Error(HttpStatusCode.Conflict, "Sản lượng tháng này đã tồn tại cho nhân viên!");

                var data = _mapper.Map<Production>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                data.TotalPayKg = data.DryLatexKg + data.CarryDryKg;

                _context.Productions.Add(data);
                if (await _context.SaveChangesAsync() == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                var result = await _context.Productions
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_Production>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Production>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật sản lượng mủ, tự động tính lại TotalPayKg
        /// </summary>
        public async Task<ResponseData<MRes_Production>> Update(MReq_Production request)
        {
            try
            {
                var data = await _context.Productions.FindAsync(request.Id);
                if (data == null || data.Status == -1)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.RawLatexKg = request.RawLatexKg;
                data.RopeLatexKg = request.RopeLatexKg;
                data.SerumKg = request.SerumKg;
                data.CarryOverKg = request.CarryOverKg;
                data.DrcRaw = request.DrcRaw;
                data.DrcSerum = request.DrcSerum;
                data.DryLatexKg = request.DryLatexKg;
                data.CarryDryKg = request.CarryDryKg;
                data.TotalPayKg = request.DryLatexKg + request.CarryDryKg;
                data.TechGrade = request.TechGrade;
                data.Note = request.Note;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                if (await _context.SaveChangesAsync() == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                var result = await _context.Productions
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == data.Id);

                return new ResponseData<MRes_Production>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Production>(result)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái sản lượng (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<int>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var updated = await _context.Productions
                    .Where(x => x.Id == id && x.Status != -1)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.Status, status)
                        .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                        .SetProperty(x => x.UpdatedBy, updatedBy));

                if (updated == 0)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<int>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS) { data = updated };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm sản lượng (set Status = -1)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id, int updatedBy)
        {
            return await UpdateStatus(id, -1, updatedBy);
        }

        /// <summary>
        /// Lấy chi tiết sản lượng theo ID
        /// </summary>
        public async Task<ResponseData<MRes_Production>> GetById(int id)
        {
            try
            {
                var data = await _context.Productions
                    .AsNoTracking()
                    .Include(x => x.Employee).ThenInclude(e => e.Tram)
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status != -1);

                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<MRes_Production>(1, (int)HttpStatusCode.OK, "")
                {
                    data = _mapper.Map<MRes_Production>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), id);
            }
        }

        /// <summary>
        /// Lấy danh sách sản lượng có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_Production>>> GetListByPaging(MReq_Production_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);
                var total = await query.CountAsync();

                int page = request.Page > 0 ? request.Page : 1;
                int record = request.Record > 0 ? request.Record : 10;

                // SQL Server 2008 R2: fetch all rồi paging in-memory
                var allData = await query
                    .OrderBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_Production>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                var list = allData.Skip((page - 1) * record).Take(record).ToList();

                return new ResponseData<List<MRes_Production>>(1, (int)HttpStatusCode.OK, "")
                {
                    data = list,
                    data2nd = total
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByPaging), request);
            }
        }

        /// <summary>
        /// Lấy danh sách sản lượng theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_Production>>> GetListByFullParam(MReq_Production_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);
                var list = await query
                    .OrderBy(x => x.Employee.Msnv)
                    .ProjectTo<MRes_Production>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Production>>(1, (int)HttpStatusCode.OK, "")
                {
                    data = list,
                    data2nd = list.Count
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByFullParam), request);
            }
        }

        #region Common functions
        private IQueryable<Production> BuildFilterQuery(MReq_Production_FullParam request)
        {
            // KHÔNG dùng Include() - ProjectTo tự load từ mapping config
            var query = _context.Productions
                .AsNoTracking()
                .Where(x => x.Status != -1);

            if (!string.IsNullOrEmpty(request.SequenceStatus))
            {
                var statusList = request.SequenceStatus.Split(',').Select(short.Parse).ToArray();
                query = query.Where(x => statusList.Contains(x.Status));
            }

            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

            if (request.TramId.HasValue)
                query = query.Where(x => x.Employee.TramId == request.TramId.Value);

            if (!string.IsNullOrEmpty(request.YearMonth))
                query = query.Where(x => x.YearMonth == request.YearMonth);

            if (!string.IsNullOrEmpty(request.TechGrade))
                query = query.Where(x => x.TechGrade == request.TechGrade);

            return query;
        }
        #endregion
    }
}
