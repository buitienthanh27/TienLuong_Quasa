# 📖 HƯỚNG DẪN: CẤU TRÚC BẢNG LƯƠNG ĐỘI & TÍCH HỢP HỆ THỐNG

**Mục Tiêu:** Giúp bạn hiểu "bảng lương dưới đội" hiện tại và cách mapping vào hệ thống ECOTECH 2A

**Ngày:** 11/04/2026  
**Version:** 1.0

---

## 🎯 QUICK START - CẦN ĐỌC GỊ TRƯỚC?

**Thứ Tự Đọc Khuyến Nghị:**

```
Step 1: XÁC ĐỊNH CẤU TRÚC HIỆN TẠI (20 PHÚT)
│
├─ [File này] Section 2: Cấu Trúc Bảng Lương Đội
│   └─ Hiểu 5 lớp dữ liệu trong bảng lương đội
│
└─ [File này] Section 3: Sheet Chi Tiết
    └─ Xác định những sheet nào dùng để tính lương

Step 2: HỌC HỆ THỐNG MỚI (1 GIỜ)
│
├─ 11_NHÓM_1_DỮ_LIỆU_NỀN.md [Section 1-2]
│   └─ Hiểu "Dữ Liệu Nền" là gì
│
├─ 12_NHÓM_2_PHÁT_SINH.md [Section 1-3]
│   └─ Hiểu "Phát Sinh" tương ứng với cái gì ở bảng lương
│
├─ 13_NHÓM_3_XỬ_LÝ.md [Section 1-2.1]
│   └─ Hiểu "5 Phases Tính Lương" là gì
│
└─ 14_NHÓM_4_BÁO_CÁO.md [Section 1-2]
    └─ Hiểu "Báo Cáo" cần những gì

Step 3: SO SÁNH & MAPPING (2 GIỜ)
│
├─ [File này] Section 4: Mapping Chi Tiết
│   └─ Bảng lương đội sheet nào → Nhóm nào của hệ thống
│
└─ [File này] Section 5: Checklists Triển Khai
    └─ Chuẩn bị dữ liệu cần thiết

Step 4: TRIỂN KHAI (TUỲ VỤC BẠN)
│
└─ Bắt đầu xây dựng hệ thống dựa trên cấu trúc mới
```

---

## 📊 SECTION 2: CẤU TRÚC BẢNG LƯƠNG ĐỘI HIỆN TẠI

### 2.1 5 Lớp Dữ Liệu TRonment

Bảng lương đội hiện tại chia thành 5 lớp:

```
┌─────────────────────────────────────────────────────────────┐
│  LỚP 1: DANH MỤC NHÂN SỰ                                    │
│  ├─ Sheet: MSNV 2025, DS CN MOI UP                         │
│  └─ Mục Đích: Chuẩn hóa mã nhân viên, họ tên               │
│     └─ Phục vụ VLOOKUP cho toàn bộ sheet khác             │
│     └─ Ràng buộc: Nếu tên/mã sai → lỗi toàn chain         │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  LỚP 2: CHẤM CÔNG & CÔNG VỤC                               │
│  ├─ Sheets:                                                 │
│  │  ├─ CHẤM CÔNG (Công thường, CN, Cạo 2 lát)             │
│  │  ├─ Công cây non (Riêng vườn cây năm 1)                │
│  │  ├─ CC NGƯỜI LÀO (Lao workers)                          │
│  │  └─ CHẤM CÔNG Axit (Acid tapping specific)             │
│  └─ Mục Đích: Ghi nhận công làm từng hạng                 │
│     └─ Nguồn từ hệ thống chấm công/quét thẻ              │
│     └─ Các loại công: Thường, CN, Cây non, Người Lào      │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  LỚP 3: SẢN LƯỢNG & QUY KHÔ                                │
│  ├─ Sheets:                                                 │
│  │  ├─ sản lượng (Mủ tạp, Mủ xirum, Mủ sirum)            │
│  │  ├─ MỦ DÂY, MỦ SIRUM, MỦ XIRUM (Chi tiết từng loại)  │
│  │  ├─ THCKT (Tổng hợp công việc KT)                     │
│  │  └─ bảng kê công (Công + Quy khô tổng hợp)           │
│  └─ Mục Đích: Xác định khối lượng sản phẩm & quy khô      │
│     └─ Hệ số DRC: Khối tươi × DRC = Khối khô             │
│     └─ Hạng KT: A/B/C/D quy định đơn giá & lương          │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  LỚP 4: TÍNH LƯƠNG CHI TIẾT                                │
│  ├─ Sheets:                                                 │
│  │  ├─ TRẠM 1, TRẠM 2 (Tính lương từng cây)              │
│  │  ├─ Đội 3, Đội 4, Đội 5, Đội 6 (Tính lương từng đội)  │
│  │  └─ THCKT (Tổng hợp chi tiết KT)                      │
│  └─ Mục Đích: Tính lương chi tiết từng nhân viên          │
│     └─ Lương chăm sóc (công × phụ cấp)                    │
│     └─ Lương sản lượng (quy khô × đơn giá)                │
│     └─ Phụ cấp nhân lực                                    │
│     └─ Công thức: (Chăm sóc + Sản lượng + Phụ cấp)       │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  LỚP 5: BÁO CÁO ĐẦU RA                                     │
│  ├─ Sheets:                                                 │
│  │  ├─ CÁCH TÍNH LƯƠNG (Hướng dẫn tính)                  │
│  │  ├─ LƯƠNG ĐỘI (Lương chốt cuối)                       │
│  │  └─ BẢNG PHÂN TOÁN (Phân bổ chi phí)                  │
│  └─ Mục Đích: Output cuối cùng để trả lương & kế toán     │
│     └─ Không chứa công thức tính (chỉ kéo từ trên)       │
│     └─ Dùng cho: Nhân sự (trả lương), Kế toán (ghi sổ)   │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Luồng Data Flow Hiện Tại

```
DANH MỤC NHÂN SỰ (MSNV 2025)
          ↓
          VLOOKUP ← chuẩn hóa tên & mã
          ↓
CHẤM CÔNG ─────→ Công thường, CN, Cây non
   ↓             ↓
   └───→ Tính phụ cấp
        (Công × Hệ số phụ cấp)

SẢN LƯỢNG ─────→ Khối lượng tươi
   ↓             ↓
   └───→ Tính DRC
        (Tươi × Hệ số DRC = Khô)
        ↓
        Hạng KT (A/B/C/D)
        ↓
        Tính lương sản lượng
        (Khô × Đơn giá theo hạng)

TRẠM 1 / TRẠM 2 (Tính Lương Chi Tiết)
├─ Lương chăm sóc: Công × Phụ cấp
├─ Lương sản lượng: Khô × Đơn giá
├─ Phụ cấp nhân lực: Các khoản cộng thêm
└─ Tổng: Cộng 3 khoản trên

LƯƠNG ĐỘI (Báo Cáo)
├─ Kéo lại từ TRẠM 1, TRẠM 2
├─ Gom nhân viên còn lại (CB, Lao worker)
└─ Chốt cuối cùng để trả & ghi sổ
```

---

## 📑 SECTION 3: SHEET CHI TIẾT VÀ CÔNG THỨC

### 3.1 Sheet Input Gốc (Bạn cần kiểm tra)

**Sheet: MSNV 2025 / DS CN MOI UP**
```
│ Column      │ Type  │ Mục Đích                      │ Mapping Nhóm 1 │
├─────────────┼───────┼──────────────────────────────┼────────────────┤
│ Mã NH (MSNV)│ Text  │ Mã nhân viên                  │ msnv (PK)      │
│ Họ Tên      │ Text  │ Tên nhân viên                 │ name           │
│ Chức Vụ     │ Text  │ Vị trí công việc             │ position       │
│ Bộ Phận     │ Text  │ Phòng/Đội                    │ department     │
│ Trạm/Đội    │ Number│ Trạm 1/2 hoặc Đội 1-9       │ tram_id        │
│ Hạng        │ Text  │ A/B/C/D hoặc Cấp             │ technical_grade│
└─────────────┴───────┴──────────────────────────────┴────────────────┘

✓ Công việc cần làm: 
  - Kiểm tra: Có bao nhiêu nhân viên? Hạng nào nhiều nhất?
  - So sánh: Mã MSNV có format nào? Có match 2026-NNN không?
    → Nếu không, cần convert sang format mới (2026-NNN)
```

**Sheet: CHẤM CÔNG**
```
│ Column      │ Type  │ Mục Đích                      │ Mapping Nhóm 2 │
├─────────────┼───────┼──────────────────────────────┼────────────────┤
│ Mã NH       │ Text  │ Mã từ MSNV 2025              │ msnv (FK)      │
│ Công        │ Number│ Ngày làm                      │ working_days   │
│ CN (Công NT)│ Number│ Công chủ nhật                 │ Advance/Special│
│ Cạo 2 Lát   │ Number│ Ca tapping đặc biệt           │ Special_task   │
│ Khộp Nặng   │ Number│ Công vực khó khăn            │ difficult_zone │
│ Công Cây Non│ Number│ Công vườn cây < 1 năm        │ Young_tree_days│
└─────────────┴───────┴──────────────────────────────┴────────────────┘

✓ Công việc cần làm:
  - Kiểm tra: Tổng cộng/người ≈ 26 ngày/tháng?
  - So sánh: Loại công nào? (Thường, CN, Khó, Cây non)
    → Map sang nhóm 2: attendance (working_days, absent_days, special_days)
```

**Sheet: sản lượng**
```
│ Column      │ Type  │ Mục Đích                      │ Mapping Nhóm 2 │
├─────────────┼───────┼──────────────────────────────┼────────────────┤
│ Mã NH       │ Text  │ Mã nhân viên                  │ msnv (FK)      │
│ Mủ Tạp      │ Number│ Khối lượng mủ tạp (tươi)     │ fresh_weight   │
│ Mủ Xirum    │ Number│ Khối lượng mủ xirum (tươi)   │ fresh_weight   │
│ Mủ Sirum    │ Number│ Khối lượng mủ sirum (tươi)   │ fresh_weight   │
│ DRC         │ Number│ Hệ số quy khô                 │ drc_coef       │
│ Quy Khô     │ Number│ Khối lượng quy khô (tính)     │ dry_weight     │
│ Hạng KT     │ Text  │ A/B/C/D (hạng kỹ thuật)      │ technical_grade│
│ Đơn Giá     │ Number│ Giá/kg theo hạng             │ unit_price     │
│ Thành Tiền  │ Number│ Quy Khô × Đơn Giá           │ total_amount   │
└─────────────┴───────┴──────────────────────────────┴────────────────┘

✓ Công việc cần làm:
  - Kiểm tra: 
    • Hạng A/B/C/D có bao nhiêu người?
    • DRC hệ số bao nhiêu? (Thường 0.30-0.35)
    • Đơn giá khác nhau bao nhiêu giữa các hạng?
  - So sánh: 
    • Công thức = Raw (Mủ) × DRC = Dry (Quy Khô) ✓
    • Lương = Dry × Đơn Giá (theo hạng) ✓
    → Map sang nhóm 2: rubber_drc table
```

### 3.2 Sheet Tính Toán Trung Gian (Cần Hiểu Logic)

**Sheet: TRẠM 1, TRẠM 2, Đội 3-6**
```
CÔNG THỨC TÍNH LƯƠNG:
═══════════════════════════════════════════════════════════

1. LƯƠNG CHĂM SÓC
   ───────────────────────
   = Công Thường × Phụ Cấp Chăm Sóc/Ngày
   + Công CN × Phụ Cấp CN/Ngày
   + Công Khó × Phụ Cấp Khó/Ngày
   = X₁ (Kíp)

   ✓ Mapping: Nhóm 3 - Phase 3: Care salary calculation
   ✓ Check: Phụ cấp/ngày hiện là bao nhiêu? (từ Nhóm 1)

2. LƯƠNG SẢN LƯỢNG
   ───────────────────────
   = Quy Khô × Đơn Giá (Hạng A/B/C/D)
   = X₂ (Kíp)

   ✓ Mapping: Nhóm 3 - Phase 3: Production salary calculation
   ✓ Check: DRC formula & hạng KT có đúng không?

3. PHỤ CẤP NHÂN LỰC
   ───────────────────────
   = Phụ Cấp Công (nếu công >= 26)
   + Phụ Cấp Chế Độ (theo vị trí)
   + Phụ Cấp Khác (nếu có)
   = X₃ (Kíp)

   ✓ Mapping: Nhóm 2 - Allowances module
   ✓ Check: Danh sách phụ cấp là gì? Bao nhiêu tiền/cái?

4. TỔNG LƯƠNG (GROSS)
   ───────────────────────
   = X₁ + X₂ + X₃
   = Gross Salary (Kíp)

   ✓ Mapping: Nhóm 3 - Phase 3 output

5. KHẤU TRỪ (Nếu Có)
   ───────────────────────
  = Bảo Hiểm (BHXH 8% + BHYT 1.5% - chỉ cán bộ)
   + Thuế Thu Nhập (6 bậc, nếu vượt ngưỡng)
   + Tạm Ứng (Trừ tiền đã cho trước)
   = Y (Kíp)

   ✓ Mapping: Nhóm 3 - Phase 4: Deductions
   ✓ Check: 
     • BHXH 8% áp cho ai? (CNKT không, Cán bộ có)
     • Thuế suất bao nhiêu?
     • Có tạm ứng không?

6. LƯƠNG NET (THỰC LÃNH)
   ───────────────────────
   = (X₁ + X₂ + X₃) - Y
   = Net Salary (Kíp)

   ✓ Mapping: Nhóm 3 - Phase 5 output
```

**Sheet: THCKT (Tổng Hợp Chi Tiết KT)**
```
Mục Đích: Chứa tất cả dữ liệu chi tiết để kéo sang báo cáo
├─ Công từ CHẤM CÔNG
├─ Quy Khô từ sản lượng
├─ Hạng KT từ sản lượng
├─ Lương từ TRẠM 1/2
├─ Phụ cấp, khấu trừ
└─ Tổng hợp toàn đội

✓ Mapping: Đây là "Bridge Sheet" giữa chi tiết & báo cáo
          → Map sang Nhóm 3: payroll (table lưu kết quả tính lương)
```

### 3.3 Sheet Báo Cáo Đầu Ra

**Sheet: LƯƠNG ĐỘI**
```
Mục Đích: Chốt lương cuối cùng để trả & ghi sổ
├─ Kéo lại thành tiền từ TRẠM 1, TRẠM 2, Đội 3-6
├─ Bổ sung: Cán bộ, Bảo vệ, Tạp vụ (từ các sheet riêng)
├─ Gom: Tất cả nhân viên
└─ Output: Danh sách lương chốt

Columns:
│ MSNV │ Họ Tên │ Chức Vụ │ Lương | Phụ Cấp | Khấu Trừ | Lương Net │ Ngân hàng │

✓ Mapping: Nhóm 4 - Payslip / Salary Report
          → Dữ liệu output từ Nhóm 3
          → Dùng cho: HR (trả lương), Kế toán (ghi sổ)
```

**Sheet: BẢNG PHÂN TOÁN**
```
Mục Đích: Phân bổ chi phí lương vào các bộ phận/project
├─ Gom từng bộ phận: Sản xuất, Quản lý, Kinh doanh
├─ Cộng lương & phụ cấp theo bộ phận
└─ Output: Phân bổ chi phí

Columns:
│ Bộ phận │ Mã KT │ Tổng Lương | Tổng Phụ Cấp | Tổng Khấu Trừ │ Tổng Net │

✓ Mapping: Nhóm 4 - Cost Allocation Report
          → Phân bổ từ payroll_id → cost_center
```

---

## 🔗 SECTION 4: MAPPING CHI TIẾT

### 4.1 Bảng Mapping: Sheet Hiện Tại → Nhóm Hệ Thống

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│ BẢNG MAPPING: SHEET HIỆN TẠI → NHÓM ECOTECH 2A                                 │
├─────────────────────────────────────────────────────────────────────────────────┤

NHÓM 1: DỮ LIỆU NỀN (Master Data)
║
╠─ Sheet: MSNV 2025 / DS CN MOI UP
║  └─ → Table: employees
║     ├─ Columns: msnv, name, position, department, tram_id, technical_grade
║     └─ Action: Import → Validate format 2026-NNN
║
╠─ Sheet: Danh mục phụ cấp (nếu có)
║  └─ → Table: allowances / salary_scale
║     └─ Action: Extract → Define phụ cấp/hạng/đơn giá
║
└─ Cần từ Phòng Tổ chức:
   ├─ Bảng hệ số DRC (drc_coefficient)
   ├─ Bảng hạng KT A/B/C/D & đơn giá (salary_scale)
   ├─ Bảng tỷ giá Bath → Kíp (exchange_rates)
   └─ Bảng phụ cấp cố định (allowances)


NHÓM 2: PHÁT SINH (Transactions)
║
╠─ Sheet: CHẤM CÔNG, Công cây non, CC NGƯỜI LÀO
║  └─ → Table: attendance
║     ├─ Columns: msnv, year_month, working_days, absent_days, special_days
║     └─ Action: Import → Validate total_days <= 26
║
╠─ Sheet: sản lượng (MỦ DÂY, MỦ XIRUM, MỦ SIRUM)
║  └─ → Table: rubber_drc
║     ├─ Columns: msnv, fresh_weight, drc_coef, dry_weight, technical_grade
║     └─ Action: Import → Validate formula (fresh × coef = dry)
║
╠─ Sheet: (nếu có) Danh sách tạm ứng
║  └─ → Table: advances
║     └─ Action: Import → Validate balance
║
└─ Cần Kiểm tra:
   ├─ Công tháng nào? (year_month format)
   ├─ Quy khô công thức có đúng không?
   └─ Tạm ứng từ đâu? (chi tiết)


NHÓM 3: XỬ LÝ (Processing)
║
╠─ Sheet: TRẠM 1, TRẠM 2, Đội 3-6 (Tính Lương Chi Tiết)
║  └─ → Logic: 5 Phases (Phase 1-5)
║     ├─ Phase 1: Performance → technical_grade
║     ├─ Phase 2: Lương cơ bản → salary_scale lookup
║     ├─ Phase 3: Tính lương (Chăm sóc + Sản lượng + Phụ cấp)
║     ├─ Phase 4: Khấu trừ (BHXH, BHYT, Thuế)
║     └─ Phase 5: Lương net + Phân bổ
║
╠─ Sheet: THCKT (Tổng Hợp Chi Tiết)
║  └─ → Table: payroll
║     ├─ Columns: msnv, year_month, care_salary, production_salary, allowances,
║     │            deductions, net_salary, status
║     └─ Action: Compare → Verify formula match
║
└─ Cần So Sánh:
   ├─ Công thức tính lương = 5 Phases?
   ├─ Phụ cấp tính như nào?
   └─ Khấu trừ gồm những gì?


NHÓM 4: BÁO CÁO (Reporting)
║
╠─ Sheet: LƯƠNG ĐỘI
║  └─ → Output: Payslip Report
║     ├─ Columns: msnv, name, position, salary, allowances, deductions, net
║     └─ Action: Verify → Dữ liệu match Phase 5 output
║
╠─ Sheet: BẢNG PHÂN TOÁN
║  └─ → Output: Cost Allocation Report
║     ├─ Columns: department, cost_code, total_salary, total_deductions
║     └─ Action: Verify → Phân bổ theo bộ phận
║
└─ Cần So Sánh:
   ├─ Báo cáo đủ cho HR & Kế toán không?
   ├─ Có cần thêm columns gì không?
   └─ Format báo cáo hợp lý không?

└─────────────────────────────────────────────────────────────────────────────────┘
```

### 4.2 Công Thức So Sánh: Excel Hiện Tại ↔ Hệ Thống Mới

**Excel Hiện Tại:**
```
Lương = (Công × Phụ Cấp Chăm Sóc) + (Quy Khô × Đơn Giá) + Phụ Cấp
       - (Bảo Hiểm + Thuế) [nếu có]
```

**Hệ Thống Mới (ECOTECH 2A):**
```
PHASE 3: Calculated_Salary = (Care_Working_Days × Care_Rate) 
                           + (DRC_Dry_Weight × Unit_Price_By_Grade) 
                           + Allowances
                           = Gross_Salary (Bath)

PHASE 4: Net_Salary_Bath = Gross - (BHXH + BHYT + Tax)

PHASE 5: Net_Salary_Kip = Net_Salary_Bath × Exchange_Rate (Monthly)
```

**Sự Khác Biệt:**
```
ITEM                        │ Excel Hiện Tại      │ Hệ Thống Mới       │
────────────────────────────┼─────────────────────┼────────────────────┤
Tiền tệ                     │ VND (chưa rõ)       │ Bath (input) → Kíp │
DRC Quy Khô                 │ Chỉ trong sản lượng  │ Trong rubber_drc TB│
Hạng Kỹ Thuật              │ Ở sản lượng          │ Trong employees TB │
Bảo Hiểm                    │ Chỉ có/không        │ Chỉ INDIRECT = YES  │
Phụ Cấp                     │ Cộng vào lương       │ Trong allowances TB│
Tỷ Giá Tháng                │ Chưa có              │ Bắt buộc (Exchange)│
────────────────────────────┴─────────────────────┴────────────────────┘

KHUYẾN NGHỊ: 
✓ Đọc công thức Excel → Compare với Phase 3-5 của 13_NHÓM_3_XỬ_LÝ.md
✓ Điều chỉnh: DRC, Hạng, Bảo hiểm, Tỷ giá
```

---

## ✅ SECTION 5: CHECKLISTS CHUẨN BỊ TRIỂN KHAI

### 5.1 Checklist Dữ Liệu Cần Xác Nhận

**Từ Bảng Lương Excel Hiện Tại:**

- [ ] **DANH MỤC NHÂN SỰ**
  - [ ] Tổng bao nhiêu nhân viên?
  - [ ] Mã MSNV hiện tại format gì? (vd: 001, 2026-001)
  - [ ] Có cần convert sang 2026-NNN không?
  - [ ] Hạng KT: Có bao nhiêu A, B, C, D?
  - [ ] Loại công nhân: Ai là DIRECT (Kíp), ai là INDIRECT (Đô)?
  - [ ] Export danh mục: `Employees_Current_Data.xlsx`

- [ ] **CHẤM CÔNG THÁNG**
  - [ ] Tháng nào? (vd: Tháng 12/2025)
  - [ ] Tổng công/người ≈ bao nhiêu ngày?
  - [ ] Loại công: Thường, CN, Khó, Cây non
  - [ ] Export: `Attendance_Dec2025.xlsx`

- [ ] **SẢN LƯỢNG & QUY KHÔ**
  - [ ] DRC hệ số: Bao nhiêu? (thường 0.30-0.35)
  - [ ] Công thức DRC = Fresh × Coef? (✓ confirm)
  - [ ] Hạng KT: Ai A, ai B, ai C, ai D?
  - [ ] Đơn giá/kg: Khác nhau bao nhiêu giữa các hạng?
  - [ ] Export: `Production_Drc_Dec2025.xlsx`

- [ ] **PHỤ CẤP**
  - [ ] Danh sách phụ cấp: Gồm những gì?
  - [ ] Mức cộng: Bao nhiêu Kíp/mỗi loại?
  - [ ] Khi nào được hưởng? (Điều kiện)
  - [ ] Export: `Allowances_List.xlsx`

- [ ] **HỆ SỐ & THAM SỐ**
  - [ ] Phụ cấp chăm sóc/ngày: Bao nhiêu? (Kíp)
  - [ ] Bảo hiểm: Áp cho ai? (CNKT không, Cán bộ có)
  - [ ] Thuế: 6 bậc, ngưỡng bao nhiêu?
  - [ ] Tỷ giá tháng 12/2025: 1 Bath = ? Kíp
  - [ ] Export: `System_Parameters_Dec2025.xlsx`

### 5.2 Checklist So Sánh & Phân Tích

**Cần Đọc & Hiểu:**

- [ ] **Từ Tài Liệu Hệ Thống:**
  - [ ] 11_NHÓM_1_DỮ_LIỆU_NỀN.md (Sections 1-2)
    - [ ] Bảng employees cần fields gì?
    - [ ] Dữ liệu nền bao gồm những gì?
  
  - [ ] 12_NHÓM_2_PHÁT_SINH.md (Sections 1-3)
    - [ ] Chấm công table structure
    - [ ] Sản lượng & DRC table structure
    - [ ] Phụ cấp định nghĩa
  
  - [ ] 13_NHÓM_3_XỬ_LÝ.md (Sections 1-2.2)
    - [ ] 5 Phases là gì?
    - [ ] Công thức Phase 3 là gì?
    - [ ] Phase 5 output = lương net?
  
  - [ ] 14_NHÓM_4_BÁO_CÁO.md (Sections 1-2)
    - [ ] Báo cáo cần columns nào?
    - [ ] Phân bổ chi phí tính sao?

- [ ] **Từ Bảng Lương Excel:**
  - [ ] So sánh: Công thức ở TRẠM 1/2 = Phase 3-5 không?
  - [ ] So sánh: Phụ cấp = Nhóm 2 allowances không?
  - [ ] So sánh: Hạng KT = technical_grade không?
  - [ ] So sánh: Báo cáo = Nhóm 4 output không?

### 5.3 Checklist Chuẩn Bị Dữ Liệu Input

**Để Triển Khai Hệ Thống (Ưu Tiên):**

- [ ] **Tier 1 - MUST HAVE (Ngay lập tức)**
  - [ ] Danh sách nhân viên (MSNV, Name, Grade, Department, Tram)
  - [ ] Hệ số DRC (Fresh Weight Coefficient)
  - [ ] Bảng đơn giá theo hạng A/B/C/D (trong Bath)
  - [ ] Tỷ giá tháng 12/2025: 1 Bath = ? Kíp
  - [ ] Phụ cấp cán bộ: Mức cộng hàng tháng

- [ ] **Tier 2 - SHOULD HAVE (Tuần này)**
  - [ ] Danh sách phụ cấp chi tiết (cây non, khó, etc.)
  - [ ] Bảng hệ số bảo hiểm (8% BHXH, 1.5% BHYT)
  - [ ] Ngưỡng thuế & suất thuế 6 bậc
  - [ ] Mã kế toán cho từng bộ phận

- [ ] **Tier 3 - NICE TO HAVE (Khi sẵn sàng)**
  - [ ] Danh sách ngày lễ 2026
  - [ ] Quy trình tách DIRECT/INDIRECT
  - [ ] Định nghĩa vùng khó khăn & hệ số support

---

## 📚 SECTION 6: HÀNH ĐỘNG TIẾP THEO

### 6.1 Tuần 1 (This Week)

**Schedule:**

| Thứ | Công Việc | Source File | Dòng Thời Gian |
|---|---|---|---|
| T2 | Xác định data inventory | Bảng lương Excel hiện tại | 1h |
| T3 | Kiểm tra cấu trúc dữ liệu | SECTION 3 file này | 1.5h |
| T4 | So sánh Excel ↔ Hệ thống mới | SECTION 4 file này | 1h |
| T5 | Export 5 file cơ bản | Tài liệu này + Excel | 2h |
| T6 | Gửi yêu cầu PTO | Email template | 0.5h |

**Goal:** 
- ✅ Hoàn thành checklist Section 5.1
- ✅ Có 5 file cơ bản export
- ✅ Gửi email yêu cầu PTO

---

### 6.2 Tuần 2 (Next Week)

**Schedule:**

| Ngày | Công Việc | Source File | Dòng Thời Gian |
|---|---|---|---|
| All | Đọc Nhóm 1-4 Chi Tiết | 11-14 MD files | 3-4h |
| All | Chuẩn Bị Test Data | Export files | 1-2h |
| All | Xây Dựng Database Schema | 6_DATABASE_SCHEMA_ERD.md | 2-3h |

**Goal:**
- ✅ Hiểu toàn bộ 5 Phases tính lương
- ✅ Database schema ready
- ✅ Test data chuẩn bị

---

## 🎓 KEY TAKEAWAYS

```
1. BẢNG LƯƠNG ĐỘI HIỆN TẠI = 5 Lớp Dữ Liệu
   ├─ Lớp 1: Danh Mục Nhân Sự → [Nhóm 1]
   ├─ Lớp 2: Chấm Công & Sản Lượng → [Nhóm 2]
   ├─ Lớp 3: Xử Lý (TRẠM 1/2) → [Nhóm 3]
   ├─ Lớp 4: THCKT (Tổng Hợp) → [Nhóm 3 Output]
   └─ Lớp 5: Báo Cáo (LƯƠNG ĐỘI) → [Nhóm 4]

2. HỆ THỐNG MỚI = 4 NHÓM
   ├─ Nhóm 1: Master Data (Bảng cứng không đổi)
   ├─ Nhóm 2: Transactions (Dữ liệu tháng)
   ├─ Nhóm 3: Processing (5 Phases tính lương)
   └─ Nhóm 4: Reporting (Báo cáo & Phân bổ)

3. MAPPING RÕ RÀNG = DỄ TRIỂN KHAI
   ├─ Xác định cái cũ = cái mới
   ├─ So sánh công thức
   ├─ Chuẩn bị dữ liệu input
   └─ Xây dựng theo hệ thống → Không mất công

4. ECOTECH 2A = Bath (Input) → Kíp (Output)
   ├─ Khác với Excel cũ (chưa rõ tiền tệ)
   ├─ Phải tính tỷ giá hàng tháng
   ├─ DIRECT ≠ INDIRECT (Insurance khác)
   └─ DRC & Hạng KT = KỲ THEN!!!
```

---

**📋 File Name:** 20_HUONG_DAN_TRINH_LAYOUT_TAI_LIEU.md  
**Status:** READY FOR USE  
**Date:** 11/04/2026  
**Version:** 1.0

