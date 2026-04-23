# ⚡ QUICK REFERENCE - BẢNG LƯƠNG ĐỘI & HỆ THỐNG ECOTECH 2A

**Mục Đích:** Tìm nhanh phần cần đọc, sheet nào đối ứng với nhóm nào

**Cập Nhật:** 11/04/2026

---

## 📌 TÓM TẮT NHANH

### Bạn Đang Ở Đâu?

```
Hiện Tại: "Bảng lương dưới đội" (Excel Spreadsheet)
         ├─ 5 lớp dữ liệu: Danh mục, Chấm công, Sản lượng, 
         │               Tính lương chi tiết, Báo cáo
         └─ Công thức: (Công × Phụ cấp) + (Quy khô × Giá) - Khấu trừ

Mục Tiêu: "Hệ thống lương ECOTECH 2A" (Phần mềm Database)
         ├─ 4 Nhóm dữ liệu: Master Data, Transactions, Processing, Reports
         └─ Công thức: Bath (input) → 5 Phases → Kíp (output)

CẦU NỐI: Bảng lương đội (hiện tại) = Input cho hệ thống
         (Dữ liệu không mất, chỉ tổ chức lại thôi!)
```

---

## 🗺️ BẢNG TÌNHCHO KIẾM NHANH

### Nếu Bạn Muốn Biết...

| Câu Hỏi | Đọc File | Section | Thời Gian |
|---|---|---|---|
| **"Sheet nào dùng cho dữ liệu nền?"** | 20_HUONG_DAN_... | 3.1 | 10 phút |
| **"DRC là gì? Công thức sao?"** | 20_HUONG_DAN_... | 3.1 "sản lượng" | 5 phút |
| **"Tính lương giống Excel cũ không?"** | 20_HUONG_DAN_... | 4.2 | 15 phút |
| **"5 Phases là gì?"** | 13_NHOМ_3_XU_LY.md | Section 2.2 | 30 phút |
| **"Phụ cấp tính sao?"** | 12_NHOМ_2_PHAT_SINH.md | Section 5 | 20 phút |
| **"Cần chuẩn bị những dữ liệu nào?"** | 20_HUONG_DAN_... | 5.1-5.3 | 1 giờ |
| **"Bảo hiểm áp cho ai?"** | 11_NHOМ_1_DU_LIEU_NEN.md | Section 2.3 | 5 phút |
| **"Tỷ giá tính khi nào?"** | 13_NHOМ_3_XU_LY.md | Phase 3 | 10 phút |

---

## 🎯 NƯỚC ĐI TỪNG BƯỚC

### Week 1: Xác Định (3-4 giờ)

**Step 1:** Mở file 20_HUONG_DAN_TRINH_LAYOUT_TAI_LIEU.md
- [ ] Đọc Section 2: Cấu Trúc Bảng Lương (20 phút)
- [ ] Đọc Section 3: Sheet Chi Tiết (30 phút)

**Step 2:** Mở file bảng lương Excel hiện tại
- [ ] Xác định: Có bao nhiêu sheet? Làm chuyện gì?
- [ ] So sánh Section 3 → Sheet Excel

**Step 3:** Chuẩn bị Checklist
- [ ] Điền vào Section 5.1: Dữ liệu gì?
- [ ] Điền vào Section 5.3: Bao nhiêu nhân viên?

**Step 4:** Export Dữ Liệu
- [ ] 5 files cơ bản: Employees, Attendance, Production, Allowances, Parameters

---

### Week 2: Học Hệ Thống (4-5 giờ)

**Step 1:** Nhóm 1 - Dữ Liệu Nền (1 giờ)
- [ ] 11_NHOМ_1_DU_LIEU_NEN.md Sections 1-2
- [ ] Ghi chú: employees table cần fields nào?

**Step 2:** Nhóm 2 - Phát Sinh (1 giờ)
- [ ] 12_NHOМ_2_PHAT_SINH.md Sections 1-3
- [ ] Ghi chú: attendance & rubber_drc table structure

**Step 3:** Nhóm 3 - Xử Lý (1.5 giờ) ⭐ QUAN TRỌNG
- [ ] 13_NHOМ_3_XU_LY.md Sections 1-2.2 (5 PHASES)
- [ ] Ghi chú: So sánh công thức với Excel cũ

**Step 4:** Nhóm 4 - Báo Cáo (1 giờ)
- [ ] 14_NHOМ_4_BAO_CAO.md Sections 1-2
- [ ] Ghi chú: Báo cáo cần gì?

---

### Week 3: Triển Khai (Tuỳ theo)

**Step 1:** Chuẩn Bị Database
- [ ] Tải 6_DATABASE_SCHEMA_ERD.md
- [ ] Setup 10 tables theo schema

**Step 2:** Import Dữ Liệu
- [ ] Import 5 files cơ bản vào database

**Step 3:** Test Calculation
- [ ] Chọn 1 nhân viên test
- [ ] Run 5 Phases → So sánh kết quả với Excel cũ

---

## 📊 BẢNG SO SÁNH: EXCEL ↔ HỆ THỐNG

```
┌────────────────────────────────────────────────────────────────────────┐
│ ITEM             │ EXCEL HIỆN TẠI       │ ECOTECH 2A SYSTEM          │
├────────────────────────────────────────────────────────────────────────┤
│ Tiền Tệ          │ VND (chưa rõ)        │ Bath → Kíp (rõ ràng)       │
│ Công Thức Lương  │ 1 cái ở TRẠM 1/2     │ 5 Phases (Phase 1-5)       │
│ DRC Quy Khô      │ Ở sản lượng sheet    │ Ở rubber_drc table         │
│ Hạng KT (A-D)    │ Ở sản lượng          │ Ở employees table          │
│ Phụ Cấp          │ Cộng vào công thức   │ Ở allowances table         │
│ Bảo Hiểm         │ Chỉ có/không         │ Chỉ INDIRECT (rõ ràng)     │
│ Tỷ Giá Tháng     │ KHÔNG CÓ             │ BẮTBUỘC (exchange_rates)  │
│ Hạn Chế          │ Manual, dễ sai       │ Validation, kiểm tra chặt  │
│ Audit Trail      │ Không ghi lại        │ Lưu ai, khi nào, sửa gì   │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 🔍 FILE & SECTION MAPPING

### Nếu Bạn Muốn Biết Về...

**👥 Danh Mục Nhân Sự / Hồ Sơ Lao Động**
```
Excel: Sheet "MSNV 2025" / "DS CN MOI UP"
       ├─ Columns: Mã NH, Họ Tên, Chức Vụ, Bộ Phận, Trạm, Hạng
       └─ Dùng để: VLOOKUP cho toàn bộ

Hệ Thống:
  ├─ File: 11_NHOМ_1_DU_LIEU_NEN.md
  ├─ Section: 2.1 (Database Table - employees)
  └─ Table: employees
     ├─ msnv (VARCHAR 2026-NNN)
     ├─ name, position, department
     ├─ tram_id, technical_grade
     ├─ employee_type (DIRECT/INDIRECT)
     ├─ difficult_zone, insurance_included
     └─ bank_account

Công Việc:
  [ ] Import danh mục từ Excel
  [ ] Xác định: DIRECT vs INDIRECT là ai?
  [ ] Hạng KT: A/B/C/D có tương ứng không?
  [ ] MSNV: Convert sang 2026-NNN format?
```

**📋 Chấm Công**
```
Excel: Sheet "CHẤM CÔNG" + "CC NGƯỜI LÀO"
       ├─ Columns: Mã NH, Công, CN, Cạo 2 Lát, Khộp Nặng, Cây non
       └─ Mục Đích: Ghi nhận công làm từng loại

Hệ Thống:
  ├─ File: 12_NHOМ_2_PHAT_SINH.md
  ├─ Section: 2 (MODULE: Attendance)
  └─ Table: attendance
     ├─ msnv, year_month
     ├─ working_days
     ├─ absent_days, sick_days, leave_days
     ├─ special_days (Cây non, Cạo 2 lát, etc.)
     └─ status (DRAFT → APPROVED)

Công Việc:
  [ ] Xác định: Tổng công ≈ 26 ngày/tháng?
  [ ] Loại công: Gom vào working_days, special_days hay separate?
  [ ] Import: Từ Excel → Database attendance table
  [ ] Validate: Total <= 26, MSNV exists, no duplicates
```

**🏭 Sản Lượng & Quy Khô (DRC)**
```
Excel: Sheet "sản lượng" + "MỦ DÂY", "MỦ XIRUM", "MỦ SIRUM"
       ├─ Columns: Mã NH, Khối Tươi, DRC, Quy Khô, Hạng KT, Đơn Giá, Thành Tiền
       └─ Công Thức: Fresh × DRC = Dry, Dry × Price = Amount

Hệ Thống:
  ├─ File: 12_NHOМ_2_PHAT_SINH.md
  ├─ Section: 3.2 (MODULE: DRC - Dry Rubber Content)
  └─ Table: rubber_drc
     ├─ msnv, year_month
     ├─ fresh_weight (kg tươi)
     ├─ drc_coefficient (hệ số)
     ├─ dry_weight = fresh × coef (GIỮ NGUYÊN thập phân)
     ├─ technical_grade (A/B/C/D)
     ├─ unit_price (theo hạng)
     └─ total_amount = dry × price (LÀMtròn)

Công Việc:
  [ ] Kiểm tra: DRC hệ số bao nhiêu? (0.30, 0.35?)
  [ ] Xác định: Hạng KT = technical_grade có giống Excel không?
  [ ] So sánh: Công thức dry_weight = fresh × coef?
  [ ] Đơn giá: Khác nhau bao nhiêu giữa A/B/C/D?
  [ ] Import: Từ "sản lượng" sheet → rubber_drc table
```

**💰 Tính Lương (5 PHASES) ⭐ QUAN TRỌNG**
```
Excel: Sheet "TRẠM 1", "TRẠM 2", "Đội 3-6"
       ├─ Công Thức: (Công × PC) + (Quy khô × Giá) + PC + ...
       │            - (BHXH + Thuế) = Lương Net
       └─ Tính Manual ở từng sheet

Hệ Thống: 5 PHASES (Structured & Automated)
  ├─ File: 13_NHOМ_3_XU_LY.md
  ├─ Section: 2.2 (QUY TRÌNH 5 PHASES)
  │
  ├─ PHASE 1: Performance → Grade A/B/C/D → Coefficient
  │   └─ A=1.0, B=0.95, C=0.90, D=0.85
  │
  ├─ PHASE 2: Base Salary → Lookup salary_scale
  │   └─ By: tram_id + grade + effective_date
  │
  ├─ PHASE 3: Main Calculation (QUAN TRỌNG!)
  │   └─ = (Care_Days × Care_Rate) + (Dry_Weight × Unit_Price) + Allowances
  │   └─ Tính Bath (chưa × Exchange Rate chưa)
  │
  ├─ PHASE 4: Deductions
  │   └─ BHXH (8%), BHYT (1.5%) - chỉ INDIRECT
  │   └─ Thuế 6 bậc (nếu vượt ngưỡng)
  │
  └─ PHASE 5: Net Salary + Phân Bổ
      └─ Net_Kip = Net_Bath × Exchange_Rate
      └─ Phân bổ theo cost_center

So Sánh:
  [ ] Excel công thức = Phase 3 không?
  [ ] Chăm sóc = (Công × PC) ? ✓
  [ ] Sản lượng = (Quy khô × Giá) ? ✓
  [ ] Khấu trừ = BHXH + BHYT + Thuế ? ✓
  [ ] Tỷ giá: Khi nào nhân (Phase 5)?
```

**📄 Báo Cáo Đầu Ra**
```
Excel: Sheet "LƯƠNG ĐỘI", "BẢNG PHÂN TOÁN"
       ├─ "LƯƠNG ĐỘI": Danh sách lương chốt
       └─ "BẢNG PHÂN TOÁN": Phân bổ chi phí theo bộ phận

Hệ Thống:
  ├─ File: 14_NHOМ_4_BAO_CAO.md
  ├─ Section: 2-3 (Payslip & Cost Allocation Reports)
  │
  └─ Báo Cáo 1: Payslip (Từng Nhân Viên)
     ├─ MSNV, Name, Position
     ├─ Care Salary, Production Salary, Allowances
     ├─ Deductions (BHXH, BHYT, Tax)
     └─ Net Salary (Kíp)
  
  └─ Báo Cáo 2: Cost Allocation (Từng Bộ Phận)
     ├─ Department
     ├─ Total Salary, Total Allowances, Total Deductions
     └─ Total Net Salary

So Sánh:
  [ ] Báo cáo = Output từ Phase 5?
  [ ] Columns: Có đủ không? Cần thêm gì?
  [ ] Format: Hợp lý cho HR & Kế toán?
```

---

## 🎓 CÁC THUẬT NGỮ QUAN TRỌNG

| Thuật Ngữ | Định Nghĩa | Dùng Ở Đâu |
|---|---|---|
| **DRC** | Dry Rubber Content (Hệ số quy khô) | rubber_drc table |
| **Quy Khô** | Dry Weight = Fresh × DRC | Phase 3 |
| **Hạng KT** | Technical Grade (A/B/C/D) | employees table |
| **DIRECT** | CN trực tiếp (100% Kíp, no insurance) | employees.employee_type |
| **INDIRECT** | CN gián tiếp (office, insurance) | employees.employee_type |
| **BHXH** | Bảo hiểm xã hội (8%) | Phase 4 deductions |
| **BHYT** | Bảo hiểm y tế (1.5%) | Phase 4 deductions |
| **Phase** | Bước tính lương (5 phases tổng cộng) | 13_NHOМ_3 |
| **Allowances** | Phụ cấp (cộng thêm vào lương) | allowances table |
| **Advance** | Tạm ứng (tiền trả trước, trừ sau) | advances table |
| **Exchange Rate** | Tỷ giá Bath → Kíp | exchange_rates table, Phase 5 |
| **Cost Center** | Bộ phận/Project (để phân bổ chi phí) | payroll.cost_center |

---

## ✅ QUICK CHECKLIST - THỨ TỰ KIỂM TRA

### Tiền Chuẩn Bị (Ngay Hôm Nay)
- [ ] Mở bảng lương Excel → Xác định 5 lớp dữ liệu
- [ ] Mở 20_HUONG_DAN_... file → Đọc Section 2-3
- [ ] Mapping: Sheet hiện tại → Nhóm hệ thống

### Chuẩn Bị Dữ Liệu (Tuần 1)
- [ ] Export: Danh sách nhân viên → employees_export.xlsx
- [ ] Export: Chấm công tháng 12/2025 → attendance_export.xlsx
- [ ] Export: Sản lượng & DRC → production_export.xlsx
- [ ] Export: Phụ cấp danh sách → allowances_export.xlsx
- [ ] Export: Hệ số & tham số → parameters_export.xlsx

### Học Hệ Thống (Tuần 2)
- [ ] Đọc 11_NHÓM_1 → Hiểu Master Data
- [ ] Đọc 12_NHÓM_2 → Hiểu Transactions
- [ ] Đọc 13_NHÓM_3 → Hiểu 5 Phases (Main!)
- [ ] Đọc 14_NHÓM_4 → Hiểu Reporting

### Triển Khai (Tuần 3+)
- [ ] Setup Database schema (10 tables)
- [ ] Import 5 export files vào database
- [ ] Test: 1 nhân viên → Run 5 Phases
- [ ] Verify: Result = Manual calculation?
- [ ] Go Live!

---

## 📞 LIÊN HỆ & HỖ TRỢ

**Nếu Có Thắc Mắc:**

| Câu Hỏi | Hỏi File | Section | Người Liên Hệ |
|---|---|---|---|
| DRC là gì, công thức? | 20_HUONG_DAN_... | 3.1 | Tech Team |
| 5 Phases logic? | 13_NHOМ_3_XU_LY.md | 2.2 | Finance |
| Dữ liệu export như nào? | 20_HUONG_DAN_... | 5.3 | HR/Ops |
| Phụ cấp áp lên ai? | 12_NHOМ_2_PHAT_SINH.md | Section 5 | HR |
| Tỷ giá tháng 12? | 11_NHOМ_1_DU_LIEU_NEN.md | Section 7 | Finance |
| DIRECT vs INDIRECT? | 16_ECOTECH_2A_SPECIFICS_... | Item #2 | HR/Finance |

---

**⚡ Start Here:** [20_HUONG_DAN_TRINH_LAYOUT_TAI_LIEU.md](./20_HUONG_DAN_TRINH_LAYOUT_TAI_LIEU.md)

**📚 Core System Docs:** 11-14_NHOМ_*.md

**🎯 This File:** Quick lookup guide

---

**Created:** 11/04/2026  
**Version:** 1.0  
**Status:** READY TO USE

