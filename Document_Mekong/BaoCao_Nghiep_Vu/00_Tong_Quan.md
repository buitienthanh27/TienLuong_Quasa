# 00 — TỔNG QUAN NGHIỆP VỤ TÍNH LƯƠNG ĐỘI TRỰC TIẾP (MEKONG)

> **Phạm vi:** Phase 1 phần mềm tính lương — chỉ **Đội trực tiếp** (công nhân khai thác mủ cao su).
> **Dữ liệu phân tích:** 2 file Excel tháng 01/2026 — Đợt 1 và Đợt 2 của Đội sản xuất 1.
> **Đơn vị:** Công ty TNHH Cao Su Mekong (Caoutchouc Mekong Co., Ltd) — vườn cây cao su tại Campuchia, báo cáo về công ty mẹ Việt Nam.

---

## 1. Mục đích hai file

| File | Tên gốc | Vai trò nghiệp vụ |
|---|---|---|
| **Đợt 1** | `Copy_ĐỘI 1_BẢNG LƯƠNG_TRỰC TIẾP_THÁNG 01_2026_ĐỢT 1.xlsx` | Tạm ứng lương nửa đầu tháng (01 → 15/01/2026). Chỉ trả 1 hạng mục: **Cạo, trút và giao nộp mủ**. Không khấu trừ BHXH/TNCN. |
| **Đợt 2** | `Copy of ĐỘI 1_BẢNG LƯƠNG_TRỰC TIẾP_THÁNG 01_2026_ĐỢT 2.xls - version 1.0. 3.25.2026 9.xlsx` | Quyết toán lương nửa cuối tháng (16 → 31/01/2026) + tổng hợp cả tháng. 13+ hạng mục công việc. **Khấu trừ BHXH 2% và TNCN** trên tổng lương cả tháng. Trừ lại phần đã ứng ở Đợt 1. |

**Quan hệ:** Đợt 1 = ứng lương giữa kỳ → toàn bộ số đã trả ở Đợt 1 trở thành **công nợ "Lương đã nhận đợt 1"** ở Đợt 2 (cột `M15` / `L15` trong sheet `TTL 1`, `TTL 2`). Đợt 2 là kỳ quyết toán cuối tháng: tính tổng lương cả tháng, trừ BHXH + TNCN + phần ứng Đợt 1 → ra **Thực nhận** Đợt 2.

---

## 2. Quy mô dữ liệu (tháng 01/2026)

| Chỉ số | Giá trị |
|---|---|
| Tổng quỹ lương tháng (Đội 1) | 857.093.284 Riel |
| Đợt 1 | 7.400,8 công · 236.433.358 Riel (27,6%) |
| Đợt 2 | 19.427,8 công · 620.659.926 Riel (72,4%) |
| Tổng công nhân | 484 CN chính + 28 CN bổ sung (sheet `TTL 2` ở file Đợt 2) = 512 |
| BHXH đã trừ (chỉ Đợt 2) | 11.550.637 Riel |
| TNCN đã trừ (chỉ Đợt 2) | 4.524.349 Riel |
| Đơn vị tiền | **Riel (KHR)** — tiền Campuchia |

---

## 3. Vòng đời bảng lương 1 tháng

```
┌──────────────┐   ┌──────────────┐   ┌──────────────┐   ┌──────────────┐
│ Chấm công    │   │ Cân đo sản   │   │ Đợt 1        │   │ Đợt 2        │
│ (hàng ngày)  │──▶│ lượng mủ     │──▶│ (15/tháng)   │──▶│ (cuối tháng) │
│ CHAM CONG    │   │ SAN LUONG    │   │ Ứng 1/2 tháng│   │ Quyết toán   │
└──────────────┘   └──────────────┘   └──────────────┘   └──────────────┘
                                              │                   │
                                              ▼                   ▼
                                         PTL + CĐCN         BHXH + TNCN
                                         TTL 01 (thanh      TTL 1 + TTL 2
                                         toán đợt 1)        (thanh toán
                                                             quyết toán)
```

**Ghi chú quan trọng:** Đơn vị lương gốc là **"công"** (công = ngày công chuẩn). Mọi hạng mục (sản lượng mủ, ngày cạo, phụ cấp, chuyên cần, thiết kế mặt cạo, PCCC...) đều được **quy đổi về "công"** bằng cách chia cho định mức **31.947 Riel/công**, rồi mới nhân ngược lại ra tiền. Công thức cốt lõi:

> **Tổng tiền lương CN = Tổng công × 31.947 Riel**

---

## 4. Vai trò người dùng (đề xuất)

| Vai trò | Trách nhiệm |
|---|---|
| Tổ trưởng / Quản lý đội | Nhập chấm công hàng ngày (sheet `CHAM CONG`), ghi nhận sản lượng mủ cân từng ngày (`SAN LUONG`) |
| Kế toán đội | Lập bảng `KL 1`, `KL 2` (quy đổi công), `PTL` (phân toán lương), `TTL` (thanh toán lương) |
| Kế toán trưởng / Giám đốc đội | Duyệt bảng lương, ký `CĐCN` (Cân đối công nợ) |
| Công ty mẹ (Việt Nam) | Báo cáo tổng hợp tháng (sheet `tổng hợp cty`) — cần tỷ giá **30.456** (Riel → VND hoặc Riel → USD? — cần làm rõ) |

---

## 5. Cấu trúc file theo lớp nghiệp vụ

| Lớp | Sheet (Đợt 1) | Sheet (Đợt 2) | Vai trò |
|---|---|---|---|
| Danh mục | `TÊN HÀNG MỤC`, `Tên Tháng 1` | `tên hang mục`, `Tên thang 01-26` | Danh sách nhân viên, danh mục công việc |
| Dữ liệu thô | `CHAM CONG`, `SAN LUONG`, `ct thuc te`, `GA 1..4` | `CHAM CONG`, `SAN LUONG` | Chấm công hàng ngày, sản lượng mủ cân hàng ngày |
| Quy đổi công | `KL 1` | `KL 1`, `KL 2` | Gom dữ liệu thô → quy đổi ra **tổng công** mỗi CN |
| Phân toán | `PTL T01-26` | `PTL ` | Phân bổ quỹ lương theo hạng mục (Chăm sóc / Thu hoạch / Bảo vệ) |
| Thanh toán | `TTL 01` | `TTL 1` (CN chính), `TTL 2` (CN bổ sung 485-512) | Bảng thanh toán lương cho từng CN, có chữ ký nhận |
| Quyết toán | `CĐCN T01-26`, `tổng hợp cty` | `CĐCN` | Cân đối công nợ, báo cáo lên cty mẹ |
| Phụ trợ | `Sheet1`, `KL 1` (ẩn) | `Sheet3` | Sổ tay/biến tạm |

---

## 6. Cần làm rõ với khách

- Hệ số **30.456** xuất hiện cạnh cột phân toán (`PTL!I14 = D14 × 30456`) và cột `V16` ở `KL 1` Đợt 1 (`=ROUND(U16×30456, 0)`) — nghi là tỷ giá quy đổi Riel → VND hoặc Riel → USD để báo cáo cty mẹ. Cần xác nhận.
- Định mức **31.947 Riel/công** là cố định toàn hệ thống — cần xác nhận đây là lương tối thiểu vùng Campuchia hay định mức nội bộ cty.
- Đợt 1 dùng biểu thuế TNCN cũ (ngưỡng 1,2M/2M/8,5M), Đợt 2 dùng biểu mới (1,5M/2M/8,5M) — về bản chất Đợt 1 không tính thuế (vì chỉ là ứng) nhưng cột `O15` vẫn có công thức, cần xác nhận không dùng.
