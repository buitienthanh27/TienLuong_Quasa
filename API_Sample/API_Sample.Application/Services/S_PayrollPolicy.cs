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
    public interface IS_PayrollPolicy
    {
        Task<ResponseData<MRes_PayrollPolicy>> Create(MReq_PayrollPolicy request);
        Task<ResponseData<MRes_PayrollPolicy>> Update(MReq_PayrollPolicy request);
        Task<ResponseData<MRes_PayrollPolicy>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_PayrollPolicy>> GetById(int id);
        Task<ResponseData<List<MRes_PayrollPolicy>>> GetListByPaging(MReq_PayrollPolicy_FullParam request);
        Task<ResponseData<List<MRes_PayrollPolicy>>> GetListByFullParam(MReq_PayrollPolicy_FullParam request);
    }

    /// <summary>
    /// Quản lý chính sách tính lương (PayrollPolicy)
    /// </summary>
    public class S_PayrollPolicy : BaseService<S_PayrollPolicy>, IS_PayrollPolicy
    {
        private readonly IMapper _mapper;

        public S_PayrollPolicy(MainDbContext context, IMapper mapper, ILogger<S_PayrollPolicy> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo mới chính sách tính lương
        /// </summary>
        public async Task<ResponseData<MRes_PayrollPolicy>> Create(MReq_PayrollPolicy request)
        {
            try
            {
                var validationError = await ValidatePayrollPolicy(request);
                if (validationError != null)
                    return Error(HttpStatusCode.BadRequest, validationError);

                if (await _context.PayrollPolicies.AnyAsync(x => x.Code == request.Code.Trim().ToUpper() && x.Status != -1))
                    return Error(HttpStatusCode.Conflict, "Mã chính sách đã tồn tại");

                var data = _mapper.Map<PayrollPolicy>(request);
                data.Code = request.Code.Trim().ToUpper();
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.PayrollPolicies.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_PayrollPolicy>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_PayrollPolicy>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        /// <summary>
        /// Cập nhật chính sách tính lương
        /// </summary>
        public async Task<ResponseData<MRes_PayrollPolicy>> Update(MReq_PayrollPolicy request)
        {
            try
            {
                var data = await _context.PayrollPolicies.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                var validationError = await ValidatePayrollPolicy(request, request.Id);
                if (validationError != null)
                    return Error(HttpStatusCode.BadRequest, validationError);

                if (await _context.PayrollPolicies.AnyAsync(x => x.Code == request.Code.Trim().ToUpper() && x.Status != -1 && x.Id != request.Id))
                    return Error(HttpStatusCode.Conflict, "Mã chính sách đã tồn tại");

                _mapper.Map(request, data);
                data.Code = request.Code.Trim().ToUpper();
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_PayrollPolicy>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_PayrollPolicy>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái chính sách
        /// </summary>
        public async Task<ResponseData<MRes_PayrollPolicy>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.PayrollPolicies.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_PayrollPolicy>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_PayrollPolicy>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa mềm chính sách
        /// </summary>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.PayrollPolicies
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
        /// Lấy chi tiết chính sách theo ID
        /// </summary>
        public async Task<ResponseData<MRes_PayrollPolicy>> GetById(int id)
        {
            try
            {
                var data = await _context.PayrollPolicies
                    .AsNoTracking()
                    .Include(x => x.EmployeeType)
                    .Include(x => x.Tram)
                    .Include(x => x.Position)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var result = _mapper.Map<MRes_PayrollPolicy>(data);
                if (result != null && data != null)
                {
                    result.EmployeeTypeName = data.EmployeeType?.Name;
                    result.TramCode = data.Tram?.Code;
                    result.PositionName = data.Position?.Name;
                }

                return new ResponseData<MRes_PayrollPolicy>
                {
                    data = result,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách chính sách có phân trang
        /// </summary>
        public async Task<ResponseData<List<MRes_PayrollPolicy>>> GetListByPaging(MReq_PayrollPolicy_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_PayrollPolicy> data = new List<MRes_PayrollPolicy>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderByDescending(x => x.Priority)
                        .ThenBy(x => x.Code)
                        .ProjectTo<MRes_PayrollPolicy>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_PayrollPolicy>>
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
        /// Lấy danh sách chính sách theo bộ lọc
        /// </summary>
        public async Task<ResponseData<List<MRes_PayrollPolicy>>> GetListByFullParam(MReq_PayrollPolicy_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.Code)
                    .ProjectTo<MRes_PayrollPolicy>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_PayrollPolicy>>
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
        /// Validate chính sách trước khi Create/Update
        /// </summary>
        private async Task<string?> ValidatePayrollPolicy(MReq_PayrollPolicy request, int? excludeId = null)
        {
            // 1. XOR: phải có đúng 1 trong 2 (DivisorValue hoặc DivisorParamCode)
            bool hasValue = request.DivisorValue.HasValue && request.DivisorValue.Value > 0;
            bool hasParamCode = !string.IsNullOrEmpty(request.DivisorParamCode);

            if (hasValue && hasParamCode)
                return "Chỉ được nhập DivisorValue HOẶC DivisorParamCode, không được cả hai";

            if (!hasValue && !hasParamCode)
                return "Phải nhập DivisorValue hoặc DivisorParamCode";

            // 2. Nếu có DivisorParamCode, validate tồn tại
            if (hasParamCode)
            {
                var exists = await _context.SystemParameters
                    .AnyAsync(x => x.ParamCode == request.DivisorParamCode && x.Status == 1);
                if (!exists)
                    return $"SystemParameter '{request.DivisorParamCode}' không tồn tại";
            }

            // 3. EffectiveDate <= EndDate (nếu EndDate có giá trị)
            if (request.EndDate.HasValue && request.EffectiveDate > request.EndDate.Value)
                return "Ngày hiệu lực phải nhỏ hơn hoặc bằng ngày kết thúc";

            // 4. Unique constraint: không duplicate (EmployeeTypeId, TramId, PositionId, EffectiveDate, Priority)
            var duplicate = await _context.PayrollPolicies
                .AsNoTracking()
                .Where(x => x.Status == 1
                            && (excludeId == null || x.Id != excludeId)
                            && x.EmployeeTypeId == request.EmployeeTypeId
                            && x.TramId == request.TramId
                            && x.PositionId == request.PositionId
                            && x.EffectiveDate == request.EffectiveDate
                            && x.Priority == request.Priority)
                .AnyAsync();

            if (duplicate)
                return "Đã tồn tại policy với cùng EmployeeType, Tram, Position, EffectiveDate và Priority";

            return null;
        }

        private IQueryable<PayrollPolicy> BuildFilterQuery(MReq_PayrollPolicy_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.PayrollPolicies.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (!string.IsNullOrEmpty(request.Code))
                query = query.Where(x => x.Code.StartsWith(request.Code.Trim().ToUpper()));

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(x => x.Name.Contains(request.Name.Trim()));

            if (request.EmployeeTypeId.HasValue)
                query = query.Where(x => x.EmployeeTypeId == request.EmployeeTypeId.Value);

            if (request.TramId.HasValue)
                query = query.Where(x => x.TramId == request.TramId.Value);

            if (request.PositionId.HasValue)
                query = query.Where(x => x.PositionId == request.PositionId.Value);

            if (request.EffectiveDate.HasValue)
                query = query.Where(x => x.EffectiveDate <= request.EffectiveDate.Value
                                         && (x.EndDate == null || x.EndDate >= request.EffectiveDate.Value));

            return query;
        }
        #endregion
    }
}
