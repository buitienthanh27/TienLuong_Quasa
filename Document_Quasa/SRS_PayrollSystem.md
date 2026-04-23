# 💼 SOFTWARE REQUIREMENTS SPECIFICATION (SRS)
## Hệ Thống Tính Lương - Dựa Trên Phân Tích Excel

---

## 1. TỔNG QUAN HỆ THỐNG

### 1.1 Mục Đích
Tính toán lương tháng cho nhân viên làm việc theo 2 ca: **TRẠM 1** và **TRẠM 2**

### 1.2 Dữ Liệu Đầu Vào (Inputs)
```
┌─────────────────────────────────────────┐
│         DATA SOURCES                    │
├─────────────────────────────────────────┤
│ 1. MSNV 2025 - Danh sách nhân viên      │
│    └─ Mã số NV, Tên, Chức vụ, etc.     │
│ 2. CHẤM CÔNG - Số công/ngày làm việc   │
│ 3. sản lượng - Khối lượng công việc    │
│ 4. CÁCH TÍNH LƯƠNG - Bảng lương cơ bản │
└─────────────────────────────────────────┘
```

### 1.3 Dữ Liệu Đầu Ra (Outputs)
```
┌─────────────────────────────────────────┐
│      PAYROLL REPORT                     │
├─────────────────────────────────────────┤
│ Mã NV | Tên | Lương Cơ Bản | Thuế | Net │
│ ...                                     │
└─────────────────────────────────────────┘
```

---

## 2. ENTITIES & DATA MODELS

### 2.1 Nhân Viên (Employee)

```sql
CREATE TABLE Employees {
  msnv: INTEGER PRIMARY KEY,
  ten: VARCHAR,
  chuc_vu: VARCHAR,
  tram_id: INTEGER (1=TRẠM1, 2=TRẠM2)
}
```

### 2.2 Chấm Công (Attendance)

```sql
CREATE TABLE Attendance {
  msnv: INTEGER,
  nam_thang: DATE,
  so_cong: DECIMAL,
  ghi_chu: VARCHAR
}
```

### 2.3 Sản Lượng (Output/Performance)

```sql
CREATE TABLE Performance {
  msnv: INTEGER,
  nam_thang: DATE,
  san_luong: DECIMAL,
  phan_loai: CHAR (A/B/C/D/E) -- Quality rating
}
```

### 2.4 Bảng Lương (Salary Scale)

```sql
CREATE TABLE SalaryScale {
  phan_loai: CHAR,
  tram: INTEGER (1/2),
  luong_co_ban: DECIMAL,
  he_so_phu_cap: DECIMAL
}
```

### 2.5 Lương (Payroll)

```sql
CREATE TABLE Payroll {
  msnv: INTEGER,
  nam_thang: DATE,
  
  -- Inputs
  luong_co_ban: DECIMAL,
  so_cong: DECIMAL,
  san_luong: DECIMAL,
  
  -- Calculations
  luong_tinh: DECIMAL,
  thue: DECIMAL,
  bhxh: DECIMAL,
  
  -- Output
  luong_net: DECIMAL
}
```

---

## 3. ALGORITHM & BUSINESS LOGIC

### 3.1 PHASE 1: TRẠM 1 & TRẠM 2 - Tính Sản Lượng Điều Chỉnh

**Input:**
- Sản lượng thô từ 'sản lượng' sheet
- Phân loại chất lượng (A, B, C, D, E...)

**Process:**

```
IF phan_loai = "A" THEN
  san_luong_DC = san_luong * 9.2  (TRẠM 1)
  san_luong_DC = san_luong * 7.7  (TRẠM 2)
ELSE IF phan_loai = "B" THEN
  san_luong_DC = san_luong * 8.9  (TRẠM 1)
  san_luong_DC = san_luong * 7.4  (TRẠM 2)
ELSE IF phan_loai = "C" THEN
  san_luong_DC = san_luong * 8.6  (TRẠM 1)
  san_luong_DC = san_luong * 7.1  (TRẠM 2)
...
END IF

san_luong_DC_final = ROUND(san_luong_DC, 0)
```

**Output:**
- `san_luong_dieu_chinh`: Sản lượng sau điều chỉnh theo chất lượng

**Hệ Số (Coefficients):**

| Tram | A | B | C | D | E |
|---|---|---|---|---|---|
| 1 | 9.2 | 8.9 | 8.6 | ? | ? |
| 2 | 7.7 | 7.4 | 7.1 | ? | ? |

---

### 3.2 PHASE 2: CÁCH TÍNH LƯƠNG - Công Thức Cơ Bản

**Input:**
- Sản lượng điều chỉnh
- Bảng lương công khai từ CÁCH TÍNH LƯƠNG sheet

**Key Coefficients:**

```
HE_SO_KHOAN = 360 / 27 * 22 = 292.59
HE_SO_CHIA = 500 (dùng để scale)
THANG_LUONG_COEF = V9 / 500
```

**Process:**

```
Bảng công khai được tính từ:
  - 360 ngày / 27 ngày làm việc × 22 ngày thực tế
  - Chia nhỏ theo so cấp/chức vụ

Công thức: Lương Cơ Bản = 
  (Sản_Lượng × 300) / (27 × 22) 
  = Sản_Lượng × 292.59 / (số công)
```

**Output:**
- `luong_co_ban_tham_chieu`: Lương tham chiếu từ bảng

---

### 3.3 PHASE 3: LƯƠNG ĐỘI - Tính Lương Cuối Cùng

**KEY FORMULA (Cực Kỳ Quan Trọng):**

```
luong_tinh = (luong_co_ban + so_cong) / P7 * luong_he_so

WHERE:
  P7 = 27.0 (có thể điều chỉnh)
  so_cong = số công từ CHẤM CÔNG
  luong_co_ban = từ CÁCH TÍNH LƯƠNG
  luong_he_so = từ employee
```

**Chia Nhỏ (Step by Step):**

```
STEP 1: Lấy bản lương công khai
  luong_co_ban = VLOOKUP(msnv, CÁCH TÍNH LƯƠNG, col_17)

STEP 2: Lấy số công đã làm
  so_cong = VLOOKUP(msnv, CC_NGUOI_LAO,col_34)

STEP 3: Tính tổng input
  tong_input = luong_co_ban + so_cong

STEP 4: Chia cho tham số P7 (khoán)
  after_khoan = tong_input / P7

STEP 5: Nhân với hệ số nhân viên
  luong_tinh = after_khoan * employee_coef

STEP 6: Tính thuế lũy tiến (xem 3.4)
  thue = calc_tax(luong_tinh)

STEP 7: Tính lương net
  luong_net = luong_tinh - thue - bhxh
```

---

### 3.4 PHASE 4: Thuế TNCN (Lũy Tiến)

**Quy Định Thuế Lũy Tiến 6 Mức:**

```python
def calculate_tax(salary):
    """
    Thuế Thu Nhập Cá Nhân - Progressive Tax
    """
    if salary <= 5_000_000:
        return salary * 0.05
    elif salary <= 10_000_000:
        return (salary - 5_000_000) * 0.10 + 250_000
    elif salary <= 20_000_000:
        return (salary - 10_000_000) * 0.15 + 750_000
    elif salary <= 25_000_000:
        return (salary - 20_000_000) * 0.20 + 2_250_000
    elif salary <= 65_000_000:
        return (salary - 25_000_000) * 0.25 + 2_685_000
    else:
        return (salary - 65_000_000) * 0.35 + 10_685_000
```

**Excel Version (IF Statement):**

```
=IF(I13>65000000,
    ((I13-65000000)*0.35+10685000),
    IF(I13>25000000,
        ((I13-25000000)*0.25+2685000),
        IF(I13>20000000,
            ((I13-20000000)*0.20+2250000),
            ...
        )
    )
)
```

---

### 3.5 PHASE 5: Quy Định Khấu Trừ

```python
def calculate_deductions(salary_gross):
    """
    Khấu trừ bảo hiểm xã hội
    """
    bhxh_percent = 0.08        # 8% BHXH
    bhyt_percent = 0.015       # 1.5% BHYT
    bhut_percent = 0.00        # 0% BH Thất nghiệp
    
    total_insurance = salary_gross * (bhxh_percent + bhyt_percent + bhut_percent)
    return total_insurance
```

---

## 4. WORKFLOW & FLOW CHART

### 4.1 Data Flow Diagram

```
┌─────────────────┐
│    MSNV 2025    │
│   (Employee     │
│    Registry)    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐      ┌──────────────┐      ┌──────────────┐
│   CHẤM CÔNG     │      │sản lượng     │      │CÁCH TÍNH     │
│  (Attendance)   │──┬───│(Performance) │──┬───│LƯƠNG(Salary) │
└────────┬────────┘  │   └──────────────┘  │   │(Scale)       │
         │           │                     │   └──────────────┘
         │  ┌────────┴─────────────┐       │         │
         │  ▼                      ▼       ▼         ▼
         └─→ TRẠM 1 & 2 Sheet ────→ LƯƠNG ĐỘI
                (Calculate         (Final
                 Adjusted Output)   Payroll)
                      │                  │
                      │        ┌─────────┴──────────┐
                      ▼        ▼                    ▼
                   BẢNG PHÂN TOÁN            LUONG NET
                   (Cost Distribution)      (Net Salary)
```

### 4.2 Execution Order

```
1. LOAD DATA
   ├─ Employees (MSNV 2025)
   ├─ Attendance (CHẤM CÔNG)
   ├─ Performance (sản lượng)
   └─ Salary Scale (CÁCH TÍNH LƯƠNG)

2. CALCULATE PHASE 1 (TRẠM 1 & TRẠM 2)
   ├─ Get performance rating
   ├─ Calculate adjusted output (formula: output × coef)
   └─ Store adjusted output

3. CALCULATE PHASE 2 (CÁCH TÍNH LƯƠNG)
   ├─ Get base salary from table
   └─ Store reference salary

4. CALCULATE PHASE 3 (LƯƠNG ĐỘI) - MAIN CALCULATION
   ├─ Input: base_salary + attendance_days
   ├─ Divide by P7 parameter (khoan)
   ├─ Multiply by employee coefficient
   └─ Store salary_gross

5. CALCULATE PHASE 4 (TAX)
   ├─ Apply progressive tax formula
   └─ Store tax amount

6. CALCULATE PHASE 5 (DEDUCTIONS)
   ├─ Calculate insurance (BHXH, BHYT)
   └─ Store deduction amount

7. CALCULATE FINAL
   ├─ salary_net = salary_gross - tax - deductions
   └─ Generate report

8. OUTPUT
   └─ Generate Payroll Report
```

---

## 5. KEY PARAMETERS & CONFIGURATION

### 5.1 Tham Số Cơ Bản (Configurable)

| Parameter | Value | Location | Mục Đích |
|---|---|---|---|
| **P7** | 27.0 | LƯƠNG ĐỘI!P7 | Tham số khoán - điều chỉnh tổng lương |
| **HE_SO_KHOAN** | 292.59 | CÁCH TÍNH LƯƠNG!O11 | Hệ số khoán cơ bản (360/27*22) |
| **HE_SO_CHIA** | 500 | CÁCH TÍNH LƯƠNG!V10 | Hệ số chia đơn vị |
| **THUE_RATE[6]** | [5%, 10%, 15%, 20%, 25%, 35%] | LƯƠNG ĐỘI!J13 | Mức thuế lũy tiến |
| **BHXH_RATE** | 8% | (Config) | Tỷ lệ BHXH |
| **BHYT_RATE** | 1.5% | (Config) | Tỷ lệ BHYT |

### 5.2 Hệ Số Chất Lượng (Performance Coefficients)

**TRẠM 1:**
```
A: 9.2, B: 8.9, C: 8.6, D: ?, E: ?
```

**TRẠM 2:**
```
A: 7.7, B: 7.4, C: 7.1, D: ?, E: ?
```

---

## 6. VALIDATION RULES & CONSTRAINTS

```python
# Input Validation
assert salary >= 0, "Lương không được âm"
assert attendance >= 0 and attendance <= 30, "Công phải trong 0-30 ngày"
assert performance_rating in ['A', 'B', 'C', 'D', 'E'], "Xếp loại không hợp lệ"

# Business Logic Validation
assert P7 > 0, "P7 phải dương (không chia cho 0)"
assert salary_gross >= salary_calculated, "Lương gross phải >= tính toán"
assert tax <= salary_gross * 0.35, "Thuế không vượt 35%"
assert net >= 0, "Lương net phải >= 0"

# Cross-validation
assert len(employees) == len(salaries), "Số NV phải khớp số lương"
```

---

## 7. ERROR HANDLING

| Error | Cause | Solution |
|---|---|---|
| `#REF!` Error | Sheet được tham chiếu bị xóa | Sửa VLOOKUP hoặc import data |
| Division by Zero | P7 = 0 | Validate P7 > 0 |
| Negative Salary | Bad input | Validate input >= 0 |
| Missing Employee | MSNV không tìm thấy | Kiểm tra MSNV 2025 sheet |
| Circular Reference | Formula tham chiếu lẫn nhau | Xóa/sửa công thức |

---

## 8. REPORT OUTPUT FORMAT

```
LƯƠNG ĐỘI 1 - THÁNG 12/2025
================================

Mã NV | Tên         | Lương Cơ Bản | Công | Lương Tính | Thuế    | BHXH   | Lương Net
------|-------------|--------------|------|-----------|---------|--------|----------
001   | Nguyễn Văn A| 25,000,000   | 27   | 27,500,000| 2,000,00| 2,20,00| 23,300,000
...

Tổng Cộng:
  - Tổng Lương Cơ Bản: XXX
  - Tổng Thuế: XXX
  - Tổng BHXH: XXX
  - Tổng Lương Net: XXX
```

---

**SRS v1.0 - Updated: 09/04/2025**

