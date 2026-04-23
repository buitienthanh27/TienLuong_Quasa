# 01 — CẤU TRÚC FILE ĐỢT 1 (Tạm ứng 01 → 15/01/2026)

File: `Copy_ĐỘI 1_BẢNG LƯƠNG_TRỰC TIẾP_THÁNG 01_2026_ĐỢT 1.xlsx`
Số sheet: **16** (6 hiện + 10 ẩn)

## 1. Danh sách sheet

| # | Sheet | Kích thước | Trạng thái | Mục đích |
|---|---|---|---|---|
| 1 | `SL KHOÁNG T10 NT` | 48×16 | ẩn | Dữ liệu sản lượng khoán tháng trước — phụ trợ |
| 2 | `ct thuc te` | 205×252 | ẩn | Chấm công thực tế chi tiết (rất rộng) |
| 3 | `TÊN HÀNG MỤC` | 38×3 | ẩn | Danh mục hạng mục công việc (cạo, chăm sóc, thu hoạch, bảo vệ...) |
| 4 | `Tên Tháng 1` | 580×22 | ẩn | **Danh sách nhân viên** của tháng (Mã CMND, Họ tên, Năm sinh, Phần cạo) |
| 5 | `CHAM CONG` | 515×63 | ẩn | **Bảng chấm công** chi tiết theo ngày cho từng CN |
| 6 | `SAN LUONG` | 525×47 | ẩn | **Bảng cân sản lượng mủ** hàng ngày theo CN |
| 7 | `GA 1` | 192×233 | ẩn | Giao ca 1 — dữ liệu giao nhận mủ |
| 8 | `GA 2` | 158×53 | ẩn | Giao ca 2 |
| 9 | `GA 3` | 108×226 | ẩn | Giao ca 3 |
| 10 | `GA 4` | 121×227 | ẩn | Giao ca 4 |
| 11 | `tổng hợp cty` | 30×14 | **hiện** | Báo cáo tổng hợp cho công ty mẹ |
| 12 | `PTL T01-26` | 48×9 | **hiện** | **Bảng phân toán lương** (Phân bổ quỹ lương theo hạng mục) |
| 13 | `CĐCN T01-26` | 42×26 | **hiện** | **Bảng cân đối công nợ** đợt 1 |
| 14 | `KL 1` | 509×216 | ẩn | **Bảng khối lượng** — quy đổi về "công" từng CN |
| 15 | `Sheet1` | 486×9 | ẩn | Nháp |
| 16 | `TTL 01` | 509×23 | **hiện** | **Bảng thanh toán lương** cho từng CN (có chữ ký) |

## 2. Sheet `Tên Tháng 1` — Danh mục nhân viên

**Loại:** Master data — input (nhập tay / import một lần).

| Cột | Tên | Giải thích |
|---|---|---|
| A | STT | Số thứ tự |
| B | Năm sinh (Nam) | Năm sinh nếu là công nhân nam |
| C | Năm sinh (Nữ) | Năm sinh nếu là công nhân nữ |
| D | CMND | Số CMND (khoá định danh CN) |
| E, F | Họ và tên | Tên CN (Khmer + La-tinh) |
| G | Phần cạo | Mã phần/lô cạo được giao (1..n) — dùng phân nhóm |

**Nguồn:** Nhập tay. **Không có công thức**. Các sheet khác tham chiếu về đây bằng `='Tên Tháng 1'!E4`...

## 3. Sheet `CHAM CONG` — Bảng chấm công (input)

**Loại:** Dữ liệu thô — nhập hàng ngày.
- Hàng 1-7: header (ngày 01 → 15 tháng 01).
- Cột A-G: Mã CN, họ tên.
- Cột H → BK (63 cột): chấm công theo từng ngày × từng loại công việc (cạo, D2, phát cỏ, PCCC, chuyên cần...).
- Cột tổng hợp cuối: `AQ` (cạo 16k), `AR` (cạo D2 17k), ... dùng làm nguồn cho `KL 1`.

## 4. Sheet `SAN LUONG` — Bảng sản lượng mủ (input)

**Loại:** Dữ liệu thô — nhập sau mỗi ngày cân mủ.
- Mỗi dòng = 1 CN. Cột là các ngày trong kỳ × các hạng mục mủ.
- Các cột tổng: `AE` (sản lượng mủ đông — quy KHR?), `AK505/506/507` = tổng sản lượng quy 3 mức giá (455/505/555).

## 5. Sheet `KL 1` — Bảng khối lượng & quy đổi công (tính toán)

**Loại:** Sheet tính toán chính của Đợt 1. Dòng dữ liệu bắt đầu từ **row 16**.

| Cột | Header (VI) | Nguồn | Loại |
|---|---|---|---|
| A | STT | gán tay | input |
| B-F | Năm sinh / CMND / Họ tên | `='Tên Tháng 1'!…` | **link** |
| G | Phần cạo | input | input |
| H | Sản lượng mủ 455 KHR/kg (kg) | input/link `SAN LUONG` | input |
| I | Công quy đổi từ mủ 455 | `=ROUND(H16×$I$14/31947, 2)` | **formula** |
| J | Sản lượng mủ 505 | input | input |
| K | Công quy đổi từ mủ 505 | `=ROUND(J16×$K$14/31947, 2)` | **formula** |
| L | Sản lượng mủ 555 | `='SAN LUONG'!AE9` | **link** |
| M | Công quy đổi từ mủ 555 | `=ROUND(L16×$M$14/31947, 2)` | **formula** |
| N | Ngày cạo D2 (17k) | `='CHAM CONG'!AR8` | **link** |
| O | Công quy đổi cạo D2 | `=ROUND(N16×$O$14/31947, 2)` | **formula** |
| P | Ngày cạo thường (16k) | `='CHAM CONG'!AQ8` | **link** |
| Q | Công quy đổi cạo thường | `=ROUND(P16×$Q$14/31947, 2)` | **formula** |
| R | Ngày phụ cấp xăng | `=P16` (bằng ngày cạo thường) | **formula** |
| S | Công quy phụ cấp xăng | `=ROUND(R16×$S$14/31947, 2)` | **formula** |
| T | Ngày học cạo úp | input | input |
| U | Công quy học cạo úp | `=ROUND(T16×$U$14/31947, 2)` | **formula** |
| V | Tiền quy ra VND? | `=ROUND(U16×30456, 0)` | **formula (tỷ giá?)** |
| W | Tổng tiền | tổng X × 31947 | **formula** |
| **X** | **Tổng công** | `=I16+K16+M16+Q16+S16+O16` | **formula KEY** |
| Y | Chữ ký nhận | — | input (giấy) |

**Row 14 — Hằng số hardcode (đơn giá):**
`I14=455`, `K14=505`, `M14=555`, `O14=17000`, `Q14=16000`, `S14=1000`, `U14=8000`.

## 6. Sheet `TTL 01` — Bảng thanh toán lương Đợt 1

**Loại:** Bảng đầu ra chính — in ra cho CN ký nhận.

| Cột | Header | Công thức (dòng 15 làm ví dụ) | Ý nghĩa |
|---|---|---|---|
| A | STT | input | |
| B-G | Họ tên, CMND, phần cạo | link `KL 1` / `Tên Tháng 1` | |
| H | Định mức | `31947` (hardcode) | Đơn giá 1 công |
| I | Công đợt 1 | `='KL 1'!X16` | Tổng công đợt 1 |
| J | Thành tiền đợt 1 | `=ROUND(H15×I15, 0)` | **Lương đợt 1** |
| K | Công đợt 2 | bỏ trống ở Đợt 1 | |
| L | Thành tiền đợt 2 | bỏ trống ở Đợt 1 | |
| M | Tổng lương | `=J15+L15` (ở Đợt 1 = J15) | |
| N | Công nợ (lương đã nhận đợt 1) | `=L15` (ở Đợt 1 = 0) | |
| O | TNCN | biểu cũ (xem doc 03) — thực tế = 0 | |
| P | Tổng nợ | `=N15+O15` | |
| Q | Thực nhận | `=M15-L15` | **Thực nhận đợt 1 = J15** |
| R | Ký nhận | — | input giấy |
| S-V | GA / đối chiếu | input | |

## 7. Sheet `PTL T01-26` — Bảng phân toán lương

**Loại:** Báo cáo tổng hợp — phân bổ quỹ lương theo hạng mục công việc.
- Row 14: nhóm `VƯỜN CÂY KINH DOANH NHÓM I`.
- Row 15-21: chi tiết **Cạo, trút và giao nộp mủ** (nhóm B — Thu hoạch).
- Cột C: Đơn giá = 31.947 (tất cả dòng).
- Cột D: Công (tham chiếu `tổng hợp cty!G...`).
- Cột E: Thành tiền `=ROUND(C×D, 0)`.
- Cột F: Tổng cộng.

## 8. Sheet `CĐCN T01-26` — Bảng cân đối công nợ

**Loại:** Báo cáo thu chi quỹ lương đợt 1.
- `G11 = 'PTL T01-26'!E22` — Tổng lương đợt 1.
- `G12 = 0` — Đợt 2 chưa có.
- `G14 = G12` — Đã nhận đợt 1 (ở Đợt 1 = 0, vì chưa chi).
- `G16, G17` — TNCN, tạm ứng cá nhân (Đợt 1 = 0).
- `G18 = G10 - G13 - G15` — Thực nhận cần chi đợt 1.

## 9. Sheet `tổng hợp cty` — Báo cáo cho công ty mẹ

**Loại:** Báo cáo tổng.

| Dòng | Hạng mục | Công thức |
|---|---|---|
| A13-B13 | Cạo, trút và giao nộp mủ | `G13 = SUM(G14:G19)` |
| F14 | Số ngày cạo 16k (toàn đội) | `='CHAM CONG'!AQ500` |
| G14 | Công quy | `=ROUND(F14×D14, 1)` với `D14=0.5008` = 16000/31947 |
| F15 | Số ngày cạo D2 (toàn đội) | `='CHAM CONG'!AR500` |
| G15 | Công quy | `=ROUND(F15×D15, 1)` với `D15=0.5321` = 17000/31947 |
| F17-F19 | Sản lượng mủ đông (kg) 3 mức | `=ROUND(J17×0.44, 0)` (0.44 = DRC, tỷ lệ mủ khô) |
| J17-J19 | Sản lượng mủ tươi (kg) | link `SAN LUONG` |
| G17-G19 | Công quy theo giá 455/505/555 | `=ROUND((F×D)/31947, 1)` |

**Hệ số đặc biệt:** `0.44` = DRC (Dry Rubber Content) — quy đổi mủ tươi → mủ đông.

## 10. Tính chất Đợt 1

- Chỉ **1 loại công việc chính**: Cạo, trút và giao nộp mủ. Tất cả CN đều thuộc nhóm cạo.
- **Không trừ BHXH**. Không trừ TNCN thực sự.
- Là khoản **tạm ứng giữa kỳ** — sẽ trừ hết ở Đợt 2.
- Không có bảng thâm niên, tháng 13, kiểm kê, PCCC, thiết kế mặt cạo — các khoản đó dồn về Đợt 2.
