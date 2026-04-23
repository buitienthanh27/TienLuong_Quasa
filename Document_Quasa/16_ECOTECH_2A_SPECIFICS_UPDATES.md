# 📌 CẬP NHẬT ECOTECH 2A - THÔNG TIN CHI TẾT TỨ CUỘC HỌP

**Ngày:** 10/04/2025
**Công Ty:** ECOTECH 2A - Khai Thác Cao Su Lào
**Status:** Thông tin từ cuộc họp xác nhận, cần cập nhật vào hệ thống

---

## 📋 TÓNG GỠP THIẾU THIỀU

| STT | NỘI DUNG | FILE CẦN CẬP NHẬT | NGUYÊN NHÂN |
|---|---|---|---|
| 1 | Tiền tệ kép (Bath + Kíp) | 11, 12, 13, 14 | ECOTECH tính Bath, báo cáo Kíp |
| 2 | Hạng kỹ thuật A-D | 11, 12 | CNKT có 4 hạng đơn giá riêng |
| 3 | Quy khô (DRC) | 12, 13 | Tính từ khối lượng tươi |
| 4 | Mã lao động 2026 | 11 | Phòng Tổ chức phát hành, format mới |
| 5 | Tỷ giá hàng tháng | 11, 13 | Vietinbank → Phòng Tổ chức xác nhận |
| 6 | Bảo hiểm chỉ cán bộ | 13 | CN kỹ thuật trực tiếp KHÔNG có |
| 7 | Lương ngày lễ | 12, 13 | Trả mặt → Tính tạm ứng |
| 8 | Vùng khó khăn | 11 | Support thêm đơn giá |
| 9 | Phân loại CN (DIRECT/INDIRECT) | 11, 12 | Kíp vs Đô |
| 10 | Bổ công chăm sóc | 12 | Outsource hoặc nội bộ |

---

## 📝 CHÍNH SÁCH CHI TIẾT CẦN CẬP NHẬT

### 1. MÃ LAO ĐỘNG 2026

**Từ Anh Nhật - Phòng Tổ Chức:**
- Có quy định mới về mã lao động năm 2026
- Cần **cài đặt tự động phát sinh mã** theo quy tắc
- **Hành động:** Đợi PTO cung cấp quy tắc chi tiết → Cài vào hệ thống

**Cập nhật vào:**
- File 11 → Section "Hồ Sơ Lao Động" → Thêm format mã
- Employee Table → Field: `msnv VARCHAR(20)` (không phải INT)
- System procedure → Auto-generate MSNV theo format 2026-NNN

---

### 2. TIỀN TỆ KÉP: BATH + KÍP

**Từ Chị Phượng - Phòng TCTL:**
- Đơn giá lương tính theo **Bath (THB)** (tiền Thái)
- Sau đó **quy ra Kíp (LAK)** theo tỷ giá hàng tháng (tiền Lào)
- Đơn giá còn lại (công việc chăm sóc, phụ cấp) tính theo **Kíp**
- Tất cả báo cáo: **Kíp**

**Chi Tiết:**
```
Lương CNKT:
├─ Tính từ DRC × Đơn Giá (Bath) × Hạng
├─ Quy ra Kíp: Lương_Bath × Tỷ_Giá = Lương_Kíp
└─ Báo cáo: Chỉ Kíp

Lương Khác (Phụ cấp, Chăm sóc):
├─ Tính trực tiếp Kíp
└─ Báo cáo: Kíp
```

**Cập nhật vào:**
- File 11 → Section "Cài Đơn Giá" → Thêm cột Bath/Kíp
- File 12 → "Đánh Giá Kỹ Thuật" → Explain DRC tính Bath
- File 13 → "Phase 3" → Công thức quy đổi Bath → Kíp
- File 14 → "Báo Cáo" → Tất cả tính Kíp

**Database Table Cần Thêm:**
```sql
CREATE TABLE exchange_rates (
  rate_id INT AUTO_INCREMENT PRIMARY KEY,
  year_month VARCHAR(7),
  exchange_rate DECIMAL(10,4),  -- 1 Bath = ? Kíp
  source VARCHAR(50),            -- Vietinbank
  confirmed_by VARCHAR(50),      -- Phòng Tổ chức
  confirmed_date TIMESTAMP
);
```

---

### 3. HẠNG KỸ THUẬT: A, B, C, D

**Từ Chị Phượng - Phòng TCTL:**
- Hạng kỹ thuật: **A, B, C, D** (mỗi hạng đơn giá riêng)
- Hệ số tính điểm riêng từng hạng (do phòng kỹ thuật quy định)
- **QLKT chỉ đánh giá 1 lần vào cuối tháng**
  - Chấm ghi nhận tại đội → Có phúc tra → Lấy số liệu cuối cùng
- Đánh giá ngẫu nhiên các phần cây

**Chi Tiết:**
```
Hạng A: Đơn giá cao nhất
Hạng B: Đơn giá thấp hơn A
Hạng C: Đơn giá thấp hơn B
Hạng D: Đơn giá thấp nhất (nếu có)

Ví Dụ (tùy thực tế ECOTECH):
├─ Hạng A: 1,500,000 Kíp/kg
├─ Hạng B: 1,300,000 Kíp/kg
├─ Hạng C: 1,100,000 Kíp/kg
└─ Hạng D: 900,000 Kíp/kg
```

**Cập nhật vào:**
- File 11 → Section "Hồ Sơ Lao Động" → Field `technical_grade CHAR(1)`
- File 11 → Section "Cài Đơn Giá" → Thêm bảng giá theo hạng A-D
- File 12 → "Đánh Giá Kỹ Thuật" → Explain quy trình đánh giá
  - 1 lần/tháng, cuối tháng
  - Phúc tra: lấy số liệu cuối
- File 13 → "Phase 1" → Sửa công thức (coefficient từ hạng)

**Employee Table Thêm:**
```sql
ALTER TABLE employees ADD technical_grade CHAR(1);  -- A/B/C/D
```

---

### 4. QUY KHÔ (DRC - Dry Rubber Content)

**Từ Chi Tiết Từ Sheet:**
- Quy khô CNKT: Dựa trên **số DRC chốt cuối tháng** do Trạm cán + Đội chốt
- Công thức:
  ```
  Quy_Khô = DRC × Khối_Lượng_Tươi (GIỮ NGUYÊN thập phân!)
  ```
- **Thành tiền CNKT = Quy Khô × Đơn Giá (hạng A/B/C/D) → LÀMTRÒN**

**Chi Tiết:**
```
VD: CNKT 001 - Tháng 4/2026
├─ Khối lượng tươi: 500 kg
├─ DRC (hệ số): 0.30
├─ Quy khô = 500 × 0.30 = 150.00 kg (GIỮ thập phân)
├─ Hạng: A
├─ Đơn giá (hạng A): 1,500,000 Kíp/kg
├─ Thành tiền = 150.00 × 1,500,000 = 225,000,000 Kíp
└─→ LÀMTRÒN (không lấy 0.00, chỉ 225,000,000)
```

**Cập nhật vào:**
- File 12 → Section "Đánh Giá Kỹ Thuật" → Thêm sub-section "DRC"
  - Nhập: Khối lượng tươi (kg)
  - Auto: DRC × khối lượng = Quy khô
- File 13 → "Phase 3" → Công thức tính từ DRC
- File 14 → Báo cáo thành tiền CNKT

**Database Table Cần Thêm:**
```sql
CREATE TABLE rubber_drc (
  drc_id INT AUTO_INCREMENT PRIMARY KEY,
  msnv VARCHAR(20),
  year_month VARCHAR(7),
  fresh_weight DECIMAL(12,2),       -- kg tươi
  drc_coefficient DECIMAL(5,4),     -- VD: 0.30
  dry_weight DECIMAL(12,4),         -- = fresh × drc (GIỮ thập phân)
  technical_grade CHAR(1),
  unit_price DECIMAL(12,0),
  total_amount DECIMAL(14,0),       -- = dry × unit (LÀMtròn)
  FOREIGN KEY (msnv) REFERENCES employees(msnv)
);
```

---

### 5. LƯƠNG NGÀY LỄ

**Từ Chí Tiết Sheet:**
- CN kỹ thuật **đi làm ngày lễ → trả tiền mặt trực tiếp**
- Tính vào mục **"tạm ứng"** (temporary advance)
- Mức chi trả **không cố định** (tuỳ VP, công ty)
- **Cuối tháng:** Ghi nhận vào bảng lương để **trừ lại tiền tạm ứng**

**Chi Tiết:**
```
Quá Trình:
└─ Ngày Lễ CN làm
   ├─ Trả tiền mặt: 1,000,000 Kíp (VD)
   ├─ Ghi chép: "Tạm ứng ngày lễ"
   └─ Status: Chưa có trong bảng lương

└─ Cuối tháng:
   ├─ Ghi nhận: +1,000,000 Kíp (tạm ứng)
   ├─ Tính lương bình thường: 18,000,000 Kíp
   └─ Trừ tiền tạm ứng: -1,000,000 Kíp
      → Lương thực nhận: 17,000,000 Kíp
```

**Cập nhật vào:**
- File 12 → "Công Nợ & Tạm Ứng" → Thêm chi tiết về tạm ứng ngày lễ
- File 13 → "Phase 5" → Xử lý tạm ứng (trừ từ lương)
- File 14 → Báo cáo rõ: Nợ không trừ vs Trừ từ lương

---

### 6. BẢO HIỂM - CHỈ CÓ CÁN BỘ

**Từ Sheet STT 7 & 14:**
- **CN trực tiếp KHÔNG tham gia bảo hiểm** (100% tại Lào)
- **Một số CN kỹ thuật ở Lào vẫn đóng bảo hiểm** (cũ, năm 1 lương)
- **CN gián tiếp (cán bộ) CÓ bảo hiểm**

**Chi Tiết:**
```
DIRECT (CN Kỹ Thuật - Lào):
├─ KHÔNG BHXH, KHÔNG BHYT
├─ KHÔNG đóng bảo hiểm
└─ Lương = Base (không khấu trừ)

INDIRECT (Cán Bộ VP):
├─ CÓ BHXH (8%)
├─ CÓ BHYT (1.5%)
└─ Lương = Base - BHXH - BHYT
```

**Cập nhật vào:**
- File 11 → "Hồ Sơ Lao Động" → Field `insurance_included BOOLEAN` (chỉ indirect)
- File 13 → "Phase 4" → Sửa: Chỉ tính BHXH/BHYT nếu `insurance_included = 1`
- File 12 → "Bảo Hiểm" → Rõ: Chỉ cán bộ

**Database:**
```sql
ALTER TABLE employees ADD insurance_included BOOLEAN DEFAULT 0;
```

---

### 7. VÙNG KHÓ KHĂN - HỖ TRỢ THÊM ĐƠN GIÁ

**Từ Chi Tiết Sheet:**
- Ở những vùng khó khăn, công ty hỗ trợ đơn giá thêm
- Định nghĩa vùng khó khăn: Phòng kỹ thuật quy định

**Chi Tiết:**
```
Ví Dụ:
├─ Vùng bình thường: Đơn giá = 1,000,000 Kíp
├─ Vùng khó khăn:
│  ├─ Hệ số hỗ trợ: 1.15 (tăng 15%)
│  └─ Đơn giá thực: 1,000,000 × 1.15 = 1,150,000 Kíp
```

**Cập nhật vào:**
- File 11 → "Hồ Sơ Lao Động" → Field `difficult_zone BOOLEAN`
- File 11 → Section mới: "Hệ Số Hỗ Trợ Vùng Khó Khăn"
- File 13 → "Phase 2" → Công thức: Base × coefficient_zone

**Database:**
```sql
ALTER TABLE employees ADD difficult_zone BOOLEAN DEFAULT 0;
CREATE TABLE zone_support (
  zone_id INT AUTO_INCREMENT PRIMARY KEY,
  zone_code VARCHAR(20),
  support_coefficient DECIMAL(5,2)
);
```

---

### 8. PHÂN LOẠI CÔNG NHÂN: DIRECT vs INDIRECT

**Từ Sheet STT 8:**
- **DIRECT:** CN trực tiếp 100% tại Lào, trả Kíp
- **INDIRECT:** CN gián tiếp, trả Đô, quy sang Kíp để báo cáo
- Công ty đang làm nội quy, nên phân loại rõ

**Chi Tiết:**
```
DIRECT (Trực Tiếp):
├─ CN kỹ thuật khai thác
├─ Tính lương: Kíp
├─ KHÔNG bảo hiểm
└─ Báo cáo: Kíp

INDIRECT (Gián Tiếp):
├─ Cán bộ VP
├─ Tiền thường Đô, quy sang Kíp báo cáo
├─ CÓ bảo hiểm
└─ Báo cáo: Quy sang Kíp
```

**Cập nhật vào:**
- File 11 → "Hồ Sơ Lao Động" → Field `employee_type ENUM('DIRECT', 'INDIRECT')`
- File 12 → "Phân Loại Công Nhân" → Giải thích khác nhau
- File 13 → "Phase 3" → Tính toán khác cho từng loại

---

### 9. LƯƠNG CHĂM SÓC VƯỜN CÂY

**Từ Sheet STT 3:**
- Có thể thuê ngoài (khoán) hoặc công nhân nội bộ
- **Năm ngoái:** Công nhân đội chăm sóc
- **Năm nay:** Thuê ngoài
- **Đơn giá hiện tại:** 25,000 Kíp

**Chi Tiết:**
```
Hai Hình Thức:
├─ Outsource (khoán):
│  ├─ Thuê bên ngoài quản lý
│  ├─ Trả cứng: 25,000 Kíp/unit
│  └─ Không phức tạp
│
└─ Nội Bộ:
   ├─ Công nhân đội chăm sóc
   ├─ Tính lương giống CNKT (DRC)
   └─ Áp dụng: Nếu năng cây theo hạng
```

**Cập nhật vào:**
- File 12 → "Bổ Công Chăm Sóc" → Thêm chi tiết về "Lương Chăm Sóc"
  - Nếu nội bộ: Tính từ DRC × Đơn giá
  - Nếu outsource: Trả cứng
- File 13 → Phase 3: Kiểm tra có CN chăm sóc không → Cách tính

---

### 10. PHỤ CẤP - CỐ ĐỊNH

**Từ Sheet STT 4:**
- Các loại phụ cấp thường có giá cố định
- Thay đổi khi có văn bản ban hành mới
- **Người phụ trác:** Chị Phượng sẽ cung cấp danh sách

**Chi Tiết:**
```
Phụ Cấp Thường:
├─ Phụ cấp ngày lễ (nếu không đi làm đó không có)
├─ Phụ cấp khác (tuỳ công ty)
└─ Cập nhật: 1 năm 1 lần (hoặc có văn bản)
```

**Cập nhật vào:**
- File 11 → "Cài Hệ Số" → Bảng phụ cấp cậu định (giảng sau)
- File 12 → "Phụ Cấp & Chế Độ" → Danh sách phụ cấp
- Đợi: Chị Phượng cung cấp file "cài định định mức công chăm sóc & phụ cấp"

---

### 11. LOẠI LƯƠNG: KHỐI VP & KHÁC

**Từ Sheet STT 6:**
- **Lương VP ở Đội:** Chưa  có trong file hiện tại
- **Lương Bảo Vệ, Tạp Vụ:** Tính vào trực tiếp (do Đội phụ trách)

**Chi Tiết:**
```
Loại Công Nhân:
├─ CNKT (Kỹ Thuật): Tính từ DRC
├─ VP (Văn Phòng): Lương cơ bản + phụ cấp
├─ Bảo Vệ: Lương trực tiếp (cô định)
└─ Tạp Vụ: Lương trực tiếp (có định)
```

**Cập nhật vào:**
- File 12 → "Phân Loại Công Nhân" → Thêm VP, Bảo vệ, Tạp vụ
- File 11 → "Hồ Sơ Lao Động" → Field `position_code` (công ty cung cấp)
- Phòng Tổ chức cần cung cấp: Mã vị trí 2026 (Anh Nhật)

---

## 📁 FILES CẦN YÊU CẦU TỪ ECOTECH 2A

Theo yêu cầu của khách hàng (Sheet STT 13):

| # | File Cần | Người Cung Cấp | Mục Đích |
|---|---|---|---|
| 1 | File quy trình kỹ thuật | Phòng Kỹ Thuật | Chấm điểm CNKT, xếp hạng A-D |
| 2 | File xác nhận tỷ giá hàng tháng | P Tổ chức (get từ P Kế toán) | Tỷ giá Bath/Kíp |
| 3 | File bổ công & định mức chăm sóc | P Tổ chức (get từ Chị Phượng) | Phụ cấp, lương chăm sóc |
| 4 | File quy định ngày lễ & phụ cấp | P Tổ chức | Ngày lễ, phụ cấp hàng tháng |
| 5 | File mã lao động 2026 | Anh Nhật - P Tổ chức | Format auto-generate MSNV |

**Hành động:**
- [ ] Gửi email đến: Anh Nhật (PTO), Chị Phượng (TCTL), Phòng Kỹ Thuật
- [ ] Yêu cầu 5 files trên trong tuần
- [ ] Nhận -> Cập nhật vào hệ thống

---

## ✅ CHECKLIST CẬP NHẬT

**File 11 - NHÓM 1 (Dữ Liệu Nền):**
- [ ] Tổng quan: Thêm ECOTECH context
- [ ] Hồ sơ lao động: Format 2026-NNN, DIRECT/INDIRECT, technical_grade, difficult_zone, insurance
- [ ] Cài đơn giá: Thêm Bath/Kíp, Hạng A-D
- [ ] Module: Tỷ giá hàng tháng
- [ ] Module: Quy khô (DRC)
- [ ] Module: Hỗ trợ vùng khó khăn
- [ ] Module: Ngày lễ & Phụ cấp

**File 12 - NHÓM 2 (Phát Sinh Transactions):**
- [ ] Chấm công: Phân DIRECT/INDIRECT
- [ ] Bổ công: Thêm "Tạm ứng ngày lễ"
- [ ] Phụ cấp: Cập nhật cấu trúc
- [ ] Đánh giá: DRC, Hạng A-D
- [ ] Bổ công chăm sóc: Outsource vs nội bộ

**File 13 - NHÓM 3 (Xử Lý Processing):**
- [ ] Phase 1: Hạng A-D coefficient
- [ ] Phase 3: Công thức DRC × Giá, Bath → Kíp
- [ ] Phase 4: BHXH/BHYT chỉ INDIRECT
- [ ] Phase 5: Trừ tạm ứng ngày lễ
- [ ] Reconciliation: Kiểm tra DRC

**File 14 - NHÓM 4 (Báo Cáo Reporting):**
- [ ] Payslip: Tính Kíp, rõ DIRECT/INDIRECT
- [ ] Summary: Tách DIRECT vs INDIRECT
- [ ] Tax/Insurance: Chỉ INDIRECT
- [ ] Allocation: Kíp

**File 15 - Hướng Dẫn:**
- [ ] Quy cycle: Thêm bước DRC cuối tháng
- [ ] Critical issues: Cập nhật 5 files từ EcoTech
- [ ] FAQ: Thêm Q&A về tỷ giá, DRC, hạng

---

## 🚀 KỲ TIẾP THEO

1. **Tuần 1:** Gửi email yêu cầu 5 files
2. **Tuần 2:** Nhận files → Phân tích chi tiết
3. **Tuần 3:** Cập nhật toàn bộ 11-15 files
4. **Tuần 4:** Setup test month (Tháng 4/2026 ECOTECH)
5. **Tuần 5:** Tính lương test → Verify kết quả

---

**📌 FILE CẬP NHẬT CHI TIẾT: 16_ECOTECH_2A_SPECIFICS_UPDATES.md - v1.0**
**Ngày:** 10/04/2025
**Status:** READY FOR IMPLEMENTATION

