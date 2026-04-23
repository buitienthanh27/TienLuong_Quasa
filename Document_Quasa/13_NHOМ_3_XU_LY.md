# 📋 NHÓM 3: XỬ LÝ (Processing Layer)
## Tài Liệu Chi Tiết - ECOTECH 2A Module

---

## 1. TỔNG QUAN

**Nhóm 3** là lớp xử lý, nơi tính lương thực tế diễn ra. Đây là "trái tim" của hệ thống payroll.

```
Nhóm 3: Xử Lý (Processing)
├─ Tính Lương Chính (Main Calculation - 5 Phases)
├─ Truy Thu & Truy Lãnh (Recovery & Additional Payments)
└─ Cân Đối & Chốt Quỹ (Reconciliation)

Đặc Điểm:
├─ Xử lý logic phức tạp (5 phases)
├─ Dựa vào Nhóm 1 (Master Data) & Nhóm 2 (Transactions)
├─ Sinh ra kết quả chính: Lương ròng (Net Salary)
├─ Cần xác nhận bởi KT trước khi pay
└─ Lưu trữ lịch sử tính lương (audit trail)
```

---

## 2. MODULE: TÍNH LƯƠNG CHÍNH (Main Payroll Calculation)

### 2.1 Định Nghĩa

**Mục Đích:** Tính lương theo sơ đồ 5 phases từ phân tích Excel

**Dữ Liệu Input:**

```
Từ Nhóm 1 (Master Data):
├─ Lương cơ bản (Salary Scale)
├─ Hệ số vườn cây (System Parameters)
└─ Tiêu chuẩn kế toán

Từ Nhóm 2 (Transactions):
├─ Chấm công (Working Days)
├─ Hiệu suất (Grade & Coefficient)
├─ Phụ cấp (Allowances)
├─ Tạm ứng (Advances)
└─ Điều chỉnh bổ công (Care Adjustments)
```

**Database Table (Payroll Results):**

```sql
CREATE TABLE payroll (
  payroll_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month VARCHAR(7),
  
  -- Phase 1: Performance Adjustment
  performance_coef DECIMAL(5,2),
  
  -- Phase 2: Base Salary
  base_salary DECIMAL(12,2),
  
  -- Phase 3: Main Calculation  
  working_days DECIMAL(5,2),
  calculated_salary DECIMAL(12,2),
  
  -- Phase 4: Deductions (Tax + Insurance)
  gross_salary DECIMAL(12,2),
  bhxh_deduction DECIMAL(12,2),
  bhyt_deduction DECIMAL(12,2),
  taxable_income DECIMAL(12,2),
  income_tax DECIMAL(12,2),
  total_deductions DECIMAL(12,2),
  
  -- Phase 5: Adjustments & Net
  allowances DECIMAL(12,2),
  advances DECIMAL(12,2),
  bonuses DECIMAL(12,2),
  adjustments DECIMAL(12,2),
  net_salary DECIMAL(12,2),
  
  -- Allocation (Phase 5)
  cost_center_1 DECIMAL(12,2),
  cost_center_2 DECIMAL(12,2),
  
  -- Metadata
  calculated_date TIMESTAMP,
  calculated_by INT,
  status ENUM('DRAFT', 'CALCULATED', 'VERIFIED', 'APPROVED', 'PAID'),
  
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 2.2 QUY TRÌNH 5 PHASES

```
═════════════════════════════════════════════════════════════════════════════════
                          PAYROLL CALCULATION: 5 PHASES
═════════════════════════════════════════════════════════════════════════════════

PHASE 1: PERFORMANCE ADJUSTMENT (Điều Chỉnh Hiệu Suất)
─────────────────────────────────────────────────────────────────────────────────
Input:
  • Performance Grade (A/B/C/D/E) từ Nhóm 2
  • Sản lượng & chất lượng

Process:
  • Grade A → Coefficient = 1.0
  • Grade B → Coefficient = 0.95
  • Grade C → Coefficient = 0.90
  • Grade D → Coefficient = 0.85 (D/E coefficient cần xác nhận)
  • Grade E → Coefficient = 0.80

Output:
  • performance_coef = (từ bảng trên)

SQL:
  SELECT coef FROM salary_scale WHERE grade = ? AND tram = ?


PHASE 2: BASE SALARY LOOKUP (Lương Cơ Bản)
─────────────────────────────────────────────────────────────────────────────────
Input:
  • MSNV (để lấy tram_id)
  • Grade từ Phase 1
  • System month

Process:
  • VLOOKUP: tram_id + grade → base_salary
  • Check: Có adjustment lương không (từ system_parameters)?
  • Apply effective_date: lương có sự thay đổi tháng nào không?

Output:
  • base_salary = (lương cơ bản theo grade & tram)

SQL:
  SELECT base_salary FROM salary_scale 
  WHERE tram_id = ? AND grade = ? 
  AND effective_date <= STR_TO_DATE(? , '%Y-%m')
  ORDER BY effective_date DESC LIMIT 1


PHASE 3: MAIN CALCULATION FORMULA (Tính Lương Chính)
─────────────────────────────────────────────────────────────────────────────────
Input:
  • base_salary (từ Phase 2)
  • working_days (từ Attendance Nhóm 2)
  • P7 parameter (từ System Parameters)
  • performance_coef (từ Phase 1)
  • allowances (từ Allowances Nhóm 2)

Formula:
  │ calculated_salary = (base_salary + allowances + working_days) / P7 × performance_coef │

Giải Thích:
  • base_salary: Lương cơ bản (25M cho trạm 1 cấp A)
  • allowances: Phụ cấp (nhà ở, xăng xe = 2.7M)
  • working_days: Số công (24 ngày)
  • P7: Tham số khoán (27.0) - CẦN LÀMRÕ MỤC ĐÍCH!
  • performance_coef: Hệ số hiệu suất (1.0 cho Grade A)

Ví Dụ:
  calculated_salary = (25,000,000 + 2,700,000 + 24) / 27.0 × 1.0
                    = 27,700,024 / 27.0 × 1.0
                    = 1,025,926 VND

  ⚠️ KẾT QUẢ LỖI: Quá nhỏ! Công thức không đúng!

Output:
  • calculated_salary = (sanity check needed)


PHASE 4: TAX & INSURANCE DEDUCTIONS (Thuế & Bảo Hiểm)
─────────────────────────────────────────────────────────────────────────────────
Step 4.1: Gross Salary
  gross_salary = calculated_salary

Step 4.2: Insurance Deductions (Khấu Trừ Bảo Hiểm)
  bhxh_deduction = gross_salary × 8%        (Bảo Hiểm Xã Hội)
  bhyt_deduction = gross_salary × 1.5%      (Bảo Hiểm Y Tế)
  total_insurance = bhxh_deduction + bhyt_deduction

Step 4.3: Taxable Income (Thu Nhập Chịu Thuế)
  taxable_income = gross_salary - total_insurance - threshold
  
  Ngưỡng: Tuỳ thuộc vào luật thuế
  (Thường là 11M cho nhân viên độc thân, tuỳ dân tộc & địa phương)

Step 4.4: Progressive Tax - 6 Brackets (Thuế Lũy Tiến 6 Bậc)
  
  Bậc 1: 0       → 5,000,000      @ 5%
  Bậc 2: 5,000,000  → 10,000,000  @ 10%
  Bậc 3: 10,000,000 → 20,000,000  @ 15%
  Bậc 4: 20,000,000 → 25,000,000  @ 20%
  Bậc 5: 25,000,000 → 65,000,000  @ 25%
  Bậc 6: 65,000,000 → ∞            @ 35%

  Algorithm:
    IF taxable_income <= 5M:
      tax = taxable_income × 0.05
    ELSIF taxable_income <= 10M:
      tax = 5M × 0.05 + (taxable_income - 5M) × 0.10
    ... (tương tự cho 4 bậc còn lại)

Step 4.5: Total Deductions
  total_deductions = bhxh_deduction + bhyt_deduction + income_tax

Output:
  • gross_salary
  • bhxh_deduction, bhyt_deduction, income_tax
  • total_deductions


PHASE 5: ADJUSTMENTS, ALLOCATIONS & NET SALARY (Điều Chỉnh & Lương Net)
─────────────────────────────────────────────────────────────────────────────────
Step 5.1: Gather Adjustments
  • advances: từ Advances table (truy thu, tạm ứng)
  • bonuses: từ Care Adjustments table (thưởng, phạt)
  • adjustments: từ Care Adjustments table (sửa lỗi)
  
  total_adjustments = advances + bonuses + adjustments
  (Chú ý: âm/dương)

Step 5.2: Calculate Net Salary
  net_salary = gross_salary - total_deductions + total_adjustments

Step 5.3: Cost Allocation (Phân Bổ Chi Phí)
  • Lương tính toán được phân bổ theo bộ phận
  • department msnv → cost_center
  • Từ accounting_standards table
  
  cost_allocation_rate = cost_center_rate / 100
  allocated_salary = net_salary × cost_allocation_rate
  
  Ghi vào: payroll.cost_center_N (N = cost center ID)

Step 5.4: Final Payroll Record
  • Tất cả các trường điền đầy đủ
  • Status = CALCULATED
  • Lưu vào table payroll

Output:
  net_salary, cost_allocations

═════════════════════════════════════════════════════════════════════════════════
```

### 2.3 Ví Dụ Tính Toán Chi Tiết

```python
# Ví Dụ: Tính lương MSNV 001 tháng 12/2025

# NHẬP LIỆU THÁNG 12/2025
msnv = 1
year_month = '2025-12'

# ------- PHASE 1: PERFORMANCE ADJUSTMENT -------
grade = 'A'  # từ performance table
coef_dict = {'A': 1.0, 'B': 0.95, 'C': 0.90, 'D': 0.85, 'E': 0.80}
performance_coef = coef_dict[grade]  # 1.0
print(f"Phase 1 - Performance Coefficient: {performance_coef}")

# ------- PHASE 2: BASE SALARY -------
tram_id = 1  # từ employees table
base_salary = 25_000_000  # VLOOKUP(grade=A, tram=1) = 25M
print(f"Phase 2 - Base Salary (Grade {grade}, Tram {tram_id}): {base_salary:,.0f} VND")

# ------- PHASE 3: MAIN CALCULATION -------
# Input từ Nhóm 2
working_days = 24  # từ attendance
allowances = 2_700_000  # housing + transport + family
P7 = 27.0  # từ system parameters

# ⚠️ CÔNG THỨC HIỆN TẠI CÓ VẤN ĐỀ!
# calculated_salary = (base_salary + allowances + working_days) / P7 × performance_coef
# Điều này cho kết quả quá nhỏ (1M+)

# GIẢI PHÁP: Cần LÀM RÕ:
# • P7 là gì? (Số ngày trong tháng? Hệ số khoán? Tham số khác?)
# • Công thức đúng là gì?
# • Làm sao từ 25M lương cơ bản → ~19-20M lương net?

# TẠM THỜI GIẢ ĐỊNH:
# calculated_salary = (base_salary + allowances) × (working_days / 26) × performance_coef
#                   = (25M + 2.7M) × (24 / 26) × 1.0
#                   = 27.7M × 0.923
#                   = 25,575,310 VND

calculated_salary = (base_salary + allowances) * (working_days / 26) * performance_coef
print(f"Phase 3 - Calculated Salary: {calculated_salary:,.0f} VND")

# ------- PHASE 4: TAX & INSURANCE -------
gross_salary = calculated_salary

BHXH_RATE = 0.08
BHYT_RATE = 0.015
bhxh_deduction = gross_salary * BHXH_RATE  # 8%
bhyt_deduction = gross_salary * BHYT_RATE  # 1.5%
total_insurance = bhxh_deduction + bhyt_deduction

# Taxable income (cộng BHXH vào)
tax_threshold = 0  # Giả định
taxable_income = gross_salary - total_insurance - tax_threshold

# Progressive Tax
def calculate_progressive_tax(income):
    """Tính thuế lũy tiến 6 bậc"""
    brackets = [
        (5_000_000, 0.05),
        (10_000_000, 0.10),
        (20_000_000, 0.15),
        (25_000_000, 0.20),
        (65_000_000, 0.25),
        (float('inf'), 0.35)
    ]
    
    tax = 0
    prev_bracket = 0
    
    for bracket_limit, rate in brackets:
        if income <= prev_bracket:
            break
        taxable_in_bracket = min(income, bracket_limit) - prev_bracket
        tax += taxable_in_bracket * rate
        prev_bracket = bracket_limit
    
    return tax

income_tax = calculate_progressive_tax(taxable_income)
print(f"Phase 4 - BHXH: {bhxh_deduction:,.0f}, BHYT: {bhyt_deduction:,.0f}, Tax: {income_tax:,.0f}")

total_deductions = bhxh_deduction + bhyt_deduction + income_tax
print(f"Phase 4 - Total Deductions: {total_deductions:,.0f} VND")

# ------- PHASE 5: ADJUSTMENTS & NET SALARY -------
# từ Nhóm 2
advances = -5_000_000  # truy thu
bonuses = 3_500_000    # thưởng + điều chỉnh

total_adjustments = advances + bonuses
net_salary = gross_salary - total_deductions + total_adjustments

print(f"Phase 5 - Adjustments: {total_adjustments:,.0f}")
print(f"Phase 5 - NET SALARY: {net_salary:,.0f} VND")

# Cost Allocation
cost_center_1 = net_salary * (100 / 100)  # 100%
print(f"Phase 5 - Cost Center Allocation: {cost_center_1:,.0f} VND")

print("\n" + "="*70)
print("SUMMARY PAYROLL MSNV 001 - THÁNG 12/2025")
print("="*70)
print(f"Gross Salary        : {gross_salary:>15,.0f} VND")
print(f"BHXH (8%)           : {-bhxh_deduction:>15,.0f} VND")
print(f"BHYT (1.5%)         : {-bhyt_deduction:>15,.0f} VND")
print(f"Income Tax          : {-income_tax:>15,.0f} VND")
print(f"Adjustments         : {total_adjustments:>15,.0f} VND")
print(f"─" * 70)
print(f"NET SALARY (LƯƠNG NET) : {net_salary:>12,.0f} VND")
```

### 2.4 SQL: Tính Toán Tự Động

```sql
-- Procedure để tính lương 1 nhân viên

DELIMITER //

CREATE PROCEDURE CalculatePayroll(
    IN p_msnv INT,
    IN p_year_month VARCHAR(7)
)
BEGIN
    DECLARE v_tram_id INT;
    DECLARE v_grade CHAR(1);
    DECLARE v_performance_coef DECIMAL(5,2);
    DECLARE v_base_salary DECIMAL(12,2);
    DECLARE v_working_days DECIMAL(5,2);
    DECLARE v_allowances DECIMAL(12,2);
    DECLARE v_p7 DECIMAL(5,2);
    DECLARE v_calculated_salary DECIMAL(12,2);
    DECLARE v_bhxh_deduction DECIMAL(12,2);
    DECLARE v_bhyt_deduction DECIMAL(12,2);
    DECLARE v_income_tax DECIMAL(12,2);
    DECLARE v_total_adjustments DECIMAL(12,2);
    DECLARE v_net_salary DECIMAL(12,2);
    
    -- Phase 1: Get performance coefficient
    SELECT COALESCE(coef_dict, 1.0) INTO v_performance_coef
    FROM performance
    WHERE msnv = p_msnv AND year_month = p_year_month;
    
    -- Phase 2: Get base salary
    SELECT e.tram_id INTO v_tram_id FROM employees e WHERE e.msnv = p_msnv;
    SELECT s.base_salary INTO v_base_salary
    FROM salary_scale s
    WHERE s.tram_id = v_tram_id AND s.grade = (
        SELECT grade FROM performance WHERE msnv = p_msnv AND year_month = p_year_month
    )
    LIMIT 1;
    
    -- Phase 3: Main calculation
    SELECT working_days INTO v_working_days FROM attendance 
    WHERE msnv = p_msnv AND year_month = p_year_month;
    
    SELECT SUM(amount) INTO v_allowances FROM allowances_benefits
    WHERE msnv = p_msnv AND year_month = p_year_month AND status = 'APPROVED';
    
    SELECT CAST(param_value AS DECIMAL) INTO v_p7
    FROM system_parameters WHERE param_name = 'P7_ADJUSTMENT' AND active = 1;
    
    SET v_calculated_salary = (v_base_salary + v_allowances) * (v_working_days / 26) * v_performance_coef;
    
    -- Phase 4: Deductions
    SET v_bhxh_deduction = v_calculated_salary * 0.08;
    SET v_bhyt_deduction = v_calculated_salary * 0.015;
    SET v_income_tax = CalculateProgressiveTax(v_calculated_salary - v_bhxh_deduction - v_bhyt_deduction);
    
    -- Phase 5: Adjustments
    SELECT COALESCE(SUM(amount), 0) INTO v_total_adjustments
    FROM (
        SELECT amount FROM advances WHERE msnv = p_msnv AND year_month = p_year_month AND status = 'APPROVED'
        UNION ALL
        SELECT amount FROM care_adjustments WHERE msnv = p_msnv AND year_month = p_year_month AND status = 'APPROVED'
    ) t;
    
    SET v_net_salary = v_calculated_salary - v_bhxh_deduction - v_bhyt_deduction - v_income_tax + v_total_adjustments;
    
    -- Insert into payroll table
    INSERT INTO payroll (
        msnv, year_month, performance_coef, base_salary, working_days, 
        calculated_salary, gross_salary, bhxh_deduction, bhyt_deduction,
        income_tax, total_deductions, net_salary, status, calculated_date, calculated_by
    ) VALUES (
        p_msnv, p_year_month, v_performance_coef, v_base_salary, v_working_days,
        v_calculated_salary, v_calculated_salary, v_bhxh_deduction, v_bhyt_deduction,
        v_income_tax, (v_bhxh_deduction + v_bhyt_deduction + v_income_tax), 
        v_net_salary, 'CALCULATED', NOW(), 1
    );
    
    SELECT 'Payroll calculated successfully' AS message;
END //

DELIMITER ;
```

### 2.5 Test Cases

```python
class TestPayrollCalculation:
    
    def test_phase1_performance_coefficient(self):
        """Test: Hệ số hiệu suất"""
        grades = {'A': 1.0, 'B': 0.95, 'C': 0.90}
        for grade, coef in grades.items():
            result = get_performance_coef(grade)
            assert result == coef

    def test_phase2_base_salary_lookup(self):
        """Test: Lookup lương cơ bản"""
        salary = lookup_base_salary(grade='A', tram=1)
        assert salary == 25_000_000

    def test_phase3_main_calculation(self):
        """Test: Công thức tính lương chính"""
        # Giả định công thức: (base + allowances) * (days/26) * coef
        result = calculate_phase3(
            base_salary=25_000_000,
            allowances=2_700_000,
            working_days=24,
            performance_coef=1.0
        )
        expected = (25_000_000 + 2_700_000) * (24 / 26) * 1.0
        assert result == expected

    def test_phase4_progressive_tax_6_brackets(self):
        """Test: Thuế lũy tiến 6 bậc"""
        cases = [
            (3_000_000, 150_000),      # 3M × 5% = 150K
            (7_000_000, 700_000),      # 5M×5% + 2M×10% = 250K + 200K = 450K (ví dụ)
            (50_000_000, 6_975_000),   # Complex calculation
        ]
        for income, expected_tax in cases:
            result = calculate_tax(income)
            assert abs(result - expected_tax) < 1000  # Allow 1K rounding

    def test_phase5_net_salary_with_adjustments(self):
        """Test: Lương net với điều chỉnh"""
        gross = 25_000_000
        deductions = 4_000_000
        adjustments = 3_500_000
        
        net = calculate_net_salary(gross, deductions, adjustments)
        assert net == 25_000_000 - 4_000_000 + 3_500_000

    def test_full_payroll_calculation(self):
        """Test: Tính lương hoàn toàn"""
        payroll = calculate_full_payroll(
            msnv=1,
            month='2025-12'
        )
        assert payroll['status'] == 'CALCULATED'
        assert payroll['net_salary'] > 0
        assert payroll['net_salary'] <= payroll['gross_salary']
```

---

## 3. MODULE: TRUY THU & TRUY LÃNH (Recovery & Additional Payments)

### 3.1 Định Nghĩa

**Mục Đích:** Xử lý các trường hợp truy thu tiền từ lương trước, hoặc truy lãnh tiền bổ sung

**Trường Hợp:**

```
TRUY THU (Recovery):
├─ Truy thu lương tháng trước (tính sai)
├─ Truy thu tiền tạm ứng
├─ Truy thu lãi (0.5-1% trên công nợ)
└─ Truy thu phạt vượt quá

TRUY LÃNH (Additional):
├─ Bổ sung lương tháng trước (tính thiếu)
├─ Bổ sung thưởng
├─ Bổ sung bhxh (tháng trước chưa đóng)
└─ Các khoản bổ sung khác
```

**Database Table:**

```sql
CREATE TABLE payroll_recovery (
  recovery_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month VARCHAR(7),
  recovery_type ENUM('RECOVERY', 'ADDITIONAL'),
  related_month VARCHAR(7),  -- Tháng liên quan
  amount DECIMAL(12,2),
  reason VARCHAR(255),
  status ENUM('PENDING', 'APPROVED', 'PROCESSED'),
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 3.2 Quy Trình Xử Lý

```
TRUY THU & TRUY LÃNH
═════════════════════════════════════════════════════════════

Step 1: Phát Hiện
├─ Sửa lỗi sau khi tính lương
├─ Audit trail: Nhân viên kế toán phát hiện
└─ Ghi nhận: MSNV | Tháng | Loại | Số tiền | Lý Do

Step 2: Kiểm Tra
├─ Check: Truy thu hay truy lãnh?
├─ Check: Số tiền hợp lý?
├─ Check: Lý do rõ ràng?
└─ Get approval từ quản lý

Step 3: Xử Lý
├─ Nếu RECOVERY: Trừ vào lương tháng hiện tại
├─ Nếu ADDITIONAL: Cộng vào lương tháng hiện tại
└─ Status: PROCESSED

Step 4: Báo Cáo
├─ Lập bảng: Truy thu/truy lãnh tháng N
├─ Tổng số: Recovery N triệu, Additional M triệu
└─ Net: M - N triệu
```

### 3.3 Ví Dụ

```
Tháng 11: Tính lương nhân viên 001 = 20M
Tháng 11: Sau Payment: Phát hiện tính sai (tính thiếu BHYT)
Điều chỉnh: +300K

Tháng 12: Xử lý truy lãnh
├─ related_month = '2025-11'
├─ amount = 300_000
├─ recovery_type = 'ADDITIONAL'
└─ Status: PROCESSED

Khi tính lương tháng 12:
├─ Lương tháng 12 bình thường: 20.5M
├─ + Truy lãnh tháng 11: +0.3M
└─ Total đãy được nhân viên = 20.8M
```

---

## 4. MODULE: CÂN ĐỐI & CHỐT QUỸ (Reconciliation) [NEW]

### 4.1 Định Nghĩa

**Mục Đích:** Kiểm toán, xác nhận khoản tiền trước khi thanh toán lương

**Dữ Liệu Lưu Trữ:**

```
Tháng, Số Nhân Viên, Tổng Lương Gross, Tổng Thuế, Tổng Bảo Hiểm, 
Tổng Lương Net, Chênh Lệch, Trạng Thái
```

**Database Table:**

```sql
CREATE TABLE payroll_reconciliation (
  reconcile_id INT AUTO_INCREMENT PRIMARY KEY,
  year_month VARCHAR(7),
  payroll_count INT,              -- Số nhân viên được tính
  total_gross DECIMAL(14,2),      -- Tổng lương gross
  total_bhxh DECIMAL(14,2),       -- Tổng BHXH
  total_bhyt DECIMAL(14,2),       -- Tổng BHYT
  total_tax DECIMAL(14,2),        -- Tổng thuế TNCN
  total_deductions DECIMAL(14,2), -- Tổng khấu trừ
  total_net DECIMAL(14,2),        -- Tổng lương net
  total_advances DECIMAL(14,2),   -- Tổng truy thu
  total_bonuses DECIMAL(14,2),    -- Tổng thưởng
  variance DECIMAL(14,2),         -- Chênh lệch toán học
  is_balanced BOOLEAN,            -- Cân đối? (variance = 0)
  reconciled_by INT,              -- Người kiểm toán
  reconciled_date TIMESTAMP,      -- Khi nào
  status ENUM('DRAFT', 'RECONCILED', 'APPROVED', 'PAID'),
  notes VARCHAR(500)
);
```

### 4.2 Quy Trình Cân Đối

```
QUY TRÌNH CÂN ĐỐI & CHỐT QUỸ
═════════════════════════════════════════════════════════════

Step 1: Tính Lương Tất Cả Nhân Viên
├─ Chạy procedure: CALCULATE_ALL_EMPLOYEES
├─ Tháng: 12/2025
├─ Số nhân viên: N (vd: 100)
└─ Status: CALCULATED

Step 2: Tổng Hợp Số Liệu
├─ SUM(gross_salary) = Gross Total
├─ SUM(bhxh_deduction) = BHXH Total
├─ SUM(bhyt_deduction) = BHYT Total
├─ SUM(income_tax) = Tax Total
├─ SUM(net_salary) = Net Total
├─ SUM(advances) = Advances Total
└─ SUM(bonuses) = Bonuses Total

Step 3: Kiểm Toán Cân Đối (Audit)

  Công Thức Cân Đối:
  ─────────────────
  Gross - (BHXH + BHYT + Tax) + (Advances + Bonuses) = Net
  
  Variance = |Net - (Gross - BHXH - BHYT - Tax + Advances + Bonuses)|
  
  Nếu Variance ≈ 0: Cân Đối ✓
  Nếu Variance > 0: Chênh Lệch → Tìm lỗi

Step 4: Điều Chỉnh Nếu Cần
├─ Nếu có chênh lệch:
│  ├─ Tìm trong lỗi tính toán
│  ├─ Kiểm tra rounding errors
│  └─ Sửa dữ liệu input (nếu cần)
└─ Lập lại bảng cân đối

Step 5: Phê Duyệt Cân Đối
├─ Người kiểm toán duyệt
├─ Sign off: Reconciled by
├─ Timestamp: Reconciled Date
└─ Status: RECONCILED

Step 6: Chương Trình Thanh Toán
├─ Generate payment file (transfers to bank)
├─ Status: PAID
└─ Lưu audit trail
```

### 4.3 SQL: Tính Tổng Hợp

```sql
-- Procedure tính toán cân đối

DELIMITER //

CREATE PROCEDURE ReconcileMonthlyPayroll(
    IN p_year_month VARCHAR(7)
)
BEGIN
    DECLARE v_payroll_count INT;
    DECLARE v_total_gross DECIMAL(14,2);
    DECLARE v_total_bhxh DECIMAL(14,2);
    DECLARE v_total_bhyt DECIMAL(14,2);
    DECLARE v_total_tax DECIMAL(14,2);
    DECLARE v_total_deductions DECIMAL(14,2);
    DECLARE v_total_net DECIMAL(14,2);
    DECLARE v_total_advances DECIMAL(14,2);
    DECLARE v_total_bonuses DECIMAL(14,2);
    DECLARE v_variance DECIMAL(14,2);
    DECLARE v_is_balanced BOOLEAN;
    
    -- Count payrolls
    SELECT COUNT(*) INTO v_payroll_count FROM payroll 
    WHERE year_month = p_year_month AND status = 'CALCULATED';
    
    -- Sum up all columns
    SELECT 
        SUM(gross_salary),
        SUM(bhxh_deduction),
        SUM(bhyt_deduction),
        SUM(income_tax),
        SUM(total_deductions),
        SUM(net_salary)
    INTO 
        v_total_gross, v_total_bhxh, v_total_bhyt, v_total_tax,
        v_total_deductions, v_total_net
    FROM payroll
    WHERE year_month = p_year_month AND status = 'CALCULATED';
    
    -- Calculate variance
    SET v_variance = ABS(v_total_net - (v_total_gross - v_total_deductions));
    SET v_is_balanced = (v_variance < 100);  -- Allow 100 VND rounding error
    
    -- Insert reconciliation record
    INSERT INTO payroll_reconciliation (
        year_month, payroll_count, total_gross, total_bhxh, total_bhyt,
        total_tax, total_deductions, total_net, variance, is_balanced,
        reconciled_by, reconciled_date, status
    ) VALUES (
        p_year_month, v_payroll_count, v_total_gross, v_total_bhxh, v_total_bhyt,
        v_total_tax, v_total_deductions, v_total_net, v_variance, v_is_balanced,
        1, NOW(), 'RECONCILED'
    );
    
    -- Report
    SELECT CONCAT(
        'Reconciliation complete for ', p_year_month, '. ',
        'Payrolls: ', v_payroll_count, ', ',
        'Total Net: ', FORMAT(v_total_net, 0), ' VND, ',
        'Variance: ', FORMAT(v_variance, 2), ' VND, ',
        IF(v_is_balanced, 'BALANCED ✓', 'UNBALANCED ✗')
    ) AS reconciliation_report;
    
END //

DELIMITER ;
```

### 4.4 Ví Dụ Báo Cáo Cân Đối

```
BẢNG CÂN ĐỐI LƯƠNG - THÁNG 12/2025
═════════════════════════════════════════════

Số Nhân Viên:                100
────────────────────────────────────

TỔNG LOẠI LƯƠNG:
  Tổng Lương Gross         : 2,500,000,000 VND
  Tổng BHXH (8%)           :   200,000,000 VND
  Tổng BHYT (1.5%)         :    37,500,000 VND
  Tổng Thuế TNCN           :   225,000,000 VND
  ─────────────────────────────────────────
  Tổng Khấu Trừ            :   462,500,000 VND
  
  Tổng Truy Thu / Thưởng   :    50,000,000 VND
  ─────────────────────────────────────────
  TỔNG LƯƠNG NET           : 2,087,500,000 VND
  
KIỂM TOÁN CÂN ĐỐI:
  Công Thức:
    2,500M - 462.5M + 50M = 2,087.5M ✓
  
  Chênh Lệch:    0 VND
  Trạng Thái:    CÂN ĐỐI ✓
  
Được Kiểm Toán Bởi: Trương Kim Tuyên
Ngày Kiểm Toán:     10/01/2026
Trạng Thái:         SẴN SÀNG THANH TOÁN
```

---

## 5. CHECKLIST NHÓM 3: XỬ LÝ

**Chuẩn Bị Nhóm 3:**

- [ ] Tính Lương Chính (Main Calculation)
  - [ ] 5 Phase calculation logic hoàn tất
  - [ ] SQL procedure hoạt động đúng
  - [ ] Test cases PASS
  - [ ] Thực hiện tính lương cho tháng thử nghiệm
  
- [ ] Truy Thu & Truy Lãnh (Recovery)
  - [ ] Identify all recovery cases
  - [ ] Process correctly
  - [ ] Audit trail complete
  
- [ ] Cân Đối & Chốt Quỹ (Reconciliation)
  - [ ] Reconciliation logic implemented
  - [ ] Database table created
  - [ ] SQL procedure working
  - [ ] Monthly reconciliation PASSED

---

**📋 NHÓM 3: XỬ LÝ - v1.0**
**Chuẩn Bị Chi Tiết: 10/04/2025**
**Trạng Thái: NEED CLARIFICATION (P7 parameter)**

