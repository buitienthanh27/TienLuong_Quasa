# 02 — CẤU TRÚC FILE ĐỢT 2 (Quyết toán 16 → 31/01/2026)

File: `Copy of ĐỘI 1_BẢNG LƯƠNG_TRỰC TIẾP_THÁNG 01_2026_ĐỢT 2.xls - version 1.0. 3.25.2026 9.xlsx`
Số sheet: **11** (6 hiện + 5 ẩn)

## 1. Danh sách sheet

| # | Sheet | Kích thước | Trạng thái | Mục đích |
|---|---|---|---|---|
| 1 | `Tên thang 01-26` | 533×40 | ẩn | **Danh sách nhân viên** (như file Đợt 1) |
| 2 | `tên hang mục` | 38×3 | ẩn | Danh mục hạng mục công việc |
| 3 | `SAN LUONG` | 518×138 | ẩn | Sản lượng mủ chi tiết (mở rộng hơn Đợt 1) |
| 4 | `CHAM CONG` | 531×108 | ẩn | Chấm công chi tiết (mở rộng) |
| 5 | `PTL ` *(có dấu cách cuối tên)* | 41×9 | **hiện** | Bảng phân toán lương |
| 6 | `CĐCN` | 44×26 | **hiện** | Bảng cân đối công nợ cả tháng |
| 7 | `KL 1` | 542×226 | ẩn | **Bảng khối lượng mở rộng — 13+ cột quy đổi công** (KEY) |
| 8 | `KL 2` | 58×164 | ẩn | **Khối lượng CN bổ sung** (485-512), thuộc nhóm vườn kiến thiết / bảo vệ |
| 9 | `Sheet3` | 515×4 | ẩn | Nháp |
| 10 | `TTL 1` | 510×23 | **hiện** | **Thanh toán lương CN chính (1..484)** |
| 11 | `TTL 2` | 58×22 | **hiện** | **Thanh toán CN bổ sung (485..512) — 28 CN** |

## 2. Điểm khác biệt quan trọng so với Đợt 1

| Tiêu chí | Đợt 1 | Đợt 2 |
|---|---|---|
| Kỳ lương | 01 → 15/01 | 16 → 31/01 (nhưng **quyết toán cả tháng**) |
| Số hạng mục công việc trên `KL 1` | 5 (cạo 16k, D2 17k, xăng 1k, sản lượng 3 giá 455/505/555, học cạo úp 8k) | **13+** (thêm: chuyên cần, mủ dây, thưởng/phạt KT, mì+dầu, thiết kế mặt cạo, kiểm kê đầu vụ, PCCC + gác lửa, ...) |
| Bảng thanh toán | Chỉ `TTL 01` (484 CN) | `TTL 1` (484 CN chính) + `TTL 2` (28 CN bổ sung — bảo vệ, phụ trợ) |
| BHXH | Không | **Có — 2%**, trần 1.200.000 Riel |
| TNCN | Công thức biểu cũ (ít khi áp) | **Biểu mới**, áp trên (Tổng lương − lương T13 − BHXH) |
| Lương T13 | Không có cột | **Có cột riêng (J15)** |
| Thâm niên | Không có | **Có cột riêng (Q11 ở `TTL 1`)** |
| Sheet KL 1 bắt đầu dữ liệu | row **16** | row **17** |

## 3. Sheet `KL 1` (Đợt 2) — Bảng khối lượng & quy đổi công (MỞ RỘNG)

**Loại:** Sheet tính toán trung tâm. Dòng dữ liệu từ **row 17**. Row 15 chứa hệ số/đơn giá hardcode.

### 3.1 Cấu trúc cột (trích dẫn công thức mẫu dòng 17)

| Cột | Header | Công thức | Loại |
|---|---|---|---|
| A | STT | input | |
| B | `='Tên thang'!D4` | link | |
| C-D | Họ tên | link | |
| E | Phần cạo | input | |
| **F** | Sản lượng mủ **455 KHR/kg** (kg quy khô) | `='SAN LUONG'!AP9 × 0.44` | **formula (DRC 0,44)** |
| G | Công quy mủ 455 | `=ROUND((F17×$G$15)/31947, 2)` với `$G$15=455` | formula |
| **H** | Sản lượng mủ **505 KHR/kg** | `='SAN LUONG'!AO9 × 0.44` | formula |
| I | Công quy mủ 505 | `=ROUND((H17×$I$15)/31947, 2)` với `$I$15=505` | formula |
| **J** | Sản lượng mủ **555 KHR/kg** | `='SAN LUONG'!AV9 × 0.44` | formula |
| K | Công quy mủ 555 | `=ROUND((J17×$K$15)/31947, 2)` với `$K$15=555` | formula |
| **M** | Số ngày cạo mủ 16.000 Riel | `='CHAM CONG'!BC7` | link |
| N | Công quy cạo 16k | `=ROUND(M17×16000/31947, 2)` | formula |
| O | Công quy phụ cấp xăng | `=ROUND(M17×1000/31947, 2)` (dùng lại M17) | formula |
| **P** | Số ngày chuyên cần | `='CHAM CONG'!BO7` | link |
| Q | Công quy chuyên cần | `=ROUND(P17×0.0939, 2)` (**0,0939 ≈ 3.000/31.947**) | formula |
| **R** | Mủ dây (kg) | `='SAN LUONG'!DW9` | link |
| S | Công quy mủ dây | `=ROUND(R17×1000/31947, 2)` | formula |
| **T** | Thưởng/phạt kỹ thuật (Riel, **có thể âm**) | `='CHAM CONG'!BW7` | link |
| U | Công quy thưởng/phạt | `=ROUND(T17/31947, 2)` | formula |
| **V** | Số ngày cạo D2 (17k) | `='CHAM CONG'!BD7` | link |
| W | Công quy cạo D2 | `=ROUND(V17×$V$15, 2)` với `$V$15=0.5321` = 17000/31947 | formula |
| **X** | Phụ cấp mì + dầu (ngày) | `='CHAM CONG'!BT7` | link |
| Y | Công quy mì+dầu | `=ROUND(X17×1.0956, 1)` (**1,0956 ≈ 35.000/31.947**) | formula |
| **Z** | Thiết kế mặt cạo (ngày) | hardcode `6.03` từng CN | **input** |
| AA | Công quy thiết kế MC | `=ROUND(Z17×$Z$15, 2)+0.01` với `$Z$15=0.2827` (≈9.031/31.947) | formula **(+0.01!)** |
| **AB** | Kiểm kê cây cao su đầu vụ (ngày) | hardcode `6.03` | input |
| AC | Công quy kiểm kê | `=ROUND(AB17×$AB$15, 2)` với `$AB$15=1.2` | formula |
| **AD** | PCCC + gác lửa (ngày) | hardcode `6.03` | input |
| AE | Công quy PCCC | `=ROUND(AD17×$AD$15, 2)` với `$AD$15=0.7128` (≈22.772/31.947) | formula |
| AF | (Có công thức `#REF!` — lỗi) | `=ROUND(#REF!*#REF!, 2)` | **lỗi** |
| **AG** | **TỔNG CÔNG** | `=G17+I17+K17+N17+Q17+S17+U17+W17+Y17+AC17+AE17+AA17+O17` | **formula KEY** |

### 3.2 Row 15 — Hằng số hardcode (đơn giá quy đổi công)

| Ô | Giá trị | Ý nghĩa |
|---|---|---|
| `$G$15` | 455 | Đơn giá mủ loại 1 (KHR/kg) |
| `$I$15` | 505 | Đơn giá mủ loại 2 |
| `$K$15` | 555 | Đơn giá mủ loại 3 |
| `$O$15` | 1.000 | Đơn giá phụ cấp xăng |
| `$S$15` | 1.000 | Đơn giá mủ dây |
| `$V$15` | 0,5321 | Định mức công cạo D2 (17.000/31.947) |
| `$Z$15` | 0,2827 | Định mức công thiết kế MC (≈9.031/31.947) |
| `$AB$15` | 1,2 | Định mức công kiểm kê đầu vụ (cao — công việc khó) |
| `$AD$15` | 0,7128 | Định mức công PCCC + gác lửa (≈22.772/31.947) |

## 4. Sheet `KL 2` (Đợt 2) — CN bổ sung (485..512)

**Loại:** Khối lượng của 28 CN nhóm **vườn kiến thiết (VƯỜN CÂY 2011)** + **Bảo vệ**.
- Dòng dữ liệu từ row 17. Đơn giá khác: `H15=450` (mủ), `T15=1.4661` (kiểm kê cuối năm), `W15=2` (bảo vệ), `X15=1.0955`, `AA15=1.4661`.
- `G17 = ROUND(H17/30131, 2)` (đơn giá khác: **30.131**, không phải 31.947 — cần làm rõ).
- `H17 = ROUND(F17 × 44% × 450, 0)` — tiền mủ.
- `I17 = 31 ngày`, `J17 = ROUND(I17 × 1.252, 2)` — công bảo vệ thường xuyên (1,252 công/ngày).
- `AB17 = J17+M17+S17+W17+Y17+O17+L17` — **Tổng công** của CN bổ sung.

## 5. Sheet `TTL 1` (Đợt 2) — Thanh toán lương CN chính

**Loại:** Bảng thanh toán quyết toán tháng, in ký.

| Cột | Header | Công thức | Loại |
|---|---|---|---|
| A | STT | input | |
| B-D | Họ tên, Phần cạo | link `Tên thang 01-26` | |
| E | Định mức | `31947` | hằng số |
| F | Công đợt 1 | `=ROUND(G15/31947, 2)` | formula (suy ngược) |
| **G** | **Lương đợt 1** | hardcode `345028` *(không công thức!)* | **input cứng** |
| H | Công đợt 2 | `='KL 1'!AG17` | link |
| I | Lương đợt 2 | `=ROUND(E15×H15, 0)` | formula |
| J | Tiền lương tháng 13 | input (thường = 0) | input |
| K | (gộp T13+thâm niên nhập tay) | input | input |
| **L** | **TỔNG LƯƠNG** | `=G15+I15+K15+J15` | formula |
| M | Công nợ (lương đợt 1 đã nhận) | `=G15` | formula |
| N | BHXH 2% | biểu ngưỡng (xem doc 03) | formula |
| O | TNCN | biểu lũy tiến (xem doc 03) | formula |
| P | Tổng nợ | `=SUM(M15:O15)` | formula |
| Q | Thâm niên | input | input |
| **R** | **Thực nhận đợt 2** | `=L15-P15` | formula |
| S | Ký nhận | — | |

## 6. Sheet `TTL 2` (Đợt 2) — CN bổ sung (485..512)

Giống `TTL 1` nhưng:
- Cột `G` (Công đợt 2) link sang `'KL 2'!AB17..`.
- Cột `R` (không phải `R15` như `TTL 1`) chứa lương hardcode từng CN (`R15=1608205`...).
- Ít cột hơn (không có cột thâm niên riêng).

## 7. Sheet `PTL` (có khoảng trắng ở cuối tên) — Phân toán

Khác `PTL T01-26` Đợt 1: **chia 3 nhóm** A/B/C rõ ràng.

| Nhóm | Row | Hạng mục | Công (row) | Công (row 15=A, 18=B, 22=C) |
|---|---|---|---|---|
| **A** Chăm sóc | 15-17 | Phát cỏ luồng (16), PCCC + gác lửa (17) | 383,1 + 2.080,6 = 2.463,7 | 78.707.824 Riel |
| **B** Thu hoạch | 18-21 | Thiết kế MC (19) 825,2 ; Kiểm kê (20) 3.502,6 ; Cạo mủ (21) 12.222,3 | 16.550,1 | 528.726.044 Riel |
| **C** Bảo vệ | 22-23 | Bảo vệ cố định | 414,0 (hardcode!) | 13.226.058 Riel |

- `I14 = ROUND(D14 × 30456, 0)` — quy đổi tổng công × **30.456** (tỷ giá?).

## 8. Sheet `CĐCN` — Cân đối công nợ cả tháng

Khác Đợt 1: có **4 khoản lương** (đợt 1, đợt 2, T13, thưởng đầu vụ) và chi tiết khấu trừ.

- `G10 = G11+G12+G13+G14` — tổng lương cả tháng.
- `G11 = J11 = 'TTL 2'!V45` — lương đợt 1.
- `G12 = 'PTL '!E24` — lương đợt 2.
- `G18 = G19+G20+G21` — tổng phải thu (TNCN + tạm ứng + khác).
- `J11..J18` — các chỉ số phụ đối chiếu từ `TTL 2`.

## 9. Các lỗi / điểm yếu phát hiện

1. Cột `AF` sheet `KL 1` chứa `=ROUND(#REF!*#REF!, 2)` ở nhiều dòng — tham chiếu gãy.
2. Cột `G` (Lương đợt 1) ở `TTL 1` là **số cứng hardcode** cho từng CN — không link về file Đợt 1. Rủi ro sai lệch rất cao.
3. Biểu TNCN ở `O15` thiếu nhánh cho TN > 8,5M → trả `FALSE`.
4. Tên sheet `PTL ` có dấu cách cuối — dễ lỗi khi đọc file tự động.
5. `KL 2` dùng định mức **30.131** thay vì **31.947** — cần xác nhận.
