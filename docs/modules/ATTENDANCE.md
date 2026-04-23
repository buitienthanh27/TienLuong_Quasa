# Module: ATTENDANCE (Chấm Công)

## 1. Tổng Quan

**Mục đích**: Quản lý dữ liệu chấm công hàng tháng của nhân viên.

**Entity**: `Attendance`

**Dependencies**:
- `Employee` - Nhân viên được chấm công
- Input cho `Payroll` - Tính lương cần dữ liệu ngày công

---

## 2. Entity Structure

```csharp
[Table("attendance")]
public partial class Attendance : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("employee_id")]
    public int EmployeeId { get; set; }              // FK → Employee

    [Required]
    [Column("year_month")]
    [StringLength(7)]
    public string YearMonth { get; set; }            // Format: "YYYY-MM"

    [Column("working_days", TypeName = "decimal(5,2)")]
    public decimal WorkingDays { get; set; }         // Số ngày công thực tế

    [Column("sunday_days", TypeName = "decimal(5,2)")]
    public decimal SundayDays { get; set; }          // Số ngày CN làm việc

    [Column("absent_days", TypeName = "decimal(5,2)")]
    public decimal AbsentDays { get; set; }          // Số ngày nghỉ không lương

    [Column("sick_days", TypeName = "decimal(5,2)")]
    public decimal SickDays { get; set; }            // Số ngày nghỉ ốm

    [Column("leave_days", TypeName = "decimal(5,2)")]
    public decimal LeaveDays { get; set; }           // Số ngày phép

    [Column("attendance_status")]
    [StringLength(20)]
    public string AttendanceStatus { get; set; } = "DRAFT";  // DRAFT | CONFIRMED

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
```

---

## 3. Service Interface

```csharp
public interface IS_Attendance
{
    Task<ResponseData<MRes_Attendance>> Create(MReq_Attendance request);
    Task<ResponseData<MRes_Attendance>> Update(MReq_Attendance request);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_Attendance>> GetById(int id);
    Task<ResponseData<List<MRes_Attendance>>> GetListByPaging(MReq_Attendance_FullParam request);
    Task<ResponseData<List<MRes_Attendance>>> GetListByFullParam(MReq_Attendance_FullParam request);
    Task<ResponseData<int>> BulkImport(MReq_Attendance_BulkImport request);
}
```

---

## 4. API Endpoints

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/Attendance/Create` | Thêm chấm công |
| PUT | `/Attendance/Update` | Cập nhật chấm công |
| DELETE | `/Attendance/Delete/{id}` | Xóa chấm công |
| GET | `/Attendance/GetById/{id}` | Xem chi tiết |
| GET | `/Attendance/GetListByPaging` | Danh sách phân trang |
| GET | `/Attendance/GetListByFullParam` | Danh sách đầy đủ |
| POST | `/Attendance/BulkImport` | Import hàng loạt |

---

## 5. Request/Response Models

### 5.1 MReq_Attendance
```csharp
public class MReq_Attendance : BaseModel.History
{
    public int Id { get; set; }
    
    [Required]
    public int EmployeeId { get; set; }
    
    [Required]
    [StringLength(7)]
    public string YearMonth { get; set; }      // "2026-04"
    
    public decimal WorkingDays { get; set; }
    public decimal SundayDays { get; set; }
    public decimal AbsentDays { get; set; }
    public decimal SickDays { get; set; }
    public decimal LeaveDays { get; set; }
    
    [StringLength(20)]
    public string? AttendanceStatus { get; set; }
}
```

### 5.2 MReq_Attendance_FullParam
```csharp
public class MReq_Attendance_FullParam : PagingRequestBase
{
    public int? EmployeeId { get; set; }
    public string? YearMonth { get; set; }
    public string? AttendanceStatus { get; set; }
}
```

### 5.3 MReq_Attendance_BulkImport
```csharp
public class MReq_Attendance_BulkImport
{
    [Required]
    public string YearMonth { get; set; }
    
    public int CreatedBy { get; set; }
    
    public List<MReq_Attendance_BulkItem> Items { get; set; }
}

public class MReq_Attendance_BulkItem
{
    public int EmployeeId { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal SundayDays { get; set; }
    public decimal AbsentDays { get; set; }
    public decimal SickDays { get; set; }
    public decimal LeaveDays { get; set; }
}
```

### 5.4 MRes_Attendance
```csharp
public class MRes_Attendance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? Msnv { get; set; }           // From Employee
    public string? EmployeeName { get; set; }   // From Employee
    public int? TramId { get; set; }            // From Employee.Tram
    public string? TramName { get; set; }       // From Employee.Tram
    
    public string YearMonth { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal SundayDays { get; set; }
    public decimal AbsentDays { get; set; }
    public decimal SickDays { get; set; }
    public decimal LeaveDays { get; set; }
    public string AttendanceStatus { get; set; }
    public short Status { get; set; }
}
```

---

## 6. Business Rules

### 6.1 Unique Constraint
```csharp
// Mỗi nhân viên chỉ có 1 record chấm công mỗi tháng
var isExists = await _context.Attendances.AnyAsync(x =>
    x.EmployeeId == request.EmployeeId &&
    x.YearMonth == request.YearMonth &&
    x.Status != -1);
if (isExists)
    return Error(HttpStatusCode.Conflict, "Chấm công tháng này đã tồn tại cho nhân viên!");
```

### 6.2 Bulk Import Logic
```csharp
// Import hàng loạt: update nếu đã tồn tại, insert nếu chưa
foreach (var item in request.Items)
{
    var existing = await _context.Attendances.FirstOrDefaultAsync(x =>
        x.EmployeeId == item.EmployeeId &&
        x.YearMonth == request.YearMonth &&
        x.Status != -1);

    if (existing != null)
    {
        // Update existing
        existing.WorkingDays = item.WorkingDays;
        existing.SundayDays = item.SundayDays;
        // ... update other fields
        updatedCount++;
    }
    else
    {
        // Insert new
        _context.Attendances.Add(new Attendance { ... });
        insertedCount++;
    }
}
```

### 6.3 Status Values
- `DRAFT` - Mới nhập, có thể sửa
- `CONFIRMED` - Đã xác nhận, sẵn sàng tính lương

---

## 6.4 Ký Hiệu Chấm Công (Quy Định Quasa)

| Ký hiệu | Tên gọi | Mô tả |
|---------|---------|-------|
| **X** | Công thường | Ngày làm việc bình thường |
| **CN** | Chủ nhật | Làm việc ngày chủ nhật (hệ số cao hơn) |
| **L** | Nghỉ lễ | Nghỉ ngày lễ có lương |
| **P** | Nghỉ phép | Nghỉ phép năm có lương |
| **Ô** | Nghỉ ốm | Nghỉ ốm có BHXH (nếu áp dụng) |
| **CT** | Công tác | Đi công tác (tính công như bình thường + phụ cấp) |
| **NB** | Nghỉ bù | Nghỉ bù ngày lễ/CN đã làm việc |
| **KP** | Không phép | Nghỉ không phép (không lương) |
| **TS** | Thai sản | Nghỉ thai sản (chế độ BHXH) |
| **HĐ** | Học/Đào tạo | Đi học/đào tạo do công ty cử |
| **/X** | Nửa công | Làm nửa ngày (0.5 công) |

### Quy Định Chấm Công Quasa

1. **Ngày công chuẩn**: P7 = 27 ngày/tháng (có thể cấu hình trong SystemParameter)
2. **Công chủ nhật**: Hệ số 76.9 (cao hơn công thường 46.1)
3. **Nghỉ lễ làm việc**: Trả tiền mặt trực tiếp (ghi tạm ứng), cuối tháng trừ lại trong bảng lương
4. **Công tác (CT)**: Tính như công thường + phụ cấp công tác nếu có
5. **Chấm công hằng ngày**: Phải ghi nhận đầy đủ trước khi chốt kỳ lương

---

## 7. Database Constraints

```csharp
// Unique: (EmployeeId, YearMonth)
modelBuilder.Entity<Attendance>()
    .HasIndex(a => new { a.EmployeeId, a.YearMonth })
    .IsUnique();

// FK to Employee (Required, Restrict delete)
modelBuilder.Entity<Attendance>()
    .HasOne(a => a.Employee)
    .WithMany(e => e.Attendances)
    .HasForeignKey(a => a.EmployeeId)
    .OnDelete(DeleteBehavior.Restrict);
```

---

## 8. Filter Query Logic

```csharp
private IQueryable<Attendance> BuildFilterQuery(MReq_Attendance_FullParam request)
{
    var query = _context.Attendances.AsNoTracking()
        .Include(x => x.Employee).ThenInclude(e => e.Tram)
        .AsQueryable();

    if (status.Length > 0)
        query = query.Where(x => status.Contains(x.Status));

    if (request.EmployeeId.HasValue)
        query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);

    if (!string.IsNullOrWhiteSpace(request.YearMonth))
        query = query.Where(x => x.YearMonth == request.YearMonth);

    if (!string.IsNullOrWhiteSpace(request.AttendanceStatus))
        query = query.Where(x => x.AttendanceStatus == request.AttendanceStatus);

    return query;
}
```

---

## 9. Code Examples

### Create Attendance
```javascript
POST /Attendance/Create
{
    "employeeId": 1,
    "yearMonth": "2026-04",
    "workingDays": 22,
    "sundayDays": 2,
    "absentDays": 1,
    "sickDays": 0,
    "leaveDays": 1,
    "attendanceStatus": "DRAFT"
}
```

### Bulk Import
```javascript
POST /Attendance/BulkImport
{
    "yearMonth": "2026-04",
    "createdBy": 1,
    "items": [
        { "employeeId": 1, "workingDays": 22, "sundayDays": 2, "absentDays": 1, "sickDays": 0, "leaveDays": 1 },
        { "employeeId": 2, "workingDays": 25, "sundayDays": 4, "absentDays": 0, "sickDays": 1, "leaveDays": 0 },
        { "employeeId": 3, "workingDays": 24, "sundayDays": 3, "absentDays": 0, "sickDays": 0, "leaveDays": 2 }
    ]
}
```

### Response
```json
{
    "result": 1,
    "data": 3,
    "error": {
        "code": 200,
        "message": "Import thành công: 2 mới, 1 cập nhật"
    }
}
```

---

## 10. Liên Kết Với Payroll

```csharp
// Payroll service lấy Attendance để tính lương
var attendance = await _context.Attendances.AsNoTracking()
    .FirstOrDefaultAsync(a => 
        a.EmployeeId == emp.Id && 
        a.YearMonth == yearMonth && 
        a.Status != -1);

if (attendance == null)
{
    summary.Errors.Add($"NV {emp.Msnv}: Chưa có chấm công tháng {yearMonth}");
    continue;
}

// Sử dụng attendance.WorkingDays trong công thức
var calculatedSalary = (baseRate + attendance.WorkingDays) / p7 * perfCoef;
```
