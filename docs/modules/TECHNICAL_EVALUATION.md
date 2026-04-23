# Module: TECHNICAL_EVALUATION (Đánh Giá Kỹ Thuật)

## 1. Tổng Quan

**Mục đích**: Quản lý quy trình đánh giá kỹ thuật công nhân khai thác mủ, xác định hạng kỹ thuật (A/B/C/D) để tính đơn giá lương.

**Entity**: `Performance` (TechnicalEvaluation)

**Dependencies**:
- `Employee` - Công nhân được đánh giá
- Output cho `Payroll` - Hạng kỹ thuật quyết định đơn giá

---

## 2. Quy Trình Đánh Giá (Quasa)

### 2.1 Flow Chấm Điểm

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    QUY TRÌNH ĐÁNH GIÁ KỸ THUẬT QUASA                        │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  Cuối tháng                                                                 │
│  ┌──────────────┐                                                           │
│  │ Đội trưởng/  │──▶ Chấm điểm toàn bộ CN trong đội                         │
│  │ QLKT đội     │    - Đánh giá ngẫu nhiên các phần cây                     │
│  └──────────────┘    - Ghi nhận điểm + xếp hạng A/B/C/D                     │
│         │                                                                   │
│         ▼                                                                   │
│  ┌──────────────┐                                                           │
│  │ Hệ thống lưu │──▶ TeamScore, TeamGrade, TeamScoredAt, TeamScoredBy       │
│  │ điểm đội     │                                                           │
│  └──────────────┘                                                           │
│         │                                                                   │
│         ▼                                                                   │
│  ┌──────────────┐     ┌─────────────────────────────────────┐              │
│  │ Công ty      │ YES │ Điểm công ty override điểm đội      │              │
│  │ phúc tra?    │────▶│ CompanyGrade = FinalGrade           │              │
│  └──────────────┘     └─────────────────────────────────────┘              │
│         │ NO                                                                │
│         ▼                                                                   │
│  ┌──────────────────────────────────────┐                                  │
│  │ Điểm đội = Điểm cuối cùng            │                                  │
│  │ TeamGrade = FinalGrade                │                                  │
│  └──────────────────────────────────────┘                                  │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 2.2 Quy Tắc Ưu Tiên

| Trường hợp | Kết quả |
|------------|---------|
| Chỉ có điểm đội | FinalGrade = TeamGrade |
| Công ty đã phúc tra | FinalGrade = CompanyGrade (override) |
| CN phúc tra nhưng công ty chưa chấm lại | FinalGrade = TeamGrade |

**Nguyên tắc**: "Trường hợp Công ty phúc tra đột xuất chấm điểm → sẽ lấy điểm của Công ty chấm làm xếp hạng kỹ thuật"

---

## 3. Entity Structure

### 3.1 Performance (TechnicalEvaluation)

```csharp
[Table("performance")]
public partial class Performance : BaseEntity
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

    // ========== ĐIỂM ĐỘI CHẤM (Default) ==========
    [Column("team_score", TypeName = "decimal(5,2)")]
    public decimal TeamScore { get; set; }           // Điểm số 0-100

    [Column("team_grade")]
    [StringLength(1)]
    public string TeamGrade { get; set; }            // A/B/C/D

    [Column("team_scored_at", TypeName = "datetime")]
    public DateTime? TeamScoredAt { get; set; }

    [Column("team_scored_by")]
    public int? TeamScoredBy { get; set; }           // FK → Account

    [Column("team_notes")]
    [StringLength(500)]
    public string? TeamNotes { get; set; }           // Ghi chú đánh giá

    // ========== ĐIỂM CÔNG TY PHÚC TRA (Override) ==========
    [Column("company_reviewed")]
    public bool CompanyReviewed { get; set; } = false;  // Đã phúc tra chưa

    [Column("company_score", TypeName = "decimal(5,2)")]
    public decimal? CompanyScore { get; set; }

    [Column("company_grade")]
    [StringLength(1)]
    public string? CompanyGrade { get; set; }

    [Column("company_reviewed_at", TypeName = "datetime")]
    public DateTime? CompanyReviewedAt { get; set; }

    [Column("company_reviewed_by")]
    public int? CompanyReviewedBy { get; set; }      // FK → Account

    [Column("company_notes")]
    [StringLength(500)]
    public string? CompanyNotes { get; set; }

    // ========== COMPUTED FIELDS ==========
    // Kết quả cuối cùng (logic ở Service/DTO)
    // FinalGrade = CompanyReviewed ? CompanyGrade : TeamGrade
    // FinalScore = CompanyReviewed ? CompanyScore : TeamScore

    // Relationships
    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}
```

---

## 4. Bảng Quy Đổi Điểm → Hạng

| Điểm | Hạng | Đơn giá Trạm 1 (Bath/kg) | Đơn giá Trạm 2 (Bath/kg) |
|------|------|---------------------------|---------------------------|
| >= 90 | A | 9.2 | 7.7 |
| >= 80 | B | 8.9 | 7.4 |
| >= 70 | C | 8.6 | 7.1 |
| < 70 | D | 8.3 | 6.8 |

**Ghi chú**: 
- Trạm 1 có đơn giá cao hơn Trạm 2 (vùng khó khăn)
- Đơn giá thường cố định 1 năm, do Phòng KT quy định

---

## 5. Service Interface

```csharp
public interface IS_Performance
{
    // CRUD cơ bản
    Task<ResponseData<MRes_Performance>> Create(MReq_Performance request);
    Task<ResponseData<MRes_Performance>> Update(MReq_Performance request);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_Performance>> GetById(int id);
    Task<ResponseData<List<MRes_Performance>>> GetListByPaging(MReq_Performance_FullParam request);
    
    // Đội chấm điểm
    Task<ResponseData<MRes_Performance>> TeamScore(MReq_TeamScore request);
    Task<ResponseData<int>> TeamScoreBulk(MReq_TeamScoreBulk request);
    
    // Công ty phúc tra
    Task<ResponseData<MRes_Performance>> CompanyReview(MReq_CompanyReview request);
    
    // Lấy kết quả cuối cùng
    Task<ResponseData<MRes_Performance>> GetFinalGrade(int employeeId, string yearMonth);
}
```

---

## 6. API Endpoints

| Method | Endpoint | Mô tả | Quyền |
|--------|----------|-------|-------|
| POST | `/Performance/Create` | Tạo đánh giá mới | Đội trưởng |
| PUT | `/Performance/Update` | Cập nhật đánh giá | Đội trưởng |
| DELETE | `/Performance/Delete/{id}` | Xóa đánh giá | Đội trưởng |
| GET | `/Performance/GetById/{id}` | Xem chi tiết | Tất cả |
| GET | `/Performance/GetListByPaging` | Danh sách phân trang | Tất cả |
| POST | `/Performance/TeamScore` | Đội chấm điểm | Đội trưởng/QLKT |
| POST | `/Performance/TeamScoreBulk` | Đội chấm hàng loạt | Đội trưởng/QLKT |
| POST | `/Performance/CompanyReview` | Công ty phúc tra | Phòng KT công ty |
| GET | `/Performance/GetFinalGrade` | Lấy hạng cuối cùng | Tất cả |

---

## 7. Request/Response Models

### 7.1 MReq_TeamScore (Đội chấm điểm)
```csharp
public class MReq_TeamScore
{
    [Required]
    public int EmployeeId { get; set; }
    
    [Required]
    public string YearMonth { get; set; }
    
    [Required]
    [Range(0, 100)]
    public decimal Score { get; set; }
    
    public string? Notes { get; set; }
    
    public int ScoredBy { get; set; }
}
```

### 7.2 MReq_TeamScoreBulk (Chấm hàng loạt)
```csharp
public class MReq_TeamScoreBulk
{
    [Required]
    public string YearMonth { get; set; }
    
    public int ScoredBy { get; set; }
    
    public List<MReq_TeamScoreItem> Items { get; set; }
}

public class MReq_TeamScoreItem
{
    public int EmployeeId { get; set; }
    public decimal Score { get; set; }
    public string? Notes { get; set; }
}
```

### 7.3 MReq_CompanyReview (Công ty phúc tra)
```csharp
public class MReq_CompanyReview
{
    [Required]
    public int PerformanceId { get; set; }  // ID của record đã có điểm đội
    
    [Required]
    [Range(0, 100)]
    public decimal Score { get; set; }
    
    public string? Notes { get; set; }
    
    public int ReviewedBy { get; set; }
}
```

### 7.4 MRes_Performance
```csharp
public class MRes_Performance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? Msnv { get; set; }
    public string? EmployeeName { get; set; }
    public int? TramId { get; set; }
    public string? TramName { get; set; }
    
    public string YearMonth { get; set; }
    
    // Điểm đội
    public decimal TeamScore { get; set; }
    public string TeamGrade { get; set; }
    public DateTime? TeamScoredAt { get; set; }
    public string? TeamNotes { get; set; }
    
    // Điểm công ty (nếu có)
    public bool CompanyReviewed { get; set; }
    public decimal? CompanyScore { get; set; }
    public string? CompanyGrade { get; set; }
    public DateTime? CompanyReviewedAt { get; set; }
    public string? CompanyNotes { get; set; }
    
    // Kết quả cuối cùng
    public decimal FinalScore { get; set; }
    public string FinalGrade { get; set; }
    
    public short Status { get; set; }
}
```

---

## 8. Business Logic

### 8.1 Tính Grade từ Score
```csharp
public static string ScoreToGrade(decimal score)
{
    return score switch
    {
        >= 90m => "A",
        >= 80m => "B",
        >= 70m => "C",
        _      => "D"
    };
}
```

### 8.2 Team Score Service
```csharp
public async Task<ResponseData<MRes_Performance>> TeamScore(MReq_TeamScore request)
{
    try
    {
        // 1. Check if already exists for this month
        var existing = await _context.Performances.FirstOrDefaultAsync(x =>
            x.EmployeeId == request.EmployeeId &&
            x.YearMonth == request.YearMonth &&
            x.Status != -1);

        if (existing != null)
        {
            // Update existing
            existing.TeamScore = request.Score;
            existing.TeamGrade = ScoreToGrade(request.Score);
            existing.TeamScoredAt = DateTime.UtcNow;
            existing.TeamScoredBy = request.ScoredBy;
            existing.TeamNotes = request.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = request.ScoredBy;
        }
        else
        {
            // Create new
            var perf = new Performance
            {
                EmployeeId = request.EmployeeId,
                YearMonth = request.YearMonth,
                TeamScore = request.Score,
                TeamGrade = ScoreToGrade(request.Score),
                TeamScoredAt = DateTime.UtcNow,
                TeamScoredBy = request.ScoredBy,
                TeamNotes = request.Notes,
                CompanyReviewed = false,
                Status = 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.ScoredBy
            };
            _context.Performances.Add(perf);
        }

        await _context.SaveChangesAsync();
        return new ResponseData<MRes_Performance>(1, 200, "Chấm điểm thành công");
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(TeamScore), request);
    }
}
```

### 8.3 Company Review Service
```csharp
public async Task<ResponseData<MRes_Performance>> CompanyReview(MReq_CompanyReview request)
{
    try
    {
        var perf = await _context.Performances.FindAsync(request.PerformanceId);
        if (perf == null || perf.Status == -1)
            return Error(HttpStatusCode.NotFound, "Không tìm thấy bản đánh giá!");

        // Cập nhật điểm công ty (override)
        perf.CompanyReviewed = true;
        perf.CompanyScore = request.Score;
        perf.CompanyGrade = ScoreToGrade(request.Score);
        perf.CompanyReviewedAt = DateTime.UtcNow;
        perf.CompanyReviewedBy = request.ReviewedBy;
        perf.CompanyNotes = request.Notes;
        perf.UpdatedAt = DateTime.UtcNow;
        perf.UpdatedBy = request.ReviewedBy;

        await _context.SaveChangesAsync();
        return new ResponseData<MRes_Performance>(1, 200, "Phúc tra thành công - Điểm công ty sẽ được sử dụng");
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(CompanyReview), request);
    }
}
```

### 8.4 Get Final Grade (cho Payroll)
```csharp
public async Task<ResponseData<MRes_Performance>> GetFinalGrade(int employeeId, string yearMonth)
{
    var perf = await _context.Performances
        .AsNoTracking()
        .Include(x => x.Employee)
        .FirstOrDefaultAsync(x =>
            x.EmployeeId == employeeId &&
            x.YearMonth == yearMonth &&
            x.Status != -1);

    if (perf == null)
        return Error(HttpStatusCode.NotFound, "Chưa có đánh giá kỹ thuật cho CN này trong tháng!");

    var result = _mapper.Map<MRes_Performance>(perf);
    
    // Tính kết quả cuối cùng
    if (perf.CompanyReviewed && perf.CompanyScore.HasValue)
    {
        result.FinalScore = perf.CompanyScore.Value;
        result.FinalGrade = perf.CompanyGrade!;
    }
    else
    {
        result.FinalScore = perf.TeamScore;
        result.FinalGrade = perf.TeamGrade;
    }

    return new ResponseData<MRes_Performance>(1, 200) { data = result };
}
```

---

## 9. Tích Hợp Với Payroll

```csharp
// Trong S_Payroll.Calculate()
// Phase 1: Lấy hệ số hiệu suất từ Performance

var perfResult = await _s_Performance.GetFinalGrade(emp.Id, yearMonth);
string grade;
if (perfResult.result == 1 && perfResult.data != null)
{
    grade = perfResult.data.FinalGrade;  // Đã xử lý logic override
}
else
{
    grade = emp.TechnicalGrade ?? "C";  // Fallback to employee's default grade
}

decimal perfCoef = grade switch
{
    "A" => 1.00m,
    "B" => 0.95m,
    "C" => 0.90m,
    "D" => 0.85m,
    _   => 1.00m
};
```

---

## 10. Permissions

| Role | Quyền |
|------|-------|
| Công nhân | Xem điểm của bản thân |
| Đội trưởng | Chấm điểm đội, xem tất cả trong đội |
| QLKT | Chấm điểm, sửa điểm đội |
| Phòng KT Công ty | Phúc tra, override điểm |
| Admin | Full access |

---

## 11. UI Requirements

### 11.1 Màn Hình Đội Chấm Điểm
- Danh sách CN trong đội theo tháng
- Input điểm số (0-100)
- Auto tính Grade khi nhập điểm
- Bulk score (chấm nhiều CN cùng lúc)
- Filter: Chưa chấm / Đã chấm

### 11.2 Màn Hình Công Ty Phúc Tra
- Danh sách CN đã được đội chấm
- Hiển thị điểm đội
- Input điểm phúc tra
- Badge "Đã phúc tra" cho các record có CompanyReviewed = true
- So sánh điểm đội vs điểm công ty
