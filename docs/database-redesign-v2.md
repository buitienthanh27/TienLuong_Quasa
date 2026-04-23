# Database Redesign V2 - Theo góp ý khách hàng

> **Ngày:** 2026-04-22
> **Cập nhật:** Dựa trên meeting với EcoTech 2A

---

## 1. TÓM TẮT YÊU CẦU NGHIỆP VỤ

### 1.1 Phân loại lao động

| Loại | Đơn vị tiền | Trả lương | Bảo hiểm | Ghi chú |
|------|-------------|-----------|----------|---------|
| **CNKT (Công nhân kỹ thuật)** | Bath → Kíp | Kíp | Không (trừ 1 số CN Lào có BH riêng) | 100% tại Lào |
| **Bảo vệ, Tạp vụ** | Kíp | Kíp | Không | Do Đội phụ trách |
| **Cán bộ gián tiếp** | Đô (Bath) | Đô | Có | Do P.Tổ chức làm |
| **Chăm sóc vườn cây** | Kíp | Kíp | Không | Thuê ngoài (khoán) |

### 1.2 Quy tắc tính lương CNKT

```
1. Quy khô = Khối lượng tươi × DRC (giữ nguyên số thập phân)
2. Thành tiền = Quy khô × Đơn giá theo hạng KT (làm tròn, không thập phân)
3. Đơn giá theo hạng A/B/C/D (ít thay đổi, ~1 năm/lần)
4. Vùng khó khăn: Cty hỗ trợ thêm đơn giá
5. DRC: Do Trạm cán + Đội chốt cuối tháng
6. Hạng KT: QLKT đánh giá 1 lần cuối tháng (có thể phúc tra)
```

### 1.3 Các loại phụ cấp & công

| Loại | Đơn giá | Đơn vị | Ghi chú |
|------|---------|--------|---------|
| Công chăm sóc | 25,000 | Kíp/công | Cố định |
| Phụ cấp khác | Cố định | Kíp | Thay đổi theo văn bản |
| Công ngày lễ | Không cố định | - | Trả tiền mặt → ghi tạm ứng → trừ cuối tháng |

### 1.4 Tỷ giá

- Nguồn: Vietinbank cuối tháng
- P.Kế toán cung cấp → P.Tổ chức xác nhận & ban hành
- Chung 1 tỷ giá cho toàn công ty/tháng

### 1.5 Mã lao động 2026

- Tự động phát sinh theo quy tắc (cần file quy định)

---

## 2. THIẾT KẾ DATABASE MỚI

### 2.1 Bảng `employee_type` - Loại lao động

```sql
CREATE TABLE [employee_type] (
    [id] int NOT NULL IDENTITY,
    [code] nvarchar(20) NOT NULL,            -- 'CNKT', 'BV', 'TV', 'CB', 'CS'
    [name] nvarchar(100) NOT NULL,           -- 'Công nhân kỹ thuật', 'Bảo vệ'...
    [salary_currency] nvarchar(10) NOT NULL, -- 'THB', 'LAK' (đơn vị tính lương)
    [payment_currency] nvarchar(10) NOT NULL,-- 'LAK', 'THB' (đơn vị trả lương)
    [has_insurance] bit NOT NULL DEFAULT 0,  -- Có bảo hiểm không
    [calculation_method] nvarchar(50) NOT NULL, -- 'PRODUCTION', 'FIXED', 'DAILY'
    [description] nvarchar(500) NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_employee_type] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_employee_type_code] ON [employee_type] ([code]) WHERE [status] != -1;
```

**Seed data:**
```sql
INSERT INTO employee_type (code, name, salary_currency, payment_currency, has_insurance, calculation_method, description, status, created_at, created_by)
VALUES 
('CNKT', N'Công nhân kỹ thuật', 'THB', 'LAK', 0, 'PRODUCTION', N'Lương theo sản lượng mủ, đơn giá Bath quy Kíp', 1, GETUTCDATE(), 1),
('BV', N'Bảo vệ', 'LAK', 'LAK', 0, 'FIXED', N'Lương cố định, đơn giá Kíp', 1, GETUTCDATE(), 1),
('TV', N'Tạp vụ', 'LAK', 'LAK', 0, 'FIXED', N'Lương cố định, đơn giá Kíp', 1, GETUTCDATE(), 1),
('CB', N'Cán bộ gián tiếp', 'THB', 'THB', 1, 'FIXED', N'Do P.Tổ chức, đơn giá Đô, có bảo hiểm', 1, GETUTCDATE(), 1),
('CS', N'Chăm sóc vườn', 'LAK', 'LAK', 0, 'DAILY', N'Thuê ngoài (khoán), 25000 Kíp/công', 1, GETUTCDATE(), 1);
```

---

### 2.2 Bảng `technical_grade` - Hạng kỹ thuật & Hệ số điểm

```sql
CREATE TABLE [technical_grade] (
    [id] int NOT NULL IDENTITY,
    [grade] nvarchar(5) NOT NULL,            -- 'A', 'B', 'C', 'D'
    [name] nvarchar(100) NOT NULL,           -- 'Hạng A - Xuất sắc'
    [point_coefficient] decimal(5,2) NOT NULL, -- Hệ số tính điểm (do P.KT quy định)
    [min_score] decimal(5,2) NULL,           -- Điểm tối thiểu để đạt hạng
    [max_score] decimal(5,2) NULL,           -- Điểm tối đa
    [description] nvarchar(500) NULL,
    [effective_date] date NOT NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_technical_grade] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_tech_grade_date] ON [technical_grade] ([grade], [effective_date]) WHERE [status] != -1;
```

**Seed data:**
```sql
INSERT INTO technical_grade (grade, name, point_coefficient, min_score, max_score, description, effective_date, status, created_at, created_by)
VALUES 
('A', N'Hạng A - Xuất sắc', 1.00, 90, 100, N'Chất lượng cao nhất', '2025-01-01', 1, GETUTCDATE(), 1),
('B', N'Hạng B - Tốt', 0.95, 75, 89.99, N'Chất lượng tốt', '2025-01-01', 1, GETUTCDATE(), 1),
('C', N'Hạng C - Khá', 0.90, 60, 74.99, N'Chất lượng khá', '2025-01-01', 1, GETUTCDATE(), 1),
('D', N'Hạng D - Trung bình', 0.85, 0, 59.99, N'Chất lượng trung bình', '2025-01-01', 1, GETUTCDATE(), 1);
```

---

### 2.3 Bảng `rubber_unit_price` - Đơn giá mủ theo hạng

```sql
CREATE TABLE [rubber_unit_price] (
    [id] int NOT NULL IDENTITY,
    [tram_id] int NOT NULL,                  -- FK → tram
    [grade] nvarchar(5) NOT NULL,            -- 'A', 'B', 'C', 'D'
    [unit_price] decimal(18,4) NOT NULL,     -- Đơn giá (Bath/kg)
    [currency] nvarchar(10) NOT NULL DEFAULT 'THB', -- Luôn là Bath
    [difficult_area_bonus] decimal(18,4) NULL, -- Hỗ trợ thêm vùng khó khăn
    [description] nvarchar(500) NULL,
    [effective_date] date NOT NULL,          -- Ngày hiệu lực (~1 năm đổi 1 lần)
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_rubber_unit_price] PRIMARY KEY ([id]),
    CONSTRAINT [FK_rubber_price_tram] FOREIGN KEY ([tram_id]) REFERENCES [tram] ([id])
);

CREATE UNIQUE INDEX [IX_rubber_price_tram_grade_date] 
    ON [rubber_unit_price] ([tram_id], [grade], [effective_date]) WHERE [status] != -1;
```

**Seed data (ví dụ - cần số liệu thực từ khách hàng):**
```sql
-- Giả sử đơn giá Bath/kg
INSERT INTO rubber_unit_price (tram_id, grade, unit_price, currency, difficult_area_bonus, effective_date, status, created_at, created_by)
VALUES 
-- TRẠM 1
(1, 'A', 150.00, 'THB', 10.00, '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'B', 145.00, 'THB', 10.00, '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'C', 140.00, 'THB', 10.00, '2025-01-01', 1, GETUTCDATE(), 1),
(1, 'D', 135.00, 'THB', 10.00, '2025-01-01', 1, GETUTCDATE(), 1),
-- TRẠM 2
(2, 'A', 150.00, 'THB', 0.00, '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'B', 145.00, 'THB', 0.00, '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'C', 140.00, 'THB', 0.00, '2025-01-01', 1, GETUTCDATE(), 1),
(2, 'D', 135.00, 'THB', 0.00, '2025-01-01', 1, GETUTCDATE(), 1);
```

---

### 2.4 Bảng `exchange_rate` - Tỷ giá hàng tháng

```sql
CREATE TABLE [exchange_rate] (
    [id] int NOT NULL IDENTITY,
    [year_month] nvarchar(7) NOT NULL,       -- '2025-11'
    [from_currency] nvarchar(10) NOT NULL,   -- 'THB'
    [to_currency] nvarchar(10) NOT NULL,     -- 'LAK'
    [rate] decimal(18,6) NOT NULL,           -- 611.230000
    [source] nvarchar(100) NULL,             -- 'Vietinbank'
    [approved_by] int NULL,                  -- FK → account (P.Tổ chức xác nhận)
    [approved_at] datetime NULL,
    [description] nvarchar(500) NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_exchange_rate] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_exchange_rate_month_currencies] 
    ON [exchange_rate] ([year_month], [from_currency], [to_currency]) WHERE [status] != -1;
```

---

### 2.5 Bảng `work_type` - Loại công việc & Đơn giá

```sql
CREATE TABLE [work_type] (
    [id] int NOT NULL IDENTITY,
    [code] nvarchar(50) NOT NULL,            -- 'REGULAR', 'SUNDAY', 'CARE', 'HOLIDAY'...
    [name] nvarchar(200) NOT NULL,           -- 'Công thường', 'Công chủ nhật'...
    [unit_price] decimal(18,4) NOT NULL,     -- Đơn giá
    [currency] nvarchar(10) NOT NULL,        -- 'LAK', 'THB'
    [calculation_type] nvarchar(50) NOT NULL,-- 'PER_DAY', 'PER_DRC', 'MULTIPLIER'
    [multiplier] decimal(5,2) NULL,          -- Hệ số nhân (VD: CN = 1.67 lần)
    [applies_to_type] nvarchar(20) NULL,     -- 'CNKT', 'BV', 'ALL'
    [description] nvarchar(500) NULL,
    [effective_date] date NOT NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_work_type] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_work_type_code_date] ON [work_type] ([code], [effective_date]) WHERE [status] != -1;
```

**Seed data:**
```sql
INSERT INTO work_type (code, name, unit_price, currency, calculation_type, multiplier, applies_to_type, effective_date, status, created_at, created_by)
VALUES 
('CARE', N'Công chăm sóc vườn', 25000, 'LAK', 'PER_DAY', NULL, 'CS', '2025-01-01', 1, GETUTCDATE(), 1),
('SUNDAY', N'Công chủ nhật', 0, 'LAK', 'MULTIPLIER', 1.67, 'CNKT', '2025-01-01', 1, GETUTCDATE(), 1),
('HARDSHIP', N'Phụ cấp vùng khó khăn', 20000, 'LAK', 'PER_DAY', NULL, 'CNKT', '2025-01-01', 1, GETUTCDATE(), 1),
('DOUBLE_CUT', N'Công cạo 2 lát', 0, 'LAK', 'MULTIPLIER', 2.00, 'CNKT', '2025-01-01', 1, GETUTCDATE(), 1),
('YOUNG_TREE', N'Công cây non', 0, 'LAK', 'MULTIPLIER', 0.67, 'CNKT', '2025-01-01', 1, GETUTCDATE(), 1);
```

---

### 2.6 Bảng `holiday` - Ngày lễ trong năm

```sql
CREATE TABLE [holiday] (
    [id] int NOT NULL IDENTITY,
    [year] int NOT NULL,                     -- 2025, 2026...
    [holiday_date] date NOT NULL,            -- Ngày lễ cụ thể
    [name] nvarchar(200) NOT NULL,           -- 'Tết Nguyên đán'
    [country] nvarchar(10) NOT NULL,         -- 'VN', 'LA', 'BOTH'
    [is_paid] bit NOT NULL DEFAULT 1,        -- Có trả lương không
    [bonus_rate] decimal(5,2) NULL,          -- Hệ số thưởng (nếu đi làm)
    [description] nvarchar(500) NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_holiday] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_holiday_date] ON [holiday] ([holiday_date]) WHERE [status] != -1;
```

---

### 2.7 Bảng `advance_payment` - Tạm ứng & Truy thu

```sql
CREATE TABLE [advance_payment] (
    [id] int NOT NULL IDENTITY,
    [employee_id] int NOT NULL,              -- FK → employee
    [year_month] nvarchar(7) NOT NULL,       -- '2025-11'
    [payment_type] nvarchar(20) NOT NULL,    -- 'ADVANCE', 'RECOVERY', 'HOLIDAY_PAY'
    [amount] decimal(18,4) NOT NULL,         -- Số tiền
    [currency] nvarchar(10) NOT NULL,        -- 'LAK', 'THB'
    [payment_date] date NULL,                -- Ngày chi tiền mặt
    [reason] nvarchar(500) NULL,             -- Lý do (VD: Tiền công ngày lễ 01/01)
    [is_deducted] bit NOT NULL DEFAULT 0,    -- Đã trừ vào lương chưa
    [deducted_in_month] nvarchar(7) NULL,    -- Trừ vào tháng nào
    [approved_by] int NULL,
    [approved_at] datetime NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_advance_payment] PRIMARY KEY ([id]),
    CONSTRAINT [FK_advance_employee] FOREIGN KEY ([employee_id]) REFERENCES [employee] ([id])
);

CREATE INDEX [IX_advance_emp_month] ON [advance_payment] ([employee_id], [year_month]);
```

---

### 2.8 Bảng `technical_evaluation` - Đánh giá hạng kỹ thuật

```sql
CREATE TABLE [technical_evaluation] (
    [id] int NOT NULL IDENTITY,
    [employee_id] int NOT NULL,              -- FK → employee
    [year_month] nvarchar(7) NOT NULL,       -- '2025-11'
    [evaluated_grade] nvarchar(5) NOT NULL,  -- Hạng đánh giá: A/B/C/D
    [evaluation_score] decimal(5,2) NULL,    -- Điểm đánh giá
    [evaluated_by] int NOT NULL,             -- QLKT đánh giá
    [evaluated_at] datetime NOT NULL,
    [is_reviewed] bit NOT NULL DEFAULT 0,    -- Đã phúc tra chưa
    [reviewed_grade] nvarchar(5) NULL,       -- Hạng sau phúc tra
    [reviewed_score] decimal(5,2) NULL,
    [reviewed_by] int NULL,
    [reviewed_at] datetime NULL,
    [final_grade] nvarchar(5) NOT NULL,      -- Hạng cuối cùng (= reviewed nếu có, else evaluated)
    [note] nvarchar(1000) NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    [updated_at] datetime NULL,
    [updated_by] int NULL,
    CONSTRAINT [PK_technical_evaluation] PRIMARY KEY ([id]),
    CONSTRAINT [FK_tech_eval_employee] FOREIGN KEY ([employee_id]) REFERENCES [employee] ([id])
);

CREATE UNIQUE INDEX [IX_tech_eval_emp_month] 
    ON [technical_evaluation] ([employee_id], [year_month]) WHERE [status] != -1;
```

---

### 2.9 Bảng `employee_code_rule` - Quy tắc mã lao động

```sql
CREATE TABLE [employee_code_rule] (
    [id] int NOT NULL IDENTITY,
    [year] int NOT NULL,                     -- 2026
    [prefix] nvarchar(20) NOT NULL,          -- 'NV-2026-'
    [digit_count] int NOT NULL DEFAULT 4,    -- Số chữ số: 0001
    [current_sequence] int NOT NULL DEFAULT 0, -- Số hiện tại
    [description] nvarchar(500) NULL,
    [status] smallint NOT NULL DEFAULT 1,
    [created_at] datetime NOT NULL,
    [created_by] int NOT NULL,
    CONSTRAINT [PK_employee_code_rule] PRIMARY KEY ([id])
);

CREATE UNIQUE INDEX [IX_emp_code_rule_year] ON [employee_code_rule] ([year]) WHERE [status] != -1;
```

---

### 2.10 Cập nhật bảng `employee` - Thêm trường

```sql
ALTER TABLE [employee] ADD
    [employee_type_id] int NULL,             -- FK → employee_type
    [is_difficult_area] bit NOT NULL DEFAULT 0, -- Vùng khó khăn
    [has_insurance] bit NOT NULL DEFAULT 0,  -- Có BH riêng (CN Lào)
    [nationality] nvarchar(20) NULL,         -- 'VN', 'LA'
    [insurance_note] nvarchar(500) NULL;     -- Ghi chú BH đặc biệt

ALTER TABLE [employee] ADD 
    CONSTRAINT [FK_employee_type] FOREIGN KEY ([employee_type_id]) 
    REFERENCES [employee_type] ([id]);
```

---

## 3. CÔNG THỨC TÍNH LƯƠNG CNKT (MỚI)

```
BƯỚC 1: Lấy DRC chốt cuối tháng
   DRC = SELECT drc_value FROM drc_rate 
         WHERE tram_id=? AND year_month=? AND status=1

BƯỚC 2: Tính quy khô (giữ nguyên số thập phân)
   DryLatexKg = RawLatexKg × DRC
              + RopeLatexKg × DRC  
              + SerumKg × DRC_Serum

BƯỚC 3: Lấy hạng kỹ thuật cuối cùng
   FinalGrade = SELECT final_grade FROM technical_evaluation 
                WHERE employee_id=? AND year_month=?

BƯỚC 4: Lấy đơn giá theo hạng (Bath/kg)
   UnitPrice = SELECT unit_price + ISNULL(difficult_area_bonus, 0)
               FROM rubber_unit_price 
               WHERE tram_id=? AND grade=? AND effective_date <= ?
               ORDER BY effective_date DESC

BƯỚC 5: Tính thành tiền mủ (Bath, làm tròn không thập phân)
   RubberSalary_THB = FLOOR(DryLatexKg × UnitPrice)

BƯỚC 6: Quy ra Kíp
   ExchangeRate = SELECT rate FROM exchange_rate 
                  WHERE year_month=? AND from_currency='THB' AND to_currency='LAK'
   RubberSalary_LAK = RubberSalary_THB × ExchangeRate

BƯỚC 7: Tính các khoản khác (Kíp)
   CareSalary = CareDays × 25000
   HardshipBonus = HardshipDays × WorkType.unit_price
   SundayBonus = SundayDays × RegularDayPay × 0.67  -- (thêm 67%)
   
BƯỚC 8: Trừ tạm ứng
   Advances = SELECT SUM(amount) FROM advance_payment 
              WHERE employee_id=? AND year_month=? AND is_deducted=0

BƯỚC 9: Tính lương NET (Kíp)
   GrossSalary = RubberSalary_LAK + CareSalary + HardshipBonus + SundayBonus + Allowances
   NetSalary = GrossSalary - Advances
   
   -- Cập nhật tạm ứng đã trừ
   UPDATE advance_payment SET is_deducted=1, deducted_in_month=? WHERE ...
```

---

## 4. TỔNG HỢP TẤT CẢ BẢNG

### Bảng hiện có (cập nhật)

| Bảng | Thay đổi |
|------|----------|
| `employee` | Thêm `employee_type_id`, `is_difficult_area`, `has_insurance`, `nationality` |
| `attendance` | Giữ nguyên |
| `production` | Giữ nguyên |
| `drc_rate` | Giữ nguyên |
| `payroll` | Cập nhật logic |

### Bảng mới (cần tạo)

| # | Bảng | Mục đích |
|---|------|----------|
| 1 | `employee_type` | Loại lao động (CNKT, BV, TV, CB, CS) |
| 2 | `technical_grade` | Hạng kỹ thuật A/B/C/D + hệ số điểm |
| 3 | `rubber_unit_price` | Đơn giá mủ theo hạng/trạm (Bath) |
| 4 | `exchange_rate` | Tỷ giá hàng tháng (Vietinbank) |
| 5 | `work_type` | Loại công & đơn giá (chăm sóc, CN...) |
| 6 | `holiday` | Ngày lễ VN + Lào |
| 7 | `advance_payment` | Tạm ứng & truy thu |
| 8 | `technical_evaluation` | Đánh giá hạng KT + phúc tra |
| 9 | `employee_code_rule` | Quy tắc mã NV tự động |

---

## 5. CÂU HỎI CẦN LÀM RÕ VỚI KHÁCH HÀNG

### Đã có file:
- [x] File Excel lương tháng 11/2025

### Cần bổ sung:
1. [ ] **File quy trình kỹ thuật** - Chấm điểm CNKT xếp hạng
2. [ ] **File xác nhận tỷ giá** - Mẫu từ P.Tổ chức
3. [ ] **File bổ công & định mức chăm sóc** - Đơn giá, quy trình
4. [ ] **File quy định ngày lễ** - Danh sách + phụ cấp
5. [ ] **Quy tắc mã lao động 2026** - Format, prefix

### Câu hỏi:
1. **Đơn giá mủ** theo hạng A/B/C/D hiện tại là bao nhiêu Bath/kg?
2. **Vùng khó khăn** hỗ trợ thêm bao nhiêu Bath/kg?
3. **Tỷ giá** tháng 11/2025 là bao nhiêu?
4. **Hệ số công CN** là gấp bao nhiêu lần (1.5? 1.67? 2.0)?
5. **Lương BV, TV** cố định bao nhiêu/tháng?

---

## 6. NEXT STEPS

Sau khi khách hàng xác nhận:

1. [ ] Tạo Entities cho các bảng mới
2. [ ] Migration database
3. [ ] Seed data mặc định
4. [ ] Tạo Services + Controllers
5. [ ] Tạo Frontend pages quản lý:
   - Loại lao động
   - Hạng kỹ thuật & đơn giá mủ
   - Tỷ giá hàng tháng
   - Ngày lễ
   - Tạm ứng/Truy thu
   - Đánh giá hạng KT
6. [ ] Cập nhật logic tính lương S_Payroll
7. [ ] Test với dữ liệu tháng 11/2025
