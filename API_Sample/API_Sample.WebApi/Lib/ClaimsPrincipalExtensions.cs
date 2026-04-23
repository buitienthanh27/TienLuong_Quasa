using System.Security.Claims;

namespace API_Sample.WebApi.Lib
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Lấy Account Id (hoặc UserId) từ JWT Token của request hiện tại.
        /// Sử dụng trong Controller: int accountId = User.GetAccountId();
        /// Thường dùng dể lấy thông tin người dùng đã đăng nhập để ghi vào CreatedBy, UpdatedBy, hoặc kiểm tra quyền hạn của người dùng.
        /// </summary>
        public static int GetAccountId(this ClaimsPrincipal user)
        {
            if (user == null) return 0;

            // Đổi "AccountId" thành loại chuỗi claim mà hệ thống bên lúc phát JWT đang sử dụng. 
            // Chuẩn mặc định của .NET thường là ClaimTypes.NameIdentifier (nếu không tìm thấy "AccountId")
            var value = user.FindFirstValue("AccountId") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrWhiteSpace(value)) return 0;

            return int.TryParse(value, out var id) ? id : 0;
        }

        /// <summary>
        /// Lấy Account Id (hoặc UserId) từ JWT Token của request hiện tại.
        /// Sử dụng trong Controller: int accountId = User.GetAccountId();
        /// Thường dùng dể lấy thông tin người dùng đã đăng nhập để ghi vào CreatedBy, UpdatedBy, hoặc kiểm tra quyền hạn của người dùng.
        /// </summary>
        public static string GetUserName(this ClaimsPrincipal user)
        {
            if (user == null) return string.Empty;

            // Đổi "AccountId" thành loại chuỗi claim mà hệ thống bên lúc phát JWT đang sử dụng. 
            // Chuẩn mặc định của .NET thường là ClaimTypes.NameIdentifier (nếu không tìm thấy "AccountId")
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            return value;
        }
    }
}
