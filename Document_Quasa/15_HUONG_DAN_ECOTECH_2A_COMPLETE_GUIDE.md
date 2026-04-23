# 📑 HƯỚNG DẪN TOÀN BỘ ECOTECH 2A MODULE - 4 NHÓM

## 📊 CẤU TRÚC ECOTECH 2A PAYROLL SYSTEM

```
ECOTECH 2A - PAYROLL SYSTEM ARCHITECTURE
═════════════════════════════════════════════════════════════════════════════════

┌──────────────────────────────────────────────────────────────────────────────┐
│                          NHÓM 1: DỮ LIỆU NỀN                                 │
│                    (Master Data - Configuration Layer)                       │
├─────────────────┬─────────────────┬──────────────────┬──────────────────────┤
│ Hồ Sơ Lao Động  │ Cài Đơn Giá     │ Cài Hệ Số        │ Quản Lý Ngày Lễ [NEW]│
│ (Employees)     │ (Salary Scale)  │ (Parameters)     │ (Holiday Calendar)   │
├─────────────────┴─────────────────┴──────────────────┴──────────────────────┤
│ • Tĩnh (ít thay đổi)                                                        │
│ • Tham chiếu bởi Nhóm 2 & 3                                                 │
│ • Cần audit trail                                                           │
└──────────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌──────────────────────────────────────────────────────────────────────────────┐
│                        NHÓM 2: PHÁT SINH                                     │
│                 (Transactions - Input Layer Hàng Tháng)                      │
├──────────────┬──────────────────┬──────────────┬──────────┬────────────────┤
│ Chấm Công    │ Công Nợ & Tạm   │ Phụ Cấp &    │ Đánh Giá  │ Bổ Công        │
│ (Attendance) │ Ứng [NEW]        │ Chế Độ [NEW] │ Kỹ Thuật  │ Chăm Sóc [NEW] │
│              │ (Advances)       │ (Allowances) │(Performan│ (Care Adjust)  │
├──────────────┴──────────────────┴──────────────┴──────────┴────────────────┤
│ • Động (thay đổi hàng tháng)                                                │
│ • Input cho Nhóm 3                                                          │
│ • Cần xác nhận bởi quản lý                                                  │
└──────────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌──────────────────────────────────────────────────────────────────────────────┐
│                        NHÓM 3: XỬ LÝ                                         │
│                  (Processing - Calculation Layer)                            │
├─────────────────────────────┬──────────────┬───────────────────────────────┤
│ Tính Lương Chính            │ Truy Thu &   │ Cân Đối & Chốt Quỹ [NEW]    │
│ (Main Payroll Calculation)  │ Truy Lãnh    │ (Reconciliation)            │
│ └─ 5 PHASES                 │ (Recovery)   │ └─ Verify & Confirm         │
│    1. Hiệu Suất             │              │ • Payment Approval          │
│    2. Lương Cơ Bản          │              │ • Audit Trail               │
│    3. Tính Lương Chính      │              │ • Ready to Pay              │
│    4. Thuế & BHXH           │              │                             │
│    5. Điều Chỉnh & Net      │              │                             │
├─────────────────────────────┴──────────────┴───────────────────────────────┤
│ • Xử lý logic phức tạp                                                      │
│ • Là "trái tim" của hệ thống (sinh ra lương NET)                            │
│ • Kết quả: Payroll records (tất cả nhân viên)                              │
└──────────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌──────────────────────────────────────────────────────────────────────────────┐
│                       NHÓM 4: BÁO CÁO                                        │
│                    (Reporting - Output Layer)                               │
├─────────────┬──────────────┬──────────────┬──────────────┬─────────────────┤
│ Báo Cáo     │ Báo Cáo      │ Báo Cáo      │ Báo Cáo      │ Dashboard &     │
│ Lương Chi   │ Tóm Tắt      │ Phân Bổ      │ Thuế & BHXH  │ Quản Lý         │
│ Tiết        │ (Summary)    │ Chi Phí      │ (Tax/Ins)    │ (Management)    │
│ (Details)   │              │ (Allocation) │              │                 │
├─────────────┴──────────────┴──────────────┴──────────────┴─────────────────┤
│ • PAYSLIP (PDF cá nhân)                                                     │
│ • PAYROLL REGISTER (Excel)                                                  │
│ • KPI & Trend Analysis                                                      │
│ • Nộp cục thuế, bảo hiểm                                                    │
└──────────────────────────────────────────────────────────────────────────────┘
                                    ↓
                       LƯƠNG ĐƯỢC THANH TOÁN CHO NHÂN VIÊN
```

---

## 📚 DANH SÁCH FILE TẠISCHUYÊN BIỆT

### **NHÓM 1: DỮ LIỆU NỀN**
- **File:** [11_NHÓM_1_DỮ_LIỆU_NỀN.md](11_NHOМ_1_DU_LIEU_NEN.md)
- **Mục Đích:** Cấu hình master data, tham số hệ thống
- **Bao Gồm:**
  - 2.1: Hồ Sơ Lao Động (Employees)
  - 2.2: Cài Đơn Giá (Salary Scale)
  - 2.3: Cài Hệ Số (System Parameters)
  - 2.4: Quản Lý Ngày Lễ (Holiday Calendar) [NEW]
  - 2.5: Tiêu Chuẩn QTKT (Accounting Standards) [NEW]
- **Người Dùng:** Admin, IT, Kế Toán Trưởng
- **Tần Suất:** Setup 1 lần, update theo quy định (năm hoặc khi thay đổi lương)
- **Kiểm Tra:** Checklist 14 items

### **NHÓM 2: PHÁT SINH**
- **File:** [12_NHÓM_2_PHÁT_SINH.md](12_NHOМ_2_PHAT_SINH.md)
- **Mục Đích:** Input dữ liệu hàng tháng
- **Bao Gồm:**
  - 2.1: Chấm Công (Attendance)
  - 2.2: Công Nợ & Tạm Ứng (Advances) [NEW]
  - 2.3: Phụ Cấp & Chế Độ (Allowances) [NEW]
  - 2.4: Đánh Giá Kỹ Thuật (Performance)
  - 2.5: Bổ Công Chăm Sóc (Care Adjustments) [NEW]
- **Người Dùng:** Quản Lý Bộ Phận, Nhân Sự, Kế Toán
- **Tần Suất:** Hàng tháng (vào ngày 1-20 của tháng)
- **Kiểm Tra:** Workflow phê duyệt 2-3 cấp

### **NHÓM 3: XỬ LÝ**
- **File:** [13_NHÓM_3_XỬ_LÝ.md](13_NHOМ_3_XU_LY.md)
- **Mục Đích:** Tính lương thực tế
- **Bao Gồm:**
  - 2.1: Tính Lương Chính - 5 PHASES
    - PHASE 1: Điều Chỉnh Hiệu Suất (Coefficient)
    - PHASE 2: Lookup Lương Cơ Bản
    - PHASE 3: Công Thức Tính Lương Chính ⚠️ **NEED CLARIFICATION: P7 parameter**
    - PHASE 4: Thuế & Bảo Hiểm (6-tier tax)
    - PHASE 5: Điều Chỉnh & Lương Net
  - 2.2: Truy Thu & Truy Lãnh (Recovery)
  - 2.3: Cân Đối & Chốt Quỹ (Reconciliation) [NEW]
- **Người Dùng:** Kế Toán, IT (quản trị DB)
- **Tần Suất:** 1-2 lần mỗi tháng (tính + verify)
- **Sql:** Có procedure tính toán tự động

### **NHÓM 4: BÁO CÁO**
- **File:** [14_NHÓM_4_BÁO_CÁO.md](14_NHOМ_4_BAO_CAO.md)
- **Mục Đích:** Xuất báo cáo cho các bên liên quan
- **Bao Gồm:**
  - 2.1: Báo Cáo Lương Chi Tiết
    - PAYSLIP (PDF cá nhân)
    - PAYROLL REGISTER (Excel)
  - 2.2: Báo Cáo Tóm Tắt (Summary)
  - 2.3: Báo Cáo Phân Bổ Chi Phí (Allocation)
  - 2.4: Báo Cáo Thuế & Bảo Hiểm (Tax/Insurance)
  - 2.5: Dashboard & Quản Lý (Management)
- **Người Dùng:** Kế Toán, Tài Chính, Quản Lý, Nhân Viên
- **Tần Suất:** Hàng tháng (sau khi Nhóm 3 hoàn tất)

---

## ⚡ QUICK START GUIDE

### **Ngày 1: Setup Nhóm 1 (DỮ LIỆU NỀN)**

```
🎯 MỤC ĐÍCH: Chuẩn Bị Dữ Liệu Cơ Bản

📋 CHECKLIST:
  ☐ 1. Nhập danh sách nhân viên (MSNV, Tên, Vị Trí, Bộ Phận, Trạm)
  ☐ 2. Nhập bảng lương cơ bản (Grade A-E, Trạm 1-2, Lương, Hệ Số)
  ☐ 3. Cài đặt hệ số (P7, BHXH, BHYT, Thuế)
  ☐ 4. Nhập danh sách ngày lễ (năm hiện tại)
  ☐ 5. Cài đặt mã kế toán (Department → Accounting Code)

📝 FILE HỖ TRỢ:
  → [11_NHÓM_1_DỮ_LIỆU_NỀN.md](11_NHOМ_1_DU_LIEU_NEN.md)
     └─ Section 2-6: Chi tiết từng module
     └─ Ví dụ dữ liệu & SQL schema

⏱️ THỜI GIAN: 2-3 ngày
👤 NGƯỜI PHỤ TRÁCH: Admin IT + Kế Toán Trưởng
```

### **Ngày 4-20: Nhập Nhóm 2 (PHÁT SINH HÀNG THÁNG)**

```
🎯 MỤC ĐÍCH: Nhập Dữ Liệu Hoạt Động Tháng N

📋 CHECKLIST (Vào ngày 1-15 hàng tháng):
  ☐ 1. Nhập Chấm Công (Working Days, Absent Days)
  ☐ 2. Nhập/Cập nhật Công Nợ (Nếu có tạm ứng, truy thu)
  ☐ 3. Ghi nhận Phụ Cấp (Housing, Transport, Family, v.v)
  ☐ 4. Nhập Đánh Giá Kỹ Thuật (Grade A-E)
  ☐ 5. Ghi nhận Bổ Công (Thưởng, Phạt, Điều Chỉnh)

📝 FILE HỖ TRỢ:
  → [12_NHÓM_2_PHÁT_SINH.md](12_NHOМ_2_PHAT_SINH.md)
     └─ Section 2-6: Chi tiết từng module
     └─ Workflow phê duyệt & ví dụ

⏱️ THỜI GIAN: 10-15 ngày
👤 NGƯỜI PHỤ TRÁCH: Quản Lý Bộ Phận, Nhân Sự, Kế Toán
✅ PHẢI HOÀN TẤT TRƯỚC: Ngày 20 tháng (để tính lương đúng hạn)
```

### **Ngày 21-25: Tính Lương - Nhóm 3 (XỬ LÝ)**

```
🎯 MỤC ĐÍCH: Tính Lương Thực Tế + Verify

📋 CHECKLIST:
  ☐ 1. Verify toàn bộ dữ liệu Nhóm 2 (double-check)
  ☐ 2. Chạy Tính Lương (5 Phases)
  ☐ 3. Verify Payroll Results (check sample records)
  ☐ 4. Xử Lý Truy Thu/Lãnh (nếu có)
  ☐ 5. Cân Đối & Phê Duyệt (Reconciliation)
  ☐ 6. GET FINAL APPROVAL từ Kế Toán Trưởng

📝 FILE HỖ TRỢ:
  → [13_NHÓM_3_XỬ_LÝ.md](13_NHOМ_3_XU_LY.md)
     └─ Section 2: Chi tiết 5 Phases + Ví Dụ Tính Toán
     └─ SQL Procedure: Tính tự động

⚠️ CRITICAL:
   • PHASE 3 có vấn đề? P7 parameter cần LÀM RÕ!
   • Công thức hiện tại cho kết quả lương quá nhỏ
   → Xem Section 2.3 trong 13_NHÓM_3_XỬ_LÝ.md (dòng "GIẢI PHÁP:")

⏱️ THỜI GIAN: 3-5 ngày
👤 NGƯỜI PHỤ TRÁCH: Kế Toán, IT
```

### **Ngày 26-28: Báo Cáo - Nhóm 4 (BÁO CÁO)**

```
🎯 MỤC ĐÍCH: Xuất Báo Cáo & Thông Báo

📋 CHECKLIST:
  ☐ 1. Tạo PAYSLIP cá nhân (PDF) → Gửi cho mỗi NV
  ☐ 2. Xuất PAYROLL REGISTER → Lưu trữ
  ☐ 3. Tạo Summary Reports → Gửi quản lý
  ☐ 4. Tạo Tax Report → Chuẩn bị nộp cục thuế
  ☐ 5. Tạo Insurance Report → Chuẩn bị nộp bảo hiểm
  ☐ 6. Tạo Cost Allocation → Gửi kế toán (ghi chép)
  ☐ 7. Generate Dashboard → KPI & Trend Analysis

📝 FILE HỖ TRỢ:
  → [14_NHÓM_4_BÁO_CÁO.md](14_NHOМ_4_BAO_CAO.md)
     └─ Section 2-6: Chi tiết từng báo cáo
     └─ Ví dụ format + SQL View

⏱️ THỜI GIAN: 2-3 ngày
👤 NGƯỜI PHỤ TRÁCH: Kế Toán, BI Team
```

---

## 🔄 QUY CYCLE TÍNH LƯƠNG HÀNG THÁNG

```
MONTH N: CYCLE HỎ SỊ TÍNH LƯƠNG
═════════════════════════════════════════════════════════════════════

TUẦN 1-2 (Ngày 1-15)
├─ Nhân Sự: Nhập Chấm Công
├─ Bộ Phận: Nhập Performance, Allows, Care Adjustments
├─ Kế Toán: Review & Approve từng module
└─ Deadline: Ngày 20 (phải hoàn tất dữ liệu Nhóm 2)

TUẦN 3 (Ngày 21-25)
├─ Kế Toán: Double-check Nhóm 2
├─ IT: Chạy Tính Lương (5 Phases)
├─ Kế Toán: Verify Results & Cân Đối
├─ KT Trưởng: Phê duyệt (Sign-off)
└─ Deadline: Ngày 25 (Payroll Approved)

TUẦN 4 (Ngày 26-28)
├─ IT: Tạo Báo Cáo (Payslips, Register, Tax, Insurance)
├─ Kế Toán: Ghi Chép Kế Toán (Cost Allocation)
├─ HR: Gửi PAYSLIP cho nhân viên
├─ Finance: Approve Payment
└─ Deadline: Ngày 28 (Ready to Pay)

LAST DAY (Ngày 30-31)
├─ Finance: Chuyển khoản Lương
├─ Kế Toán: Archive dữ liệu
└─ Hoàn tất
```

---

## 🎯 KEY PERFORMANCE INDICATORS (KPIs)

```
TARGETS CHO ECOTECH 2A PAYROLL:

✅ ACCURACY:
   └─ Variance trong Reconciliation: < 1% (hoặc < 100 VND)

✅ TIMELINESS:
   └─ Lương tính xong trước ngày 25
   └─ Payslips gửi trước ngày 28

✅ COVERAGE:
   └─ 100% nhân viên active được tính lương

✅ COMPLIANCE:
   └─ Tax Report nộp trước ngày 10 tháng sau
   └─ Insurance Report nộp đúng hạn
   └─ Audit trail hoàn toàn (who, when, what)
   └─ Zero unlocked records (để tránh sửa sau)

✅ EFFICIENCY:
   └─ Người phụ trách mỗi giai đoạn <= 5 người
   └─ Không manual calculation (tất cả tự động)
   └─ Automation rate: >= 90%
```

---

## ⚠️ CRITICAL ISSUES CẦN LÀM RÕ

```
📌 ISSUE 1: P7 PARAMETER (NHÓM 3, PHASE 3)
   Location: [13_NHÓM_3_XỬ_LÝ.md](13_NHOМ_3_XU_LY.md), Section 2.3
   Problem: Công thức hiện tại cho kết quả quá nhỏ
   Formula: (base_salary + allowances + working_days) / P7 × coef
   Result: ~1M (quá nhỏ! Phải là ~19-20M)
   
   Solutions cần xem xét:
   ☐ Option A: P7 = 27 (số ngày) → công thức nên là (base + allow) × (days/27) × coef
   ☐ Option B: P7 là hệ số khoán để chuẩn hóa → cần làm rõ mục đích
   ☐ Option C: Công thức gốc từ Excel có sai không? → verify từ file LƯƠNG
   
   ACTION: User cần cung cấp:
   • Công thức tính lương chính từ ECOTECH 2A documentation
   • P7 là gì? Mục đích?
   • Hoặc: Provide sample data (base, allowances, days) + expected gross salary

📌 ISSUE 2: GRADE D & E COEFFICIENTS (NHÓM 1 & 2)
   Location: [11_NHÓM_1_DỮ_LIỆU_NỀN.md](11_NHOМ_1_DU_LIEU_NEN.md), Section 3
   Problem: Hệ số Grade D & E chưa biết
   Current:  Grade A=1.0, B=0.95, C=0.90, D=0.85?, E=0.80?
   
   ACTION: Confirm từ ECOTECH 2A hoặc Excel trước

📌 ISSUE 3: NEW MODULES (ECOTECH 2A-SPECIFIC)
   Modules: Holiday Calendar, Accounting Standards, Advanced, Allowances, Care Adjust, Reconciliation
   Status: ✅ Designed in [10_ECOTECH_2A_MAPPING_SRS.md](10_ECOTECH_2A_MAPPING_SRS.md)
           📋 Detailed specs in 11-14_NHÓM_[1-4]
           ⏳ Need: Confirmation from ECOTECH 2A team
           ⏳ Need: Business rules (Holiday usage policy, etc)

📌 ISSUE 4: TAX THRESHOLD & RATES
   Current: Ngưỡng = 0? (Phụ thuộc vào luật thuế VN)
   6-tier brackets: Defined
   ACTION: Confirm current year tax rates từ Cục Thuế
```

---

## 💬 FAQ - FREQUENTLY ASKED QUESTIONS

```
Q1: Khi nào nên sử dụng từng NHÓM?
A:  • NHÓM 1: Chỉ setup 1 lần (sau đó update theo lương thay đổi)
    • NHÓM 2: Hàng tháng (khi có hoạt động nhân viên)
    • NHÓM 3: Sau khi Nhóm 2 hoàn tất (tính lương lần duy nhất/tháng)
    • NHÓM 4: Cuối tháng (sau khi Nhóm 3 approved)

Q2: Nếu tính sai lương tháng N, sửa thế nào?
A:  Có 2 cách:
    1. Update Nhóm 2 dữ liệu input → Tính lại Nhóm 3 (nếu còn trong tháng)
       ⚠️ MỎI: Nhóm 3 phải unlock (không thể sửa nếu đã lock)
    2. Create "Recovery" record (Nhóm 3) → Adjust tháng hiện tại
       ✅ SAFER: Không cần sửa lương cũ, chỉ điều chỉnh tháng mới

Q3: Làm sao audit trail tất cả thay đổi?
A:  Hệ thống có log tất cả:
    • Who: user_id (ai thực hiện)
    • When: timestamp (khi nào)
    • What: old_value → new_value (nội dung thay đổi)
    • Where: table_name (bảng nào)
    Tất cả Nhóm 1-4 đều ghi log.

Q4: Có thể export lương để import vào system khác không?
A:  Có. Format support:
    • Excel (.xlsx) - từ Payroll Register
    • CSV (.csv) - format chung
    • XML/JSON - API export
    Xem [14_NHÓM_4_BÁO_CÁO.md](14_NHOМ_4_BAO_CAO.md) Section 8

Q5: Nếu bộ phận quên nhập lương tháng N làm sao?
A:  1. Manual input nếu còn trong deadline (trước ngày 25)
    2. Nếu quá deadline:
       • Tính "partial" lương (0 ngày công = 0)
       • Create adjustment lương tháng N+1 (bề) hoặc tháng N+1 (bổ)
       • Ghi log: Lý do tại sao late

Q6: Reconciliation không cân đối phát hiện sai? (Variance > 0)
A:  Kiểm tra:
    1. Rounding errors (tính toán có tích tụ làm tròn không?)
    2. Double-check đầu vào Nhóm 2 (có missing records không?)
    3. Check Phase 3-4 calculation (logic có bug không?)
    4. Verify: Tax brackets, BHXH/BHYT rates
    5. Audit log: Có ai manual edit payroll records không?
    Nếu vẫn không rõ → escalate to IT + KT Trưởng
```

---

## 📞 LIÊN HỆ & HỖ TRỢ

```
ECOTECH 2A PAYROLL SYSTEM - SUPPORT

Vấn Đề về NHÓM 1 (Dữ Liệu Nền):
  → Contact: Admin IT
  → File: [11_NHÓM_1_DỮ_LIỆU_NỀN.md](11_NHOМ_1_DU_LIEU_NEN.md)
  → Thời Gian: 2-3 ngày vào setup

Vấn Đề về NHÓM 2 (Phát Sinh):
  → Contact: Quản Lý Bộ Phận / Kế Toán
  → File: [12_NHÓM_2_PHÁT_SINH.md](12_NHOМ_2_PHAT_SINH.md)
  → Thời Gian: Hàng tháng ngày 1-20

Vấn Đề về NHÓM 3 (Xử Lý):
  → Contact: Kế Toán + IT
  → File: [13_NHÓM_3_XỬ_LÝ.md](13_NHOМ_3_XU_LY.md) ⚠️ **P7 cần LÀM RÕ**
  → Thời Gian: Hàng tháng ngày 21-25

Vấn Đề về NHÓM 4 (Báo Cáo):
  → Contact: Kế Toán / BI Team
  → File: [14_NHÓM_4_BÁO_CÁO.md](14_NHOМ_4_BAO_CAO.md)
  → Thời Gian: Hàng tháng ngày 26-28

URGENT ISSUE (P7, Tax, Grade D/E):
  → Contact: Kế Toán Trưởng + ECOTECH 2A Team
  → Items to Clarify (từ [10_ECOTECH_2A_MAPPING_SRS.md](10_ECOTECH_2A_MAPPING_SRS.md), Section 8)
  → Timeline: Cần trả lời trước khi tính lương lần đầu
```

---

**📑 HƯỚNG DẪN HOÀN CHỈNH ECOTECH 2A - v1.0**
**Ngày Tạo: 10/04/2025**
**Trạng Thái: READY FOR IMPLEMENTATION**
**⚠️ CRITICAL: Xem P7 Parameter Issue trước khi bắt đầu**

