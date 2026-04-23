# 📋 TỪ ĐIỂN CỘT - LƯƠNG ĐỘI 1 THÁNG 12

## Sheet 20: TRẠM 1 (113 hàng × 34 cột)

### Phân Tích Cấu Trúc

| Cột | Header | Kiểu Dữ Liệu | Công Thức/Ghi Chú |
|---|---|---|---|
| A | ? | ? | (Cần check) |
| B | CỘNG HÒA DÂN CHỦ NHÂN DÂN LÀO | Title/Header | Tiêu đề sheet |
| C | (MSNV?) | Number/Text | Mã số nhân viên → Dùng VLOOKUP đến 'MSNV 2025' |
| D | ? | ? | (Cần check) |
| E | (Hệ số?) | Number | Có reference đến TRẠM 2: `=TRẠM 2!E8` |
| F | (Tính toán?) | Number | Công thức: `=G16*25000` |
| G | (Lương?) | Number | Công thức: `=F16*25000` |
| H | (Sản lượng?) | Number | VLOOKUP từ 'sản lượng'!$D$13:$AM$113 |
| J | (Khoán?) | Number | Công thức: `=H16*$AE$6` |
| K | (Tính toán?) | Number | Công thức: `=+H16` |
| ... | ... | ... | ... |
| AE | **Hệ số chung** | Formula | References: `='sản lượng'!AT7`, `='sản lượng'!AU10`, etc. |

### Key Formulas
- **Row 16+:** VLOOKUP từ MSNV 2025
- **Column AE:** Hệ số tính từ 'sản lượng' sheet (`=AE8/AE7`)
- **VLOOKUP Pattern:** `=VLOOKUP(C16,'sheet'!$range,col,0)`

### Cảnh Báo
- ⚠️ Cell AE chứa hệ số quan trọng
- ⚠️ Multisheet references → cần giám sát

---

## Sheet 29: TRẠM 2 (128 hàng × 34 cột)

| Cột | Header | Kiểu Dữ Liệu | Ghi Chú |
|---|---|---|---|
| A | (Chưa phân tích) | ? | Cần kiểm tra |
| B | (Chưa phân tích) | ? | Cần kiểm tra |
| C | (Chưa phân tích) | ? | Cần kiểm tra |
| ... | ... | ... | ... |
| AD | (Formula) | Formula | External link/Reference |

**Note:** Sheet này có 34 cột như TRẠM 1 (likelihood có cấu trúc tương tự).

---

## Sheet 32: LƯƠNG ĐỘI (69 hàng × 20 cột)

### Phân Tích Cấu Trúc

| Cột | Header | Kiểu Dữ Liệu | Công Thức/Công Dụng |
|---|---|---|---|
| A | CỘNG HÒA DÂN CHỦ NHÂN DÂN LÀO | Title | Tiêu đề |
| B | (STT?) | Number | Số thứ tự |
| C | (MSNV?) | Text | Mã số nhân viên |
| D | (Tên?) | Text | Tên nhân viên |
| E | (Lương cơ bản?) | Number | `=SUM(E13)` - Tổng tiền |
| F | **Lương từ CÁCH TÍNH LƯƠNG** | Number | `=VLOOKUP(C13,'CÁCH TÍNH LƯƠNG'!$C$11:$S$18,17,0)` |
| H | (Công?) | Number | Số công làm việc |
| I | **Lương tính toán** | Number | `=(F13+H13)/P7*E13` - Formula phức tạp |
| J | **Thuế TNCN** (tính lũy tiến) | Number | `=IF(I13>65M,..25%+10.685M,IF(I13>25M,..20%+2.685M,..))` |
| K | (Khác?) | Number | (Cần check) |
| P | (Tham số?) | Number | P7 - Tham số tính toán (dùng trong công thức I) |
| ... | ... | ... | ... |

### Key Formulas
```
E12: =SUM(E13)              ← Tổng tiền cơ bản
F13: =VLOOKUP(C13,'CÁCH TÍNH LƯƠNG'!$C$11:$S$18,17,0)  ← Lương từ sheet khác
I13: =(F13+H13)/P7*E13      ← Lương tính = (lương + công) / param × cơ bản
J13: IF statement (lũy tiến thuế)   ← Tính thuế với 3 mức khác nhau
```

### Cảnh Báo
- ⚠️ Column J có logic thuế lũy tiến (3 mức)
- ⚠️ Column F liên kết với CÁCH TÍNH LƯƠNG
- ⚠️ Column P7 là tham số quan trọng (cần giám sát)

---

## Sheet 30: CÁCH TÍNH LƯƠNG (43 hàng × 26 cột)

### Phân Tích Cấu Trúc

| Cột | Header | Kiểu Dữ Liệu | Công Thức/Công Dụng |
|---|---|---|---|
| A | ? | ? | (Cần check) |
| B | CỘNG HÒA DÂN CHỦ NHÂN DÂN LÀO | Title | Tiêu đề |
| C | (Tính năng?) | Text | Tên hạng/loại |
| H | (Tham số?) | Number | Denominator |
| I | (Tính toán?) | Number | `=I12+I13` |
| J | (Tỷ lệ?) | Number | `=I11/H11` |
| O | (Khoán?) | Number | `=360/27*22` = 292.594... (hệ số khoán) |
| Q | (Lương?) | Number | `=(300*J11*G11)/27*22` |
| R | (Tham số?) | Number | `=+SUM(...)` |
| S | (Tổng?) | Number | `=O11+Q11+P11+R11` |
| V | (Chia?) | Number | `=+V9/500` (hệ số chia) |
| ... | ... | ... | ... |

### Key Formulas
```
Row 10: SUM formulas (totals)
Row 11: Calculation formulas
O11: 360/27*22 = Base coefficient
V10: =+V9/500  = Division coefficient
```

### Cảnh Báo
- ⚠️ Sheet này chứa #REF! errors trong một số ô
- ⚠️ Chứa hệ số cơ bản và công thức tính
- ⚠️ Cần verify công thức sau khi sửa lỗi

---

## Sheet 35: BẢNG PHÂN TOÁN (79 hàng × 14 cột)

### Phân Tích Cấu Trúc

| Cột | Header | Kiểu Dữ Liệu | Công Thức/Công Dụng |
|---|---|---|---|
| A | (Loại chi phí?) | Text | Item name |
| B | (Số tiền?) | Number | Amount |
| E | (Phân toán?) | Number | `=SUM(E16:E19,E35,E20,E25,E38,E30)` |
| G | (Chi phí?) | Number | `=SUM(G16:G19,G35,G20,G25,G38,G30)` |
| J | **#REF! Error** ⚠️ | Formula | `=#REF!` - LỖI |
| K | (Tỷ lệ?) | Number | `=K10/100` (percentage) |
| ... | ... | ... | ... |

### Key Formulas
```
J9: =#REF!              ← ERROR - Cần sửa
J10: =K10/100           ← Tính percentage
K12: =+THCKT!Z10        ← Reference từ sheet THCKT
K13: =+E15-K12          ← Phân toán
I14: =#REF!             ← ERROR - Cần sửa
```

### Cảnh Báo
- 🔴 **#REF! Errors** trong J9 và I14
- ⚠️ Các tổng SUM phức tạp
- ⚠️ Reference đến sheet THCKT có thể bị lỗi

---

## 🚨 HƯỚNG DẪN ĐIỀN ĐẦY ĐỦ TỪ ĐIỂN CỘT

1. **Mở file** `LƯƠNG ĐỘI 1 THÁNG 12.2025 2 BẢN.xlsx` trong Excel
2. **Navgate** đến Sheet 20 (TRẠM 1)
3. **Chọn Row 1** (toàn bộ headers)
4. **Copy** tất cả nội dung
5. **Paste** vào bảng template trên, cột Header
6. **Lặp lại** với các sheets khác: 29, 30, 32, 35, 38

---

**Status:** ⏳ Cần điền thủ công bằng Excel  
**Độ ưu tiên:** MEDIUM

