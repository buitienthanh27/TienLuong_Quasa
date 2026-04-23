# 04 — QUY TRÌNH NGHIỆP VỤ TÍNH LƯƠNG ĐỘI TRỰC TIẾP

## 1. Dòng dữ liệu tổng thể

```
┌─────────────────────────────────────────────────────────────────┐
│                      1 KỲ LƯƠNG = 1 THÁNG                        │
│                                                                   │
│   ┌─────────┐                              ┌─────────┐            │
│   │ NỬA ĐẦU │  (01 → 15)                   │ NỬA SAU │ (16 → 31) │
│   └────┬────┘                              └────┬────┘            │
│        │                                         │                │
│        ▼                                         ▼                │
│   [Chấm công ngày]                         [Chấm công ngày]      │
│   [Cân mủ ngày]                            [Cân mủ ngày]         │
│        │                                         │                │
│        ▼                                         │                │
│   [Đợt 1 — tạm ứng]                              │                │
│   Tính lương sơ bộ                               │                │
│   Không trừ BHXH/TNCN ────────┐                  │                │
│   Chi ứng cho CN              │                  │                │
│                               │                  ▼                │
│                               │           [Đợt 2 — quyết toán]   │
│                               │           Tính lương cả tháng    │
│                               └───────▶ Trừ Lương đã ứng Đ1     │
│                                          Trừ BHXH 2%             │
│                                          Trừ TNCN lũy tiến       │
│                                          → Thực nhận Đợt 2       │
└─────────────────────────────────────────────────────────────────┘
```

## 2. Các bước nghiệp vụ

### Bước 1 — Thiết lập kỳ lương (đầu tháng)

| Input | Nguồn |
|---|---|
| Danh sách CN kỳ này | Copy từ tháng trước, thêm/bớt CN mới (`Tên Tháng 1` / `Tên thang 01-26`) |
| Phân công phần cạo | Do quản lý đội giao, ghi cột `G` / `E11` |
| Đơn giá hiệu lực | 31.947 · 455 · 505 · 555 · 16.000 · 17.000... (xác nhận còn hiệu lực) |

### Bước 2 — Chấm công & cân sản lượng hàng ngày

| Input | Người nhập | Vào sheet |
|---|---|---|
| Công cạo thường / D2 mỗi ngày | Tổ trưởng | `CHAM CONG` (cột theo ngày × CN) |
| Ngày phụ cấp mì + dầu | Tổ trưởng | `CHAM CONG!BT` (Đợt 2) |
| Ngày chuyên cần | Tổ trưởng | `CHAM CONG!BO` |
| Thưởng/phạt kỹ thuật | Giám sát KT | `CHAM CONG!BW` (có thể âm) |
| Sản lượng mủ tươi cân (kg) | Nhân viên cân | `SAN LUONG` (AP, AO, AV…) |
| Sản lượng mủ dây | Nhân viên cân | `SAN LUONG!DW` |

### Bước 3 — Lập bảng Đợt 1 (ngày 15-17)

| Bước con | Sheet | Thao tác |
|---|---|---|
| 3.1 | `KL 1` (Đợt 1) | Tổng hợp tự động từ `CHAM CONG` + `SAN LUONG`. Quy đổi công mọi hạng mục. Cột X = tổng công. |
| 3.2 | `TTL 01` | Lấy `X` → nhân 31.947 → thành tiền đợt 1. Không khấu trừ. |
| 3.3 | `PTL T01-26` | Phân bổ tổng quỹ đợt 1 theo hạng mục (cạo, sản lượng 3 giá, xăng...) |
| 3.4 | `tổng hợp cty` | Báo cáo lên công ty mẹ (có tỷ giá 30.456) |
| 3.5 | `CĐCN T01-26` | Cân đối thu/chi đợt 1 |
| 3.6 | — | Trình duyệt, chi tiền mặt, CN ký nhận vào `TTL 01!R` |

**Kết quả Đợt 1:** Mỗi CN nhận được **"Lương đợt 1"** = công đợt 1 × 31.947 (làm tròn).

### Bước 4 — Lập bảng Đợt 2 + quyết toán tháng (đầu tháng kế tiếp)

| Bước con | Sheet | Thao tác |
|---|---|---|
| 4.1 | `KL 1` (Đợt 2) | Tổng hợp nửa cuối tháng. 13+ hạng mục (gồm chuyên cần, mủ dây, thưởng/phạt, mì+dầu, thiết kế MC, kiểm kê, PCCC...). Cột `AG` = tổng công đợt 2. |
| 4.2 | `KL 2` (Đợt 2) | Bảng công CN bổ sung (485..512) — phụ trợ, bảo vệ thường xuyên 31 công/tháng, kiểm kê cuối năm... |
| 4.3 | `TTL 1` | **Bảng quyết toán CN chính.** Cột G = Lương đợt 1 (hiện hardcode, cần link DB), H = Công đợt 2, I = Lương đợt 2, L = Tổng lương, N = BHXH, O = TNCN, R = Thực nhận đợt 2. |
| 4.4 | `TTL 2` | Bảng thanh toán CN bổ sung. |
| 4.5 | `PTL ` | Phân bổ quỹ đợt 2 theo 3 nhóm A/B/C (chăm sóc / thu hoạch / bảo vệ). |
| 4.6 | `CĐCN` | Cân đối toàn tháng: tổng lương = đợt 1 + đợt 2 + T13 + thưởng đầu vụ. Đã thu: đợt 1 đã ứng. Phải thu: TNCN + tạm ứng. Thực nhận = chênh lệch. |
| 4.7 | — | Duyệt, chi tiền, CN ký vào `TTL 1!S`. |

### Bước 5 — Báo cáo & nộp thuế/BHXH

- Tổng BHXH phải nộp = `SUM(N15:N498)` của `TTL 1` + `TTL 2`. Nộp cho NSSF Campuchia.
- Tổng TNCN phải nộp = `SUM(O...)`. Nộp cho thuế vụ.
- Báo cáo quỹ lương quy đổi × 30.456 cho công ty mẹ Việt Nam.

## 3. Điểm kiểm soát (approval)

| Sau bước | Ai duyệt | Gì cần duyệt |
|---|---|---|
| 3.1 (KL 1 đợt 1) | Tổ trưởng + KT đội | Kiểm đối chiếu với `CHAM CONG`, `SAN LUONG` |
| 3.2 (TTL 01) | KT trưởng | Tổng số tiền, định mức 31.947 |
| 3.5 (CĐCN đợt 1) | Giám đốc đội | Phê duyệt chi ứng |
| 4.3 (TTL 1 đợt 2) | KT trưởng | Đối chiếu Lương đợt 1 đã chi, BHXH, TNCN |
| 4.6 (CĐCN tháng) | Giám đốc đội + KT trưởng cty | Ký duyệt trước khi chi |

## 4. Inputs mỗi giai đoạn

### Input Đợt 1 (cần có trước ngày lập bảng đợt 1):
- Toàn bộ `CHAM CONG` nửa đầu tháng (cột ngày 01..15).
- Toàn bộ `SAN LUONG` nửa đầu tháng.
- Danh sách CN chốt phân phần cạo.
- Bảng đơn giá còn hiệu lực.

### Input Đợt 2 (cần có trước ngày lập bảng đợt 2):
- Toàn bộ `CHAM CONG` nửa sau tháng + các cột **đặc biệt** (chuyên cần `BO`, cạo D2 `BD`, mì+dầu `BT`, thưởng/phạt `BW`).
- Toàn bộ `SAN LUONG` nửa sau tháng.
- **Số Lương đợt 1 đã chi cho từng CN** (từ DB, không hardcode).
- Danh sách CN có lương T13, thưởng đầu vụ, thâm niên (nhập tay).
- Bảng công cố định: Thiết kế MC, Kiểm kê đầu vụ, PCCC+gác lửa (hiện 6,03 ngày/CN — cần input kỳ).
- Công bảo vệ cố định (414 công/tháng/đội — cần input từ tổ bảo vệ).

## 5. Edge cases quan sát được

| Tình huống | Cách xử lý hiện tại | Cảnh báo cho phần mềm |
|---|---|---|
| CN âm công do phạt KT nặng | `U17 < 0`, cộng vào tổng làm công âm | Cần chặn khi tổng công âm, hoặc quy định tối thiểu |
| CN nghỉ hết nửa tháng | Tất cả cột = 0 → Tổng công = 0 → Lương = 0 | Vẫn cần xuất bảng để lưu vết |
| Lương tháng > 8,5M | Công thức TNCN trả FALSE | **Bug — cần fix (thêm nhánh 15%/20%)** |
| Lương tháng ≤ 400k | BHXH bị tính sàn 8.000 Riel | Thu nhập thấp vẫn bị trừ — xác nhận lại với cty |
| Lương đợt 1 chi nhầm / chi bù | Hardcode sai sẽ kéo sai Đợt 2 | Phần mềm phải link real-time sang dữ liệu đợt 1 |
| CN mới vào giữa tháng | Chỉ có số ngày chấm công, không có sản lượng nửa đầu | Phần mềm tính pro-rata tự động theo thời điểm tham gia |
| CN nghỉ việc giữa tháng | Vẫn tính công đã làm, quyết toán ngay đợt chi gần nhất | Cần trạng thái "đã nghỉ" trên master CN |
