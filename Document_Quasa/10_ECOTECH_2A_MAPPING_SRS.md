# 🗺️ ÁNH XẠ ECOTECH 2A → SRS PAYROLL SYSTEM
## Tương Ứng Các Module ECOTECH 2A Với Thiết Kế SRS

---

## 1. TỔNG QUAN ÁNH XẠ

```
ECOTECH 2A - PM TIỀN LƯƠNG        →    SRS PAYROLL SYSTEM
═══════════════════════════════════════════════════════════════

NHÓM 1: Dữ Liệu Nền               →    DATABASE LAYER / MASTER DATA
├─ Hồ sơ lao động                 →    Employees Table (MSNV 2025)
├─ Cài đơn giá                    →    Salary_Scale Table (CÁCH TÍNH LƯƠNG)
├─ Cài hệ số vườn cây             →    System_Parameters (Coefficients)
├─ Quản lý ngày lễ                →    Holiday_Calendar Table (NEW)
└─ Tiêu chuẩn QTKT                →    Accounting_Standards Table (NEW)

NHÓM 2: Phát Sinh                 →    TRANSACTION INPUT LAYER
├─ Chấm công                      →    Attendance Table (CHẤM CÔNG)
├─ Công nợ & tạm ứng             →    Advances Table (NEW)
├─ Phụ cấp & chế độ               →    Allowances & Benefits Table (NEW)
├─ Đánh giá kỹ thuật              →    Performance Table (sản lượng)
└─ Bổ công chăm sóc               →    Care_Adjustments Table (NEW)

NHÓM 3: Xử Lý                     →    CALCULATION ENGINE (5 PHASES)
├─ Truy thu & truy lãnh           →    Phase 3 (Main Calc) - Adjustments
├─ Cân đối & chốt QK              →    Phase 4 (Reconciliation) - NEW
└─ Tính lương (CHÍNH)             →    Phases 1-5 (Main Payroll Calc)
                                        ├─ Phase 1: Performance Adjustment
                                        ├─ Phase 2: Base Salary Lookup
                                        ├─ Phase 3: Main Calculation
                                        ├─ Phase 4: Tax & Deductions
                                        └─ Phase 5: Cost Allocation

NHÓM 4: Báo Cáo                   →    REPORTING & OUTPUT LAYER
└─ Báo cáo lương tổng hợp         →    Payroll Report Engine
                                        ├─ v_payroll_summary (Tóm tắt)
                                        ├─ v_allocation_summary (Phân bổ)
                                        └─ Various Export Formats (PDF, Excel)
```

---

## 2. CHI TIẾT ÁNH XẠ NHÓM 1: DỮ LIỆU NỀN

### 2.1 Hồ Sơ Lao Động ↔ Employees Table

| ECOTECH 2A | SRS | Ghi Chú |
|---|---|---|
| Mã NV | msnv | Primary Key |
| Họ Tên | name | VARCHAR |
| Chức Vụ | position | VARCHAR |
| Bộ Phận | department | VARCHAR |
| Ngày Vào | hire_date | DATE |
| Trạm Làm (1/2) | tram_id | INT FK |
| Trạng Thái | active | BOOLEAN |

### 2.2 Cài Đơn Giá ↔ Salary_Scale Table

| ECOTECH 2A | SRS | Ghi Chú |
|---|---|---|
| Cấp Bậc | grade | CHAR (A/B/C/D/E) |
| Trạm | tram_id | INT (1/2) |
| Hệ Số | coefficient | DECIMAL |
| Lương Cơ Bản | base_salary | DECIMAL |
| Hiệu Lực Từ | effective_date | DATE |

### 2.3 Cài Hệ Số Vườn Cây ↔ System_Parameters Table

| ECOTECH 2A | SRS | Ghi Chú |
|---|---|---|
| P7 (Tham Số Khoán) | param_name='P7_ADJUSTMENT' | DECIMAL 27.0 |
| Hệ Số TRAM1 A/B/C | param_name='COEF_TRAM1_*' | DECIMAL 9.2/8.9/8.6 |
| Hệ Số TRAM2 A/B/C | param_name='COEF_TRAM2_*' | DECIMAL 7.7/7.4/7.1 |
| Tỷ Lệ BHXH | param_name='BHXH_RATE' | DECIMAL 0.08 |
| Tỷ Lệ BHYT | param_name='BHYT_RATE' | DECIMAL 0.015 |

### 2.4 Quản Lý Ngày Lễ (NEW) ↔ Holiday_Calendar Table

```sql
CREATE TABLE holiday_calendar (
  holiday_id INT AUTO_INCREMENT PRIMARY KEY,
  holiday_date DATE,
  holiday_name VARCHAR(100),
  holiday_type ENUM('OFFICIAL', 'SPECIAL', 'MAKE_UP'),
  year INT,
  notes VARCHAR(255)
);
```

### 2.5 Tiêu Chuẩn QTKT (NEW) ↔ Accounting_Standards Table

```sql
CREATE TABLE accounting_standards (
  std_id INT AUTO_INCREMENT PRIMARY KEY,
  standard_name VARCHAR(100),
  accounting_code VARCHAR(20),
  department VARCHAR(50),
  description VARCHAR(255),
  active BOOLEAN
);
```

---

## 3. CHI TIẾT ÁNH XẠ NHÓM 2: PHÁT SINH

### 3.1 Chấm Công ↔ Attendance Table

| ECOTECH 2A | SRS | Ghi Chú |
|---|---|---|
| Mã NV | msnv | FK |
| Tháng/Năm | year_month | DATE |
| Số Công | work_days | DECIMAL |
| Ghi Chú | remarks | VARCHAR |

### 3.2 Công Nợ & Tạm Ứng (NEW) ↔ Advances Table

```sql
CREATE TABLE advances (
  advance_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month DATE,
  amount DECIMAL(12,2),
  advance_type ENUM('ADVANCE', 'DEBT_RECOVERY'),
  notes VARCHAR(255),
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 3.3 Phụ Cấp & Chế Độ (NEW) ↔ Allowances & Benefits Table

```sql
CREATE TABLE allowances_benefits (
  allowance_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month DATE,
  allowance_type ENUM('HOUSING', 'TRANSPORT', 'FAMILY', 'HAZARD', 'SPECIAL'),
  amount DECIMAL(12,2),
  description VARCHAR(255),
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 3.4 Đánh Giá Kỹ Thuật ↔ Performance Table

| ECOTECH 2A | SRS | Ghi Chú |
|---|---|---|
| Mã NV | msnv | FK |
| Tháng/Năm | year_month | DATE |
| Sản Lượng | output | DECIMAL |
| Cấp Xếp Loại | quality_rating | CHAR (A/B/C/D/E) |
| Trạm | tram_id | INT (1/2) |

### 3.5 Bổ Công Chăm Sóc (NEW) ↔ Care_Adjustments Table

```sql
CREATE TABLE care_adjustments (
  care_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month DATE,
  care_type ENUM('BONUS', 'PENALTY', 'ADJUSTMENT'),
  amount DECIMAL(12,2),
  reason VARCHAR(255),
  approved_by INT,
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

---

## 4. CHI TIẾT ÁNH XẠ NHÓM 3: XỬ LÝ

### 4.1 GIAI ĐOẠN Tính Lương (CHÍNH)

```
TÍNH LƯƠNG (CHÍNH) - NHÓM 3
═══════════════════════════════════════════════════════════════

SRS PHASE 1: Điều Chỉnh Hiệu Suất
├─ Input: Performance (sản lượng), Quality Grade (A/B/C/D/E)
├─ Công Thức: adjusted_output = output × grade_coefficient
├─ Process: IF grade='A' THEN coef=9.2 (TRAM1) ELSE ...
└─ Output: san_luong_dieu_chinh

SRS PHASE 2: Lương Cơ Bản + Hệ Số
├─ Input: Salary_Scale (CÁCH TÍNH LƯƠNG)
├─ Công Thức: base_salary = lookup từ bảng lương
└─ Output: luong_co_ban_tham_chieu

SRS PHASE 3: Tính Lương Chính (CORE)
├─ Input: base_salary, work_days, P7 parameter
├─ Công Thức: salary_gross = (base + days) / P7 × coef
├─ Process: Điều chỉnh theo hệ số vườn cây
└─ Output: luong_tinh_toan

SRS PHASE 4: Thuế & Khấu Trừ
├─ Input: salary_gross
├─ Công Thức: 
│  ├─ tax = progressive_tax(salary_gross) [6 tiers]
│  ├─ bhxh = salary_gross × 0.08
│  └─ bhyt = salary_gross × 0.015
└─ Output: tax, bhxh, bhyt

SRS PHASE 5: Lương Net & Phân Bổ
├─ Input: salary_gross, tax, deductions
├─ Công Thức: net_salary = gross - tax - deductions
├─ Process: Phân bổ theo cost centers
└─ Output: lương_net, phân_bổ_chi_phí
```

### 4.2 Truy Thu & Truy Lãnh (NEW) ↔ Phase 3 Adjustments

```sql
-- Trong Payroll Table, thêm columns:
ALTER TABLE payroll ADD COLUMN (
  recovery_amount DECIMAL(12,2) COMMENT 'Truy thu từ tháng trước',
  interest_amount DECIMAL(12,2) COMMENT 'Truy lãnh (lãi)',
  adjusted_gross DECIMAL(12,2) COMMENT 'Gross sau điều chỉnh',
  recovery_reason VARCHAR(255)
);
```

### 4.3 Cân Đối & Chốt QK (NEW) ↔ Phase 4 Reconciliation

```sql
-- Tạo bảng reconciliation:
CREATE TABLE payroll_reconciliation (
  reconcile_id INT AUTO_INCREMENT PRIMARY KEY,
  year_month DATE,
  payroll_count INT,
  total_gross DECIMAL(12,2),
  total_tax DECIMAL(12,2),
  total_deductions DECIMAL(12,2),
  total_net DECIMAL(12,2),
  
  -- Kiểm tra cân bằng
  variance DECIMAL(12,2),
  is_balanced BOOLEAN,
  reconciled_by INT,
  reconciled_date TIMESTAMP,
  
  notes VARCHAR(500)
);
```

---

## 5. CHI TIẾT ÁNH XẠ NHÓM 4: BÁO CÁO

### 5.1 Báo Cáo Lương Tổng Hợp

| Báo Cáo | Loại | Output | Input |
|---|---|---|---|
| **BÁO CÁO LƯƠNG TỔNG HỢP** | CSV/PDF | | Payroll Table |
| Danh Sách Lương Chi Tiết | Detail | Name, Gross, Tax, Net | v_payroll_summary |
| Tóm Tắt Theo Trạm | Summary | Total Gross, Tax by TRAM | Aggregation |
| Tóm Tắt Theo Bộ Phận | Summary | Total Gross, Tax by Dept | Aggregation + Cost Allocation |
| Phân Bổ Chi Phí | Distribution | Amount by Cost Center | v_allocation_summary |
| Báo Cáo Thuế | Tax | Tax Bracket Analysis | Tax Calculation |
| Báo Cáo Bảo Hiểm | Benefits | BHXH, BHYT Summary | Insurance Calc |

### 5.2 Views & Queries Báo Cáo

```sql
-- VIEW 1: Tóm Tắt Lương
CREATE VIEW v_payroll_summary AS
SELECT 
  p.year_month,
  e.msnv,
  e.name,
  e.position,
  e.department,
  p.salary_base,
  p.work_days,
  p.salary_calculated as luong_tinh,
  p.tax_amount as thue,
  p.bhxh_amount,
  p.bhyt_amount,
  p.net_salary as luong_net,
  p.status
FROM payroll p
JOIN employees e ON p.msnv = e.msnv;

-- VIEW 2: Phân Bổ Chi Phí
CREATE VIEW v_allocation_summary AS
SELECT 
  ca.payroll_id,
  cc.dept_name,
  SUM(ca.allocated_amount) as phan_bo_chi_phi,
  COUNT(*) as so_nhan_vien
FROM cost_allocations ca
JOIN cost_centers cc ON ca.dept_id = cc.dept_id
GROUP BY ca.payroll_id, cc.dept_name;

-- VIEW 3: Báo Cáo Thuế Lũy Tiến
CREATE VIEW v_tax_analysis AS
SELECT 
  p.msnv,
  e.name,
  p.salary_calculated,
  CASE 
    WHEN p.salary_calculated <= 5000000 THEN 'Bậc 1: 5%'
    WHEN p.salary_calculated <= 10000000 THEN 'Bậc 2: 10%'
    WHEN p.salary_calculated <= 20000000 THEN 'Bậc 3: 15%'
    WHEN p.salary_calculated <= 25000000 THEN 'Bậc 4: 20%'
    WHEN p.salary_calculated <= 65000000 THEN 'Bậc 5: 25%'
    ELSE 'Bậc 6: 35%'
  END as tax_bracket,
  p.tax_amount,
  ROUND(p.tax_amount / p.salary_calculated * 100, 2) as thue_phan_tram
FROM payroll p
JOIN employees e ON p.msnv = e.msnv;
```

---

## 6. BẢNG TƯƠNG ỨNG ĐỦ ĐẦY

| ECOTECH 2A Module | SRS Layer | Database Table | Purpose | Status |
|---|---|---|---|---|
| **NHÓM 1** | | | | |
| Hồ sơ lao động | Master Data | employees | Quản lý nhân viên | ✅ Existing |
| Cài đơn giá | Master Data | salary_scale | Bảng lương | ✅ Existing |
| Cài hệ số vườn cây | Config | system_parameters | Các tham số | ✅ Existing |
| Quản lý ngày lễ | Master Data | holiday_calendar | Ngày lễ | 🆕 NEW |
| Tiêu chuẩn QTKT | Master Data | accounting_standards | Tiêu chuẩn kế toán | 🆕 NEW |
| **NHÓM 2** | | | | |
| Chấm công | Transaction | attendance | Số công | ✅ Existing |
| Công nợ & tạm ứng | Transaction | advances | Tạm ứng & truy thu | 🆕 NEW |
| Phụ cấp & chế độ | Transaction | allowances_benefits | Phụ cấp | 🆕 NEW |
| Đánh giá kỹ thuật | Transaction | performance | Sản lượng & chất lượng | ✅ Existing |
| Bổ công chăm sóc | Transaction | care_adjustments | Điều chỉnh & thưởng | 🆕 NEW |
| **NHÓM 3** | | | | |
| Tính lương (CHÍNH) | Calculation | payroll | 5 Phases calc | ✅ Existing |
| Truy thu & truy lãnh | Adjustment | payroll + reconciliation | Điều chỉnh | 🆕 NEW |
| Cân đối & chốt QK | Reconciliation | payroll_reconciliation | Cân bằng | 🆕 NEW |
| **NHÓM 4** | | | | |
| Báo cáo lương tổng hợp | Reporting | v_payroll_summary | Report | ✅ Existing |

---

## 7. CÁC NGUYÊN TẮC ÁNH XẠ

### 7.1 Khớp Đặt Tên (Name Mapping)

```
ECOTECH 2A (Tiếng Việt)         →    SRS (English + Tiếng Việt)
────────────────────────────────────────────────────────────────
Mã NV                           →    msnv (Mã Số Nhân Viên)
Hồ sơ lao động                  →    employees (Bảng Nhân Viên)
Chấm công                       →    attendance (Bảng Chấm Công)
Sản lượng                       →    performance (Bảng Hiệu Suất)
Cấp xếp loại                    →    quality_rating (A/B/C/D/E)
Lương cơ bản                    →    salary_base (Lương Cơ Bản)
Tính lương                      →    salary_calculated (Lương Tính)
Thuế                            →    tax_amount (Thế Thuế TNCN)
BHXH                            →    bhxh_amount (Bảo Hiểm Xã Hội)
Lương net                       →    net_salary (Lương Net)
Phân bổ                         →    cost_allocations (Phân Bổ Chi Phí)
```

### 7.2 Quy Tắc Kiến Trúc

```
ECOTECH 2A          →    SRS Architecture
─────────────────────────────────────────────────

Nhóm 1              →    DATABASE LAYER (Master Data)
├─ Input từ UI      →    Employees, Salary Scale
├─ Tham số config   →    System Parameters
└─ Reference data   →    Holiday Calendar, Standards

Nhóm 2              →    TRANSACTION LAYER (Data Inputs)
├─ Chấm công        →    Attendance Input
├─ Phát sinh khác   →    Advances, Allowances, Care Adj
└─ Hiệu suất        →    Performance Input

Nhóm 3              →    CALCULATION LAYER (Processing)
├─ Phase 1-5        →    Salary Calculation Engine
├─ Điều chỉnh       →    Recovery & Adjustments
└─ Chốt sổ          →    Reconciliation

Nhóm 4              →    REPORTING LAYER (Output)
└─ Báo cáo          →    Report Engine + Views
```

---

## 8. DANH SÁCH CÂU HỎI CẦN GIẢI THÍCH

### Từ ECOTECH 2A, Tôi Cần Biết:

```
❓ NHÓM 1: DỮ LIỆU NỀN

1. Quản lý ngày lễ
   Q: Hệ thống có lưu danh sách ngày lễ không?
   Q: Cách sử dụng: ảnh hưởng đến tính lương (tính thêm hay tính bình thường)?
   Q: Ngày lễ có tính vào "số công" hay không?

2. Tiêu chuẩn QTKT
   Q: Tiêu chuẩn này dùng để làm gì?
   Q: Liên quan trực tiếp đến tính lương hay chỉ trong báo cáo?
   Q: Có bao nhiêu tiêu chuẩn khác nhau?

❓ NHÓM 2: PHÁT SINH

3. Công Nợ & Tạm Ứng
   Q: Tạm ứng là gì? (Lương tạm vào giữa tháng?)
   Q: Công nợ = tiền thu hồi lại từ lương trước?
   Q: Cách xử lý: trừ trong lương hiện tại hay vay thêm?

4. Phụ Cấp & Chế Độ
   Q: Phụ cấp loại nào? (Nhà ở, đi lại, gia đình, v.v?)
   Q: Cộng vào lương hay tính riêng?
   Q: Có áp dụng thuế không?

5. Bổ Công Chăm Sóc
   Q: Bổ công = điều chỉnh số công thêm?
   Q: Chăm sóc = thưởng thêm hay cắt giảm?
   Q: Ảnh hưởng = tính vào "số công" hay "lương"?

❓ NHÓM 3: XỬ LÝ

6. Truy Thu & Truy Lãnh
   Q: Truy thu từ đâu? (Tháng trước không đủ tiền, công ty phải thu hồi?)
   Q: Truy lãnh là gì? (Tính lãi?)
   Q: Công thức tính: % bao nhiêu?

7. Cân Đối & Chốt QK
   Q: Chốt QK = lock dữ liệu tháng để không sửa thêm?
   Q: Cân đối kiểm tra gì? (Tổng lương, tổng thuế, v.v?)
   Q: Nếu không cân bằng thì xử lý thế nào?

❓ NHÓM 4: BÁNG CÁO

8. Báo Cáo Lương Tổng Hợp
   Q: Báo cáo này dành cho ai? (Kế toán, HR, giám đốc?)
   Q: Tần suất: hàng tuần, hàng tháng?
   Q: Format: Excel, PDF, HTML?
   Q: Các chỉ số cần: Lương, thuế, bảo hiểm, BHXH riêng?
```

---

## 9. TÌNH TRẠNG PHỦ HỘNGRAPH

| Module ECOTECH 2A | Phủ Hộng Trong SRS | Tình Trạng | Ghi Chú |
|---|---|---|---|
| ✅ Hồ sơ lao động | 100% | ✅ Đầy đủ | Employees table |
| ✅ Cài đơn giá | 100% | ✅ Đầy đủ | Salary_scale table |
| ✅ Hệ số vườn cây | 100% | ✅ Đầy đủ | System_parameters |
| ⚠️ Quản lý ngày lễ | 0% | 🆕 Cần tạo | Holiday_calendar NEW |
| ⚠️ Tiêu chuẩn QTKT | 0% | 🆕 Cần tạo | Accounting_standards NEW |
| ✅ Chấm công | 100% | ✅ Đầy đủ | Attendance table |
| ⚠️ Công nợ & tạm ứng | 0% | 🆕 Cần tạo | Advances table NEW |
| ⚠️ Phụ cấp & chế độ | 0% | 🆕 Cần tạo | Allowances_benefits NEW |
| ✅ Đánh giá kỹ thuật | 100% | ✅ Đầy đủ | Performance table |
| ⚠️ Bổ công chăm sóc | 50% | ⚠️ Riêng phần | Care_adjustments NEW |
| ✅ Tính lương (CHÍNH) | 100% | ✅ Đầy đủ | Phases 1-5 |
| ⚠️ Truy thu & truy lãnh | 50% | ⚠️ Riêng phần | Adjustment logic |
| ⚠️ Cân đối & chốt QK | 30% | ⚠️ Cần mở rộng | Reconciliation NEW |
| ✅ Báo cáo lương | 90% | ✅ Đầy đủ | Mostly covered |

---

## 10. KỊ HOẠCH TIẾP THEO

✅ **Xin Hình Ảnh/Tài Liệu Từ ECOTECH 2A:**

```
❓ Bạn có thể cung cấp hình ảnh/tài liệu sau để làm chi tiết hơn:

1. Sơ đồ quy trình ECOTECH 2A (Flow diagram)
   ├─ Luồng dữ liệu từ Nhóm 1 → 2 → 3 → 4
   ├─ Dependencies giữa các module
   └─ Điểm gửi/nhận dữ liệu

2. Định nghĩa các module/tính năng
   ├─ Mô tả chi tiết mỗi module
   ├─ Công thức tính toán cụ thể
   ├─ Quy tắc kinh doanh
   └─ Định dạng dữ liệu input/output

3. Hình ảnh giao diện (nếu đã có prototype)
   ├─ Màn hình nhập PLC
   ├─ Màn hình tính lương
   ├─ Báo cáo lương
   └─ Cấu hình tham số

4. Bảng tra cứu/Reference Data
   ├─ Mã loại phụ cấp
   ├─ Loại bộ phận chi phí
   ├─ Hệ số điều chỉnh theo tháng
   └─ Ngưỡng thuế/bảo hiểm

5. Tài liệu hiện tại
   ├─ Quy trình tính lương tấtành
   ├─ Công thức Excel hiện dùng
   ├─ Báo cáo mẫu
   └─ Định nghĩa nghiệp vụ
```

---

**ÁNH XẠ & MAPPING v1.0**
**Chuẩn Bị Chi Tiết: 10/04/2026**
**Trạng Thái: CHỜ THÊM THÔNG TIN TỨ ECOTECH 2A**

