# ⚠️ DANH SÁCH Ô RỦI RO - LƯƠNG ĐỘI 1 THÁNG 12

## 🎯 TÓMSÁTQUÁT RỦI RO

| Loại Rủi Ro | Số Lượng | Mức Độ | Giải Pháp |
|---|---|---|---|
| **#REF! Errors** | 100+ | 🔴 **CAO** | Sửa công thức/xóa link ngoài |
| **External Links** | 10+ | 🟠 **TRUNG** | Break links hoặc copy values |
| **Manual Adjustments** | ? | 🟡 **THẤP** | Ghi chú & giám sát |
| **Financial Parameters** | ? | 🟡 **THẤP** | Tạo bảng tham chiếu |

---

## 🔴 RỦI RO CAO: #REF! ERRORS

### Sheet: "PLC (2)" - 100+ #REF! Errors

**Nguyên nhân:** Các công thức VLOOKUP tham chiếu đến sheet không tồn tại (marked as #REF!)

**Danh sách 30 ô lỗi đầu tiên:**

| Sheet | Ô | Công Thức | Severity | Giải Pháp |
|---|---|---|---|---|
| PLC (2) | C4 | =VLOOKUP(B3,#REF!,3,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | I4 | =VLOOKUP(H3,#REF!,3,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | C5 | =VLOOKUP(B3,#REF!,14,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | I5 | =VLOOKUP(H3,#REF!,14,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | C6 | =VLOOKUP(B3,#REF!,15,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | I6 | =VLOOKUP(H3,#REF!,15,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | C7 | =VLOOKUP(B3,#REF!,16,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | I7 | =VLOOKUP(H3,#REF!,16,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | C8 | =VLOOKUP(B4,#REF!,16,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| PLC (2) | I8 | =VLOOKUP(H4,#REF!,16,0) | 🔴 CAO | Tìm đúng sheet + VLOOKUP range |
| ... | ... | ... | ... | ... |
| PLC (2) | (Total) | 100+ VLOOKUP lỗi | 🔴 CAO | **Cần sửa toàn bộ** |

**Hành động ngay lập tức:**
1. ❌ **Không sử dụng** Sheet "PLC (2)" cho tính toán lương
2. ✅ **Tìm** sheet gốc được tham chiếu
3. ✅ **Thay thế** công thức hoặc
4. ✅ **Copy data** vào workbook hiện tại

---

### Sheet: "CÁCH TÍNH LƯƠNG" - Multiple #REF! Errors

| Ô | Công Thức | Lỗi | Giải Pháp |
|---|---|---|---|
| AH103 | =+AH102-'BẢNG PHÂN TOÁN'!#REF! | Tham chiếu `BẢNG PHÂN TOÁN` lỗi | Sửa lại #REF! trong BẢNG PHÂN TOÁN |
| AH27 | =+AH26-'BẢNG PHÂN TOÁN'!#REF! | Tham chiếu `BẢNG PHÂN TOÁN` lỗi | Sửa lại #REF! trong BẢNG PHÂN TOÁN |
| AH23 | =+AH22-'BẢNG PHÂN TOÁN'!#REF! | Tham chiếu `BẢNG PHÂN TOÁN` lỗi | Sửa lại #REF! trong BẢNG PHÂN TOÁN |

**Ghi chú:** Tất cả lỗi AH trong sheet này du tham chiếu đến `BẢNG PHÂN TOÁN` → Cần sửa BẢNG PHÂN TOÁN trước!

---

### Tìm kiếm #REF! Error trong toàn workbook

**Cách thực hiện:**
```
1. Ctrl+H (Find & Replace)
2. Find: #REF!
3. Click "Find All"
4. Excel sẽ liệt kê tất cả ô chứa #REF!
```

**Kết quả dự kiến:** 100+ cells

---

## 🟠 RỦI RO TRUNG: EXTERNAL LINKS

### Links Phát Hiện
| Sheet | Ô | Link Đề Tham Chiếu | Link Target | Severity |
|---|---|---|---|---|
| TRẠM 1 | AE57 | =+'[7]TRẠM 1'!$AA$60 | File không xác định [7] | 🟠 TRUNG |
| TRẠM 1 | AE58 | =+'[7]TRẠM 2'!$AA$75 | File không xác định [7] | 🟠 TRUNG |
| TRẠM 2 | AD76 | =+'[2]TRẠM 2'!$AA$75 | File không xác định [2] | 🟠 TRUNG |
| TRẠM 2 | AD79 | ='[2]TRẠM 2'!$O$75 | File không xác định [2] | 🟠 TRUNG |
| TRẠM 2 | AD3 | ='TRẠM 1'!AD4 | **OK - Trong workbook** | ✅ SAFE |

**Cảnh báo:** `[7]` và `[2]` chỉ file Excel khác đã bị xóa hoặc di chuyển!

### Giải Pháp

**Tùy chọn 1: Break Links (Nhanh nhất)**
```
Edit > Links > Break Link
→ Sẽ convert công thức thành values
→ Dữ liệu không cập nhật, nhưng ổn định
```

**Tùy chọn 2: Tìm file gốc & Re-link**
```
1. Tìm file trở về từng [2], [7]
2. Edit > Links > Edit Link
3. Browse đến file đúng
4. Cập nhật link
```

**Tùy chọn 3: Copy Data Vào (Khuyến cáo)**
```
1. Mở file gốc
2. Copy dữ liệu cần thiết
3. Paste vào workbook hiện tại
4. Chỉnh sửa công thức để thay vì link, sử dụng direct reference
5. Delete các link cũ
```

---

## 🟡 RỦI RO THẤP: MANUAL ADJUSTMENTS

### Cách Phát Hiện
```
1. Mở từng Sheet
2. Nhấp vào ô
3. Nếu ô chứa Con số cố định (không có dấu =) → Manual adjustment
4. Ghi chú lại
```

### Ví Dụ Dự Kiến

| Sheet | Ô | Giá Trị | Người Điều Chỉnh | Ngày | Lý Do | Ghi Chú |
|---|---|---|---|---|---|---|
| TRẠM 1 | D50 | 500000 | Chị Hạ | 08/12 | Phí CPP |                                           | Cần xác nhân |
| LƯƠNG ĐỘI | B5 | 30000000 | Anh Sơn | 01/12 | Lương cơ bản tăng | Cần approval |
| BẢNG PHÂN TOÁN | C10 | 0.85 | Quản lý | 01/12 | Hệ số DRC | Giám sát hàng kỳ |

**Khuyến cáo:**
- ✅ Ghi chú **người thực hiện** & **ngày** thay đổi
- ✅ Giải thích **lý do** điều chỉnh
- ✅ **Audit trail** (để kiến toàn viên kiểm tra)

---

## 🟡 RỦI RO THẤP: FINANCIAL PARAMETERS & DRC

### Cách Xác Định

**Financial Parameters:**
```
Những ô được tham chiếu nhiều lần (=$E$1, =$F$5, etc.)
- Thường là hệ số bảo hiểm, thuế, phần trăm phụ cấp
- Cần ghi chú & giám sát thay đổi
```

**DRC (Direct Resource Cost):**
```
Thường nằm trong BẢNG PHÂN TOÁN
- Hệ số phân toán chi phí trực tiếp
- Giá trị: thường 0.5-1.0
- Cần approval từ quản lý trước khi thay đổi
```

### Bảng Ghi Chú

| Parameter | Sheet | Ô | Giá Trị | Tần Suất Kiểm | Người Phụ Trách |
|---|---|---|---|---|---|
| Bảo hiểm | CÁCH TÍNH LƯƠNG | ? | ? | Hàng năm | HR |
| Thuế TNCN | CÁCH TÍNH LƯƠNG | ? | ? | Hàng năm | Kế toán |
| Hệ số khoán | LƯƠNG ĐỘI | ? | ? | Hàng quý | Quản lý |
| Hệ số DRC | BẢNG PHÂN TOÁN | ? | ? | Hàng kỳ | Quản lý chi phí |

---

## 📋 KIỂM SACTONG HỢP

### Priority 1: FIX IMMEDIATELY (Tuần này)
- [ ] Tìm nguyên nhân #REF! errors trong PLC (2)
  - [ ] Liệu sheet gốc đang tồn tại?
  - [ ] Hoặc sheet đã bị xóa/rename?
  - [ ] Hoặc link bị break?

- [ ] Tìm và fix #REF! trong CÁCH TÍNH LƯƠNG
  - [ ] Sửa lại BẢNG PHÂN TOÁN
  - [ ] Update công thức AH23, AH27, AH103

- [ ] Break hoặc Fix external links
  - [ ] AE57, AE58 (TRẠM 1)
  - [ ] AD76, AD79 (TRẠM 2)

### Priority 2: DOCUMENT & MONITOR (Tuần này - Tuần sau)
- [ ] Ghi lại tất cả manual adjustments
- [ ] Tạo bảng financial parameters
- [ ] Đánh dấu các ô DRC

### Priority 3: AUDIT (Tháng sau)
- [ ] Kiểm toán toàn bộ công thức
- [ ] Verify hệ số với quy định
- [ ] Update báo cáo chi tiết

---

## 📎 DANH SÁCH HỌC DÀI

**Phần 1 - #REF! Errors (Ưu tiên NGAY):**
- Tìm file source của VLOOKUP
- Hoặc import dữ liệu vào workbook
- Hoặc thay thế công thức

**Phần 2 - External Links (Ưu tiên CAO):**
- Break link hoặc copy values
- Hoặc find & relink files

**Phần 3 - Manual & Parameters (Ưu tiên TRUNG):**
- Ghi chú & giám sát

---

**Ngày tạo:** 09/04/2025  
**Status:** ⚠️ **URGENT - CẢNH BÁO RỦI RO CAO**  
**Người review:** Kiểm toán viên / Quản lý tài chính

