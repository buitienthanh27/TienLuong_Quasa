# 🧮 QUY CHẾ THAM SỐ - HỆ SỐ NGẦM

## ⚠️ CẢNH BÁO QUAN TRỌNG

**File này sử dụng HỆ SỐ NGẦM từ nhiều vị trí khác nhau:**
- Các hệ số được nhập trong CÁCH TÍNH LƯƠNG
- Các hệ số trong BẢNG PHÂN TOÁN
- Các công thức nhân trong LƯƠNG ĐỘI

**Nhiều hệ số hiện không rõ ràng - cần GHI CHÚ để tránh sai sót**

---

## 📊 DANH SÁCH HỆ SỐ NGẦM PHÁT HIỆN

### Loại A: Hệ Số BẢNG LƯƠNG CỦA NHÀ NƯỚC (100% cần kiểm tra)

| Tên Hệ Số | Giá Trị Dự Kiến | Sheet Đang Dùng | Khoản Mục Chịu Tác Động | Link/Công Thức | Ghi Chú |
|---|---|---|---|---|---|
| Hệ số bảo hiểm xã hội | 0.08 (8%) | CÁCH TÍNH LƯƠNG | Khấu trừ BHXH | ? | Theo quy định Nhà nước |
| Hệ số bảo hiểm y tế | 0.025 (2.5%) | CÁCH TÍNH LƯƠNG | Khấu trừ BHYT | ? | Theo quy định Nhà nước |
| Hệ số bảo hiểm thất nghiệp | 0.0 (0%) | CÁCH TÍNH LƯƠNG | Khấu trừ bảo hiểm thất nghiệp | ? | Có thể áp dụng hoặc không |
| Hệ số thuế Thu nhập cá nhân | 0.05 (5%) hoặc 0-35% | CÁCH TÍNH LƯƠNG | Khấu trừ thuế TNCN | ? | Tùy theo khung lương |
| (Cộng hạng mục > 9,000,000) | Áp dụng cumulative | CÁCH TÍNH LƯƠNG | Tính thuế lũy tiến | ? | Nếu lương > threshold |

### Loại B: Hệ Số NỘI BỘ CÔNG TY

| Tên Hệ Số | Giá Trị Dự Kiến | Sheet Dùng | Mục Đích | Link/Công Thức | Ghi Chú |
|---|---|---|---|---|---|
| Hệ số khoán Cơ bản | 1.0 | LƯƠNG ĐỘI | Tính lương cơ bản | Cell: ? | Base salary multiplier |
| Hệ số tăng lương (NCS) | 1.1-1.3 | LƯƠNG ĐỘI | Phụ cấp nhân công suất | Cell: ? | Phụ cấp hiệu suất cao |
| Hệ số phụ cấp trách nhiệm | 0.1-0.5 | TRẠM 1/TRẠM 2 | Phụ cấp chức vụ | Cell: ? | Tuỳ vị trí công việc |
| Hệ số phụ cấp nguy hiểm/độc hại | 0.05-0.1 | TRẠM 1/TRẠM 2 | Phụ cấp công việc nặng | Cell: ? | Tuỳ cung cấp dịch vụ |

### Loại C: Hệ Số PHÂN TOÁN CHI PHÍ

| Tên Hệ Số | Giá Trị | Sheet Dùng | Mục Đích | Công Thức | Ghi Chú |
|---|---|---|---|---|---|
| Tỷ lệ phân toán chi phí chung | 0.05-0.1 (5-10%) | BẢNG PHÂN TOÁN | Phân toán overhead | ? | Tuỳ từng kỳ |
| Tỷ lệ phân toán lợi nhuận | 0.1-0.2 (10-20%) | BẢNG PHÂN TOÁN | Phân toán profit | ? | Tuỳ kế hoạch |
| Hệ số DRC (Direct Resource Cost) | ? | BẢNG PHÂN TOÁN | Phân toán resource | Cell: ? | **CẦN KIỂM TRA** |

### Loại D: Hệ Số ĐẶC BIỆT/ĐIỀU CHỈNH

| Tên Hệ Số | Giá Trị Hiện Tại | Sheet | Mục Đích | Tác Động | Ghi Chú |
|---|---|---|---|---|---|
| Hệ số điều chỉnh thị trường | ? | ? | Điều chỉnh thị trường lao động | ? | **CẦN XÁC ĐỊNH** |
| Hệ số lương eo hẹp | ? | ? | Điều chỉnh cho vị trí ít tuyển | ? | **CẦN XÁC ĐỊNH** |
| (Hệ số khác...) | ? | ? | ? | ? | **CẦN KIỂM TRA** |

---

## 🔍 CÁCH TÌM HỆ SỐ ẨN

### Phương pháp 1: Tìm trong CÁCH TÍNH LƯƠNG Sheet
```
1. Mở Sheet "CÁCH TÍNH LƯƠNG"
2. Tìm kiếm các ô chứa:
   - Số thập phân nhỏ hơn 1 (0.08, 0.105, 0.85, v.v.)
   - Phần trăm (10%, 5%, etc.)
   - Công thức SUM/VLOOKUP
3. Ghi chú lại vị trí ô (Column letter + Row number)
```

### Phương pháp 2: Tìm ô được khóa ($)
```
Excel Find & Replace (Ctrl+H):
- Find: \$[A-Z]+\$[0-9]+
- Specify: Formulas
- Regular expression: ON
- Find All
→ Tất cả hệ số được khóa sẽ được liệt kê
```

### Phương pháp 3: Tìm từ LƯƠNG ĐỘI Sheet
```
1. Mở Sheet "LƯƠNG ĐỘI"
2. Chọn cột có công thức (thường là Column D, E, F,...)
3. Nhấp vào ô công thức
4. Xem công thức:
   - Nếu có $X$Y → Hệ số tại X:Y
   - Nếu có *0.105 → Hệ số là 0.105
   - Thêm vào bảng
```

---

## 📋 BẢNG TÓM TẮT - NHỮNG HỆ SỐ CẦN XÁC ĐỊNH

| Mục | Hệ Số | Giá Trị | Link | Status |
|---|---|---|---|---|
| 1 | Hệ số khoán 27 ngày | 360/27*22 = 292.59 | CÁCH TÍNH LƯƠNG!O11 | ✅ **Found** |
| 2 | Hệ số chia 500 | ÷500 | CÁCH TÍNH LƯƠNG!V10 | ✅ **Found** |
| 3 | Hệ số tỷ lệ lương | J11 = I11/H11 | CÁCH TÍNH LƯƠNG!J11 | ✅ **Found** |
| 4 | Hệ số từ sản lượng | AE6 = AE8/AE7 | TRẠM 1!AE6 | ✅ **Found** |
| 5 | Bảo hiểm xã hội | ? | LƯƠNG ĐỘI | ⏳ **Check thuế lũy tiến** |
| 6 | Thuế TNCN (Mức 1) | 25% (>65M) | LƯƠNG ĐỘI!J13 | ✅ **Found** |
| 7 | Thuế TNCN (Mức 2) | 20% (>25M) | LƯƠNG ĐỘI!J13 | ✅ **Found** |
| 8 | Thuế TNCN (Mức 3) | Mức thấp hơn | LƯƠNG ĐỘI!J13 | ✅ **Found** |
| 9 | Bản lương từ VLOOKUP | CÁCH TÍNH LƯƠNG Sheet | LƯƠNG ĐỘI!F13 | ✅ **Found** |
| 10 | Hệ số phân toán | K10 = percentage | BẢNG PHÂN TOÁN!K10 | ✅ **Found** |

---

## ⚠️ HỆ SỐ CÓ RỦI RO CAO

### #REF! Errors liên quan đến hệ số:

Các công thức sau đang lỗi - **cần sửa ngay**:

```
Sheet "CÁCH TÍNH LƯƠNG":
- AH103: =+AH102-'BẢNG PHÂN TOÁN'!#REF!    ← Tham chiếu lỗi
- AH27: =+AH26-'BẢNG PHÂN TOÁN'!#REF!      ← Tham chiếu lỗi
- AH23: =+AH22-'BẢNG PHÂN TOÁN'!#REF!      ← Tham chiếu lỗi

Sheet "PLC (2)":
- 100+ cells: =VLOOKUP(B#,#REF!,X,0)       ← Múi VLOOKUP lỗi
```

**Giải pháp:** Sửa các công thức trên trước khi xác định hệ số

---

## 🎯 BƯỚC TIẾP THEO

1. ✅ Mở file trong Excel
2. ✅ Navigate đến CÁCH TÍNH LƯƠNG
3. ✅ Ghi chú lại tất cả hệ số
4. ✅ Cập nhật bảng trên
5. ✅ Xác định hệ số DRC từ BẢNG PHÂN TOÁN
6. ✅ Lập báo cáo với format: "Ô/Cụm ô | Nguồn | Hệ số | Ràng buộc | Giải thích"

---

**Ngày tạo:** 09/04/2025  
**Status:** ⏳ Pending - Cần điền thủ công  
**Người phụ trách:** Kiểm toán viên

