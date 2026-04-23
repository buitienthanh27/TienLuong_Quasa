# Module: PAYROLL (Tính Lương)

## 1. Tổng Quan

**Mục đích**: Tính toán lương hàng tháng cho nhân viên dựa trên chấm công, hiệu suất, phụ cấp và các tham số hệ thống.

**Entities liên quan**:
- `Payroll` - Bảng lương chính
- `PayrollDetail` - Chi tiết các khoản tính
- `CostAllocation` - Phân bổ chi phí

**Dependencies**:
- `Employee` - Thông tin nhân viên
- `Attendance` - Chấm công tháng
- `Performance` - Đánh giá hiệu suất
- `Allowance` - Phụ cấp
- `SalaryScale` - Bảng lương theo bậc
- `SystemParameter` - Tham số hệ thống (BHXH, thuế...)
- `CostCenter` - Trung tâm chi phí

---

## 2. Entity Structure

### 2.1 Payroll (Bảng lương chính)
```csharp
public class Payroll : BaseEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }           // FK → Employee
    public string YearMonth { get; set; }         // Format: "YYYY-MM" (e.g., "2026-04")
    
    // Calculated fields
    public decimal BaseSalary { get; set; }       // Lương cơ bản từ SalaryScale
    public decimal PerformanceCoef { get; set; }  // Hệ số hiệu suất (0.8 - 1.0)
    public decimal WorkingDays { get; set; }      // Số ngày công từ Attendance
    public decimal GrossSalary { get; set; }      // Lương gross = calculated salary
    
    // Deductions
    public decimal Bhxh { get; set; }             // BHXH 8%
    public decimal Bhyt { get; set; }             // BHYT 1.5%
    public decimal IncomeTax { get; set; }        // Thuế TNCN (progressive)
    public decimal TotalDeductions { get; set; }  // Tổng khấu trừ
    
    // Allowances & Net
    public decimal TotalAllowances { get; set; }  // Tổng phụ cấp
    public decimal NetSalary { get; set; }        // Thực lĩnh = Gross + Allowances - Deductions
    
    // Status
    public string PayrollStatus { get; set; }     // DRAFT | CONFIRMED | LOCKED
    public DateTime? CalculatedAt { get; set; }
    public int? CalculatedBy { get; set; }
    
    // Relationships
    public virtual Employee Employee { get; set; }
    public virtual ICollection<PayrollDetail> PayrollDetails { get; set; }
    public virtual ICollection<CostAllocation> CostAllocations { get; set; }
}
```

### 2.2 PayrollDetail (Chi tiết khoản tính)
```csharp
public class PayrollDetail
{
    public int Id { get; set; }
    public int PayrollId { get; set; }            // FK → Payroll
    public int Phase { get; set; }                // Giai đoạn tính (1-5)
    public string ItemCode { get; set; }          // Mã khoản mục
    public string Description { get; set; }       // Mô tả
    public decimal Amount { get; set; }           // Số tiền
    
    // Phase meanings:
    // 1 = PERF_COEF (Hệ số hiệu suất)
    // 2 = BASE_RATE (Lương cơ bản)
    // 3 = CALC_SALARY (Lương tính)
    // 4 = BHXH, BHYT, TAX (Các khoản trừ)
    // 5 = NET_SALARY (Lương thực nhận)
}
```

### 2.3 CostAllocation (Phân bổ chi phí)
```csharp
public class CostAllocation
{
    public int Id { get; set; }
    public int PayrollId { get; set; }            // FK → Payroll
    public int CostCenterId { get; set; }         // FK → CostCenter
    public decimal AllocatedAmount { get; set; }  // Số tiền phân bổ
}
```

---

## 3. Business Logic

### 3.0 Quy Định Làm Tròn & Định Dạng Số (Quasa)

#### Quy Tắc Định Dạng
| Loại dữ liệu | Quy tắc | Ví dụ |
|--------------|---------|-------|
| **Số thập phân** | Dấu phân cách là **dấu chấm** (.) | 9.25 kg |
| **Số thập phân** | Cắt lấy 2 số sau dấu thập phân (TRUNCATE, không làm tròn) | 9.256 → 9.25 |
| **Cân mủ tươi** | Không có số lẻ (cắt bỏ phần thập phân) | 9.3 kg → 9 kg |
| **Tổng lương** | Làm tròn đến đơn vị Ngàn (3 số cuối = 000) | 10,768,456 → 10,768,000 |

#### Code Helper Truncate
```csharp
// Hàm cắt số thập phân (không làm tròn)
public static decimal TruncateDecimal(decimal value, int decimals)
{
    var factor = (decimal)Math.Pow(10, decimals);
    return Math.Truncate(value * factor) / factor;
}

// Ví dụ sử dụng:
// Mủ quy khô: giữ 2 số thập phân (truncate)
var dryLatex = TruncateDecimal(freshLatex * drc, 2);  // 9.256 → 9.25

// Mủ tươi: không số lẻ
var freshLatexRounded = Math.Truncate(freshLatex);  // 9.3 → 9

// Tổng lương: làm tròn đến Ngàn
var netSalary = Math.Round(calculatedNet / 1000) * 1000;  // 10,768,456 → 10,768,000
```

### 3.1 Công Thức Tính Lương

```
┌─────────────────────────────────────────────────────────────────────┐
│                      CÔNG THỨC TÍNH LƯƠNG                          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Input:                                                             │
│  ├── Employee.TramId, Employee.TechnicalGrade                       │
│  ├── Attendance.WorkingDays (ngày công thực tế)                     │
│  ├── Performance.Grade (xếp loại A/B/C/D/E)                         │
│  ├── SalaryScale.BaseRate (lương cơ bản theo bậc + trạm)            │
│  └── SystemParameter: P7 (ngày công chuẩn), BHXH_RATE, BHYT_RATE    │
│                                                                     │
│  Phase 1: Hệ số hiệu suất                                           │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ PerformanceCoef = switch(Grade) {                           │   │
│  │     "A" => 1.00,                                            │   │
│  │     "B" => 0.95,                                            │   │
│  │     "C" => 0.90,                                            │   │
│  │     "D" => 0.85,                                            │   │
│  │     "E" => 0.80,                                            │   │
│  │     _   => 1.00                                             │   │
│  │ }                                                           │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  Phase 2: Lấy lương cơ bản từ SalaryScale                           │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ BaseRate = SalaryScale.BaseRate                             │   │
│  │            WHERE TramId = emp.TramId                        │   │
│  │              AND Grade = emp.TechnicalGrade                 │   │
│  │              AND EffectiveDate <= monthEnd                  │   │
│  │            ORDER BY EffectiveDate DESC                      │   │
│  │            TAKE 1                                           │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  Phase 3: Tính lương                                                │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ CalculatedSalary = (BaseRate + WorkingDays) / P7 * PerfCoef │   │
│  │                                                             │   │
│  │ Trong đó:                                                   │   │
│  │   P7 = Số ngày công chuẩn (default: 27)                     │   │
│  │   WorkingDays = Từ Attendance                               │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  Phase 4: Các khoản trừ                                             │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ IF (emp.InsuranceIncluded) {                                │   │
│  │     BHXH = CalculatedSalary * BHXH_RATE (default 8%)        │   │
│  │     BHYT = CalculatedSalary * BHYT_RATE (default 1.5%)      │   │
│  │ }                                                           │   │
│  │                                                             │   │
│  │ IncomeTax = CalculateProgressiveTax(CalculatedSalary)       │   │
│  │ // Thuế lũy tiến theo bảng TAX_TH_1..6, TAX_RATE_1..6       │   │
│  │                                                             │   │
│  │ TotalDeductions = BHXH + BHYT + IncomeTax                   │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│  Phase 5: Tổng kết                                                  │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ TotalAllowances = SUM(Allowance.CalculatedAmount)           │   │
│  │                   WHERE EmployeeId AND YearMonth            │   │
│  │                                                             │   │
│  │ GrossSalary = CalculatedSalary                              │   │
│  │ NetSalary = GrossSalary + TotalAllowances - TotalDeductions │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 3.2 Thuế Lũy Tiến (Progressive Tax)
```csharp
// Default tax brackets (có thể config qua SystemParameter)
var taxThresholds = new List<(decimal threshold, decimal rate)>
{
    (5_000_000,  0.05m),    // 5%  cho 0 - 5 triệu
    (10_000_000, 0.10m),    // 10% cho 5 - 10 triệu
    (18_000_000, 0.15m),    // 15% cho 10 - 18 triệu
    (32_000_000, 0.20m),    // 20% cho 18 - 32 triệu
    (52_000_000, 0.25m),    // 25% cho 32 - 52 triệu
    (80_000_000, 0.30m),    // 30% cho 52 - 80 triệu
    (decimal.MaxValue, 0.35m) // 35% cho > 80 triệu
};

// Tính thuế theo từng bậc
decimal tax = 0;
decimal previousThreshold = 0;
foreach (var (threshold, rate) in taxThresholds.OrderBy(t => t.threshold))
{
    if (income <= previousThreshold) break;
    var taxableAmount = Math.Min(income, threshold) - previousThreshold;
    if (taxableAmount > 0)
        tax += taxableAmount * rate;
    previousThreshold = threshold;
}
```

---

## 4. Service Interface

```csharp
public interface IS_Payroll
{
    // Tính lương hàng loạt
    Task<ResponseData<MRes_PayrollCalculateSummary>> Calculate(MReq_PayrollCalculate request);
    
    // Tính lại cho 1 nhân viên
    Task<ResponseData<MRes_PayrollResult>> Recalculate(int payrollId, int calculatedBy);
    
    // Xem chi tiết
    Task<ResponseData<MRes_PayrollResult>> GetDetailById(int payrollId);
    
    // Danh sách có phân trang
    Task<ResponseData<List<MRes_Payroll>>> GetListByPaging(MReq_Payroll_FullParam request);
    
    // Danh sách theo tháng
    Task<ResponseData<List<MRes_Payroll>>> GetListByYearMonth(string yearMonth);
}
```

---

## 5. API Endpoints

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/Payroll/Calculate` | Tính lương hàng loạt cho tháng |
| POST | `/Payroll/Recalculate/{id}` | Tính lại lương cho 1 payroll |
| GET | `/Payroll/GetDetailById/{id}` | Xem chi tiết bảng lương |
| GET | `/Payroll/GetListByPaging` | Danh sách có phân trang |
| GET | `/Payroll/GetListByYearMonth` | Danh sách theo tháng |

---

## 6. Request/Response Models

### 6.1 MReq_PayrollCalculate
```csharp
public class MReq_PayrollCalculate
{
    [Required]
    public string YearMonth { get; set; }      // "2026-04"
    public List<int>? EmployeeIds { get; set; } // null = tất cả nhân viên
    public int CalculatedBy { get; set; }
}
```

### 6.2 MRes_PayrollCalculateSummary
```csharp
public class MRes_PayrollCalculateSummary
{
    public string YearMonth { get; set; }
    public int TotalEmployees { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public decimal TotalNetSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalAllowances { get; set; }
    public List<string> Errors { get; set; }
}
```

### 6.3 MRes_PayrollResult
```csharp
public class MRes_PayrollResult
{
    public MRes_Payroll Payroll { get; set; }
    public List<MRes_PayrollDetail> Details { get; set; }
    public List<MRes_CostAllocation> CostAllocations { get; set; }
}
```

---

## 7. Workflow Tính Lương

```
┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐
│ BƯỚC 1  │────▶│ BƯỚC 2  │────▶│ BƯỚC 3  │────▶│ BƯỚC 4  │────▶│ BƯỚC 5  │
│ Chấm    │     │ Đánh    │     │ Phụ     │     │ Tính    │     │ Phê     │
│ Công    │     │ Giá     │     │ Cấp     │     │ Lương   │     │ Duyệt   │
└─────────┘     └─────────┘     └─────────┘     └─────────┘     └─────────┘
     │               │               │               │               │
     ▼               ▼               ▼               ▼               ▼
 Attendance      Performance     Allowance       Payroll         Status
 CONFIRMED       CONFIRMED       CONFIRMED       DRAFT          LOCKED
```

### Status Flow
- `DRAFT` → Mới tính, có thể tính lại
- `CONFIRMED` → Đã xác nhận, có thể tính lại
- `LOCKED` → Đã khóa, KHÔNG thể tính lại

---

## 8. System Parameters Cần Thiết

| Code | Mô tả | Default |
|------|-------|---------|
| P7 | Số ngày công chuẩn/tháng | 27 |
| BHXH_RATE | Tỷ lệ BHXH nhân viên | 0.08 (8%) |
| BHYT_RATE | Tỷ lệ BHYT nhân viên | 0.015 (1.5%) |
| TAX_TH_1 | Ngưỡng thuế bậc 1 | 5,000,000 |
| TAX_RATE_1 | Thuế suất bậc 1 | 5 |
| TAX_TH_2..6 | Ngưỡng thuế bậc 2-6 | ... |
| TAX_RATE_2..6 | Thuế suất bậc 2-6 | ... |

---

## 9. Business Rules

1. **Prerequisite**: Phải có `Attendance` cho nhân viên trong tháng tính lương
2. **Performance**: Nếu không có, dùng `Employee.TechnicalGrade` làm Grade
3. **SalaryScale**: Lấy bảng lương có `EffectiveDate <= monthEnd`, mới nhất
4. **InsuranceIncluded**: Nếu `Employee.InsuranceIncluded = false`, không tính BHXH/BHYT
5. **Recalculate**: Không thể tính lại nếu `PayrollStatus = "LOCKED"`
6. **Overwrite**: Khi tính lại, xóa hết `PayrollDetails` cũ và tạo mới

---

## 9.1 Quy Trình Đánh Giá Kỹ Thuật (KTNN) - Quasa

### Workflow Chấm Điểm Kỹ Thuật

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    QUY TRÌNH CHẤM ĐIỂM KỸ THUẬT                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────┐     ┌──────────────┐     ┌─────────────────┐                 │
│  │ BƯỚC 1   │────▶│   BƯỚC 2     │────▶│    BƯỚC 3       │                 │
│  │ Đội chấm │     │ Công ty      │     │ Xác định        │                 │
│  │ điểm CN  │     │ phúc tra?    │     │ kết quả cuối    │                 │
│  └──────────┘     └──────────────┘     └─────────────────┘                 │
│       │                 │                      │                           │
│       ▼                 ▼                      ▼                           │
│   Điểm Đội          Điểm Công ty          Grade A/B/C/D                   │
│   (default)         (override)            → Đơn giá                       │
│                                                                             │
│  Logic quyết định:                                                          │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ IF (CongtyDaPhucTra == true)                                        │   │
│  │     FinalGrade = GradeTuCongty    // Điểm Công ty chấm (override)   │   │
│  │ ELSE                                                                 │   │
│  │     FinalGrade = GradeTuDoi       // Điểm Đội chấm (default)        │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
│  Ghi chú:                                                                   │
│  - Đội chấm cho công nhân: Thực hiện cuối mỗi tháng                        │
│  - Công ty phúc tra: Đột xuất, không báo trước, lấy mẫu ngẫu nhiên         │
│  - Nếu Công ty đã phúc tra → Điểm Công ty có độ ưu tiên cao hơn            │
│  - Xếp hạng kỹ thuật quyết định đơn giá CN được hưởng tháng đó             │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Entity Performance (Đánh giá kỹ thuật)
```csharp
public class Performance : BaseEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string YearMonth { get; set; }
    
    // Điểm đội chấm (default)
    public decimal TeamScore { get; set; }
    public string TeamGrade { get; set; }       // A/B/C/D
    public DateTime? TeamScoredAt { get; set; }
    public int? TeamScoredBy { get; set; }
    
    // Điểm công ty phúc tra (override)
    public bool CompanyReviewed { get; set; } = false;
    public decimal? CompanyScore { get; set; }
    public string? CompanyGrade { get; set; }   // A/B/C/D
    public DateTime? CompanyReviewedAt { get; set; }
    public int? CompanyReviewedBy { get; set; }
    
    // Kết quả cuối cùng (computed)
    public string FinalGrade => CompanyReviewed ? CompanyGrade : TeamGrade;
    public decimal FinalScore => CompanyReviewed ? CompanyScore.Value : TeamScore;
}
```

### Điểm → Xếp Hạng
```csharp
// Quy đổi điểm thành hạng kỹ thuật
public static string ScoreToGrade(decimal score)
{
    return score switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        _     => "D"
    };
}
```

---

## 9.2 Tạm Ứng (Advance Payment) - Quasa

### Quy Định Tạm Ứng
- **DRC Tạm ứng**: 40% (fixed)
- **Khi nào tạm ứng**: Giữa tháng hoặc khi CN cần
- **Trừ lại**: Cuối tháng trừ vào bảng lương chính

### Entity Advance
```csharp
public class Advance : BaseEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string YearMonth { get; set; }
    
    public DateTime AdvanceDate { get; set; }   // Ngày tạm ứng
    public decimal Amount { get; set; }          // Số tiền tạm ứng
    public string Reason { get; set; }           // Lý do (lễ, đặc biệt...)
    
    // Tracking
    public bool IsDeducted { get; set; } = false;  // Đã trừ vào lương chưa
    public int? DeductedInPayrollId { get; set; }  // Trừ trong Payroll nào
}
```

### Công Thức Tính Với Tạm Ứng
```csharp
// Phase 5 (cập nhật): Tổng kết có tạm ứng
TotalAdvances = SUM(Advance.Amount)
                WHERE EmployeeId AND YearMonth AND IsDeducted = false

NetSalary = GrossSalary + TotalAllowances - TotalDeductions - TotalAdvances
```

### API Endpoints Tạm Ứng
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/Advance/Create` | Tạo tạm ứng |
| GET | `/Advance/GetByEmployee/{id}` | Xem tạm ứng theo NV |
| GET | `/Advance/GetPendingByMonth` | Danh sách chưa trừ |

---

## 10. Code Examples

### Gọi API Tính Lương
```javascript
// Tính lương cho tất cả nhân viên tháng 04/2026
POST /Payroll/Calculate
{
    "yearMonth": "2026-04",
    "employeeIds": null,  // null = tất cả
    "calculatedBy": 1
}

// Tính lương cho danh sách cụ thể
POST /Payroll/Calculate
{
    "yearMonth": "2026-04",
    "employeeIds": [1, 2, 3, 4, 5],
    "calculatedBy": 1
}
```

### Response Thành Công
```json
{
    "result": 1,
    "data": {
        "yearMonth": "2026-04",
        "totalEmployees": 150,
        "successCount": 145,
        "failedCount": 5,
        "totalGrossSalary": 850000000,
        "totalNetSalary": 720000000,
        "totalDeductions": 130000000,
        "totalAllowances": 50000000,
        "errors": [
            "NV156: Chưa có chấm công tháng 2026-04",
            "NV157: Chưa có chấm công tháng 2026-04"
        ]
    },
    "data2nd": 145
}
```
