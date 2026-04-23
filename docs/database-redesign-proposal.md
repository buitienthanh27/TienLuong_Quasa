# Database Redesign Proposal - Loại bỏ HARDCODE

> **Ngày:** 2026-04-22
> **Mục tiêu:** Thiết kế lại database để mọi hệ số, tham số đều cấu hình được qua UI

---

## 1. VẤN ĐỀ HIỆN TẠI

### 1.1 Hardcode trong S_Payroll.cs

```csharp
// Hệ số hiệu suất - HARDCODE
"A" => 1.00m, "B" => 0.95m, "C" => 0.90m, "D" => 0.85m, "E" => 0.80m

// Fallback values - HARDCODE
if (bhxhRate == 0) bhxhRate = 0.08m;
if (bhytRate == 0) bhytRate = 0.015m;
if (p7 == 0) p7 = 27.0m;

// Thuế 6 bậc - HARDCODE fallback
(5000000, 0.05m), (10000000, 0.10m), ...
```

### 1.2 Thiếu bảng cấu hình

- Không có nơi lưu hệ số hiệu suất (Performance Coefficient)
- Không có nơi lưu hệ số công từng loại (46.1, 76.9, 30.7...)
- Không có nơi lưu đơn giá theo hạng kỹ thuật

---

## 2. ĐỀ XUẤT BẢNG MỚI

### 2.1 Bảng `performance_coefficient` - Hệ số hiệu suất

**Mục đích:** Lưu hệ số hiệu suất (1.0, 0.95, 0.90...) thay vì hardcode

```sql
CREATE TABLE [performance_coefficient] (
    [id] int NOT NULL IDENTITY,
    [grade] nvarchar(5) NOT NULL,           -- A, B, C, D, E
    [coefficient] decimal(5,4) NOT NULL,     -- 1.0000, 0.9500, 0.9000...
    [description] nvarchar(200) NULL,
    [effective_date] date NOT NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_performance_coefficient] PRIMARY KEY ([id])
);

-- Unique: 1 hệ số per grade per effective_date
CREATE UNIQUE INDEX [IX_perf_coef_grade_date] 
    ON [performance_coefficient] ([grade], [effective_date]) 
    WHERE [status] != -1;
```

**Seed data:**
```sql
INSERT INTO performance_coefficient (grade, coefficient, description, effective_date, status, created_at, created_by)
VALUES 
('A', 1.0000, 'Hạng A - Xuất sắc', '2025-01-01', 1, GETUTCDATE(), 1),
('B', 0.9500, 'Hạng B - Tốt', '2025-01-01', 1, GETUTCDATE(), 1),
('C', 0.9000, 'Hạng C - Khá', '2025-01-01', 1, GETUTCDATE(), 1),
('D', 0.8500, 'Hạng D - Trung bình', '2025-01-01', 1, GETUTCDATE(), 1),
('E', 0.8000, 'Hạng E - Yếu', '2025-01-01', 1, GETUTCDATE(), 1);
```

---

### 2.2 Bảng `attendance_coefficient` - Hệ số công

**Mục đích:** Lưu hệ số công cho từng loại công (46.1, 76.9, 30.7...)

```sql
CREATE TABLE [attendance_coefficient] (
    [id] int NOT NULL IDENTITY,
    [tram_id] int NOT NULL,                  -- FK → tram
    [attendance_type] nvarchar(50) NOT NULL, -- REGULAR, SUNDAY, YOUNG_TREE, HARDSHIP, DOUBLE_CUT, CARE
    [coefficient] decimal(10,4) NOT NULL,    -- 46.1, 76.9, 30.7...
    [unit] nvarchar(20) NULL,                -- 'KIP/DRC', 'KIP/DAY'
    [description] nvarchar(200) NULL,
    [effective_date] date NOT NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_attendance_coefficient] PRIMARY KEY ([id]),
    CONSTRAINT [FK_att_coef_tram] FOREIGN KEY ([tram_id]) REFERENCES [tram] ([id])
);

CREATE UNIQUE INDEX [IX_att_coef_tram_type_date] 
    ON [attendance_coefficient] ([tram_id], [attendance_type], [effective_date]) 
    WHERE [status] != -1;
```

**Seed data (theo Excel):**
```sql
-- TRẠM 1
INSERT INTO attendance_coefficient (tram_id, attendance_type, coefficient, unit, description, effective_date, status, created_at, created_by)
VALUES 
(1, 'REGULAR', 46.1, 'KIP/DRC', 'Công thường × DRC', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'SUNDAY', 76.9, 'KIP/DRC', 'Công CN × DRC (gấp 1.67)', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'YOUNG_TREE', 30.7, 'KIP/DRC', 'Công cây non × DRC', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'HARDSHIP', 20000, 'KIP/DAY', 'Phụ cấp khộp nặng/ngày', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'DOUBLE_CUT', 46.1, 'KIP/DRC', 'Cạo 2 lát × DRC', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'CARE', 25000, 'KIP/DAY', 'Công chăm sóc/ngày', '2025-01-01', 1, GETUTCDATE(), 1);

-- TRẠM 2 (có thể khác)
INSERT INTO attendance_coefficient (tram_id, attendance_type, coefficient, unit, description, effective_date, status, created_at, created_by)
VALUES 
(2, 'REGULAR', 46.1, 'KIP/DRC', 'Công thường × DRC', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'SUNDAY', 76.9, 'KIP/DRC', 'Công CN × DRC', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'YOUNG_TREE', 30.7, 'KIP/DRC', 'Công cây non × DRC', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'HARDSHIP', 20000, 'KIP/DAY', 'Phụ cấp khộp nặng/ngày', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'DOUBLE_CUT', 46.1, 'KIP/DRC', 'Cạo 2 lát × DRC', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'CARE', 25000, 'KIP/DAY', 'Công chăm sóc/ngày', '2025-01-01', 1, GETUTCDATE(), 1);
```

---

### 2.3 Bảng `rubber_price` - Đơn giá mủ theo hạng

**Mục đích:** Lưu đơn giá kg mủ khô theo hạng kỹ thuật và trạm

```sql
CREATE TABLE [rubber_price] (
    [id] int NOT NULL IDENTITY,
    [tram_id] int NOT NULL,                  -- FK → tram
    [grade] nvarchar(5) NOT NULL,            -- A, B, C, D, E (hạng kỹ thuật)
    [price_per_kg] decimal(18,4) NOT NULL,   -- Đơn giá VND/kg hoặc Bath/kg
    [currency] nvarchar(10) NOT NULL,        -- 'VND', 'THB', 'LAK'
    [description] nvarchar(200) NULL,
    [effective_date] date NOT NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_rubber_price] PRIMARY KEY ([id]),
    CONSTRAINT [FK_rubber_price_tram] FOREIGN KEY ([tram_id]) REFERENCES [tram] ([id])
);

CREATE UNIQUE INDEX [IX_rubber_price_tram_grade_date] 
    ON [rubber_price] ([tram_id], [grade], [effective_date]) 
    WHERE [status] != -1;
```

**Seed data:**
```sql
-- TRẠM 1 - Đơn giá theo hạng (Bath/kg)
INSERT INTO rubber_price (tram_id, grade, price_per_kg, currency, description, effective_date, status, created_at, created_by)
VALUES 
(1, 'A', 92000, 'LAK', 'Hạng A - Chất lượng cao nhất', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'B', 89000, 'LAK', 'Hạng B - Chất lượng tốt', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'C', 86000, 'LAK', 'Hạng C - Chất lượng khá', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'D', 83000, 'LAK', 'Hạng D - Chất lượng trung bình', '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'E', 80000, 'LAK', 'Hạng E - Chất lượng thấp', '2025-01-01', 1, GETUTCDATE(), 1);

-- TRẠM 2
INSERT INTO rubber_price (tram_id, grade, price_per_kg, currency, description, effective_date, status, created_at, created_by)
VALUES 
(2, 'A', 77000, 'LAK', 'Hạng A - Chất lượng cao nhất', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'B', 74000, 'LAK', 'Hạng B - Chất lượng tốt', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'C', 71000, 'LAK', 'Hạng C - Chất lượng khá', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'D', 68000, 'LAK', 'Hạng D - Chất lượng trung bình', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'E', 65000, 'LAK', 'Hạng E - Chất lượng thấp', '2025-01-01', 1, GETUTCDATE(), 1);
```

---

### 2.4 Bảng `tax_bracket` - Bậc thuế TNCN

**Mục đích:** Lưu bậc thuế lũy tiến thay vì hardcode

```sql
CREATE TABLE [tax_bracket] (
    [id] int NOT NULL IDENTITY,
    [bracket_order] int NOT NULL,            -- 1, 2, 3, 4, 5, 6
    [min_income] decimal(18,4) NOT NULL,     -- 0, 5M, 10M...
    [max_income] decimal(18,4) NOT NULL,     -- 5M, 10M, MAX...
    [tax_rate] decimal(5,4) NOT NULL,        -- 0.05, 0.10, 0.15...
    [description] nvarchar(200) NULL,
    [effective_date] date NOT NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_tax_bracket] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_tax_bracket_order_date] 
    ON [tax_bracket] ([bracket_order], [effective_date]) 
    WHERE [status] != -1;
```

**Seed data:**
```sql
INSERT INTO tax_bracket (bracket_order, min_income, max_income, tax_rate, description, effective_date, status, created_at, created_by)
VALUES 
(1, 0, 5000000, 0.05, 'Bậc 1: 0 - 5 triệu (5%)', '2025-01-01', 1, GETUTCDATE(), 1),
(2, 5000000, 10000000, 0.10, 'Bậc 2: 5 - 10 triệu (10%)', '2025-01-01', 1, GETUTCDATE(), 1),
(3, 10000000, 18000000, 0.15, 'Bậc 3: 10 - 18 triệu (15%)', '2025-01-01', 1, GETUTCDATE(), 1),
(4, 18000000, 32000000, 0.20, 'Bậc 4: 18 - 32 triệu (20%)', '2025-01-01', 1, GETUTCDATE(), 1),
(5, 32000000, 52000000, 0.25, 'Bậc 5: 32 - 52 triệu (25%)', '2025-01-01', 1, GETUTCDATE(), 1),
(6, 52000000, 9999999999, 0.35, 'Bậc 6: Trên 52 triệu (35%)', '2025-01-01', 1, GETUTCDATE(), 1);
```

---

### 2.5 Bảng `exchange_rate` - Tỷ giá ngoại tệ

**Mục đích:** Lưu tỷ giá hàng tháng (Bath, USD, VND → KIP)

```sql
CREATE TABLE [exchange_rate] (
    [id] int NOT NULL IDENTITY,
    [from_currency] nvarchar(10) NOT NULL,   -- 'THB', 'USD', 'VND'
    [to_currency] nvarchar(10) NOT NULL,     -- 'LAK'
    [rate] decimal(18,4) NOT NULL,           -- 611.23
    [year_month] nvarchar(7) NOT NULL,       -- '2025-11'
    [description] nvarchar(200) NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_exchange_rate] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_exchange_rate_currencies_month] 
    ON [exchange_rate] ([from_currency], [to_currency], [year_month]) 
    WHERE [status] != -1;
```

**Seed data:**
```sql
INSERT INTO exchange_rate (from_currency, to_currency, rate, year_month, description, status, created_at, created_by)
VALUES 
('THB', 'LAK', 611.23, '2025-11', 'Tỷ giá Bath → Kíp tháng 11/2025', 1, GETUTCDATE(), 1),
('THB', 'LAK', 615.50, '2025-12', 'Tỷ giá Bath → Kíp tháng 12/2025', 1, GETUTCDATE(), 1),
('USD', 'LAK', 21500, '2025-11', 'Tỷ giá USD → Kíp tháng 11/2025', 1, GETUTCDATE(), 1);
```

---

## 3. CẬP NHẬT SYSTEM_PARAMETER

Bổ sung các tham số còn thiếu:

```sql
-- Các tham số cần thêm
INSERT INTO system_parameter (param_code, param_name, param_value, data_type, effective_date, description, status, created_at, created_by)
VALUES 
-- Hệ số khoán cơ bản
('BASE_COEFFICIENT', 'Hệ số khoán cơ bản', 292.59, 'DECIMAL', '2025-01-01', '360/27×22 = 292.59', 1, GETUTCDATE(), 1),

-- Số ngày công chuẩn/tháng
('STANDARD_WORK_DAYS', 'Số ngày công chuẩn', 26, 'INT', '2025-01-01', 'Số ngày làm việc chuẩn/tháng', 1, GETUTCDATE(), 1),

-- Ngưỡng miễn thuế
('TAX_EXEMPTION', 'Ngưỡng miễn thuế TNCN', 11000000, 'DECIMAL', '2025-01-01', 'Thu nhập dưới mức này không chịu thuế', 1, GETUTCDATE(), 1),

-- Đơn vị tiền tệ mặc định
('DEFAULT_CURRENCY', 'Đơn vị tiền tệ mặc định', 'LAK', 'STRING', '2025-01-01', 'LAK = Kíp Lào', 1, GETUTCDATE(), 1);
```

---

## 4. TỔNG HỢP TẤT CẢ BẢNG

### 4.1 Bảng hiện có (giữ nguyên)

| Bảng | Mục đích |
|------|----------|
| `tram` | Danh sách trạm (T1, T2) |
| `employee` | Danh sách nhân viên |
| `department` | Phòng ban |
| `position` | Chức vụ |
| `attendance` | Chấm công tháng |
| `production` | Sản lượng mủ |
| `drc_rate` | Tỷ lệ DRC theo tháng/trạm |
| `salary_scale` | Bảng lương cơ bản theo hạng/trạm |
| `system_parameter` | Tham số hệ thống chung |
| `allowance_type` | Loại phụ cấp |
| `allowance` | Phụ cấp theo NV/tháng |
| `cost_center` | Trung tâm chi phí |
| `payroll` | Kết quả tính lương |
| `payroll_detail` | Chi tiết từng khoản |
| `cost_allocation` | Phân bổ chi phí |
| `audit_log` | Lịch sử thay đổi |

### 4.2 Bảng mới (cần tạo)

| Bảng | Mục đích | Thay thế hardcode |
|------|----------|-------------------|
| `performance_coefficient` | Hệ số hiệu suất A/B/C/D/E | `GetPerformanceCoefficient()` |
| `attendance_coefficient` | Hệ số công (46.1, 76.9, 30.7...) | Công thức tính lương |
| `rubber_price` | Đơn giá mủ theo hạng/trạm | Tính tiền sản lượng |
| `tax_bracket` | Bậc thuế TNCN lũy tiến | `CalculateProgressiveTax()` |
| `exchange_rate` | Tỷ giá hàng tháng | Convert tiền tệ |

---

## 5. CÔNG THỨC TÍNH LƯƠNG MỚI

### 5.1 Luồng tính lương cho Công nhân kỹ thuật (CNKT)

```
BƯỚC 1: Lấy DRC từ drc_rate (theo trạm, tháng)
   DRC = SELECT drc_value FROM drc_rate WHERE tram_id=? AND year_month=?

BƯỚC 2: Tính tiền sản lượng mủ
   DryLatexKg = RawLatexKg × DRC + RopeLatexKg × DRC + SerumKg × DRC_Serum
   RubberPrice = SELECT price_per_kg FROM rubber_price WHERE tram_id=? AND grade=?
   ProductionSalary = DryLatexKg × RubberPrice

BƯỚC 3: Tính tiền công từng loại
   AttCoef_Regular = SELECT coefficient FROM attendance_coefficient WHERE type='REGULAR'
   AttCoef_Sunday = SELECT coefficient FROM attendance_coefficient WHERE type='SUNDAY'
   ...
   
   RegularPay = RegularDays × AttCoef_Regular × DRC
   SundayPay = SundayDays × AttCoef_Sunday × DRC
   YoungTreePay = YoungTreeDays × AttCoef_YoungTree × DRC
   HardshipPay = HardshipDays × AttCoef_Hardship (không × DRC vì đơn vị KIP/DAY)
   CarePay = CareDays × AttCoef_Care (không × DRC)

BƯỚC 4: Tính tổng lương brutto
   GrossSalary = ProductionSalary + RegularPay + SundayPay + ... + Allowances

BƯỚC 5: Tính thuế TNCN lũy tiến
   TaxBrackets = SELECT * FROM tax_bracket ORDER BY bracket_order
   Tax = CalculateProgressiveTax(GrossSalary, TaxBrackets)

BƯỚC 6: Tính các khoản khấu trừ
   BHXH = GrossSalary × BHXH_RATE (nếu có bảo hiểm)
   BHYT = GrossSalary × BHYT_RATE (nếu có bảo hiểm)
   TotalDeductions = Tax + BHXH + BHYT

BƯỚC 7: Tính lương NET
   NetSalary = GrossSalary - TotalDeductions
```

### 5.2 Luồng tính lương cho Cán bộ (INDIRECT)

```
BƯỚC 1: Lấy lương cơ bản từ salary_scale
   BaseSalary = SELECT base_rate FROM salary_scale WHERE tram_id=? AND grade=?

BƯỚC 2: Tính hệ số hiệu suất
   PerfCoef = SELECT coefficient FROM performance_coefficient WHERE grade=?

BƯỚC 3: Tính lương theo công thức Excel
   P7 = SELECT param_value FROM system_parameter WHERE param_code='P7'
   WorkDays = Attendance.TotalDays
   
   CalculatedSalary = (BaseSalary + WorkDays) / P7 × PerfCoef

BƯỚC 4-7: Giống CNKT
```

---

## 6. CHECKLIST IMPLEMENTATION

- [ ] Tạo Entity `PerformanceCoefficient.cs`
- [ ] Tạo Entity `AttendanceCoefficient.cs`
- [ ] Tạo Entity `RubberPrice.cs`
- [ ] Tạo Entity `TaxBracket.cs`
- [ ] Tạo Entity `ExchangeRate.cs`
- [ ] Cập nhật `MainDbContext.cs` thêm DbSet
- [ ] Tạo Migration
- [ ] Tạo DTOs (MReq_, MRes_)
- [ ] Tạo Services (S_PerformanceCoefficient, S_AttendanceCoefficient...)
- [ ] Tạo Controllers
- [ ] Cập nhật `AutoMapperProfile.cs`
- [ ] Cập nhật `S_Payroll.cs` sử dụng bảng mới
- [ ] Tạo Frontend pages để quản lý các bảng mới
- [ ] Seed data mặc định
- [ ] Test tính lương

---

## 7. KẾT LUẬN

Với thiết kế này:
- **100% tham số có thể cấu hình qua UI**
- **Không còn hardcode** trong business logic
- **Có lịch sử thay đổi** theo effective_date
- **Dễ mở rộng** khi có yêu cầu mới
- **Audit trail** đầy đủ

Bạn có muốn tôi bắt đầu implement không?
