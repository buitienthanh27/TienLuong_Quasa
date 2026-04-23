# 📊 PHÂN TÍCH CHI TIẾT - BẢNG PHÂN TOÁN
## Quy Tắc Phân Bổ Chi Phí Theo Bộ Phận/Phòng Ban

---

## 1. MỤC ĐÍCH BẢNG PHÂN TOÁN

**Mục Tiêu:** Phân bổ tất cả chi phí lương từ LƯƠNG ĐỘI sang các bộ phận chi phí khác nhau

**Input:**
- Dữ liệu lương từ `LƯƠNG ĐỘI` sheet (cột K: Net Salary)
- Tỷ lệ phân bổ cho từng bộ phận (%)

**Output:**
- Chi phí lương phân bổ cho: Bộ Phận A, B, C, D, ...

---

## 2. CẤU TRÚC DỮ LIỆU

### 2.1 Sheet Structure

```
┌───────┬──────────────────────────────────────────────────┐
│ Cột   │ Nội Dung                                         │
├───────┼──────────────────────────────────────────────────┤
│ A-D   │ Mô Tả bộ phận / Cột chuẩn                       │
│ E-K   │ Chi phí cho từng bộ phận (14 cột dữ liệu)       │
└───────┴──────────────────────────────────────────────────┘

Rows:
├─ Row 1-5: Header
├─ Row 6-10: Tỷ lệ phân bổ (%)
├─ Row 11: Kiểu tính toán (Percentage hay Direct)
├─ Row 12-30: Data từ LƯƠNG ĐỘI
│   ├─ E: Chi phí gốc từ LƯƠNG ĐỘI
│   ├─ F-K: Phân bổ theo tỷ lệ (E * Rate)
└─ Row 31+: Tổng cộng
```

### 2.2 Các Cột Chính

```
E: Chi Phí Gốc (Total)
  └─ Source: =LƯƠNG ĐỘI!K13 (Lương net)

F-K: Bộ Phận Phân Bổ (6 columns)
  ├─ F: Bộ Phận 1 (Sản Xuất)
  ├─ G: Bộ Phận 2 (Quản Lý)
  ├─ H: Bộ Phận 3 (Kinh Doanh)
  ├─ I: Bộ Phận 4 (Hành Chính)
  ├─ J: Bộ Phận 5 (Kỹ Thuật)
  └─ K: Bộ Phận 6 (Khác)
```

---

## 3. CÔNG THỨC PHÂN TOÁN

### 3.1 Formula Pattern

```excel
# Hàng 12 (dòng đầu tiên dữ liệu)
E15: =SUM(E16:E19,E35,E20,E25,E38,E30)

# Giải thích: Cộng từ các hàng:
# E16:E19 = hàng 16 đến 19 (liên tiếp)
# E35     = hàng 35 (riêng lẻ)
# E20     = hàng 20 (riêng lẻ)
# E25     = hàng 25 (riêng lẻ)
# E38     = hàng 38 (riêng lẻ)
# E30     = hàng 30 (riêng lẻ)

Tức: SUM(E16:E19, E35, E20, E25, E38, E30)
     = E16 + E17 + E18 + E19 + E35 + E20 + E25 + E38 + E30
```

### 3.2 Phân Bổ Theo Tỷ Lệ

```excel
# Cột F (Bộ Phận 1) - hàng 15
F15: =ROUND(E15 * $F$11, 0)

WHERE:
  E15 = Total cost (từ multiple hàng)
  $F$11 = Tỷ lệ phân bổ cho Bộ Phận 1 (VD: 30%)
  ROUND(..., 0) = Làm tròn thành số nguyên

# Công thức tương tự cho các cột G-K
G15: =ROUND(E15 * $G$11, 0)  # Bộ Phận 2
H15: =ROUND(E15 * $H$11, 0)  # Bộ Phận 3
...
```

### 3.3 Tỷ Lệ Phân Bổ (Row 11)

```
┌────────────┬──────────┬──────────┬──────────┬──────────┬──────────┬──────────┐
│ Tham Số    │    F     │    G     │    H     │    I     │    J     │    K     │
│ (Row 11)   │   30%    │   25%    │   20%    │   10%    │   10%    │   5%     │
├────────────┼──────────┼──────────┼──────────┼──────────┼──────────┼──────────┤
│ Tổng       │  30%     │  25%     │  20%     │  10%     │  10%     │  5%      │
│            │ = 100%                                                          │
└────────────┴──────────┴──────────┴──────────┴──────────┴──────────┴──────────┘
```

---

## 4. ALGORITHM - BƯỚC TÍNH PHÂN TOÁN

```python
def calculate_allocation(total_cost, allocation_rates):
    """
    Phân toán chi phí gốc theo tỷ lệ
    
    Args:
        total_cost: Tổng chi phí cần phân toán
        allocation_rates: Dict {dept: percentage}
    
    Returns:
        Dict {dept: allocated_amount}
    """
    result = {}
    
    # STEP 1: Validate input
    assert total_cost >= 0, "Chi phí phải >= 0"
    total_rate = sum(allocation_rates.values())
    assert abs(total_rate - 1.0) < 0.01, f"Tỷ lệ phải = 100%, nhưng = {total_rate*100}%"
    
    # STEP 2: Tính phân toán cho từng bộ phận
    for dept, rate in allocation_rates.items():
        allocated = round(total_cost * rate, 0)
        result[dept] = allocated
    
    # STEP 3: Validation - tổng phần bổ = tổng gốc
    total_allocated = sum(result.values())
    assert total_allocated == total_cost, \
        f"Sai số: Tổng phân toán {total_allocated} != gốc {total_cost}"
    
    return result

# EXAMPLE
total = 1_000_000
rates = {
    'SanXuat': 0.30,
    'QuanLy': 0.25,
    'KinhDoanh': 0.20,
    'HanhChinh': 0.10,
    'KyThuat': 0.10,
    'Khac': 0.05
}

result = calculate_allocation(total, rates)
# Output: {SanXuat: 300000, QuanLy: 250000, ...}
```

---

## 5. PHỨC TẠP - NON-CONTIGUOUS RANGE

### 5.1 Vấn Đề

Excel formula sử dụng **non-contiguous ranges** (các hàng không liên tiếp):

```
E15: =SUM(E16:E19, E35, E20, E25, E38, E30)
         ││││││││    ││││   ││││   ││││   ││││
         Liên tiếp    Riêng  Riêng  Riêng  Riêng
```

### 5.2 Giải Thích

```
Hàng 16-19:   Dữ liệu từ bộ phận A (4 nhân viên)
Hàng 20:      Cộng trừ điều chỉnh bộ phận A
Hàng 25:      Dữ liệu từ bộ phận B (5 nhân viên)
Hàng 30:      Cộng trừ điều chỉnh bộ phận B
Hàng 35:      Dữ liệu từ bộ phận C (thêm rows)
Hàng 38:      Cộng trừ điều chỉnh bộ phận C
```

### 5.3 Khi Triển Khai - Chuyển Thành Code

```python
# Thay vì SUM non-contiguous, cải tiến:
cost_elements = [
    E16, E17, E18, E19,  # Bộ phận A: 4 hàng liên tiếp
    E20,                  # Điều chỉnh A
    E25_to_E34,           # Bộ phận B: tối đa 10 hàng
    E35,                  # Bộ phận C
    E38                   # Điều chỉnh C
]

total_cost = sum(cost_elements)

# BETTER: Sử dụng structure rõ ràng
def sum_cost_from_structure():
    """Tối ưu: dùng query filter hoặc GROUP BY"""
    
    # SELECT SUM(amount) FROM payroll
    # WHERE (row BETWEEN 16 AND 19) 
    #    OR row IN (20, 25, 30, 35, 38)
```

---

## 6. CHI PHÍ #REF! ERRORS - VẤN ĐỀ HIỆN TẠI

### 6.1 Vị Trí Lỗi

```
K10: =#REF!   (Cell này tham chiếu sheet bị xóa)
K12: =#REF!   (Tham chiếu external file [10]PHAN_TOAN)

Công thức gốc (dự đoán):
K10: =SomeSheet!$A$10  (sheet không tồn tại)
K12: ='[10]PHAN_TOAN'!$A$1  (file [10] bị xóa)
```

### 6.2 Tác Động

```
├─ K10 bị lỗi
│  └─ K11 = K10/100  →  #REF! (lỗi lan truyền)
│  └─ K12 = K11 * cost  →  #REF!
│  └─ Toàn bộ cột K không dùng được

├─ K12 = ='[10]PHAN_TOAN'!$A$1  →  #REF!
│  └─ Bất kỳ công thức nào tham chiếu K12 cũng lỗi
```

### 6.3 Giải Pháp

**Option A: Sửa Công Thức**
```excel
# BEFORE:
K10: =#REF!

# AFTER (nếu biết tham chiếu):
K10: ='BẢNG PHÂN TOÁN'!$A$10  (hoặc sheet đúng)
K12: =VLOOKUP(...)  (thay external link)
```

**Option B: Clean Up (Loại Bỏ)**
```
Nếu cột K không đúng mục đích:
  ├─ Xóa K10, K12
  ├─ Update tất cả công thức tham chiếu
  └─ Xác nhận toàn bộ work
```

---

## 7. IMPLEMENTATION CHECKLIST

### 7.1 Data Structure (Database)

```python
class AllocationRules:
    """Quy tắc phân toán"""
    dept_name: str
    percentage: float  # 0-1
    accounting_code: str
    description: str

class CostAllocation:
    """Phân toán chi phí"""
    allocation_id: int
    payroll_id: int (từ LƯƠNG ĐỘI)
    total_cost: float
    allocations: Dict[str, float]  # {dept_name: amount}
    created_date: datetime
```

### 7.2 Business Logic Layer

```python
class AllocationEngine:
    
    def load_allocation_rates(self, config):
        """Load tỷ lệ phân toán từ config hoặc DB"""
        pass
    
    def validate_rates(self, rates):
        """Kiểm tra tỷ lệ = 100%"""
        total = sum(rates.values())
        if abs(total - 1.0) > 0.01:
            raise ValueError(f"Tỷ lệ không = 100%: {total*100}%")
    
    def allocate(self, total_cost, rates):
        """Thực hiện phân toán"""
        self.validate_rates(rates)
        allocations = {}
        for dept, rate in rates.items():
            allocations[dept] = round(total_cost * rate, 0)
        return allocations
    
    def generate_report(self, allocations):
        """Output: Bảng phân toán"""
        pass
```

### 7.3 Testing

```python
def test_allocation_sums_to_total():
    """Test: Tổng phân toán = tổng gốc"""
    total = 1_000_000
    rates = {
        'A': 0.30, 'B': 0.25, 'C': 0.20, 
        'D': 0.10, 'E': 0.10, 'F': 0.05
    }
    
    result = engine.allocate(total, rates)
    assert sum(result.values()) == total

def test_allocation_percentage_validation():
    """Test: Tỷ lệ phải = 100%"""
    invalid_rates = {'A': 0.30, 'B': 0.25}  # Total = 55%
    
    with pytest.raises(ValueError):
        engine.allocate(1000000, invalid_rates)

def test_non_contiguous_sum():
    """Test: SUM non-contiguous range"""
    costs = [100000] * 10  # 10 rows
    selected = [costs[0:4], costs[5], costs[9]]  # E16:E19, E25, E35
    total = sum(sum(x) if isinstance(x, list) else x 
                for x in selected)
    assert total == 600000
```

---

## 8. MIGRATION PLAN - Từ Excel → Production Software

### 8.1 Phase 1: Data Import

```
BẢNG PHÂN TOÁN (Excel)
    ↓ Convert
database:
  ├─ allocation_rules table
  │  ├─ dept | percentage | code | desc
  │  └─ 6 rows of data
  │
  └─ cost_allocations table
     ├─ id | payroll_id | total | dept_name | amount
     └─ Multiple rows (6 dept × num_employees)
```

### 8.2 Phase 2: API Layer

```python
# REST API endpoints
POST   /api/allocations/calculate
  Input: {total_cost, payroll_id, month, year}
  Output: {A: 300000, B: 250000, ...}

GET    /api/allocation_rules
  Output: {dept: {name, percentage, code}}

PUT    /api/allocation_rules/{dept}
  Input: {percentage, code}
```

### 8.3 Phase 3: Reports

```
Monthly Allocation Report:
├─ Payroll Month: 12/2025
├─ Total Cost: XXX
├─ By Department:
│  ├─ Sản Xuất (30%): XXX
│  ├─ Quản Lý (25%): XXX
│  └─ ...
└─ Validation: ✓ Sum = Total
```

---

## 9. RISK & MITIGATION

| Risk | Cause | Impact | Mitigation |
|---|---|---|---|
| Rounding Error | Các số không chia hết | Mất/thừa vài đồng | Round last item to total |
| % != 100% | Config sai | Tiền mất/thừa đ.k | Auto-validation + alarm |
| Missing Dept | Sheet bị xóa | Lỗi allocation | Soft delete + archive |
| #REF! Errors | External links | Allocation fail | Fix/remove before import |

---

**PHÂN TÍCH BẢNG PHÂN TOÁN v1.0**
**Last Updated: 09/04/2025**

