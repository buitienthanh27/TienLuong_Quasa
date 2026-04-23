# Module: ADVANCE_PAYMENT (Tạm Ứng)

## 1. Tổng Quan

**Mục đích**: Quản lý tạm ứng lương cho công nhân, đặc biệt trong dịp lễ hoặc khi CN có nhu cầu.

**Entity**: `Advance`

**Dependencies**:
- `Employee` - Công nhân tạm ứng
- `Payroll` - Trừ tạm ứng khi tính lương cuối tháng

---

## 2. Quy Định Tạm Ứng (Quasa)

### 2.1 Nguyên Tắc Chung

| Tiêu chí | Quy định |
|----------|----------|
| **DRC Tạm ứng** | 40% (fixed) - áp dụng khi tính tiền tạm ứng từ sản lượng |
| **Thời điểm** | Giữa tháng hoặc trước ngày lễ |
| **Giới hạn** | Không quá 50% lương dự kiến tháng |
| **Trừ lại** | Cuối tháng, trừ vào bảng lương chính |
| **Phê duyệt** | Cần Đội trưởng duyệt trước khi chi |

### 2.2 Loại Tạm Ứng

| Code | Loại | Mô tả |
|------|------|-------|
| `ADV_HOLIDAY` | Tạm ứng lễ | Tạm ứng dịp lễ Tết |
| `ADV_SPECIAL` | Tạm ứng đặc biệt | Trường hợp đột xuất |
| `ADV_MID_MONTH` | Tạm ứng giữa tháng | Tạm ứng định kỳ giữa tháng |
| `ADV_EMERGENCY` | Tạm ứng khẩn cấp | Trường hợp cấp bách |

### 2.3 Workflow

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│ CN yêu cầu  │────▶│ Đội trưởng  │────▶│ Chi tiền    │────▶│ Trừ vào     │
│ tạm ứng     │     │ phê duyệt   │     │ mặt         │     │ bảng lương  │
└─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘
    PENDING           APPROVED           DISBURSED           DEDUCTED
```

---

## 3. Entity Structure

```csharp
[Table("advance")]
public partial class Advance : BaseEntity
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

    [Column("advance_date", TypeName = "datetime")]
    public DateTime AdvanceDate { get; set; }        // Ngày tạm ứng

    [Column("advance_type")]
    [StringLength(20)]
    public string AdvanceType { get; set; }          // ADV_HOLIDAY, ADV_SPECIAL...

    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }              // Số tiền tạm ứng

    [Column("drc_applied", TypeName = "decimal(5,4)")]
    public decimal DrcApplied { get; set; } = 0.40m; // DRC = 40% (fixed)

    [Column("reason")]
    [StringLength(500)]
    public string? Reason { get; set; }              // Lý do tạm ứng

    // Approval
    [Column("advance_status")]
    [StringLength(20)]
    public string AdvanceStatus { get; set; } = "PENDING";  // PENDING | APPROVED | DISBURSED | DEDUCTED | REJECTED

    [Column("approved_by")]
    public int? ApprovedBy { get; set; }

    [Column("approved_at", TypeName = "datetime")]
    public DateTime? ApprovedAt { get; set; }

    // Disbursement (chi tiền)
    [Column("disbursed_at", TypeName = "datetime")]
    public DateTime? DisbursedAt { get; set; }

    [Column("disbursed_by")]
    public int? DisbursedBy { get; set; }

    // Deduction (trừ vào lương)
    [Column("is_deducted")]
    public bool IsDeducted { get; set; } = false;

    [Column("deducted_in_payroll_id")]
    public int? DeductedInPayrollId { get; set; }    // FK → Payroll

    [Column("deducted_at", TypeName = "datetime")]
    public DateTime? DeductedAt { get; set; }

    // Relationships
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }

    [ForeignKey("DeductedInPayrollId")]
    public virtual Payroll? DeductedInPayroll { get; set; }
}
```

---

## 4. Service Interface

```csharp
public interface IS_Advance
{
    // CRUD
    Task<ResponseData<MRes_Advance>> Create(MReq_Advance request);
    Task<ResponseData<MRes_Advance>> Update(MReq_Advance request);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_Advance>> GetById(int id);
    Task<ResponseData<List<MRes_Advance>>> GetListByPaging(MReq_Advance_FullParam request);
    
    // Workflow
    Task<ResponseData<MRes_Advance>> Approve(int id, int approvedBy);
    Task<ResponseData<MRes_Advance>> Reject(int id, int rejectedBy, string reason);
    Task<ResponseData<MRes_Advance>> MarkDisbursed(int id, int disbursedBy);
    
    // For Payroll
    Task<ResponseData<List<MRes_Advance>>> GetPendingByEmployee(int employeeId, string yearMonth);
    Task<ResponseData<decimal>> GetTotalPendingAmount(int employeeId, string yearMonth);
    Task<ResponseData<int>> MarkDeducted(List<int> advanceIds, int payrollId);
}
```

---

## 5. API Endpoints

| Method | Endpoint | Mô tả | Quyền |
|--------|----------|-------|-------|
| POST | `/Advance/Create` | Tạo yêu cầu tạm ứng | CN, Kế toán |
| PUT | `/Advance/Update` | Cập nhật tạm ứng | Kế toán |
| DELETE | `/Advance/Delete/{id}` | Xóa tạm ứng | Kế toán |
| GET | `/Advance/GetById/{id}` | Xem chi tiết | Tất cả |
| GET | `/Advance/GetListByPaging` | Danh sách phân trang | Tất cả |
| PUT | `/Advance/Approve/{id}` | Phê duyệt | Đội trưởng |
| PUT | `/Advance/Reject/{id}` | Từ chối | Đội trưởng |
| PUT | `/Advance/MarkDisbursed/{id}` | Đánh dấu đã chi | Kế toán |
| GET | `/Advance/GetPendingByEmployee` | Tạm ứng chờ trừ | Kế toán |
| GET | `/Advance/GetTotalPendingAmount` | Tổng tiền chờ trừ | Kế toán |

---

## 6. Request/Response Models

### 6.1 MReq_Advance
```csharp
public class MReq_Advance : BaseModel.History
{
    public int Id { get; set; }
    
    [Required]
    public int EmployeeId { get; set; }
    
    [Required]
    public string YearMonth { get; set; }
    
    [Required]
    public DateTime AdvanceDate { get; set; }
    
    [Required]
    public string AdvanceType { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    public decimal DrcApplied { get; set; } = 0.40m;
    
    public string? Reason { get; set; }
}
```

### 6.2 MReq_Advance_FullParam
```csharp
public class MReq_Advance_FullParam : PagingRequestBase
{
    public int? EmployeeId { get; set; }
    public string? YearMonth { get; set; }
    public string? AdvanceType { get; set; }
    public string? AdvanceStatus { get; set; }
    public bool? IsDeducted { get; set; }
}
```

### 6.3 MRes_Advance
```csharp
public class MRes_Advance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? Msnv { get; set; }
    public string? EmployeeName { get; set; }
    public int? TramId { get; set; }
    public string? TramName { get; set; }
    
    public string YearMonth { get; set; }
    public DateTime AdvanceDate { get; set; }
    public string AdvanceType { get; set; }
    public string AdvanceTypeName { get; set; }  // Display name
    public decimal Amount { get; set; }
    public decimal DrcApplied { get; set; }
    public string? Reason { get; set; }
    
    public string AdvanceStatus { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DisbursedAt { get; set; }
    
    public bool IsDeducted { get; set; }
    public int? DeductedInPayrollId { get; set; }
    public DateTime? DeductedAt { get; set; }
    
    public short Status { get; set; }
}
```

---

## 7. Business Logic

### 7.1 Tính Tiền Tạm Ứng (với DRC 40%)

```csharp
// Khi CN muốn tạm ứng dựa trên sản lượng mủ
public decimal CalculateAdvanceFromProduction(decimal freshLatexKg, decimal unitPrice)
{
    const decimal DRC_ADVANCE = 0.40m;  // DRC 40% cho tạm ứng
    
    // Quy khô với DRC tạm ứng
    var dryLatex = freshLatexKg * DRC_ADVANCE;
    
    // Tiền sản lượng tạm ứng
    var amount = Math.Round(dryLatex * unitPrice, 0);
    
    return amount;
}
```

### 7.2 Approve Advance
```csharp
public async Task<ResponseData<MRes_Advance>> Approve(int id, int approvedBy)
{
    try
    {
        var adv = await _context.Advances.FindAsync(id);
        if (adv == null || adv.Status == -1)
            return Error(HttpStatusCode.NotFound, "Không tìm thấy yêu cầu tạm ứng!");

        if (adv.AdvanceStatus != "PENDING")
            return Error(HttpStatusCode.BadRequest, "Chỉ có thể duyệt yêu cầu đang chờ!");

        adv.AdvanceStatus = "APPROVED";
        adv.ApprovedBy = approvedBy;
        adv.ApprovedAt = DateTime.UtcNow;
        adv.UpdatedAt = DateTime.UtcNow;
        adv.UpdatedBy = approvedBy;

        await _context.SaveChangesAsync();
        return new ResponseData<MRes_Advance>(1, 200, "Phê duyệt tạm ứng thành công");
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(Approve), new { id, approvedBy });
    }
}
```

### 7.3 Tích Hợp Với Payroll
```csharp
// Trong S_Payroll.Calculate() - Phase 5
public async Task<decimal> GetTotalPendingAdvances(int employeeId, string yearMonth)
{
    return await _context.Advances
        .Where(a => a.EmployeeId == employeeId &&
                    a.YearMonth == yearMonth &&
                    a.AdvanceStatus == "DISBURSED" &&
                    !a.IsDeducted &&
                    a.Status != -1)
        .SumAsync(a => a.Amount);
}

// Sau khi tính lương xong
public async Task MarkAdvancesDeducted(int employeeId, string yearMonth, int payrollId)
{
    var pendingAdvances = await _context.Advances
        .Where(a => a.EmployeeId == employeeId &&
                    a.YearMonth == yearMonth &&
                    a.AdvanceStatus == "DISBURSED" &&
                    !a.IsDeducted &&
                    a.Status != -1)
        .ToListAsync();

    foreach (var adv in pendingAdvances)
    {
        adv.IsDeducted = true;
        adv.DeductedInPayrollId = payrollId;
        adv.DeductedAt = DateTime.UtcNow;
        adv.AdvanceStatus = "DEDUCTED";
    }

    await _context.SaveChangesAsync();
}
```

---

## 8. Status Flow

```
┌─────────┐
│ PENDING │ ← Mới tạo
└────┬────┘
     │ Approve
     ▼
┌──────────┐
│ APPROVED │ ← Đã duyệt, chờ chi tiền
└────┬─────┘
     │ MarkDisbursed
     ▼
┌───────────┐
│ DISBURSED │ ← Đã chi tiền, chờ trừ lương
└─────┬─────┘
      │ Payroll.Calculate
      ▼
┌──────────┐
│ DEDUCTED │ ← Đã trừ vào lương
└──────────┘

┌─────────┐
│ PENDING │
└────┬────┘
     │ Reject
     ▼
┌──────────┐
│ REJECTED │ ← Từ chối
└──────────┘
```

---

## 9. Validation Rules

```csharp
// 1. Không được tạm ứng quá 50% lương dự kiến
public async Task<bool> ValidateAdvanceLimit(int employeeId, string yearMonth, decimal newAmount)
{
    var estimatedSalary = await EstimateSalary(employeeId, yearMonth);
    var totalPending = await GetTotalPendingAdvances(employeeId, yearMonth);
    
    var maxAllowed = estimatedSalary * 0.50m;
    return (totalPending + newAmount) <= maxAllowed;
}

// 2. Không được tạm ứng khi đã có lương tháng đó
public async Task<bool> HasPayrollLocked(int employeeId, string yearMonth)
{
    return await _context.Payrolls.AnyAsync(p =>
        p.EmployeeId == employeeId &&
        p.YearMonth == yearMonth &&
        p.PayrollStatus == "LOCKED" &&
        p.Status != -1);
}
```

---

## 10. UI Requirements

### 10.1 Màn Hình Tạo Tạm Ứng
- Chọn nhân viên (autocomplete)
- Chọn tháng (mặc định tháng hiện tại)
- Chọn loại tạm ứng
- Nhập số tiền (hiển thị limit còn lại)
- Nhập lý do
- Preview: DRC áp dụng, số tiền quy đổi

### 10.2 Màn Hình Phê Duyệt
- Danh sách chờ duyệt
- Filter: Tất cả / Đội của tôi / Theo tháng
- Bulk approve/reject
- Hiển thị history tạm ứng của CN

### 10.3 Màn Hình Theo Dõi
- Danh sách tạm ứng đã chi
- Đánh dấu đã trừ khi chạy payroll
- Report: Tổng tạm ứng theo đội / tháng
