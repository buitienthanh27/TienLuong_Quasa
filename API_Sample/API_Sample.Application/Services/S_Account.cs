using API_Sample.Application.Ultilities;
using API_Sample.Data.EF;
using API_Sample.Data.Entities;
using API_Sample.Models.Common;
using API_Sample.Models.Enums;
using API_Sample.Models.Request;
using API_Sample.Models.Response;
using API_Sample.Utilities.Constants;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_Sample.Application.Services
{
    public interface IS_Account
    {
        Task<ResponseData<MRes_Token>> Login(MReq_Account_Login request);
        Task<ResponseData<MRes_Token>> RefreshToken(MReq_Token_Refresh request);
        Task<ResponseData<bool>> RevokeToken(string userName);
        Task<ResponseData<MRes_Account>> Create(MReq_Account request);
        Task<ResponseData<MRes_Account>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<List<MRes_Account>>> UpdateStatusList(string sequenceIds, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Account>> GetById(int id);
        Task<ResponseData<List<MRes_Account>>> GetListByPaging(MReq_Account_FullParam request);
        Task<ResponseData<List<MRes_Account>>> GetListByFullParam(MReq_Account_FullParam request);
        Task<ResponseData<List<MRes_Account>>> GetListBySpFullParam(MReq_Account_FullParam request);
    }

    public class S_Account : BaseService<S_Account>, IS_Account
    {
        private readonly IMapper _mapper;
        private readonly IJwtHelper _jwtHelper;

        public S_Account(MainDbContext context, IMapper mapper, ILogger<S_Account> logger, IJwtHelper jwtHelper)
            : base(context, logger)
        {
            _mapper = mapper;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Hàm đăng nhập, xác thực tài khoản và mật khẩu, nếu thành công sẽ trả về accessToken và refreshToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Token>> Login(MReq_Account_Login request)
        {
            try
            {
                request.UserName = request.UserName?.Trim().ToLower();
                request.Password = request.Password?.Trim();

                var findUser = await _context.Accounts.FirstOrDefaultAsync(x => (x.UserName == request.UserName || x.Phone == request.UserName || x.Email == request.UserName) && (x.Status == (short)EN_Account_Status.Active || x.Status == (short)EN_Account_Status.Locked));
                if (findUser == null)
                    return Error(HttpStatusCode.NotFound, "Tài khoản hoặc mật khẩu không đúng");

                // Verify password
                bool loginSuccess = BCrypt.Net.BCrypt.Verify(request.Password, findUser.Password);
                if (!loginSuccess)
                    return Error(HttpStatusCode.Conflict, "Tài khoản hoặc mật khẩu không chính xác");
                
                // Generate accessToken
                var claims = new[]
                {
                    new Claim("AccountId", findUser.Id.ToString()),
                    new Claim(ClaimTypes.Email, findUser.Email ?? ""),
                    new Claim(ClaimTypes.MobilePhone, findUser.Phone ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, findUser.UserName)
                };
                var accessToken = _jwtHelper.BuildToken(claims, 30);
                var refreshToken = _jwtHelper.GenerateRefreshToken();

                findUser.RefreshToken = refreshToken;
                findUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return new ResponseData<MRes_Token>(1, (int)HttpStatusCode.OK, "Đăng nhập thành công")
                {
                    data = new MRes_Token
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Login), new { request.UserName });
            }
        }

        /// <summary>
        /// Hàm làm mới accessToken bằng refreshToken, nếu refreshToken hợp lệ và chưa hết hạn sẽ trả về accessToken mới và refreshToken mới, đồng thời cập nhật refreshToken mới vào database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Token>> RefreshToken(MReq_Token_Refresh request)
        {
            try
            {
                var principal = _jwtHelper.GetPrincipalFromExpiredToken(request.AccessToken);
                var username = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == username);

                if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return Error(HttpStatusCode.BadRequest, "Không tìm thấy tài khoản hoặc phiên đã hết hạn, hãy đăng nhập lại");

                var newAccessToken = _jwtHelper.BuildToken(principal.Claims.ToArray(), 30);
                var newRefreshToken = _jwtHelper.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<MRes_Token>(1, (int)HttpStatusCode.OK, "Refresh token thành công")
                {
                    data = new MRes_Token
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken
                    }
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(RefreshToken), request);
            }
        }

        /// <summary>
        /// Hàm thu hồi token, thường dùng khi người dùng đăng xuất hoặc khi phát hiện token bị lộ, sẽ xóa refreshToken trong database để token không còn giá trị sử dụng nữa
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<bool>> RevokeToken(string userName)
        {
            try
            {
                var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user == null) return Error(HttpStatusCode.BadRequest, "Không tìm thấy tài khoản");

                user.RefreshToken = null;
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

                return new ResponseData<bool>(1, (int)HttpStatusCode.OK, "Revoke token thành công")
                {
                    data = true
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(RevokeToken), new { userName });
            }
        }

        /// <summary>
        /// Hàm tạo mới bản ghi, thường dùng cho các trường hợp thêm mới dữ liệu, cần quan tâm đến việc kiểm tra dữ liệu đầu vào
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Account>> Create(MReq_Account request)
        {
            try
            {
                request.UserName = request.UserName?.Trim().ToLower();
                request.Password = request.Password?.Trim().ToLower();
                var isExistsUserName = await _context.Accounts.AnyAsync(x => x.UserName == request.UserName && x.Status != -1);
                if (isExistsUserName)
                    return Error(HttpStatusCode.Conflict, "Trùng lặp tên đăng nhập");

                var data = _mapper.Map<Account>(request);
                data.Password = BCrypt.Net.BCrypt.HashString(request.Password.Trim(), SaltRevision.Revision2Y);
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = request.CreatedBy;

                _context.Accounts.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

                return new ResponseData<MRes_Account>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Account>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(Create), new { request.UserName, request.Email, request.Phone });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái cho 1 bản ghi, thường dùng để thay đổi trạng thái kích hoạt, trạng thái xóa mềm,... mà không cần quan tâm đến các trường dữ liệu khác, hoặc khi chỉ cần thay đổi trạng thái mà không cần lấy lại dữ liệu sau khi cập nhật
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Account>> UpdateStatus(int id, short status, int updatedBy)
        {
            try
            {
                var data = await _context.Accounts.FindAsync(id);
                if (data == null)
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

                data.Status = status;
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = updatedBy;

                var save = await _context.SaveChangesAsync();
                if (save == 0)
                {
                    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);
                }

                return new ResponseData<MRes_Account>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<MRes_Account>(data)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái cho nhiều bản ghi cùng lúc
        /// </summary>
        /// <param name="sequenceIds"></param>
        /// <param name="status"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Account>>> UpdateStatusList(string sequenceIds, short status, int updatedBy)
        {
            try
            {
                List<int> ids = JsonConvert.DeserializeObject<List<int>>(sequenceIds);
                if (ids == null || !ids.Any())
                {
                    return Error(HttpStatusCode.BadRequest, MessageErrorConstants.DO_NOT_FIND_DATA);
                }

                var now = DateTime.UtcNow;
                var updatedCount = await _context.Accounts
                    .Where(x => ids.Contains(x.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Status, status)
                        .SetProperty(p => p.UpdatedBy, updatedBy)
                        .SetProperty(p => p.UpdatedAt, now)
                    );

                if (updatedCount == 0)
                {
                    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);
                }

                //Trả về danh sách đã được cập nhật trạng thái, nếu không cần thiết có thể bỏ qua bước này để tối ưu hiệu năng
                var datas = await _context.Accounts.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();

                return new ResponseData<List<MRes_Account>>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
                {
                    data = _mapper.Map<List<MRes_Account>>(datas)
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(UpdateStatusList), new { sequenceIds, status, updatedBy });
            }
        }

        /// <summary>
        /// Xóa cứng, chỉ xóa khi không còn ràng buộc dữ liệu với bảng khác (thường hiếm khi sử dụng)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseData<int>> Delete(int id)
        {
            try
            {
                var deletedCount = await _context.Accounts.Where(x => x.Id == id).ExecuteDeleteAsync();

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
        /// Lấy chỉ tiết bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseData<MRes_Account>> GetById(int id)
        {
            try
            {
                var data = await _context.Accounts
                    .AsNoTracking()
                    .ProjectTo<MRes_Account>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return new ResponseData<MRes_Account>
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
        /// Lấy danh sách theo tham số đầy đủ, có phân trang, dùng cho các trường hợp cần lấy dữ liệu theo điều kiện lọc và cần quan tâm đến phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Account>>> GetListByPaging(MReq_Account_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                int count = await query.CountAsync();
                List<MRes_Account> data = new List<MRes_Account>();

                if (count > 0)
                {
                    int page = request.Page > 0 ? request.Page : 1;
                    int record = request.Record > 0 ? request.Record : 10;

                    // SQL Server 2008 R2: fetch all rồi paging in-memory
                    var allData = await query
                        .OrderBy(x => x.UserName)
                        .ProjectTo<MRes_Account>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    data = allData.Skip((page - 1) * record).Take(record).ToList();
                }

                return new ResponseData<List<MRes_Account>>
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
        /// Lấy danh sách theo tham số đầy đủ, không phân trang, dùng cho các trường hợp cần lấy hết dữ liệu theo điều kiện lọc, hoặc chỉ lấy 1 vài bản ghi theo điều kiện lọc mà không cần quan tâm đến phân trang
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Account>>> GetListByFullParam(MReq_Account_FullParam request)
        {
            try
            {
                var query = BuildFilterQuery(request);

                var data = await query
                    .ProjectTo<MRes_Account>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return new ResponseData<List<MRes_Account>>
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

        /// <summary>
        /// Lấy danh sách theo tham số đầy đủ bằng stored procedure
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<MRes_Account>>> GetListBySpFullParam(MReq_Account_FullParam request)
        {
            try
            {
                string[] arrParams = {
                    "@SequenceStatus",
                    "@SearchText",
                    "@Page",
                    "@Record"
                };
                object[] arrValues = {
                    request.SequenceStatus,
                    request.SearchText,
                    request.Page,
                    request.Record
                };
                var callSp = await StoreProcedure.GetListAsync<MRes_Account>(_context.Database.GetConnectionString(), "sp_account_getlist_by_fullparam", arrParams, arrValues);
                return new ResponseData<List<MRes_Account>>
                {
                    data = callSp,
                    result = 1
                };
            }
            catch (Exception ex)
            {
                return CatchException(ex, nameof(GetListBySpFullParam), request);
            }
        }

        #region Common method
        // Hàm dùng chung để Build điều kiện Filter (cần thiết đưa riêng khi muốn tái sử dụng logic lọc)
        private IQueryable<Account> BuildFilterQuery(MReq_Account_FullParam request)
        {
            var status = request.SequenceStatus?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(short.Parse)
                .ToArray() ?? Array.Empty<short>();

            var query = _context.Accounts.AsNoTracking();

            if (status.Length > 0)
                query = query.Where(x => status.Contains(x.Status));

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                var searchTextLower = request.SearchText.ToUpper().Trim();
                query = query.Where(x => x.UserName.StartsWith(searchTextLower) || x.Phone.StartsWith(searchTextLower) || x.Email.StartsWith(searchTextLower));
            }

            return query;
        }
        #endregion
    }
}