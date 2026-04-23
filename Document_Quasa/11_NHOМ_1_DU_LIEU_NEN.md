# 📋 NHÓM 1: DỮ LIỆU NỀN (Master Data Layer)
## Tài Liệu Chi Tiết - ECOTECH 2A Module

---

## 1. TỔNG QUAN

**Nhóm 1** là lớp dữ liệu cơ bản, nền tảng cho toàn bộ hệ thống tính lương ECOTECH 2A.

⚠️ **CONTEXT: ECOTECH 2A - Công Ty Khai Thác Cao Su ở Lào**
- Tiền tệ: **Bath (THB)** → Quy ra **Kíp (LAK)** theo tỷ giá hàng tháng
- Hạng kỹ thuật: **A, B, C, D** (đơn giá riêng từng hạng)
- Quy khô (DRC): Tính từ khối lượng tươi × hệ số
- Công nhân: Trực tiếp (Kíp) + Gián tiếp (Đô, quy sang Kíp báo cáo)
- Bảo hiểm: Chỉ cho cán bộ, không cho công nhân kỹ thuật
- Mã lao động: Quy định 2026 (cài đặt tự động)

```
Nhóm 1: Dữ Liệu Nền (EcoTech 2A)
├─ Hồ sơ lao động (Employee Master - Mã 2026)
├─ Cài đơn giá (Salary Configuration - Bath & Kíp)
├─ Hạng kỹ thuật (Technical Grade - A/B/C/D)
├─ Hệ số hỗ trợ (Support Zone Coefficient)
├─ Tỷ giá hàng tháng (Monthly Exchange Rate)
├─ Quản lý ngày lễ (Holiday Calendar)
└─ Tiêu chuẩn QTKT (Accounting Standards)

Đặc Điểm:
├─ Dữ liệu tĩnh (ít thay đổi, hoặc 1 năm 1 lần)
├─ Tham chiếu bởi Nhóm 2 & 3
├─ Cần bảo mật cao
├─ Tiền tệ kép (Bath + Kíp)
└─ Cần audit trail (ai, khi nào, thay đổi gì)
```

---

## 2. MODULE: HỒ SƠ LAO ĐỘNG

### 2.1 Định Nghĩa

**Mục Đích:** Quản lý thông tin nhân viên - dữ liệu cơ bản để tính lương

**Dữ Liệu Lưu Trữ:**
```
Mã NV, Tên, Chức vụ, Bộ phận, Ngày vào, Trạm làm (1/2), Trạng thái
```

**Database Table:**
```sql
CREATE TABLE employees (
  msnv VARCHAR(20) PRIMARY KEY,  -- Format: 2026-XXX (Quy định 2026 từ PTO)
  employee_code VARCHAR(20),      -- Mã nhân viên   name VARCHAR(100) NOT NULL,
  position VARCHAR(50),           -- VD: CNKT, VP, Bảo vệ, Tạp vụ
  department VARCHAR(50),         -- Đội, Phòng, v.v
  employee_type ENUM('DIRECT', 'INDIRECT'),  -- Trực tiếp (Kíp) vs gián tiếp (Đô)
  technical_grade CHAR(1),        -- A/B/C/D (chỉ cho CNKT)
  difficult_zone BOOLEAN DEFAULT 0,  -- Vùng khó khăn (hỗ trợ thêm)
  insurance_included BOOLEAN DEFAULT 0,  -- Bảo hiểm (chỉ cán bộ)
  hire_date DATE,
  active BOOLEAN DEFAULT 1,
  bank_account VARCHAR(30),       -- Tài khoản ngân hàng
  created_date TIMESTAMP,
  updated_date TIMESTAMP
);
```

### 2.2 Quy Trình Nhập Liệu

```
NHẬP HỒ SƠ LAO ĐỘNG
═══════════════════════════════════════════════════════════

Step 1: Chuẩn Bị Dữ Liệu
├─ Import từ Excel/CSV: MSNV 2025 (từ phân tích Excel)
├─ Định dạng: 1 dòng = 1 nhân viên
└─ Fields: MSNV | Tên | Chức Vụ | Bộ Phận | Trạm | Ngày Vào

Step 2: Validation
├─ MSNV không được trùng
├─ MSNV định dạng: số nguyên 3-6 ký tự
├─ Tên không để trống
├─ Trạm chỉ là 1 hoặc 2
└─ Ngày vào < ngày hiện tại

Step 3: Lưu Trữ
├─ Lưu vào bảng employees
├─ Tạo audit log ghi lại (user, thời gian, action)
└─ Set active = 1

Step 4: Verify
└─ Kiểm tra: 100% dòng nhập được lưu, không mất dữ liệu
```

### 2.3 Quy Tắc Kinh Doanh (ECOTECH 2A)

```
BUSINESS RULES:

1. MÃ LAO ĐỘNG 2026: Format YYYY-NNN (VD: 2026-001)
   ├─ Phòng Tổ chức (PTO) phát hành & cài đặt tự động
   ├─ Quy định năm 2026
   └─ Một MSNV/Năm chỉ tồn tại 1 lần

2. LOẠI CÔNG NHÂN: DIRECT (Kíp) vs INDIRECT (Đô)
   ├─ DIRECT (100%): Công nhân kỹ thuật tại Lào
      ├─ Tính lương bằng Kíp
      ├─ KHÔNG tham gia bảo hiểm
      └─ Bao gồm: Phụ cấp cơ bản (không có BHXH/BHYT)
   └─ INDIRECT: Cán bộ VP
      ├─ Tính lương bằng Đô, quy sang Kíp để báo cáo
      ├─ Tham gia bảo hiểm
      └─ Có quyền lợi xã hội

3. HẠNG KỸ THUẬT: A/B/C/D (chỉ cho CNKT tại Lào)
   ├─ Mỗi hạng có đơn giá riêng
   ├─ QLKT đánh giá 1 lần/tháng (cuối tháng)
   ├─ Phúc tra lại: Lấy số liệu đánh giá cuối cùng
   └─ Đánh giá: Chỉ quần thảo các phần cây ngẫu nhiên

4. VÙNG KHÓ KHĂN: Support thêm đơn giá
   ├─ Cấu hình: difficult_zone = 1
   ├─ Hỗ trợ: Base_salary × coefficient_support
   └─ Xác định bởi Phòng kỹ thuật

5. Nhân viên có thể thay đổi bộ phận/hạng
   └─ Record history (ngày thay đổi, từ hạng nào → hạng nào)

6. Khi nhân viên thôi việc
   └─ Set active = 0 (soft delete), không xóa dữ liệu
   └─ Lương tính riêng (tính đến ngày thôi việc)
```

### 2.4 Ví Dụ Dữ Liệu

| MSNV | Tên | Chức Vụ | Bộ Phận | Trạm | Ngày Vào | Active |
|---|---|---|---|---|---|---|
| 001 | Nguyễn Văn A | Công Nhân | Sản Xuất | 1 | 2020-01-15 | 1 |
| 002 | Trần Thị B | Trưởng Nhóm | Quản Lý | 2 | 2019-06-01 | 1 |
| 003 | Lê Văn C | Kỹ Sư | Kỹ Thuật | 1 | 2021-03-10 | 1 |

### 2.5 Test Cases

```python
class TestEmployeesMasterData:
    
    def test_import_employees_valid(self):
        """Test: Import nhân viên hợp lệ"""
        employees = [
            {'msnv': 1, 'name': 'Nguyễn A', 'tram_id': 1},
            {'msnv': 2, 'name': 'Trần B', 'tram_id': 2},
        ]
        result = import_employees(employees)
        assert result['success'] == True
        assert result['total'] == 2
        assert result['errors'] == 0

    def test_duplicate_msnv_rejected(self):
        """Test: Reject MSNV trùng"""
        duplicates = [
            {'msnv': 1, 'name': 'Nguyễn A'},
            {'msnv': 1, 'name': 'Trần B'},  # Duplicate
        ]
        result = import_employees(duplicates)
        assert result['error'] == 'DUPLICATE_MSNV'

    def test_soft_delete_on_termination(self):
        """Test: Thôi việc không xóa, chỉ set active=0"""
        terminate_employee(msnv=1)
        emp = get_employee(1)
        assert emp['active'] == 0
        assert emp['msnv'] == 1  # Still exists in DB
        
    def test_salary_calculation_for_terminated(self):
        """Test: Lương tính có cắt ngắn không?"""
        emp = terminate_employee(msnv=1, termination_date='2025-06-15')
        payroll = calculate_payroll(msnv=1, month='06/2025')
        # Kiểm tra: chỉ tính từ ngày 1-15/6, không phải cả tháng
```

---

## 3. MODULE: CÀI ĐƠN GIÁ (Salary Scale)

### 3.1 Định Nghĩa

**Mục Đích:** Bảng lương cơ bản theo cấp bậc & trạm làm

**Dữ Liệu Lưu Trữ:**
```
Cấp Bậc (A/B/C/D/E), Trạm (1/2), Lương Cơ Bản, Hệ Số Điều Chỉnh
```

**Database Table:**
```sql
CREATE TABLE salary_scale (
  scale_id INT AUTO_INCREMENT PRIMARY KEY,
  grade CHAR(1) (A/B/C/D/E),
  tram_id INT (1 or 2),
  base_salary DECIMAL(12,2),
  coefficient DECIMAL(5,2),
  effective_date DATE,
  description VARCHAR(255)
);

-- Data bao gồm:
-- TRAM1: Grade A=9.2, B=8.9, C=8.6, D=?, E=?
-- TRAM2: Grade A=7.7, B=7.4, C=7.1, D=?, E=?
```

### 3.2 Quy Trình Cơi Đấu Giá

```
CÀI ĐƠN GIÁ LƯƠNG
═══════════════════════════════════════════════════════════

Step 1: Chuẩn Bị Bảng Lương
├─ Lấy từ sheet "CÁCH TÍNH LƯƠNG"
├─ Định dạng: Grade | Trạm | Lương | Hệ Số
└─ Hiệu lực từ: Tháng mấy?

Step 2: Validation
├─ Grade phải là A-E
├─ Trạm phải là 1 hoặc 2
├─ Lương > 0
├─ Hệ số > 0
└─ Không duplicate (grade + tram + effective_date)

Step 3: Lưu Trữ
├─ Lưu vào salary_scale table
├─ Lưu effective_date để có thể lịch sử
└─ Audit log: who, when, what changed

Step 4: Sử Dụng
└─ Khi tính lương, VLOOKUP từ bảng này
```

### 3.3 Ví Dụ Dữ Liệu

| Scale_ID | Grade | Trạm | Base_Salary | Coefficient | Effective_Date |
|---|---|---|---|---|---|
| 1 | A | 1 | 25,000,000 | 9.2 | 2025-01-01 |
| 2 | B | 1 | 22,000,000 | 8.9 | 2025-01-01 |
| 3 | C | 1 | 20,000,000 | 8.6 | 2025-01-01 |
| 4 | A | 2 | 19,000,000 | 7.7 | 2025-01-01 |
| 5 | B | 2 | 17,000,000 | 7.4 | 2025-01-01 |

### 3.4 Test Cases

```python
class TestSalaryScale:
    
    def test_lookup_correct_salary(self):
        """Test: Tìm lương đúng theo grade & trạm"""
        salary = lookup_salary(grade='A', tram=1)
        assert salary == 25_000_000

    def test_invalid_grade_raises_error(self):
        """Test: Grade không hợp lệ → error"""
        with pytest.raises(InvalidGradeError):
            lookup_salary(grade='Z', tram=1)

    def test_historical_salary_lookup(self):
        """Test: Lương theo tháng (khi có adjustment)"""
        # Tháng 1-5: Lương A=25M
        # Tháng 6: Tăng lên A=26M
        salary_may = lookup_salary_for_month('2025-05', 'A', 1)
        salary_jun = lookup_salary_for_month('2025-06', 'A', 1)
        assert salary_may == 25_000_000
        assert salary_jun == 26_000_000
```

---

## 4. MODULE: CÀI HỆ SỐ VƯỜN CÂY (System Parameters)

### 4.1 Định Nghĩa

**Mục Đích:** Cầu hình các tham số hệ thống, hệ số điều chỉnh

**Tham Số Lưu Trữ:**

```
P7 = 27.0                    (Tham số khoán)
HE_SO_KHOАН = 292.59         (Hệ số cơ bản)
COEF_TRAM1_A = 9.2           (Hệ số TRAM1 cấp A)
COEF_TRAM1_B = 8.9           (Hệ số TRAM1 cấp B)
COEF_TRAM1_C = 8.6           (Hệ số TRAM1 cấp C)
COEF_TRAM2_A = 7.7           (Hệ số TRAM2 cấp A)
COEF_TRAM2_B = 7.4           (Hệ số TRAM2 cấp B)
COEF_TRAM2_C = 7.1           (Hệ số TRAM2 cấp C)
BHXH_RATE = 0.08             (Tỷ lệ BHXH 8%)
BHYT_RATE = 0.015            (Tỷ lệ BHYT 1.5%)
TAX_THRESHOLDS = [...]        (Ngưỡng thuế 6 bậc)
TAX_RATES = [...]             (Tỷ lệ thuế 6 bậc)
```

**Database Table:**
```sql
CREATE TABLE system_parameters (
  param_id INT AUTO_INCREMENT PRIMARY KEY,
  param_name VARCHAR(100) UNIQUE,
  param_value VARCHAR(255),
  data_type ENUM('INT', 'DECIMAL', 'STRING', 'JSON'),
  description VARCHAR(255),
  version INT,
  active BOOLEAN DEFAULT 1,
  effective_date DATE,
  last_modified TIMESTAMP,
  modified_by INT
);
```

### 4.2 Quy Trình Cấu Hình

```
CÀI ĐẠT HỆ SỐ VƯỜN CÂY
═══════════════════════════════════════════════════════════

Step 1: Xác Định Tham Số
├─ Từ phân tích Excel → SRS
├─ P7 = 27.0 (mục đích: cần làm rõ)
├─ Hệ số TRAM1/TRAM2
├─ Tỷ lệ BHXH, BHYT
└─ Ngưỡng & tỷ lệ thuế

Step 2: Lưu Trữ
├─ Param_name = tên tham số (unique)
├─ Param_value = giá trị
├─ Data_type = kiểu (INT, DECIMAL, STRING)
├─ Effective_date = khi nào sử dụng
└─ Version = log thay đổi

Step 3: Validation
├─ Tham số bắt buộc (P7, hệ số, thuế) phải có
├─ Tỷ lệ thuế: tổng 6 bậc phải hợp lý
├─ Hệ số > 0
└─ BHXH + BHYT <= 15%

Step 4: Versioning
├─ Khi thay đổi, tạo version mới (version++)
├─ Keep history (để trace lại)
└─ Audit log: who changed, when, from what to what
```

### 4.3 Ví Dụ Dữ Liệu

| Param_ID | Param_Name | Param_Value | Data_Type | Effective_Date |
|---|---|---|---|---|
| 1 | P7_ADJUSTMENT | 27.0 | DECIMAL | 2025-01-01 |
| 2 | COEF_TRAM1_A | 9.2 | DECIMAL | 2025-01-01 |
| 3 | COEF_TRAM1_B | 8.9 | DECIMAL | 2025-01-01 |
| 4 | BHXH_RATE | 0.08 | DECIMAL | 2025-01-01 |
| 5 | BHYT_RATE | 0.015 | DECIMAL | 2025-01-01 |
| 6 | TAX_THRESHOLDS | [5M,10M,20M,25M,65M] | JSON | 2025-01-01 |
| 7 | TAX_RATES | [0.05,0.10,0.15,0.20,0.25,0.35] | JSON | 2025-01-01 |

### 4.4 Test Cases

```python
class TestSystemParameters:
    
    def test_get_parameter_value(self):
        """Test: Lấy giá trị tham số"""
        p7 = get_parameter('P7_ADJUSTMENT')
        assert p7 == 27.0
        
    def test_update_parameter_with_version(self):
        """Test: Cập nhật tham số, tạo version mới"""
        old_version = get_parameter_version('P7_ADJUSTMENT')
        update_parameter('P7_ADJUSTMENT', 27.5)
        new_version = get_parameter_version('P7_ADJUSTMENT')
        assert new_version > old_version

    def test_effective_date_parameter(self):
        """Test: Tham số có hiệu lực từ ngày nào"""
        # Tháng 1-5: P7=27.0
        # Tháng 6+: P7=27.5
        p7_may = get_parameter_for_date('P7_ADJUSTMENT', '2025-05-01')
        p7_jun = get_parameter_for_date('P7_ADJUSTMENT', '2025-06-01')
        assert p7_may == 27.0
        assert p7_jun == 27.5

    def test_tax_thresholds_validation(self):
        """Test: Ngưỡng thuế phải tăng dần"""
        thresholds = get_parameter('TAX_THRESHOLDS')
        for i in range(len(thresholds)-1):
            assert thresholds[i] < thresholds[i+1]
```

---

## 5. MODULE: QUẢN LÝ NGÀY LỄ (Holiday Calendar) [NEW]

### 5.1 Định Nghĩa

**Mục Đích:** Lưu danh sách ngày lễ, ngày nghỉ để ảnh hưởng đến tính lương

**Dữ Liệu Lưu Trữ:**
```
Ngày Lễ, Tên Lễ, Loại (Chính Thức/Đặc Biệt/Bổ Trợ), Năm
```

**Database Table:**
```sql
CREATE TABLE holiday_calendar (
  holiday_id INT AUTO_INCREMENT PRIMARY KEY,
  holiday_date DATE,
  holiday_name VARCHAR(100),
  holiday_type ENUM('OFFICIAL', 'SPECIAL', 'MAKE_UP'),
  year INT,
  hours_off INT DEFAULT 8 (số giờ nghỉ),
  description VARCHAR(255)
);
```

### 5.2 Quy Trình Quản Lý

```
QUẢN LÝ NGÀY LỄ
═══════════════════════════════════════════════════════════

Step 1: Định Nghĩa Loại
├─ OFFICIAL: Ngày lễ chính thức (1+, ĐH, Tết, v.v)
├─ SPECIAL: Ngày học tập, hội thảo
└─ MAKE_UP: Ngày bù (để bổ sung ngày làm việc)

Step 2: Nhập Dữ Liệu
├─ Mỗi năm: danh sách ~20 ngày lễ
├─ Format: Ngày|Tên|Loại|Năm
└─ Validation: Ngày hợp lệ, không trùng

Step 3: Ảnh Hưởng Đến Tính Lương
├─ Ngày lễ chính thức: có tính lương (100%) hay không?
├─ Ngày bù: tính vào "số công" không?
└─ Nếu làm vào ngày lễ: tính 1.5x hay 2x?

Step 4: Sử Dụng
└─ Khi tính lương: check ngày công có trùng ngày lễ không?
```

### 5.3 Ví Dụ Dữ Liệu

| Holiday_Date | Holiday_Name | Holiday_Type | Year |
|---|---|---|---|
| 2025-01-01 | Ngày Thơm Đầu Năm | OFFICIAL | 2025 |
| 2025-02-01 | Tết Nguyên Đán | OFFICIAL | 2025 |
| 2025-04-30 | Ngày Giải Phóng | OFFICIAL | 2025 |
| 2025-09-02 | Ngày Quốc Khánh | OFFICIAL | 2025 |
| 2025-03-15 | Hội Thảo Công Ty | SPECIAL | 2025 |

---

## 6. MODULE: TIÊU CHUẨN QTKT (Accounting Standards) [NEW]

### 6.1 Định Nghĩa

**Mục Đích:** Định nghĩa tiêu chuẩn kế toán, mã khoản mục

**Dữ Liệu Lưu Trữ:**
```
Tên Tiêu Chuẩn, Mã Kế Toán, Bộ Phận, Mô Tả
```

**Database Table:**
```sql
CREATE TABLE accounting_standards (
  std_id INT AUTO_INCREMENT PRIMARY KEY,
  standard_name VARCHAR(100),
  accounting_code VARCHAR(20), -- VD: 5210, 5220
  department VARCHAR(50),
  cost_center VARCHAR(50),
  description VARCHAR(255)
);
```

### 6.2 Ví Dụ Dữ Liệu

| Std_Name | Accounting_Code | Department | Description |
|---|---|---|---|
| Chi Phí Lương Sản Xuất | 5210 | Sản Xuất | Lương công nhân |
| Chi Phí Lương Quản Lý | 5220 | Quản Lý | Lương quản lý |
| Chi Phí Lương Kinh Doanh | 5230 | Kinh Doanh | Lương bán hàng |
| Chi Phí Lương Hành Chính | 5240 | Hành Chính | Lương hành chính |
| Chi Phí BHXH | 3100 | Tất Cả | Bảo hiểm xã hội |

---

## 7. MODULE: TỶ GIÁ HÀNG THÁNG (Monthly Exchange Rate) [ECOTECH]

**Định Nghĩa:** Tỷ giá Bath (THB) → Kíp (LAK) hàng tháng 
- Nguồn: Vietinbank
- Xác nhận bởi: Phòng Tổ Chức
- Cập nhật: Đầu tháng hoặc khi có thay đổi

**Database Table:**
```sql
CREATE TABLE exchange_rates (
  rate_id INT AUTO_INCREMENT PRIMARY KEY,
  year_month VARCHAR(7),                -- VD: 2026-04
  exchange_rate DECIMAL(10,4),         -- 1 Bath = ? Kíp
  source VARCHAR(50),                  -- Vietinbank
  confirmed_by VARCHAR(50),            -- P Tổ chức person
  confirmed_date DATE,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

**Quy Trình:**
1. P Kế Toán: Lấy tỷ giá từ Vietinbank
2. P Tổ Chức: Xác nhận & công bố tỷ giá chính thức
3. Update vào table exchange_rates
4. Phase 3 tính lương: Sử dụng rate để quy Bath → Kíp

**Ví Dụ:**
| Year-Month | Exchange Rate | Source | Confirmed |
|---|---|---|---|
| 2026-04 | 103.50 | Vietinbank | 01/04/2026 |
| 2026-05 | 104.20 | Vietinbank | 01/05/2026 |

---

## 8. MODULE: QUY KHÔ (DRC - Dry Rubber Content) [ECOTECH]

**Định Nghĩa:** Tính khối lượng cao su khô từ khối lượng tươi × Hệ số DRC

**Công Thức:**
```
Quy_Khô (kg) = Khối_Lượng_Tươi (kg) × DRC_Coefficient
              → GIỮ NGUYÊN thập phân (VD: 150.00 kg)

Thành_Tiền (Kíp) = Quy_Khô × Đơn_Giá (Hạng A/B/C/D)
                 → LÀMtròn (không lấy 0.00)
```

**Database Table:**
```sql
CREATE TABLE rubber_drc (
  drc_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv VARCHAR(20),
  year_month VARCHAR(7),
  fresh_weight DECIMAL(12,2),          -- kg tươi (input)
  drc_coefficient DECIMAL(5,4),        -- VD: 0.30
  dry_weight DECIMAL(12,4),            -- = fresh × coef (GIỮ 4 thập phân)
  technical_grade CHAR(1),             -- A/B/C/D (from employees)
  unit_price DECIMAL(12,0),            -- Kíp/kg
  total_amount DECIMAL(14,0),          -- = dry × unit (LÀMtròn)
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

**Ví Dụ:**
```
CNKT 2026-001 (Hạng A):
├─ Khối Tươi:    500.00 kg (input)
├─ DRC:          0.30
├─ Quy Khô:      = 500 × 0.30 = 150.0000 kg (GIỮ)
├─ Đơn Giá:      1,500,000 Kíp/kg (hạng A)
├─ Thành Tiền:   = 150 × 1,500,000 = 225,000,000 Kíp (LÀMtròn)
└─ Lương CNKT:   225,000,000 Kíp (dùng cho tính lương)
```

**Quy Trình:**
1. Trạm cán + Đội: Chốt khối lượng tươi, DRC hàng ngày
2. Cuối tháng: Tổng hợp DRC, xác nhận hạng kỹ thuật (A-D)
3. Phase 3: Lookup DRC → Tính thành tiền → Lương CNKT

---

## 9. MODULE: HỆ SỐ HỖ TRỢ VÙNG KHÓ KHĂN [ECOTECH]

**Định Nghĩa:** Hệ số tăng thêm lương cho công nhân ở vùng khó khăn

**Database Table:**
```sql
CREATE TABLE zone_support (
  zone_id INT AUTO_INCREMENT PRIMARY KEY,
  zone_code VARCHAR(20),
  zone_name VARCHAR(100),
  support_coefficient DECIMAL(5,2),    -- VD: 1.15 (tăng 15%)
  description VARCHAR(255),
  effective_date DATE
);
```

**Công Thức:**
```
Lương_Vùng_Khó_Khăn = Lương_Base × support_coefficient

Ví Dụ:
├─ Vùng bình thường: support_coefficient = 1.00
├─ Vùng khó khăn:    support_coefficient = 1.15 (tăng 15%)
└─ Lương có hỗ trợ:  Lương × 1.15
```

**Ví Dụ Data:**
| Zone_Code | Zone_Name | Support_Coefficient |
|---|---|---|
| Z01 | Bình Thường | 1.00 |
| Z02 | Vùng Cao | 1.15 |
| Z03 | Vùng Biên Giới | 1.25 |

**Cách Sử Dụng:**
1. HR/PTO: Gán `difficult_zone = 1` cho CN ở vùng khó khăn
2. Phase 3: Lookup zone_support → Áp dụng coefficient
3. Lương cuối = Lương × coefficient

**Employee Integration:**
```sql
-- Ở bảng employees:
ALTER TABLE employees ADD difficult_zone BOOLEAN DEFAULT 0;
-- Nếu = 1 → Áp dụng hệ số hỗ trợ
```

---

## 10. MODULE: NGÀY LỄ & PHỤ CẤP NỘI BỘ [ECOTECH]

**A. Ngày Lễ & Chính Sách Tạm Ứng**

**Quy Trình:**
```
Ngày Lễ CN Đi Làm:
├─ Trả tiền mặt ngay: (do Phòng Tổ chức quy định số tiền)
├─ Ghi nhận: "Tạm ứng ngày lễ"
└─ Status: Chờ cuối tháng reconcile

Cuối Tháng:
├─ Ghi nhận: +Số tiền tạm ứng (vào mục "Advances")
├─ Tính lương bình thường: Lương_Base
├─ Trừ tiền tạm ứng: -Số tiền tăm ứng
└─ Lương Net: Lương - Tạm Ứng
```

**Database:**
```sql
-- Thêm bảng holiday_advance để ghi nhận
CREATE TABLE holiday_advances (
  hld_adv_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv VARCHAR(20),
  year_month VARCHAR(7),
  holiday_date DATE,
  advance_amount DECIMAL(12,0),        -- Kíp
  status ENUM('pending','settled'),
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

**B. Phụ Cấp Cố Định Hàng Tháng**

**Danh Sách Phụ Cấp (Đợi từ EcoTech):**
| Allowance_Code | Allowance_Name | BaseAmount_Kip | Effective_From |
|---|---|---|---|
| (Chờ P Tổ chức cung cấp) | ... | ... | ... |

**Cập Nhật:** 
- Chị Phượng sẽ gửi file "Quy Định Phụ Cấp Công Chăm Sóc 2026"
- Dùng file đó update bảng allowances trong hệ thống

---

## 11. SỬ DỤNG NHÓM 1 TRONG QUY TRÌNH TÍNH LƯƠNG

```
NHÓM 1: DỮ LIỆU NỀN
        ↓
    THAM CHIẾU BỞI
        ↓
NHÓM 2: PHÁT SINH (Chấm công, Performance, v.v)
        ↓
    KẾT HỢP VỚI
        ↓
NHÓM 3: XỬ LÝ (Tính lương bằng 5 phases)
        ↓
    SINH RA
        ↓
NHÓM 4: BÁO CÁO (Báo cáo lương)


Ví Dụ Cụ Thể:
═════════════════════════════════════════════════

1. Lookup nhân viên (MN 001)
   → employees TB: Tìm được Nguyễn Văn A, Trạm 1

2. Lookup lương cơ bản (Cấp A, Trạm 1)
   → salary_scale TB: Lương = 25M, Hệ số = 9.2

3. Lookup hệ số điều chỉnh
   → system_parameters TB: P7 = 27.0, BHXH = 8%

4. Lấy chấm công tháng 12/2025
   → attendance TB: Công = 27 ngày

5. Lấy hiệu suất & chất lượng
   → performance TB: Sản lượng = 1000, Cấp = A

6. Vào Nhóm 3: Tính lương
   → Phase 1: 1000 × 9.2 = 9200
   → Phase 2: 25M (lookup)
   → Phase 3: (25M + 27) / 27 × 1.0 = ...
   → Phase 4: Tính thuế, BHXH
   → Phase 5: Lương net + Phân bổ

7. Output: Lương cuối = 21,500,000 VND
```

---

## 12. CHECKLIST NHÓM 1: DỮ LIỆU NỀN [ECOTECH 2A]

**Chuẩn Bị Nhóm 1:**

- [ ] **Hồ Sơ Lao Động (Employees)** [MODULE 1]
  - [ ] Format mã 2026-NNN (từ P Tổ chức)
  - [ ] Loại công nhân: DIRECT vs INDIRECT
  - [ ] Hạng kỹ thuật: A/B/C/D (cho CNKT)
  - [ ] Vùng khó khăn: Mark những CN ở vùng support
  - [ ] Bảo hiểm: Chỉ cán bộ = true, CNKT = false
  - [ ] Validate: Không trùng MSNV, Loại/Hạng hợp lệ
  
- [ ] **Cài Đơn Giá Lương (Salary Scale)** [MODULE 2]
  - [ ] Đơn giá tính **Bath (THB)** cho CNKT
  - [ ] 4 hạng: A, B, C, D (mỗi hạng đơn giá riêng)
  - [ ] Đơn giá khác tính **Kíp (LAK)** (chăm sóc, phụ cấp)
  - [ ] Effective_date rõ ràng
  - [ ] Test: 100% import thành công
  
- [ ] **Hệ Số Vườn Cây (System Parameters)**
  - [ ] Hệ số theo hạng (A/B/C/D)
  - [ ] Hệ số TRAM (1 hoặc 2)
  - [ ] BHXH: 8% (chỉ INDIRECT)
  - [ ] BHYT: 1.5% (chỉ INDIRECT)
  - [ ] Thuế: 6 bậc với ngưỡng & tỷ lệ
  
- [ ] **Quản Lý Ngày Lễ (Holiday Calendar)** [MODULE 10]
  - [ ] Tất cả ngày lễ Việt Nam năm 2026
  - [ ] Tất cả ngày lễ Lào năm 2026
  - [ ] Loại: Official (không trả) vs Work-day (tạm ứng)
  - [ ] Mức tạm ứng: TBD (chờ từ P Tổ chức)
  
- [ ] **Hệ Số Hỗ Trợ Vùng Khó Khăn (Zone Support)** [MODULE 9]
  - [ ] Code & tên vùng
  - [ ] Support coefficient (1.15 = 15% tăng)
  - [ ] Effective từ ngày nào
  
- [ ] **Tỷ Giá Hàng Tháng (Exchange Rate)** [MODULE 7]
  - [ ] 1 Bath = ? Kíp (hàng tháng)
  - [ ] Nguồn: Vietinbank
  - [ ] Xác nhận bởi: P Tổ chức
  - [ ] Cập nhật trước 5 ngày làm tháng
  
- [ ] **DRC & Quy Khô (Dry Rubber Content)** [MODULE 8]
  - [ ] Hệ số DRC cho từng loại cao su
  - [ ] Khối lượng tươi: Input từ Trạm cán
  - [ ] Formula: Fresh × DRC = Dry (GIỮ thập phân)
  - [ ] Cuối tháng: Chốt DRC & hạng kỹ thuật
  
- [ ] **Phụ Cấp Cố Định (Allowances)** [MODULE 10]
  - [ ] Danh sách phụ cấp từ P Tổ chức
  - [ ] Mức tính Kíp
  - [ ] Effective date: Khi có văn bản
  
- [ ] **Tiêu Chuẩn QTKT (Accounting Standards)**
  - [ ] Mã kế toán: 5xxx cho lương
  - [ ] Ánh xạ: Bộ phận ↔ Mã
  
- [ ] **Phân Loại Công Nhân (Employee Types)** [NEW - ECOTECH]
  - [ ] DIRECT: Lành tính = 100% Kíp, không bảo hiểm
  - [ ] INDIRECT: Lương = Đô, quy sang Kíp, có bảo hiểm
  - [ ] Mapping: CNKT = DIRECT, Cán bộ = INDIRECT

---

## 13. TÍCH HỢP PHÒNG TỔNG HỢP (PTO - Phòng Tổ Chức)

**Yêu Cầu Từ PTO:**

1. **Mã Lao Động 2026 (MSNV)**
   - Format: YYYY-NNN (VD: 2026-001)
   - PTO: Phát sinh & maintain
   - System: Validate format, không trùng
   - Status: 🟡 Chờ quy tắc chi tiết từ Anh Nhật

2. **Tỷ Giá Hàng Tháng**
   - Nguồn: P Kế toán (Vietinbank)
   - Xác nhận: P Tổ chức
   - Cộng bố: Đầu tháng
   - Status: 🟡 Chờ process từ Phòng Tổ chức

3. **Quy Định Phụ Cấp & Chăm Sóc**
   - Chi tiết: Phụ cấp tháng, lương chăm sóc
   - Status: 🟡 Chờ file từ Chị Phượng

4. **Quy Định Ngày Lễ & Tạm Ứng**
   - Ngày lễ: Việt Nam + Lào 2026
   - Mức tạm ứng: Chờ định
   - Status: 🟡 Chờ file từ P Tổ chức

---

**📋 NHÓM 1: DỮ LIỆU NỀN - v2.0 [ECOTECH 2A]**
**Ngày Cập Nhật: 10/04/2026**
**Trạng Thái: READY FOR DEPLOY (chờ 4 files PTO)**



