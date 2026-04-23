# API Conventions — API_Sample

## Controller pattern

- `[ApiController]` + `[Route("[controller]/[action]")]` + `[Authorize]` mặc định ở class.
- Inject service qua constructor, field `private readonly IS_X _s_X;`.
- Action **chỉ** làm 3 việc: gán identity (`request.CreatedBy = User.GetAccountId()`), gọi service, `return Ok(res)`.
- Không try/catch trong Controller — exception đã bắt ở Service và Middleware.
- Không trả `BadRequest()`, `NotFound()`, `StatusCode(...)` — luôn `Ok(ResponseData<T>)`. HTTP 4xx/5xx do `ValidationMiddleware` (lỗi ModelState) hoặc exception handler đảm nhiệm.

## HTTP verb mapping

| Verb | Action | Ghi chú |
|------|--------|---------|
| `[HttpPost]` | `Create` | Body: `MReq_X` |
| `[HttpPut]` | `Update`, `UpdateStatus`, `UpdateStatusList` | Body hoặc query tuỳ method |
| `[HttpDelete]` | `Delete` | Mặc định gọi `UpdateStatus(id, -1, ...)` (xoá mềm) |
| `[HttpGet]` | `GetById`, `GetListByPaging`, `GetListByFullParam`, `GetListBySpFullParam` | Query: `[FromQuery] MReq_X_FullParam` |

## Authentication

- Mặc định `[Authorize]`. Endpoint công khai (login, register) phải `[AllowAnonymous]` rõ ràng.
- Lấy account id: `User.GetAccountId()` (extension trong `API_Sample.WebApi/Lib/ClaimsPrincipalExtensions.cs`).
- Token issue qua `JwtHelper` (Application/Ultilities). Refresh token nếu có lưu vào entity `Account`.

## Response format — `ResponseData<T>`

```jsonc
{
  "result": 1,          // 1 = success | 0 = logic error | -1 = system exception
  "time": 1734567890,   // unix seconds
  "dataDescription": "",
  "data": { ... },      // payload chính
  "data2nd": 123,       // payload phụ (vd: total count cho paging)
  "error": { "code": 200, "message": "" }
}
```

- Success → `new ResponseData<T>(1, (int)HttpStatusCode.OK, MessageErrorConstants.XXX_SUCCESS) { data = ... }`.
- Logic error → `return Error(HttpStatusCode.X, "thông báo")` từ `BaseService` — sẽ implicit cast sang `ResponseData<T>`.
- Exception → `return CatchException(ex, nameof(Method), request)` — tự log + rollback.
- List có paging: `data` = list, `data2nd` = total count.

## Paging

- Class lọc kế thừa `PagingRequestBase` (có `Page`, `Record`).
- Pattern: `Skip((Page-1)*Record).Take(Record)`.
- Sort mặc định: `OrderBy(x => x.Sort)` nếu entity có cột `Sort`.

## Validation

- Dùng DataAnnotation trên `MReq_X` (`[Required]`, `[StringLength(n)]`, `[EmailAddress]`...).
- ModelState lỗi → middleware tự trả `ResponseData<T>` HTTP 400 với `result=0`.
- Validate nghiệp vụ (trùng code, không tìm thấy bản ghi) → `Error(HttpStatusCode.Conflict|.NotFound, "msg")` trong service.

## Rate Limiting

- Cấu hình qua AspNetCoreRateLimit trong `appsettings.json` + `Program.cs`. Không bypass.

## Swagger

- Mọi controller hiển thị trong Swagger. JWT Bearer được cấu hình sẵn — test bằng nút "Authorize".
