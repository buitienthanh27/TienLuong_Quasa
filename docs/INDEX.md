# API_Sample Documentation Index

**Hệ thống**: Quản lý Lương và Nhân sự (HRMS)  
**Stack**: ASP.NET Core, EF Core, SQL Server, JWT Authentication  
**Cập nhật**: 2026-04-22

---

## Hướng Dẫn Sử Dụng Tài Liệu

Khi Agent cần code một feature mới, đọc theo thứ tự:

1. **Đọc quy tắc chung**: `.claude/CLAUDE.md` và `.claude/rules/`
2. **Đọc kiến trúc**: `docs/architecture/OVERVIEW.md`
3. **Đọc module cụ thể**: `docs/modules/{ModuleName}.md`
4. **Tham khảo patterns**: `docs/architecture/COMMON_PATTERNS.md`

---

## Cấu Trúc Tài Liệu

```
docs/
├── INDEX.md                           # File này
├── UI_Design_Specification.md        # Thiết kế UI/UX cho Figma
├── QUASA_BUSINESS_RULES.md           # ★ Quy định nghiệp vụ Quasa (cập nhật 04/2026)
│
├── architecture/                      # Kiến trúc hệ thống
│   ├── OVERVIEW.md                   # Tổng quan N-Tier, request flow
│   ├── COMMON_PATTERNS.md            # Patterns CRUD, ResponseData, BaseService
│   └── TECH_STACK.md                 # NuGet packages, JWT config, EF commands
│
└── modules/                           # Chi tiết từng module
    ├── ACCOUNT.md                    # Authentication, JWT, Login/Logout
    ├── EMPLOYEE.md                   # Quản lý nhân viên
    ├── ATTENDANCE.md                 # Chấm công + ký hiệu chấm công
    ├── PAYROLL.md                    # Tính lương (phức tạp nhất) + quy tắc làm tròn
    ├── TECHNICAL_EVALUATION.md       # Đánh giá kỹ thuật (Đội chấm + Công ty phúc tra)
    ├── ADVANCE_PAYMENT.md            # Tạm ứng (DRC 40%)
    └── MASTER_DATA.md                # Danh mục: Tram, Department, Position, 
                                      # SalaryScale, SystemParameter, CostCenter,
                                      # AllowanceType
```

---

## Quick Reference

### Modules Hiện Có

| Module | Entity | Service | Controller | Độ Phức Tạp |
|--------|--------|---------|------------|-------------|
| Account | Account | S_Account | AccountController | Cao (Auth) |
| Employee | Employee | S_Employee | EmployeeController | Trung bình |
| Tram | Tram | S_Tram | TramController | Thấp |
| Attendance | Attendance | S_Attendance | AttendanceController | Trung bình |
| Performance | Performance | S_Performance | PerformanceController | Trung bình (đánh giá KT) |
| Advance | Advance | S_Advance | AdvanceController | Trung bình (tạm ứng) |
| Allowance | Allowance | - | - | Chưa implement |
| Payroll | Payroll, PayrollDetail | S_Payroll | PayrollController | Cao |
| SalaryScale | SalaryScale | S_SalaryScale | SalaryScaleController | Thấp |
| SystemParameter | SystemParameter | S_SystemParameter | SystemParameterController | Thấp |
| CostCenter | CostCenter | S_CostCenter | CostCenterController | Thấp |

### Entities Trong Database (15)

```
Core:           Account, Image, Product
Organizational: Tram, Department, Position, Employee
Payroll Config: SystemParameter, SalaryScale, AllowanceType, CostCenter
Activities:     Attendance, Performance, Allowance, Advance
Payroll:        Payroll, PayrollDetail, CostAllocation
Audit:          AuditLog
```

### Quy Định Đặc Thù Quasa (Cập nhật 04/2026)

| Quy định | Chi tiết |
|----------|----------|
| **Định dạng số thập phân** | Dấu chấm (.) là dấu phân cách |
| **Số thập phân** | Cắt lấy 2 số (TRUNCATE, không làm tròn) |
| **Cân mủ tươi** | Không có số lẻ (9.3kg → 9kg) |
| **Tổng lương** | Làm tròn đến Ngàn (10,768,456 → 10,768,000) |
| **DRC Tạm ứng** | 40% (fixed) |
| **Đánh giá KT** | Đội chấm (default) + Công ty phúc tra (override) |

### Service Method Chuẩn

Mỗi service nên có các method sau (nếu applicable):

```csharp
// Create
Task<ResponseData<MRes_Entity>> Create(MReq_Entity request)

// Update
Task<ResponseData<MRes_Entity>> Update(MReq_Entity request)

// Update Status (single)
Task<ResponseData<MRes_Entity>> UpdateStatus(int id, short status, int updatedBy)

// Update Status (bulk)
Task<ResponseData<List<MRes_Entity>>> UpdateStatusList(string sequenceIds, short status, int updatedBy)

// Delete (hard)
Task<ResponseData<int>> Delete(int id)

// Get by ID
Task<ResponseData<MRes_Entity>> GetById(int id)

// Get list with paging
Task<ResponseData<List<MRes_Entity>>> GetListByPaging(MReq_Entity_FullParam request)

// Get list without paging
Task<ResponseData<List<MRes_Entity>>> GetListByFullParam(MReq_Entity_FullParam request)
```

---

## Thêm Module Mới - Checklist

### 1. Entity (API_Sample.Data)
- [ ] Tạo file `Entities/{Entity}.cs`
- [ ] Kế thừa `BaseEntity` (có Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
- [ ] Đặt `[Table("snake_case")]` và `[Column("snake_case")]`
- [ ] Thêm `DbSet<Entity>` vào `MainDbContext.cs`
- [ ] Cấu hình relationships và indexes trong `OnModelCreating`

### 2. DTOs (API_Sample.Models)
- [ ] Tạo `Request/MReq_{Entity}.cs` kế thừa `BaseModel.History`
- [ ] Tạo `Request/MReq_{Entity}_FullParam` kế thừa `PagingRequestBase`
- [ ] Tạo `Response/MRes_{Entity}.cs`

### 3. AutoMapper (API_Sample.Application)
- [ ] Thêm `CreateMap<MReq_{Entity}, Entity>()` (ignore audit fields)
- [ ] Thêm `CreateMap<Entity, MRes_{Entity}>()` (include navigation)

### 4. Service (API_Sample.Application)
- [ ] Tạo `Services/S_{Entity}.cs` với interface `IS_{Entity}` và class `S_{Entity}`
- [ ] Kế thừa `BaseService<S_{Entity}>`
- [ ] Implement các method chuẩn
- [ ] Thêm `BuildFilterQuery` trong `#region Common functions`

### 5. Controller (API_Sample.WebApi)
- [ ] Tạo `Controllers/{Entity}Controller.cs`
- [ ] Đặt `[ApiController]`, `[Route("[controller]/[action]")]`, `[Authorize]`
- [ ] Inject service qua constructor
- [ ] Gọi `User.GetAccountId()` cho CreatedBy/UpdatedBy

### 6. DI Registration (API_Sample.WebApi)
- [ ] Thêm `builder.Services.AddScoped<IS_{Entity}, S_{Entity}>()` trong `Program.cs`

---

## Lưu Ý Quan Trọng

### KHÔNG ĐƯỢC:
- Viết business logic trong Controller
- Throw exception trong Service (dùng `Error()` và `CatchException()`)
- Trả về `BadRequest()`, `NotFound()` trong Controller (luôn `return Ok(res)`)
- Dùng `.Update()` + `SaveChanges()` cho bulk update
- Quên `.AsNoTracking()` cho query read-only
- Hardcode CreatedBy/UpdatedBy (lấy từ `User.GetAccountId()`)

### PHẢI:
- Mọi response đều là `ResponseData<T>`
- Mọi method service đều bọc trong `try/catch` với `CatchException()`
- Normalize input (Trim, ToUpper cho code fields)
- Check unique trước khi Create/Update
- Check dependencies trước khi Delete
- Dùng `ProjectTo<>()` cho query list
- Dùng `ExecuteUpdateAsync`/`ExecuteDeleteAsync` cho bulk operations

---

## Liên Hệ & Tham Khảo

- **Coding Rules**: `.claude/rules/`
- **Existing Services**: `API_Sample.Application/Services/`
- **Sample Code**: Xem `S_Product.cs` cho CRUD cơ bản, `S_Payroll.cs` cho logic phức tạp
