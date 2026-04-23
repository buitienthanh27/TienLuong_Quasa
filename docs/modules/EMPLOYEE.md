# Module: EMPLOYEE (Quản Lý Nhân Viên)

## 1. Tổng Quan

**Mục đích**: Quản lý thông tin nhân viên, liên kết với cơ cấu tổ chức (Trạm, Phòng ban, Chức vụ).

**Entity**: `Employee`

**Relationships**:
- `Tram` (Required) - Trạm/chi nhánh
- `Department` (Optional) - Phòng ban
- `Position` (Optional) - Chức vụ
- `Attendance` (Collection) - Chấm công
- `Performance` (Collection) - Hiệu suất
- `Allowance` (Collection) - Phụ cấp
- `Payroll` (Collection) - Bảng lương

---

## 2. Entity Structure

```csharp
[Table("employee")]
public partial class Employee : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("msnv")]
    [StringLength(20)]
    public string Msnv { get; set; }           // Mã số nhân viên (UNIQUE)

    [Required]
    [Column("full_name")]
    [StringLength(100)]
    public string FullName { get; set; }       // Họ tên đầy đủ

    [Column("full_name_local")]
    [StringLength(100)]
    public string? FullNameLocal { get; set; } // Tên không dấu

    [Column("tram_id")]
    public int TramId { get; set; }            // FK → Tram (Required)

    [Column("department_id")]
    public int? DepartmentId { get; set; }     // FK → Department (Optional)

    [Column("position_id")]
    public int? PositionId { get; set; }       // FK → Position (Optional)

    [Column("technical_grade")]
    [StringLength(5)]
    public string? TechnicalGrade { get; set; } // Bậc kỹ thuật: A, B, C, D, E

    [Column("hire_date", TypeName = "date")]
    public DateTime? HireDate { get; set; }    // Ngày vào làm

    [Column("birth_date", TypeName = "date")]
    public DateTime? BirthDate { get; set; }   // Ngày sinh

    [Column("gender")]
    [StringLength(10)]
    public string? Gender { get; set; }        // Nam/Nữ/Khác

    [Column("insurance_included")]
    public bool InsuranceIncluded { get; set; } = true; // Có đóng BHXH không

    // Navigation properties
    [ForeignKey("TramId")]
    public virtual Tram Tram { get; set; }

    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }

    [ForeignKey("PositionId")]
    public virtual Position? Position { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; }
    public virtual ICollection<Performance> Performances { get; set; }
    public virtual ICollection<Allowance> Allowances { get; set; }
    public virtual ICollection<Payroll> Payrolls { get; set; }
}
```

---

## 3. Service Interface

```csharp
public interface IS_Employee
{
    Task<ResponseData<MRes_Employee>> Create(MReq_Employee request);
    Task<ResponseData<MRes_Employee>> Update(MReq_Employee request);
    Task<ResponseData<MRes_Employee>> UpdateStatus(int id, short status, int updatedBy);
    Task<ResponseData<List<MRes_Employee>>> UpdateStatusList(string sequenceIds, short status, int updatedBy);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_Employee>> GetById(int id);
    Task<ResponseData<List<MRes_Employee>>> GetListByPaging(MReq_Employee_FullParam request);
    Task<ResponseData<List<MRes_Employee>>> GetListByFullParam(MReq_Employee_FullParam request);
}
```

---

## 4. API Endpoints

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/Employee/Create` | Thêm nhân viên mới |
| PUT | `/Employee/Update` | Cập nhật thông tin |
| PUT | `/Employee/UpdateStatus` | Cập nhật trạng thái 1 NV |
| PUT | `/Employee/UpdateStatusList` | Cập nhật trạng thái nhiều NV |
| DELETE | `/Employee/Delete/{id}` | Xóa cứng (nếu chưa có lương) |
| GET | `/Employee/GetById/{id}` | Xem chi tiết |
| GET | `/Employee/GetListByPaging` | Danh sách có phân trang |
| GET | `/Employee/GetListByFullParam` | Danh sách không phân trang |

---

## 5. Request/Response Models

### 5.1 MReq_Employee
```csharp
public class MReq_Employee : BaseModel.History
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(20)]
    public string Msnv { get; set; }          // Auto uppercase + trim
    
    [Required]
    [StringLength(100)]
    public string FullName { get; set; }
    
    [StringLength(100)]
    public string? FullNameLocal { get; set; }
    
    [Required]
    public int TramId { get; set; }
    
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    
    [StringLength(5)]
    public string? TechnicalGrade { get; set; }
    
    public DateTime? HireDate { get; set; }
    public DateTime? BirthDate { get; set; }
    
    [StringLength(10)]
    public string? Gender { get; set; }
    
    public bool InsuranceIncluded { get; set; } = true;
}
```

### 5.2 MReq_Employee_FullParam
```csharp
public class MReq_Employee_FullParam : PagingRequestBase
{
    public string? SearchText { get; set; }     // Tìm theo MSNV hoặc tên
    public int? TramId { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public string? TechnicalGrade { get; set; }
}
```

### 5.3 MRes_Employee
```csharp
public class MRes_Employee
{
    public int Id { get; set; }
    public string Msnv { get; set; }
    public string FullName { get; set; }
    public string? FullNameLocal { get; set; }
    
    public int TramId { get; set; }
    public string? TramCode { get; set; }       // From navigation
    public string? TramName { get; set; }       // From navigation
    
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; } // From navigation
    
    public int? PositionId { get; set; }
    public string? PositionName { get; set; }   // From navigation
    
    public string? TechnicalGrade { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public bool InsuranceIncluded { get; set; }
    
    public short Status { get; set; }
}
```

---

## 6. Business Rules

### 6.1 Validation
```csharp
// 1. MSNV phải unique (loại trừ soft-deleted)
if (await _context.Employees.AnyAsync(x => x.Msnv == request.Msnv && x.Status != -1))
    return Error(HttpStatusCode.Conflict, "Mã số nhân viên đã tồn tại!");

// 2. Trạm phải tồn tại
if (!await _context.Trams.AnyAsync(x => x.Id == request.TramId && x.Status != -1))
    return Error(HttpStatusCode.BadRequest, "Trạm không tồn tại!");

// 3. MSNV auto uppercase + trim
request.Msnv = request.Msnv?.Trim().ToUpper();
```

### 6.2 Delete Constraint
```csharp
// Không thể xóa nhân viên đã có dữ liệu lương
var hasPayroll = await _context.Payrolls.AnyAsync(x => x.EmployeeId == id);
if (hasPayroll)
    return Error(HttpStatusCode.BadRequest, "Không thể xoá nhân viên đã có dữ liệu lương!");
```

### 6.3 Status Values
- `-1` = Đã xóa (soft delete)
- `0` = Không hoạt động
- `1` = Đang hoạt động

---

## 7. Filter Query Logic

```csharp
private IQueryable<Employee> BuildFilterQuery(MReq_Employee_FullParam request)
{
    var query = _context.Employees.AsNoTracking()
        .Include(x => x.Tram)
        .Include(x => x.Department)
        .Include(x => x.Position)
        .AsQueryable();

    // Filter by status
    if (request.SequenceStatus has values)
        query = query.Where(x => status.Contains(x.Status));

    // Search by MSNV or FullName
    if (!string.IsNullOrWhiteSpace(request.SearchText))
        query = query.Where(x => x.Msnv.Contains(searchText) || x.FullName.Contains(searchText));

    // Filter by Tram
    if (request.TramId.HasValue)
        query = query.Where(x => x.TramId == request.TramId.Value);

    // Filter by Department
    if (request.DepartmentId.HasValue)
        query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

    // Filter by Position
    if (request.PositionId.HasValue)
        query = query.Where(x => x.PositionId == request.PositionId.Value);

    // Filter by Grade
    if (!string.IsNullOrWhiteSpace(request.TechnicalGrade))
        query = query.Where(x => x.TechnicalGrade == request.TechnicalGrade);

    return query;
}
```

---

## 8. AutoMapper Configuration

```csharp
// MReq → Entity (ignore audit fields)
CreateMap<MReq_Employee, Employee>()
    .ForMember(d => d.Status, opt => opt.Ignore())
    .ForMember(d => d.CreatedAt, opt => opt.Ignore())
    .ForMember(d => d.CreatedBy, opt => opt.Ignore())
    .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
    .ForMember(d => d.UpdatedBy, opt => opt.Ignore());

// Entity → MRes (include navigation data)
CreateMap<Employee, MRes_Employee>()
    .ForMember(d => d.TramCode, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Code : null))
    .ForMember(d => d.TramName, opt => opt.MapFrom(s => s.Tram != null ? s.Tram.Name : null))
    .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null))
    .ForMember(d => d.PositionName, opt => opt.MapFrom(s => s.Position != null ? s.Position.Name : null));
```

---

## 9. Database Constraints

```csharp
// Unique index on Msnv
modelBuilder.Entity<Employee>()
    .HasIndex(e => e.Msnv)
    .IsUnique();

// FK to Tram (Required, Restrict delete)
modelBuilder.Entity<Employee>()
    .HasOne(e => e.Tram)
    .WithMany(t => t.Employees)
    .HasForeignKey(e => e.TramId)
    .OnDelete(DeleteBehavior.Restrict);

// FK to Department (Optional, SetNull on delete)
modelBuilder.Entity<Employee>()
    .HasOne(e => e.Department)
    .WithMany(d => d.Employees)
    .HasForeignKey(e => e.DepartmentId)
    .OnDelete(DeleteBehavior.SetNull);

// FK to Position (Optional, SetNull on delete)
modelBuilder.Entity<Employee>()
    .HasOne(e => e.Position)
    .WithMany(p => p.Employees)
    .HasForeignKey(e => e.PositionId)
    .OnDelete(DeleteBehavior.SetNull);
```

---

## 10. Code Examples

### Create Employee
```javascript
POST /Employee/Create
{
    "msnv": "NV001",
    "fullName": "Nguyễn Văn A",
    "fullNameLocal": "Nguyen Van A",
    "tramId": 1,
    "departmentId": 2,
    "positionId": 3,
    "technicalGrade": "B",
    "hireDate": "2026-01-15",
    "birthDate": "1990-05-20",
    "gender": "Nam",
    "insuranceIncluded": true
}
```

### Get List with Filters
```javascript
GET /Employee/GetListByPaging?SearchText=nguyen&TramId=1&TechnicalGrade=B&Page=1&Record=20
```

### Update Status List (Soft Delete)
```javascript
PUT /Employee/UpdateStatusList
{
    "sequenceIds": "[1, 2, 3]",
    "status": -1,
    "updatedBy": 1
}
```

---

## 11. Related Modules

| Module | Relationship | Ghi chú |
|--------|--------------|---------|
| Tram | N:1 (Required) | Mỗi NV thuộc 1 Trạm |
| Department | N:1 (Optional) | Mỗi NV thuộc 1 Phòng ban |
| Position | N:1 (Optional) | Mỗi NV có 1 Chức vụ |
| Attendance | 1:N | Chấm công theo tháng |
| Performance | 1:N | Đánh giá theo tháng |
| Allowance | 1:N | Phụ cấp theo tháng |
| Payroll | 1:N | Bảng lương theo tháng |
