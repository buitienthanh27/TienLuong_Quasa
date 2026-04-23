# HƯỚNG DẪN PHÂN TÍCH FILE EXCEL - LƯƠNG ĐỘI 1

## 📋 BƯỚC 1: CHUẨN BỊ

File cần phân tích:
```
2025/tháng 12/Đội 1/LƯƠNG ĐỘI 1 THÁNG 12.2025 2  BẢN.xlsx
```

Yêu cầu phân tích:
1. ✅ **Từ điển cột** cho mỗi sheet
2. ✅ **Bảng Quy chế tham số** - trích xuất hệ số ngầm
3. ✅ **Đánh dấu ô rủi ro** (#REF!, link ngoài, ô điều chỉnh tay, ô tham số tài chính)

---

## 📊 BƯỚC 2: MỞ FILE VÀ KHÁM PHÁ SHEETS

Mở file Excel và kiểm tra **tất cả các sheets** có trong workbook.

Các sheet cần phân tích:
- [ ] TRẠM 1
- [ ] TRẠM 2
- [ ] LƯƠNG ĐỘI
- [ ] CÁCH TÍNH LƯƠNG
- [ ] BẢNG PHÂN TOÁN
- [ ] (Sheet khác: ___________________)

---

## 📋 BƯỚC 3: LẬP TỪ ĐIỂN CỘT

Cho **mỗi sheet**, tạo một bảng ghi lại:

### Template Từ Điển Cột

| Sheet Name | Cột | Header | Kiểu dữ liệu | Ghi chú |
|---|---|---|---|---|
| TRẠM 1 | A | | | |
| TRẠM 1 | B | | | |
| TRẠM 1 | C | | | |
| ... | ... | ... | ... | ... |

**Kiểu dữ liệu có thể là:**
- Text (A, B, C, ...)
- Number (Số tiền, hệ số)
- Date (Ngày tháng)
- Formula (Công thức)
- Reference (Link, tham chiếu)

---

## 🔧 BƯỚC 4: TRÍ XUẤT BẢNG "QUY CHẾ THAM SỐ"

Tạo một sheet riêng tên **"Quy chế tham số"** với cấu trúc:

| Tên Hệ Số | Giá Trị | Sheet Đang Dùng | Khoản Mục Chịu Tác Động | Ghi Chú |
|---|---|---|---|---|
| Hệ số lương cơ bản | 1.0 | LƯƠNG ĐỘI | Tổng lương | |
| Hệ số bảo hiểm | 0.105 | CÁCH TÍNH LƯƠNG | Tính lương net | |
| Hệ số thuế | 0.05 | CÁCH TÍNH LƯƠNG | Khấu trừ thuế | |
| ... | ... | ... | ... | ... |

**Cách tìm hệ số ngầm:**
1. ✅ Tìm tất cả công thức =...
2. ✅ Nhìn các ô có % hoặc số thập phân
3. ✅ Xem các ô được "tham chiếu" nhiều lần
4. ✅ Kiểm tra các ô được "khóa" ($A$1)

---

## ⚠️ BƯỚC 5: ĐÁI DẤU ÔK RỦI RO

Tạo sheet **"Ô Rủi Ro"** với cấu trúc:

| Loại Rủi Ro | Sheet | Ô | Công Thức / Giá Trị | Mức Độ | Hành Động Khuyến Cáo |
|---|---|---|---|---|---|
| #REF! error | CÁCH TÍNH LƯƠNG | F15 | =INDIRECT(...) | 🔴 CAO | Sửa công thức |
| External Link | LƯƠNG ĐỘI | B5 | =[file.xlsx]Sheet!A1 | 🟠 TRUNG | Cập nhật hoặc xóa link |
| Manual adjustment | TRẠM 1 | D20 | 150000 | 🟡 THẤP | Ghi chú lý do |
| DRC Parameter | BẢNG PHÂN TOÁN | C10 | 0.85 | 🟡 THẤP | Giám sát thay đổi |

**Các loại rủi ro cần kiểm tra:**
1. ❌ **#REF!** - Công thức tham chiếu đã bị xóa
2. 🔗 **Link ngoài** - Công thức tham chiếu file khác
3. 🖱️ **Ô điều chỉnh tay** - Giá trị nhập thủ công (không có công thức)
4. 📊 **Ô tham số tài chính & DRC** - Chứa hệ số, tỷ lệ

---

## 📝 BƯỚC 6: TẠO BÁO CÁO CHI TIẾT

Sau khi hoàn thành 3 bước trên, tạo **báo cáo tổng hợp** với format:

| Ô/Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ |
|---|---|---|---|---|
| B5:B20 | Manual input | - | Cộng = B5:B20 | Danh sách nhân viên |
| C5:C20 | TRẠM 1!D10 | ×1.0 | C > 0 | Lương cơ bản từ TRẠM 1 |
| D5:D20 | =C5*$E$1 | E1 (1.2) | D >= C | Lương khoán = cơ bản × hệ số |
| E5:E20 | =D5*$E$2 | E2 (0.105) | E <= D×10% | Bảo hiểm = khoán × 10.5% |
| ... | ... | ... | ... | ... |

---

## 💡 MẸO & CHÚ Ý

### Để tìm công thức:
- Sử dụng **Ctrl+`** để hiện công thức
- Sử dụng **Find & Replace (Ctrl+H)** tìm "="
- Nhấp vào ô và xem **Formula Bar** trên cùng

### Để tìm hệ số:
- Tìm ô được "khóa" (==**$A$1**)
- Tìm ô chứa %
- Tìm ô có giá trị 0 < x < 1

### Để tìm link ngoài:
- Find tìm "**[**" (dấu mở ngoặc vuông)
- Hoặc **Ctrl+Shift+F5** > Edit Links

### Để tìm #REF!:
- Sử dụng **Find & Replace** tìm "**#REF!**"

---

## 📌 DANH SÁCH KIỂM TRA

- [ ] Đã mở file LƯƠNG ĐỘI 1 THÁNG 12.2025
- [ ] Đã liệt kê tất cả sheets
- [ ] Đã tạo **từ điển cột** cho từng sheet
- [ ] Đã tạo **bảng Quy chế tham số**
- [ ] Đã tạo **bảng Ô Rủi Ro** và đánh dấu
- [ ] Đã tạo **báo cáo tổng hợp** chi tiết (nếu cần)
- [ ] Đã lưu file phân tích với tên: `PHÂN_TÍCH_[Tên_File_Gốc].xlsx`

---

## 📨 OUTPUT CUỐI CÙNG

Tạo file mới tên: **PHÂN_TÍCH_LƯƠNG_ĐỘI_1_THÁNG_12.xlsx**

Chứa các sheet:
1. **Từ Điển Cột** - Liệt kê tất cả cột từ mỗi sheet
2. **Quy Chế Tham Số** - Bảng hệ số ngầm
3. **Ô Rủi Ro** - Danh sách cảnh báo
4. **Báo Cáo Chi Tiết** - Phân tích sâu (nếu có)
5. **Ghi Chú Nghiệp Vụ** - Tham khảo thêm

