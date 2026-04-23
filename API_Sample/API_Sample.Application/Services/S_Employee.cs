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
    public interface IS_Employee
    {
        Task<ResponseData<MRes_Employee>> Create(MReq_Employee request);
        Task<ResponseData<MRes_Employee>> Update(MReq_Employee request);
        Task<ResponseData<MRes_Employee>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<List<MRes_Employee>>> UpdateStatusList(string sequenceIds, short status, int updatedBy);
        Task<ResponseData<MRes_Employee>> UpdateTaxableStatus(int employeeId, bool isTaxable, int updatedBy);
        Task<ResponseData<int>> BulkUpdateTaxableStatus(string employeeIds, bool isTaxable, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Employee>> GetById(int id);
        Task<ResponseData<List<MRes_Employee>>> GetListByPaging(MReq_Employee_FullParam request);
        Task<ResponseData<List<MRes_Employee>>> GetListByFullParam(MReq_Employee_FullParam request);
    }

    /// <summary>
    /// Quản lý nhân viên (công nhân cao su, bảo vệ, cán bộ...)
    /// </summary>
    public class S_Employee : BaseService<S_Employee>, IS_Employee
    {
        private readonly IMapper _mapper;

        public S_Employee(MainDbContext context, IMapper mapper, ILogger<S_Employee> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới nhân viên, kiểm tra trùng MSNV
        /// </summary>
        public async Task<ResponseData<MRes_Employee>> Create(MReq_Employee request)
        {
            try
            {
                request.Msnv = request.Msnv?.Trim().ToUpper();
                var isExistsMsnv = await _context.Employees.AnyAsync(x => x.Msnv == request.Msnv && x.Status != -1);
                if (isExistsMsnv)
                    return Error(HttpStatusCode.Conflict, "Mã số nhân viên đã tồn tại!");

                var tramExists = await _context.Trams.AnyAsync(x => x.Id == request.TramId && x.Status != -1);
                if (!tramExists)
                    return Error(HttpStatusCode.BadRequest, "Trạm không tồn tại!");

                var data = _mapper.Map<Employee>(request);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.Employees.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                await _context.Entry(data).Reference(x => x.Tram).LoadAsync();

                return new ResponseData<MRes_Employee>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Employee>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        public async Task<ResponseData<MRes_Employee>> Update(MReq_Employee request)
        {
            try
            {
                request.Msnv = request.Msnv?.Trim().ToUpper();
                var isExistsMsnv = await _context.Employees.AnyAsync(x => x.Msnv == request.Msnv && x.Status != -1 && x.Id != request.Id);
                if (isExistsMsnv)
                    return Error(HttpStatusCode.Conflict, "Mã số nhân viên đã tồn tại!");

                var data = await _context.Employees.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                _mapper.Map(request, data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                await _context.Entry(data).Reference(x => x.Tram).LoadAsync();

                return new ResponseData<MRes_Employee>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Employee>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái nhân viên (1 = active, 0 = inactive, -1 = xóa mềm)
        /// </summary>
        public async Task<ResponseData<MRes_Employee>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.Employees.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Employee>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Employee>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái nhiều nhân viên cùng lúc
        /// </summary>
        public async Task<ResponseData<List<MRes_Employee>>> UpdateStatusList(string sequenceIds, short status, int updatedBy)
        {
            try
            {
                List<int> ids = JsonConvert.DeserializeObject<List<int>>(sequenceIds);
                if (ids == null || !ids.Any())
                    return Error(HttpStatusCode.BadRequest, MessageErrorConstants.DO_NOT_FIND_DATA);

                var now = DateTime.UtcNow;
                var updatedCount = await _context.Employees
                    .Where(x => ids.Contains(x.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Status, status)
                        .SetProperty(p => p.UpdatedBy, updatedBy)
                        .SetProperty(p => p.UpdatedAt, now)
                    );

                if (updatedCount == 0)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                var datas = await _context.Employees.AsNoTracking()
                    .Include(x => x.Tram)
                    .Where(x => ids.Contains(x.Id)).ToListAsync();

                return new ResponseData<List<MRes_Employee>>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<List<MRes_Employee>>(datas)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatusList), new { sequenceIds, status, updatedBy });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái chịu thuế cho 1 nhân viên
        /// </summary>
        public async Task<ResponseData<MRes_Employee>> UpdateTaxableStatus(int employeeId, bool isTaxable, int updatedBy)
        {
            try
            {
                var data = await _context.Employees.FindAsync(employeeId);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.IsTaxable = isTaxable;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Employee>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Employee>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateTaxableStatus), new { employeeId, isTaxable, updatedBy });
            }
        }

        /// <summary>
        /// Cập nhật hàng loạt trạng thái chịu thuế cho nhiều nhân viên
        /// </summary>
        public async Task<ResponseData<int>> BulkUpdateTaxableStatus(string employeeIds, bool isTaxable, int updatedBy)
        {
            try
            {
                List<int> ids = JsonConvert.DeserializeObject<List<int>>(employeeIds);
                if (ids == null || !ids.Any())
                    return Error(HttpStatusCode.BadRequest, MessageErrorConstants.DO_NOT_FIND_DATA);

                var now = DateTime.UtcNow;
                var updatedCount = await _context.Employees
                    .Where(x => ids.Contains(x.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.IsTaxable, isTaxable)
                        .SetProperty(p => p.UpdatedBy, updatedBy)
                        .SetProperty(p => p.UpdatedAt, now)
                    );

                if (updatedCount == 0)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<int>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = updatedCount
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(BulkUpdateTaxableStatus), new { employeeIds, isTaxable, updatedBy });
            }
        }

        /// <summary>
        /// Xóa cứng nhân viên (chỉ khi không có dữ liệu lương)
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var hasPayroll = await _context.Payrolls.AnyAsync(x => x.EmployeeId == id);
                if (hasPayroll)
                    return Error(HttpStatusCode.BadRequest, "Không thể xoá nhân viên đã có dữ liệu lương!");

                var deletedCount = await _context.Employees.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        /// Lấy chi tiết nhân viên theo ID bao gồm Trạm, Phòng ban, Chức vụ
        /// </summary>
        public async Task<ResponseData<MRes_Employee>> GetById(int id)
        {
            try
            {
                var data = await _context.Employees.AsNoTracking()
                    .Include(x => x.Tram)
                    .Include(x => x.Department)
                    .Include(x => x.Position)
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status != -1);

                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<MRes_Employee>(1, (int)HttpStatusCode.OK, "")
                {
                    data = _mapper.Map<MRes_Employee>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên có phân trang (tương thích SQL Server 2008 R2)
        /// </summary>
        public async Task<ResponseData<List<MRes_Employee>>> GetListByPaging(MReq_Employee_FullParam request)
        {
            try
            {
                int page = request.Page > 0 ? request.Page : 1;
                int record = request.Record > 0 ? request.Record : 10;

                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Employee> data = new List<MRes_Employee>();

                if (count > 0)
                {
                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allEntities = await query
                        .Include(x => x.Tram)
                        .Include(x => x.Department)
                        .Include(x => x.Position)
                        .Include(x => x.EmployeeType)
                        .OrderBy(x => x.Msnv)
                        .ToListAsync();

                    var entities = allEntities.Skip((page - 1) * record).Take(record).ToList();
                    data = _mapper.Map<List<MRes_Employee>>(entities);
                }

                return new ResponseData<List<MRes_Employee>>
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
        /// Lấy danh sách nhân viên theo bộ lọc (không phân trang)
        /// </summary>
        public async Task<ResponseData<List<MRes_Employee>>> GetListByFullParam(MReq_Employee_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Msnv)
                    .ProjectTo<MRes_Employee>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Employee>>
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
        private IQueryable<Employee> BuildFilterQuery(MReq_Employee_FullParam request)
        {
            var statusList = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToList() ?? new List<short>();

            var query = _context.Employees.AsNoTracking();

            if (statusList.Count > 0)
                query = query.Where(x => statusList.Contains(x.Status));

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchText = request.SearchText.Trim();
                query = query.Where(x => x.Msnv.Contains(searchText) || x.FullName.Contains(searchText));
            }

            if (request.TramId.HasValue)
                query = query.Where(x => x.TramId == request.TramId.Value);

            if (request.DepartmentId.HasValue)
                query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

            if (request.PositionId.HasValue)
                query = query.Where(x => x.PositionId == request.PositionId.Value);

            if (!string.IsNullOrWhiteSpace(request.TechnicalGrade))
                query = query.Where(x => x.TechnicalGrade == request.TechnicalGrade);

            return query;
        }
        #endregion
    }
}
