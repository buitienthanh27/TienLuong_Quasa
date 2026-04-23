# Tech Stack — API_Sample

Stack đã phê duyệt. KHÔNG thêm dependency mới mà không thảo luận.

## Runtime & framework

- **.NET / ASP.NET Core** (Web API, project .csproj theo SDK style).
- **Entity Framework Core** + provider **SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer`).
- **AutoMapper** + `AutoMapper.Extensions.Microsoft.DependencyInjection` cho DI và `AutoMapper.Extensions.QueryableExtensions` (`ProjectTo`).
- **Newtonsoft.Json** cho serialize log và deserialize `sequenceIds`.

## Authentication & security

- **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`).
- Helper: `API_Sample.Application/Ultilities/JwtHelper.cs`.
- Claims extension: `API_Sample.WebApi/Lib/ClaimsPrincipalExtensions.cs` → `User.GetAccountId()`.
- **AspNetCoreRateLimit** — IP rate limit, cấu hình `appsettings.json` (`IpRateLimiting`).
- **SecurityHeadersMiddleware** — trong `API_Sample.WebApi/Middlewares/`.
- **Encryptor** — `API_Sample.Utilities/Encryptor.cs` cho hash password, mã hoá string.

## Logging

- **ILogger<T>** chuẩn của ASP.NET Core, output ra console + file (`API_Sample.WebApi/Logs/`).

## Documentation

- **Swashbuckle / Swagger** — Swagger UI ở `/swagger`. JWT Authorize button đã setup.

## Email & external

- SMTP: `API_Sample.Application/Ultilities/SendMailSMTP.cs`.
- Gọi API ngoài: `Base_CallApi.cs` + `CallApi.cs`. Wrapper service đặt trong `ServiceExternal/`.

## Stored procedure

- Helper `StoreProcedure.GetListAsync<T>` (Application/Ultilities) — gọi raw ADO.NET.

## Khi cần thêm package

1. Kiểm tra package đã có trong `*.csproj` chưa.
2. Nếu phải thêm: thêm vào project hợp lý (vd EF package vào `API_Sample.Data`, không vào `WebApi`).
3. Cập nhật rule này nếu là dependency cốt lõi.

## Cấm

- Không dùng MediatR / CQRS — dự án dùng Service pattern thuần.
- Không dùng FluentValidation — dùng DataAnnotation trên `MReq_X`.
- Không dùng Dapper song song với EF Core (trừ qua `StoreProcedure` helper hiện có).
- Không thêm Vue/React frontend vào solution.
