# Security — API_Sample

## Authentication

- **JWT Bearer** bắt buộc cho mọi endpoint trừ `[AllowAnonymous]` (login, register, public lookup).
- Token issue/validate qua `JwtHelper` (`API_Sample.Application/Ultilities/JwtHelper.cs`).
- Cấu hình secret/issuer/audience trong `appsettings.json` → KHÔNG commit secret thật. Production dùng env var hoặc secret manager.
- Lấy account id: `User.GetAccountId()`. KHÔNG đọc `User.Identity.Name` thủ công.

## Authorization

- `[Authorize]` mặc định ở class Controller.
- Check role/permission ở Service nếu cần phân quyền chi tiết.

## Password / data sensitive

- Hash qua `Encryptor` (`API_Sample.Utilities/Encryptor.cs`). KHÔNG lưu plain text.
- KHÔNG log password, token, JWT vào `_logger`.
- Khi `CatchException` serialize parameter, đảm bảo DTO không chứa field nhạy cảm — đánh dấu `[JsonIgnore]` trên `MReq_X.Password` nếu cần.

## Rate limiting

- AspNetCoreRateLimit cấu hình trong `appsettings.json` (`IpRateLimiting`).
- Endpoint nhạy cảm (login) cần policy nghiêm ngặt hơn — config theo route.

## CORS

- Cấu hình allowed origins trong `Program.cs`. KHÔNG dùng `AllowAnyOrigin()` ở production.

## SQL injection

- LINQ + EF Core đã parameterize. Khi dùng `StoreProcedure.GetListAsync` truyền qua `arrParams`/`arrValues` — KHÔNG nối chuỗi SQL.

## Validation đầu vào

- Tận dụng DataAnnotation (`[Required]`, `[StringLength]`, `[EmailAddress]`, `[RegularExpression]`).
- Sanitize search text: trim + uppercase cho code, slug cho name. Không pass raw HTML vào response.

## Headers

- `SecurityHeadersMiddleware` set `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, `Content-Security-Policy`. Không tắt.

## HTTPS

- Bắt buộc `app.UseHttpsRedirection()` ở production. Dev có thể bỏ qua.

## Secret management

- `appsettings.Production.json` KHÔNG chứa connection string thật — dùng `Environment.GetEnvironmentVariable` hoặc User Secrets.
- `.gitignore` phải loại trừ `appsettings.*.json` chứa secret.

## Audit log

- Mọi action mutate (Create/Update/Delete) lưu `CreatedBy/UpdatedBy = User.GetAccountId()`.
- Exception log qua `CatchException` → file log trong `API_Sample.WebApi/Logs/`.
