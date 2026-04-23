# 🐛 ISSUES FOUND & TECHNICAL DEBT
## Các Vấn Đề Phát Hiện Từ Phân Tích Excel

---

## 1. CRITICAL ISSUES (Blockable)

### 1.1 Issue: 100+ #REF! Errors in Formulas

**Severity:** 🔴 **CRITICAL**

**Location:**
- `CÁCH TÍNH LƯƠNG` sheet: Multiple cells
- `BẢNG PHÂN TOÁN` sheet: K10, K12, and formula chains
- `PLC(2)` sheet: Entire sheet affected

**Root Cause:**
```
Referenced sheets deleted or external file links broken:
  ├─ [7]TRẠM 1 - File [7] not found (external workbook)
  ├─ [7]TRẠM 2 - File [7] not found
  ├─ [2]TRẠM 2 - File [2] not found (external workbook)
  └─ [10]PHAN_TOAN - File [10] not found
```

**Example Erroring Formula:**
```excel
K10: =REF_ERROR!$A$1          # External link broken
K12: ='[10]PHAN_TOAN'!$A$1    # File [10] missing
E14: =IF(#REF!, ...)          # Error propagation
```

**Impact:**
- ❌ BẢNG PHÂN TOÁN sheet cannot allocate costs properly
- ❌ Reports based on these cells show errors
- ❌ Cannot import data into new system without fixing
- ❌ Blocks payroll processing

**Solution:**

**Option A: Find & Restore Missing Files**
```bash
# Search for files [7], [2], [10]
find C://Users/*/Desktop -name "*TRẠM*" -type f
find C://Dropbox -name "*PHAN*TOAN*" -type f
find C://SharePoint -name "*[7]*" -type f

# If found, update links in Excel:
Data → Edit Links → Update Link Reference
```

**Option B: Rebuild Formulas**
```excel
# BEFORE:
K10: ='[10]PHAN_TOAN'!$A$1   # ERROR

# AFTER (remove external ref, use internal data):
K10: =0.05          # Direct value or lookup from local table
K12: =VLOOKUP(...)  # Use internal sheet reference

# Or if logic unclear:
K10: =IFERROR(REF_ERROR, 0)  # Temporarily suppress error
```

**Option C: Migration Path (Recommended for New System)**
```
Step 1: Document what K10, K12 SHOULD contain
Step 2: Find the business logic or source values
Step 3: Replace with explicit values/formulas
Step 4: Test against historical data
Step 5: Import into new database
```

**Recommended Action:**
- ✅ Spend 4-8 hours searching for missing Excel files
- ✅ If not found, meet with finance team to clarify intent
- ✅ Rebuild formulas based on business logic
- ✅ Create automated fix script for new data

**Timeline:** Fix before data migration (PHASE 1)

---

### 1.2 Issue: Missing Grade D/E Coefficients

**Severity:** 🔴 **CRITICAL** (For Production)

**Location:**
- `TRẠM 1` sheet, `TRẠM 2` sheet: IF statement formulas

**Current State:**
```excel
# TRẠM 1 (TRAM1.O16):
=ROUND(
  IF(N16="A", M16*9.2,
    IF(N16="B", M16*8.9,
      IF(N16="C", M16*8.6,
        IF(N16="D", M16*?, ...)  # UNKNOWN
      )
    )
  ), 
  0
)

# Known coefficients:
Grade A: 9.2
Grade B: 8.9
Grade C: 8.6
Grade D: ???  ← MISSING
Grade E: ???  ← MISSING
```

**Impact:**
- ❌ Cannot process employees with Grade D or E quality rating
- ❌ Incomplete test coverage
- ❌ System cannot go to production without this

**Solution:**

**IMMEDIATE ACTION:**
```bash
# 1. Search Excel file for Grade D/E values
CTRL+F "D"
CTRL+F "E"

# 2. Check formulas in other sheets
Search in TRẠM 2, LƯƠNG ĐỘI for pattern

# 3. Interview user/finance team:
Question: "What are the production quality grades D and E?"
Question: "What are the current coefficients for these grades?"
Question: "Have employees ever worked on Grade D/E before?"
```

**Research:**
- Open Excel file → TRẠM 1 sheet → Column N (quality_rating)
- Filter by unique values: Find all distinct grades
- If D/E appear in data, look at corresponding cells in formula

**Fallback Options:**
```
If data shows D/E coefficients:
  ├─ Extract from data values
  ├─ Reverse-engineer from results
  └─ Use interpolation (e.g., E = (D + C)/2)

If no historical data exists:
  ├─ Use temporary value (e.g., D=C, E=B)
  ├─ Archive as "TBD - waiting for clarification"
  └─ Plan to update in Phase 2
```

**Recommended Action:**
- ✅ Contact business owner (Finance Manager, HR Manager)
- ✅ Ask for official Grade D/E coefficients
- ✅ Document in SRS specification
- ✅ Update test cases

**Timeline:** Resolve in PHASE 1 (Week 2-3)

---

### 1.3 Issue: P7 Parameter Purpose Unclear

**Severity:** 🟡 **HIGH** (Blocks Validation)

**Location:**
- `LƯƠNG ĐỘI` sheet: Cell P7 = 27.0
- Used in main formula: `=(F13+H13)/P7*E13`

**Problem:**
```
The value P7 = 27.0 appears to be a "khoán" (khoan/quota) adjustment parameter.

Questions:
  1. Is 27.0 fixed or adjustable monthly?
  2. What is the business meaning? (27 working days? 27th parameter?)
  3. Can it change based on:
     ├─ Month/season?
     ├─ Department?
     ├─ Economic conditions?
     └─ Management decision?
  4. Has it ever been changed historically?
  5. Is there documentation of how it was calculated?
```

**Current Understanding:**
- Value: 27.0 (appears to be related to 27-day working month)
- Usage: Divisor to adjust total salary calculation
- Impact: If P7 changes 1%, all salaries change ~1%

**Risk:**
- ❌ If P7 is supposed to vary by month but doesn't → Salary miscalculation
- ❌ If P7 should be configurable but isn't → User frustration
- ❌ If P7 formula is complex but we're using fixed value → Wrong output

**Solution:**

**Investigate:**
```
1. Historical Analysis
   - Check 12 months of payroll
   - Extract P7 value for each month
   - Create trend report:
     
     Month | P7 Value | Changed?
     Sep   | 27.0     | No
     Oct   | 27.0     | No
     Nov   | 27.0     | No
     Dec   | 27.0     | No
     → Conclusion: Fixed value OR all the same due to copy-paste

2. Interview Key Stakeholders
   Q1: "What does P7 = 27.0 represent in your business?"
   Q2: "Is this value ever adjusted? If yes, how often and why?"
   Q3: "Does this relate to the 27-day working month?"
   Q4: "Should we make this configurable for future flexibility?"
   
3. Documentation Search
   - Look for payroll manual/procedures
   - Search email for "P7", "27.0", "khoán"
   - Check finance glossary

4. Formula Reverse-Engineering
   - Calculate: salary_gross = (base + days) / 27.0 * coef
   - If P7 = 30 (typical month days), salary would be:
     = (base + days) / 30 * coef ≈ 10% lower
   - Ask: "Would payroll be 10% different if we used 30?"
```

**Recommended Action:**
- ✅ Create configuration table for P7
- ✅ Make P7 configurable in database
- ✅ Get written documentation of P7 purpose
- ✅ Plan historical audit trail

**Temporary Solution (For Testing):**
```sql
INSERT INTO system_parameters 
(param_name, param_value, description, data_type)
VALUES 
('P7_ADJUSTMENT', '27.0', 'Khoán parameter - purpose TBD', 'DECIMAL');

-- Can be updated later:
-- UPDATE system_parameters SET param_value = '28.0' WHERE param_name = 'P7_ADJUSTMENT';
```

**Timeline:** Clarify in PHASE 1 (Week 2), make configurable in PHASE 2

---

## 2. HIGH-PRIORITY ISSUES

### 2.1 Issue: Tax Thresholds Not Verified Against 2025 Law

**Severity:** 🟡 **HIGH**

**Location:**
- `LƯƠNG ĐỘI` sheet: J13 formula (Tax calculation)

**Current Tax Law (From Excel):**
```
Tier 1: Salary ≤ 5,000,000 VND @ 5%
Tier 2: 5,000,001 - 10,000,000 @ 10% + 250,000
Tier 3: 10,000,001 - 20,000,000 @ 15% + 750,000
Tier 4: 20,000,001 - 25,000,000 @ 20% + 2,250,000
Tier 5: 25,000,001 - 65,000,000 @ 25% + 2,685,000
Tier 6: > 65,000,000 @ 35% + 10,685,000
```

**Risk:**
- ❌ Vietnam updates tax law annually (usually Dec/Jan)
- ❌ Excel may have outdated 2024 thresholds
- ❌ Using wrong thresholds = wrong tax = compliance issue
- ❌ Could trigger audit/penalties

**Solution:**

**Validation Task:**
```
1. Check Vietnam Tax Authority Website
   - Government tax website
   - Finance ministry announcements
   - Search: "Vietnam personal income tax 2025"

2. Company Finance Department
   - Ask: "Are these tax thresholds current for 2025?"
   - Request: Official tax rate schedule
   - Get: Written approval of rates used

3. Tax Consultant Review
   - Forward specification to external tax advisor
   - Get written confirmation of correctness

4. Documentation
   - Record source of each threshold
   - Note effective date (e.g., "Effective 01/01/2025")
   - Create version history
```

**Action:**
- ✅ Schedule meeting with finance/tax team
- ✅ Get written confirmation of 2025 tax rates
- ✅ Document source & effective date
- ✅ Create tax rate configuration table

**Database Schema:**
```sql
CREATE TABLE tax_brackets (
  bracket_id INT PRIMARY KEY,
  effective_date DATE,
  min_salary DECIMAL(12,2),
  max_salary DECIMAL(12,2),
  rate DECIMAL(5,4),
  base_tax DECIMAL(12,2),
  description VARCHAR(255)
);

INSERT INTO tax_brackets VALUES
(1, '2025-01-01', 0, 5000000, 0.05, 0),
(2, '2025-01-01', 5000001, 10000000, 0.10, 250000),
...
(6, '2025-01-01', 65000001, 999999999, 0.35, 10685000);
```

**Timeline:** Verify in PHASE 1, implement by PHASE 2

---

### 2.2 Issue: External Links to Deleted Workbooks

**Severity:** 🟡 **HIGH**

**Location:**
- Multiple sheets reference `[7]`, `[2]`, `[10]` external workbooks
- Examples: `='[7]TRẠM 1'!$AA$60`, `='[2]TRẠM 2'!$AA$75`

**Current Count:**
```
[7]TRẠM 1:  ~15 references found
[7]TRẠM 2:  ~5 references
[2]TRẠM 2:  ~8 references
[10]PHAN_TOAN: ~3 references

Total: ~31 broken external links
```

**Impact:**
- ❌ Any formula using these links shows #REF!
- ❌ Cannot copy/move file without breaking references
- ❌ Collaborators on different computers get errors
- ❌ Difficult to share via email/cloud

**Solution:**

**Immediate (This Month):**
```
1. Search for the files
   # Windows search
   File Explorer → Search → "TRẠM"
   
   # PowerShell
   Get-ChildItem -Path C:/ -Recurse -Filter "*TRẠM*"
   
   # Mac/Linux
   find ~ -name "*TRẠM*" 2>/dev/null

2. If found:
   ├─ Copy files to shared location
   ├─ Update workbook links via Edit → Links
   └─ Test & verify

3. If not found:
   ├─ Ask team members to locate
   ├─ Check version control (Git/SharePoint)
   ├─ Check backup/archive folders
   └─ Check colleague's computers
```

**Long-term (For New System):**
```
1. Don't use external links
2. Import all data into single database
3. Create views/reports for different departments
4. Version control via Git (not Excel files)
```

**Action:**
- ✅ Spend 2-4 hours locating files [7], [2], [10]
- ✅ If found: Update links & test
- ✅ If not found: Document & plan workaround
- ✅ Plan to eliminate external links in new system

**Timeline:** Resolve before migration (PHASE 1)

---

## 3. MEDIUM-PRIORITY ISSUES

### 3.1 Issue: No Version Control / Audit Trail

**Severity:** 🟠 **MEDIUM**

**Problem:**
- ❌ Excel file has no version history (not in Git)
- ❌ Cannot track who changed what when
- ❌ No protection against accidental overwrites
- ❌ Multiple people sharing one file = conflicts
- ❌ Finance team cannot verify data integrity

**Risk:**
- Changes not tracked
- Blame assignment impossible
- Cannot rollback to previous version
- Compliance issues (audit trail required)

**Solution:**

**For Current Excel File:**
```
1. Enable Excel version history
   ├─ Save to OneDrive/SharePoint (auto version)
   ├─ File → Info → Version History
   └─ Allows recovery of past versions

2. Password protection
   ├─ Review → Protect Sheet
   ├─ Set cell-level permissions
   └─ Require password for changes

3. Create backup copies
   ├─ Save monthly: PAYROLL_2025_01.xlsx
   ├─ PAYROLL_2025_02.xlsx
   ├─ PAYROLL_2025_03.xlsx
   └─ Archive with date stamps
```

**For New System:**
```
Database Changes → Audit Log:
  timestamp | user | table | action | old_value | new_value
  ├─ All payroll updates logged
  ├─ User can see who approved what
  ├─ Can review changes before finalizing
  └─ Supports compliance audits
```

**Action:**
- ✅ Enable Excel version history immediately
- ✅ Move file to OneDrive/SharePoint
- ✅ Create archive copies (monthly)
- ✅ Plan audit logging for new system

**Timeline:** Implement for Excel immediately, new system in PHASE 4

---

### 3.2 Issue: Manual Parameter Adjustments Not Documented

**Severity:** 🟠 **MEDIUM**

**Problem:**
- ❌ P7 value can be manually changed without tracking
- ❌ Tax rates could be updated without record
- ❌ Allocation percentages changeable but not logged
- ❌ No "why" or "by whom"
- ❌ Finance cannot explain historical differences

**Risk:**
- Inconsistent calculations
- Inability to investigate anomalies
- Compliance & audit challenges
- Team doesn't understand logic

**Solution:**

**Create Parameter Audit Table:**
```sql
CREATE TABLE parameter_history (
  change_id INT AUTO_INCREMENT PRIMARY KEY,
  param_name VARCHAR(100),
  old_value VARCHAR(255),
  new_value VARCHAR(255),
  changed_by INT,
  changed_date TIMESTAMP,
  reason VARCHAR(500),
  approved_by INT
);

-- Example:
INSERT INTO parameter_history VALUES
(1, 'P7_ADJUSTMENT', '27.0', '27.5', 
 4, '2025-04-15', 'April adjustment per finance approval', 1);
```

**For Excel:**
```
Create "PARAMETER_LOG" sheet:

Date       | User | Parameter | Old | New | Reason | Approved_By
2025-03-15 | John | P7        | 27  | 27  | Review | Finance_Manager
2025-04-01 | Jane | TAX_RATE  | ... | ... | Update | CEO
```

**Action:**
- ✅ Create parameter history table/log
- ✅ Require approval for parameter changes
- ✅ Document reason for each change
- ✅ Track user & timestamp
- ✅ Make parameters easily auditable

**Timeline:** Implement in PHASE 3, apply retroactively

---

## 4. LOW-PRIORITY ISSUES (Technical Debt)

### 4.1 Issue: Inconsistent Column Naming

**Location:** Excel sheets have Vietnamese column names, sometimes inconsistent

**Examples:**
```
Sheet TRẠM 1:  "Sản_Lượng_ĐC" vs "san_luong_dc" (mixed case/spacing)
Sheet CÁCH TÍNH LƯƠNG: "Hệ_Số" vs "HeSo" vs "he_so" (inconsistent)
```

**Solution:**
```sql
-- Create standardized column catalog
CREATE TABLE column_catalog (
  excel_column_name VARCHAR(100),
  database_column_name VARCHAR(100),
  business_name VARCHAR(200),
  data_type VARCHAR(50),
  format VARCHAR(100),
  notes VARCHAR(500)
);

-- Then migrate with consistent names
ALTER TABLE employees RENAME COLUMN msnv TO employee_id;
```

**Timeline:** Fix during database design (PHASE 2)

---

### 4.2 Issue: Performance - Non-Contiguous Formulas

**Severity:** 🟢 **LOW** (Functional but slow)

**Location:**
- `BẢNG PHÂN TOÁN` sheet: E15 formula uses non-contiguous ranges
- `=SUM(E16:E19, E35, E20, E25, E38, E30)`

**Problem:**
- Harder to maintain (are all rows included?)
- Slower calculation with large datasets
- Error-prone (easy to miss a range)

**Better Approach:**
```sql
-- Instead of non-contiguous SUM, use structured query:
SELECT SUM(amount) FROM cost_allocations
WHERE row_id IN (16, 17, 18, 19, 35, 20, 25, 38, 30);

-- Or use WHERE clause:
SELECT SUM(amount) FROM cost_allocations
WHERE allocation_type IN ('PayrollA', 'PayrollB', 'Adjustment');
```

**Timeline:** Fix in new system during PHASE 2

---

## 5. SUMMARY TABLE

| Issue | Priority | Status | Owner | Deadline |
|---|---|---|---|---|
| #REF! errors (100+) | 🔴 CRITICAL | TBD | Finance+IT | PHASE 1 |
| Missing Grade D/E | 🔴 CRITICAL | TBD | Finance | PHASE 1 |
| P7 purpose unclear | 🟡 HIGH | TBD | Finance | PHASE 1 |
| Tax thresholds (2025) | 🟡 HIGH | TBD | Finance+Tax | PHASE 1 |
| External links broken | 🟡 HIGH | TBD | IT | PHASE 1 |
| No version control | 🟠 MEDIUM | TBD | IT | Now |
| Manual changes not logged | 🟠 MEDIUM | TBD | IT | PHASE 2 |
| Naming inconsistencies | 🟢 LOW | TBD | BA | PHASE 2 |
| Non-contiguous formulas | 🟢 LOW | TBD | Dev | PHASE 2 |

---

## 6. NEXT STEPS

**THIS WEEK:**
- [ ] Search for files [7], [2], [10]
- [ ] Contact Finance Manager about P7 & Grade D/E
- [ ] Get tax threshold confirmation for 2025
- [ ] Enable Excel version control (SharePoint)

**NEXT WEEK:**
- [ ] Document all findings
- [ ] Create issue tracking board
- [ ] Plan remediation for each issue
- [ ] Assign owners

**DURING PHASE 1:**
- [ ] Fix all CRITICAL issues
- [ ] Resolve HIGH priority items
- [ ] Plan MEDIUM priority fixes

---

**ISSUES & TECHNICAL DEBT v1.0**
**Last Updated: 09/04/2025**
**Status: Ready for Triage**

