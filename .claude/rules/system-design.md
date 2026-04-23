# System Design — API_Sample

## Request lifecycle

```
Client
  │ HTTP + JWT
  ▼
[Program.cs middleware pipeline]
  ├─ SecurityHeadersMiddleware
  ├─ AspNetCoreRateLimit (IpRateLimit)
  ├─ Authentication (JWT Bearer)
  ├─ Authorization
  └─ ValidationMiddleware (ModelState → ResponseData 400)
  ▼
[Controller]  ── thin: gán identity + gọi service
  ▼
[Service S_X : BaseService<S_X>]  ── business logic
  ├─ try { ... validate, query, save ... }
  ├─ Error(HttpStatusCode, msg)        → result = 0
  ├─ CatchException(ex, name, params)  → result = -1, auto log + rollback
  └─ return ResponseData<T>(1, ...)    → result = 1
  ▼
[MainDbContext] ── EF Core → SQL Server
  ▼
ResponseData<T> JSON (HTTP 200) → Client
```

## Kiến trúc layer

| Layer | Project | Trách nhiệm | Cấm |
|-------|---------|------------|-----|
| Presentation | `API_Sample.WebApi` | HTTP, DI, middleware, JWT | Business logic, truy cập DbContext |
| Application | `API_Sample.Application` | Business logic, transaction | Truy cập HttpContext, trả về `IActionResult` |
| Data | `API_Sample.Data` | EF Core, entity | Tham chiếu Application/WebApi |
| Models | `API_Sample.Models` | DTO thuần | Logic, EF dependency |
| Utilities | `API_Sample.Utilities` | Helper tĩnh | Phụ thuộc DbContext |

Dependency direction: **WebApi → Application → Data**. Models & Utilities được mọi tầng dùng.

## Standard response — `ResponseData<T>`

| Field | Ý nghĩa |
|-------|---------|
| `result` | `1` thành công · `0` lỗi nghiệp vụ · `-1` exception hệ thống |
| `time` | Unix seconds khi tạo response |
| `data` | Payload chính (DTO) |
| `data2nd` | Payload phụ — paging total count, meta |
| `error.code` | HTTP status logic (200/404/409/500...) |
| `error.message` | Mô tả lỗi tiếng Việt |

HTTP status thật luôn là 200 (trừ ModelState 400 từ middleware). Mọi xử lý success/fail dựa vào `result`.

## Transaction model

- DbContext scoped per request (DI mặc định).
- Transaction tường minh khi cần atomic nhiều `SaveChanges`:
  ```csharp
  using var tx = await _context.Database.BeginTransactionAsync();
  // ... nhiều bước ...
  await tx.CommitAsync();
  ```
- `Error()` và `CatchException()` tự rollback nếu phát hiện `CurrentTransaction != null`.

## Service pattern chuẩn

Mỗi entity có 1 service `S_X` cung cấp 8–9 method (xem `naming-conventions.md`). Pattern:

1. Validate input (trùng mã, tồn tại, ràng buộc).
2. Map DTO → Entity (`_mapper.Map`).
3. Set audit (`CreatedAt/By`, `UpdatedAt/By`).
4. `SaveChangesAsync` → check `>0`.
5. Trả `ResponseData<MRes_X>` với `_mapper.Map` ngược.

Filter dùng chung tách hàm `BuildFilterQuery(request) → IQueryable<X>` trong `#region Common functions`.

## DI registration

Trong `Program.cs`:
```csharp
builder.Services.AddDbContext<MainDbContext>(opt => opt.UseSqlServer(connStr));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddScoped<IS_Product, S_Product>();
builder.Services.AddScoped<IS_Account, S_Account>();
```

Service mới phải được đăng ký scoped tại đây.

## Audit & soft delete

- `Status = -1` = xoá mềm. Mọi query list `.Where(x => x.Status != -1)`.
- `CreatedBy/UpdatedBy` lấy từ JWT claim (`User.GetAccountId()`), set ở Controller, dùng ở Service.
