# 📊 BÁO CÁO CHI TIẾT - FORMAT CHUYÊN SÂU

## Ô/Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ

### **SECTION A: LƯƠNG CƠ BẢN & CÔNG SUẤT (TRẠM 1 Sheet)**

| Ô/Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ |
|---|---|---|---|---|
| C16+ | MSNV 2025 | - | C > 0 | Mã số nhân viên - VLOOKUP từ MSNV 2025 sheet |
| G16+ | F16 × 25000 | 25000 (fixed) | G = F × 25000 | Tính toán từ hệ số × 25000đ |
| H16+ | 'sản lượng'!D13:AM113 | - | H >= 0 | Sản lượng từ sheet sản lượng, VLOOKUP column 35 |
| J16+ | H16 × **$AE$6** | AE6 = hệ số sản lượng | J > 0 | Tính công = Sản lượng × Hệ số (tham chiếu $AE$6) |
| K16+ | H16 | - | K = H | Copy sản lượng gốc |
| AE1:AE10 | 'sản lượng'!AT7, AU10, AV10 | - | AE > 0 | **HỆ SỐ CHÍNH** - Tính từ sản lượng áp dụng cho cột J |

### **SECTION B: LƯƠNG ĐỘI - TÍNH TOÁN LƯƠNG CUỐI CÙNG**

| Ô/Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ |
|---|---|---|---|---|
| E12:E69 | =SUM(E13) | - | E >= 0 | **Tổng lương cơ bản** |
| F13:F69 | VLOOKUP từ 'CÁCH TÍNH LƯƠNG'!$C$11:$S$18, col 17 | - | F >= 0 | Bản lương từ bảng công khai (CÁCH TÍNH LƯƠNG) |
| H13:H69 | 'CC NGƯỜI LÀO'!$C$12:$AJ$18, col 34 | - | H >= 0 | Số công từ bảng chấm công |
| **I13:I69** | **(F13+H13)/P7 × E13** | **P7** (tham số) | 0 < I | **LƯƠNG TÍNH** = (Bản + Công)/Tham_số × Cơ_bản |
| **J13:J69** | **Lũy tiến thuế** | 3 mức khác nhau | J >= 0 | **THUẾ TNCN:** IF(I>65M, (I-65M)×25%+10.685M, IF(I>25M, (I-25M)×20%+2.685M, ...)) |
| P7 | Manual input | **KEY PARAM** | 0 < P7 < 100 | **⚠️ THAM SỐ QUAN TRỌNG** - Chia động để điều chỉnh tổng lương |

### **SECTION C: CÁCH TÍNH LƯƠNG - CÁC HỆ SỐ CÔNG KHAI**

| Ô/Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ |
|---|---|---|---|---|
| O11 | 360 / 27 × 22 | = 292.59 (fixed) | O11 > 0 | **HỆ SỐ KHOÁN** - Tính từ 360 ngày / 27 × 22 ngày làm việc |
| V10 | V9 / 500 | ÷ 500 | V10 < 1 | Hệ số chia (khoán chia 500 để lấy đơn vị nhỏ) |
| J11 | I11 / H11 | - | J11 > 0 | Tỷ lệ lương = Tính_toán / Denominator |
| Row 11 | Các SUM formulas | - | > 0 | Hàng công thức tính toán cơ bản |
| Row 10 | Các SUM(11:16) | - | > 0 | Hàng tổng hợp các hạng mục |

### **SECTION D: TRẠM 2 - TƯƠNG TỰ TRẠM 1**

| Ô/Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ |
|---|---|---|---|---|
| AD3 | = 'TRẠM 1'!AD4 | - | AD3 > 0 | **Cross-reference** giữa TRẠM 1 & TRẠM 2 |
| E8 | = 'TRẠM 1'!E8 | - | E8 > 0 | Cross-reference hệ số từ TRẠM 1 |
| B16+, G16+, H16+ | (Tương tự TRẠM 1) | (Tương tự) | (Tương tự) | Cấu trúc giống TRẠM 1 |
| AE5:AE9 | 'sản lượng' sheet refs | - | > 0 | Hệ số từ sheet sản lượng cho TRẠM 2 |

### **SECTION E: BẢNG PHÂN TOÁN - PHÂN TOÁN CHI PHÍ**

| Ô/Cụm Ô | Nguồn Dữ Liệu | Hệ Số Chi Phối | Ràng Buộc | Giải Thích Nghiệp Vụ |
|---|---|---|---|---|
| E15 | =SUM(E16:E19, E35, E20, E25, E38, E30) | - | E15 > 0 | **Tổng chi phí phân toán** (từ nhiều hạng mục không liên tục) |
| G15 | =SUM(G16:G19, G35, G20, G25, G38, G30) | - | G15 > 0 | **Tổng chi phí khác** (cấu trúc tương tự E15) |
| J10 | =#REF! ⚠️ | N/A | **ERROR** | **LỖI - CẦN SỬA** |
| K10 | =K10/100 | ÷ 100 | 0 < K10 < 1 | **Tỷ lệ phân toán** (chuyển % sang decimal) |
| K12 | = +THCKT!Z10 | - | K12 > 0 | Reference từ sheet THCKT (chi phí khác) |
| K13 | = +E15 - K12 | - | K13 > 0 | **Phân toán thực** = Tổng - Chi_khác |
| I14 | =#REF! ⚠️ | N/A | **ERROR** | **LỖI - CẦN SỬA** |
| J14 | = +G14 - I14 | - | J14 > 0 | Phân toán G = G14 - I14 |

---

## 📌 DANH SÁCH CÁC THAM SỐ QUAN TRỌNG (GOLDEN CELLS)

### 🔴 THAM SỐ CẦN GIÁM SÁT (Priority List)

| Tham Số | Ô | Giá Trị | Tác Động | Yêu Cầu Approval |
|---|---|---|---|---|
| **P7** | LƯƠNG ĐỘI!P7 | ? | Điều chỉnh tổng lương ÷P7 | 🔴 CAO - Quản lý phê duyệt |
| **AE6** | TRẠM 1!AE6 | = AE8/AE7 | Hệ số công sản lượng | 🟠 TRUNG - Tự động tính |
| **O11** | CÁCH TÍNH LƯƠNG!O11 | 292.59 | Hệ số khoán cơ bản | 🟡 THẤP - Fixed |
| **K10** | BẢNG PHÂN TOÁN!K10 | ? | Tỷ lệ phân toán | 🟠 TRUNG - Hàng kỳ |

### 🟡 HỆ SỐ THUẾ (TAX TABLE)

| Mức | Giới Hạn Dưới | Giới Hạn Trên | Thuế Suất | Cộng Dồn | Áp Dụng Ở |
|---|---|---|---|---|---|
| 1 | 0 | 5,000,000đ | 5% | - | LƯƠNG ĐỘI!J13 |
| 2 | 5,000,000đ | 10,000,000đ | 10% | + 250,000đ | LƯƠNG ĐỘI!J13 |
| 3 | 10,000,000đ | 20,000,000đ | 15% | + 750,000đ | LƯƠNG ĐỘI!J13 |
| 4 | 20,000,000đ | 25,000,000đ | 20% | + 2,250,000đ | LƯƠNG ĐỘI!J13 |
| 5 | 25,000,000đ | 65,000,000đ | 25% | + 2,685,000đ | LƯƠNG ĐỘI!J13 |
| 6 | > 65,000,000đ | ∞ | 35% | + 10,685,000đ | LƯƠNG ĐỘI!J13 |

**Ghi chú:** Trong IF statement của J13, chỉ test 3 mức (65M, 25M, khác)

---

## ⚠️ PHÁT HIỆN LỖI & RỦI RO

### 🔴 ERROR - CẦN SỬA NGAY

| Vị Trí | Lỗi | Hậu Quả | Giải Pháp |
|---|---|---|---|
| BẢNG PHÂN TOÁN!J9 | =#REF! | Không tính được phân toán | Tìm cell/sheet gốc |
| BẢNG PHÂN TOÁN!I14 | =#REF! | Lỗi phân toán G | Tìm cell/sheet gốc |
| CÁCH TÍNH LƯƠNG (multiple) | =#REF!-'BẢNG PHÂN TOÁN'!#REF! | Cascade error | Sửa BẢNG PHÂN TOÁN trước |

### 🟠 WARNING - EXTERNAL LINKS

| Vị Trí | Link | Nguy Hiểm | Giải Pháp |
|---|---|---|---|
| TRẠM 1!AE57 | ='[7]TRẠM 1'!$AA$60 | File [7] bị mất | Break link hoặc find file |
| TRẠM 1!AE58 | ='[7]TRẠM 2'!$AA$75 | File [7] bị mất | Break link hoặc find file |
| TRẠM 2!AD76 | ='[2]TRẠM 2'!$AA$75 | File [2] bị mất | Break link hoặc find file |

---

## 🎯 NGUYÊN TẮC TÍNH TOÁN

### Công Thức Lương Chính

```
BƯỚC 1: Lấy bản lương công khai
  F = VLOOKUP(MSNV, CÁCH TÍNH LƯƠNG, 17)

BƯỚC 2: Lấy công làm việc
  H = VLOOKUP(MSNV, 'CC NGƯỜI LÀO', 34)

BƯỚC 3: Tính lương cơ bản
  Lương_cơ_bản = E (từ TRẠM/TRẠM 2)

BƯỚC 4: Tính lương thực tế
  I = (F + H) / P7 × E
  Trong đó: P7 = tham số điều chỉnh

BƯỚC 5: Tính thuế lũy tiến
  J = IF(I > 65M, ..., IF(I > 25M, ..., ...))

BƯỚC 6: Lương net
  Net = I - J
```

### Qui Tắc Ràng Buộc

1. **E >= 0:** Lương cơ bản không âm
2. **F >= 0:** Bản lương không âm
3. **H >= 0:** Công không âm
4. **I >= F (minimum):** Lương tính >= bản lương
5. **0 < P7:** Tham số phải dương (không chia cho 0)
6. **J < I:** Thuế phải nhỏ hơn lương

---

**Ngày cập nhật:** 09/04/2025  
**Trạng thái:** ✅ Chi tiết hoàn chỉnh  
**Người phụ trách:** Kiểm toán viên

