using API_Sample.Application.Ultilities;
using API_Sample.Data.EF;
using API_Sample.Models.Common;
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
    public interface IS_Image
    {
        Task<ResponseData<MRes_Image>> GetById(int id);
        Task<ResponseData<BaseModel.Image>> GetByIdCustomResponse(int id);
        Task<ResponseData<List<MRes_Image>>> GetListByListId(List<int> ids);
        Task DeleteImageOld(int? oldId, int? newId = 0);
        Task DeleteListImage(List<int> ids);
    }

    /// <summary>
    /// Quản lý hình ảnh (lưu trữ, truy xuất, xoá mềm ảnh trong hệ thống)
    /// </summary>
    public class S_Image : BaseService<S_Image>, IS_Image
    {
        private readonly IMapper _mapper;

        public S_Image(MainDbContext context, IMapper mapper, ILogger<S_Image> logger)
            : base(context, logger)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy chi tiết hình ảnh theo ID (trả về MRes_Image)
        /// </summary>
        public async Task<ResponseData<MRes_Image>> GetById(int id)
        {
            try
            {
                var data = await _context.Images
                    .AsNoTracking()
                    .ProjectTo<MRes_Image>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<MRes_Image>
                {
                    data = data,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetById), new { id });
            }
        }

        /// <summary>
        /// Lấy chi tiết hình ảnh theo ID (trả về BaseModel.Image cho response tuỳ chỉnh)
        /// </summary>
        public async Task<ResponseData<BaseModel.Image>> GetByIdCustomResponse(int id)
        {
            try
            {
                var data = await _context.Images
                    .AsNoTracking()
                    .ProjectTo<BaseModel.Image>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<BaseModel.Image>
                {
                    data = data,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetByIdCustomResponse), new { id });
            }
        }

        /// <summary>
        /// Lấy danh sách hình ảnh theo danh sách ID
        /// </summary>
        public async Task<ResponseData<List<MRes_Image>>> GetListByListId(List<int> ids)
        {
            try
            {
                var data = await _context.Images
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.Id))
                    .ProjectTo<MRes_Image>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                if (data == null || !data.Any())
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                return new ResponseData<List<MRes_Image>>
                {
                    data = data,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListByListId), new { ids });
            }
        }

        /// <summary>
        /// Xoá mềm hình ảnh cũ khi thay thế bằng ảnh mới (chỉ xoá nếu oldId khác newId)
        /// </summary>
        public async Task DeleteImageOld(int? oldId, int? newId = 0)
        {
            try
            {
                if (oldId > 0 && oldId != newId)
                {
                    await _context.Images
                        .Where(x => x.Id == oldId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.Status, -1)
                            .SetProperty(p => p.Timer, DateTime.UtcNow)
                        );
                }
            }
            catch (Exception ex)
            {
                var paramStr = JsonConvert.SerializeObject(new { oldId, newId });
                _logger.LogError(ex, $"{nameof(S_Image)}.{nameof(DeleteImageOld)} Exception. Parameters: {paramStr}");
            }
        }

        /// <summary>
        /// Xoá mềm danh sách hình ảnh theo danh sách ID
        /// </summary>
        public async Task DeleteListImage(List<int> ids)
        {
            try
            {
                if (ids != null && ids.Any())
                {
                    var cleanIds = ids.Where(x => x != 0).ToList();
                    if (cleanIds.Any())
                    {
                        await _context.Images
                            .Where(x => cleanIds.Contains(x.Id))
                            .ExecuteUpdateAsync(s => s
                                .SetProperty(p => p.Status, -1)
                                .SetProperty(p => p.Timer, DateTime.UtcNow)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                var paramStr = JsonConvert.SerializeObject(new { ids });
                _logger.LogError(ex, $"{nameof(S_Image)}.{nameof(DeleteListImage)} Exception. Parameters: {paramStr}");
            }
        }
    }
}

