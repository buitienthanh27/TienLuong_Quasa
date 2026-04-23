# Monitoring & Logging — API_Sample

## Logging

- Standard: **Microsoft.Extensions.Logging** (`ILogger<TService>`), inject vào `BaseService<T>`.
- Output: console (development) + file `API_Sample.WebApi/Logs/` (theo cấu hình `Program.cs`).
- Cấp log:
  - `LogInformation` — bước nghiệp vụ quan trọng (login thành công, gửi email, gọi API ngoài).
  - `LogWarning` — validation fail lặp lại, rate limit hit, dữ liệu nghi ngờ.
  - `LogError` — exception (đã có `CatchException` lo).

## Pattern bắt buộc

Mọi exception trong service phải đi qua `CatchException(ex, nameof(Method), parameters)`. Output log có format:
```
{ServiceName}.{MethodName} Exception_Logger. Parameters: {json}
```

KHÔNG `try/catch` rồi `_logger.LogError` thủ công — luôn `return CatchException(...)`.

## Audit trail

- Lưu trong DB qua `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy` của entity.
- Hành động xoá → `Status = -1` + `UpdatedAt/By` (giữ lịch sử).
- Cần audit chi tiết hơn (history table) → tạo entity `{Entity}History` riêng, ghi trong service.

## Performance / health

- Endpoint health check: nếu chưa có, thêm `app.MapHealthChecks("/health")` trong `Program.cs`.
- Long query → cân nhắc `StoreProcedure.GetListAsync` thay vì LINQ phức tạp.
- Theo dõi rate limit log của `AspNetCoreRateLimit` để tune `IpRateLimiting` policy.

## Thông tin nhạy cảm

- KHÔNG log: password, JWT, refresh token, OTP.
- DTO chứa field nhạy cảm khi log qua `CatchException` → đánh `[JsonIgnore]` trên property hoặc tạo DTO log riêng.

## Production

- Log level production: `Information` cho app, `Warning` cho `Microsoft.*` và `System.*`.
- Rotate file log (Serilog rolling file hoặc tool ngoài).
- Không expose stack trace cho client — `error.message` chỉ chứa thông điệp ngắn (đã được `CatchException` chuẩn hoá).
