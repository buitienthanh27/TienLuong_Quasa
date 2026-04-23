# ĐẶC TẢ YÊU CẦU HỆ THỐNG QUẢN LÝ LƯƠNG - ĐỘI 1
# SYSTEM REQUIREMENTS SPECIFICATION (SRS)
## ECOTECH 2A - Payroll Management System

---

| Thông tin tài liệu | Chi tiết |
|---|---|
| **Dự án** | Chuyển đổi hệ thống lương từ Excel sang phần mềm quản trị |
| **Phiên bản** | 1.0 |
| **Ngày tạo** | 13/04/2026 |
| **File Excel gốc** | `LƯƠNG ĐỘI 1 THÁNG 12.2025 2 BẢN.xlsx` |
| **Đường dẫn** | `2025/tháng 12/Đội 1/` |
| **Kích thước file** | 2.6 MB, 59 sheets |
| **Đối tượng đọc** | CEO (nghiệp vụ) & Team Dev (kỹ thuật) |
| **Trạng thái** | Draft v1.0 |

---

# MỤC LỤC

- [PHẦN 1: ĐẶC TẢ YÊU CẦU NGHIỆP VỤ](#phần-1-đặc-tả-yêu-cầu-nghiệp-vụ)
- [PHẦN 2: KIẾN TRÚC HỆ THỐNG & MÔ HÌNH DỮ LIỆU](#phần-2-kiến-trúc-hệ-thống--mô-hình-dữ-liệu)
- [PHẦN 3: YÊU CẦU CHỨC NĂNG & USE CASES](#phần-3-yêu-cầu-chức-năng--use-cases)
- [PHẦN 4: GIẢI MÃ CÔNG THỨC & LOGIC TÍNH TOÁN](#phần-4-giải-mã-công-thức--logic-tính-toán)
- [PHẦN 5: TÓM TẮT CHÍNH SÁCH LƯƠNG (CHO CEO)](#phần-5-tóm-tắt-chính-sách-lương-cho-ceo)
- [PHỤ LỤC](#phụ-lục)

---
---

# PHẦN 1: ĐẶC TẢ YÊU CẦU NGHIỆP VỤ
## (Business Requirement Specification)

---

## 1.1 MỤC TIÊU HỆ THỐNG

### 1.1.1 Vấn đề hiện tại (AS-IS)

Hiện tại, toàn bộ việc tính lương Đội 1 đang được thực hiện trên **1 file Excel duy nhất** với 59 sheets. Hệ thống này tồn tại nhiều vấn đề nghiêm trọng:

| STT | Vấn đề | Mức độ | Mô tả |
|---|---|---|---|
| 1 | **Lỗi #REF!** | NGHIÊM TRỌNG | 100+ lỗi công thức tham chiếu đến sheet/file đã bị xoá |
| 2 | **Liên kết ngoài** | CAO | 10+ tham chiếu đến file Excel khác đã mất ([7], [2]) |
| 3 | **Hệ số ngầm** | TRUNG BÌNH | Các hệ số chính sách nằm thẳng trong công thức, không tách riêng |
| 4 | **Phụ thuộc VLOOKUP** | CAO | Lệch tên/mã dẫn đến mất sản lượng, mất phụ cấp, lương sai hàng loạt |
| 5 | **Quá nhiều sheets** | TRUNG BÌNH | 59 sheets khó quản lý, dễ nhầm lẫn |
| 6 | **Không kiểm soát thay đổi** | CAO | Không có log ai sửa gì, khi nào |
| 7 | **Không có phân quyền** | CAO | Bất kỳ ai mở file đều có thể sửa hệ số |

### 1.1.2 Mục tiêu chuyển đổi (TO-BE)

| STT | Mục tiêu | Mô tả | Độ ưu tiên |
|---|---|---|---|
| 1 | **Số hoá quy trình** | Chuyển toàn bộ logic tính lương từ Excel sang phần mềm | P0 - Bắt buộc |
| 2 | **Chuẩn hoá dữ liệu** | Tách hệ số, tham số thành bảng chính sách độc lập | P0 - Bắt buộc |
| 3 | **Kiểm soát thay đổi** | Ghi log mọi thay đổi hệ số, công thức, dữ liệu | P0 - Bắt buộc |
| 4 | **Phân quyền** | Phân quyền nhập liệu, duyệt, xem báo cáo | P1 - Quan trọng |
| 5 | **Báo cáo tự động** | Xuất bảng phân toán, phiếu lương tự động | P1 - Quan trọng |
| 6 | **Giảm lỗi** | Loại bỏ #REF!, external links, lỗi lookup | P0 - Bắt buộc |

### 1.1.3 Phạm vi hệ thống (Scope)

```
TRONG PHẠM VI (In Scope):
  [x] Tính lương công nhân khai thác (TRẠM 1, TRẠM 2)
  [x] Tính lương cán bộ đội (CÁCH TÍNH LƯƠNG)
  [x] Tính lương CB-CNV người Lào, bảo vệ, tạp vụ (LƯƠNG ĐỘI)
  [x] Phân toán quỹ lương (BẢNG PHÂN TOÁN)
  [x] Quản lý chấm công
  [x] Quản lý sản lượng & quy khô (DRC)
  [x] Quản lý hệ số, tham số chính sách
  [x] Báo cáo cho CEO và kế toán

NGOÀI PHẠM VI (Out of Scope):
  [ ] Tính lương các đội khác (Đội 2, 3, 4, 5, 6)
  [ ] Hệ thống HR toàn diện
  [ ] Module tuyển dụng
  [ ] Module đào tạo
```

---

## 1.2 QUY TRÌNH NGHIỆP VỤ TOÀN TRÌNH

### 1.2.1 Tổng quan luồng nghiệp vụ (Business Flow)

```
================================================================================
              QUY TRÌNH TÍNH LƯƠNG ĐỘI 1 - HÀNG THÁNG
================================================================================

GIAI ĐOẠN 1: NHẬP LIỆU ĐẦU THÁNG (Ngày 1-5)
│
│  [1.1] Cập nhật danh mục nhân sự
│        Sheet: MSNV 2025, DS CN MOI UP
│        → Mã nhân viên, họ tên, vị trí, trạm/đội
│
│  [1.2] Cập nhật hệ số tháng mới (nếu có thay đổi)
│        Sheet: TRẠM 1!E8 (tỷ giá), TRẠM 1!AE5-AE9 (DRC)
│        → Tỷ giá Bath/Kip, DRC mủ tạp, DRC mủ serum
│
▼
GIAI ĐOẠN 2: GHI NHẬN HÀNG NGÀY (Ngày 1 - cuối tháng)
│
│  [2.1] Chấm công hàng ngày
│        Sheet: CHẤM CÔNG, CC NGƯỜI LÀO, Công cây non
│        → Công thường, công chủ nhật, công khộp nặng,
│          cạo 2 lát, cây non
│
│  [2.2] Ghi nhận sản lượng
│        Sheet: sản lượng, MỦ DÂY, MỦ SIRUM, MỦ XIRUM
│        → Kg mủ tạp, mủ serum theo từng người
│
▼
GIAI ĐOẠN 3: XỬ LÝ TRUNG GIAN (Ngày 25-28)
│
│  [3.1] Tổng hợp công việc kỹ thuật (THCKT)
│        Sheet: THCKT
│        → Tổng hợp DRC, quy khô, công theo đội/trạm
│
│  [3.2] Xếp hạng kỹ thuật
│        Sheet: sản lượng
│        → Hạng A/B/C/D cho từng công nhân
│
▼
GIAI ĐOẠN 4: TÍNH LƯƠNG (Ngày 28-30)
│
│  [4.1] Tính lương công nhân khai thác
│        Sheet: TRẠM 1, TRẠM 2
│        → Lương chăm sóc + Lương sản lượng + Phụ cấp
│
│  [4.2] Tính lương cán bộ đội
│        Sheet: CÁCH TÍNH LƯƠNG
│        → Lương bậc + Lương hiệu quả + Phụ cấp
│
│  [4.3] Tính lương nhóm khác
│        Sheet: LƯƠNG ĐỘI
│        → Lương theo công chuẩn + Thuế TNCN
│
▼
GIAI ĐOẠN 5: BÁO CÁO & DUYỆT (Ngày 30 - ngày 5 tháng sau)
│
│  [5.1] Lập bảng phân toán
│        Sheet: BẢNG PHÂN TOÁN
│        → Phân bổ quỹ lương theo khoản mục
│
│  [5.2] Trình ký duyệt
│        → CEO duyệt, kế toán thanh toán
│
▼
GIAI ĐOẠN 6: THANH TOÁN
     → Chi trả lương
================================================================================
```

### 1.2.2 Ma trận RACI

| Hoạt động | Kế toán đội | QLKT | Kế toán công ty | CEO |
|---|---|---|---|---|
| Nhập chấm công | **R** | A | I | - |
| Nhập sản lượng | I | **R** | A | - |
| Xếp hạng kỹ thuật | - | **R** | A | I |
| Tính lương trạm | **R** | C | A | - |
| Tính lương cán bộ | **R** | - | A | I |
| Lập bảng phân toán | **R** | C | **A** | I |
| Duyệt lương | - | C | **R** | **A** |
| Thanh toán | - | - | **R** | A |

> R = Responsible (Thực hiện), A = Accountable (Chịu trách nhiệm), C = Consulted, I = Informed

---

## 1.3 CÁC NHÓM ĐỐI TƯỢNG TÍNH LƯƠNG

Đội 1 có 4 nhóm đối tượng chính, mỗi nhóm có cơ chế tính lương **KHÁC NHAU**:

| Nhóm | Đối tượng | Sheet tính | Logic tính | Số lượng (ước) |
|---|---|---|---|---|
| **NHÓM A** | Công nhân khai thác TRẠM 1 | `TRẠM 1` | Chăm sóc + Sản lượng + Phụ cấp | ~50-60 người |
| **NHÓM B** | Công nhân khai thác TRẠM 2 | `TRẠM 2` | Tương tự TRẠM 1, hệ số thấp hơn | ~60-70 người |
| **NHÓM C** | Cán bộ đội | `CÁCH TÍNH LƯƠNG` | Lương bậc + Hiệu quả + Phụ cấp | ~10-15 người |
| **NHÓM D** | CB-CNV người Lào, Bảo vệ, Tạp vụ | `LƯƠNG ĐỘI` | Lương tháng / công chuẩn × công thực | ~20-30 người |

---
---

# PHẦN 2: KIẾN TRÚC HỆ THỐNG & MÔ HÌNH DỮ LIỆU
## (System Architecture & Data Model)

---

## 2.1 THỰC THỂ (Entities)

### 2.1.1 Sơ đồ thực thể

```
================================================================================
                       SƠ ĐỒ THỰC THỂ (ENTITY MAP)
================================================================================

    [EMPLOYEES]          [DEPARTMENTS]         [STATIONS]
    Nhân viên            Phòng ban/Đội         Trạm khai thác
         │                    │                      │
         └────────────────────┼──────────────────────┘
                              │
    [ATTENDANCE]         [PRODUCTION]          [TECH_GRADE]
    Chấm công            Sản lượng             Hạng kỹ thuật
         │                    │                      │
         └────────────────────┼──────────────────────┘
                              │
    [SALARY_SCALE]       [SALARY_PARAMS]       [EXCHANGE_RATE]
    Bảng lương/bậc       Hệ số chính sách      Tỷ giá
         │                    │                      │
         └────────────────────┼──────────────────────┘
                              │
    [PAYROLL]            [PAYROLL_DETAIL]       [TAX_BRACKETS]
    Bảng lương tháng     Chi tiết lương         Biểu thuế
         │                    │
         └────────────────────┘
                              │
    [COST_ALLOCATION]    [ALLOWANCES]           [AUDIT_LOGS]
    Phân toán            Phụ cấp                Nhật ký thay đổi
================================================================================
```

### 2.1.2 Mô tả chi tiết từng thực thể

| STT | Entity | Tên tiếng Việt | Nguồn Excel | Mô tả |
|---|---|---|---|---|
| 1 | `employees` | Nhân viên | MSNV 2025, DS CN MOI UP | Thông tin cá nhân, mã NV, tên, vị trí |
| 2 | `departments` | Phòng ban/Đội | Cấu trúc workbook | Đội 1, các trạm |
| 3 | `stations` | Trạm khai thác | TRẠM 1, TRẠM 2 | Vùng địa bàn khai thác |
| 4 | `attendance` | Chấm công | CHẤM CÔNG, CC NGƯỜI LÀO | Công thường, CN, khộp nặng, cây non... |
| 5 | `production` | Sản lượng | sản lượng, MỦ DÂY/SIRUM | Kg mủ tạp, mủ serum theo người |
| 6 | `tech_grades` | Hạng kỹ thuật | sản lượng | Hạng A/B/C/D hàng tháng |
| 7 | `salary_scale` | Bảng lương bậc | CÁCH TÍNH LƯƠNG | Lương bậc theo chức danh |
| 8 | `salary_params` | Hệ số chính sách | Các ô tham số nằm rải rác | DRC, tỷ giá, hệ số phụ cấp... |
| 9 | `exchange_rates` | Tỷ giá | TRẠM 1!E8 | Tỷ giá Bath/Kip theo tháng |
| 10 | `drc_rates` | Hệ số DRC | TRẠM 1!AE5-AE9, THCKT | DRC mủ tạp, mủ serum |
| 11 | `payroll` | Bảng lương tháng | TRẠM 1, TRẠM 2, LƯƠNG ĐỘI | Tổng hợp lương hàng tháng |
| 12 | `payroll_details` | Chi tiết lương | Các cột trong TRẠM 1/2 | Từng khoản lương chi tiết |
| 13 | `tax_brackets` | Biểu thuế TNCN | LƯƠNG ĐỘI!J13 | 5 nấc thuế luỹ tiến |
| 14 | `cost_allocation` | Phân toán | BẢNG PHÂN TOÁN | Phân bổ quỹ lương |
| 15 | `allowances` | Phụ cấp | TRẠM 1/2 cột P-Y | Các loại phụ cấp nghề |
| 16 | `audit_logs` | Nhật ký thay đổi | **MỚI** (chưa có trong Excel) | Ghi log mọi thay đổi |

---

## 2.2 CÁC BẢNG DỮ LIỆU (Database Tables)

### BẢNG 1: `employees` — Nhân viên

> **Nguồn Excel:** Sheet `MSNV 2025`, `DS CN MOI UP`
> **Vị trí tham chiếu:** TRẠM 1!B16+, C16+ | TRẠM 2!B16+, C16+

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả | Nguồn Excel |
|---|---|---|---|---|---|
| 1 | `employee_id` | INT (PK) | NOT NULL | Mã tự động | Auto |
| 2 | `employee_code` | VARCHAR(20) | NOT NULL | Mã nhân viên (VD: 2025-001) | MSNV 2025!A |
| 3 | `full_name` | NVARCHAR(100) | NOT NULL | Họ và tên | MSNV 2025!B |
| 4 | `station_id` | INT (FK) | NULL | Mã trạm (TRẠM 1/2) | Xác định từ sheet |
| 5 | `department_id` | INT (FK) | NOT NULL | Mã đội/phòng | Đội 1 |
| 6 | `employee_type` | ENUM | NOT NULL | 'WORKER','OFFICER','LAO','GUARD','MISC' | Xác định từ sheet |
| 7 | `position` | NVARCHAR(50) | NULL | Chức vụ | CÁCH TÍNH LƯƠNG |
| 8 | `hire_date` | DATE | NULL | Ngày vào làm | DS CN MOI UP |
| 9 | `is_active` | BOOLEAN | NOT NULL | Đang làm việc | DEFAULT TRUE |
| 10 | `created_at` | DATETIME | NOT NULL | Ngày tạo | Auto |
| 11 | `updated_at` | DATETIME | NOT NULL | Ngày cập nhật | Auto |

```sql
CREATE TABLE employees (
    employee_id    INT PRIMARY KEY AUTO_INCREMENT,
    employee_code  VARCHAR(20) NOT NULL UNIQUE,
    full_name      NVARCHAR(100) NOT NULL,
    station_id     INT NULL,
    department_id  INT NOT NULL,
    employee_type  ENUM('WORKER','OFFICER','LAO','GUARD','MISC') NOT NULL,
    position       NVARCHAR(50) NULL,
    hire_date      DATE NULL,
    is_active      BOOLEAN NOT NULL DEFAULT TRUE,
    created_at     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (station_id) REFERENCES stations(station_id),
    FOREIGN KEY (department_id) REFERENCES departments(department_id)
);
```

---

### BẢNG 2: `stations` — Trạm khai thác

> **Nguồn Excel:** Cấu trúc workbook TRẠM 1, TRẠM 2
> **Ý nghĩa:** Mỗi trạm có MẶT BẰNG HỆ SỐ KHÁC NHAU

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả | Nguồn Excel |
|---|---|---|---|---|---|
| 1 | `station_id` | INT (PK) | NOT NULL | Mã trạm | Auto |
| 2 | `station_name` | NVARCHAR(50) | NOT NULL | Tên trạm | "TRẠM 1", "TRẠM 2" |
| 3 | `department_id` | INT (FK) | NOT NULL | Thuộc đội nào | Đội 1 |
| 4 | `grade_a_rate` | DECIMAL(5,2) | NOT NULL | Hệ số hạng A | TRẠM 1: 9.2 / TRẠM 2: 7.7 |
| 5 | `grade_b_rate` | DECIMAL(5,2) | NOT NULL | Hệ số hạng B | TRẠM 1: 8.9 / TRẠM 2: 7.4 |
| 6 | `grade_c_rate` | DECIMAL(5,2) | NOT NULL | Hệ số hạng C | TRẠM 1: 8.6 / TRẠM 2: 7.1 |
| 7 | `grade_d_rate` | DECIMAL(5,2) | NOT NULL | Hệ số hạng D | TRẠM 1: 8.3 / TRẠM 2: 6.8 |
| 8 | `is_hardship_zone` | BOOLEAN | NOT NULL | Vùng khó khăn | DEFAULT FALSE |

```sql
CREATE TABLE stations (
    station_id       INT PRIMARY KEY AUTO_INCREMENT,
    station_name     NVARCHAR(50) NOT NULL,
    department_id    INT NOT NULL,
    grade_a_rate     DECIMAL(5,2) NOT NULL,
    grade_b_rate     DECIMAL(5,2) NOT NULL,
    grade_c_rate     DECIMAL(5,2) NOT NULL,
    grade_d_rate     DECIMAL(5,2) NOT NULL,
    is_hardship_zone BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (department_id) REFERENCES departments(department_id)
);

-- Dữ liệu mẫu:
-- INSERT INTO stations VALUES (1, 'TRẠM 1', 1, 9.2, 8.9, 8.6, 8.3, FALSE);
-- INSERT INTO stations VALUES (2, 'TRẠM 2', 1, 7.7, 7.4, 7.1, 6.8, TRUE);
```

---

### BẢNG 3: `departments` — Phòng ban / Đội

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả |
|---|---|---|---|---|
| 1 | `department_id` | INT (PK) | NOT NULL | Mã đội |
| 2 | `department_name` | NVARCHAR(50) | NOT NULL | Tên đội |
| 3 | `department_code` | VARCHAR(10) | NOT NULL | Mã viết tắt |

```sql
CREATE TABLE departments (
    department_id   INT PRIMARY KEY AUTO_INCREMENT,
    department_name NVARCHAR(50) NOT NULL,
    department_code VARCHAR(10) NOT NULL UNIQUE
);
```

---

### BẢNG 4: `attendance` — Chấm công

> **Nguồn Excel:** Sheet `CHẤM CÔNG`, `CC NGƯỜI LÀO`, `Công cây non`
> **Vị trí tham chiếu:** TRẠM 1!P16-V16 | LƯƠNG ĐỘI!E13

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả | Nguồn Excel |
|---|---|---|---|---|---|
| 1 | `attendance_id` | INT (PK) | NOT NULL | Mã tự động | Auto |
| 2 | `employee_id` | INT (FK) | NOT NULL | Mã nhân viên | Lookup từ MSNV |
| 3 | `month` | INT | NOT NULL | Tháng | Tháng 12 |
| 4 | `year` | INT | NOT NULL | Năm | 2025 |
| 5 | `regular_days` | DECIMAL(5,2) | NOT NULL | Công thường | TRẠM 1!P16 |
| 6 | `sunday_days` | DECIMAL(5,2) | DEFAULT 0 | Công chủ nhật | TRẠM 1!Q16 |
| 7 | `young_tree_days` | DECIMAL(5,2) | DEFAULT 0 | Công cây non | TRẠM 1!R16 |
| 8 | `hardship_days` | DECIMAL(5,2) | DEFAULT 0 | Công khộp nặng | TRẠM 1!S16 |
| 9 | `double_cut_days` | DECIMAL(5,2) | DEFAULT 0 | Cạo 2 lát thường | TRẠM 1!T16 |
| 10 | `double_cut_sunday` | DECIMAL(5,2) | DEFAULT 0 | Cạo 2 lát CN | TRẠM 1!U16 |
| 11 | `care_days` | DECIMAL(5,2) | DEFAULT 0 | Công chăm sóc | TRẠM 1!F16 |
| 12 | `total_days` | DECIMAL(5,2) | COMPUTED | Tổng công | SUM các cột |

```sql
CREATE TABLE attendance (
    attendance_id     INT PRIMARY KEY AUTO_INCREMENT,
    employee_id       INT NOT NULL,
    month             INT NOT NULL CHECK (month BETWEEN 1 AND 12),
    year              INT NOT NULL,
    regular_days      DECIMAL(5,2) NOT NULL DEFAULT 0,
    sunday_days       DECIMAL(5,2) NOT NULL DEFAULT 0,
    young_tree_days   DECIMAL(5,2) NOT NULL DEFAULT 0,
    hardship_days     DECIMAL(5,2) NOT NULL DEFAULT 0,
    double_cut_days   DECIMAL(5,2) NOT NULL DEFAULT 0,
    double_cut_sunday DECIMAL(5,2) NOT NULL DEFAULT 0,
    care_days         DECIMAL(5,2) NOT NULL DEFAULT 0,
    UNIQUE KEY uk_emp_period (employee_id, month, year),
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);
```

---

### BẢNG 5: `production` — Sản lượng

> **Nguồn Excel:** Sheet `sản lượng`, `MỦ DÂY`, `MỦ SIRUM`
> **Vị trí tham chiếu:** TRẠM 1!H16, K16 | THCKT

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả | Nguồn Excel |
|---|---|---|---|---|---|
| 1 | `production_id` | INT (PK) | NOT NULL | Mã tự động | Auto |
| 2 | `employee_id` | INT (FK) | NOT NULL | Mã nhân viên | Lookup |
| 3 | `month` | INT | NOT NULL | Tháng | 12 |
| 4 | `year` | INT | NOT NULL | Năm | 2025 |
| 5 | `raw_latex_kg` | DECIMAL(10,2) | DEFAULT 0 | Mủ tạp (kg) | TRẠM 1!H16 |
| 6 | `carry_over_kg` | DECIMAL(10,2) | DEFAULT 0 | Mủ truy lĩnh (kg) | TRẠM 1!K16 |
| 7 | `serum_kg` | DECIMAL(10,2) | DEFAULT 0 | Mủ serum (kg) | MỦ SIRUM |
| 8 | `rope_latex_kg` | DECIMAL(10,2) | DEFAULT 0 | Mủ dây (kg) | MỦ DÂY |
| 9 | `tech_grade` | ENUM('A','B','C','D') | NULL | Hạng kỹ thuật | TRẠM 1!N16 |
| 10 | `dry_latex_kg` | DECIMAL(10,2) | COMPUTED | Mủ quy khô | = raw_latex × DRC |
| 11 | `carry_dry_kg` | DECIMAL(10,2) | COMPUTED | Truy lĩnh quy khô | = carry_over × DRC_ref |
| 12 | `total_pay_kg` | DECIMAL(10,2) | COMPUTED | Tổng kg trả lương | = dry + carry_dry |

```sql
CREATE TABLE production (
    production_id   INT PRIMARY KEY AUTO_INCREMENT,
    employee_id     INT NOT NULL,
    month           INT NOT NULL,
    year            INT NOT NULL,
    raw_latex_kg    DECIMAL(10,2) NOT NULL DEFAULT 0,
    carry_over_kg   DECIMAL(10,2) NOT NULL DEFAULT 0,
    serum_kg        DECIMAL(10,2) NOT NULL DEFAULT 0,
    rope_latex_kg   DECIMAL(10,2) NOT NULL DEFAULT 0,
    tech_grade      ENUM('A','B','C','D') NULL,
    UNIQUE KEY uk_emp_period (employee_id, month, year),
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);
```

---

### BẢNG 6: `salary_params` — Hệ số & Tham số chính sách

> **Nguồn Excel:** Các ô tham số nằm rải rác trong nhiều sheet
> **Mục đích:** TẬP TRUNG toàn bộ hệ số vào 1 bảng duy nhất (khắc phục điểm yếu Excel)

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả |
|---|---|---|---|---|
| 1 | `param_id` | INT (PK) | NOT NULL | Mã tự động |
| 2 | `param_code` | VARCHAR(30) | NOT NULL | Mã tham số (unique) |
| 3 | `param_name` | NVARCHAR(100) | NOT NULL | Tên mô tả |
| 4 | `param_value` | DECIMAL(15,4) | NOT NULL | Giá trị |
| 5 | `param_unit` | VARCHAR(20) | NULL | Đơn vị (kip, %, ratio) |
| 6 | `category` | VARCHAR(30) | NOT NULL | Nhóm tham số |
| 7 | `effective_from` | DATE | NOT NULL | Hiệu lực từ |
| 8 | `effective_to` | DATE | NULL | Hết hiệu lực (NULL = còn hiệu lực) |
| 9 | `requires_approval` | BOOLEAN | NOT NULL | Cần duyệt khi thay đổi |
| 10 | `updated_by` | INT | NULL | Người cập nhật cuối |

```sql
CREATE TABLE salary_params (
    param_id          INT PRIMARY KEY AUTO_INCREMENT,
    param_code        VARCHAR(30) NOT NULL UNIQUE,
    param_name        NVARCHAR(100) NOT NULL,
    param_value       DECIMAL(15,4) NOT NULL,
    param_unit        VARCHAR(20) NULL,
    category          VARCHAR(30) NOT NULL,
    effective_from    DATE NOT NULL,
    effective_to      DATE NULL,
    requires_approval BOOLEAN NOT NULL DEFAULT FALSE,
    updated_by        INT NULL
);
```

**Dữ liệu mẫu (trích từ file Excel):**

| param_code | Tên tham số | Giá trị | Đơn vị | Nhóm | Nguồn Excel |
|---|---|---|---|---|---|
| `CARE_RATE` | Lương chăm sóc/công | 25000 | kip/công | ALLOWANCE | TRẠM 1!G16 formula |
| `REGULAR_COEFF` | Hệ số phụ cấp ngày thường | 46.1 | ratio | ALLOWANCE | TRẠM 1!Y16 formula |
| `SUNDAY_COEFF` | Hệ số phụ cấp chủ nhật | 76.9 | ratio | ALLOWANCE | TRẠM 1!Y16 formula |
| `YOUNG_TREE_COEFF` | Hệ số phụ cấp cây non | 30.7 | ratio | ALLOWANCE | TRẠM 1!Y16 formula |
| `HARDSHIP_RATE` | Phụ cấp khộp nặng/công | 20000 | kip/công | ALLOWANCE | TRẠM 1!Y16 formula |
| `DOUBLE_CUT_RATE` | Phụ cấp cạo 2 lát/công | 100000 | kip/công | ALLOWANCE | TRẠM 1!Y16 formula |
| `EXCHANGE_RATE` | Tỷ giá Bath/Kip | 600 | kip/bath | FINANCE | TRẠM 1!E8 |
| `STD_WORK_DAYS` | Công chuẩn tháng | 27 | ngày | POLICY | CÁCH TÍNH LƯƠNG!O11 |
| `ACTUAL_PAY_DAYS` | Ngày trả lương thực | 22 | ngày | POLICY | CÁCH TÍNH LƯƠNG!O11 |
| `BASE_SALARY_360` | Lương bậc chuẩn | 360 | đơn vị | POLICY | CÁCH TÍNH LƯƠNG!O11 |
| `OFFICER_STD_DAYS` | Công chuẩn cán bộ | giá trị P7 | ngày | POLICY | LƯƠNG ĐỘI!P7 |

---

### BẢNG 7: `drc_rates` — Hệ số DRC theo tháng

> **Nguồn Excel:** TRẠM 1!AE5-AE9, THCKT!AN7-AN13
> **Mục đích:** Lưu hệ số quy đổi mủ thực tế → mủ trả lương

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả | Nguồn Excel |
|---|---|---|---|---|---|
| 1 | `drc_id` | INT (PK) | NOT NULL | Mã tự động | Auto |
| 2 | `month` | INT | NOT NULL | Tháng | 12 |
| 3 | `year` | INT | NOT NULL | Năm | 2025 |
| 4 | `station_id` | INT (FK) | NULL | Trạm (null = toàn đội) | TRẠM 1 / TRẠM 2 |
| 5 | `drc_raw_latex` | DECIMAL(8,4) | NOT NULL | DRC mủ tạp | TRẠM 1!AE6 = AE8/AE7 |
| 6 | `drc_reference` | DECIMAL(8,4) | NULL | DRC tham chiếu (truy lĩnh) | TRẠM 1!AE5 |
| 7 | `drc_serum` | DECIMAL(8,4) | NULL | DRC mủ serum | TRẠM 1!AE9 |

```sql
CREATE TABLE drc_rates (
    drc_id         INT PRIMARY KEY AUTO_INCREMENT,
    month          INT NOT NULL,
    year           INT NOT NULL,
    station_id     INT NULL,
    drc_raw_latex  DECIMAL(8,4) NOT NULL,
    drc_reference  DECIMAL(8,4) NULL,
    drc_serum      DECIMAL(8,4) NULL,
    UNIQUE KEY uk_period_station (month, year, station_id),
    FOREIGN KEY (station_id) REFERENCES stations(station_id)
);
```

---

### BẢNG 8: `payroll` — Bảng lương tháng (Header)

> **Nguồn Excel:** TRẠM 1, TRẠM 2, CÁCH TÍNH LƯƠNG, LƯƠNG ĐỘI
> **Mục đích:** Lưu tổng hợp lương từng người theo tháng

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả | Nguồn Excel |
|---|---|---|---|---|---|
| 1 | `payroll_id` | INT (PK) | NOT NULL | Mã tự động | Auto |
| 2 | `employee_id` | INT (FK) | NOT NULL | Mã nhân viên | TRẠM 1!C16 |
| 3 | `month` | INT | NOT NULL | Tháng | 12 |
| 4 | `year` | INT | NOT NULL | Năm | 2025 |
| 5 | `payroll_type` | ENUM | NOT NULL | Loại bảng lương | 'WORKER','OFFICER','LAO_STAFF' |
| 6 | `care_salary` | DECIMAL(15,0) | DEFAULT 0 | Lương chăm sóc | TRẠM 1!G16 |
| 7 | `production_salary` | DECIMAL(15,0) | DEFAULT 0 | Lương sản lượng | TRẠM 1!O16 |
| 8 | `allowance_total` | DECIMAL(15,0) | DEFAULT 0 | Tổng phụ cấp | TRẠM 1!Y16 |
| 9 | `base_salary` | DECIMAL(15,0) | DEFAULT 0 | Lương bậc (cán bộ) | CÁCH TÍNH LƯƠNG!O11 |
| 10 | `performance_salary` | DECIMAL(15,0) | DEFAULT 0 | Lương hiệu quả (CB) | CÁCH TÍNH LƯƠNG!Q11 |
| 11 | `gross_salary` | DECIMAL(15,0) | NOT NULL | Tổng lương trước thuế | TRẠM 1!Z16 |
| 12 | `tax_amount` | DECIMAL(15,0) | DEFAULT 0 | Thuế TNCN | LƯƠNG ĐỘI!J13 |
| 13 | `net_salary` | DECIMAL(15,0) | NOT NULL | Lương thực nhận | TRẠM 1!AA16 / LƯƠNG ĐỘI!K13 |
| 14 | `status` | ENUM | NOT NULL | Trạng thái | 'DRAFT','APPROVED','PAID' |
| 15 | `approved_by` | INT (FK) | NULL | Người duyệt | — |
| 16 | `approved_at` | DATETIME | NULL | Ngày duyệt | — |

```sql
CREATE TABLE payroll (
    payroll_id          INT PRIMARY KEY AUTO_INCREMENT,
    employee_id         INT NOT NULL,
    month               INT NOT NULL,
    year                INT NOT NULL,
    payroll_type        ENUM('WORKER','OFFICER','LAO_STAFF') NOT NULL,
    care_salary         DECIMAL(15,0) NOT NULL DEFAULT 0,
    production_salary   DECIMAL(15,0) NOT NULL DEFAULT 0,
    allowance_total     DECIMAL(15,0) NOT NULL DEFAULT 0,
    base_salary         DECIMAL(15,0) NOT NULL DEFAULT 0,
    performance_salary  DECIMAL(15,0) NOT NULL DEFAULT 0,
    gross_salary        DECIMAL(15,0) NOT NULL,
    tax_amount          DECIMAL(15,0) NOT NULL DEFAULT 0,
    net_salary          DECIMAL(15,0) NOT NULL,
    status              ENUM('DRAFT','APPROVED','PAID') NOT NULL DEFAULT 'DRAFT',
    approved_by         INT NULL,
    approved_at         DATETIME NULL,
    created_at          DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uk_emp_period (employee_id, month, year),
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);
```

---

### BẢNG 9: `payroll_details` — Chi tiết từng khoản lương

> **Nguồn Excel:** Các cột chi tiết trong TRẠM 1/2, CÁCH TÍNH LƯƠNG
> **Mục đích:** Ghi chi tiết từng dòng tiền để truy vết

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả |
|---|---|---|---|---|
| 1 | `detail_id` | INT (PK) | NOT NULL | Mã tự động |
| 2 | `payroll_id` | INT (FK) | NOT NULL | Mã bảng lương |
| 3 | `item_code` | VARCHAR(30) | NOT NULL | Mã khoản lương |
| 4 | `item_name` | NVARCHAR(100) | NOT NULL | Tên khoản |
| 5 | `quantity` | DECIMAL(10,2) | NULL | Số lượng (công, kg) |
| 6 | `rate` | DECIMAL(15,4) | NULL | Đơn giá / Hệ số |
| 7 | `amount` | DECIMAL(15,0) | NOT NULL | Thành tiền |
| 8 | `source_sheet` | VARCHAR(50) | NULL | Sheet gốc trong Excel |
| 9 | `source_cell` | VARCHAR(10) | NULL | Ô gốc trong Excel |

```sql
CREATE TABLE payroll_details (
    detail_id    INT PRIMARY KEY AUTO_INCREMENT,
    payroll_id   INT NOT NULL,
    item_code    VARCHAR(30) NOT NULL,
    item_name    NVARCHAR(100) NOT NULL,
    quantity     DECIMAL(10,2) NULL,
    rate         DECIMAL(15,4) NULL,
    amount       DECIMAL(15,0) NOT NULL,
    source_sheet VARCHAR(50) NULL,
    source_cell  VARCHAR(10) NULL,
    FOREIGN KEY (payroll_id) REFERENCES payroll(payroll_id)
);
```

**Các item_code chuẩn:**

| item_code | Tên khoản | Áp dụng cho | Nguồn |
|---|---|---|---|
| `CARE_SAL` | Lương chăm sóc | WORKER | TRẠM!G16 |
| `PROD_SAL` | Lương sản lượng | WORKER | TRẠM!O16 |
| `ALLOW_REGULAR` | Phụ cấp công thường | WORKER | TRẠM!Y16 (phần 1) |
| `ALLOW_SUNDAY` | Phụ cấp công CN | WORKER | TRẠM!Y16 (phần 2) |
| `ALLOW_YOUNG_TREE` | Phụ cấp cây non | WORKER | TRẠM!Y16 (phần 3) |
| `ALLOW_HARDSHIP` | Phụ cấp khộp nặng | WORKER | TRẠM!Y16 (phần 4) |
| `ALLOW_DOUBLE_CUT` | Phụ cấp cạo 2 lát | WORKER | TRẠM!Y16 (phần 5) |
| `BASE_SAL` | Lương bậc | OFFICER | CTL!O11 |
| `PERF_SAL` | Lương hiệu quả | OFFICER | CTL!Q11 |
| `SUPPLEMENT` | Phụ cấp/Bổ sung | OFFICER | CTL!P11+R11 |
| `TAX_PIT` | Thuế TNCN | LAO_STAFF | LĐ!J13 |

---

### BẢNG 10: `tax_brackets` — Biểu thuế TNCN luỹ tiến

> **Nguồn Excel:** LƯƠNG ĐỘI!J13 (công thức IF lồng nhiều cấp)
> **Giải mã từ:** `IF(I>65M, (I-65M)*25%+10.685M, IF(I>25M, ...))`

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả |
|---|---|---|---|---|
| 1 | `bracket_id` | INT (PK) | NOT NULL | Mã tự động |
| 2 | `min_income` | DECIMAL(15,0) | NOT NULL | Thu nhập tối thiểu |
| 3 | `max_income` | DECIMAL(15,0) | NULL | Thu nhập tối đa (NULL = vô cùng) |
| 4 | `tax_rate` | DECIMAL(5,4) | NOT NULL | Thuế suất |
| 5 | `cumulative_tax` | DECIMAL(15,0) | NOT NULL | Thuế cộng dồn từ mức trước |
| 6 | `effective_from` | DATE | NOT NULL | Hiệu lực từ |

```sql
CREATE TABLE tax_brackets (
    bracket_id     INT PRIMARY KEY AUTO_INCREMENT,
    min_income     DECIMAL(15,0) NOT NULL,
    max_income     DECIMAL(15,0) NULL,
    tax_rate       DECIMAL(5,4) NOT NULL,
    cumulative_tax DECIMAL(15,0) NOT NULL DEFAULT 0,
    effective_from DATE NOT NULL
);

-- Dữ liệu từ LƯƠNG ĐỘI!J13:
INSERT INTO tax_brackets (min_income, max_income, tax_rate, cumulative_tax, effective_from) VALUES
(0,         1300000,   0.0000,  0,        '2025-01-01'),
(1300000,   5000000,   0.0500,  0,        '2025-01-01'),
(5000000,   10000000,  0.1000,  250000,   '2025-01-01'),
(10000000,  20000000,  0.1500,  750000,   '2025-01-01'),
(20000000,  25000000,  0.2000,  2250000,  '2025-01-01'),
(25000000,  65000000,  0.2500,  2685000,  '2025-01-01'),
(65000000,  NULL,      0.3500,  10685000, '2025-01-01');
```

---

### BẢNG 11: `cost_allocation` — Phân toán quỹ lương

> **Nguồn Excel:** Sheet `BẢNG PHÂN TOÁN`
> **Vị trí:** E15, F15-K15 | Cột phân bổ theo bộ phận

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả | Nguồn Excel |
|---|---|---|---|---|---|
| 1 | `allocation_id` | INT (PK) | NOT NULL | Mã tự động | Auto |
| 2 | `month` | INT | NOT NULL | Tháng | 12 |
| 3 | `year` | INT | NOT NULL | Năm | 2025 |
| 4 | `cost_category` | VARCHAR(50) | NOT NULL | Khoản mục chi phí | BẢNG PHÂN TOÁN!A |
| 5 | `total_amount` | DECIMAL(15,0) | NOT NULL | Tổng chi phí gốc | BẢNG PHÂN TOÁN!E15 |
| 6 | `dept_production` | DECIMAL(15,0) | DEFAULT 0 | Bộ phận Sản xuất (30%) | BẢNG PHÂN TOÁN!F15 |
| 7 | `dept_management` | DECIMAL(15,0) | DEFAULT 0 | Bộ phận Quản lý (25%) | BẢNG PHÂN TOÁN!G15 |
| 8 | `dept_business` | DECIMAL(15,0) | DEFAULT 0 | Bộ phận Kinh doanh (20%) | BẢNG PHÂN TOÁN!H15 |
| 9 | `dept_admin` | DECIMAL(15,0) | DEFAULT 0 | Bộ phận Hành chính (10%) | BẢNG PHÂN TOÁN!I15 |
| 10 | `dept_technical` | DECIMAL(15,0) | DEFAULT 0 | Bộ phận Kỹ thuật (10%) | BẢNG PHÂN TOÁN!J15 |
| 11 | `dept_other` | DECIMAL(15,0) | DEFAULT 0 | Bộ phận Khác (5%) | BẢNG PHÂN TOÁN!K15 |

```sql
CREATE TABLE cost_allocation (
    allocation_id    INT PRIMARY KEY AUTO_INCREMENT,
    month            INT NOT NULL,
    year             INT NOT NULL,
    cost_category    VARCHAR(50) NOT NULL,
    total_amount     DECIMAL(15,0) NOT NULL,
    dept_production  DECIMAL(15,0) NOT NULL DEFAULT 0,
    dept_management  DECIMAL(15,0) NOT NULL DEFAULT 0,
    dept_business    DECIMAL(15,0) NOT NULL DEFAULT 0,
    dept_admin       DECIMAL(15,0) NOT NULL DEFAULT 0,
    dept_technical   DECIMAL(15,0) NOT NULL DEFAULT 0,
    dept_other       DECIMAL(15,0) NOT NULL DEFAULT 0
);
```

---

### BẢNG 12: `audit_logs` — Nhật ký thay đổi (MỚI — chưa có trong Excel)

| STT | Tên trường | Kiểu dữ liệu | Nullable | Mô tả |
|---|---|---|---|---|
| 1 | `log_id` | INT (PK) | NOT NULL | Mã tự động |
| 2 | `table_name` | VARCHAR(50) | NOT NULL | Bảng bị thay đổi |
| 3 | `record_id` | INT | NOT NULL | ID bản ghi bị thay đổi |
| 4 | `field_name` | VARCHAR(50) | NOT NULL | Trường bị thay đổi |
| 5 | `old_value` | TEXT | NULL | Giá trị cũ |
| 6 | `new_value` | TEXT | NULL | Giá trị mới |
| 7 | `changed_by` | INT (FK) | NOT NULL | Người thay đổi |
| 8 | `changed_at` | DATETIME | NOT NULL | Thời điểm thay đổi |
| 9 | `reason` | NVARCHAR(200) | NULL | Lý do thay đổi |

```sql
CREATE TABLE audit_logs (
    log_id      INT PRIMARY KEY AUTO_INCREMENT,
    table_name  VARCHAR(50) NOT NULL,
    record_id   INT NOT NULL,
    field_name  VARCHAR(50) NOT NULL,
    old_value   TEXT NULL,
    new_value   TEXT NULL,
    changed_by  INT NOT NULL,
    changed_at  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reason      NVARCHAR(200) NULL,
    INDEX idx_table_record (table_name, record_id),
    INDEX idx_changed_at (changed_at)
);
```

---

## 2.3 ERD — SƠ ĐỒ QUAN HỆ THỰC THỂ

### 2.3.1 Sơ đồ tổng quan (Text-based ERD)

```
================================================================================
                ERD — HỆ THỐNG QUẢN LÝ LƯƠNG ĐỘI 1
================================================================================

  ┌──────────────────┐         ┌──────────────────┐
  │   departments    │         │     stations     │
  │──────────────────│    1:N  │──────────────────│
  │ department_id PK │◄────────│ station_id PK    │
  │ department_name  │         │ station_name     │
  │ department_code  │         │ department_id FK │
  └──────────────────┘         │ grade_a_rate     │
         │                     │ grade_b_rate     │
         │ 1:N                 │ grade_c_rate     │
         │                     │ grade_d_rate     │
         ▼                     └──────────────────┘
  ┌──────────────────┐                │
  │    employees     │                │ 1:N
  │──────────────────│    N:1         │
  │ employee_id PK   │────────────────┘
  │ employee_code    │
  │ full_name        │
  │ station_id FK    │
  │ department_id FK │
  │ employee_type    │
  └──────────────────┘
         │
         │ 1:N (mỗi tháng 1 bản ghi)
         │
    ┌────┴──────────┬──────────┬──────────┐
    │               │          │          │
    ▼               ▼          ▼          ▼
┌──────────┐  ┌───────────┐  ┌───────┐  ┌─────────┐
│attendance│  │production │  │payroll│  │drc_rates│
│──────────│  │───────────│  │───────│  │─────────│
│employee  │  │employee   │  │emp    │  │month    │
│month/year│  │month/year │  │m/y    │  │year     │
│reg_days  │  │raw_latex  │  │care   │  │station  │
│sun_days  │  │carry_over │  │prod   │  │drc_raw  │
│young_tree│  │serum      │  │allow  │  │drc_ref  │
│hardship  │  │tech_grade │  │gross  │  │drc_serum│
│dbl_cut   │  │           │  │tax    │  └─────────┘
└──────────┘  └───────────┘  │net    │
                              │status │
                              └───────┘
                                 │
                                 │ 1:N
                                 ▼
                          ┌────────────────┐
                          │payroll_details │
                          │────────────────│
                          │payroll_id FK   │
                          │item_code       │
                          │item_name       │
                          │quantity         │
                          │rate             │
                          │amount           │
                          └────────────────┘

  ┌───────────────┐     ┌─────────────────┐     ┌──────────────┐
  │salary_params  │     │ tax_brackets    │     │ audit_logs   │
  │───────────────│     │─────────────────│     │──────────────│
  │param_code     │     │ min_income      │     │ table_name   │
  │param_value    │     │ max_income      │     │ record_id    │
  │category       │     │ tax_rate        │     │ field_name   │
  │effective_from │     │ cumulative_tax  │     │ old/new_value│
  └───────────────┘     └─────────────────┘     │ changed_by   │
                                                └──────────────┘

================================================================================
```

### 2.3.2 Mối quan hệ chi tiết

| Quan hệ | Bảng 1 | Bảng 2 | Loại | Mô tả |
|---|---|---|---|---|
| R1 | `departments` | `employees` | 1:N | Một đội có nhiều nhân viên |
| R2 | `departments` | `stations` | 1:N | Một đội có nhiều trạm |
| R3 | `stations` | `employees` | 1:N | Một trạm có nhiều nhân viên |
| R4 | `employees` | `attendance` | 1:N | Mỗi tháng 1 bản ghi chấm công |
| R5 | `employees` | `production` | 1:N | Mỗi tháng 1 bản ghi sản lượng |
| R6 | `employees` | `payroll` | 1:N | Mỗi tháng 1 bản ghi lương |
| R7 | `payroll` | `payroll_details` | 1:N | Mỗi bảng lương có nhiều dòng chi tiết |
| R8 | `stations` | `drc_rates` | 1:N | Mỗi trạm có DRC riêng theo tháng |

---
---

# PHẦN 3: YÊU CẦU CHỨC NĂNG & USE CASES
## (Functional Requirements & Use Cases)

---

## 3.1 DANH SÁCH USE CASE TỔNG QUAN

```
================================================================================
                USE CASE DIAGRAM — BIỂU DIỄN VĂN BẢN
================================================================================

   ACTOR: Kế toán đội                    ACTOR: QLKT (Quản lý kỹ thuật)
   │                                     │
   ├── UC01: Quản lý danh mục nhân viên  │
   ├── UC02: Nhập chấm công hàng ngày    │
   │                                     ├── UC03: Nhập sản lượng
   │                                     ├── UC04: Xếp hạng kỹ thuật
   │
   ├── UC05: Cập nhật hệ số chính sách
   ├── UC06: Tính lương công nhân (TRẠM 1/2)
   ├── UC07: Tính lương cán bộ đội
   ├── UC08: Tính lương nhóm khác (LƯƠNG ĐỘI)
   ├── UC09: Tính thuế TNCN
   ├── UC10: Lập bảng phân toán
   ├── UC11: Xuất phiếu lương
   ├── UC12: Xuất báo cáo tổng hợp
   │
   ACTOR: CEO / Giám đốc              ACTOR: Kế toán công ty
   │                                   │
   ├── UC13: Duyệt bảng lương         ├── UC14: Thanh toán lương
   ├── UC15: Xem báo cáo              ├── UC16: Kiểm toán nội bộ
   │
   ACTOR: Admin hệ thống
   │
   ├── UC17: Quản lý phân quyền
   ├── UC18: Xem nhật ký thay đổi (Audit Log)
================================================================================
```

---

## 3.2 CHI TIẾT TỪNG USE CASE

### UC01: Quản lý danh mục nhân viên

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC01 |
| **Tên** | Quản lý danh mục nhân viên |
| **Actor** | Kế toán đội |
| **Mô tả** | Thêm, sửa, xoá, tìm kiếm nhân viên trong hệ thống |
| **Input Excel** | Sheet `MSNV 2025`, `DS CN MOI UP` |
| **Tiền điều kiện** | Actor đã đăng nhập |
| **Hậu điều kiện** | Dữ liệu nhân viên được cập nhật trong DB |

**Luồng chính (Main Flow):**
1. Actor mở màn hình "Danh mục nhân viên"
2. Hệ thống hiển thị danh sách nhân viên hiện tại
3. Actor chọn Thêm mới / Sửa / Tìm kiếm
4. Hệ thống validate: mã NV không trùng, tên bắt buộc
5. Hệ thống lưu và ghi audit log

**Luồng ngoại lệ:**
- Mã NV đã tồn tại → Báo lỗi "Mã nhân viên đã tồn tại"
- Thiếu trường bắt buộc → Highlight trường lỗi

---

### UC02: Nhập chấm công hàng ngày

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC02 |
| **Tên** | Nhập chấm công hàng ngày |
| **Actor** | Kế toán đội |
| **Mô tả** | Nhập các loại công cho từng nhân viên theo ngày/tháng |
| **Input Excel** | Sheet `CHẤM CÔNG`, `CC NGƯỜI LÀO`, `Công cây non` |
| **Ánh xạ ô Excel** | → TRẠM 1!P16:V16 (các cột công) |

**Luồng dữ liệu (Data Flow):**

```
NHẬP LIỆU:                    XỬ LÝ:                      KẾT QUẢ:
Sheet CHẤM CÔNG         →     Validate công <= 31    →   bảng attendance
  - Công thường (P16)          Validate loại công          - regular_days
  - Công CN (Q16)              Tính tổng công              - sunday_days
  - Cây non (R16)                                          - young_tree_days
  - Khộp nặng (S16)                                        - hardship_days
  - Cạo 2 lát (T16)                                        - double_cut_days
  - Cạo 2 lát CN (U16)                                     - double_cut_sunday
  - Công chăm sóc (F16)                                    - care_days
```

**Ràng buộc nghiệp vụ:**
- Tổng công thường + CN ≤ 31 ngày/tháng
- Công khộp nặng chỉ phát sinh ở người thuộc vùng khộp
- Công cây non chỉ phát sinh ở người có vườn cây năm nhất
- Công cạo 2 lát chỉ phát sinh ở người được phân công

---

### UC03: Nhập sản lượng

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC03 |
| **Tên** | Nhập sản lượng khai thác |
| **Actor** | QLKT |
| **Mô tả** | Nhập kg mủ tạp, mủ serum, mủ dây cho từng công nhân |
| **Input Excel** | Sheet `sản lượng`, `MỦ DÂY`, `MỦ SIRUM` |
| **Ánh xạ ô Excel** | → TRẠM 1!H16 (mủ tạp), K16 (truy lĩnh), N16 (hạng KT) |

**Luồng dữ liệu:**

```
NHẬP LIỆU:                    XỬ LÝ:                       KẾT QUẢ:
Sheet sản lượng         →     Validate kg >= 0        →   bảng production
  - Mủ tạp kg (H16)           Quy khô: J16=H16×AE6        - raw_latex_kg
  - Truy lĩnh (K16)           Truy lĩnh: L16=K16×AE5      - carry_over_kg
  - Mủ serum (MỦ SIRUM)       Tổng: M16=J16+L16           - serum_kg
  - Hạng KT (N16)             Validate hạng A/B/C/D        - tech_grade
```

---

### UC04: Xếp hạng kỹ thuật

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC04 |
| **Tên** | Xếp hạng kỹ thuật A/B/C/D |
| **Actor** | QLKT |
| **Mô tả** | Đánh giá và xếp hạng kỹ thuật cho từng công nhân cuối tháng |
| **Input Excel** | Sheet `sản lượng` → TRẠM 1!N16 |
| **Ý nghĩa** | Hạng KT quyết định trực tiếp đơn giá sản lượng |

**Logic nghiệp vụ:**
- Mỗi công nhân được đánh giá 1 hạng/tháng
- Hạng chỉ nhận 4 giá trị: A, B, C, D
- Nếu không có hạng → Tiền sản lượng = 0
- Hệ số tương ứng theo trạm (xem bảng `stations`)

---

### UC06: Tính lương công nhân khai thác (QUAN TRỌNG NHẤT)

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC06 |
| **Tên** | Tính lương công nhân khai thác |
| **Actor** | Hệ thống (tự động) + Kế toán đội (kiểm tra) |
| **Mô tả** | Tính lương cho công nhân TRẠM 1 và TRẠM 2 |
| **Input Excel** | Sheet `TRẠM 1`, `TRẠM 2` |
| **Output** | bảng payroll + payroll_details |

**Luồng dữ liệu chi tiết:**

```
================================================================================
           UC06: LUỒNG TÍNH LƯƠNG CÔNG NHÂN KHAI THÁC
================================================================================

 BƯỚC 1: LẤY DỮ LIỆU ĐẦU VÀO
 ┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐
 │ employees        │   │ attendance       │   │ production       │
 │ - employee_code  │   │ - care_days      │   │ - raw_latex_kg   │
 │ - station_id     │   │ - regular_days   │   │ - carry_over_kg  │
 │ - employee_type  │   │ - sunday_days    │   │ - tech_grade     │
 └──────────────────┘   │ - young_tree     │   └──────────────────┘
                         │ - hardship       │
                         │ - double_cut     │
                         └──────────────────┘
                                │
                                ▼
 BƯỚC 2: LẤY HỆ SỐ
 ┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐
 │ stations         │   │ drc_rates        │   │ salary_params    │
 │ - grade_X_rate   │   │ - drc_raw_latex  │   │ - CARE_RATE      │
 │   (theo trạm)    │   │ - drc_reference  │   │ - REGULAR_COEFF  │
 └──────────────────┘   │ - drc_serum      │   │ - SUNDAY_COEFF   │
                         └──────────────────┘   │ - v.v...         │
                                                └──────────────────┘
                                │
                                ▼
 BƯỚC 3: TÍNH TOÁN (xem PHẦN 4 để biết công thức)
 ┌──────────────────────────────────────────────────────────────────┐
 │ 3a. Lương chăm sóc    = care_days × 25.000                     │
 │ 3b. Quy khô           = raw_latex × DRC                        │
 │ 3c. Lương sản lượng   = total_pay_kg × grade_rate × tỷ_giá    │
 │ 3d. Phụ cấp           = SUM(từng loại công × hệ số tương ứng) │
 │ 3e. Tổng lương        = 3a + 3c + 3d                           │
 └──────────────────────────────────────────────────────────────────┘
                                │
                                ▼
 BƯỚC 4: LƯU KẾT QUẢ
 ┌──────────────────┐   ┌──────────────────┐
 │ payroll          │   │ payroll_details  │
 │ - gross_salary   │   │ - CARE_SAL       │
 │ - net_salary     │   │ - PROD_SAL       │
 │ - status=DRAFT   │   │ - ALLOW_*        │
 └──────────────────┘   └──────────────────┘
================================================================================
```

---

### UC07: Tính lương cán bộ đội

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC07 |
| **Tên** | Tính lương cán bộ đội |
| **Actor** | Hệ thống + Kế toán đội |
| **Input Excel** | Sheet `CÁCH TÍNH LƯƠNG` |
| **Logic gốc** | O11, Q11, S11 |

**Luồng dữ liệu:**

```
NHẬP LIỆU:                         XỬ LÝ:                        KẾT QUẢ:
CÁCH TÍNH LƯƠNG              →     Tính lương bậc          →    bảng payroll
  - Chức danh / Bậc (G11)          = 360/27×22 = 292.59          - base_salary
  - Sản lượng đơn vị (I12)         Tính tỷ lệ hoàn thành        - performance_salary
  - Kế hoạch (H12)                 = I12/H12                     - gross_salary
  - Hệ số vị trí (G11)             Tính lương hiệu quả
  - Công thực tế (22/27)           = 300×J11×G11/27×22
                                    Tổng = O11+Q11+P11+R11
```

---

### UC08: Tính lương nhóm khác (LƯƠNG ĐỘI)

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC08 |
| **Tên** | Tính lương CB-CNV người Lào, Bảo vệ, Tạp vụ |
| **Actor** | Hệ thống + Kế toán đội |
| **Input Excel** | Sheet `LƯƠNG ĐỘI` |
| **Logic gốc** | I13, J13, K13 |

**Luồng dữ liệu:**

```
NHẬP LIỆU:                     XỬ LÝ:                          KẾT QUẢ:
LƯƠNG ĐỘI                →     Tính lương tháng          →    bảng payroll
  - Công thực tế (E13)         I13 = (F13+H13)/P7 × E13       - gross_salary
  - Lương bậc (F13)            Tính thuế luỹ tiến              - tax_amount
  - Phụ cấp (H13)             J13 = tax_progressive(I13)       - net_salary
  - Công chuẩn P7             Lương net                         = I13 − J13
                                K13 = I13 − J13
```

---

### UC09: Tính thuế TNCN

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC09 |
| **Tên** | Tính thuế thu nhập cá nhân |
| **Actor** | Hệ thống (tự động) |
| **Input Excel** | LƯƠNG ĐỘI!J13 |
| **Logic** | Thuế luỹ tiến 7 bậc |

**Pseudocode (giải mã từ công thức IF lồng của J13):**

```python
def calculate_tax(taxable_income):
    """
    Tính thuế TNCN luỹ tiến
    Nguồn: LƯƠNG ĐỘI!J13
    """

    if taxable_income <= 1_300_000:
        return 0

    elif taxable_income <= 5_000_000:
        return (taxable_income - 1_300_000) * 0.05

    elif taxable_income <= 10_000_000:
        return (taxable_income - 5_000_000) * 0.10 + 250_000

    elif taxable_income <= 20_000_000:
        return (taxable_income - 10_000_000) * 0.15 + 750_000

    elif taxable_income <= 25_000_000:
        return (taxable_income - 20_000_000) * 0.20 + 2_250_000

    elif taxable_income <= 65_000_000:
        return (taxable_income - 25_000_000) * 0.25 + 2_685_000

    else:
        return (taxable_income - 65_000_000) * 0.35 + 10_685_000
```

---

### UC10: Lập bảng phân toán

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC10 |
| **Tên** | Lập bảng phân toán quỹ lương |
| **Actor** | Hệ thống + Kế toán |
| **Input Excel** | Sheet `BẢNG PHÂN TOÁN` |
| **Output** | bảng cost_allocation |

**Luồng dữ liệu:**

```
NHẬP LIỆU:                      XỬ LÝ:                          KẾT QUẢ:
BẢNG PHÂN TOÁN             →    Tổng chi phí gốc          →    cost_allocation
  - Lương CN trực tiếp          E15 = SUM(non-contiguous)       - total_amount
  - Lương sản lượng              Phân bổ theo tỷ lệ:            - dept_production (30%)
  - Phụ cấp nghề                 F15 = ROUND(E15×30%, 0)        - dept_management (25%)
  - Các khoản khác               G15 = ROUND(E15×25%, 0)        - dept_business (20%)
                                 H15 = ROUND(E15×20%, 0)        - dept_admin (10%)
                                 I15 = ROUND(E15×10%, 0)        - dept_technical (10%)
                                 K15 = ROUND(E15×5%, 0)         - dept_other (5%)

VALIDATION: SUM(F:K) = E (Tổng phân bổ = Tổng gốc)
```

---

### UC13: Duyệt bảng lương

| Thuộc tính | Nội dung |
|---|---|
| **Mã** | UC13 |
| **Tên** | Duyệt bảng lương hàng tháng |
| **Actor** | CEO / Giám đốc |
| **Mô tả** | Xem tổng hợp và phê duyệt |

**Luồng chính:**
1. Hệ thống trình bảng lương trạng thái DRAFT
2. CEO xem tổng hợp: tổng quỹ lương, số người, trung bình
3. CEO có thể drill-down xem chi tiết từng người
4. CEO chọn "Duyệt" hoặc "Từ chối" (ghi lý do)
5. Nếu duyệt: status → APPROVED, ghi audit log
6. Nếu từ chối: status → REJECTED, gửi thông báo kế toán

---

## 3.3 MA TRẬN USE CASE vs DATABASE TABLE

| Use Case | employees | attendance | production | salary_params | drc_rates | stations | payroll | payroll_details | tax_brackets | cost_allocation | audit_logs |
|---|---|---|---|---|---|---|---|---|---|---|---|
| UC01 | **CRUD** | — | — | — | — | R | — | — | — | — | **W** |
| UC02 | R | **CRU** | — | — | — | — | — | — | — | — | **W** |
| UC03 | R | — | **CRU** | — | R | — | — | — | — | — | **W** |
| UC04 | R | — | **U** | — | — | — | — | — | — | — | **W** |
| UC05 | — | — | — | **CRU** | **CRU** | **U** | — | — | **CRU** | — | **W** |
| UC06 | R | R | R | R | R | R | **CU** | **C** | — | — | **W** |
| UC07 | R | R | R | R | — | — | **CU** | **C** | — | — | **W** |
| UC08 | R | R | — | R | — | — | **CU** | **C** | R | — | **W** |
| UC09 | — | — | — | — | — | — | **U** | **C** | R | — | — |
| UC10 | — | — | — | — | — | — | R | R | — | **C** | **W** |
| UC13 | — | — | — | — | — | — | **U** | — | — | — | **W** |

> C = Create, R = Read, U = Update, D = Delete, W = Write log

---
---

# PHẦN 4: GIẢI MÃ CÔNG THỨC & LOGIC TÍNH TOÁN
## (Formula & Logic Deep-Dive)

**Đây là phần QUAN TRỌNG NHẤT cho Team Dev.**

---

## 4.1 BẢNG ÁNH XẠ TỔNG QUAN: EXCEL → HỆ THỐNG

### 4.1.1 TRẠM 1 — Lương công nhân khai thác (Sheet 20)

| STT | Nghiệp vụ | Sheet & Ô Excel | Công thức Excel | Logic cho Dev (If-Then-Else) | DB Field |
|---|---|---|---|---|---|
| 1 | **Mã nhân viên** | TRẠM 1!B16 | `=VLOOKUP(C16,'MSNV 2025'!...,col,0)` | `SELECT employee_code FROM employees WHERE full_name = C16` | `employees.employee_code` |
| 2 | **Lương chăm sóc** | TRẠM 1!G16 | `=F16 * 25000` | `care_salary = attendance.care_days × params.CARE_RATE` | `payroll.care_salary` |
| 3 | **Mủ tạp (kg)** | TRẠM 1!H16 | `=VLOOKUP(C16,'sản lượng'!...,35,0)` | `SELECT raw_latex_kg FROM production WHERE employee_id = ? AND month = ? AND year = ?` | `production.raw_latex_kg` |
| 4 | **Mủ quy khô** | TRẠM 1!J16 | `=H16 * $AE$6` | `dry_latex = raw_latex_kg × drc_rates.drc_raw_latex` | Computed |
| 5 | **Mủ truy lĩnh quy khô** | TRẠM 1!L16 | `=K16 * $AE$5` | `carry_dry = carry_over_kg × drc_rates.drc_reference` | Computed |
| 6 | **Tổng mủ trả lương** | TRẠM 1!M16 | `=SUM(J16, L16)` | `total_pay_kg = dry_latex + carry_dry` | Computed |
| 7 | **Hạng kỹ thuật** | TRẠM 1!N16 | `=VLOOKUP(...)` từ sản lượng | `SELECT tech_grade FROM production WHERE ...` | `production.tech_grade` |
| 8 | **Tiền sản lượng** | TRẠM 1!O16 | *(xem giải mã 4.2.1)* | `prod_salary = total_pay_kg × grade_rate × exchange_rate` | `payroll.production_salary` |
| 9 | **Tổng phụ cấp** | TRẠM 1!Y16 | *(xem giải mã 4.2.2)* | `allowance = SUM(từng loại phụ cấp)` | `payroll.allowance_total` |
| 10 | **Tổng lương** | TRẠM 1!Z16 | `=O16 + G16 + Y16` | `gross = prod_salary + care_salary + allowance` | `payroll.gross_salary` |

### 4.1.2 TRẠM 2 — Tương tự TRẠM 1 nhưng hệ số khác

| STT | Điểm khác với TRẠM 1 | TRẠM 1 | TRẠM 2 | Ý nghĩa |
|---|---|---|---|---|
| 1 | Hệ số hạng A | 9.2 | 7.7 | TRẠM 2 đơn giá thấp hơn 1.5 |
| 2 | Hệ số hạng B | 8.9 | 7.4 | Tương tự |
| 3 | Hệ số hạng C | 8.6 | 7.1 | Tương tự |
| 4 | Hệ số hạng D | 8.3 | 6.8 | Tương tự |
| 5 | Cross-ref | — | `AD3 = 'TRẠM 1'!AD4` | TRẠM 2 lấy một số hệ số từ TRẠM 1 |
| 6 | External link | `AE57 = '[7]TRẠM 1'!$AA$60` | `AD76 = '[2]TRẠM 2'!$AA$75` | Cả 2 đều có link ngoài CẦN XOÁ |

> **Lưu ý cho Dev:** Logic tính lương TRẠM 1 và TRẠM 2 là **GIỐNG NHAU**, chỉ khác ở bảng `stations.grade_X_rate`. Viết 1 function duy nhất, truyền station_id vào để lấy hệ số.

### 4.1.3 CÁCH TÍNH LƯƠNG — Lương cán bộ đội (Sheet 30)

| STT | Nghiệp vụ | Sheet & Ô Excel | Công thức Excel | Logic cho Dev | DB Field |
|---|---|---|---|---|---|
| 1 | **Lương bậc** | CTL!O11 | `=360/27*22` | `base_sal = (BASE_360 / STD_DAYS) × ACTUAL_DAYS` | `payroll.base_salary` |
| 2 | **Sản lượng đơn vị** | CTL!I12 | `='TRẠM 2'!M76/1000` | `unit_production = SUM(production WHERE station_id=2) / 1000` | Computed |
| 3 | **Tỷ lệ hoàn thành** | CTL!J12 | `=I12/H12` | `completion_rate = actual_production / planned_production` | Computed |
| 4 | **Lương hiệu quả** | CTL!Q11 | `=(300*J11*G11)/27*22` | `perf_sal = (300 × completion_rate × position_coeff) / STD_DAYS × ACTUAL_DAYS` | `payroll.performance_salary` |
| 5 | **Tổng thu nhập** | CTL!S11 | `=O11+Q11+P11+R11` | `gross = base_sal + perf_sal + supplement + carry_over` | `payroll.gross_salary` |

### 4.1.4 LƯƠNG ĐỘI — CB-CNV người Lào, Bảo vệ (Sheet 32)

| STT | Nghiệp vụ | Sheet & Ô Excel | Công thức Excel | Logic cho Dev | DB Field |
|---|---|---|---|---|---|
| 1 | **Công thực tế** | LĐ!E13 | `=VLOOKUP(C13,'CC NGƯỜI LÀO'!...,34,0)` | `actual_days = attendance.regular_days` | Từ attendance |
| 2 | **Lương bậc** | LĐ!F13 | `=VLOOKUP(C13,'CÁCH TÍNH LƯƠNG'!...,17,0)` | `base = salary_scale lookup` | Từ salary_scale |
| 3 | **Lương tính** | LĐ!I13 | `=(F13+H13)/P7*E13` | `calc_sal = (base + supplement) / std_days × actual_days` | `payroll.gross_salary` |
| 4 | **Thuế TNCN** | LĐ!J13 | `=IF(I13>65M,...IF(I13>25M,...))` | `tax = calculate_tax(calc_sal)` *(xem UC09)* | `payroll.tax_amount` |
| 5 | **Lương net** | LĐ!K13 | `=I13-J13` | `net = calc_sal − tax` | `payroll.net_salary` |

### 4.1.5 BẢNG PHÂN TOÁN — Phân bổ quỹ lương (Sheet 35)

| STT | Nghiệp vụ | Sheet & Ô Excel | Công thức Excel | Logic cho Dev | DB Field |
|---|---|---|---|---|---|
| 1 | **Tổng chi phí** | BPT!E15 | `=SUM(E16:E19,E35,E20,E25,E38,E30)` | `total = SUM(cost_items WHERE category IN (...))` | `cost_allocation.total_amount` |
| 2 | **Sản xuất 30%** | BPT!F15 | `=ROUND(E15*$F$11, 0)` | `prod = ROUND(total × 0.30)` | `cost_allocation.dept_production` |
| 3 | **Quản lý 25%** | BPT!G15 | `=ROUND(E15*$G$11, 0)` | `mgmt = ROUND(total × 0.25)` | `cost_allocation.dept_management` |
| 4 | **Hạng D đơn giá** | BPT!F15 | `=5.2 * J9` | `grade_d_price = 5.2 × base_unit_price` | Computed |
| 5 | **Mủ xirum đơn giá** | BPT!F26 | `=6.1 * J9` | `serum_price = 6.1 × base_unit_price` | Computed |
| 6 | **Lương CN trực tiếp** | BPT!G13 | `=G14+G27+G38` | `direct_labor = prod_sal + allowance + other` | Computed |
| 7 | **Tham chiếu THCKT** | BPT!K12 | `=+THCKT!Z10` | `thckt_ref = query from intermediate calc` | Computed |

---

## 4.2 GIẢI MÃ CÔNG THỨC PHỨC TẠP

### 4.2.1 Giải mã: Tiền sản lượng (TRẠM 1!O16)

**Công thức gốc trong Excel (dạng IF lồng):**

```
O16 = IF(N16="A", M16 * 9.2 * $E$8,
      IF(N16="B", M16 * 8.9 * $E$8,
      IF(N16="C", M16 * 8.6 * $E$8,
      IF(N16="D", M16 * 8.3 * $E$8,
      0))))
```

**Giải mã thành Pseudocode cho Dev:**

```python
def calculate_production_salary(employee, month, year):
    """
    Tính lương sản lượng cho công nhân khai thác

    Nguồn: TRẠM 1!O16 hoặc TRẠM 2!O16
    Phụ thuộc: production, stations, drc_rates, salary_params
    """

    # BƯỚC 1: Lấy sản lượng quy khô
    prod = get_production(employee.id, month, year)
    drc = get_drc_rate(employee.station_id, month, year)

    dry_latex = prod.raw_latex_kg * drc.drc_raw_latex      # J16 = H16 × AE6
    carry_dry = prod.carry_over_kg * drc.drc_reference     # L16 = K16 × AE5
    total_pay_kg = dry_latex + carry_dry                    # M16 = J16 + L16

    # BƯỚC 2: Lấy hệ số hạng kỹ thuật theo TRẠM
    station = get_station(employee.station_id)
    grade = prod.tech_grade  # A, B, C, hoặc D

    if grade == 'A':
        grade_rate = station.grade_a_rate    # TRẠM 1: 9.2 | TRẠM 2: 7.7
    elif grade == 'B':
        grade_rate = station.grade_b_rate    # TRẠM 1: 8.9 | TRẠM 2: 7.4
    elif grade == 'C':
        grade_rate = station.grade_c_rate    # TRẠM 1: 8.6 | TRẠM 2: 7.1
    elif grade == 'D':
        grade_rate = station.grade_d_rate    # TRẠM 1: 8.3 | TRẠM 2: 6.8
    else:
        return 0  # Không có hạng → không có tiền sản lượng

    # BƯỚC 3: Lấy tỷ giá
    exchange_rate = get_param('EXCHANGE_RATE')  # E8 = 600

    # BƯỚC 4: Tính tiền
    production_salary = total_pay_kg * grade_rate * exchange_rate

    return production_salary
```

### 4.2.2 Giải mã: Tổng phụ cấp (TRẠM 1!Y16)

**Công thức gốc trong Excel (rất phức tạp):**

```
Y16 = (P16×46.1 + Q16×76.9 + R16×30.7 + T16×46.1×2 + U16×76.9×2)
      × [hệ_số_tiền_DRC_ở_cụm_AD]
      + S16 × 20.000
      + V16 × 100.000
```

**Giải mã thành Pseudocode cho Dev:**

```python
def calculate_allowance(employee, attendance, month, year):
    """
    Tính tổng phụ cấp nghề cho công nhân khai thác

    Nguồn: TRẠM 1!Y16
    Phụ thuộc: attendance, salary_params
    """

    # === PHẦN 1: Phụ cấp theo hệ số (nhân với hệ số DRC/tiền) ===

    regular_allow    = attendance.regular_days    * 46.1     # Công thường × 46.1
    sunday_allow     = attendance.sunday_days     * 76.9     # Công CN × 76.9
    young_tree_allow = attendance.young_tree_days * 30.7     # Cây non × 30.7
    dbl_cut_regular  = attendance.double_cut_days * 46.1 * 2 # Cạo 2 lát thường × 46.1 × 2
    dbl_cut_sunday   = attendance.double_cut_sunday * 76.9 * 2  # Cạo 2 lát CN × 76.9 × 2

    subtotal_coeff = (regular_allow + sunday_allow + young_tree_allow
                      + dbl_cut_regular + dbl_cut_sunday)

    # Nhân thêm hệ số tiền/DRC từ cụm AD (cần xác nhận giá trị cụ thể)
    ad_factor = get_ad_factor(employee.station_id, month, year)
    part1 = subtotal_coeff * ad_factor

    # === PHẦN 2: Phụ cấp cố định (cộng trực tiếp thành tiền) ===

    hardship_allow   = attendance.hardship_days * 20_000     # Khộp nặng: 20.000 kip/công
    double_cut_bonus = (attendance.double_cut_days
                       + attendance.double_cut_sunday) * 100_000  # Cạo 2 lát: 100.000 kip/công

    part2 = hardship_allow + double_cut_bonus

    # === TỔNG ===
    total_allowance = part1 + part2

    return total_allowance
```

**Đặc biệt lưu ý cho Dev:**
```
RÀNG BUỘC:
  - hardship_days CHỈ phát sinh ở người thuộc vùng khộp nặng
    → IF employee.station.is_hardship_zone == FALSE → hardship_days = 0

  - young_tree_days CHỈ phát sinh ở người có vườn cây năm nhất
    → Cần thêm field hoặc flag để xác định

  - double_cut_days CHỈ phát sinh ở người được phân công cạo 2 lát
    → Cần thêm phân công công việc hàng tháng
```

### 4.2.3 Giải mã: Lương cán bộ đội (CÁCH TÍNH LƯƠNG!Q11)

**Công thức gốc:**

```
O11 = 360 / 27 * 22                    (= 293.33 → lương bậc)
I12 = 'TRẠM 2'!M76 / 1000             (sản lượng đơn vị)
J12 = I12 / H12                        (tỷ lệ hoàn thành)
Q11 = (300 * J11 * G11) / 27 * 22     (lương hiệu quả)
S11 = O11 + Q11 + P11 + R11           (tổng thu nhập)
```

**Giải mã:**

```python
def calculate_officer_salary(officer, month, year):
    """
    Tính lương cán bộ đội

    Nguồn: CÁCH TÍNH LƯƠNG (Sheet 30)
    Khác hoàn toàn với công nhân khai thác
    """

    # BƯỚC 1: Lương bậc theo chức danh
    # O11 = 360/27×22
    base_salary = (360 / 27) * 22  # = 293.33
    # Ý nghĩa: Lương chuẩn 360, chia 27 công chuẩn, nhân 22 công thực trả

    # BƯỚC 2: Tỷ lệ hoàn thành kế hoạch
    # I12 = TRẠM 2!M76/1000 (sản lượng thực hiện của đơn vị)
    actual_production = get_station_total_production(station_id=2) / 1000
    planned_production = officer.planned_target  # H12

    completion_rate = actual_production / planned_production  # J12 = I12/H12

    # BƯỚC 3: Lương hiệu quả
    # Q11 = (300 × J11 × G11) / 27 × 22
    position_coeff = officer.position_coefficient  # G11 — hệ số vị trí

    performance_salary = (300 * completion_rate * position_coeff) / 27 * 22

    # BƯỚC 4: Phụ cấp và truy lĩnh
    supplement = officer.supplement  # P11
    carry_over = officer.carry_over  # R11

    # BƯỚC 5: Tổng thu nhập
    # S11 = O11 + Q11 + P11 + R11
    gross = base_salary + performance_salary + supplement + carry_over

    return gross

    # RÀNG BUỘC:
    # - Nếu sản lượng thực hiện < kế hoạch → completion_rate < 1 → lương hiệu quả GIẢM
    # - Nếu công thực tế < 22 → cả lương bậc và hiệu quả đều giảm theo tỷ lệ
    # - Cán bộ đội chịu ràng buộc ĐỒNG THỜI bởi: ngày công + chức danh + sản lượng đơn vị
```

### 4.2.4 Giải mã: Lương nhóm LƯƠNG ĐỘI (I13, J13, K13)

**Công thức gốc:**

```
E13 = VLOOKUP(C13, 'CC NGƯỜI LÀO'!..., 34, 0)     (công thực tế)
F13 = VLOOKUP(C13, 'CÁCH TÍNH LƯƠNG'!..., 17, 0)  (lương bậc)
I13 = (F13 + H13) / P7 * E13                        (lương tính)
J13 = IF(I13>65M, ..., IF(I13>25M, ...))            (thuế)
K13 = I13 - J13                                      (lương net)
```

**Giải mã:**

```python
def calculate_lao_staff_salary(employee, month, year):
    """
    Tính lương CB-CNV người Lào, Bảo vệ, Tạp vụ

    Nguồn: LƯƠNG ĐỘI (Sheet 32)
    Logic: Lương tháng chia theo công
    """

    # BƯỚC 1: Lấy công thực tế
    actual_days = get_attendance(employee.id, month, year).regular_days  # E13

    # BƯỚC 2: Lấy lương bậc
    base_pay = get_salary_scale(employee.id)  # F13

    # BƯỚC 3: Lấy phụ cấp bổ sung
    supplement = get_supplement(employee.id, month, year)  # H13

    # BƯỚC 4: Lấy công chuẩn nhóm
    std_days = get_param('OFFICER_STD_DAYS')  # P7
    # LƯU Ý: P7 là tham số nhập tay, CẦN GIÁM SÁT
    # Bảo vệ có thể dùng công chuẩn 31 ngày (khác với CB dùng 27)

    # BƯỚC 5: Tính lương
    # I13 = (F13 + H13) / P7 × E13
    calc_salary = (base_pay + supplement) / std_days * actual_days

    # BƯỚC 6: Tính thuế TNCN luỹ tiến
    # J13 = progressive_tax(I13)
    tax = calculate_progressive_tax(calc_salary)
    # (Xem hàm calculate_tax ở UC09)

    # BƯỚC 7: Lương net
    # K13 = I13 − J13
    net_salary = calc_salary - tax

    return {
        'gross': calc_salary,    # I13
        'tax': tax,              # J13
        'net': net_salary        # K13
    }

    # RÀNG BUỘC:
    # - P7 > 0 (không chia cho 0)
    # - actual_days >= 0
    # - Thuế chỉ phát sinh nếu thu nhập > 1.300.000
    # - Bảo vệ có công chuẩn riêng (31 ngày), khác CB thông thường (27 ngày)
```

---

## 4.3 BẢNG THAM CHIẾU NHANH: Ô EXCEL → TRƯỜNG DB

| Sheet | Ô Excel | Công thức ngắn gọn | → Table.Field | Ghi chú |
|---|---|---|---|---|
| TRẠM 1 | B16 | VLOOKUP từ MSNV | employees.employee_code | Lookup |
| TRẠM 1 | C16 | Họ tên | employees.full_name | Nhập liệu |
| TRẠM 1 | E8 | 600 (tỷ giá) | salary_params.EXCHANGE_RATE | Tham số |
| TRẠM 1 | F16 | Công chăm sóc | attendance.care_days | Nhập liệu |
| TRẠM 1 | G16 | F16×25000 | payroll.care_salary | Tính |
| TRẠM 1 | H16 | VLOOKUP sản lượng | production.raw_latex_kg | Lookup |
| TRẠM 1 | J16 | H16×AE6 | Computed (dry_latex) | Tính |
| TRẠM 1 | K16 | Mủ truy lĩnh | production.carry_over_kg | Nhập liệu |
| TRẠM 1 | L16 | K16×AE5 | Computed (carry_dry) | Tính |
| TRẠM 1 | M16 | J16+L16 | Computed (total_pay_kg) | Tính |
| TRẠM 1 | N16 | Hạng KT | production.tech_grade | Lookup |
| TRẠM 1 | O16 | M16×rate×E8 | payroll.production_salary | Tính |
| TRẠM 1 | P16 | Công thường | attendance.regular_days | Nhập liệu |
| TRẠM 1 | Q16 | Công CN | attendance.sunday_days | Nhập liệu |
| TRẠM 1 | R16 | Công cây non | attendance.young_tree_days | Nhập liệu |
| TRẠM 1 | S16 | Công khộp nặng | attendance.hardship_days | Nhập liệu |
| TRẠM 1 | T16 | Cạo 2 lát | attendance.double_cut_days | Nhập liệu |
| TRẠM 1 | U16 | Cạo 2 lát CN | attendance.double_cut_sunday | Nhập liệu |
| TRẠM 1 | V16 | SUM(T16:U16) | Computed | Tính |
| TRẠM 1 | Y16 | Tổng phụ cấp | payroll.allowance_total | Tính |
| TRẠM 1 | Z16 | O16+G16+Y16 | payroll.gross_salary | Tính |
| TRẠM 1 | AA16 | = Z16 | payroll.net_salary | CN không có thuế |
| TRẠM 1 | AE5 | DRC tham chiếu | drc_rates.drc_reference | Tham số |
| TRẠM 1 | AE6 | AE8/AE7 | drc_rates.drc_raw_latex | Tham số |
| TRẠM 1 | AE9 | DRC serum | drc_rates.drc_serum | Tham số |
| CTL | O11 | 360/27×22 | payroll.base_salary | Tính |
| CTL | I12 | TRẠM 2!M76/1000 | Computed (unit_prod) | Lookup |
| CTL | J12 | I12/H12 | Computed (completion) | Tính |
| CTL | Q11 | 300×J11×G11/27×22 | payroll.performance_salary | Tính |
| CTL | S11 | O11+Q11+P11+R11 | payroll.gross_salary | Tính |
| LĐ | E13 | VLOOKUP CC | attendance.regular_days | Lookup |
| LĐ | F13 | VLOOKUP CTL | salary_scale | Lookup |
| LĐ | I13 | (F+H)/P7×E | payroll.gross_salary | Tính |
| LĐ | J13 | IF luỹ tiến | payroll.tax_amount | Tính |
| LĐ | K13 | I13−J13 | payroll.net_salary | Tính |
| LĐ | P7 | Nhập tay | salary_params.OFFICER_STD_DAYS | THAM SỐ KHOÁ |
| BPT | E15 | SUM(non-contig) | cost_allocation.total_amount | Tính |
| BPT | F15 | E15×30% | cost_allocation.dept_production | Tính |
| BPT | G15 | E15×25% | cost_allocation.dept_management | Tính |
| BPT | J9 | Đơn giá cơ bản | salary_params (base_unit) | Tham số |

---
---

# PHẦN 5: TÓM TẮT CHÍNH SÁCH LƯƠNG (CHO CEO)
## (Payroll Policy Summary)

---

## 5.1 TỔNG QUAN CƠ CHẾ TÍNH LƯƠNG ĐỘI 1

```
================================================================================
           CEO SUMMARY: CƠ CHẾ TÍNH LƯƠNG ĐỘI 1
================================================================================

  CÔNG NHÂN KHAI THÁC (TRẠM 1 & TRẠM 2):
  ========================================

  Thu nhập = [1] LƯƠNG CHĂM SÓC  +  [2] LƯƠNG SẢN LƯỢNG  +  [3] PHỤ CẤP NGHỀ

  Trong đó:
  [1] = Số công chăm sóc  ×  25.000 kip/công
  [2] = Kg mủ quy khô  ×  Hệ số hạng KT  ×  Tỷ giá 600
  [3] = Tổng các loại phụ cấp theo công phát sinh


  CÁN BỘ ĐỘI:
  ========================================

  Thu nhập = [1] LƯƠNG BẬC  +  [2] LƯƠNG HIỆU QUẢ  +  [3] PHỤ CẤP  +  [4] TRUY LĨNH

  Trong đó:
  [1] = Lương chuẩn theo chức danh (292.59)
  [2] = 300  ×  Tỷ lệ hoàn thành KH  ×  Hệ số vị trí  /  27  ×  22
  [3] = Phụ cấp cố định
  [4] = Truy lĩnh tháng trước (nếu có)


  NHÓM NGƯỜI LÀO / BẢO VỆ / TẠP VỤ:
  ========================================

  Lương net = (Lương bậc + Phụ cấp) / Công chuẩn × Công thực tế  −  Thuế TNCN

================================================================================
```

---

## 5.2 CHÍNH SÁCH PHỤ CẤP

| STT | Loại phụ cấp | Đơn giá / Hệ số | Điều kiện | Áp dụng cho |
|---|---|---|---|---|
| 1 | **Công thường** | × 46.1 (nhân hệ số DRC/tiền) | Có công khai thác ngày thường | Tất cả CN |
| 2 | **Công chủ nhật** | × 76.9 (nhân hệ số DRC/tiền) | Có công khai thác ngày CN | Tất cả CN |
| 3 | **Cây non** | × 30.7 (nhân hệ số DRC/tiền) | Người có vườn cây năm nhất | Một số CN |
| 4 | **Khộp nặng** | 20.000 kip/công | Vùng rừng khộp nặng | CN vùng khộp |
| 5 | **Cạo 2 lát thường** | × 46.1 × 2 (nhân DRC) + 100.000/công | Phân công cạo 2 lát ngày thường | CN được phân công |
| 6 | **Cạo 2 lát CN** | × 76.9 × 2 (nhân DRC) + 100.000/công | Phân công cạo 2 lát ngày CN | CN được phân công |
| 7 | **Lương chăm sóc** | 25.000 kip/công | Có công chăm sóc | Tất cả CN |

---

## 5.3 CHÍNH SÁCH HẠNG KỸ THUẬT & ĐƠN GIÁ

### Bảng đơn giá sản lượng theo hạng và trạm:

| Hạng KT | TRẠM 1 | TRẠM 2 | BẢNG PHÂN TOÁN (chuẩn) | Chênh lệch T1−T2 |
|---|---|---|---|---|
| **A** | 9.2 | 7.7 | — | 1.5 |
| **B** | 8.9 | 7.4 | — | 1.5 |
| **C** | 8.6 | 7.1 | — | 1.5 |
| **D** | 8.3 | 6.8 | 5.2 | 1.5 |

**Ý nghĩa cho CEO:**
- Công ty đang phân biệt đơn giá theo vùng/trạm
- TRẠM 1 được hưởng đơn giá cao hơn TRẠM 2 (chênh 1.5 đơn vị mỗi hạng)
- Đây là cơ chế bù đắp điều kiện lao động khác nhau giữa các trạm
- Hệ số này không chỉ là phụ cấp riêng mà được mã hoá vào đơn giá sản lượng

---

## 5.4 CHÍNH SÁCH THUẾ TNCN

| Bậc | Thu nhập từ | Thu nhập đến | Thuế suất | Thuế cộng dồn |
|---|---|---|---|---|
| Miễn thuế | 0 | 1.300.000 | 0% | 0 |
| Bậc 1 | 1.300.000 | 5.000.000 | 5% | 0 |
| Bậc 2 | 5.000.000 | 10.000.000 | 10% | 250.000 |
| Bậc 3 | 10.000.000 | 20.000.000 | 15% | 750.000 |
| Bậc 4 | 20.000.000 | 25.000.000 | 20% | 2.250.000 |
| Bậc 5 | 25.000.000 | 65.000.000 | 25% | 2.685.000 |
| Bậc 6 | 65.000.000 | Không giới hạn | 35% | 10.685.000 |

**Lưu ý:** Chỉ áp dụng cho nhóm LƯƠNG ĐỘI (cán bộ, bảo vệ, tạp vụ). Công nhân khai thác hiện tại chưa thấy logic trừ thuế trong sheet TRẠM 1/2.

---

## 5.5 CHÍNH SÁCH DRC & QUY KHÔ

| Tham số | Ý nghĩa | Cách tính | Tác động |
|---|---|---|---|
| **DRC mủ tạp** | Hệ số quy đổi mủ tươi → mủ khô | = Mủ quy khô / Mủ tạp (AE8/AE7) | Toàn bộ lương sản lượng |
| **DRC tham chiếu** | DRC dùng để tính truy lĩnh | Lấy từ tháng trước (AE5) | Phần truy lĩnh |
| **DRC mủ serum** | Hệ số riêng cho mủ serum | Từ sheet MỦ SIRUM (AE9) | Phần serum |

**Ý nghĩa cho CEO:**
- Công ty không trả theo kg mủ tươi mà theo "kg quy lương" sau quy đổi DRC
- Cùng kg thực tế, nếu DRC thấp thì tiền thấp hơn
- DRC là tham số KỸ THUẬT do bộ phận kỹ thuật cung cấp, ảnh hưởng TRỰC TIẾP đến lương

---

## 5.6 CHÍNH SÁCH TỶ GIÁ

| Tham số | Giá trị hiện tại | Vị trí Excel | Tác động |
|---|---|---|---|
| Tỷ giá Bath/Kip | 600 | TRẠM 1!E8 | Toàn bộ tiền sản lượng |

**Ý nghĩa:** Nếu tỷ giá thay đổi cuối tháng, toàn bộ cột tiền sản lượng thay đổi đồng loạt.

---

## 5.7 CHÍNH SÁCH PHÂN TOÁN QUỸ LƯƠNG

| Bộ phận | Tỷ lệ | Ý nghĩa |
|---|---|---|
| Sản xuất | 30% | Chi phí lương phân bổ cho sản xuất |
| Quản lý | 25% | Chi phí lương phân bổ cho quản lý |
| Kinh doanh | 20% | Chi phí lương phân bổ cho kinh doanh |
| Hành chính | 10% | Chi phí lương phân bổ cho hành chính |
| Kỹ thuật | 10% | Chi phí lương phân bổ cho kỹ thuật |
| Khác | 5% | Chi phí lương phân bổ cho mục khác |
| **TỔNG** | **100%** | |

---

## 5.8 CÁC THAM SỐ CEO CẦN GIÁM SÁT

| Tham số | Vị trí Excel | Mức độ quan trọng | Lý do |
|---|---|---|---|
| **P7** (Công chuẩn LƯƠNG ĐỘI) | LƯƠNG ĐỘI!P7 | **NGHIÊM TRỌNG** | Thay đổi P7 = thay đổi lương TOÀN BỘ nhóm |
| **E8** (Tỷ giá Bath/Kip) | TRẠM 1!E8 | **CAO** | Thay đổi = toàn bộ tiền sản lượng thay đổi |
| **AE6** (DRC mủ tạp) | TRẠM 1!AE6 | **CAO** | Thay đổi = sản lượng quy khô thay đổi |
| **Hạng KT** | sản lượng!N16 | **TRUNG BÌNH** | QLKT đánh giá sai hạng = tiền sai |
| **Tỷ lệ phân toán** | BPT!F11:K11 | **TRUNG BÌNH** | Thay đổi = cấu trúc chi phí kế toán đổi |

---
---

# PHỤ LỤC

---

## PHỤ LỤC A: DANH SÁCH 59 SHEETS TRONG WORKBOOK

```
NHÓM 1 — DANH MỤC NHÂN SỰ (2 sheets):
  - MSNV 2025
  - DS CN MOI UP

NHÓM 2 — CHẤM CÔNG (5 sheets):
  - CHẤM CÔNG
  - CHẤM CÔNG Axit
  - Công cây non / cây non
  - Chấm công CB ĐỘI 1
  - CC NGƯỜI LÀO

NHÓM 3 — SẢN LƯỢNG & QUY KHÔ (6 sheets):
  - sản lượng
  - MỦ DÂY
  - MỦ SIRUM
  - MỦ XIRUM
  - THCKT
  - bảng kê công

NHÓM 4 — BẢNG TÍNH LƯƠNG THEO ĐƠN VỊ (9+ sheets):
  - TRẠM 1 *** (Sheet 20 — TRỌNG TÂM)
  - TRẠM 2 *** (Sheet 29 — TRỌNG TÂM)
  - Đội 3 / Đ3-TC / Đ3-XC
  - Đội 4
  - Đội 5 / Đội 5-KV71ha
  - Đội 6

NHÓM 5 — BÁO CÁO ĐẦU RA (4 sheets):
  - CÁCH TÍNH LƯƠNG *** (Sheet 30)
  - LƯƠNG ĐỘI *** (Sheet 32)
  - BẢNG PHÂN TOÁN *** (Sheet 35)
  - BẢNG PHÂN TOÁN (2) (Sheet 38)

NHÓM 6 — SHEETS KHÁC / PHỤ TRỢ (~33 sheets):
  - PLC, PLC (2) — có 100+ lỗi #REF!
  - Các sheet trung gian, backup, test
```

---

## PHỤ LỤC B: DANH SÁCH LỖI CẦN SỬA TRƯỚC KHI TRIỂN KHAI

| STT | Vị trí | Lỗi | Mức độ | Hành động |
|---|---|---|---|---|
| 1 | PLC (2) | 100+ lỗi #REF! | NGHIÊM TRỌNG | Xoá hoặc sửa sheet |
| 2 | BẢNG PHÂN TOÁN!J9 | #REF! | CAO | Tìm cell gốc, sửa công thức |
| 3 | BẢNG PHÂN TOÁN!I14 | #REF! | CAO | Tìm cell gốc, sửa công thức |
| 4 | CÁCH TÍNH LƯƠNG (nhiều ô) | #REF!−'BẢNG PHÂN TOÁN'!#REF! | CAO | Sửa BPT trước, sau đó CTL |
| 5 | TRẠM 1!AE57 | ='[7]TRẠM 1'!$AA$60 | TRUNG BÌNH | Break link hoặc tìm file [7] |
| 6 | TRẠM 1!AE58 | ='[7]TRẠM 2'!$AA$75 | TRUNG BÌNH | Break link |
| 7 | TRẠM 2!AD76 | ='[2]TRẠM 2'!$AA$75 | TRUNG BÌNH | Break link hoặc tìm file [2] |
| 8 | TRẠM 2!AD79 | ='[2]TRẠM 2'!$O$75 | TRUNG BÌNH | Break link |

---

## PHỤ LỤC C: THUẬT NGỮ (Glossary)

| Thuật ngữ | Tiếng Việt | Ý nghĩa trong hệ thống |
|---|---|---|
| **DRC** | Dry Rubber Content | Hệ số quy đổi mủ tươi sang mủ khô, quyết định sản lượng trả lương |
| **Hạng KT** | Hạng Kỹ Thuật | Xếp loại A/B/C/D, quyết định đơn giá sản lượng |
| **Khộp nặng** | Rừng khộp nặng | Vùng địa bàn khó khăn, được hưởng phụ cấp thêm 20.000 kip/công |
| **Cây non** | Vườn cây năm nhất | Vườn cao su mới trồng, công nhân được hệ số phụ cấp riêng (30.7) |
| **Cạo 2 lát** | Cạo mủ 2 lần/ngày | Chế độ đặc thù, được nhân gấp đôi hệ số + 100.000 kip/công |
| **Quy khô** | Quy đổi khô | Chuyển từ mủ tươi (kg) sang mủ khô (kg) để tính lương |
| **Truy lĩnh** | Truy lĩnh tháng trước | Sản lượng chưa thanh toán tháng trước, tính bổ sung |
| **Bảng phân toán** | Cost Allocation | Phân bổ quỹ lương vào các khoản mục kế toán |
| **Tỷ giá** | Exchange Rate | Tỷ giá Bath/Kip (hiện tại 600) |
| **P7** | Công chuẩn nhóm | Số công chuẩn dùng để chia lương cho nhóm LƯƠNG ĐỘI |
| **THCKT** | Tổng hợp công kỹ thuật | Sheet trung gian chuẩn hoá dữ liệu đầu vào cho lương |
| **CTL** | Cách Tính Lương | Sheet tính lương cán bộ đội |
| **BPT** | Bảng Phân Toán | Sheet phân bổ quỹ lương theo khoản mục |

---

## PHỤ LỤC D: CHECKLIST TRIỂN KHAI CHO DEV TEAM

### Phase 1: Thiết lập Database (Tuần 1)
```
[ ] Tạo 12 tables theo schema ở trên
[ ] Nhập dữ liệu mẫu salary_params (toàn bộ hệ số)
[ ] Nhập dữ liệu mẫu tax_brackets (7 bậc thuế)
[ ] Nhập dữ liệu stations (TRẠM 1, TRẠM 2 với hệ số)
[ ] Tạo audit_logs trigger
```

### Phase 2: Import dữ liệu (Tuần 2)
```
[ ] Import employees từ MSNV 2025
[ ] Import attendance tháng 12 từ CHẤM CÔNG
[ ] Import production tháng 12 từ sản lượng
[ ] Import drc_rates tháng 12 từ TRẠM 1!AE5-AE9
[ ] Validate: số bản ghi khớp với Excel
```

### Phase 3: Business Logic (Tuần 3-4)
```
[ ] Code: calculate_production_salary() — UC06
[ ] Code: calculate_allowance() — UC06
[ ] Code: calculate_officer_salary() — UC07
[ ] Code: calculate_lao_staff_salary() — UC08
[ ] Code: calculate_progressive_tax() — UC09
[ ] Code: generate_cost_allocation() — UC10
[ ] Unit test: Kiểm tra kết quả khớp với Excel tháng 12
```

### Phase 4: Testing & UAT (Tuần 5)
```
[ ] So sánh kết quả tính lương với file Excel gốc
[ ] Test boundary: công = 0, sản lượng = 0, không có hạng KT
[ ] Test thuế: mỗi bậc thuế
[ ] Test phân toán: tổng phân bổ = tổng gốc
[ ] UAT với kế toán đội
```

### Phase 5: Go-live (Tuần 6)
```
[ ] CEO duyệt hệ thống
[ ] Đào tạo người dùng
[ ] Chạy song song 1 tháng (Excel + Hệ thống mới)
[ ] Cắt chuyển hoàn toàn
```

---

**— HẾT TÀI LIỆU —**

| Thông tin | Chi tiết |
|---|---|
| **Tổng số trang** | ~30 trang |
| **Tổng số bảng DB** | 12 tables |
| **Tổng số Use Cases** | 18 |
| **Tổng số công thức giải mã** | 25+ công thức |
| **Tổng số hệ số chính sách** | 15+ tham số |
| **Độ ưu tiên đọc** | PHẦN 4 (Công thức) > PHẦN 2 (DB) > PHẦN 3 (UC) > PHẦN 5 (CEO) > PHẦN 1 |

**Tài liệu này được tạo từ phân tích file:**
`2025/tháng 12/Đội 1/LƯƠNG ĐỘI 1 THÁNG 12.2025 2 BẢN.xlsx`

**Ngày tạo:** 13/04/2026
**Phiên bản:** 1.0
