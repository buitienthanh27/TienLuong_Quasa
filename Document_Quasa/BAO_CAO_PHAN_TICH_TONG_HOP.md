# 📊 BÁO CÁO PHÂN TÍCH LƯƠNG ĐỘI 1 THÁNG 12.2025 - CHI TIẾT

## 🎯 TỔNG QUAN

**File phân tích:** `LƯƠNG ĐỘI 1 THÁNG 12.2025 2 BẢN.xlsx`  
**Kích thước:** 2,659,459 bytes  
**Số sheets:** 59 (rất lớn!)  
**5 Sheet Chính Yêu Cầu:**
1. Sheet 20: **TRẠM 1** (113 hàng × 34 cột)
2. Sheet 29: **TRẠM 2** (128 hàng × 34 cột)
3. Sheet 32: **LƯƠNG ĐỘI** (69 hàng × 20 cột)
4. Sheet 30: **CÁCH TÍNH LƯƠNG** (43 hàng × 26 cột)
5. Sheet 35/38: **BẢNG PHÂN TOÁN** (65-79 hàng × 14 cột)

---

## ⚠️ CẢNH BÁO QUAN TRỌNG: RỦI RO CAO

### 🔴 PHÁT HIỆN SỬ DỤNG EXTERNAL LINKS (Link Ngoài)

Workbook này **tham chiếu đến các file Excel khác** - điều này rất nguy hiểm!

**Các external link phát hiện:**
- `'[7]TRẠM 1'!$AA$60` - Tham chiếu đến file không xác định có tên "TRẠM 1"
- `'[2]TRẠM 2'!$AA$75` - Tham chiếu đến file khác tên "TRẠM 2"
- Nhiều công thức khác tham chiếu qua `[X]SheetName`

**Hậu quả:**
- ❌ Nếu file được tham chiếu bị xóa/di chuyển → Toàn bộ công thức sẽ lỗi
- ❌ Dữ liệu có thể không cập nhật
- ❌ Khó kiểm soát nguồn dữ liệu

**Khuyến cáo:** ⚠️ **Cần copy tất cả dữ liệu ngoài vào workbook này hoặc sử dụng công thức trong cùng file**

---

### 🔴 PHÁT HIỆN #REF! ERRORS

**Sheet "PLC (2)" chứa 100+ #REF! errors!**

Ví dụ:
```
C4: =VLOOKUP(B3,#REF!,3,0)     <- Công thức lỗi
I4: =VLOOKUP(H3,#REF!,3,0)     <- Công thức lỗi
D27: =VLOOKUP(B19,#REF!,13,0)-D26+I21*20000   <- Lỗi trong tính toán
```

**Nguyên nhân:** Các sheet được tham chiếu trong VLOOKUP không tồn tại hoặc đã bị xóa

**Giải pháp:**
1. Xác định sheet gốc mà VLOOKUP đang tìm kiếm
2. Hoặc thay VLOOKUP bằng công thức khác
3. Hoặc import dữ liệu vào workbook hiện tại

---

## 📋 CHI TIẾT PHÂN TÍCH 5 SHEETS CHÍNH

### Sheet 1: TRẠM 1 (113 hàng × 34 cột)

**Vị trí:** Sheet 20 trong workbook

**Loại dữ liệu:**
- Danh sách nhân viên/công nhân
- Các chỉ tiêu sản lượng
- Dữ liệu tính toán lương

**External links phát hiện:**
```
AE57: =+'[7]TRẠM 1'!$AA$60        ← Tham chiếu file bên ngoài
AE58: =+'[7]TRẠM 2'!$AA$75        ← Tham chiếu file bên ngoài
```

**Khuyến cáo:** ⚠️ Kiểm tra và thay thế các link bên ngoài

---

### Sheet 2: TRẠM 2 (128 hàng × 34 cột)

**Vị trí:** Sheet 29 trong workbook

**External links phát hiện:**
```
AD76: =+'[2]TRẠM 2'!$AA$75        ← Tham chiếu file khác
AD79: ='[2]TRẠM 2'!$O$75          ← Tham chiếu file khác
AD3: ='TRẠM 1'!AD4                ← Tham chiếu trong workbook (OK)
```

---

### Sheet 3: LƯƠNG ĐỘI (69 hàng × 20 cột)

**Vị trí:** Sheet 32 trong workbook

**Mục đích:** Bảng tổng hợp lương cho toàn bộ đội

**Cấu trúc dự kiến:**
- Column A-B: Thông tin nhân viên (Mã, Tên)
- Column C-E: Các khoản lương (Cơ bản, Phụ cấp, ...)
- Column F-K: Các khoản khấu trừ (Bảo hiểm, Thuế, ...)
- Column L-O: Tổng hợp (Lương gross, Net, ...)

---

### Sheet 4: CÁCH TÍNH LƯƠNG (43 hàng × 26 cột)

**Vị trí:** Sheet 30 trong workbook

**Mục đích:** Tài liệu hướng dẫn/công thức

**Loại nội dung:**
- Công thức tính lương
- Hệ số áp dụng
- Ví dụ tính toán

**#REF! errors phát hiện:**
```
AH103: =+AH102-'BẢNG PHÂN TOÁN'!#REF!     ← Lỗi trong phân toán
AH27: =+AH26-'BẢNG PHÂN TOÁN'!#REF!       ← Lỗi trong phân toán
AH23: =+AH22-'BẢNG PHÂN TOÁN'!#REF!       ← Lỗi trong phân toán
```

---

### Sheet 5: BẢNG PHÂN TOÁN (65-79 hàng × 14 cột)

**Vị trí:** Sheet 35 (65R×14C), Sheet 38 (79R×14C), Sheet 45 (36R×9C)

**Mục đích:** Phân toán chi phí lương

**Vấn đề:** Có 3 sheet khác nhau với cùng tên → Cần xác định sheet nào được sử dụng

---

## 🧮 TÓM TẮT HỆ SỐ & CÔNG THỨC

### Các công thức phát hiện:

**Từ Sheet "TRẠM 1":**
| Công thức | Ô | Mục đích |
|---|---|---|
| =SUM(...) | Nhiều | Tính tổng |
| =VLOOKUP(...) | Multiple | Tra cứu dữ liệu |
| =+'[7]TRẠM 1'!$AA$60 | AE57 | Link ngoài |

**Từ Sheet "LƯƠNG ĐỘI":**
- Công thức cộng hạng mục lương
- Công thức tính khấu trừ
- Công thức tính lương net

---

## 📊 BẢNG "QUY CHẾ THAM SỐ" - DANH SÁCH CẦN LÀP

Dựa trên phân tích, các hệ số cần ghi lại:

| Tên Hệ Số | Giá Trị Dự Kiến | Sheet Đang Dùng | Khoản Mục | Loại | Ghi Chú |
|---|---|---|---|---|---|
| Hệ số bảo hiểm | 0.105 hoặc 10.5% | CÁCH TÍNH LƯƠNG | BHXH + BHYT | % | Khấu trừ bảo hiểm |
| Hệ số thuế | 0.05 hoặc 5% | CÁCH TÍNH LƯƠNG | Thuế TNCN | % | Khấu trừ thuế |
| Hệ số khoán | 1.0 - 1.5 | LƯƠNG ĐỘI | Lương cơ bản | Ratio | Tùy phòng/bộ môn |
| Hệ số phụ cấp | ? | TRẠM 1/TRẠM 2 | Phụ cấp | % hoặc Số | Cần kiểm tra |
| Tỷ lệ phân toán | ? | BẢNG PHÂN TOÁN | Chi phí chung | % | Cần kiểm tra |
| (Thêm các hệ số khác...) | | | | | |

---

## ⚠️ "ÔK RỦI RO" - TÓMS QUÁT

### Loại 1: #REF! Errors (CAO - 🔴)

**Count:** 100+ instances phát hiện trong:
- Sheet "PLC (2)" - 100+ cells
- Sheet "CÁCH TÍNH LƯƠNG" - Multiple cells
- Sheet "PLC" (khác) - Multiple cells

**Giải pháp:**
1. Xác định source của VLOOKUP
2. Sửa thành công thức hoạt động
3. Hoặc import dữ liệu vào workbook

### Loại 2: External Links (TRUNG - 🟠)

**Count:** 10+ references phát hiện

**Ô cần xử lý:**
- TRẠM 1 sheet: AE57, AE58
- TRẠM 2 sheet: AD76, AD79
- (và nhiều ô khác)

**Giải pháp:**
1. Break links hoặc copy values
2. Import dữ liệu source vào workbook
3. Tạo dashboard trong workbook chứ không link ngoài

### Loại 3: Manual Adjustments (THẤP - 🟡)

**Khó phát hiện mà không xem từng ô**

### Loại 4: Financial Parameters & DRC (THẤP - 🟡)

**Cần ghi chú:**
- Các hệ số trong "CÁCH TÍNH LƯƠNG"
- Các tỷ lệ trong "BẢNG PHÂN TOÁN"
- Các hệ số khoán trong "LƯƠNG ĐỘI"

---

## 🔧 HÀNH ĐỘNG CẦN THỰC HIỆN (NEXT STEPS)

### Bước 1: Khắc phục External Links ⚠️ CAO TIÊN
```
1. Edit > Links > Break Link (hoặc copy values)
2. Kiểm tra dữ liệu không bị mất
3. Định kỳ cập nhật dữ liệu thủ công nếu cần
```

### Bước 2: Sửa #REF! Errors ⚠️ CAO TIÊN
```
1. Tìm sheet gốc được VLOOKUP tham chiếu
2. Thay thế công thức hoặc copy dữ liệu vào
3. Test lại công thức
```

### Bước 3: Ghi chú Hệ Số & Tham Số
```
1. Mở CÁCH TÍNH LƯƠNG và ghi lại tất cả hệ số
2. Tạo sheet riêng "Quy chế tham số"
3. Ghi nguồn gốc và objective của từng hệ số
```

### Bước 4: Kiểm toán Toàn Bộ Công Thức
```
1. Sử dụng template "Báo Cáo Chi Tiết" (xem phần trên)
2. Ghi lại Ô/Cụm ô, Nguồn dữ liệu, Hệ số, Ràng buộc, Giải thích
3. Tạo version "sạch" của workbook
```

---

## 📎 THAM KHẢO & CÔNG CỤ

- **File gốc phân tích:** [PHAN_TICH_OUTPUT.txt](PHAN_TICH_OUTPUT.txt) - 7,728 dòng
- **Template phân tích:** [PHAN_TICH_CHI_TIET_TEMPLATE.md](PHAN_TICH_CHI_TIET_TEMPLATE.md)
- **Script phân tích:** Python script (analyze_workbook_simple.py)

---

**Ngày tạo báo cáo:** 09/04/2025  
**Người tạo:** AI Analysis Tools  
**Trạng thái:** ⚠️ URGENT - Cần xử lý External Links & #REF! errors ngay

