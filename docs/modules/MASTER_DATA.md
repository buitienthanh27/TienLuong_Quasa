# Module: MASTER DATA (Danh Mục)

## Tổng Quan

Các module danh mục dùng chung trong hệ thống. Tất cả đều tuân theo pattern CRUD chuẩn.

---

# 1. TRAM (Trạm/Chi Nhánh)

## Entity Structure
```csharp
[Table("tram")]
public partial class Tram : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(10)]
    public string Code { get; set; }           // Unique, UPPERCASE

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    // Collections
    public virtual ICollection<Employee> Employees { get; set; }
    public virtual ICollection<SalaryScale> SalaryScales { get; set; }
}
```

## Service Interface
```csharp
public interface IS_Tram
{
    Task<ResponseData<MRes_Tram>> Create(MReq_Tram request);
    Task<ResponseData<MRes_Tram>> Update(MReq_Tram request);
    Task<ResponseData<MRes_Tram>> UpdateStatus(int id, short status, int updatedBy);
    Task<ResponseData<List<MRes_Tram>>> UpdateStatusList(string sequenceIds, short status, int updatedBy);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_Tram>> GetById(int id);
    Task<ResponseData<List<MRes_Tram>>> GetListByPaging(MReq_Tram_FullParam request);
    Task<ResponseData<List<MRes_Tram>>> GetListByFullParam(MReq_Tram_FullParam request);
}
```

## Business Rules
```csharp
// Code unique (uppercase, trim)
request.Code = request.Code?.Trim().ToUpper();
if (await _context.Trams.AnyAsync(x => x.Code == request.Code && x.Status != -1))
    return Error(HttpStatusCode.Conflict, "Mã trạm đã tồn tại!");

// Cannot delete if has employees
var hasEmployees = await _context.Employees.AnyAsync(x => x.TramId == id && x.Status != -1);
if (hasEmployees)
    return Error(HttpStatusCode.BadRequest, "Không thể xoá trạm đang có nhân viên!");
```

## API Endpoints
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/Tram/Create` | Thêm trạm |
| PUT | `/Tram/Update` | Cập nhật |
| PUT | `/Tram/UpdateStatus` | Cập nhật trạng thái |
| PUT | `/Tram/UpdateStatusList` | Cập nhật nhiều |
| DELETE | `/Tram/Delete/{id}` | Xóa |
| GET | `/Tram/GetById/{id}` | Chi tiết |
| GET | `/Tram/GetListByPaging` | Danh sách phân trang |
| GET | `/Tram/GetListByFullParam` | Danh sách full |

---

# 2. DEPARTMENT (Phòng Ban)

## Entity Structure
```csharp
[Table("department")]
public partial class Department : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; }           // Unique

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("parent_id")]
    public int? ParentId { get; set; }         // Self-reference for hierarchy

    // Self-referencing
    [ForeignKey("ParentId")]
    public virtual Department? Parent { get; set; }
    public virtual ICollection<Department> Children { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}
```

## Database Constraint
```csharp
// Unique Code
modelBuilder.Entity<Department>()
    .HasIndex(d => d.Code)
    .IsUnique();

// Self-referencing relationship
modelBuilder.Entity<Department>()
    .HasOne(d => d.Parent)
    .WithMany(d => d.Children)
    .HasForeignKey(d => d.ParentId)
    .OnDelete(DeleteBehavior.SetNull);
```

---

# 3. POSITION (Chức Vụ)

## Entity Structure
```csharp
[Table("position")]
public partial class Position : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(20)]
    public string Code { get; set; }           // Unique

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("type")]
    [StringLength(20)]
    public string? Type { get; set; }          // Quản lý, Nhân viên, Công nhân

    public virtual ICollection<Employee> Employees { get; set; }
}
```

---

# 4. SALARY_SCALE (Thang Lương)

## Entity Structure
```csharp
[Table("salary_scale")]
public partial class SalaryScale : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("tram_id")]
    public int TramId { get; set; }            // FK → Tram

    [Required]
    [Column("grade")]
    [StringLength(5)]
    public string Grade { get; set; }          // A, B, C, D, E

    [Column("coefficient", TypeName = "decimal(5,2)")]
    public decimal Coefficient { get; set; }   // Hệ số lương

    [Column("base_rate", TypeName = "decimal(18,4)")]
    public decimal BaseRate { get; set; }      // Mức lương cơ bản (VND)

    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; } // Ngày hiệu lực

    [ForeignKey("TramId")]
    public virtual Tram Tram { get; set; }
}
```

## Service Interface
```csharp
public interface IS_SalaryScale
{
    Task<ResponseData<MRes_SalaryScale>> Create(MReq_SalaryScale request);
    Task<ResponseData<MRes_SalaryScale>> Update(MReq_SalaryScale request);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_SalaryScale>> GetById(int id);
    Task<ResponseData<List<MRes_SalaryScale>>> GetListByPaging(MReq_SalaryScale_FullParam request);
    Task<ResponseData<List<MRes_SalaryScale>>> GetListByFullParam(MReq_SalaryScale_FullParam request);
}
```

## Business Rules
```csharp
// Unique: (TramId, Grade, EffectiveDate)
var isExists = await _context.SalaryScales.AnyAsync(x =>
    x.TramId == request.TramId &&
    x.Grade == request.Grade &&
    x.EffectiveDate == request.EffectiveDate &&
    x.Status != -1);
if (isExists)
    return Error(HttpStatusCode.Conflict, "Bảng lương này đã tồn tại!");
```

## Database Constraint
```csharp
modelBuilder.Entity<SalaryScale>()
    .HasIndex(s => new { s.TramId, s.Grade, s.EffectiveDate })
    .IsUnique();
```

## Cách Sử Dụng Trong Payroll
```csharp
// Lấy SalaryScale áp dụng tại thời điểm tính lương
var scale = await _context.SalaryScales.AsNoTracking()
    .Where(s => s.TramId == emp.TramId 
             && s.Grade == grade 
             && s.EffectiveDate <= monthEnd 
             && s.Status != -1)
    .OrderByDescending(s => s.EffectiveDate)  // Lấy mới nhất
    .FirstOrDefaultAsync();

var baseRate = scale?.BaseRate ?? 0;
```

---

# 5. SYSTEM_PARAMETER (Tham Số Hệ Thống)

## Entity Structure
```csharp
[Table("system_parameter")]
public partial class SystemParameter : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("param_code")]
    [StringLength(50)]
    public string ParamCode { get; set; }      // Mã tham số (unique với EffectiveDate)

    [Required]
    [Column("param_name")]
    [StringLength(200)]
    public string ParamName { get; set; }      // Tên mô tả

    [Column("param_value", TypeName = "decimal(18,4)")]
    public decimal ParamValue { get; set; }    // Giá trị

    [Column("data_type")]
    [StringLength(20)]
    public string? DataType { get; set; }      // DECIMAL, PERCENT, INTEGER

    [Column("effective_date", TypeName = "date")]
    public DateTime EffectiveDate { get; set; } // Ngày hiệu lực

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }
}
```

## Service Interface
```csharp
public interface IS_SystemParameter
{
    Task<ResponseData<MRes_SystemParameter>> Create(MReq_SystemParameter request);
    Task<ResponseData<MRes_SystemParameter>> Update(MReq_SystemParameter request);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_SystemParameter>> GetById(int id);
    Task<ResponseData<List<MRes_SystemParameter>>> GetListByPaging(MReq_SystemParameter_FullParam request);
    Task<ResponseData<List<MRes_SystemParameter>>> GetListByFullParam(MReq_SystemParameter_FullParam request);
    
    // Helper method for Payroll
    Task<decimal> GetParamValue(string paramCode, DateTime effectiveDate);
}
```

## Helper Method
```csharp
public async Task<decimal> GetParamValue(string paramCode, DateTime effectiveDate)
{
    var param = await _context.SystemParameters.AsNoTracking()
        .Where(x => x.ParamCode == paramCode 
                 && x.EffectiveDate <= effectiveDate 
                 && x.Status != -1)
        .OrderByDescending(x => x.EffectiveDate)  // Lấy mới nhất
        .FirstOrDefaultAsync();

    return param?.ParamValue ?? 0;
}
```

## Danh Sách Tham Số Quan Trọng

| ParamCode | Mô tả | Default | DataType |
|-----------|-------|---------|----------|
| P7 | Số ngày công chuẩn/tháng | 27 | INTEGER |
| BHXH_RATE | Tỷ lệ BHXH nhân viên | 0.08 | PERCENT |
| BHYT_RATE | Tỷ lệ BHYT nhân viên | 0.015 | PERCENT |
| BHTN_RATE | Tỷ lệ BHTN nhân viên | 0.01 | PERCENT |
| TAX_TH_1 | Ngưỡng thuế bậc 1 | 5000000 | DECIMAL |
| TAX_RATE_1 | Thuế suất bậc 1 | 5 | PERCENT |
| TAX_TH_2 | Ngưỡng thuế bậc 2 | 10000000 | DECIMAL |
| TAX_RATE_2 | Thuế suất bậc 2 | 10 | PERCENT |
| ... | ... | ... | ... |

## Database Constraint
```csharp
// Unique: (ParamCode, EffectiveDate)
modelBuilder.Entity<SystemParameter>()
    .HasIndex(s => new { s.ParamCode, s.EffectiveDate })
    .IsUnique();
```

---

# 6. COST_CENTER (Trung Tâm Chi Phí)

## Entity Structure
```csharp
[Table("cost_center")]
public partial class CostCenter : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(30)]
    public string Code { get; set; }           // Unique

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("allocation_rate", TypeName = "decimal(5,4)")]
    public decimal? AllocationRate { get; set; } // Tỷ lệ phân bổ (0.6 = 60%)

    [Column("accounting_code")]
    [StringLength(30)]
    public string? AccountingCode { get; set; }  // Mã tài khoản kế toán (621, 622, 642...)

    public virtual ICollection<CostAllocation> CostAllocations { get; set; }
}
```

## Service Interface
```csharp
public interface IS_CostCenter
{
    Task<ResponseData<MRes_CostCenter>> Create(MReq_CostCenter request);
    Task<ResponseData<MRes_CostCenter>> Update(MReq_CostCenter request);
    Task<ResponseData<MRes_CostCenter>> UpdateStatus(int id, short status, int updatedBy);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_CostCenter>> GetById(int id);
    Task<ResponseData<List<MRes_CostCenter>>> GetListByPaging(MReq_CostCenter_FullParam request);
    Task<ResponseData<List<MRes_CostCenter>>> GetListByFullParam(MReq_CostCenter_FullParam request);
}
```

## Business Rules
```csharp
// Code unique
request.Code = request.Code?.Trim().ToUpper();
if (await _context.CostCenters.AnyAsync(x => x.Code == request.Code && x.Status != -1))
    return Error(HttpStatusCode.Conflict, "Mã trung tâm chi phí đã tồn tại!");

// Cannot delete if has allocations
var hasAllocations = await _context.CostAllocations.AnyAsync(x => x.CostCenterId == id);
if (hasAllocations)
    return Error(HttpStatusCode.BadRequest, "Không thể xoá trung tâm chi phí đang có phân bổ!");
```

## Ví Dụ Dữ Liệu
```
| Code   | Name                | AccountingCode | AllocationRate |
|--------|---------------------|----------------|----------------|
| CC_SX  | Chi phí sản xuất    | 621            | 0.60 (60%)     |
| CC_QL  | Chi phí quản lý     | 642            | 0.25 (25%)     |
| CC_BH  | Chi phí bán hàng    | 641            | 0.15 (15%)     |
```

---

# 7. ALLOWANCE_TYPE (Loại Phụ Cấp)

## Entity Structure
```csharp
[Table("allowance_type")]
public partial class AllowanceType : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [StringLength(30)]
    public string Code { get; set; }           // Unique

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Column("default_rate", TypeName = "decimal(18,4)")]
    public decimal? DefaultRate { get; set; }  // Mức mặc định

    public virtual ICollection<Allowance> Allowances { get; set; }
}
```

## Ví Dụ Dữ Liệu
```
| Code   | Name                    | DefaultRate |
|--------|-------------------------|-------------|
| PC_AN  | Phụ cấp ăn trưa         | 30000       | (30k/ngày)
| PC_DL  | Phụ cấp đi lại          | 500000      | (500k/tháng)
| PC_DD  | Phụ cấp độc hại         | 200000      | (200k/tháng)
| PC_TN  | Phụ cấp trách nhiệm     | 1000000     | (1tr/tháng)
| PC_CN  | Phụ cấp làm chủ nhật    | 50000       | (50k/ngày)
```

---

## Tổng Kết Pattern CRUD Chuẩn

Tất cả Master Data modules đều follow pattern:

```csharp
// CREATE
1. Trim/ToUpper code field
2. Check unique constraint
3. Map DTO → Entity
4. Set CreatedAt, CreatedBy
5. Add to context
6. SaveChangesAsync
7. Return mapped response

// UPDATE
1. Trim/ToUpper code field
2. Check unique (exclude current id)
3. Find existing entity
4. Map request to entity
5. Set UpdatedAt, UpdatedBy
6. SaveChangesAsync
7. Return mapped response

// DELETE
1. Check dependencies (has children?)
2. If has → return Error
3. ExecuteDeleteAsync
4. Return deleted count

// GET LIST
1. BuildFilterQuery with status, search text, etc.
2. Apply ordering
3. If paging: CountAsync → Skip/Take
4. ProjectTo<MRes_*>
5. ToListAsync
```
