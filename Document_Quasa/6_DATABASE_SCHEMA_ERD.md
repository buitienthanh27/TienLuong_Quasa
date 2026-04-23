# 🗄️ DATABASE SCHEMA & ENTITY RELATIONSHIP
## Thiết Kế Cơ Sở Dữ Liệu Cho Hệ Thống Tính Lương

---

## 1. ENTITY RELATIONSHIP DIAGRAM (ERD)

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         DATA MODEL FOR PAYROLL SYSTEM                    │
└─────────────────────────────────────────────────────────────────────────┘

        ┌──────────────┐
        │  Employees   │
        │──────────────│
        │ msnv (PK)    │◄──────────────────┐
        │ name         │                   │
        │ position     │                   │
        │ tram_id (FK) │                   │
        │ active       │                   │
        └──────────────┘                   │
              │                            │
              │ 1:N                        │ 1:N
              ▼                            │
        ┌──────────────┐          ┌────────────────────┐
        │ Attendance   │          │ Performance        │
        │──────────────│          │────────────────────│
        │ att_id (PK)  │          │ perf_id (PK)       │
        │ msnv (FK)    │          │ msnv (FK)          │
        │ date_month   │          │ date_month         │
        │ work_days    │          │ output             │
        │ remarks      │          │ quality_rating(A-E)│
        └──────────────┘          └────────────────────┘
              │                            │
              │ 1:N          Payroll      │ 1:N
              └──────┬────────────────┬───┘
                     ▼ Calculation    ▼
              ┌───────────────────────────────┐
              │      Payroll                  │
              │───────────────────────────────│
              │ payroll_id (PK)               │
              │ msnv (FK) ──────────────┐     │
              │ att_id (FK) ────────┐   │     │
              │ perf_id (FK) ────┐  │   │     │
              │ date_month       │  │   │     │
              │ salary_base (FK) │  │   │     │
              │ salary_calculated│  │   │     │
              │ tax              │  │   │     │
              │ insurance (bhxh) │  │   │     │
              │ net_salary       │  │   │     │
              │ created_date     │  │   │     │
              └───────────────────────┘───────┘
                     │
                     │ 1:N
                     ▼
        ┌─────────────────────────┐
        │  CostAllocation         │
        │─────────────────────────│
        │ allocation_id (PK)      │
        │ payroll_id (FK)         │
        │ dept_id (FK)            │
        │ allocated_amount        │
        └─────────────────────────┘
              │
              │ N:1 
              ▼
        ┌──────────────────────┐
        │  CostCenters         │
        │──────────────────────│
        │ dept_id (PK)         │
        │ dept_name            │
        │ allocation_rate (%)  │
        │ accounting_code      │
        │ active               │
        └──────────────────────┘


        ┌──────────────────────┐
        │  SalaryScale         │  [Reference Table]
        │──────────────────────│
        │ scale_id (PK)        │
        │ grade (A/B/C/D/E)    │
        │ tram (1/2)           │
        │ coefficient          │
        │ description          │
        └──────────────────────┘

        ┌──────────────────────┐
        │  SystemParameters    │  [Configuration]
        │──────────────────────│
        │ param_id (PK)        │
        │ param_name           │
        │ param_value          │
        │ data_type            │
        │ description          │
        │ last_modified        │
        └──────────────────────┘
```

---

## 2. DETAILED TABLE SCHEMAS

### 2.1 Employees Table

```sql
CREATE TABLE employees (
  msnv INT PRIMARY KEY COMMENT 'Mã Số Nhân Viên',
  name VARCHAR(100) NOT NULL COMMENT 'Họ Tên',
  position VARCHAR(50) COMMENT 'Chức Vụ',
  tram_id INT NOT NULL COMMENT 'Tram (1=TRAM1, 2=TRAM2)',
  department VARCHAR(50) COMMENT 'Bộ Phận',
  email VARCHAR(100) COMMENT 'Email',
  phone VARCHAR(20) COMMENT 'Điện Thoại',
  hire_date DATE COMMENT 'Ngày Vào Làm',
  active BOOLEAN DEFAULT 1 COMMENT '1=Hiện Tại, 0=Đã Thôi',
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  
  FOREIGN KEY (tram_id) REFERENCES trams(tram_id),
  INDEX idx_name (name),
  INDEX idx_active (active)
);
```

### 2.2 Attendance Table

```sql
CREATE TABLE attendance (
  att_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT NOT NULL COMMENT 'Mã Nhân Viên',
  year_month DATE NOT NULL COMMENT 'Tháng/Năm (VD: 2025-12-01)',
  work_days DECIMAL(5,2) COMMENT 'Số Công Làm',
  remarks VARCHAR(255) COMMENT 'Ghi Chú',
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  
  FOREIGN KEY (msnv) REFERENCES employees(msnv),
  UNIQUE KEY unique_emp_month (msnv, year_month),
  INDEX idx_month (year_month)
);
```

### 2.3 Performance Table

```sql
CREATE TABLE performance (
  perf_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT NOT NULL COMMENT 'Mã Nhân Viên',
  year_month DATE NOT NULL COMMENT 'Tháng/Năm',
  output DECIMAL(12,2) COMMENT 'Khối Lượng Công Việc',
  quality_rating CHAR(1) COMMENT 'Xếp Loại (A/B/C/D/E)',
  output_adjusted DECIMAL(12,2) COMMENT 'Sản Lượng Sau Điều Chỉnh',
  tram_id INT COMMENT 'TRAM 1 hay 2',
  remarks VARCHAR(255),
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  
  FOREIGN KEY (msnv) REFERENCES employees(msnv),
  FOREIGN KEY (tram_id) REFERENCES trams(tram_id),
  UNIQUE KEY unique_emp_month (msnv, year_month),
  INDEX idx_quality (quality_rating)
);
```

### 2.4 SalaryScale Table

```sql
CREATE TABLE salary_scale (
  scale_id INT AUTO_INCREMENT PRIMARY KEY,
  grade CHAR(1) COMMENT 'Xếp Loại (A/B/C/D/E)',
  tram_id INT COMMENT '1=TRAM1, 2=TRAM2',
  coefficient DECIMAL(5,2) COMMENT 'Hệ Số (VD: 9.2, 7.7)',
  description VARCHAR(255),
  active BOOLEAN DEFAULT 1,
  
  FOREIGN KEY (tram_id) REFERENCES trams(tram_id),
  UNIQUE KEY unique_grade_tram (grade, tram_id),
  INDEX idx_grade (grade)
);

-- INSERT Default Data:
INSERT INTO salary_scale (grade, tram_id, coefficient, description) VALUES
('A', 1, 9.2, 'TRAM1 Grade A'),
('B', 1, 8.9, 'TRAM1 Grade B'),
('C', 1, 8.6, 'TRAM1 Grade C'),
('A', 2, 7.7, 'TRAM2 Grade A'),
('B', 2, 7.4, 'TRAM2 Grade B'),
('C', 2, 7.1, 'TRAM2 Grade C');
```

### 2.5 Payroll Table (Main)

```sql
CREATE TABLE payroll (
  payroll_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT NOT NULL COMMENT 'Mã Nhân Viên',
  year_month DATE NOT NULL COMMENT 'Tháng Lương',
  
  -- Input References
  att_id INT COMMENT 'Ref to attendance',
  perf_id INT COMMENT 'Ref to performance',
  
  -- PHASE 1: Performance Adjusted
  output_adjusted DECIMAL(12,2) COMMENT 'Sản Lượng Điều Chỉnh',
  
  -- PHASE 2: Base Salary
  salary_base DECIMAL(12,2) COMMENT 'Lương Cơ Bản (From CÁCH TÍNH LƯƠNG)',
  
  -- PHASE 3: Main Calculation
  work_days DECIMAL(5,2) COMMENT 'Ngày Công',
  salary_calculated DECIMAL(12,2) COMMENT 'Lương Tính = (Base + Days) / P7 * Coef',
  
  -- PHASE 4: Tax
  tax_amount DECIMAL(12,2) COMMENT 'Thuế TNCN (Lũy Tiến)',
  tax_percent DECIMAL(5,2) COMMENT 'Tỷ Lệ Thuế (%)',
  
  -- PHASE 5: Insurance/Deductions
  bhxh_amount DECIMAL(12,2) COMMENT 'BHXH (8%)',
  bhyt_amount DECIMAL(12,2) COMMENT 'BHYT (1.5%)',
  other_deductions DECIMAL(12,2) COMMENT 'Khấu Trừ Khác',
  
  -- Final
  net_salary DECIMAL(12,2) COMMENT 'Lương Net = Tính - Thuế - Khấu Trừ',
  
  -- Metadata
  status ENUM('DRAFT', 'VERIFIED', 'APPROVED', 'PAID') DEFAULT 'DRAFT',
  created_by INT,
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  
  FOREIGN KEY (msnv) REFERENCES employees(msnv),
  FOREIGN KEY (att_id) REFERENCES attendance(att_id),
  FOREIGN KEY (perf_id) REFERENCES performance(perf_id),
  UNIQUE KEY unique_emp_month (msnv, year_month),
  INDEX idx_month (year_month),
  INDEX idx_status (status)
);
```

### 2.6 CostCenters Table

```sql
CREATE TABLE cost_centers (
  dept_id INT AUTO_INCREMENT PRIMARY KEY,
  dept_name VARCHAR(100) NOT NULL COMMENT 'Tên Bộ Phận',
  allocation_rate DECIMAL(5,4) COMMENT 'Tỷ Lệ Phân Toán (VD: 0.30)',
  accounting_code VARCHAR(20) COMMENT 'Mã Kế Toán',
  description VARCHAR(255),
  active BOOLEAN DEFAULT 1,
  
  UNIQUE KEY unique_dept_name (dept_name),
  INDEX idx_active (active)
);

-- INSERT Default Data:
INSERT INTO cost_centers (dept_name, allocation_rate, accounting_code) VALUES
('Sản Xuất', 0.30, '5210'),
('Quản Lý', 0.25, '5220'),
('Kinh Doanh', 0.20, '5230'),
('Hành Chính', 0.10, '5240'),
('Kỹ Thuật', 0.10, '5250'),
('Khác', 0.05, '5260');
```

### 2.7 CostAllocation Table

```sql
CREATE TABLE cost_allocations (
  allocation_id INT AUTO_INCREMENT PRIMARY KEY,
  payroll_id INT NOT NULL,
  dept_id INT NOT NULL,
  allocated_amount DECIMAL(12,2) COMMENT 'Số Tiền Phân Bổ',
  created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  
  FOREIGN KEY (payroll_id) REFERENCES payroll(payroll_id),
  FOREIGN KEY (dept_id) REFERENCES cost_centers(dept_id),
  INDEX idx_payroll (payroll_id),
  INDEX idx_dept (dept_id)
);
```

### 2.8 SystemParameters Table

```sql
CREATE TABLE system_parameters (
  param_id INT AUTO_INCREMENT PRIMARY KEY,
  param_name VARCHAR(100) UNIQUE NOT NULL,
  param_value VARCHAR(255),
  data_type ENUM('INT', 'DECIMAL', 'STRING', 'BOOLEAN'),
  description VARCHAR(255),
  version INT DEFAULT 1,
  active BOOLEAN DEFAULT 1,
  last_modified TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  
  INDEX idx_name (param_name)
);

-- INSERT Default Parameters:
INSERT INTO system_parameters (param_name, param_value, data_type, description) VALUES
('P7_ADJUSTMENT', '27.0', 'DECIMAL', 'Tham Số Khoán (số chia để tính lương)'),
('BASE_COEFFICIENT', '292.59', 'DECIMAL', 'Hệ Số Khoán Cơ Bản (360/27*22)'),
('BHXH_RATE', '0.08', 'DECIMAL', 'Tỷ Lệ BHXH (8%)'),
('BHYT_RATE', '0.015', 'DECIMAL', 'Tỷ Lệ BHYT (1.5%)'),
('TAX_THRESHOLDS', '5000000,10000000,20000000,25000000,65000000', 'STRING', 'Ngưỡng Thuế Lũy Tiến'),
('TAX_RATES', '0.05,0.10,0.15,0.20,0.25,0.35', 'STRING', 'Tỷ Lệ Thuế Lũy Tiến');
```

### 2.9 AuditLog Table

```sql
CREATE TABLE audit_logs (
  log_id INT AUTO_INCREMENT PRIMARY KEY,
  table_name VARCHAR(50),
  record_id INT,
  action ENUM('INSERT', 'UPDATE', 'DELETE'),
  changed_by INT,
  changed_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  old_values JSON,
  new_values JSON,
  
  INDEX idx_table (table_name),
  INDEX idx_date (changed_date)
);
```

### 2.10 Trams Reference Table

```sql
CREATE TABLE trams (
  tram_id INT PRIMARY KEY,
  tram_name VARCHAR(50) UNIQUE,
  description VARCHAR(255)
);

INSERT INTO trams (tram_id, tram_name) VALUES
(1, 'TRAM1'),
(2, 'TRAM2');
```

---

## 3. CALCULATED FIELDS (Views)

### 3.1 v_PayrollSummary (Monthly Summary)

```sql
CREATE VIEW v_payroll_summary AS
SELECT 
  p.year_month,
  COUNT(DISTINCT p.msnv) as employee_count,
  SUM(p.salary_calculated) as total_salary_calc,
  SUM(p.tax_amount) as total_tax,
  SUM(p.bhxh_amount) as total_bhxh,
  SUM(p.net_salary) as total_net_salary,
  AVG(p.salary_calculated) as avg_salary
FROM payroll p
WHERE p.status IN ('VERIFIED', 'APPROVED')
GROUP BY p.year_month;
```

### 3.2 v_AllocationSummary (Cost Distribution)

```sql
CREATE VIEW v_allocation_summary AS
SELECT 
  ca.payroll_id,
  cc.dept_name,
  cc.allocation_rate,
  SUM(ca.allocated_amount) as total_allocated,
  COUNT(*) as emp_count
FROM cost_allocations ca
JOIN cost_centers cc ON ca.dept_id = cc.dept_id
GROUP BY ca.payroll_id, cc.dept_name;
```

### 3.3 v_EmployeeTaxBracket (Tax Calculation Reference)

```sql
CREATE VIEW v_employee_tax_bracket AS
SELECT 
  p.msnv,
  e.name,
  p.salary_calculated,
  CASE 
    WHEN p.salary_calculated <= 5000000 THEN 'Tier 1: 5%'
    WHEN p.salary_calculated <= 10000000 THEN 'Tier 2: 10%'
    WHEN p.salary_calculated <= 20000000 THEN 'Tier 3: 15%'
    WHEN p.salary_calculated <= 25000000 THEN 'Tier 4: 20%'
    WHEN p.salary_calculated <= 65000000 THEN 'Tier 5: 25%'
    ELSE 'Tier 6: 35%'
  END as tax_bracket,
  p.tax_amount,
  (p.tax_amount / p.salary_calculated * 100) as tax_percent
FROM payroll p
JOIN employees e ON p.msnv = e.msnv;
```

---

## 4. INDEXES FOR PERFORMANCE

```sql
-- Performance tuning indexes
CREATE INDEX idx_payroll_emp_month ON payroll(msnv, year_month);
CREATE INDEX idx_payroll_status_month ON payroll(status, year_month);
CREATE INDEX idx_performance_tram_month ON performance(tram_id, year_month);
CREATE INDEX idx_attendance_month ON attendance(year_month);
CREATE INDEX idx_cost_alloc_dept_payroll ON cost_allocations(dept_id, payroll_id);
```

---

## 5. CONSTRAINTS & DATA INTEGRITY

```sql
-- Check constraint: Net salary >= 0
ALTER TABLE payroll 
ADD CONSTRAINT chk_net_salary_positive 
CHECK (net_salary >= 0);

-- Check constraint: Tax <= 35%
ALTER TABLE payroll 
ADD CONSTRAINT chk_tax_reasonable 
CHECK (tax_percent <= 0.35);

-- Check constraint: Allocation rates sum to 1.0
CREATE TRIGGER validate_allocation_rates
BEFORE INSERT ON cost_centers
FOR EACH ROW
BEGIN
  DECLARE total_rate DECIMAL(5,4);
  SELECT SUM(allocation_rate) INTO total_rate 
  FROM cost_centers 
  WHERE active = 1;
  
  IF ABS((total_rate + NEW.allocation_rate) - 1.0) > 0.01 THEN
    SIGNAL SQLSTATE '45000' 
    SET MESSAGE_TEXT = 'Allocation rates must sum to 100%';
  END IF;
END;
```

---

## 6. SAMPLE DML OPERATIONS

### 6.1 Insert Employee

```sql
INSERT INTO employees (msnv, name, position, tram_id, department)
VALUES (001, 'Nguyễn Văn A', 'Công Nhân', 1, 'Sản Xuất');
```

### 6.2 Insert Attendance

```sql
INSERT INTO attendance (msnv, year_month, work_days)
VALUES (001, '2025-12-01', 27);
```

### 6.3 Insert Performance

```sql
INSERT INTO performance (msnv, year_month, output, quality_rating, tram_id)
VALUES (001, '2025-12-01', 1000, 'A', 1);
```

### 6.4 Calculate & Insert Payroll

```sql
INSERT INTO payroll (
  msnv, year_month, att_id, perf_id,
  output_adjusted, salary_base, work_days,
  salary_calculated, tax_amount, bhxh_amount, net_salary
)
SELECT
  1,
  '2025-12-01',
  a.att_id,
  pf.perf_id,
  pf.output_adjusted,
  ss.scale_id,  -- Will join with salary scale table
  a.work_days,
  (SELECT param_value FROM system_parameters WHERE param_name='P7_ADJUSTMENT') as calc_base,
  ... (tax calculation)
FROM attendance a
JOIN performance pf ON a.msnv = pf.msnv
JOIN salary_scale ss ON pf.quality_rating = ss.grade
WHERE a.msnv = 1 AND a.year_month = '2025-12-01';
```

### 6.5 Allocate Costs

```sql
INSERT INTO cost_allocations (payroll_id, dept_id, allocated_amount)
SELECT
  @payroll_id,
  cc.dept_id,
  p.net_salary * cc.allocation_rate
FROM payroll p
CROSS JOIN cost_centers cc
WHERE p.payroll_id = @payroll_id AND cc.active = 1;
```

---

## 7. MIGRATION STEPS FROM EXCEL

### Step 1: Clean & Normalize Data

```
EXCEL Sheets          →  Database Tables
─────────────────────────────────────
MSNV 2025             →  employees
CHẤM CÔNG             →  attendance
sản lượng             →  performance
CÁCH TÍNH LƯƠNG       →  salary_scale (reference)
LƯƠNG ĐỘI             →  payroll (calculated)
BẢNG PHÂN TOÁN        →  cost_allocations
```

### Step 2: Import Script (Pseudocode)

```python
def migrate_from_excel():
    # 1. Load Excel workbook
    wb = load_workbook('LƯƠNG ĐỘI 1 THÁNG 12.2025.xlsx')
    
    # 2. Import static data
    import_employees(wb['MSNV 2025'])
    import_salary_scale(wb['CÁCH TÍNH LƯƠNG'])
    
    # 3. Import dynamic data
    for month in ['12/2025']:
        import_attendance(wb['CHẤM CÔNG'], month)
        import_performance(wb['sản lượng'], month)
        
        # 4. Calculate & insert payroll
        for employee in db.query(employees):
            calculate_payroll(employee, month)
        
        # 5. Generate allocations
        for payroll in db.query(payroll).filter(month=month):
            allocate_cost(payroll)
        
        # 6. Verify balances
        verify_allocation_sums()

    print("Migration complete!")
```

---

## 8. QUERY EXAMPLES

### 8.1 Monthly Payroll Report

```sql
SELECT 
  p.msnv,
  e.name,
  e.position,
  p.salary_base,
  p.work_days,
  p.salary_calculated,
  p.tax_amount,
  p.bhxh_amount,
  p.net_salary
FROM payroll p
JOIN employees e ON p.msnv = e.msnv
WHERE p.year_month = '2025-12-01'
  AND p.status = 'PAID'
ORDER BY p.msnv;
```

### 8.2 Cost  Allocation by Department

```sql
SELECT 
  cc.dept_name,
  SUM(ca.allocated_amount) as total,
  COUNT(DISTINCT ca.payroll_id) as emp_count,
  AVG(ca.allocated_amount) as avg_allocation
FROM cost_allocations ca
JOIN cost_centers cc ON ca.dept_id = cc.dept_id
WHERE YEAR_MONTH(FROM_UNIXTIME(ca.created_date)) = '2025-12'
GROUP BY cc.dept_id
ORDER BY total DESC;
```

### 8.3 Tax Summary

```sql
SELECT 
  CASE 
    WHEN p.salary_calculated <= 5000000 THEN 'Tier 1: 5%'
    ELSE 'Tier 2+: 10%+'
  END as tax_bracket,
  COUNT(*) as emp_count,
  SUM(p.tax_amount) as total_tax,
  AVG(p.tax_amount) as avg_tax
FROM payroll p
WHERE p.year_month = '2025-12-01'
GROUP BY tax_bracket;
```

---

**DATABASE SCHEMA v1.0**
**Last Updated: 09/04/2025**

