# Kiến Trúc Tổng Quan - API_Sample

## 1. Thông Tin Dự Án

**Tên**: API_Sample - Hệ thống Quản lý Lương và Nhân sự (HRMS)  
**Stack**: ASP.NET Core, EF Core, SQL Server, JWT Authentication  
**Pattern**: N-Tier Architecture (5 Projects)

---

## 2. Cấu Trúc Solution

```
API_Sample.sln
├── API_Sample.WebApi          (Presentation Layer)
├── API_Sample.Application     (Business Logic Layer)
├── API_Sample.Data            (Data Access Layer)
├── API_Sample.Models          (DTO Layer - Shared)
└── API_Sample.Utilities       (Cross-cutting - Shared)
```

### Dependency Direction
```
WebApi ──────────────────────────────────────────────┐
   │                                                 │
   ▼                                                 │
Application ─────────────────────────────────────────┤
   │                                                 │
   ▼                                                 │
Data ────────────────────────────────────────────────┘
   │                                                 │
   └─────────────► Models & Utilities ◄──────────────┘
                   (Shared bởi tất cả)
```

---

## 3. Chi Tiết Từng Layer

### 3.1 API_Sample.WebApi (Presentation)

**Vai trò**: Entry point, HTTP handling, Authentication, Middleware

| Thư mục | Nội dung |
|---------|----------|
| `Controllers/` | 9 thin controllers - chỉ gọi service và return |
| `Middlewares/` | SecurityHeadersMiddleware, Timezone |
| `Lib/` | ClaimsPrincipalExtensions (User.GetAccountId) |
| `Logs/` | File log output |
| `Program.cs` | DI, JWT config, Swagger, Rate limit, Middleware pipeline |

**Quy tắc**:
- Controller KHÔNG chứa business logic
- Chỉ làm 3 việc: gán identity, gọi service, return Ok(res)
- Mọi response đều là `Ok(ResponseData<T>)`

### 3.2 API_Sample.Application (Business Logic)

**Vai trò**: Core business logic, Service classes

| Thư mục | Nội dung |
|---------|----------|
| `Services/` | 10 service files (S_*.cs) - mỗi file chứa interface IS_* + class S_* |
| `Ultilities/` | BaseService, JwtHelper, SendMailSMTP, StoreProcedure, CallApi |
| `Mapper/` | AutoMapperProfile.cs - tất cả mapping configs |
| `ServiceExternal/` | Wrapper gọi API bên ngoài |

**Quy tắc**:
- Mọi service kế thừa `BaseService<TService>`
- Dùng `Error(HttpStatusCode, message)` cho lỗi logic
- Dùng `CatchException(ex, methodName, params)` cho exception
- KHÔNG throw exception trong service

### 3.3 API_Sample.Data (Data Access)

**Vai trò**: EF Core DbContext, Entity definitions

| Thư mục | Nội dung |
|---------|----------|
| `EF/` | MainDbContext.cs - DbContext duy nhất |
| `Entities/` | 14 entity classes |
| `Migrations/` | EF Core migrations |

**Entities (14)**:
```
Core:           Account, Image, Product
Organizational: Tram, Department, Position, Employee
Payroll Config: SystemParameter, SalaryScale, AllowanceType, CostCenter
Activities:     Attendance, Performance, Allowance
Payroll:        Payroll, PayrollDetail, CostAllocation
Audit:          AuditLog
```

### 3.4 API_Sample.Models (DTOs)

**Vai trò**: Data Transfer Objects - request/response models

| Thư mục | Nội dung |
|---------|----------|
| `Request/` | 10 MReq_*.cs files |
| `Response/` | 12 MRes_*.cs files |
| `Common/` | ResponseData<T>, BaseModel, PagingRequestBase |
| `Enums/` | EN_Account_Status, etc. |

**Naming Convention**:
- Request: `MReq_{Entity}`, `MReq_{Entity}_FullParam`
- Response: `MRes_{Entity}`

### 3.5 API_Sample.Utilities (Cross-cutting)

**Vai trò**: Shared utilities, constants, helpers

| File | Chức năng |
|------|-----------|
| `Constants/MessageErrorConstants.cs` | Thông báo lỗi chuẩn |
| `Constants/TimeZoneConstants.cs` | Timezone config |
| `Encryptor.cs` | Hash, encrypt utilities |
| `StringHelper.cs` | String manipulation, slug |
| `Utilities.cs` | CurrentTimeSeconds, misc helpers |
| `DataException.cs` | Custom exception types |

---

## 4. Request Flow

```
┌─────────────────────────────────────────────────────────────────────┐
│                         CLIENT REQUEST                              │
│                    (HTTP + JWT Bearer Token)                        │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      MIDDLEWARE PIPELINE                            │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  1. SecurityHeadersMiddleware (X-Frame-Options, CSP, etc)   │   │
│  │  2. Rate Limiting (IpRateLimiting)                          │   │
│  │  3. Authentication (JWT Bearer - validate token)            │   │
│  │  4. Authorization ([Authorize] attribute check)             │   │
│  │  5. ValidationMiddleware (ModelState → ResponseData 400)    │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                          CONTROLLER                                 │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  1. Gán identity: request.CreatedBy = User.GetAccountId()   │   │
│  │  2. Gọi service: var res = await _s_Entity.Method(request)  │   │
│  │  3. Return: return Ok(res)                                  │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                           SERVICE                                   │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  try {                                                      │   │
│  │      // 1. Validate business rules                          │   │
│  │      if (duplicate) return Error(Conflict, "msg");          │   │
│  │                                                             │   │
│  │      // 2. Map DTO → Entity                                 │   │
│  │      var data = _mapper.Map<Entity>(request);               │   │
│  │                                                             │   │
│  │      // 3. Set audit fields                                 │   │
│  │      data.CreatedAt = DateTime.UtcNow;                      │   │
│  │                                                             │   │
│  │      // 4. Save to DB                                       │   │
│  │      _context.Entities.Add(data);                           │   │
│  │      await _context.SaveChangesAsync();                     │   │
│  │                                                             │   │
│  │      // 5. Return success                                   │   │
│  │      return new ResponseData<T>(1, 201, "Success") {...}    │   │
│  │  }                                                          │   │
│  │  catch (Exception ex) {                                     │   │
│  │      return CatchException(ex, nameof(Method), request);    │   │
│  │  }                                                          │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         MainDbContext                               │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  EF Core → SQL Server                                       │   │
│  │  14 DbSet<Entity> properties                                │   │
│  │  Relationships, Indexes, Constraints                        │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      RESPONSE TO CLIENT                             │
│  {                                                                  │
│    "result": 1,           // 1=success, 0=logic error, -1=exception│
│    "time": 1734567890,    // Unix timestamp                        │
│    "data": {...},         // Main payload                          │
│    "data2nd": 100,        // Secondary (e.g., total count)         │
│    "error": { "code": 200, "message": "Success" }                  │
│  }                                                                  │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 5. Patterns & Best Practices

### 5.1 Service Pattern
```csharp
public class S_Entity : BaseService<S_Entity>, IS_Entity
{
    private readonly IMapper _mapper;
    
    // Constructor injection
    public S_Entity(MainDbContext context, IMapper mapper, ILogger<S_Entity> logger)
        : base(context, logger) { _mapper = mapper; }
    
    // Standard CRUD methods
    public async Task<ResponseData<MRes_Entity>> Create(MReq_Entity request) { ... }
    public async Task<ResponseData<MRes_Entity>> Update(MReq_Entity request) { ... }
    public async Task<ResponseData<MRes_Entity>> UpdateStatus(int id, short status, int updatedBy) { ... }
    public async Task<ResponseData<List<MRes_Entity>>> UpdateStatusList(string sequenceIds, short status, int updatedBy) { ... }
    public async Task<ResponseData<int>> Delete(int id) { ... }
    public async Task<ResponseData<MRes_Entity>> GetById(int id) { ... }
    public async Task<ResponseData<List<MRes_Entity>>> GetListByPaging(MReq_Entity_FullParam request) { ... }
    public async Task<ResponseData<List<MRes_Entity>>> GetListByFullParam(MReq_Entity_FullParam request) { ... }
    
    #region Common functions
    private IQueryable<Entity> BuildFilterQuery(MReq_Entity_FullParam request) { ... }
    #endregion
}
```

### 5.2 Error Handling
```csharp
// Logic error (result = 0)
return Error(HttpStatusCode.Conflict, "Mã đã tồn tại!");
return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

// System exception (result = -1) - auto log & rollback
return CatchException(ex, nameof(MethodName), request);
```

### 5.3 Query Patterns
```csharp
// Read-only: ALWAYS use AsNoTracking
var data = await _context.Entities.AsNoTracking()...

// Map to DTO: use ProjectTo
.ProjectTo<MRes_Entity>(_mapper.ConfigurationProvider)

// Bulk update: use ExecuteUpdateAsync
await _context.Entities.Where(x => ids.Contains(x.Id))
    .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, status));

// Bulk delete: use ExecuteDeleteAsync
await _context.Entities.Where(x => x.Id == id).ExecuteDeleteAsync();

// Soft delete: set Status = -1
data.Status = -1;
```

---

## 6. Authentication & Authorization

### JWT Configuration
```csharp
// Token structure
Claims:
- AccountId (custom claim)
- Email (ClaimTypes.Email)
- Phone (ClaimTypes.MobilePhone)
- UserName (ClaimTypes.NameIdentifier)

// Token lifetime
- Access Token: 30 minutes
- Refresh Token: 7 days (stored in DB)
```

### Lấy User ID trong Controller
```csharp
var accountId = User.GetAccountId(); // Extension method
request.CreatedBy = accountId;
```

---

## 7. Database Conventions

### Entity Structure
```csharp
// Required fields for all business entities
public int Id { get; set; }              // PK
public short Status { get; set; }        // -1=deleted, 0=inactive, 1=active
public DateTime CreatedAt { get; set; }
public int CreatedBy { get; set; }
public DateTime? UpdatedAt { get; set; }
public int? UpdatedBy { get; set; }

// Column naming
[Column("snake_case_name")]              // DB column = snake_case
public string PropertyName { get; set; } // C# property = PascalCase
```

### Unique Indexes (quan trọng)
```
Tram:            (Code)
Department:      (Code)
Position:        (Code)
Employee:        (Msnv)
CostCenter:      (Code)
AllowanceType:   (Code)
SystemParameter: (ParamCode, EffectiveDate)
SalaryScale:     (TramId, Grade, EffectiveDate)
Attendance:      (EmployeeId, YearMonth)
Performance:     (EmployeeId, YearMonth)
Payroll:         (EmployeeId, YearMonth)
```

---

## 8. Files Cần Đọc Khi Code Module Mới

1. **Trước khi code**:
   - `.claude/CLAUDE.md` - Quy tắc tổng quan
   - `.claude/rules/*.md` - Tất cả coding rules
   - `docs/modules/{ModuleName}.md` - Tài liệu module cụ thể

2. **Files tham khảo**:
   - `API_Sample.Application/Services/S_Product.cs` - Service mẫu CRUD
   - `API_Sample.Application/Services/S_Payroll.cs` - Service phức tạp
   - `API_Sample.Application/Ultilities/BaseService.cs` - Base class

3. **Khi thêm module mới**:
   - Entity → `API_Sample.Data/Entities/`
   - DbSet → `API_Sample.Data/EF/MainDbContext.cs`
   - DTOs → `API_Sample.Models/Request/` & `Response/`
   - Mapping → `API_Sample.Application/Mapper/AutoMapperProfile.cs`
   - Service → `API_Sample.Application/Services/S_{Entity}.cs`
   - Controller → `API_Sample.WebApi/Controllers/{Entity}Controller.cs`
   - DI → `API_Sample.WebApi/Program.cs`
