---
description: Skill to review security risks in the Transport Management SaaS codebase
---

# Security Review Skill - Transport Management SaaS

## Mục đích

Skill này hướng dẫn kiểm tra bảo mật cho dự án multi-tenant Transport Management SaaS.

## Khi nào kích hoạt

- Trước khi merge code liên quan đến authentication/authorization
- Khi review code xử lý dữ liệu nhạy cảm
- Khi thêm endpoint mới
- Khi thay đổi tenant isolation logic

## Security Checklist

### 1. Multi-Tenant Isolation (CRITICAL)

```csharp
// ✅ ĐÚNG: Dùng ITenantContext
var tenantId = _tenantContext.TenantId;

// ❌ SAI: Hardcode hoặc lấy từ user input
var tenantId = Guid.Parse(request.TenantId); // KHÔNG!
```

Kiểm tra:
- [ ] Mọi entity kế thừa `BaseEntity` (có TenantId)
- [ ] Global Query Filter được áp dụng
- [ ] Không có query bypass tenant filter
- [ ] Không có cross-tenant data access

### 2. Authentication & Authorization

```csharp
// ✅ Endpoint phải có authorization
group.MapPost("/", handler).RequireAuthorization("AdminAccess");

// ❌ Endpoint không được public
group.MapPost("/", handler); // THIẾU authorization!
```

Kiểm tra:
- [ ] Mọi endpoint admin có `RequireAuthorization("AdminAccess")`
- [ ] Driver endpoints kiểm tra DriverId từ token
- [ ] Rate limiting cho login endpoints
- [ ] JWT token validation đúng

### 3. Input Validation

```csharp
// ✅ FluentValidation cho mọi Command
public class CreateTripValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripValidator()
    {
        RuleFor(x => x.Weight).GreaterThan(0);
        RuleFor(x => x.DriverId).NotEmpty();
    }
}
```

Kiểm tra:
- [ ] Mọi Command có Validator
- [ ] Không trust user input trực tiếp
- [ ] Sanitize string inputs
- [ ] Validate enum values

### 4. Sensitive Data

| Dữ liệu | Quy tắc |
|---------|---------|
| Password | Bcrypt hash, KHÔNG plaintext |
| JWT Secret | Env variable, KHÔNG hardcode |
| OTP | Expire 5 phút, single-use |
| Connection String | Env variable |
| API Keys | Env variable, KHÔNG commit |

Kiểm tra:
- [ ] Không log passwords, tokens, OTP
- [ ] Không trả stack trace trong production
- [ ] Sensitive config trong appsettings không commit

### 5. SQL Injection

```csharp
// ✅ ĐÚNG: Dùng EF Core với parameters
var trips = await _context.Trips
    .Where(t => t.DriverId == driverId)
    .ToListAsync();

// ❌ SAI: Raw SQL với string concatenation
var sql = $"SELECT * FROM Trips WHERE DriverId = '{driverId}'"; // NGUY HIỂM!
```

Kiểm tra:
- [ ] Không có raw SQL với string concatenation
- [ ] Dùng parameterized queries nếu cần raw SQL

### 6. XSS Prevention

```typescript
// ✅ Vue auto-escapes by default
<span>{{ userInput }}</span>

// ❌ SAI: v-html với user input
<div v-html="userInput"></div> // NGUY HIỂM!
```

Kiểm tra:
- [ ] Không dùng v-html với user input
- [ ] Sanitize HTML nếu cần render

### 7. CORS Configuration

```csharp
// Kiểm tra CORS origins
policy.WithOrigins("https://your-domain.com") // Chỉ production domain
      .AllowCredentials();
```

Kiểm tra:
- [ ] Production CORS không cho phép wildcard *
- [ ] AllowCredentials chỉ với specific origins

## Red Flags - DỪNG và báo cáo

| Vấn đề | Mức độ |
|--------|--------|
| Hardcoded TenantId | 🔴 CRITICAL |
| Missing authorization | 🔴 CRITICAL |
| Password trong logs | 🔴 CRITICAL |
| Stack trace exposed | 🟡 HIGH |
| Missing validation | 🟡 HIGH |
| Raw SQL | 🟡 HIGH |
| v-html với user input | 🟡 HIGH |

## Audit Log Events

Kiểm tra các events cần audit:
- [ ] Login success/fail
- [ ] Permission denied
- [ ] Tenant mismatch
- [ ] Data modification (create/update/delete)

## Tham khảo

- Xem `.Codex/rules/security.md` cho chi tiết
- Xem `src/TransportManagement.API/Middleware/TenantMiddleware.cs`
- Xem `src/TransportManagement.API/Diagnostics/ISecurityAuditWriter.cs`
