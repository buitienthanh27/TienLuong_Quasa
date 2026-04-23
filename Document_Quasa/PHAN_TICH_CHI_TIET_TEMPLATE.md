# 📊 BỘ CÔNG CỤ PHÂN TÍCH LƯƠNG ĐỘI 1 THÁNG 12 - 2025

## 🎯 TÓM TẮT CÔNG VIỆC CẦN LÀMM

| STT | Công Việc | Trạng Thái | Ghi Chú |
|---|---|---|---|
| 1 | Lập "từ điển cột" cho mỗi sheet | ⏳ TBD | Liệt kê tất cả headers |
| 2 | Bóc toàn bộ hệ số ngầm | ⏳ TBD | Tạo bảng "Quy chế tham số" |
| 3 | Đánh dấu ô rủi ro | ⏳ TBD | #REF!, link ngoài, manual cells |

---

## 📋 STEP 1: ĐIỀN "TỪ ĐIỂN CỘT" CHO MỖI SHEET

Hãy **mở file gốc** và lần lượt điền thông tin vào các template dưới đây:

### Sheet: TRẠM 1

Mở TRẠM 1, copy toàn bộ data từ Row 1 (header), sau đó điền vào bảng:

| Cột | Header | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| A | | | |
| B | | | |
| C | | | |
| D | | | |
| E | | | |
| F | | | |
| G | | | |
| H | | | |
| I | | | |
| J | | | |
| K | | | |
| L | | | |
| M | | | |
| N | | | |
| O | | | |
| P | | | |
| Q | | | |
| R | | | |
| S | | | |
| T | | | |

**Kiểu dữ liệu cần lưu ý:**
- `Text` - Văn bản thường
- `Number` - Số tiền, số lượng
- `Date` - Ngày tháng
- `Formula` - Công thức (bắt đầu bằng =)
- `Reference` - Link đến sheet khác hoặc file khác

---

### Sheet: TRẠM 2

| Cột | Header | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| A | | | |
| B | | | |
| C | | | |
| D | | | |
| E | | | |
| F | | | |
| G | | | |
| H | | | |
| I | | | |
| J | | | |

---

### Sheet: LƯƠNG ĐỘI

| Cột | Header | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| A | | | |
| B | | | |
| C | | | |
| D | | | |
| E | | | |
| F | | | |
| G | | | |
| H | | | |
| I | | | |
| J | | | |

---

### Sheet: CÁCH TÍNH LƯƠNG

| Cột | Header | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| A | | | |
| B | | | |
| C | | | |
| D | | | |
| E | | | |
| F | | | |
| G | | | |
| H | | | |

---

### Sheet: BẢNG PHÂN TOÁN

| Cột | Header | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| A | | | |
| B | | | |
| C | | | |
| D | | | |
| E | | | |
| F | | | |

---

## 🧮 STEP 2: BỐC TOÀN BỘ HỆ SỐ NGẦM

### Hướng Dẫn Tìm Hệ Số

1. **Tìm kiếm các ô được khóa ($A$1)**
   - Nhấn Ctrl+H (Find & Replace)
   - Tìm: `\$[A-Z]+\$[0-9]+`
   - Chỉ định: Formulas
   - Đánh dấu "Regular Expression"

2. **Tìm kiếm các ô có %**
   - Nhấn Ctrl+H
   - Tìm: `%`
   - Hoặc format ô: Percentage

3. **Tìm kiếm các ô rõ ràng là hệ số (0 < x < 1)**
   - Nhìn các ô chứa giá trị như 0.105, 0.85, 1.2, v.v.

### Bảng "QUY CHẾ THAM SỐ"

**Hãy điền tất cả hệ số tìm được vào bảng này:**

| Tên Hệ Số | Giá Trị | Loại | Sheet Đang Dùng | Khoản Mục Chịu Tác Động | Ghi Chú |
|---|---|---|---|---|---|
| Hệ số bảo hiểm | 0.105 | Percentage | CÁCH TÍNH LƯƠNG | Tính lương net | Bảo hiểm y tế + BHXH |
| Hệ số thuế | 0.05 | Percentage | CÁCH TÍNH LƯƠNG | Khấu trừ thuế TNCN | Thu nhập cá nhân |
| Hệ số khoán cơ bản | 1.0 | Ratio | LƯƠNG ĐỘI | Tính khoán | Tương đương lương cơ bản |
| | | | | | |
| | | | | | |
| | | | | | |
| | | | | | |
| | | | | | |
| | | | | | |
| | | | | | |

---

## ⚠️ STEP 3: PHÁT HIỆN Ô RỦI RO

### Các Loại Rủi Ro Cần Tìm

#### A. #REF! Error

Nguyên nhân: Công thức tham chiếu đến ô/sheet đã bị xóa

**Cách tìm:**
- Ctrl+H, tìm `#REF!`
- Hoặc nhìn các ô màu đỏ/lỗi

**Bảng ghi nhận:**

| Sheet | Ô | Công Thức | Lý Do | Giải Pháp |
|---|---|---|---|---|
| CÁCH TÍNH LƯƠNG | F15 | =INDIRECT(...) | Tham chiếu sheet xóa | Kiểm tra lại hoặc xóa |
| | | | | |
| | | | | |

#### B. Link Ngoài (External Link)

Nguyên nhân: Công thức tham chiếu đến file Excel khác

**Cách tìm:**
- Ctrl+F, tìm `[`
- Hoặc tìm `.xlsx]`
- Hoặc Edit > Links (trong Excel)

**Bảng ghi nhận:**

| Sheet | Ô | Link | Để Nguyên Hay Xóa? | Ghi Chú |
|---|---|---|---|---|
| LƯƠNG ĐỘI | B5 | [LuongThanh10.xlsx]Sheet1!$A$1 | ⚠️ Review | File cũ hay mới? |
| | | | | |
| | | | | |

#### C. Ô Điều Chỉnh Tay (Manual Input)

Nguyên nhân: Giá trị nhập thủ công, không có công thức

**Cách nhận biết:**
- Ô không bắt đầu bằng `=`
- Ô chứa số cố định (không liên kết)
- Thường được ghi chú rõ ràng

**Bảng ghi nhận:**

| Sheet | Ô | Giá Trị | Lý Do Điều Chỉnh | Người Chỉnh | Ngày |
|---|---|---|---|---|---|
| TRẠM 1 | D20 | 150000 | Phí cấp phát | Anh Sơn | 05/12 |
| BẢNG PHÂN TOÁN | C10 | 500000 | Điều chỉnh DRC | Chị Hạ | 08/12 |
| | | | | | |
| | | | | | |

#### D. Ô Tham Số Tài Chính & DRC

Nguyên nhân: Chứa hệ số chi phối tính toán (cần giám sát thay đổi)

**Bảng ghi nhận:**

| Sheet | Cell Range | Tên Tham Số | Giá Trị | Tác Động | Mức Độ |
|---|---|---|---|---|---|
| LƯƠNG ĐỘI | E1 | Hệ số tăng lương | 1.2 | Tổng lương tăng 20% | 🔴 CAO |
| CÁCH TÍNH LƯƠNG | E2 | Hệ số bảo hiểm | 0.105 | Net salary | 🟡 TRUNG |
| BẢNG PHÂN TOÁN | C5 | Hệ số DRC | 0.85 | Phân toán chi phí | 🟡 TRUNG |
| | | | | | |

---

## 📈 STEP 4: LẬP BÁO CÁO CHI TIẾT

Sau khi hoàn thành 3 bước trên, điền vào bảng báo cáo tổng hợp (nếu cần):

| Ô / Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ |
|---|---|---|---|---|
| B5:B20 | Manual input | - | Count > 0 | Danh sách nhân viên Đội 1 |
| C5:C20 | TRẠM 1!D10 | ×1.0 | C >= 0 | Lương cơ bản từ TRẠM 1 |
| D5:D20 | =C5*$E$1 | E1 (1.2) | D >= C | Lương khoán = cơ bản × hệ số |
| E5:E20 | =D5*$E$2 | E2 (0.105) | 0 <= E <= D×0.15 | Bảo hiểm = khoán × 10.5% |
| F5:F20 | =D5-E5 | - | F = D - E | Lương net (sau khấu trừ) |
| G5:G20 | =F5*$E$3 | E3 (0.05) | G <= F | Thuế TNCN = net × 5% |
| | | | | |

---

## 📝 DANH SÁCH KIỂM TRA HOÀN THÀNH

- [ ] **Bước 1:** Đã điền từ điển cột cho tất cả 5 sheets
- [ ] **Bước 2:** Đã lập bảng quy chế tham số với mọi hệ số
- [ ] **Bước 3:** Đã tạo bảng ô rủi ro đầy đủ
- [ ] **Bước 4:** (Nếu cần) Đã lập báo cáo chi tiết

---

## 💾 LƯU LẠI KẾT QUẢ

Tạo file mới tên:
```
PHAN_TICH_LUONG_DOI_1_THANG_12_2025.xlsx
```

Chứa các sheets:
1. **Từ Điển Cột** - Liệt kê tất cả headers
2. **Quy Chế Tham Số** - Hệ số ngầm
3. **Ô Rủi Ro** - Cảnh báo lỗi & điều chỉnh
4. **Báo Cáo Chi Tiết** - (Tuỳ chọn)
5. **Ghi Chú** - Tham khảo & hướng dẫn

---

**Ngày tạo:** 2025-04-09  
**Người tạo:** AI Analysis Tools

