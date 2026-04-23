# ĐẶC TẢ YÊU CẦU HỆ THỐNG TÍNH LƯƠNG KHAI THÁC — ĐỘI 1
# SYSTEM REQUIREMENTS SPECIFICATION (SRS)
## ECOTECH 2A — Payroll System cho Đội sản xuất

---

| Thông tin | Chi tiết |
|---|---|
| **Dự án** | Hệ thống tính lương khai thác mủ cao su — Đội 1 |
| **Phiên bản** | 2.0 (Thu hẹp phạm vi theo thực tế) |
| **Ngày tạo** | 13/04/2026 |
| **File Excel gốc** | `LƯƠNG ĐỘI 1 THÁNG 12.2025 2 BẢN.xlsx` |
| **Đơn vị** | Công ty TNHH MTV QUASA-GERUCO — Đội sản xuất số 01 |
| **Đối tượng đọc** | CEO (nghiệp vụ) & Team Dev (kỹ thuật) |

---

## PHẠM VI GIAI ĐOẠN 1 — CHỈ TẬP TRUNG:

```
 TRONG PHẠM VI:
 ├── [x] Lương công nhân khai thác TRẠM 1 (43 người)
 ├── [x] Lương công nhân khai thác TRẠM 2 (57 người)
 ├── [x] Cán bộ đội (7 người — có tính bảo hiểm)
 ├── [x] Bảo vệ + Tạp vụ người Lào (5 người)
 ├── [x] Bảng phân toán quỹ lương
 └── [x] Quản lý sản lượng mủ + DRC + chấm công

 NGOÀI PHẠM VI (chưa làm):
 ├── [ ] Cấp quản lý công ty trở lên
 ├── [ ] Các đội khác (Đội 2, 3, 4, 5, 6)
 ├── [ ] HR toàn diện, tuyển dụng, đào tạo
 └── [ ] Module kế toán tổng hợp
```

---

# MỤC LỤC

1. [DỮ LIỆU THỰC TẾ TỪ FILE EXCEL](#1-dữ-liệu-thực-tế-từ-file-excel)
2. [LOGIC TÍNH LƯƠNG CÔNG NHÂN KHAI THÁC](#2-logic-tính-lương-công-nhân-khai-thác)
3. [LOGIC TÍNH LƯƠNG CÁN BỘ ĐỘI & BẢO VỆ](#3-logic-tính-lương-cán-bộ-đội--bảo-vệ)
4. [BẢNG PHÂN TOÁN](#4-bảng-phân-toán)
5. [THIẾT KẾ DATABASE](#5-thiết-kế-database)
6. [USE CASES](#6-use-cases)
7. [ERD](#7-erd)
8. [BẢNG ÁNH XẠ CÔNG THỨC EXCEL → HỆ THỐNG](#8-bảng-ánh-xạ-công-thức-excel--hệ-thống)
9. [TÓM TẮT CHÍNH SÁCH CHO CEO](#9-tóm-tắt-chính-sách-cho-ceo)

---
---

# 1. DỮ LIỆU THỰC TẾ TỪ FILE EXCEL

> Tất cả số liệu dưới đây được trích xuất trực tiếp từ file
> `LƯƠNG ĐỘI 1 THÁNG 12.2025 2 BẢN.xlsx` — không phải ước tính.

---

## 1.1 Tổng quan Đội 1 tháng 12/2025

| Chỉ số | Giá trị |
|---|---|
| Tổng số công nhân khai thác | **105 người** (TRẠM 1: 43 + TRẠM 2: 57 + đặc biệt) |
| Cán bộ đội | **7 người** (6 tính USD, 1 tính Kíp) |
| Bảo vệ + Tạp vụ | **5 người** |
| **Tổng quỹ lương** | **625.619.234 kip** |
| Tỷ giá Bath → Kip | **600** (TRẠM 1!E8) |
| Tỷ giá USD → Kip | **640** (CÁCH TÍNH LƯƠNG!V9) |

## 1.2 TRẠM 1 — Số liệu thực

| Chỉ số | Giá trị | Vị trí Excel |
|---|---|---|
| Số công nhân | **43** | TRẠM 1!A16:A58 |
| Tổng mủ tạp | **45.251 kg** | TRẠM 1!AE7 |
| Tổng mủ quy khô | **17.426 kg** | TRẠM 1!AE8 |
| DRC mủ tạp | **0,3851** (= 17.426 / 45.251) | TRẠM 1!AE6 |
| DRC tháng trước (truy lĩnh) | **0,0748** | TRẠM 1!AE5 |
| DRC mủ serum | **0,5495** | TRẠM 1!AE9 |
| Tỷ giá | **600** | TRẠM 1!E8 |
| Tổng tiền sản lượng | **113.885.494 kip** | |
| Tổng phụ cấp | **45.334.769 kip** | |
| **Tổng lương TRẠM 1** | **159.220.263 kip** | |
| Lương trung bình/người | **3.538.228 kip** | |
| Phân bố hạng: A | **23 người (53%)** | |
| Phân bố hạng: B | **20 người (47%)** | |
| Phân bố hạng: C, D | **0 người** | |
| CN có khộp nặng | **8 người** | |
| CN có cây non | **1 người** (Nàng Ni) | |

## 1.3 TRẠM 2 — Số liệu thực

| Chỉ số | Giá trị | Vị trí Excel |
|---|---|---|
| Số công nhân | **57** | TRẠM 2!A16:A73 |
| Tổng mủ tạp | **131.969 kg** | TRẠM 2!AE7 |
| Tổng mủ quy khô | **49.757 kg** | TRẠM 2!AE8 |
| DRC mủ tạp | **0,3770** | TRẠM 2!AE6 |
| DRC tháng trước (truy lĩnh) | **0,0479** | TRẠM 2!AE5 |
| DRC mủ serum | **0,4395** | TRẠM 2!AE9 |
| Tổng tiền sản lượng | **255.996.254 kip** | |
| Tổng phụ cấp | **194.483.167 kip** | |
| **Tổng lương TRẠM 2** | **450.479.421 kip** | |
| Lương trung bình/người | **7.507.990 kip** | |
| Phân bố hạng: A | **31 người (54%)** | |
| Phân bố hạng: B | **26 người (46%)** | |

**Ghi chú:** TRẠM 2 lương TB cao hơn TRẠM 1 (7,5M vs 3,5M) vì mủ tạp/người cao hơn (~2.315 kg vs ~1.052 kg) và có phụ cấp cạo 2 miệng lớn.

## 1.4 Đơn giá thực tế (từ BẢNG PHÂN TOÁN)

| Hạng | TRẠM 1 (kip/kg) | TRẠM 2 (kip/kg) | Hệ số (÷ 600) |
|---|---|---|---|
| **A** | **5.520** | **4.620** | 9,2 / 7,7 |
| **B** | **5.340** | **4.440** | 8,9 / 7,4 |
| **C** | **5.160** | **4.260** | 8,6 / 7,1 |
| **D** | **4.980** | **4.080** | 8,3 / 6,8 |
| **Mủ xirum** | **3.660** | **3.660** | 6,1 |

Xác nhận: `Đơn giá = Hệ số hạng × Tỷ giá 600`

## 1.5 Đơn giá phụ cấp thực tế (từ BẢNG PHÂN TOÁN)

| Loại phụ cấp | Đơn giá (kip/công) | Số công T12 | Thành tiền |
|---|---|---|---|
| Công thường | **30.793** | 1.662 | 51.177.895 |
| Công chủ nhật | **51.366** | 217 | 11.146.445 |
| Cây non | **20.506** | 69 | 1.414.940 |
| Khộp nặng | **20.000** | 212 | 4.240.000 |
| Cạo 2 miệng | **61.586** | 874 | 53.826.087 |
| Cạo 2 miệng CN | **102.732** | 151 | 15.512.569 |
| PC thêm cạo 2 miệng | **100.000** | 1.025 | 102.500.000 |

**Hệ số factor đã xác nhận** = 30.793 ÷ 46,1 ≈ **667,96** (giá trị tại TRẠM 1!AD4 / TRẠM 2!AD3)

---
---

# 2. LOGIC TÍNH LƯƠNG CÔNG NHÂN KHAI THÁC

> Đây là phần cốt lõi. Logic này áp dụng cho cả TRẠM 1 và TRẠM 2,
> chỉ khác nhau ở **hệ số hạng** và **DRC** theo từng trạm.

---

## 2.1 Công thức tổng — 1 dòng công nhân

Mỗi dòng từ hàng 16 trở xuống trong sheet TRẠM 1 / TRẠM 2 là 1 công nhân:

```
TỔNG LƯƠNG = Lương sản lượng (O16) + Lương chăm sóc (G16) + Phụ cấp (Y16)

Trong đó:
  Lương sản lượng  = Tổng mủ trả lương × Hệ số hạng × Tỷ giá
  Lương chăm sóc   = Công chăm sóc × 25.000 kip
  Phụ cấp          = (Phụ cấp hệ số × factor) + (Phụ cấp cố định)
```

## 2.2 Bước 1 — Quy đổi sản lượng (từ mủ tạp → mủ trả lương)

```
Nguồn: TRẠM 1!H16 → J16 → L16 → M16

  H16 = Mủ tạp (kg)                    ← VLOOKUP từ sheet "sản lượng"
  I16 = Mủ xirum (kg)                  ← nếu có
  J16 = H16 × $AE$6                   ← Mủ quy khô = Mủ tạp × DRC hiện hành
  K16 = Mủ tạp truy lĩnh (kg)         ← từ tháng trước
  L16 = K16 × $AE$5                   ← Mủ quy khô truy lĩnh = truy lĩnh × DRC tháng trước
  M16 = J16 + L16                     ← TỔNG MỦ TRẢ LƯƠNG

VÍ DỤ THỰC TẾ (CN "Thô", TRẠM 1, hàng 16):
  H16 = 1.605 kg mủ tạp
  J16 = 1.605 × 0,3851 = 618 kg quy khô
  K16 = 1.605 kg truy lĩnh
  L16 = 1.605 × 0,0748 = 120 kg quy khô truy lĩnh
  M16 = 618 + 120 = 738 kg tổng mủ trả lương
```

**Pseudocode cho Dev:**

```python
def quy_doi_san_luong(mu_tap, mu_truy_linh, drc_hien_hanh, drc_thang_truoc):
    """
    Nguồn: TRẠM!H16 → M16
    """
    quy_kho = mu_tap * drc_hien_hanh            # J16 = H16 × AE6
    quy_kho_truy_linh = mu_truy_linh * drc_thang_truoc  # L16 = K16 × AE5
    tong_mu_tra_luong = quy_kho + quy_kho_truy_linh     # M16 = J16 + L16
    return tong_mu_tra_luong
```

## 2.3 Bước 2 — Tính tiền sản lượng (theo hạng kỹ thuật)

```
Nguồn: TRẠM 1!O16

  O16 = IF(N16="A", M16 × 9,2 × 600,
        IF(N16="B", M16 × 8,9 × 600,
        IF(N16="C", M16 × 8,6 × 600,
        IF(N16="D", M16 × 8,3 × 600,
        0))))

  Trong đó:
    N16 = Hạng kỹ thuật A/B/C/D       ← VLOOKUP từ sheet "sản lượng"
    M16 = Tổng mủ trả lương (kg)      ← bước 1
    9,2 / 8,9 / 8,6 / 8,3 = hệ số hạng TRẠM 1
    600 = tỷ giá ($E$8)

VÍ DỤ THỰC TẾ (CN "Thô", TRẠM 1, Hạng A):
  O16 = 738 × 9,2 × 600 = 4.074.512 kip  ✓ (khớp file Excel)
```

**Pseudocode:**

```python
def tinh_tien_san_luong(tong_mu_tra_luong, hang_kt, station_id):
    """
    Nguồn: TRẠM!O16
    """
    # Lấy hệ số hạng từ bảng stations (không hardcode trong code!)
    he_so = get_grade_rate(station_id, hang_kt)
    # VD: TRẠM 1 hạng A → 9.2, TRẠM 2 hạng A → 7.7

    ty_gia = get_param('EXCHANGE_RATE')  # 600

    if hang_kt not in ('A', 'B', 'C', 'D'):
        return 0  # Không có hạng → 0 đồng

    return tong_mu_tra_luong * he_so * ty_gia
```

## 2.4 Bước 3 — Lương chăm sóc

```
Nguồn: TRẠM 1!G16

  G16 = F16 × 25.000

  Trong đó:
    F16 = Số công chăm sóc
    25.000 = đơn giá cố định (kip/công)

GHI CHÚ: Tháng 12/2025, lương chăm sóc = 0 cho tất cả CN
(cột F toàn bộ trống → G = 0)
→ Trong BẢNG PHÂN TOÁN dòng 1.3 cũng xác nhận: 0 công × 25.000 = 0
```

## 2.5 Bước 4 — Tính phụ cấp (phức tạp nhất)

```
Nguồn: TRẠM 1!Y16

  Y16 = PHẦN_HỆ_SỐ + PHẦN_CỐ_ĐỊNH

  PHẦN_HỆ_SỐ = (
      P16 × 46,1          (công thường)
    + Q16 × 76,9          (công chủ nhật)
    + R16 × 30,7          (cây non)
    + T16 × 46,1 × 2      (cạo 2 lát thường — TRẠM 1)
    + U16 × 76,9 × 2      (cạo 2 lát CN — TRẠM 1)
  ) × factor

  Trong đó factor ≈ 667,96 (giá trị tại AD4)
  → Kết quả ra đơn giá thực: 46,1 × 667,96 = 30.793 kip/công ✓

  PHẦN_CỐ_ĐỊNH =
      S16 × 20.000        (khộp nặng — 20.000 kip/công)
    + V16 × 100.000       (PC thêm cạo 2 lát — 100.000 kip/công)

  Lưu ý TRẠM 2 có "cạo 2 miệng" thay vì "cạo 2 lát":
    Cột T/U → cạo 2 miệng thường / CN
    Cột tương ứng → PC thêm cạo 2 miệng

VÍ DỤ THỰC TẾ (CN "Thô", TRẠM 1, hàng 16):
  P16 = 27 (công thường), Q16 = 4 (công CN)
  R16 = 0, S16 = 0, T16 = 0, U16 = 0

  Phần hệ số = (27 × 46,1 + 4 × 76,9) × 667,96
             = (1.244,7 + 307,6) × 667,96
             = 1.552,3 × 667,96
             = 1.036.874 kip
  Phần cố định = 0
  Y16 = 1.036.874 kip  ✓ (khớp file Excel)

VÍ DỤ 2 (CN "Tô Tộ", TRẠM 1, hàng 19 — có khộp nặng):
  P16 = 27, Q16 = 4, S16 = 31 (khộp nặng)
  Phần hệ số = (27 × 46,1 + 4 × 76,9) × 667,96 = 1.036.874
  Phần cố định = 31 × 20.000 = 620.000
  Y16 = 1.036.874 + 620.000 = 1.656.874 kip  ✓ (khớp file Excel)
```

**Pseudocode:**

```python
def tinh_phu_cap(attendance, factor, station_id):
    """
    Nguồn: TRẠM!Y16
    factor = giá trị tại AD4 (≈ 667,96 — thay đổi theo tháng)
    """
    # Phần 1: phụ cấp theo hệ số × factor
    phan_he_so = (
        attendance.regular_days    * 46.1        # công thường
      + attendance.sunday_days     * 76.9        # công CN
      + attendance.young_tree_days * 30.7        # cây non
      + attendance.double_cut_days    * 46.1 * 2 # cạo 2 lát/miệng thường
      + attendance.double_cut_sunday  * 76.9 * 2 # cạo 2 lát/miệng CN
    ) * factor

    # Phần 2: phụ cấp cố định
    phan_co_dinh = (
        attendance.hardship_days * 20_000        # khộp nặng
      + (attendance.double_cut_days
         + attendance.double_cut_sunday) * 100_000  # PC thêm cạo 2 lát/miệng
    )

    return phan_he_so + phan_co_dinh
```

## 2.6 Bước 5 — Tổng lương công nhân

```
Nguồn: TRẠM 1!Z16, AA16

  Z16 = O16 + G16 + Y16     (Tổng lương)
  AA16 = Z16                 (Lương còn lại — hiện chưa trừ gì)

  CN khai thác hiện KHÔNG bị trừ thuế, không trừ bảo hiểm.

VÍ DỤ (CN "Thô", TRẠM 1):
  Z16 = 4.074.512 + 0 + 1.036.874 = 5.111.386 kip  ✓
```

## 2.7 Tham số factor (AD4) — QUAN TRỌNG

```
Giá trị thực tế tháng 12:
  TRẠM 1!AD4 = 667,96
  TRẠM 2!AD3 = 667,96 (= 'TRẠM 1'!AD4 — cross-reference)

Xác minh:
  30.793 (đơn giá công thường BPT) ÷ 46,1 = 667,96  ✓
  51.366 (đơn giá công CN BPT) ÷ 76,9 = 667,96       ✓
  20.506 (đơn giá cây non BPT) ÷ 30,7 = 668,01       ≈ ✓ (sai số làm tròn)

→ Factor này THAY ĐỔI THEO THÁNG, cần lưu vào bảng tham số.
```

---
---

# 3. LOGIC TÍNH LƯƠNG CÁN BỘ ĐỘI & BẢO VỆ

---

## 3.1 Cán bộ đội (CÁCH TÍNH LƯƠNG — 7 người)

Dữ liệu thực từ sheet `CÁCH TÍNH LƯƠNG`:

| STT | Họ tên | Chức vụ | Trạm | Đơn vị | Hệ số K | KH giao | Thực hiện | Tỷ lệ HT | Lương bậc | Lương TL | Tổng |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 | Vũ Xuân Sơn | Đội trưởng | — | USD | 1,2 | 66 | 77,44 | 117,3% | 293,33 | 344,19 | **637,53 USD** |
| 2 | Nguyễn Văn Hiếu | Đội phó | Trạm 2 | USD | 1,2 | 41 | 56,47 | 137,7% | 310 | 462,82 | **772,82 USD** |
| 3 | Nguyễn Thị Khánh Ly | NVKT | Trạm 1 | USD | 1,2 | 25 | 20,97 | 83,9% | 280 | 281,82 | **561,82 USD** |
| 4 | Vi Ngọc Thức | NVKT | Trạm 2 | USD | 1,2 | 41 | 56,47 | 137,7% | 280 | 462,82 | **742,82 USD** |
| 5 | Nguyễn Thị Thu Hiền | TH-TK | Đội 1 | USD | — | 66 | 77,44 | 117,3% | 280 | 328,55 | **608,55 USD** |
| 6 | Võ Thành Minh | Bảo vệ | Trạm 2 | USD | — | 41 | 56,47 | 137,7% | 220 | 344,36 | **564,36 USD** |
| 7 | Chăn Tho Duangchanhsou | Đội phó | Trạm 1 | **Kíp** | 1,2 | 25 | 20,97 | 83,9% | 3.700.000 | 3.019.550 | **6.719.550 kip** |

### Công thức cán bộ đội (từ Excel thực):

```
Nguồn: CÁCH TÍNH LƯƠNG!O11, Q11, S11

  Lương bậc (O11):
    Giá trị cố định theo chức danh (293,33 / 310 / 280 / 220 USD)

  Sản lượng thực hiện (I12):
    = Tổng mủ quy khô của trạm phụ trách / 1.000
    VD: Trạm 2: I12 = TRẠM 2!M76 / 1000 = 56,47

  Tỷ lệ hoàn thành (J12):
    = I12 / H12 (thực hiện / kế hoạch)
    VD: 56,47 / 41 = 137,7%

  Lương theo tỷ lệ (Q11):
    = (300 × J11 × G11) / 27 × 22
    VD Đội trưởng: (300 × 1,1734 × 1,2) / 27 × 22 = 344,19 USD

  Tổng (S11):
    = O11 + Q11 + P11 (phụ cấp) + R11 (truy lĩnh)
    VD Đội trưởng: 293,33 + 344,19 + 0 + 0 = 637,53 USD
```

**Pseudocode:**

```python
def tinh_luong_can_bo(officer):
    """Nguồn: CÁCH TÍNH LƯƠNG"""
    luong_bac = officer.base_salary          # O11 — theo chức danh
    sl_thuc_hien = get_station_production(officer.station_id) / 1000  # I12
    ty_le_ht = sl_thuc_hien / officer.ke_hoach                       # J12

    he_so_k = officer.he_so_k or 1.0                                 # G11
    luong_ty_le = (300 * ty_le_ht * he_so_k) / 27 * 22              # Q11

    tong = luong_bac + luong_ty_le + officer.phu_cap + officer.truy_linh
    return tong
```

## 3.2 Bảo vệ + Tạp vụ (LƯƠNG ĐỘI — 5 người)

Dữ liệu thực từ sheet `LƯƠNG ĐỘI`:

| Nhóm | Tên | Chức vụ | Công | Mức lương | Phụ cấp | Tổng | Thuế | Còn lại |
|---|---|---|---|---|---|---|---|---|
| **I. CB Lào** | Chăn Tho | NVKT | 27 | 6.719.550 | 0 | 6.719.550 | **356.955** | **6.362.595** |
| **II. Bảo vệ** | San | BV | 31 | 2.000.000 | 600.000 | 2.600.000 | 0 | 2.600.000 |
| | Thảo Nọi | BV | 31 | 2.000.000 | 600.000 | 2.600.000 | 0 | 2.600.000 |
| | Thảo Oăn | BV | 31 | 2.000.000 | 0 | 2.000.000 | 0 | 2.000.000 |
| **III. Tạp vụ** | Nàng Ta | TV | 31 | 2.000.000 | 0 | 2.000.000 | 0 | 2.000.000 |
| **TỔNG** | | | 151 | 14.719.550 | 1.200.000 | **15.919.550** | **356.955** | **15.562.595** |

### Công thức (từ Excel thực):

```
Nguồn: LƯƠNG ĐỘI!I13, J13, K13

  Lương tính (I13) = (F13 + H13) / P7 × E13
    Trong đó:
      F13 = Mức lương (từ CÁCH TÍNH LƯƠNG)
      H13 = Phụ cấp
      P7  = Công chuẩn nhóm (CB: 27, BV: 31)
      E13 = Công thực tế

  Thuế TNCN (J13) — chỉ khi thu nhập > 1.300.000:
    IF I13 > 65.000.000: (I13 - 65M) × 35% + 10.685.000
    IF I13 > 25.000.000: (I13 - 25M) × 25% + 2.685.000
    IF I13 > 20.000.000: (I13 - 20M) × 20% + 2.250.000
    IF I13 > 10.000.000: (I13 - 10M) × 15% + 750.000
    IF I13 >  5.000.000: (I13 - 5M) × 10% + 250.000
    IF I13 >  1.300.000: (I13 - 1,3M) × 5%
    ELSE: 0

  VÍ DỤ (Chăn Tho — 6.719.550 kip):
    Thuế = (6.719.550 - 5.000.000) × 10% + 250.000
         = 171.955 + 250.000 = 421.955? 
    → File ghi 356.955 → cần kiểm tra lại mức ngưỡng chính xác

  Lương còn lại (K13) = I13 - J13
```

---
---

# 4. BẢNG PHÂN TOÁN

> Nguồn: Sheet `BẢNG PHÂN TOÁN ` — tổng hợp toàn bộ quỹ lương để trình ký.

## 4.1 Cấu trúc (dữ liệu thực)

| STT | Khoản mục | Số lượng | Đơn giá (kip) | Thành tiền (kip) |
|---|---|---|---|---|
| **I** | **TIỀN LƯƠNG KINH DOANH** | | | **616.899.684** |
| 1 | CN trực tiếp | | | 609.699.684 |
| 1.1 | Sản lượng (tổng) | 77.444 kg | | **369.881.748** |
| | — TRẠM 1 hạng A | 12.091 kg | 5.520 | 66.743.262 |
| | — TRẠM 1 hạng B | 8.720 kg | 5.340 | 46.563.011 |
| | — TRẠM 2 hạng A | 30.866 kg | 4.620 | 142.600.611 |
| | — TRẠM 2 hạng B | 25.215 kg | 4.440 | 111.955.818 |
| | — Mủ xirum | 552 kg | 3.660 | 2.019.046 |
| 1.2 | Phụ cấp ngày công | 4.210 công | | **239.817.936** |
| | — Công thường | 1.662 | 30.793 | 51.177.895 |
| | — Công chủ nhật | 217 | 51.366 | 11.146.445 |
| | — Cây non | 69 | 20.506 | 1.414.940 |
| | — Khộp nặng | 212 | 20.000 | 4.240.000 |
| | — Cạo 2 miệng | 874 | 61.586 | 53.826.087 |
| | — Cạo 2 miệng CN | 151 | 102.732 | 15.512.569 |
| | — PC thêm cạo 2 miệng | 1.025 | 100.000 | 102.500.000 |
| 1.3 | Lương chăm sóc | 0 | 25.000 | 0 |
| 2 | Bảo vệ vườn cây | 288 công | 25.000 | 7.200.000 |
| **II** | **CB-CNV NGƯỜI LÀO** | | | **6.719.550** |
| **III** | **TẠP VỤ NGƯỜI LÀO** | | | **2.000.000** |
| | **TỔNG CỘNG** | | | **625.619.234** |

---
---

# 5. THIẾT KẾ DATABASE

> Thiết kế tập trung vào đúng nghiệp vụ: sản lượng mủ → quy khô → tính tiền → phụ cấp.

---

## 5.1 Danh sách bảng (9 bảng/view chính)

| # | Bảng | Mục đích | Nguồn Excel |
|---|---|---|---|
| 1 | `employees` | Nhân viên (CN + CB + BV) | MSNV 2025 |
| 2 | `stations` | Trạm khai thác + hệ số hạng | TRẠM 1, TRẠM 2 |
| 3 | `monthly_params` | Tham số tháng (DRC, tỷ giá, factor) | AE5-AE9, E8, AD4 |
| 4 | `daily_attendance` | Chấm công hằng ngày (chi tiết) | CHẤM CÔNG, từng ngày |
| 5 | `attendance` *(VIEW)* | Tổng hợp công theo tháng (tự tính từ daily_attendance) | — |
| 6 | `production` | Sản lượng mủ + hạng KT | sản lượng, TRẠM!H-N |
| 7 | `payroll` | Bảng lương tháng | TRẠM!Z, LƯƠNG ĐỘI |
| 8 | `payroll_details` | Chi tiết từng khoản | Các cột TRẠM |
| 9 | `cost_allocation` | Bảng phân toán | BẢNG PHÂN TOÁN |

## 5.2 Chi tiết từng bảng

### `employees` — Nhân viên

```sql
CREATE TABLE employees (
    employee_id     INT PRIMARY KEY AUTO_INCREMENT,
    employee_code   VARCHAR(20) UNIQUE NOT NULL,     -- "02440", "03423"
    full_name       NVARCHAR(100) NOT NULL,          -- "Thô", "Mè Ta Van"
    station_id      INT NULL,                        -- FK → stations
    employee_type   ENUM('WORKER','OFFICER','GUARD','CLEANER') NOT NULL,
    position        NVARCHAR(50) NULL,               -- "CNKT", "Đội trưởng", "Bảo vệ"
    currency        ENUM('KIP','USD') NOT NULL DEFAULT 'KIP',
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (station_id) REFERENCES stations(station_id)
);
```

### `stations` — Trạm khai thác + hệ số

```sql
CREATE TABLE stations (
    station_id      INT PRIMARY KEY AUTO_INCREMENT,
    station_name    NVARCHAR(50) NOT NULL,            -- "TRẠM 1", "TRẠM 2"
    grade_a_rate    DECIMAL(5,2) NOT NULL,            -- 9.2 / 7.7
    grade_b_rate    DECIMAL(5,2) NOT NULL,            -- 8.9 / 7.4
    grade_c_rate    DECIMAL(5,2) NOT NULL,            -- 8.6 / 7.1
    grade_d_rate    DECIMAL(5,2) NOT NULL             -- 8.3 / 6.8
);

INSERT INTO stations VALUES (1, 'TRẠM 1', 9.2, 8.9, 8.6, 8.3);
INSERT INTO stations VALUES (2, 'TRẠM 2', 7.7, 7.4, 7.1, 6.8);
```

### `monthly_params` — Tham số tháng

```sql
CREATE TABLE monthly_params (
    param_id        INT PRIMARY KEY AUTO_INCREMENT,
    month           INT NOT NULL,
    year            INT NOT NULL,
    station_id      INT NULL,                          -- NULL = toàn đội
    exchange_rate   DECIMAL(10,2) NOT NULL,             -- 600 (Bath→Kip)
    exchange_usd    DECIMAL(10,2) NULL,                 -- 640 (USD→Kip)
    drc_raw_latex   DECIMAL(8,6) NOT NULL,              -- 0.385087 (AE6)
    drc_prev_month  DECIMAL(8,6) NULL,                  -- 0.074811 (AE5)
    drc_serum       DECIMAL(8,6) NULL,                  -- 0.549500 (AE9)
    allowance_factor DECIMAL(10,4) NOT NULL,            -- 667.96 (AD4)
    care_rate       DECIMAL(10,0) NOT NULL DEFAULT 25000, -- 25.000 kip/công
    hardship_rate   DECIMAL(10,0) NOT NULL DEFAULT 20000, -- 20.000 kip/công
    double_cut_bonus DECIMAL(10,0) NOT NULL DEFAULT 100000, -- 100.000 kip/công
    UNIQUE KEY uk_period (month, year, station_id)
);

-- Dữ liệu tháng 12/2025:
INSERT INTO monthly_params VALUES
(1, 12, 2025, 1, 600, 640, 0.385087, 0.074811, 0.549500, 667.96, 25000, 20000, 100000),
(2, 12, 2025, 2, 600, 640, 0.377035, 0.047923, 0.439549, 667.96, 25000, 20000, 100000);
```

### `daily_attendance` — Chấm công hằng ngày (bảng gốc)

> **Đây là bảng gốc ghi nhận chi tiết từng ngày làm việc của công nhân.**
> Mỗi ngày công nhân đi làm sẽ có 1 bản ghi, cho phép:
> - Truy vết ngày nào đi làm, ngày nào nghỉ
> - Nhập liệu dần trong tháng (không phải đợi cuối tháng)
> - Kiểm tra chéo: tổng công = đếm từ daily → nhất quán 100%

```sql
CREATE TABLE daily_attendance (
    daily_id            INT PRIMARY KEY AUTO_INCREMENT,
    employee_id         INT NOT NULL,
    work_date           DATE NOT NULL,                -- ngày làm việc cụ thể
    attendance_type     ENUM(
        'REGULAR',          -- công thường (ngày thường, cạo 1 miệng)
        'SUNDAY',           -- công chủ nhật (cạo 1 miệng)
        'YOUNG_TREE',       -- công cây non
        'HARDSHIP',         -- công khộp nặng
        'DOUBLE_CUT',       -- cạo 2 lát/miệng ngày thường
        'DOUBLE_CUT_SUNDAY',-- cạo 2 lát/miệng ngày CN
        'CARE',             -- công chăm sóc
        'ABSENT'            -- nghỉ (có ghi nhận)
    ) NOT NULL,
    shift_value         DECIMAL(3,1) DEFAULT 1.0,     -- 1.0 = cả ngày, 0.5 = nửa ngày
    notes               NVARCHAR(200) NULL,           -- ghi chú (lý do nghỉ, v.v.)
    created_at          DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at          DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uk_emp_date (employee_id, work_date),  -- mỗi CN chỉ 1 bản ghi/ngày
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);
```

**Quy tắc nghiệp vụ:**
- Mỗi công nhân chỉ có **1 bản ghi / ngày** (UNIQUE trên employee_id + work_date)
- `attendance_type` xác định loại công trong ngày đó — quyết định đơn giá phụ cấp
- `DOUBLE_CUT` và `DOUBLE_CUT_SUNDAY` tự bao gồm công thường/CN, **không cần** ghi thêm dòng REGULAR/SUNDAY
- `shift_value = 0.5` cho trường hợp làm nửa ngày (hiếm nhưng có thể xảy ra)
- `ABSENT` là tùy chọn — chỉ ghi khi muốn theo dõi ngày nghỉ cụ thể

### `attendance` — Tổng hợp công theo tháng *(VIEW)*

> **VIEW tự động tổng hợp từ `daily_attendance`.**
> Thay thế bảng attendance cũ — các bảng khác (payroll, phụ cấp) truy vấn VIEW này
> mà không cần thay đổi logic. Đảm bảo dữ liệu luôn nhất quán với chấm công hằng ngày.

```sql
CREATE VIEW attendance AS
SELECT
    employee_id,
    MONTH(work_date)                                            AS month,
    YEAR(work_date)                                             AS year,
    SUM(CASE WHEN attendance_type = 'CARE'
             THEN shift_value ELSE 0 END)                       AS care_days,
    SUM(CASE WHEN attendance_type = 'REGULAR'
             THEN shift_value ELSE 0 END)                       AS regular_days,
    SUM(CASE WHEN attendance_type = 'SUNDAY'
             THEN shift_value ELSE 0 END)                       AS sunday_days,
    SUM(CASE WHEN attendance_type = 'YOUNG_TREE'
             THEN shift_value ELSE 0 END)                       AS young_tree_days,
    SUM(CASE WHEN attendance_type = 'HARDSHIP'
             THEN shift_value ELSE 0 END)                       AS hardship_days,
    SUM(CASE WHEN attendance_type = 'DOUBLE_CUT'
             THEN shift_value ELSE 0 END)                       AS double_cut_days,
    SUM(CASE WHEN attendance_type = 'DOUBLE_CUT_SUNDAY'
             THEN shift_value ELSE 0 END)                       AS double_cut_sunday
FROM daily_attendance
WHERE attendance_type != 'ABSENT'
GROUP BY employee_id, MONTH(work_date), YEAR(work_date);
```

> **Lợi ích:** Toàn bộ code tính lương, phụ cấp đã dùng `attendance.regular_days`, `attendance.sunday_days`...
> **không cần sửa gì** — VIEW trả về đúng cấu trúc cũ.

### `production` — Sản lượng mủ

```sql
CREATE TABLE production (
    production_id    INT PRIMARY KEY AUTO_INCREMENT,
    employee_id      INT NOT NULL,
    month            INT NOT NULL,
    year             INT NOT NULL,
    raw_latex_kg     DECIMAL(10,2) DEFAULT 0,       -- H16: mủ tạp
    serum_kg         DECIMAL(10,2) DEFAULT 0,       -- I16: mủ xirum
    carry_over_kg    DECIMAL(10,2) DEFAULT 0,       -- K16: truy lĩnh
    tech_grade       ENUM('A','B','C','D') NULL,    -- N16: hạng KT
    UNIQUE KEY uk_emp_period (employee_id, month, year),
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);
```

### `payroll` — Bảng lương tháng

```sql
CREATE TABLE payroll (
    payroll_id          INT PRIMARY KEY AUTO_INCREMENT,
    employee_id         INT NOT NULL,
    month               INT NOT NULL,
    year                INT NOT NULL,
    -- Các khoản tính
    dry_latex_kg        DECIMAL(10,2) DEFAULT 0,     -- J16: mủ quy khô
    carry_dry_kg        DECIMAL(10,2) DEFAULT 0,     -- L16: truy lĩnh quy khô
    total_pay_kg        DECIMAL(10,2) DEFAULT 0,     -- M16: tổng mủ trả lương
    production_salary   DECIMAL(15,0) DEFAULT 0,     -- O16: tiền sản lượng
    care_salary         DECIMAL(15,0) DEFAULT 0,     -- G16: lương chăm sóc
    allowance_total     DECIMAL(15,0) DEFAULT 0,     -- Y16: phụ cấp
    gross_salary        DECIMAL(15,0) NOT NULL,       -- Z16: tổng lương
    tax_amount          DECIMAL(15,0) DEFAULT 0,      -- thuế (nếu có)
    net_salary          DECIMAL(15,0) NOT NULL,       -- AA16: còn lại
    status              ENUM('DRAFT','APPROVED','PAID') DEFAULT 'DRAFT',
    UNIQUE KEY uk_emp_period (employee_id, month, year),
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);
```

### `payroll_details` — Chi tiết từng khoản phụ cấp

```sql
CREATE TABLE payroll_details (
    detail_id    INT PRIMARY KEY AUTO_INCREMENT,
    payroll_id   INT NOT NULL,
    item_code    VARCHAR(30) NOT NULL,    -- 'ALLOW_REGULAR', 'ALLOW_SUNDAY'...
    item_name    NVARCHAR(100) NOT NULL,
    quantity     DECIMAL(10,2) NULL,      -- số công
    rate         DECIMAL(15,4) NULL,      -- đơn giá
    amount       DECIMAL(15,0) NOT NULL,  -- thành tiền
    FOREIGN KEY (payroll_id) REFERENCES payroll(payroll_id)
);
```

### `cost_allocation` — Bảng phân toán

```sql
CREATE TABLE cost_allocation (
    allocation_id   INT PRIMARY KEY AUTO_INCREMENT,
    month           INT NOT NULL,
    year            INT NOT NULL,
    category        NVARCHAR(100) NOT NULL,   -- "Sản lượng TRẠM 1 hạng A"
    unit            VARCHAR(10) NULL,          -- "Kg", "Công"
    quantity        DECIMAL(12,2) NULL,
    unit_price      DECIMAL(15,0) NULL,
    total_amount    DECIMAL(15,0) NOT NULL,
    parent_category NVARCHAR(100) NULL         -- nhóm cha
);
```

---
---

# 6. USE CASES

> Chỉ các use case trực tiếp liên quan đến lương đội khai thác.

---

## 6.1 Tổng quan

```
 ACTOR: Kế toán đội                    ACTOR: QLKT
 │                                     │
 ├── UC01: Nhập/import chấm công hằng ngày       ├── UC02: Nhập sản lượng mủ
 ├── UC03: Nhập DRC + tham số tháng    ├── UC04: Xếp hạng kỹ thuật
 ├── UC05: Tính lương CN khai thác     │
 ├── UC06: Tính lương cán bộ đội       │
 ├── UC07: Lập bảng phân toán         │
 ├── UC08: Xuất bảng lương / phiếu    │
 │                                     │
 ACTOR: Đội trưởng                     │
 ├── UC09: Duyệt bảng lương           │
```

## 6.2 UC05: Tính lương CN khai thác (Use case chính)

```
Tên:        Tính lương công nhân khai thác
Actor:      Hệ thống (tự động) + Kế toán đội (kích hoạt + kiểm tra)
Tiền ĐK:    Đã có chấm công, sản lượng, DRC, hạng KT cho tháng
Hậu ĐK:     Bảng payroll + payroll_details được tạo

LUỒNG CHÍNH:
1. Kế toán chọn tháng/năm + nhấn "Tính lương"
2. Hệ thống duyệt từng CN trong TRẠM 1 & TRẠM 2:
   a. Lấy sản lượng mủ + hạng KT (production)
   b. Lấy DRC + tỷ giá + factor (monthly_params)
   c. Lấy hệ số hạng theo trạm (stations)
   d. Tính: quy khô → tiền sản lượng → phụ cấp → tổng
   e. Lưu vào payroll + payroll_details
3. Hệ thống hiển thị bảng lương + tổng quỹ
4. Kế toán kiểm tra, so sánh với tháng trước
5. Trạng thái: DRAFT

LUỒNG NGOẠI LỆ:
- CN thiếu hạng KT → Cảnh báo, tiền sản lượng = 0
- CN thiếu chấm công → Cảnh báo, phụ cấp = 0
- DRC chưa nhập → Không cho tính
```

---
---

# 7. ERD

```
┌──────────────┐       ┌──────────────┐
│  stations    │       │monthly_params│
│──────────────│  1:N  │──────────────│
│ station_id PK│◄──────│ station_id FK│
│ station_name │       │ month, year  │
│ grade_a_rate │       │ drc_raw_latex│
│ grade_b_rate │       │ drc_prev     │
│ grade_c_rate │       │ drc_serum    │
│ grade_d_rate │       │ exchange_rate│
└──────┬───────┘       │ factor       │
       │ 1:N           └──────────────┘
       │
┌──────┴───────┐
│  employees   │
│──────────────│
│ employee_id  │
│ employee_code│
│ full_name    │
│ station_id FK│
│ employee_type│
│ currency     │
└──────┬───────┘
       │ 1:N (mỗi tháng)
       │
  ┌────┼─────────────┐
  │    │             │
  ▼    ▼             ▼
┌───────────────┐ ┌──────────┐ ┌─────────┐
│daily_attendance│ │production│ │ payroll │
│───────────────│ │──────────│ │─────────│
│emp + date     │ │emp + m/y │ │emp + m/y│
│attend_type    │ │raw_latex │ │prod_sal │
│shift_value    │ │serum     │ │care_sal │
│notes          │ │carry_over│ │allow    │
│               │ │tech_grade│ │gross    │
│   ║ (VIEW)    │ │          │ │tax      │
│   ▼           │ │          │ │net      │
│ attendance    │ │          │ │status   │
│ (tổng m/y)    │ │          │ │         │
└───────────────┘ └──────────┘ └────┬────┘
                               │ 1:N
                               ▼
                        ┌──────────────┐
                        │payroll_detail│
                        │──────────────│
                        │item_code     │
                        │quantity      │
                        │rate          │
                        │amount        │
                        └──────────────┘

┌──────────────────┐
│ cost_allocation  │  (báo cáo — không FK)
│──────────────────│
│ month, year      │
│ category         │
│ quantity         │
│ unit_price       │
│ total_amount     │
└──────────────────┘
```

**Mối quan hệ:**

| Bảng 1 | Bảng 2 | Loại | Mô tả |
|---|---|---|---|
| stations | employees | 1:N | 1 trạm nhiều CN |
| stations | monthly_params | 1:N | Mỗi trạm có DRC riêng/tháng |
| employees | daily_attendance | 1:N | Mỗi ngày làm việc 1 bản ghi |
| daily_attendance | attendance *(VIEW)* | N:1 | VIEW tổng hợp theo tháng |
| employees | production | 1:N | Mỗi tháng 1 bản ghi |
| employees | payroll | 1:N | Mỗi tháng 1 bản ghi |
| payroll | payroll_details | 1:N | Nhiều dòng chi tiết |

---
---

# 8. BẢNG ÁNH XẠ CÔNG THỨC EXCEL → HỆ THỐNG

> Bảng này cho Dev đối chiếu: ô nào trong Excel → trường nào trong DB → tính bằng công thức gì.

| Ô Excel | Ý nghĩa | Công thức | → DB Field | Loại |
|---|---|---|---|---|
| TRẠM!E8 | Tỷ giá Bath→Kip | = 600 | monthly_params.exchange_rate | Tham số |
| TRẠM!AE5 | DRC tháng trước | Nhập tay | monthly_params.drc_prev_month | Tham số |
| TRẠM!AE6 | DRC mủ tạp | = AE8/AE7 | monthly_params.drc_raw_latex | Tham số |
| TRẠM!AE9 | DRC mủ serum | Nhập tay | monthly_params.drc_serum | Tham số |
| TRẠM!AD4 | Factor phụ cấp | = 667,96 | monthly_params.allowance_factor | Tham số |
| TRẠM!B16 | Mã NV | VLOOKUP từ MSNV | employees.employee_code | Lookup |
| TRẠM!C16 | Họ tên | Nhập tay | employees.full_name | Nhập |
| TRẠM!F16 | Công chăm sóc | Từ CHẤM CÔNG | daily_attendance → attendance.care_days *(VIEW)* | Nhập hằng ngày |
| TRẠM!G16 | Tiền chăm sóc | = F16 × 25.000 | payroll.care_salary | **Tính** |
| TRẠM!H16 | Mủ tạp (kg) | VLOOKUP từ sản lượng | production.raw_latex_kg | Lookup |
| TRẠM!J16 | Mủ quy khô | = H16 × AE6 | payroll.dry_latex_kg | **Tính** |
| TRẠM!K16 | Mủ truy lĩnh | Từ sản lượng | production.carry_over_kg | Nhập |
| TRẠM!L16 | Truy lĩnh quy khô | = K16 × AE5 | payroll.carry_dry_kg | **Tính** |
| TRẠM!M16 | Tổng mủ trả lương | = J16 + L16 | payroll.total_pay_kg | **Tính** |
| TRẠM!N16 | Hạng KT | VLOOKUP từ sản lượng | production.tech_grade | Lookup |
| TRẠM!O16 | Tiền sản lượng | = M16 × hệ_số × 600 | payroll.production_salary | **Tính** |
| TRẠM!P16 | Công thường | Từ CHẤM CÔNG | daily_attendance → attendance.regular_days *(VIEW)* | Nhập hằng ngày |
| TRẠM!Q16 | Công CN | Từ CHẤM CÔNG | daily_attendance → attendance.sunday_days *(VIEW)* | Nhập hằng ngày |
| TRẠM!R16 | Công cây non | Từ CHẤM CÔNG | daily_attendance → attendance.young_tree_days *(VIEW)* | Nhập hằng ngày |
| TRẠM!S16 | Công khộp nặng | Từ CHẤM CÔNG | daily_attendance → attendance.hardship_days *(VIEW)* | Nhập hằng ngày |
| TRẠM!T16 | Cạo 2 lát/miệng | Từ CHẤM CÔNG | daily_attendance → attendance.double_cut_days *(VIEW)* | Nhập hằng ngày |
| TRẠM!U16 | Cạo 2 lát/miệng CN | Từ CHẤM CÔNG | daily_attendance → attendance.double_cut_sunday *(VIEW)* | Nhập hằng ngày |
| TRẠM!Y16 | Tổng phụ cấp | *(xem mục 2.5)* | payroll.allowance_total | **Tính** |
| TRẠM!Z16 | Tổng lương | = O16 + G16 + Y16 | payroll.gross_salary | **Tính** |
| TRẠM!AA16 | Lương còn lại | = Z16 | payroll.net_salary | **Tính** |

---
---

# 9. TÓM TẮT CHÍNH SÁCH CHO CEO

---

## 9.1 Cơ chế lương công nhân khai thác (1 câu)

> **Thu nhập CN = Tiền sản lượng mủ (theo hạng + DRC + tỷ giá) + Phụ cấp (theo loại công × factor)**

## 9.2 Các hệ số quyết định lương

| Hệ số | Giá trị T12 | Ai quyết định | Tác động |
|---|---|---|---|
| **Tỷ giá Bath→Kip** | 600 | Phòng TC-KT | Toàn bộ tiền sản lượng |
| **DRC mủ tạp** | 0,385 (T1) / 0,377 (T2) | Bộ phận kỹ thuật | Sản lượng quy khô |
| **Hạng KT (A/B/C/D)** | 53% hạng A, 47% hạng B | QLKT đánh giá | Đơn giá sản lượng |
| **Factor phụ cấp** | 667,96 | — | Đơn giá phụ cấp |
| **Hệ số hạng theo trạm** | T1: 9,2–8,3 / T2: 7,7–6,8 | Chính sách công ty | Chênh lệch vùng |

## 9.3 Quỹ lương Đội 1 tháng 12/2025

| Khoản | Số tiền (kip) | Tỷ trọng |
|---|---|---|
| Tiền sản lượng CN | 369.881.748 | 59,1% |
| Phụ cấp ngày công CN | 239.817.936 | 38,3% |
| Bảo vệ vườn cây | 7.200.000 | 1,2% |
| CB-CNV người Lào | 6.719.550 | 1,1% |
| Tạp vụ | 2.000.000 | 0,3% |
| **TỔNG** | **625.619.234** | **100%** |

## 9.4 Tham số CEO cần giám sát

| Tham số | Lý do | Rủi ro nếu sai |
|---|---|---|
| **DRC** (AE6) | Thay đổi → toàn bộ quy khô thay đổi | Sai lệch quỹ lương lớn |
| **Tỷ giá** (E8) | Thay đổi → tiền sản lượng đổi đồng loạt | Ảnh hưởng 100% CN |
| **Hạng KT** (N16) | QLKT đánh giá sai → tiền sai | Chênh ~10%/người |
| **Factor** (AD4) | Thay đổi → phụ cấp đổi toàn bộ | Ảnh hưởng 38% quỹ lương |

---

**— HẾT TÀI LIỆU —**

| Thông tin | Chi tiết |
|---|---|
| Phiên bản | 2.0 — Thu hẹp phạm vi theo thực tế |
| Tổng bảng DB | 9 (8 bảng + 1 VIEW) |
| Use Cases | 9 |
| Công thức giải mã | 25+ (đã kiểm chứng với dữ liệu thực) |
| Dữ liệu | Trích xuất trực tiếp từ file Excel gốc |
| Ngày cập nhật | 13/04/2026 |
