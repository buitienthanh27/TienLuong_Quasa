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
    public interface IS_Position
    {
        Task<ResponseData<MRes_Position>> Create(MReq_Position request);
        Task<ResponseData<MRes_Position>> Update(MReq_Position request);
        Task<ResponseData<MRes_Position>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Position>> GetById(int id);
        Task<ResponseData<List<MRes_Position>>> GetListByPaging(MReq_Position_FullParam request);
        Task<ResponseData<List<MRes_Position>>> GetListByFullParam(MReq_Position_FullParam request);
    }

    public class S_Position : BaseService<S_Position>, IS_Position
    {
        private readonly IMapper _mapper;

        public S_Position(MainDbContext context, IMapper mapper, ILogger<S_Position> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        public async Task<ResponseData<MRes_Position>> Create(MReq_Position request)
        {
            try
            {
                if (await _context.Positions.AnyAsync(x => x.Code == request.Code.Trim().ToUpper() && x.Status != -1))
                    return Error(HttpStatusCode.Conflict, "Mã chức vụ đã tồn tại");

                var data = _mapper.Map<Position>(request);
                data.Code = request.Code.Trim().ToUpper();
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;
                data.Status = 1;

                _context.Positions.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_Position>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Position>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), request);
            }
        }

        public async Task<ResponseData<MRes_Position>> Update(MReq_Position request)
        {
            try
            {
                var data = await _context.Positions.FindAsync(request.Id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                if (await _context.Positions.AnyAsync(x => x.Code == request.Code.Trim().ToUpper() && x.Status != -1 && x.Id != request.Id))
                    return Error(HttpStatusCode.Conflict, "Mã chức vụ đã tồn tại");

                _mapper.Map(request, data);
                data.Code = request.Code.Trim().ToUpper();
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = request.UpdatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Position>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Position>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Update), request);
            }
        }

        public async Task<ResponseData<MRes_Position>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.Positions.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Position>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Position>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.Positions
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

        public async Task<ResponseData<MRes_Position>> GetById(int id)
        {
            try
            {
                var data = await _context.Positions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_Position>
                {
                    data = _mapper.Map<MRes_Position>(data),
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        public async Task<ResponseData<List<MRes_Position>>> GetListByPaging(MReq_Position_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Position> data = new List<MRes_Position>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    data = await query
                        .OrderBy(x => x.Code)
                        .Skip((page - 1) * record)
                        .Take(record)
                        .ProjectTo<MRes_Position>(_mapper.ConfigurationProvider)
                        .ToListAsync();
                }

                return new ResponseData<List<MRes_Position>>
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

        public async Task<ResponseData<List<MRes_Position>>> GetListByFullParam(MReq_Position_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .OrderBy(x => x.Code)
                    .ProjectTo<MRes_Position>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Position>>
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
        private IQueryable<Position> BuildFilterQuery(MReq_Position_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.Positions.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));
            else
                query = query.Where(x => x.Status != -1);

            if (!string.IsNullOrEmpty(request.Code))
                query = query.Where(x => x.Code.StartsWith(request.Code.Trim().ToUpper()));

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(x => x.Name.Contains(request.Name.Trim()));

            if (!string.IsNullOrEmpty(request.Type))
                query = query.Where(x => x.Type == request.Type);

            return query;
        }
        #endregion
    }
}
