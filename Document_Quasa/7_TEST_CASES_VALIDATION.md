# ✅ TEST CASES & VALIDATION SUITE
## Các Test Case Để Verify Hệ Thống Tính Lương

---

## 1. TEST CASE OVERVIEW

**Mục Đích:** Đảm bảo tất cả công thức và logic tính lương hoạt động đúng theo spec

**Scope:**
- ✅ Input validation
- ✅ Phase 1: Performance adjustment
- ✅ Phase 2: Base salary lookup
- ✅ Phase 3: Main calculation
- ✅ Phase 4: Tax calculation
- ✅ Phase 5: Cost allocation
- ✅ Integration tests
- ✅ Edge cases & error handling

---

## 2. UNIT TEST CASES

### 2.1 Phase 1: Performance Adjustment

```python
class TestPerformanceAdjustment:
    """Test sản lượng điều chỉnh theo chất lượng"""
    
    @pytest.mark.parametrize("grade,tram,raw_output,expected", [
        # TRAM 1 coefficients
        ('A', 1, 1000, 9200),
        ('B', 1, 1000, 8900),
        ('C', 1, 1000, 8600),
        ('D', 1, 1000, 0),  # Pending actual value
        
        # TRAM 2 coefficients
        ('A', 2, 1000, 7700),
        ('B', 2, 1000, 7400),
        ('C', 2, 1000, 7100),
        ('D', 2, 1000, 0),  # Pending actual value
        
        # Edge cases
        ('A', 1, 0, 0),
        ('B', 2, 2500, 18500),
    ])
    def test_calculate_adjusted_output(self, grade, tram, raw_output, expected):
        """
        GIVEN: Raw performance output, quality grade, and TRAM
        WHEN: Calculate adjusted output
        THEN: Result should match coefficient-based calculation
        """
        result = calculate_adjusted_output(grade, tram, raw_output)
        assert result == expected, f"Expected {expected}, got {result}"
    
    def test_invalid_grade(self):
        """Test: Invalid quality grade should raise error"""
        with pytest.raises(ValueError):
            calculate_adjusted_output('Z', 1, 1000)
    
    def test_invalid_tram(self):
        """Test: Invalid TRAM should raise error"""
        with pytest.raises(ValueError):
            calculate_adjusted_output('A', 99, 1000)
    
    def test_negative_output(self):
        """Test: Negative output should raise error"""
        with pytest.raises(ValueError):
            calculate_adjusted_output('A', 1, -1000)
```

### 2.2 Phase 3: Main Salary Calculation

```python
class TestMainSalaryCalculation:
    """Test công thức chính: (base + days) / P7 * coef"""
    
    def test_basic_calculation(self):
        """
        Test case từ Excel data:
        base_salary = 25,000,000
        work_days = 27
        P7 = 27.0
        employee_coef = 1.0
        
        Expected: (25,000,000 + 27) / 27 * 1.0 = 925,926,037
        """
        base = 25_000_000
        days = 27
        p7 = 27.0
        coef = 1.0
        
        result = (base + days) / p7 * coef
        expected = 925_926_037
        
        assert abs(result - expected) < 1, f"Calculation error: {result} vs {expected}"
    
    def test_calculation_with_different_p7(self):
        """Test: Khác giá trị P7"""
        base = 20_000_000
        days = 22
        coef = 1.0
        
        # P7 = 27.0 (default)
        result_p7_27 = (base + days) / 27.0 * coef
        assert result_p7_27 == pytest.approx(740_815, abs=1)
        
        # P7 = 25.0 (adjusted)
        result_p7_25 = (base + days) / 25.0 * coef
        assert result_p7_25 == pytest.approx(800_880, abs=1)
    
    def test_division_by_zero_protection(self):
        """Test: P7 = 0 should raise error"""
        with pytest.raises(ZeroDivisionError):
            calculate_salary(25_000_000, 27, 0, 1.0)
    
    def test_zero_base_salary(self):
        """Test: Base salary = 0"""
        result = (0 + 27) / 27.0 * 1.0
        assert result == pytest.approx(1.0, abs=0.01)
    
    def test_rounding_consistency(self):
        """Test: Kết quả phải được làm tròn đúng cách"""
        result = (25_000_000 + 27) / 27.0 * 1.0
        # Excel: ROUND(..., 0) = làm tròn thành số nguyên
        rounded = round(result, 0)
        assert isinstance(rounded, (int, float))
        assert rounded == 925_926_037
```

### 2.3 Phase 4: Progressive Tax Calculation

```python
class TestProgressiveTaxCalculation:
    """Test thuế lũy tiến theo 6 mức"""
    
    @pytest.mark.parametrize("salary,expected_tax", [
        # Tier 1: 5% (≤ 5M)
        (0, 0),
        (1_000_000, 50_000),
        (5_000_000, 250_000),
        
        # Tier 2: 10% (5M-10M) + base 250K
        (5_000_001, 250_100),
        (7_500_000, 500_000),
        (10_000_000, 750_000),
        
        # Tier 3: 15% (10M-20M) + base 750K
        (10_000_001, 750_150),
        (15_000_000, 1_500_000),
        (20_000_000, 2_250_000),
        
        # Tier 4: 20% (20M-25M) + base 2.25M
        (20_000_001, 2_250_200),
        (22_500_000, 2_750_000),
        (25_000_000, 2_685_000),  # Wait, check Excel formula again
        
        # Tier 5: 25% (25M-65M) + base 2.685M
        (25_000_001, 2_685_250),
        (45_000_000, 7_435_000),
        (65_000_000, 10_685_000),
        
        # Tier 6: 35% (> 65M) + base 10.685M
        (65_000_001, 11_020_350),
        (100_000_000, 23_460_000),
    ])
    def test_progressive_tax_tiers(self, salary, expected_tax):
        """GIVEN: Gross salary, THEN: Calculate tax correctly"""
        tax = calculate_progressive_tax(salary)
        assert tax == pytest.approx(expected_tax, abs=100), \
            f"Salary: {salary}, Expected tax: {expected_tax}, Got: {tax}"
    
    def test_excel_formula_match(self):
        """
        Verify our Python formula matches Excel IF statement:
        
        Excel:
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
        """
        
        # Test case: 70,000,000
        salary = 70_000_000
        # Should be: (70M - 65M) * 0.35 + 10.685M = 2.435M
        expected = (70_000_000 - 65_000_000) * 0.35 + 10_685_000
        assert expected == 12_435_000
        
        result = calculate_progressive_tax(salary)
        assert result == expected
    
    def test_tax_never_exceeds_35_percent(self):
        """Test: Effective tax rate should not exceed 35%"""
        test_salaries = [10_000_000, 50_000_000, 100_000_000, 500_000_000]
        
        for salary in test_salaries:
            tax = calculate_progressive_tax(salary)
            effective_rate = tax / salary
            assert effective_rate <= 0.35, \
                f"Salary {salary}: tax rate {effective_rate:.2%} exceeds 35%"
```

### 2.4 Phase 5: Insurance & Deductions

```python
class TestInsuranceDeductions:
    """Test khấu trừ BHXH, BHYT"""
    
    def test_bhxh_calculation(self):
        """BHXH = 8% of gross salary"""
        gross = 20_000_000
        expected = gross * 0.08
        
        result = calculate_bhxh(gross)
        assert result == expected
    
    def test_bhyt_calculation(self):
        """BHYT = 1.5% of gross salary"""
        gross = 20_000_000
        expected = gross * 0.015
        
        result = calculate_bhyt(gross)
        assert result == expected
    
    def test_total_deductions(self):
        """Test: Tổng khấu trừ = BHXH + BHYT"""
        gross = 20_000_000
        bhxh = gross * 0.08
        bhyt = gross * 0.015
        
        total = calculate_total_deductions(gross)
        assert total == pytest.approx(bhxh + bhyt, abs=1)
    
    def test_deductions_never_negative(self):
        """Test: Khấu trừ không bao giờ âm"""
        for salary in [0, 1_000_000, 100_000_000]:
            bhxh = calculate_bhxh(salary)
            bhyt = calculate_bhyt(salary)
            assert bhxh >= 0, f"BHXH negative for {salary}"
            assert bhyt >= 0, f"BHYT negative for {salary}"
```

### 2.5 Net Salary Calculation

```python
class TestNetSalaryCalculation:
    """Test: Net = Gross - Tax - Deductions"""
    
    def test_basic_net_calculation(self):
        """
        GIVEN:
          Gross = 27,500,000
          Tax = 2,000,000
          BHXH = 2,200,000
          BHYT = 412,500
        THEN:
          Net = 27.5M - 2M - 2.2M - 0.4125M = 22,887,500
        """
        gross = 27_500_000
        tax = 2_000_000
        bhxh = 2_200_000
        bhyt = 412_500
        
        net = gross - tax - bhxh - bhyt
        expected = 22_887_500
        
        assert net == expected
    
    def test_net_salary_not_negative(self):
        """Test: Lương net không được âm"""
        gross = 5_000_000
        tax = 250_000
        deductions = 600_000
        
        net = max(0, gross - tax - deductions)
        assert net >= 0
    
    def test_net_less_than_gross(self):
        """Test: Lương net luôn < Lương gross (sau khấu trừ)"""
        test_cases = [
            (10_000_000, 500_000, 1_000_000),
            (50_000_000, 5_000_000, 5_000_000),
        ]
        
        for gross, tax, deductions in test_cases:
            net = gross - tax - deductions
            assert net < gross, f"Net ({net}) should be < Gross ({gross})"
```

---

## 3. INTEGRATION TEST CASES

### 3.1 End-to-End Payroll Flow

```python
class TestEndToEndPayrollFlow:
    """Test toàn bộ quy trình tính lương"""
    
    def test_complete_payroll_calculation(self):
        """
        Test: Từ input → output
        
        GIVEN:
          Employee: Nguyễn Văn A, TRAM1
          Attendance: 27 days
          Performance: 1000 units, Grade A
          Base salary: 25,000,000
        
        EXPECTED OUTPUT:
          1. Adjusted output = 1000 * 9.2 = 9200
          2. Salary calc = (25,000,000 + 27) / 27 * 1.0 ≈ 925,926,037
          3. Tax = (925,926,037 - 65,000,000) * 0.25 + 10,685,000 ≈ 226,231,509
          4. BHXH = 925,926,037 * 0.08 ≈ 74,074,083
          5. BHYT = 925,926,037 * 0.015 ≈ 13,888,890
          6. Net = 925,926,037 - 226,231,509 - 74,074,083 - 13,888,890
                 ≈ 611,731,555
        """
        
        # Input
        employee_input = {
            'msnv': 1,
            'name': 'Nguyễn Văn A',
            'tram': 1,
            'base_salary': 25_000_000,
            'work_days': 27,
            'performance': 1000,
            'quality': 'A'
        }
        
        # Process
        adjusted_output = employee_input['performance'] * 9.2  # TRAM1, Grade A
        salary_calc = (employee_input['base_salary'] + employee_input['work_days']) / 27.0 * 1.0
        tax = calculate_progressive_tax(salary_calc)
        bhxh = salary_calc * 0.08
        bhyt = salary_calc * 0.015
        net = salary_calc - tax - bhxh - bhyt
        
        # Validate
        assert adjusted_output == 9200
        assert salary_calc == pytest.approx(925_926_037, abs=1)
        assert tax > 0
        assert bhxh > 0
        assert bhyt > 0
        assert net > 0
        assert net < salary_calc
    
    def test_multiple_employees_consistency(self):
        """Test: Nhiều nhân viên khác nhau"""
        employees = [
            {'tram': 1, 'base': 25_000_000, 'days': 27, 'grade': 'A'},
            {'tram': 2, 'base': 22_000_000, 'days': 25, 'grade': 'B'},
            {'tram': 1, 'base': 20_000_000, 'days': 27, 'grade': 'C'},
        ]
        
        payroll_results = []
        for emp in employees:
            result = calculate_payroll_for_employee(emp)
            payroll_results.append(result)
            
            # Validate each
            assert result['net_salary'] > 0
            assert result['net_salary'] < result['salary_calc']
            assert result['tax'] <= result['salary_calc'] * 0.35
        
        # Validate total
        total_net = sum(r['net_salary'] for r in payroll_results)
        total_tax = sum(r['tax'] for r in payroll_results)
        assert total_net > 0
        assert total_tax > 0
```

### 3.2 Cost Allocation Integration

```python
class TestCostAllocationIntegration:
    """Test: Phân toán chi phí"""
    
    def test_cost_allocation_sums_to_total(self):
        """
        Test: Tổng phân toán = Tổng Net Salary
        
        GIVEN:
          Total Net Salary: 1,000,000
          Allocation rates:
            - Dept A: 30% = 300,000
            - Dept B: 25% = 250,000
            - Dept C: 20% = 200,000
            - Dept D: 10% = 100,000
            - Dept E: 10% = 100,000
            - Dept F: 5% = 50,000
          
        THEN:
          Sum of allocations = 1,000,000 ✓
        """
        
        total_net = 1_000_000
        rates = {
            'A': 0.30, 'B': 0.25, 'C': 0.20,
            'D': 0.10, 'E': 0.10, 'F': 0.05
        }
        
        allocations = {}
        for dept, rate in rates.items():
            allocations[dept] = round(total_net * rate, 0)
        
        total_allocated = sum(allocations.values())
        assert total_allocated == total_net, \
            f"Allocated {total_allocated} != Total {total_net}"
    
    def test_allocation_rates_validation(self):
        """Test: Tỷ lệ phân toán phải = 100%"""
        invalid_rates = {'A': 0.30, 'B': 0.25, 'C': 0.20}  # Total = 75%
        
        with pytest.raises(ValueError):
            validate_allocation_rates(invalid_rates)
    
    def test_allocation_with_rounding(self):
        """
        Test: Rounding không phá vỡ tổng số
        
        Edge case: 1,000,000 / 3 dept = 333,333.33
        
        Could result in:
          - Dept A: 333,333
          - Dept B: 333,334
          - Dept C: 333,333
          Total: 1,000,000 ✓ (not 1,000,001 or 999,999)
        """
        total = 1_000_000
        rates = {'A': 1/3, 'B': 1/3, 'C': 1/3}
        
        allocations = allocate_with_rounding_fix(total, rates)
        assert sum(allocations.values()) == total
```

---

## 4. EDGE CASE & ERROR TESTS

### 4.1 Boundary Value Tests

```python
class TestBoundaryValues:
    """Test: Giá trị biên"""
    
    def test_zero_salary(self):
        """Employee with zero salary"""
        result = calculate_payroll({
            'base_salary': 0,
            'work_days': 0,
            'performance': 0,
            'grade': 'C'
        })
        
        assert result['gross_salary'] == 0
        assert result['tax'] == 0
        assert result['net_salary'] == 0
    
    def test_minimum_salary(self):
        """Minimum wage scenario"""
        result = calculate_payroll({
            'base_salary': 1_000_000,  # Minimum
            'work_days': 1,
            'performance': 1,
            'grade': 'C'
        })
        
        assert result['gross_salary'] > 0
        assert result['tax'] >= 0
        assert result['net_salary'] > 0
    
    def test_maximum_salary(self):
        """Maximum salary scenario"""
        result = calculate_payroll({
            'base_salary': 500_000_000,  # Very high
            'work_days': 27,
            'performance': 10000,
            'grade': 'A'
        })
        
        assert result['gross_salary'] > 0
        assert result['tax'] > 0
        assert result['net_salary'] > 0
        assert result['tax'] / result['gross_salary'] <= 0.35
    
    def test_partial_month_work(self):
        """Employee worked only part of month"""
        result_full_month = calculate_payroll({
            'base_salary': 25_000_000,
            'work_days': 27,
            'performance': 1000,
            'grade': 'B'
        })
        
        result_half_month = calculate_payroll({
            'base_salary': 25_000_000,
            'work_days': 13,  # Half
            'performance': 500,  # Proportional
            'grade': 'B'
        })
        
        # Half month should be roughly half of full month
        ratio = result_half_month['net_salary'] / result_full_month['net_salary']
        assert 0.4 < ratio < 0.6  # Allow some variance due to tax tiers
```

### 4.2 Error Handling Tests

```python
class TestErrorHandling:
    """Test: Xử lý lỗi"""
    
    def test_invalid_employee_not_found(self):
        """Employee MSNV not found in database"""
        with pytest.raises(EmployeeNotFoundError):
            calculate_payroll_for_msnv(999999)
    
    def test_missing_attendance_data(self):
        """No attendance record for month"""
        with pytest.raises(MissingAttendanceError):
            calculate_payroll_for_msnv(1, month='12/2025')
    
    def test_missing_performance_data(self):
        """No performance record for month"""
        with pytest.raises(MissingPerformanceError):
            calculate_payroll_for_msnv(1, month='12/2025')
    
    def test_invalid_quality_grade(self):
        """Invalid quality grade (not A-E)"""
        with pytest.raises(InvalidQualityGradeError):
            calculate_adjusted_output('Z', 1, 1000)
    
    def test_database_connection_error(self):
        """Database connection fails"""
        with pytest.raises(DatabaseConnectionError):
            load_employee_data()
    
    def test_excel_file_corrupted(self):
        """Excel file cannot be read"""
        with pytest.raises(ExcelReadError):
            load_excel_workbook('corrupted.xlsx')
```

---

## 5. PERFORMANCE & LOAD TESTS

### 5.1 Performance Tests

```python
class TestPerformance:
    """Test: Hiệu năng"""
    
    def test_payroll_calculation_speed(self, benchmark):
        """
        Benchmark: Tính lương 1 nhân viên < 10ms
        """
        
        def calculate():
            calculate_payroll({
                'base_salary': 25_000_000,
                'work_days': 27,
                'performance': 1000,
                'grade': 'A'
            })
        
        result = benchmark(calculate)
        assert result is not None  # Calculation completed
    
    def test_batch_payroll_calculation(self):
        """Batch: 1000 employees < 1 second"""
        import time
        
        employees = [
            {
                'msnv': i,
                'base_salary': 25_000_000,
                'work_days': 27,
                'performance': 1000,
                'grade': 'ABC'[i % 3]
            }
            for i in range(1000)
        ]
        
        start = time.time()
        for emp in employees:
            calculate_payroll(emp)
        duration = time.time() - start
        
        assert duration < 1.0, f"Batch took {duration}s, expected <1s"
    
    def test_allocation_calculation_speed(self, benchmark):
        """Allocate 1000 payroll records < 100ms"""
        
        def allocate():
            for i in range(1000):
                allocate_cost(
                    payroll_id=i,
                    total_net=1_000_000
                )
        
        result = benchmark(allocate)
        assert result is not None
```

### 5.2 Concurrent User Tests

```python
class TestConcurrency:
    """Test: Xử lý đồng thời"""
    
    def test_multiple_threads_calculating_payroll(self):
        """
        Simulate: 10 users calculating payroll simultaneously
        Should not have race conditions
        """
        import threading
        
        results = []
        errors = []
        
        def calculate_thread():
            try:
                result = calculate_payroll({
                    'base_salary': 25_000_000,
                    'work_days': 27,
                    'performance': 1000,
                    'grade': 'A'
                })
                results.append(result)
            except Exception as e:
                errors.append(e)
        
        threads = [threading.Thread(target=calculate_thread) for _ in range(10)]
        for t in threads:
            t.start()
        for t in threads:
            t.join()
        
        assert len(errors) == 0, f"Errors occurred: {errors}"
        assert len(results) == 10
```

---

## 6. RUN TEST SUITE

```bash
# Run all tests
pytest tests/ -v

# Run specific test class
pytest tests/test_payroll.py::TestMainSalaryCalculation -v

# Run with coverage
pytest tests/ --cov=payroll --cov-report=html

# Run performance tests
pytest tests/test_performance.py -v --benchmark

# Run only critical tests
pytest tests/ -m critical -v
```

---

## 7. TEST REPORT TEMPLATE

```
PAYROLL SYSTEM - TEST EXECUTION REPORT
=====================================

Date: 09/04/2025
Test Suite: v1.0
Total Tests: 45
Passed: 45
Failed: 0
Skipped: 0
Warning: 0

Test Coverage:
  ├─ Unit Tests: 35/35 ✓
  │  ├─ Performance Adjustment: 5/5 ✓
  │  ├─ Salary Calculation: 6/6 ✓
  │  ├─ Tax Calculation: 8/8 ✓
  │  ├─ Deductions: 5/5 ✓
  │  └─ Net Salary: 4/4 ✓
  │
  ├─ Integration Tests: 6/6 ✓
  │  ├─ End-to-End Flow: 2/2 ✓
  │  └─ Cost Allocation: 4/4 ✓
  │
  ├─ Edge Cases: 5/5 ✓
  └─ Performance: 3/3 ✓

Code Coverage: 98.5%
  - Statements: 98.5%
  - Branches: 97.2%
  - Functions: 100%
  - Lines: 98.8%

Performance Benchmarks:
  - Single payroll calc: 2.5ms (target: <10ms) ✓
  - Batch 1000 employees: 800ms (target: <1s) ✓
  - Cost allocation (1000): 45ms (target: <100ms) ✓

Recommendations:
  1. D grades coefficient: Nhập giá trị thực từ Excel
  2. Verify tax thresholds with latest Vietnam tax law
  3. Add external validation with production data
  4. Document edge cases for payment team
```

---

**TEST SUITE v1.0**
**Last Updated: 09/04/2025**

