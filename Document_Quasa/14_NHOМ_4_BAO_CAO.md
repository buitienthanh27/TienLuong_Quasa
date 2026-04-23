# 📋 NHÓM 4: BÁO CÁO (Reporting Layer)
## Tài Liệu Chi Tiết - ECOTECH 2A Module

---

## 1. TỔNG QUAN

**Nhóm 4** là lớp báo cáo, nơi xuất dữ liệu lương thành các báo cáo hữu ích cho quản lý, kế toán, nhân viên.

```
Nhóm 4: Báo Cáo (Reporting)
├─ Báo Cáo Lương Chi Tiết (Payroll Details Report)
├─ Báo Cáo Tổng Hợp (Summary Reports)
├─ Báo Cáo Phân Bổ Chi Phí (Allocation Reports)
├─ Báo Cáo Thuế & Bảo Hiểm (Tax & Insurance Reports)
├─ Báo Cáo Nhân Viên (Employee Slips)
└─ Báo Cáo Quản Lý (Management Reports)

Đặc Điểm:
├─ Output: PDF, Excel, JSON
├─ Dựa vào Nhóm 3 (Payroll Results)
├─ Nhiều loại báo cáo cho nhiều người dùng
├─ Định dạng: nhiều ngôn ngữ (VI/EN)
└─ Lưu trữ lịch sử (audit trail)
```

---

## 2. MODULE: BÁO CÁO LƯƠNG CHI TIẾT (Payroll Details Report)

### 2.1 Định Nghĩa

**Mục Đích:** Báo cáo chi tiết lương của từng nhân viên

**Dữ Liệu Báo Cáo:**

```
THÁNG 12/2025:
├─ MSNV, Tên, Chức Vụ, Bộ Phận
├─ Lương Cơ Bản, Hệ Số Hiệu Suất
├─ Phụ Cấp Chi Tiết
├─ Số Công, Ngày Nghỉ
├─ Lương Gross, Khấu Trừ Chi Tiết
├─ Thuế từng bậc
├─ Bảo Hiểm (BHXH, BHYT)
├─ Truy Thu, Thưởng, Điều Chỉnh
└─ Lương NET
```

**Database View:**

```sql
CREATE VIEW v_payroll_detail AS
SELECT 
    p.payroll_id,
    p.msnv,
    e.name,
    e.position,
    e.department,
    p.year_month,
    p.base_salary,
    p.performance_coef,
    p.working_days,
    p.gross_salary,
    p.bhxh_deduction,
    p.bhyt_deduction,
    p.income_tax,
    p.total_deductions,
    p.net_salary,
    p.status,
    p.calculated_date
FROM payroll p
JOIN employees e ON p.msnv = e.msnv
ORDER BY p.year_month DESC, e.name;
```

### 2.2 Loại Báo Cáo Chi Tiết

```
1. PAYSLIP (Bảng Lương Cá Nhân)
   ├─ 1 file PDF per nhân viên
   ├─ In ra từng tháng
   ├─ Gửi cho từng nhân viên
   └─ Format: Chuẩn kế toán

2. PAYROLL REGISTER (Sổ Cấp Lương)
   ├─ 1 file = tất cả nhân viên 1 tháng
   ├─ nhiều cột, nhiều dòng
   ├─ In ra để kiểm toán
   └─ Format: Excel (dễ pivot, filter)

3. SUMMARY TAB (Tờ Tóm Tắt)
   ├─ Tóm tắt từng loại thu nhập/khấu trừ
   ├─ Loại: Lương, Phụ cấp, Thuế, BHXH, v.v
   └─ Total mỗi loại
```

### 2.3 Ví Dụ: PAYSLIP (Bảng Lương)

```
╔═══════════════════════════════════════════════════════════════════════╗
║          CÔNG TY CỔ PHẦN ECOTECH 2A - BẢNG LƯƠNG CÁ NHÂN              ║
╠═══════════════════════════════════════════════════════════════════════╣
║ Tháng: Tháng 12, Năm 2025                    Mã NV: 001               ║
║ Tên: Nguyễn Văn A                            Pos: Công Nhân          ║
║ Bộ Phận: Sản Xuất                            Trạm: 1                 ║
╠═══════════════════════════════════════════════════════════════════════╣
║ PHẦN THU NHẬP:                                                        ║
║   Lương Cơ Bản............................ 25,000,000 VND             ║
║   Phụ Cấp Nhà Ở.......................... 1,500,000 VND              ║
║   Phụ Cấp Xăng Xe........................ 700,000 VND               ║
║   Phụ Cấp Gia Đình....................... 500,000 VND               ║
║   ─────────────────────────────────────────────────────             ║
║   Tổng Phụ Cấp........................... 2,700,000 VND              ║
║                                                                       ║
║   Số Công............................... 24 ngày                     ║
║   Hệ Số Hiệu Suất........................ 1.0                        ║
║   Lương Tính Chính........................ 25,575,310 VND             ║
║                                                                       ║
║   Thưởng & Bổ Cộng....................... 3,500,000 VND              ║
║   Tạm Ứng (Trừ).......................... -5,000,000 VND             ║
║                                                                       ║
║   ════════════════════════════════════════════════════           ║
║   TỔNG LƯƠNG GROSS........................ 27,073,310 VND             ║
╠═══════════════════════════════════════════════════════════════════════╣
║ PHẦN KHẤU TRỪ:                                                        ║
║   BHXH (8%)............................ 2,165,865 VND               ║
║   BHYT (1.5%).......................... 406,599 VND                ║
║   Công Ty Đóng BHXH (20%).............. 5,414,662 VND              ║
║   ─────────────────────────────────────────────────────             ║
║   Tổng Bảo Hiểm......................... 2,572,464 VND              ║
║                                                                       ║
║   Thu Nhập Chịu Thuế..................... 24,500,846 VND             ║
║   Thuế Thu Nhập Cá Nhân:                                             ║
║     Bậc 1 (5M × 5%).................... 250,000 VND                ║
║     Bậc 2 (5M × 10%)................... 500,000 VND                ║
║     Bậc 3 (10M × 15%).................. 1,500,000 VND               ║
║     Bậc 4 (4.5M × 20%)................. 900,000 VND                ║
║   ─────────────────────────────────────────────────────             ║
║   TỔNG THUẾ............................ 3,150,000 VND               ║
║                                                                       ║
║   ════════════════════════════════════════════════════           ║
║   TỔNG KHẤU TRỪ......................... 5,722,464 VND               ║
╠═══════════════════════════════════════════════════════════════════════╣
║ LƯƠNG NET CẦN TRẢ                    ║ 21,350,846 VND               ║
╠═══════════════════════════════════════════════════════════════════════╣
║ Người Chuẩn Bị: Kế Toán Trần Thị B         Ngày: 10/01/2026          ║
║ Người Phê Duyệt: Trưởng BV Lê Văn C        Ngày: 10/01/2026          ║
╚═══════════════════════════════════════════════════════════════════════╝
```

### 2.4 SQL: Generate PAYSLIP

```sql
-- Generate payslip for 1 employee

DELIMITER //

CREATE PROCEDURE GeneratePayslip(
    IN p_msnv INT,
    IN p_year_month VARCHAR(7)
)
BEGIN
    DECLARE v_payroll_id INT;
    
    -- Generate or update payslip
    SELECT payroll_id INTO v_payroll_id FROM payroll
    WHERE msnv = p_msnv AND year_month = p_year_month
    LIMIT 1;
    
    IF v_payroll_id IS NULL THEN
        SELECT 'No payroll record found' AS error;
    ELSE
        -- Query detail data for payslip
        SELECT 
            e.msnv,
            e.name,
            e.position,
            e.department,
            e.tram_id,
            p.year_month,
            p.base_salary,
            p.performance_coef,
            p.working_days,
            
            COALESCE((SELECT SUM(amount) FROM allowances_benefits 
                WHERE msnv = p_msnv AND year_month = p_year_month 
                AND status = 'APPROVED'), 0) AS total_allowances,
            
            p.gross_salary,
            p.bhxh_deduction,
            p.bhyt_deduction,
            p.income_tax,
            p.total_deductions,
            p.net_salary,
            p.status,
            p.calculated_date,
            p.calculated_by
        FROM payroll p
        JOIN employees e ON p.msnv = e.msnv
        WHERE p.msnv = p_msnv AND p.year_month = p_year_month;
    END IF;
END //

DELIMITER ;
```

### 2.5 Test Cases

```python
class TestPayrollDetailsReport:
    
    def test_payslip_generation(self):
        """Test: Tạo bảng lương cá nhân"""
        payslip = generate_payslip(msnv=1, month='2025-12')
        assert payslip['msnv'] == 1
        assert payslip['net_salary'] > 0
        assert payslip['status'] == 'APPROVED'

    def test_payslip_contains_all_fields(self):
        """Test: Bảng lương có tất cả thông tin"""
        payslip = generate_payslip(msnv=1, month='2025-12')
        required_fields = [
            'msnv', 'name', 'position', 'department',
            'base_salary', 'working_days', 'gross_salary',
            'bhxh', 'bhyt', 'tax', 'net_salary'
        ]
        for field in required_fields:
            assert field in payslip

    def test_payroll_register_all_employees(self):
        """Test: Sổ cấp lương tất cả nhân viên"""
        register = generate_payroll_register(month='2025-12')
        assert len(register['employees']) >= 1
        assert 'total_net_salary' in register
```

---

## 3. MODULE: BÁO CÁO TỔNG HỢP (Summary Reports)

### 3.1 Định Nghĩa

**Mục Đích:** Tóm tắt dữ liệu lương cho quản lý, kế toán

**Loại Báo Cáo:**

```
1. SUMMARY BY DEPARTMENT (Tóm tắt theo bộ phận)
   ├─ Bộ Phận, Số NV, Tổng Gross, Tổng Deductions, Tổng Net
   
2. SUMMARY BY TRAM (Tóm tắt theo trạm)
   ├─ Trạm 1, Trạm 2
   ├─ Số NV, Tổng Lương
   
3. SUMMARY BY POSITION (Tóm tắt theo chức vụ)
   ├─ Công Nhân, Trưởng Nhóm, Kỹ Sư
   └─ Tương tự

4. MONTHLY TREND (Xu hướng hàng tháng)
   ├─ Tháng 1-12: Tổng Gross, Total Deductions, Total Net
   └─ So sánh, phân tích trend
```

### 3.2 Ví Dụ: Summary by Department

```
BÁNG CÂN BẰNG LƯƠNG THEO BỘ PHẬN - THÁNG 12/2025
═══════════════════════════════════════════════════════════════════

Bộ Phận          │ Số NV │ Tổng Gross   │ BHXH + BHYT │ Thuế      │ Tổng Net
─────────────────┼───────┼──────────────┼─────────────┼───────────┼──────────────
Sản Xuất         │  45   │ 1,192M       │ 102M        │ 112M      │ 978M
Quản Lý          │  15   │ 420M        │  36M        │ 41M       │ 343M
Kỹ Thuật         │  25   │ 650M        │  56M        │ 63M       │ 531M
Hành Chính       │  10   │ 250M        │  21M        │ 25M       │ 204M
─────────────────┼───────┼──────────────┼─────────────┼───────────┼──────────────
TỔNG CỘNG       │ 100   │ 2,512M       │ 215M        │ 241M      │ 2,056M

Tăng/Giảm so với tháng trước:
  Tổng Gross:     +2.5% (tháng 10: 2,450M)
  Tổng Net:       +1.8% (tháng 10: 2,020M)
```

### 3.3 SQL: Summary Reports

```sql
-- Summary by Department

CREATE VIEW v_summary_by_department AS
SELECT 
    e.department,
    COUNT(DISTINCT e.msnv) AS employee_count,
    SUM(p.gross_salary) AS total_gross,
    SUM(p.bhxh_deduction + p.bhyt_deduction) AS total_insurance,
    SUM(p.income_tax) AS total_tax,
    SUM(p.total_deductions) AS total_deductions,
    SUM(p.net_salary) AS total_net,
    MONTH(STR_TO_DATE(p.year_month, '%Y-%m')) AS month,
    YEAR(STR_TO_DATE(p.year_month, '%Y-%m')) AS year
FROM payroll p
JOIN employees e ON p.msnv = e.msnv
GROUP BY e.department, p.year_month
ORDER BY p.year_month DESC, e.department;

-- Monthly Trend

CREATE VIEW v_monthly_trend AS
SELECT 
    p.year_month,
    COUNT(DISTINCT p.msnv) AS payroll_count,
    SUM(p.gross_salary) AS total_gross,
    SUM(p.bhxh_deduction + p.bhyt_deduction) AS total_insurance,
    SUM(p.income_tax) AS total_tax,
    SUM(p.total_deductions) AS total_deductions,
    SUM(p.net_salary) AS total_net,
    ROUND(SUM(p.net_salary) / COUNT(DISTINCT p.msnv), 0) AS avg_net_per_employee
FROM payroll p
GROUP BY p.year_month
ORDER BY p.year_month DESC;
```

---

## 4. MODULE: BÁO CÁO PHÂN BỔ CHI PHÍ (Allocation Reports)

### 4.1 Định Nghĩa

**Mục Đích:** Phân bổ chi phí lương theo bộ phận, mã kế toán

**Dữ Liệu Báo Cáo:**

```
Mã Kế Toán, Tên, Bộ Phận, Số Tiền
└─ Phân bổ: Chi Phí Lương Tháng 12 = 2,056M VND
```

**Ví Dụ:**

```
PHÂN BỔ CHI PHÍA LƯƠNG - THÁNG 12/2025
═══════════════════════════════════════════════════════════════════

Mã KT  │ Tên Khoản Mục              │ Bộ Phận      │ Số Tiền
───────┼────────────────────────────┼─────────────┼──────────────
5210   │ Chi Phí Lương Sản Xuất      │ Sản Xuất    │ 978,000,000
5220   │ Chi Phí Lương Quản Lý       │ Quản Lý     │ 343,000,000
5230   │ Chi Phí Lương Kỹ Thuật      │ Kỹ Thuật    │ 531,000,000
5240   │ Chi Phí Lương Hành Chính    │ Hành Chính  │ 204,000,000
───────┼────────────────────────────┼─────────────┼──────────────
TỔNG   │                            │             │ 2,056,000,000

Ghi Chép Kế Toán:
Debit  5210 - Chi Phí Lương Sản Xuất        978,000,000
Debit  5220 - Chi Phí Lương Quản Lý         343,000,000
Debit  5230 - Chi Phí Lương Kỹ Thuật        531,000,000
Debit  5240 - Chi Phí Lương Hành Chính      204,000,000
       Credit 3100 - Công Nợ Lương                       2,056,000,000
```

### 4.2 SQL: Allocation Report

```sql
CREATE VIEW v_cost_allocation AS
SELECT 
    acs.accounting_code,
    acs.standard_name,
    e.department,
    SUM(p.net_salary) AS allocated_amount,
    p.year_month
FROM payroll p
JOIN employees e ON p.msnv = e.msnv
JOIN accounting_standards acs ON e.department = acs.department
GROUP BY acs.accounting_code, acs.standard_name, e.department, p.year_month
ORDER BY p.year_month DESC, acs.accounting_code;
```

---

## 5. MODULE: BÁO CÁO THUẾ & BẢO HIỂM (Tax & Insurance Reports)

### 5.1 Định Nghĩa

**Mục Đích:** Báo cáo chi tiết về thuế TNCN & bảo hiểm (nộp hàng tháng)

**Loại Báo Cáo:**

```
1. BÁO CÁO THUẾ TNCN
   ├─ Danh sách nhân viên: MSNV, Tên, Thuế
   ├─ Tổng thuế tháng
   └─ Nộp: Cục Thuế

2. BÁO CÁO BẢO HIỂM XÃ HỘI
   ├─ Danh sách: MSNV, Tên, BHXH (8%)
   ├─ Tổng BHXH & BHYT (Công ty đóng con)
   └─ Nộp: Bảo Hiểm Xã Hội

3. BÁO CÁO BHYT
   ├─ Danh sách: MSNV, Tên, BHYT (1.5%)
   ├─ Tổng BHYT (Công ty đóng con)
   └─ Nộp: BHYT
```

### 5.2 Ví Dụ: Tax Report

```
BÁO CÁO THUẾ THU NHẬP CÁ NHÂN - THÁNG 12/2025
═══════════════════════════════════════════════════════════════════

Doanh Nghiệp: Công Ty CP EcoTech 2A
Mã Số Thuế: [MST]
Kỳ Báo Cáo: Tháng 12/2025

MSNV  │ Tên              │ Lương Tính Thuế │ Thuế       │ Ghi Chú
──────┼──────────────────┼─────────────────┼────────────┼──────────
001   │ Nguyễn Văn A     │ 24,500,846      │ 3,150,000  │ OK
002   │ Trần Thị B       │ 18,200,000      │ 1,900,000  │ OK
003   │ Lê Văn C         │ 20,500,000      │ 2,450,000  │ OK
...   │ ...              │ ...             │ ...        │ ...
──────┼──────────────────┼─────────────────┼────────────┼──────────
TỔNG  │ 100 nhân viên    │ 2,450,000,000   │ 241,000,000│

Nộp Tờ Khai Thuế:
  Địa Điểm: Cục Thuế Thành Phố [XXX]
  Hạn Chót: Ngày 10 tháng 01 năm 2026
  Số Tiền Nộp: 241,000,000 VND
  Hình Thức: Chuyển khoản
```

---

## 6. MODULE: PAYROLL MANAGEMENT DASHBOARD

### 6.1 Dashboard Metrics

```
DASHBOARD - TỔNG QUAN THÁNG 12/2025

┌─────────────────────────────────────────────────────────────┐
│ KPI LƯƠNG VÀ NHÂN SỰ                                         │
├─────────────────────────────────────────────────────────────┤
│ • Số Nhân Viên Tính Lương: 100                              │
│ • Số Nhân Viên Hưởng Lương: 100 (100%)                      │
│ • Số Nhân Viên Nghỉ/Thôi Việc: 0                            │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ TỔNG HỢP TÀI CHÍNH (Tháng 12/2025)                          │
├─────────────────────────────────────────────────────────────┤
│ Tổng Lương Gross (với BHXH CT): 2,600M                      │
│ Tổng Khấu Trừ:                   544M (20.9%)               │
│ Tổng Lương Net:                  2,056M (79.1%)             │
│ Tăng/Giảm vs tháng trước:        +2.5%                       │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ CHI TIẾT KHẤU TRỪ                                            │
├─────────────────────────────────────────────────────────────┤
│ BHXH (8%):     215M   (8.3%)                                │
│ BHYT (1.5%):    39M   (1.5%)                                │
│ Thuế TNCN:     241M   (9.3%)                                │
│ Khác:           49M   (1.9%)                                │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ PHÂN BỘ LƯƠNG THEO BỘ PHẬN                                   │
├─────────────────────────────────────────────────────────────┤
│ Sản Xuất (45 NV):      978M   (47.6%)                       │
│ Quản Lý (15 NV):       343M   (16.7%)                       │
│ Kỹ Thuật (25 NV):      531M   (25.8%)                       │
│ Hành Chính (10 NV):    204M   (9.9%)                        │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ THỐNG KÊ KHÁC                                                │
├─────────────────────────────────────────────────────────────┤
│ Lương Trung Bình/NV:   20.56M                               │
│ Lương Cao Nhất:        28.5M (Msnv: 045)                    │
│ Lương Thấp Nhất:       15.2M (Msnv: 089)                    │
│ Truy Thu Tổng:         185M                                 │
│ Thưởng Tổng:           275M                                 │
└─────────────────────────────────────────────────────────────┘
```

---

## 7. CHECKLIST NHÓM 4: BÁO CÁO

**Chuẩn Bị Nhóm 4:**

- [ ] Báo Cáo Lương Chi Tiết (Payroll Details)
  - [ ] Tạo PAYSLIP (PDF) cho từng nhân viên
  - [ ] Tạo PAYROLL REGISTER (Excel) 
  - [ ] Test: Lương NET = tất cả nhân viên SUM lại = Reconciliation.Total_Net
  
- [ ] Báo Cáo Tóm Tắt (Summary Reports)
  - [ ] Summary by Department
  - [ ] Summary by Tram
  - [ ] Monthly Trend
  
- [ ] Báo Cáo Phân Bổ Chi Phí (Allocation Reports)
  - [ ] Phân bổ theo mã kế toán
  - [ ] Verify: Total allocation = Total Net Salary
  
- [ ] Báo Cáo Thuế & Bảo Hiểm (Tax & Insurance)
  - [ ] Tax report (nộp cục thuế)
  - [ ] BHXH report (nộp bảo hiểm)
  - [ ] BHYT report (nộp bảo hiểm y tế)
  
- [ ] Dashboard & Quản Lý
  - [ ] KPI metrics
  - [ ] Trend analysis
  - [ ] Management summary

---

## 8. QUIN TRÌNH XUẤT BÁGO CÁO TOÀN BỘ

```
QUIN TRÌNH XUẤT BÁO CÁO HÀNG THÁNG
═════════════════════════════════════════════════════════════

Step 1: Gửi email thông báo cho tất cả nhân viên
  ├─ "Lương tháng [month] đã được tính"
  ├─ Có link download PAYSLIP cá nhân
  └─ Hạn kiểm tra, báo cáo lỗi

Step 2: Gửi báo cáo cho quản lý bộ phận
  ├─ Summary by Department
  ├─ Danh sách lương từng nhân viên
  └─ Hạn xác nhận

Step 3: Gửi báo cáo cho kế toán
  ├─ Payroll Register (chi tiết)
  ├─ Tax Report (chuẩn bị nộp)
  ├─ Insurance Report (chuẩn bị nộp)
  └─ Cost Allocation (ghi chép kế toán)

Step 4: Gửi báo cáo cho tài chính
  ├─ Summary Report
  ├─ Cash Flow (tiền cần thanh toán)
  ├─ Trend Analysis
  └─ Forecast (dự báo tháng sau)

Step 5: Xuất dữ liệu từ hệ thống
  ├─ Backup: Payroll data (binary)
  ├─ Export: Excel/CSV (để import vào system khác)
  └─ Archive: Lưu trữ lâu dài

Step 6: Làm bảng cân đối cuối cùng
  ├─ Verify: Tất cả báo cáo consistent
  ├─ Sign off: Kiểm toán + Kế toán trưởng
  └─ Archvie: Lưu trữ hàng năm
```

---

**📋 NHÓM 4: BÁO CÁO - v1.0**
**Chuẩn Bị Chi Tiết: 10/04/2025**
**Trạng Thái: SẴN SÀNG**

