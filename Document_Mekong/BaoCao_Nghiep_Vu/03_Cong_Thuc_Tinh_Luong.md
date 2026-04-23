# 03 — CÔNG THỨC TÍNH LƯƠNG (Quy tắc nghiệp vụ)

Tài liệu này liệt kê **toàn bộ công thức Excel có ý nghĩa nghiệp vụ** trong 2 file, kèm giải thích bằng tiếng Việt để chuyển hoá trực tiếp sang phần mềm.

## 0. Nguyên lý cốt lõi

> **Mọi hạng mục công việc đều được quy đổi về đơn vị "công"**, sau đó **Tổng tiền = Tổng công × 31.947 Riel/công**.
> 31.947 Riel/công là **định mức toàn hệ thống** (hardcode ở `$H$15` của `TTL 01`, `$E$15` của `TTL 1`...).

Công thức quy đổi tổng quát:
```
Công_hạng_mục = ROUND( Khối_lượng × Đơn_giá_hạng_mục / 31947 , 2 )
```

---

## 1. Lương cơ bản (theo công cạo mủ)

### 1.1 Cạo mủ thường — đơn giá 16.000 Riel/ngày

| Ô | File/Sheet | Công thức Excel | Diễn giải |
|---|---|---|---|
| `KL 1!Q16` | Đợt 1 | `=ROUND(P16 × $Q$14/31947, 2)` (Q14=16000) | **Công quy đổi** = Số ngày cạo × 16.000 / 31.947 |
| `KL 1!N17` | Đợt 2 | `=ROUND(M17 × 16000/31947, 2)` | Tương tự |
| `tổng hợp cty!G14` | Đợt 1 | `=ROUND(F14 × D14, 1)` với D14=0,5008 | Hệ số **0,5008** = 16.000/31.947 (hardcode giảm phép chia) |

**Nghiệp vụ:** Mỗi ngày CN đi cạo mủ thường được tính 16.000 Riel. Trong hệ thống "công", 1 ngày cạo = **0,5008 công**.

### 1.2 Cạo D2 — đơn giá 17.000 Riel/ngày

| Ô | File | Công thức | Diễn giải |
|---|---|---|---|
| `KL 1!O16` | Đợt 1 | `=ROUND(N16 × $O$14/31947, 2)` (O14=17000) | Công D2 = Số ngày × 17.000/31.947 |
| `KL 1!W17` | Đợt 2 | `=ROUND(V17 × $V$15, 2)` với $V$15=0,5321 | **0,5321** = 17.000/31.947 |

**Nghiệp vụ:** Cạo D2 (kỹ thuật cao hơn, 2 lát/miệng?) — 17.000 Riel/ngày = **0,5321 công/ngày**.

### 1.3 Học cạo úp — đơn giá 8.000 Riel/ngày (chỉ Đợt 1)

| Ô | Công thức | Diễn giải |
|---|---|---|
| `KL 1!U16` Đợt 1 | `=ROUND(T16 × $U$14/31947, 2)` (U14=8000) | CN đang đào tạo — trả nửa giá |

---

## 2. Lương theo sản lượng mủ (thu hoạch)

### 2.1 Mủ đông 3 cấp chất lượng

| Cấp | Đơn giá | Đợt 1 (`KL 1`) | Đợt 2 (`KL 1`) |
|---|---|---|---|
| **455 KHR/kg** (cao) | 455 | `I16 = ROUND(H16×$I$14/31947, 2)` | `G17 = ROUND((F17×$G$15)/31947, 2)` |
| **505 KHR/kg** (trung bình) | 505 | `K16 = ROUND(J16×$K$14/31947, 2)` | `I17 = ROUND((H17×$I$15)/31947, 2)` |
| **555 KHR/kg** (thấp) | 555 | `M16 = ROUND(L16×$M$14/31947, 2)` | `K17 = ROUND((J17×$K$15)/31947, 2)` |

**Đặc biệt ở Đợt 2** — có **hệ số 0,44 (DRC = tỷ lệ mủ khô)**:
```
F17 = 'SAN LUONG'!AP9 × 0.44   (quy từ mủ tươi sang mủ đông)
```

Ở sheet `tổng hợp cty` Đợt 1:
```
F17 = ROUND(J17 × 0.44, 0)     (J17 là mủ tươi, F17 là mủ đông đã quy)
```

**Diễn giải:**
```
Công mủ loại X = ROUND(Khối_lượng_mủ_đông × giá_X / 31947 , 2)
Khối_lượng_mủ_đông = Khối_lượng_mủ_tươi × DRC_0.44
```

### 2.2 Mủ dây (Đợt 2 — hạng mục riêng)

| Ô | Công thức | Diễn giải |
|---|---|---|
| `KL 1!S17` Đợt 2 | `=ROUND(R17 × 1000/31947, 2)` | Mủ dây giá 1.000 Riel/kg |

---

## 3. Phụ cấp

### 3.1 Phụ cấp xăng — 1.000 Riel/ngày

| Ô | Công thức | Diễn giải |
|---|---|---|
| `KL 1!S16` Đợt 1 | `=ROUND(R16 × $S$14/31947, 2)` (S14=1000); `R16=P16` | Tính theo số ngày đi cạo |
| `KL 1!O17` Đợt 2 | `=ROUND(M17 × 1000/31947, 2)` | Dùng lại số ngày cạo (M17) |

### 3.2 Phụ cấp mì + dầu (Đợt 2) — 35.000 Riel/ngày

| Ô | Công thức | Diễn giải |
|---|---|---|
| `KL 1!Y17` Đợt 2 | `=ROUND(X17 × 1.0956, 1)` | **1,0956 ≈ 35.000/31.947** — đơn giá mì+dầu/ngày |

### 3.3 Chuyên cần (Đợt 2) — 3.000 Riel/ngày

| Ô | Công thức | Diễn giải |
|---|---|---|
| `KL 1!Q17` Đợt 2 | `=ROUND(P17 × 0.0939, 2)` | **0,0939 ≈ 3.000/31.947** — thưởng chuyên cần/ngày |

> **Khuyến nghị:** Lẽ ra nên viết `=ROUND(P17×3000/31947, 2)` cho rõ nghĩa, không nên hardcode 0,0939.

---

## 4. Công việc chăm sóc vườn cây (chỉ Đợt 2)

### 4.1 Thiết kế mặt cạo (9.031 Riel/ngày)
```
KL 1!AA17 = ROUND(Z17 × $Z$15, 2) + 0.01     ; $Z$15 = 0,2827 ≈ 9.031/31.947
```
**Lưu ý:** công thức có **+0,01** — có thể là hiệu chỉnh làm tròn, cần xác nhận.

### 4.2 Kiểm kê cây cao su đầu vụ (≈38.336 Riel/ngày)
```
KL 1!AC17 = ROUND(AB17 × $AB$15, 2)           ; $AB$15 = 1,2
```
Hệ số 1,2 cao — do công việc khó, đòi hỏi kỹ năng.

### 4.3 PCCC + gác lửa (22.772 Riel/ngày)
```
KL 1!AE17 = ROUND(AD17 × $AD$15, 2)           ; $AD$15 = 0,7128 ≈ 22.772/31.947
```

### 4.4 Phát cỏ luồng (trên `PTL`)
```
PTL!E16 Đợt 2 = ROUND(D16 × C16, 0)           ; D16=383,1 công ; C16=31947
```

### 4.5 Bảo vệ (cố định)
```
PTL!D23 Đợt 2 = 414    (hardcode, không công thức)
PTL!E23 = 414 × 31947 = 13.226.058 Riel
```
**Nghiệp vụ:** Tổ bảo vệ không chấm công hàng ngày — cấp cố định 414 công/tháng. **Cần bổ sung cơ chế chấm công bảo vệ trong phần mềm.**

---

## 5. Thưởng / phạt kỹ thuật (Đợt 2)

```
KL 1!T17 (Đợt 2) = 'CHAM CONG'!BW7          ; nhập trực tiếp Riel, CÓ THỂ ÂM
KL 1!U17         = ROUND(T17/31947, 2)      ; quy sang công (âm/dương)
```
**Nghiệp vụ:** Giám sát kỹ thuật ghi trực tiếp tiền thưởng hoặc phạt (số âm) vào bảng chấm công, sau đó sẽ được quy đổi ra công âm/dương.

---

## 6. Tổng công của 1 CN

### Đợt 1 — `KL 1!X16`
```
X16 = I16 + K16 + M16 + Q16 + S16 + O16
    = (mủ 455) + (mủ 505) + (mủ 555) + (cạo thường) + (xăng) + (cạo D2)
```
(Đợt 1 chưa gồm học cạo úp `U16` — `U16` có cột tiền riêng `V16 = ROUND(U16×30456, 0)` để báo cáo cty mẹ VN.)

### Đợt 2 — `KL 1!AG17`
```
AG17 = G17 + I17 + K17 + N17 + Q17 + S17 + U17 + W17 + Y17 + AA17 + AC17 + AE17 + O17
     = mủ455 + mủ505 + mủ555 + cạo16k + chuyên_cần + mủ_dây + thưởng/phạt
       + cạoD2 + mì_dầu + thiết_kế_MC + kiểm_kê + PCCC + xăng
```

### Đợt 2 CN bổ sung — `KL 2!AB17`
```
AB17 = J17 + M17 + S17 + W17 + Y17 + O17 + L17
     (công bảo vệ thường xuyên + các hạng mục kiến thiết)
```

---

## 7. Thành tiền lương từng CN

### 7.1 Đợt 1 — `TTL 01!J15`
```
J15 = ROUND( H15 × I15 , 0 )
    = ROUND( 31947 × Tổng_công_đợt_1 , 0 )
```

### 7.2 Đợt 2 — `TTL 1!I15`
```
I15 = ROUND( E15 × H15 , 0 )
    = ROUND( 31947 × Tổng_công_đợt_2 , 0 )
```

### 7.3 Tổng lương cả tháng — `TTL 1!L15`
```
L15 = G15 + I15 + K15 + J15
    = Lương_đợt_1 + Lương_đợt_2 + (T13+Thâm_niên) + T13_riêng
```
**Cảnh báo:** `G15` hiện là **hardcode** — cần link về Đợt 1 trong phần mềm.

---

## 8. Công nợ (lương đã ứng Đợt 1)

### Trong Đợt 2 — `TTL 1!M15`
```
M15 = G15      (bằng đúng Lương đợt 1)
```
**Nghiệp vụ:** Toàn bộ tiền đã nhận ở Đợt 1 được coi là "nợ" và trừ lại ở Đợt 2.

---

## 9. BHXH — chỉ khấu trừ ở Đợt 2

### Công thức — `TTL 1!N15` và `TTL 2!M15`
```
N15 = IF( L15 <= 400000 ,  400000 * 2% ,
      IF( L15 <= 1200000 , ROUND(L15 * 2%, 0) ,
                           ROUND(1200000 * 2%, 0) ) )
```

| Ngưỡng tổng lương tháng | BHXH phải đóng |
|---|---|
| ≤ 400.000 Riel | 400.000 × 2% = **8.000 Riel** (sàn) |
| 400.000 → 1.200.000 Riel | Lương × 2% |
| > 1.200.000 Riel | 1.200.000 × 2% = **24.000 Riel** (trần) |

**Nghiệp vụ:** Tỷ lệ 2% của CN, có sàn và trần. BHXH **không phát sinh ở Đợt 1**.

---

## 10. Thuế TNCN

### 10.1 Đợt 1 (biểu cũ) — `TTL 01!O15`
```
O15 = ROUND( IF( M15 <= 1200000 , 0 ,
              IF( AND(M15>1200000, M15<=2000000) , M15*5% - 60000 ,
                IF( M15 <= 8500000 , M15*10% - 160000 ))) , 0)
```

| Ngưỡng TN | Thuế |
|---|---|
| ≤ 1.200.000 | 0 |
| 1,2M → 2M | TN × 5% − 60.000 |
| 2M → 8,5M | TN × 10% − 160.000 |
| > 8,5M | (thiếu nhánh — trả FALSE) |

> Thực tế ở Đợt 1, toàn bộ được coi là ứng, thuế **không thu ở đây** (công thức tồn tại nhưng không áp dụng nghiệp vụ — cần xoá khi sang phần mềm).

### 10.2 Đợt 2 (biểu mới) — `TTL 1!O15`
```
O15 = ROUND( IF( (L15 - (K15 + N15)) <= 1500000 , 0 ,
              IF( ... (L15-K15-N15)*5% - 75000 ,
                IF( ... *10% - 175000 ))) , 0)
```

**Thu nhập tính thuế** = `L15 − K15 − N15` = **Tổng lương − Lương T13 − BHXH**.

| Ngưỡng TN tính thuế | Thuế |
|---|---|
| ≤ 1.500.000 | 0 |
| 1,5M → 2M | TN × 5% − 75.000 |
| 2M → 8,5M | TN × 10% − 175.000 |
| **> 8,5M** | **(thiếu nhánh!) ⚠** — sẽ trả FALSE |

**Khuyến nghị phần mềm:**
1. Bổ sung nhánh >8,5M: TN × 15% − 525.000 (Cambodia chuẩn) và >12,5M: TN × 20% − 1.150.000.
2. Tham số hoá toàn bộ biểu thuế theo năm hiệu lực.

---

## 11. Tổng khấu trừ & Thực nhận

### Đợt 1 — `TTL 01!P15` và `Q15`
```
P15 (Tổng nợ)  = N15 + O15        = 0 + 0 (thực tế)
Q15 (Thực nhận) = M15 - L15        = Lương đợt 1 (vì L15=0)
```

### Đợt 2 — `TTL 1!P15` và `R15`
```
P15 (Tổng nợ)    = SUM(M15:O15)   = Lương_đợt_1_đã_nhận + BHXH + TNCN
R15 (Thực nhận)  = L15 - P15       = Tổng_lương_cả_tháng − (đã_ứng + BHXH + TNCN)
```

---

## 12. Báo cáo công ty mẹ — tỷ giá 30.456

```
KL 1!V16 (Đợt 1) = ROUND(U16 × 30456, 0)
PTL!I14  (Đợt 2) = ROUND(D14 × 30456, 0)
```
**Giả định:** 30.456 là tỷ giá quy đổi Riel → đơn vị báo cáo (VND hoặc USD) cho công ty mẹ Việt Nam. **Cần xác nhận.**

---

## 13. Bảng tóm tắt hằng số hardcode (cho cấu hình phần mềm)

| Nhóm | Hằng số | Giá trị | Đơn vị | Nguồn Excel |
|---|---|---|---|---|
| Định mức | Đơn giá 1 công | **31.947** | Riel/công | `TTL 01!H15`, `TTL 1!E15`, `PTL!C14..C23` |
| Đơn giá mủ đông | Loại cao | 455 | KHR/kg | `KL 1!$I$14` Đợt 1 / `$G$15` Đợt 2 |
| Đơn giá mủ đông | Loại TB | 505 | KHR/kg | `$K$14` / `$I$15` |
| Đơn giá mủ đông | Loại thấp | 555 | KHR/kg | `$M$14` / `$K$15` |
| Đơn giá công | Cạo thường | 16.000 | Riel/ngày | `$Q$14` Đợt 1 |
| Đơn giá công | Cạo D2 | 17.000 | Riel/ngày | `$O$14` Đợt 1 |
| Đơn giá công | Học cạo úp | 8.000 | Riel/ngày | `$U$14` Đợt 1 |
| Phụ cấp | Xăng | 1.000 | Riel/ngày | `$S$14` / `$O$15` |
| Phụ cấp | Mì + dầu | 35.000 | Riel/ngày | (hệ số 1,0956) |
| Phụ cấp | Chuyên cần | 3.000 | Riel/ngày | (hệ số 0,0939) |
| Phụ cấp | Mủ dây | 1.000 | Riel/kg | `$S$15` |
| Định mức | Thiết kế MC | 9.031 | Riel/ngày | (hệ số 0,2827) |
| Định mức | Kiểm kê | 38.336 | Riel/ngày | (hệ số 1,2) |
| Định mức | PCCC + gác lửa | 22.772 | Riel/ngày | (hệ số 0,7128) |
| Định mức | Bảo vệ (cố định) | 414 | công/tháng/đội | `PTL!D23` |
| Chuyển đổi | DRC (mủ tươi → mủ đông) | 0,44 (44%) | — | `F17 = ...×0.44` |
| Tỷ giá | Báo cáo cty mẹ | 30.456 | Riel/VND? | `V16`, `I14` |
| BHXH | Tỷ lệ | 2% | — | `N15` Đợt 2 |
| BHXH | Sàn | 400.000 | Riel | |
| BHXH | Trần | 1.200.000 | Riel | |
| TNCN Đợt 1 | Ngưỡng 1 / 2 / 3 | 1,2M / 2M / 8,5M | Riel | `O15` Đợt 1 |
| TNCN Đợt 2 | Ngưỡng 1 / 2 / 3 | 1,5M / 2M / 8,5M | Riel | `O15` Đợt 2 |
| TNCN Đợt 1 | Hằng trừ | 60k / 160k | Riel | |
| TNCN Đợt 2 | Hằng trừ | 75k / 175k | Riel | |

---

## 14. Cần làm rõ (công thức mơ hồ)

- `AA17 = ROUND(Z17×$Z$15, 2) + 0.01` — tại sao cộng thêm 0,01? Làm tròn hay điều chỉnh thủ công?
- `KL 2!G17 = ROUND(H17/30131, 2)` — tại sao dùng **30.131** thay vì 31.947?
- `KL 2!AE17 = AB17*31226` — số 31.226 là gì?
- `AF` (KL 1 Đợt 2) chứa `#REF!` — cột này làm gì và tổng `AG17` có thiếu cột này không?
- Cột `K15` trong `TTL 1` được gộp "T13 + thâm niên nhập tay" — logic phân tách giữa `J15` (T13 cột riêng), `K15` và `Q15` (thâm niên) chưa rõ, cần khách chốt.
