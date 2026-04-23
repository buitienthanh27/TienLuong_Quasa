# 📋 NHÓM 2: PHÁT SINH (Transactions Layer)
## Tài Liệu Chi Tiết - ECOTECH 2A Module

---

## 1. TỔNG QUAN

**Nhóm 2** là lớp dữ liệu biến động hàng tháng, ghi nhận hoạt động thực tế của nhân viên.

```
Nhóm 2: Phát Sinh (Transactions)
├─ Chấm công (Attendance)
├─ Công nợ & Tạm Ứng (Advances & Debt Recovery)
├─ Phụ Cấp & Chế Độ (Allowances & Benefits)
├─ Đánh Giá Kỹ Thuật (Performance)
└─ Bổ Công Chăm Sóc (Care Adjustments & Bonuses)

Đặc Điểm:
├─ Dữ liệu động (thay đổi hàng tháng)
├─ Input cho Nhóm 3 (Xử Lý)
├─ Cần audit trail nghiêm ngặt (ai nhập, khi nào)
├─ Thường có data validation phức tạp
└─ Cần xác nhận bởi quản lý bộ phận
```

---

## 2. MODULE: CHẤM CÔNG (Attendance)

### 2.1 Định Nghĩa

**Mục Đích:** Ghi nhận số công (số ngày/ca làm) của nhân viên hàng tháng

**Dữ Liệu Lưu Trữ:**

```
MSNV, Tháng, Số Công Làm, Số Công Nghỉ, Ghi Chú
```

**Database Table:**
```sql
CREATE TABLE attendance (
  attendance_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month VARCHAR(7) -- Format: 2025-12
  working_days DECIMAL(5,2) -- Số công
  absent_days DECIMAL(5,2) -- Nghỉ không phép
  sick_days DECIMAL(5,2)   -- Nghỉ ốm (có lương)
  leave_days DECIMAL(5,2)  -- Nghỉ phép (có lương)
  created_date TIMESTAMP,
  created_by INT,
  approved_by INT,
  approved_date TIMESTAMP,
  status ENUM('DRAFT', 'SUBMITTED', 'APPROVED', 'LOCKED'),
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 2.2 Quy Trình Nhập Chấm Công

```
NHẬP CHẤM CÔNG
═══════════════════════════════════════════════════════════

Step 1: Chuẩn Bị Dữ Liệu
├─ Từ hệ thống chấm công/quét thẻ
├─ Export ra file Excel: MSNV | Công | Nghỉ | Ghi Chú
├─ Định dạng: 1 dòng = 1 nhân viên
└─ Thường vào ngày cuối tháng

Step 2: Validation
├─ MSNV phải có trong bảng employees
├─ Công + Nghỉ <= 26 (tối đa ngày làm trong tháng)
├─ Công >= 0
├─ Không được nhập 2 lần cùng MSNV/tháng
├─ Khi tháng có ngày lễ: công tối đa = 26 - số_le

Step 3: Phân Loại Nghỉ
├─ Nghỉ không phép (absent) → không tính lương
├─ Nghỉ ốm có giấy (sick) → tính lương 100%
├─ Nghỉ phép hàng năm (leave) → tính lương 100%
└─ Ghi chú: Lý do, người phê duyệt

Step 4: Workflow Phê Duyệt
├─ DRAFT: Nhân viên/Quản lý nhập
├─ SUBMITTED: Gửi phê duyệt
├─ APPROVED: Quản lý phê duyệt
└─ LOCKED: Sau kỳ tính lương (không sửa được)

Step 5: Lưu Trữ
└─ Lưu vào attendance TB
```

### 2.3 Ví Dụ Dữ Liệu

| Attendance_ID | MSNV | Year_Month | Working_Days | Absent_Days | Sick_Days | Leave_Days | Status |
|---|---|---|---|---|---|---|---|
| 1 | 001 | 2025-12 | 24.5 | 0.5 | 1 | 0 | APPROVED |
| 2 | 002 | 2025-12 | 26 | 0 | 0 | 0 | APPROVED |
| 3 | 003 | 2025-12 | 22 | 3 | 0 | 1 | SUBMITTED |

### 2.4 Test Cases

```python
class TestAttendance:
    
    def test_attendance_import_valid(self):
        """Test: Import chấm công hợp lệ"""
        attendance = {
            'msnv': 1,
            'year_month': '2025-12',
            'working_days': 24.5
        }
        result = import_attendance(attendance)
        assert result['success'] == True
        assert result['status'] == 'DRAFT'

    def test_total_days_validation(self):
        """Test: Tổng công + nghỉ không vượt quá 26"""
        invalid = {
            'msnv': 1,
            'working_days': 24,
            'absent_days': 3,  # 24+3 = 27 > 26
        }
        with pytest.raises(ValidationError):
            import_attendance(invalid)

    def test_no_duplicate_month(self):
        """Test: Không được nhập 2 lần MSNV + tháng"""
        import_attendance({'msnv': 1, 'year_month': '2025-12', 'working_days': 24})
        with pytest.raises(DuplicateError):
            import_attendance({'msnv': 1, 'year_month': '2025-12', 'working_days': 25})

    def test_approval_workflow(self):
        """Test: Workflow phê duyệt"""
        att = import_attendance({'msnv': 1, 'year_month': '2025-12', 'working_days': 24})
        assert att['status'] == 'DRAFT'
        
        submit_attendance(att['id'])
        att = get_attendance(att['id'])
        assert att['status'] == 'SUBMITTED'
        
        approve_attendance(att['id'], approved_by=10)
        att = get_attendance(att['id'])
        assert att['status'] == 'APPROVED'
```

---

## 3. MODULE: CÔNG NỢ & TẠM ỨNG (Advances & Debt Recovery) [NEW]

### 3.1 Định Nghĩa

**Mục Đích:** Ghi nhận nợ, tạm ứng, truy thu lương

**Dữ Liệu Lưu Trữ:**

```
MSNV, Tháng, Loại (Nợ/Truy), Số Tiền, Lý Do
```

**Database Table:**
```sql
CREATE TABLE advances (
  advance_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month VARCHAR(7),
  amount DECIMAL(12,2),
  advance_type ENUM('ADVANCE', 'DEBT_RECOVERY', 'RECOVERY_INTEREST'),
  reason VARCHAR(255),
  status ENUM('PENDING', 'APPROVED', 'DEDUCTED', 'CANCELLED'),
  created_date TIMESTAMP,
  approve_date TIMESTAMP,
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 3.2 Loại Giao Dịch

```
ADVANCE (Tạm Ứng)
├─ Nhân viên lấy tiền trước
├─ Sẽ trừ vào lương tháng sau
├─ Ví dụ: Lấy tạm 5 triệu tháng 11, trừ tháng 12

DEBT_RECOVERY (Truy Thu Nợ)
├─ Công ty truy thu nợ cũ
├─ Ví dụ: Nhân viên thiếu 2 triệu từ tháng 10, truy tháng 11
├─ Trừ vào lương chính

RECOVERY_INTEREST (Truy Thu Lãi)
├─ Lãi đơn trên khoảng nợ
├─ Ví dụ: Lãi 0.5% trên 2 triệu = 10,000
```

### 3.3 Quy Trình Nợ & Tạm Ứng

```
QUẢN LÝ CÔNG NỢ & TẠM ỨNG
═══════════════════════════════════════════════════════════

Step 1: Nhân Viên Xin Tạm Ứng
├─ Xin MSNV, Số Tiền, Lý Do
├─ Status: PENDING
└─ Chờ phê duyệt

Step 2: Quản Lý Phê Duyệt
├─ Check: Lương tháng trước >= 2× số tạm ứng?
├─ Nếu OK: Status = APPROVED
└─ Nếu không: Reject

Step 3: Tính Lương
├─ Khi tính lương tháng N
├─ Duyệt bảng advances: có APPROVED chưa trừ không?
├─ Nếu có: Trừ trong Phase 4 (Deductions)
└─ Status = DEDUCTED

Step 4: Tracking
├─ Lịch sử tạm ứng
├─ Nợ còn lại
└─ Kế hoạch truy thu
```

### 3.4 Ví Dụ Dữ Liệu

| Advance_ID | MSNV | Year_Month | Amount | Type | Reason | Status |
|---|---|---|---|---|---|---|
| 1 | 001 | 2025-11 | 5,000,000 | ADVANCE | Tạm ứng cấn | APPROVED |
| 2 | 001 | 2025-12 | -5,000,000 | DEBT_RECOVERY | Trừ tạm ứng tháng 11 | DEDUCTED |
| 3 | 002 | 2025-12 | 2,000,000 | DEBT_RECOVERY | Truy thu nợ | APPROVED |

### 3.5 Test Cases

```python
class TestAdvances:
    
    def test_advance_approval_check(self):
        """Test: Phê duyệt tạm ứng phải check lương"""
        # Lương tháng trước: 20M
        # Xin tạm ứng: 15M
        # OK: 20M >= 2×15M? NO → Reject
        with pytest.raises(ApprovalError):
            approve_advance(msnv=1, amount=15_000_000)

    def test_deduction_in_payroll_calculation(self):
        """Test: Tạm ứng được trừ trong tính lương"""
        approve_advance(msnv=1, amount=5_000_000, year_month='2025-11')
        payroll = calculate_payroll(msnv=1, month='2025-12')
        assert payroll['deductions']['advance'] == 5_000_000
        assert payroll['advance_status'] == 'DEDUCTED'

    def test_recovery_with_interest(self):
        """Test: Truy thu có tính lãi"""
        recovery = add_recovery(
            msnv=1,
            amount=2_000_000,
            interest_rate=0.005,  # 0.5%
            year_month='2025-12'
        )
        assert recovery['principal'] == 2_000_000
        assert recovery['interest'] == 10_000
        assert recovery['total'] == 2_010_000
```

---

## 4. MODULE: PHỤ CẤP & CHẾ ĐỘ (Allowances & Benefits) [NEW]

### 4.1 Định Nghĩa

**Mục Đích:** Ghi nhận các khoản cộng lương (phụ cấp, chế độ, thưởng, v.v)

**Dữ Liệu Lưu Trữ:**

```
MSNV, Tháng, Loại Phụ Cấp, Số Tiền, Lý Do
```

**Database Table:**
```sql
CREATE TABLE allowances_benefits (
  allowance_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month VARCHAR(7),
  allowance_type ENUM(
    'HOUSING',       -- Phụ cấp nhà ở
    'TRANSPORT',     -- Phụ cấp xăng xe
    'FAMILY',        -- Phụ cấp gia đình
    'HAZARD',        -- Phụ cấp độc hại
    'TECHNICAL',     -- Phụ cấp kỹ thuật
    'SHIFT',         -- Phụ cấp ca đêm
    'BONUS',         -- Thưởng tháng 13
    'SPECIAL'        -- Thưởng đặc biệt
  ),
  amount DECIMAL(12,2),
  description VARCHAR(255),
  status ENUM('PENDING', 'APPROVED', 'REJECTED'),
  approved_by INT,
  approved_date TIMESTAMP,
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 4.2 Loại Phụ Cấp

```
PHỤ CẤP HÀNG THÁNG
├─ HOUSING: 1-2 triệu (tuỳ vị trí)
├─ TRANSPORT: 500K - 1M (xăng xe)
├─ FAMILY: 500K/con (tuỳ số con)
├─ HAZARD: 1-2M (công việc nguy hiểm)
└─ TECHNICAL: 500K - 1.5M (kỹ sư, chuyên viên)

PHỤ CẤP ĐẶC BIỆT
├─ SHIFT: 1.2M (làm ca đêm)
├─ BONUS: Thưởng cố định (Tết, lễ)
└─ SPECIAL: Thưởng dự án, thưởng QK

CÓ THỂ TÍCH LŨY
└─ Một nhân viên trong 1 tháng có thể nhận nhiều loại cộng
```

### 4.3 Quy Trình Ghi Nhận

```
GIAO DỊCH PHỤ CẤP & CHẾ ĐỘ
═══════════════════════════════════════════════════════════

Step 1: Ghi Nhận Phụ Cấp
├─ Bộ Phận chuẩn bị: MSNV | Loại | Số Tiền
├─ Lý Do: (tuỳ loại, có template)
└─ Status: PENDING

Step 2: Phê Duyệt
├─ Check: Số tiền hợp lý (không vượt quy định)
├─ Check: Loại phụ cấp đúng chính sách
├─ Approved by: Quản lý/Kế toán
└─ Status: APPROVED hoặc REJECTED

Step 3: Tích Lũy
├─ Tính lại tổng phụ cấp: SUM(amount WHERE approved)
└─ Total allowances = Tất cả APPROVED

Step 4: Tính Lương (Phase 1)
├─ Base Salary + Allowances (từ bước 3)
└─ Sau đó apply coefficient
```

### 4.4 Ví Dụ Dữ Liệu

| Allowance_ID | MSNV | Year_Month | Type | Amount | Status |
|---|---|---|---|---|---|
| 1 | 001 | 2025-12 | HOUSING | 1,500,000 | APPROVED |
| 2 | 001 | 2025-12 | TRANSPORT | 700,000 | APPROVED |
| 3 | 001 | 2025-12 | FAMILY | 500,000 | APPROVED |
| 4 | 001 | 2025-12 | BONUS | 5,000,000 | PENDING |
| 5 | 002 | 2025-12 | TECHNICAL | 1,500,000 | APPROVED |

### 4.5 Test Cases

```python
class TestAllowances:
    
    def test_multiple_allowances_same_month(self):
        """Test: Nhân viên nhận nhiều loại phụ cấp"""
        add_allowance(msnv=1, type='HOUSING', amount=1.5e6, month='2025-12')
        add_allowance(msnv=1, type='TRANSPORT', amount=700e3, month='2025-12')
        add_allowance(msnv=1, type='FAMILY', amount=500e3, month='2025-12')
        
        total = get_total_allowances(msnv=1, month='2025-12')
        assert total == 1_500_000 + 700_000 + 500_000

    def test_allowance_in_payroll_phase1(self):
        """Test: Phụ cấp được add vào Phase 1"""
        base_salary = 25_000_000
        allowances = 2_700_000
        
        payroll = calculate_payroll(msnv=1, month='2025-12')
        assert payroll['phase1_total'] == base_salary + allowances

    def test_rejected_allowance_not_included(self):
        """Test: Phụ cấp REJECTED không tính"""
        add_allowance(msnv=1, type='BONUS', amount=5e6, status='REJECTED')
        total = get_total_allowances(msnv=1, month='2025-12')
        assert 5_000_000 not in total
```

---

## 5. MODULE: ĐÁNH GIÁ KỸ THUẬT (Performance)

### 5.1 Định Nghĩa

**Mục Đích:** Ghi nhận hiệu suất, sản lượng, chất lượng

**Dữ Liệu Lưu Trữ:**

```
MSNV, Tháng, Sản Lượng, Cấp (A/B/C), Ghi Chú
```

**Database Table:**
```sql
CREATE TABLE performance (
  performance_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month VARCHAR(7),
  production_quantity DECIMAL(12,2), -- Sản lượng
  quality_score DECIMAL(5,2),        -- Điểm chất lượng (0-100)
  grade CHAR(1) (A/B/C/D/E),         -- Cấp đánh giá
  created_date TIMESTAMP,
  approved_by INT,
  approved_date TIMESTAMP,
  notes VARCHAR(255),
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 5.2 Quy Trình Đánh Giá

```
NHẬP ĐÁNH GIÁ KỸ THUẬT
═══════════════════════════════════════════════════════════

Step 1: Thu Thập Dữ Liệu
├─ Sản lượng: Số sản phẩm/sản lượng thực tế
├─ Chất lượng: % lỗi, điểm chất lượng (0-100)
├─ Kỹ thuật: Kỹ năng, tuân thủ
└─ Source: Quản lý, Kiểm toán chất lượng

Step 2: Auto-Grade
├─ Nếu sản lượng >= target: Grade = A
├─ Nếu sản lượng 80-99% target: Grade = B
├─ Nếu sản lượng 60-79% target: Grade = C
├─ Nếu chất lượng < 90%: Hạ 1 cấp
└─ Manual override nếu cần

Step 3: Phê Duyệt
├─ Quản lý phê duyệt
├─ Status: APPROVED
└─ Lưu vào DB

Step 4: Sử Dụng Trong Tính Lương
├─ Phase 1: Coefficient = Hệ số Grade
└─ Ví dụ: Grade A → 1.0, Grade B → 0.95, Grade C → 0.90
```

### 5.3 Ví Dụ Dữ Liệu

| Performance_ID | MSNV | Year_Month | Quantity | Quality | Grade |
|---|---|---|---|---|---|
| 1 | 001 | 2025-12 | 1000 | 95 | A |
| 2 | 002 | 2025-12 | 850 | 90 | B |
| 3 | 003 | 2025-12 | 600 | 85 | C |

### 5.4 Test Cases

```python
class TestPerformance:
    
    def test_auto_grade_a(self):
        """Test: Sản lượng 100% → Grade A"""
        perf = add_performance(
            msnv=1,
            quantity=1000,  # Target
            quality=95
        )
        assert perf['grade'] == 'A'

    def test_grade_downgrade_quality(self):
        """Test: Chất lượng < 90% hạ cấp"""
        perf = add_performance(
            msnv=1,
            quantity=1000,  # 100% → A
            quality=85      # < 90% → Hạ xuống B
        )
        assert perf['grade'] == 'B'

    def test_performance_used_in_coefficient(self):
        """Test: Grade ảnh hưởng hệ số lương"""
        # Grade A: hệ số 1.0
        # Grade B: hệ số 0.95
        payroll_a = calculate_payroll(msnv_with_grade_a)
        payroll_b = calculate_payroll(msnv_with_grade_b)
        
        assert payroll_b['salary'] < payroll_a['salary']
```

---

## 6. MODULE: BỔ CÔNG CHĂM SÓC (Care Adjustments & Bonuses) [NEW]

### 6.1 Định Nghĩa

**Mục Đích:** Ghi nhận các khoản bổ trợ, thưởng, phạt, điều chỉnh đặc biệt

**Dữ Liệu Lưu Trữ:**

```
MSNV, Tháng, Loại (Thưởng/Phạt/Điều Chỉnh), Số Tiền, Lý Do
```

**Database Table:**
```sql
CREATE TABLE care_adjustments (
  care_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv INT,
  year_month VARCHAR(7),
  care_type ENUM('BONUS', 'PENALTY', 'ADJUSTMENT', 'COMPENSATION'),
  amount DECIMAL(12,2),
  reason VARCHAR(255),
  approved_by INT,
  approved_date TIMESTAMP,
  status ENUM('PENDING', 'APPROVED', 'REJECTED'),
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

### 6.2 Loại Điều Chỉnh

```
THƯỞNG (BONUS)
├─ Thưởng KPI: Đạt chỉ tiêu
├─ Thưởng dự án: Hoàn thành dự án
├─ Thưởng hành động: Tiết kiệm, đề xuất cải tiến
└─ Thưởng tháng 13: Năm cuối năm

PHẠ (PENALTY) - Số tiền âm
├─ Phạt vắng mặt không phép
├─ Phạt vi phạm quy định
├─ Phạt hư hỏng tài sản
└─ Phạt an toàn lao động

ĐIỀU CHỈNH (ADJUSTMENT)
├─ Sửa lỗi lương tháng trước
├─ Bổ sung công
├─ Bù trợ cấp
└─ Điều chỉnh riêng

BỒI THƯỜNG (COMPENSATION)
├─ Tai nạn lao động
├─ Bồi thường thiệt hại
└─ Hỗ trợ khó khăn
```

### 6.3 Quy Trình Ghi Nhận

```
GIAO DỊCH BỔ CÔNG CHĂM SÓC
═══════════════════════════════════════════════════════════

Step 1: Ghi Nhận Yêu Cầu
├─ Loại: BONUS/PENALTY/ADJUSTMENT/COMPENSATION
├─ MSNV, Tháng, Số Tiền, Lý Do (chi tiết)
└─ Status: PENDING

Step 2: Xét Duyệt (2-cấp)
├─ Cấp 1: Quản lý trực tiếp kiểm tra + comment
├─ Cấp 2: Trưởng bộ phận/Kế toán phê duyệt
└─ Nếu REJECTED: Quay lại bước 1

Step 3: Tích Lũy
├─ Tính tổng: SUM(amount WHERE status = APPROVED)
├─ Phân loại:
│  ├─ Thưởng (+)
│  ├─ Phạt (-)
│  └─ Điều chỉnh (±)
└─ Total care adjustment

Step 4: Tính Lương (Phase 5)
├─ Net Salary = Gross - Tax - BHXH - BHYT + Care Adjustments
└─ Cuối cùng: Lương nhân viên
```

### 6.4 Ví Dụ Dữ Liệu

| Care_ID | MSNV | Year_Month | Type | Amount | Reason | Status |
|---|---|---|---|---|---|---|
| 1 | 001 | 2025-12 | BONUS | 3,000,000 | Thưởng KPI | APPROVED |
| 2 | 001 | 2025-12 | ADJUSTMENT | 500,000 | Sửa lỗi tháng 11 | APPROVED |
| 3 | 002 | 2025-12 | PENALTY | -200,000 | Vắng mặt 1 ngày | APPROVED |
| 4 | 003 | 2025-12 | COMPENSATION | 2,000,000 | Tai nạn lao động | PENDING |

### 6.5 Test Cases

```python
class TestCareAdjustments:
    
    def test_bonus_approval_workflow(self):
        """Test: Workflow phê duyệt thưởng"""
        care = add_care_adjustment(
            msnv=1,
            type='BONUS',
            amount=3_000_000,
            month='2025-12'
        )
        assert care['status'] == 'PENDING'
        
        # Cấp 1
        approve_l1(care['id'])
        # Cấp 2
        approve_l2(care['id'])
        
        care = get_care(care['id'])
        assert care['status'] == 'APPROVED'

    def test_penalty_negative_amount(self):
        """Test: Phạt là số âm"""
        care = add_care_adjustment(
            msnv=1,
            type='PENALTY',
            amount=-200_000,
            month='2025-12'
        )
        assert care['amount'] < 0

    def test_care_in_net_salary(self):
        """Test: Điều chỉnh chăm sóc ảnh hưởng lương net"""
        # Thưởng 3M + Phạt 0.2M
        adjust = 3_000_000 - 200_000
        payroll = calculate_payroll(msnv=1, month='2025-12')
        assert payroll['care_adjustment'] == adjust
        assert payroll['net_salary'] > payroll['gross_salary']
```

---

## 7. TÂNG GIAO DỊCH NHÓM 2 - TÍNH LƯƠNG

```
NHÓM 2: PHÁT SINH (Transactions)
════════════════════════════════════════════════════════════

Dữ Liệu Input:

1. CHẤM CÔNG
   └─ Working Days = 24 ngày

2. PERFORMANCE
   └─ Grade = A (Coefficient 1.0)

3. ALLOWANCES
   └─ Housing + Transport + Family = 2.7M

4. ADVANCES
   └─ Truy thu = -5M

5. CARE ADJUSTMENTS
   └─ Bonus 3M + Adjustment +0.5M = +3.5M

→ NHÓM 3: XỬ LÝ TÍNH LƯƠNG (5 Phase)

Output: LƯƠNG NET
```

---

## 8. CHECKLIST NHÓM 2: PHÁT SINH

**Chuẩn Bị Nhóm 2:**

- [ ] Chấm công (Attendance)
  - [ ] Tất cả nhân viên active có chấm công
  - [ ] Tổng công + nghỉ <= 26
  - [ ] Workflow phê duyệt hoàn tất
  
- [ ] Công nợ & Tạm ứng (Advances)
  - [ ] Tạm ứng phê duyệt hợp lệ
  - [ ] Truy thu có lý do rõ ràng
  - [ ] Lãi được tính nếu cần
  
- [ ] Phụ cấp & Chế độ (Allowances)
  - [ ] Tất cả phụ cấp phê duyệt
  - [ ] Số tiền hợp lý
  - [ ] Không trùng lặp
  
- [ ] Đánh giá kỹ thuật (Performance)
  - [ ] Tất cả nhân viên có đánh giá
  - [ ] Grade phù hợp với sản lượng/chất lượng
  
- [ ] Bổ công chăm sóc (Care Adjustments)
  - [ ] Thưởng/Phạt có lý do
  - [ ] Workflow 2-cấp phê duyệt hoàn tất
  - [ ] Số tiền hợp lý

---

**📋 NHÓM 2: PHÁT SINH - v1.0**
**Chuẩn Bị Chi Tiết: 10/04/2025**
**Trạng Thái: SẴN SÀNG**

