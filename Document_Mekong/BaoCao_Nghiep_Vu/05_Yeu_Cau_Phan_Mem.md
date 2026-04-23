# 05 — YÊU CẦU PHẦN MỀM (Phase 1 — Đội trực tiếp)

## 0. Nguồn dữ liệu: Sổ mủ điện tử (e-Latex Logbook)

> Công ty đã triển khai xong **Sổ mủ điện tử**. Phần mềm lương **KHÔNG nhập lại** dữ liệu CN và sản lượng — thay vào đó **đồng bộ** từ Sổ mủ, rồi áp công thức đã phân tích để tính lương.

### 0.1 Phân chia trách nhiệm (System of Record)

| Dữ liệu | System of Record | Payroll sync/nhập |
|---|---|---|
| Hồ sơ công nhân (CMND, họ tên, năm sinh, phần cạo, đội) | **Sổ mủ điện tử** | Sync 1 chiều (read-only trong Payroll) |
| Sản lượng mủ hằng ngày (kg mủ tươi, cấp 455/505/555, phần/lô) | **Sổ mủ điện tử** | Sync theo kỳ (daily batch hoặc event) |
| Chấm công ngày (cạo thường / D2 / học úp / nghỉ) | **Sổ mủ điện tử** (module chấm công) | Sync theo kỳ |
| Phụ cấp đặc thù (PCCC, kiểm kê, thiết kế MC, chuyên cần, xăng, mì+dầu) | **Payroll** (nếu Sổ mủ chưa có) | Nhập tay qua Web Form |
| Đơn giá, định mức công (31.947), DRC, hằng số tháng | **Payroll** (Monthly Params) | Admin nhập/duyệt đầu kỳ |
| Thưởng/phạt kỹ thuật | **Payroll** | Nhập tay, có phê duyệt |
| Tạm ứng Đợt 1 | **Payroll** (tự sinh từ PayrollRun Đợt 1) | Tự động |

### 0.2 Cơ chế đồng bộ đề xuất

1. **API pull hằng ngày** (cron 23:59): Payroll pull sản lượng + chấm công ngày `d` từ Sổ mủ → bảng staging → validate → ghi vào `ProductionEntry` / `TimesheetEntry`.
2. **Webhook push** từ Sổ mủ khi có thay đổi hồ sơ CN (thêm/sửa/nghỉ việc) → cập nhật `NhanVien`.
3. **Period lock**: sau khi chạy payroll kỳ `T`, khoá dữ liệu Sổ mủ cho kỳ `T`; mọi điều chỉnh sau đó phải tạo **AdjustmentEntry** với lý do, người duyệt.
4. **Idempotency**: mỗi bản ghi sync có `external_id = <ma_so_mu>_<ngay>_<ma_cn>`; pull lại không tạo trùng.
5. **Đối chiếu**: mỗi kỳ có báo cáo đối soát `Sổ mủ vs Payroll` (tổng kg, tổng công) — chênh lệch >0,1% phải xử lý trước khi chốt lương.

### 0.3 Tác động tới scope phần mềm

- **Bỏ** module nhập chấm công/sản lượng thủ công (chỉ giữ chế độ dự phòng offline).
- **Giữ** Excel importer nhưng chỉ cho **migration dữ liệu lịch sử** (T12/2025 trở về trước) và đối chiếu pilot.
- **Tập trung effort** vào: Rule Engine, Monthly Params UI, Phụ cấp/Thưởng-Phạt, Báo cáo phân toán, Quyết toán Đợt 2.
- **Phụ thuộc**: Payroll cần Sổ mủ cung cấp **API tài liệu hoá** (Swagger/OpenAPI) cho các endpoint: `/employees`, `/production?date=`, `/attendance?date=`.

### 0.4 Rủi ro tích hợp cần làm rõ

- Sổ mủ có phân biệt **mủ theo cấp chất lượng (455/505/555)** chưa? Nếu chỉ có tổng kg thì thiếu thông tin để tính đơn giá.
- Sổ mủ có ghi **phần cạo / lô** chi tiết đủ cho phân toán không?
- Cơ chế **sửa hồi tố** (backdated correction) ở Sổ mủ có thông báo về Payroll không?
- Sổ mủ đang chạy cho **cả Quasa và Mekong** hay chỉ 1 trong 2? (Khác schema sẽ cần 2 adapter.)

---

## 1. Mô hình dữ liệu đề xuất (Data Model)

### 1.1 Các entity chính

```
┌─────────────┐     ┌──────────────┐     ┌──────────────┐
│  NhanVien   │     │  KyLuong     │     │  DotLuong    │
│─────────────│     │──────────────│     │──────────────│
│ id          │     │ id (YYYY-MM) │◀────│ kyLuongId    │
│ cmnd (K)    │     │ thang        │     │ soDot (1|2)  │
│ hoTen       │     │ nam          │     │ tuNgay       │
│ namSinh     │     │ doiId        │     │ denNgay      │
│ gioiTinh    │     │ trangThai    │     │ trangThai    │
│ ngayVao     │     └──────────────┘     └──────────────┘
│ ngayNghi    │
│ loai        │     ┌─────────────────────────────────┐
│  (DIRECT,   │     │  ChamCongNgay                   │
│   PHU_TRO,  │     │─────────────────────────────────│
│   BAO_VE)   │     │ id                              │
│ phanCao     │     │ nhanVienId (FK)                 │
│ doiId       │◀────│ ngay                            │
└─────────────┘     │ loaiCong (CAO_THUONG, CAO_D2,   │
                    │  CHUYEN_CAN, MI_DAU, PCCC,      │
                    │  KIEM_KE, THIET_KE_MC, PHAT_CO) │
                    │ soLuong (ngày hoặc kg)          │
                    └─────────────────────────────────┘

┌──────────────────────────┐     ┌──────────────────────────┐
│  SanLuongMuNgay          │     │  ThuongPhatKyThuat       │
│──────────────────────────│     │──────────────────────────│
│ nhanVienId (FK)          │     │ nhanVienId (FK)          │
│ ngay                     │     │ kyLuongId (FK)           │
│ muTuoiKg                 │     │ soTien (Riel, ±)         │
│ muDongKg (=muTuoi×DRC)   │     │ lyDo                     │
│ capChatLuong (455|505|   │     │ nguoiGhi                 │
│  555)                    │     └──────────────────────────┘
│ muDayKg                  │
└──────────────────────────┘

┌────────────────────────────┐    ┌────────────────────────────┐
│  KhoiLuongCN  (KL 1/KL 2)  │    │  BangLuongCN (TTL 1/TTL 2) │
│────────────────────────────│    │────────────────────────────│
│ dotLuongId (FK)            │    │ dotLuongId (FK)            │
│ nhanVienId (FK)            │    │ nhanVienId (FK)            │
│ congMu455  (công)          │    │ luongDot1                  │
│ congMu505                  │    │ congDot2                   │
│ congMu555                  │    │ luongDot2                  │
│ congCao16k                 │    │ luongThang13               │
│ congCaoD2                  │    │ luongThamNien              │
│ congXang                   │    │ tongLuong                  │
│ congChuyenCan              │    │ congNoDot1DaNhan           │
│ congMuDay                  │    │ bhxh                       │
│ congThuongPhat             │    │ tncn                       │
│ congMiDau                  │    │ tongKhauTru                │
│ congThietKeMC              │    │ thucNhan                   │
│ congKiemKe                 │    │ ngayKyNhan                 │
│ congPCCC                   │    │ chuKy (image path)         │
│ tongCong (computed)        │    └────────────────────────────┘
│ luongTruocKhauTru          │
└────────────────────────────┘

┌─────────────────────────┐     ┌────────────────────────────┐
│  CauHinhDonGia          │     │  CauHinhThue               │
│─────────────────────────│     │────────────────────────────│
│ id                      │     │ id                         │
│ ma (DINH_MUC_CONG,      │     │ loai (BHXH|TNCN)           │
│  MU_455, CAO_16K, etc.) │     │ hieuLucTu                  │
│ giaTri                  │     │ hieuLucDen                 │
│ donVi                   │     │ bangBieu (JSON: các ngưỡng)│
│ hieuLucTu               │     │ tyLe (%)                   │
│ hieuLucDen              │     │ san / tran                 │
└─────────────────────────┘     └────────────────────────────┘
```

### 1.2 Bảng cấu hình đơn giá (khởi tạo)

| Mã | Giá trị | Đơn vị | Ghi chú |
|---|---|---|---|
| `DINH_MUC_CONG` | 31.947 | Riel/công | Chuẩn toàn hệ thống |
| `DRC` | 0,44 | — | Tỷ lệ mủ tươi → mủ đông |
| `GIA_MU_455` | 455 | KHR/kg | Mủ đông loại cao |
| `GIA_MU_505` | 505 | KHR/kg | Mủ đông loại TB |
| `GIA_MU_555` | 555 | KHR/kg | Mủ đông loại thấp |
| `GIA_CAO_16K` | 16.000 | Riel/ngày | Cạo thường |
| `GIA_CAO_D2` | 17.000 | Riel/ngày | Cạo D2 |
| `GIA_HOC_CAO_UP` | 8.000 | Riel/ngày | |
| `GIA_XANG` | 1.000 | Riel/ngày | |
| `GIA_MI_DAU` | 35.000 | Riel/ngày | |
| `GIA_CHUYEN_CAN` | 3.000 | Riel/ngày | |
| `GIA_MU_DAY` | 1.000 | Riel/kg | |
| `GIA_THIET_KE_MC` | 9.031 | Riel/ngày | |
| `GIA_KIEM_KE` | 38.336 | Riel/ngày | |
| `GIA_PCCC` | 22.772 | Riel/ngày | |
| `CONG_BAO_VE_CO_DINH` | 414 | công/tháng/đội | |
| `TY_GIA_BAO_CAO_CTY_ME` | 30.456 | Riel/VND (?) | **Cần xác nhận** |
| `BHXH_TY_LE` | 2% | — | |
| `BHXH_SAN` | 400.000 | Riel | |
| `BHXH_TRAN` | 1.200.000 | Riel | |

### 1.3 Biểu thuế TNCN (JSON ví dụ, phiên bản hiệu lực 2026)

```json
{
  "loai": "TNCN_DOT_2",
  "hieuLucTu": "2026-01-01",
  "tyLeMacDinh": 0,
  "bangBieu": [
    { "den": 1500000, "tyLe": 0,   "hangTru": 0 },
    { "den": 2000000, "tyLe": 0.05, "hangTru": 75000 },
    { "den": 8500000, "tyLe": 0.10, "hangTru": 175000 },
    { "den": 12500000,"tyLe": 0.15, "hangTru": 525000 },
    { "den": null,    "tyLe": 0.20, "hangTru": 1150000 }
  ],
  "giamTru": ["LUONG_THANG_13", "BHXH"]
}
```

## 2. Tính năng cốt lõi (Phase 1)

### 2.1 Quản lý danh mục
- [ ] CRUD Nhân viên (CMND là khoá), kèm phần cạo, loại CN.
- [ ] CRUD Đơn giá có hiệu lực theo ngày (có lịch sử).
- [ ] CRUD Biểu thuế BHXH/TNCN có hiệu lực theo năm.
- [ ] CRUD Danh mục hạng mục công việc.

### 2.2 Nhập liệu hàng ngày
- [ ] Nhập chấm công theo ngày × CN × loại công (giao diện bảng, import từ Excel).
- [ ] Nhập sản lượng mủ cân: mủ tươi (kg) + cấp chất lượng + mủ dây.
- [ ] Nhập thưởng/phạt kỹ thuật (có thể âm).
- [ ] Tự động quy mủ tươi → mủ đông bằng DRC.

### 2.3 Tính lương Đợt 1
- [ ] Sinh bảng KL cho toàn bộ CN trong kỳ (01..15).
- [ ] Quy đổi mọi hạng mục → công theo công thức `ROUND(qty × gia / DINH_MUC_CONG, 2)`.
- [ ] Tính Tổng công, Thành tiền đợt 1.
- [ ] Xuất `PTL đợt 1`, `TTL 01`, `CĐCN đợt 1`.
- [ ] Chốt đợt 1 → khoá dữ liệu.

### 2.4 Tính lương Đợt 2 (quyết toán)
- [ ] Sinh bảng KL đợt 2 (16..31) với 13 hạng mục + KL 2 cho CN bổ sung.
- [ ] **Tự động đọc Lương đợt 1 từ DB** (không hardcode).
- [ ] Tính Tổng lương cả tháng = Đợt 1 + Đợt 2 + T13 + Thâm niên.
- [ ] Tính BHXH (2% có sàn/trần).
- [ ] Tính TNCN (biểu lũy tiến, có nhánh > 8,5M).
- [ ] Tính Thực nhận = Tổng lương − (Đã ứng + BHXH + TNCN).
- [ ] Xuất `PTL đợt 2`, `TTL 1`, `TTL 2`, `CĐCN tháng`.

### 2.5 Báo cáo
- [ ] Báo cáo tổng công ty mẹ (quy VND/USD qua tỷ giá).
- [ ] Báo cáo BHXH nộp NSSF.
- [ ] Báo cáo TNCN nộp thuế vụ.
- [ ] Phiếu lương từng CN (in ký).
- [ ] So sánh tháng hiện tại với tháng trước.

### 2.6 Workflow & kiểm soát
- [ ] Phê duyệt 2 cấp (KT trưởng → Giám đốc đội).
- [ ] Lịch sử chỉnh sửa từng ô.
- [ ] Khoá kỳ lương sau khi chi.
- [ ] Import từ file Excel hiện tại (migration).

## 3. Validation rules

| Rule | Cảnh báo nếu |
|---|---|
| `SoLuong_ngay_cong >= 0` | Nhập số âm (trừ thưởng/phạt) |
| `SoNgayChamCong <= ngay_trong_ky` | VD đợt 1 tối đa 15 ngày cạo |
| `Mu_tuoi_kg > 0` khi có ngày cạo > 0 | Đi cạo mà không có mủ → cần ghi chú |
| `TongCong >= 0` sau khi cộng phạt KT | Phạt quá nặng → công âm |
| `Luong_dot_1_DB == Luong_dot_1_đã_chi_thực_tế` | Sai lệch khi quyết toán |
| `BHXH = 0` ở đợt 1 | Nếu có BHXH ở đợt 1 → lỗi |
| `TNCN` phải có nhánh > 8,5M | Nếu trả FALSE → lỗi công thức |
| `Phan_cao` trong danh mục hợp lệ | CN có phần cạo không tồn tại |
| `CMND` là duy nhất | Trùng CMND → trùng CN |
| `Bảo_vệ_công_cố_định = 414` | Kiểm tra tổng phát đúng con số thoả thuận |

## 4. Edge cases cần xử lý

1. **Công âm** do phạt kỹ thuật lớn hơn công đạt → chuẩn hoá: hiển thị cảnh báo, không cho thực nhận âm.
2. **Biểu TNCN > 8,5M** — bổ sung nhánh 15% (− 525k) và > 12,5M là 20% (− 1.150k).
3. **CN mới vào giữa tháng** → pro-rate theo ngày làm.
4. **CN nghỉ việc giữa tháng** → quyết toán ngay, khoá hồ sơ.
5. **Đổi đơn giá giữa kỳ** — dùng đơn giá có hiệu lực theo ngày chấm công, không theo ngày tính lương.
6. **Hạng mục cố định (PCCC, thiết kế MC, kiểm kê)** hiện là `6,03` hardcode → cần cơ chế nhập theo kỳ, mỗi CN có thể khác.
7. **Bảo vệ 414 công cố định** — cần bảng chấm công tổ bảo vệ riêng, hoặc cơ chế nhập 1 lần.
8. **Tỷ giá 30.456** — cho phép nhập tỷ giá theo kỳ báo cáo, không hardcode.
9. **CN có T13, thâm niên** — chỉ một số CN có, cần bảng cấu hình CN nào được hưởng.
10. **Round off trong `AA17 = ROUND(...) + 0.01`** — nên xoá +0.01 nếu không xác định được mục đích.

## 5. Ưu tiên triển khai

| Ưu tiên | Module |
|---|---|
| P0 (MVP) | Master data CN + đơn giá · Nhập chấm công + sản lượng · Tính Đợt 1 · Xuất TTL 01 + CĐCN |
| P0 (MVP) | Tính Đợt 2 + BHXH + TNCN · Link Lương Đợt 1 từ DB · Xuất TTL 1, TTL 2, CĐCN tháng |
| P1 | Báo cáo cty mẹ (tỷ giá) · Báo cáo NSSF, thuế vụ · Import file Excel cũ |
| P2 | Workflow phê duyệt · Lịch sử sửa · Phiếu lương in ký · Dashboard KPI đội |
| P3 | Đội gián tiếp, VP, XCB (Phase 2) |

## 6. Câu hỏi cần chốt với khách trước khi code

1. **Tỷ giá 30.456** là gì? Riel → VND? Riel → USD? Có cập nhật theo tháng không?
2. **Định mức 30.131** ở `KL 2!G17` tại sao khác 31.947?
3. **Hệ số `+0.01` ở `AA17`** có cần giữ không?
4. **Cột `AF` #REF!** trong `KL 1` đợt 2 — chức năng gì? Có vào tổng công không?
5. **`6,03` ngày cho thiết kế MC / kiểm kê / PCCC** — đây là số cố định toàn đội hay nhập theo CN?
6. **Lương T13 và thâm niên** — công thức nào? Ai được hưởng? (Hiện nhập tay)
7. **Lương đợt 1 hardcode** — Excel hiện không link về Đợt 1, phần mềm cần link thế nào?
8. **Biểu TNCN > 8,5M** — khách hàng áp biểu Cambodia chính thức nào?
9. **Bảo vệ 414 công** — con số này tính ra từ đâu? Có đổi theo tháng không?
10. **CN bổ sung TTL 2** (485..512) thuộc nhóm nào (kiến thiết? bảo vệ?) — cần sơ đồ tổ chức.
