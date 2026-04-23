# Báo cáo Hợp nhất — Unified Payroll Engine cho Quasa & Mekong

## Mục đích
Tài liệu này tổng hợp, so sánh nghiệp vụ tính lương giữa hai công ty cao su **Quasa-Geruco** (Lào, tính theo **tiền**) và **Mekong** (Campuchia, tính theo **công**), đồng thời đề xuất một kiến trúc **phần mềm lương hợp nhất (single engine)** — chạy cho cả hai công ty chỉ bằng cách thay đổi **rule cấu hình** (không fork code, không build hai app riêng).

## ⚠️ Phạm vi Giai đoạn 1 (MVP)
**Chỉ tính lương cho công nhân trực tiếp khai thác** (`employee_type = DIRECT_WORKER`) của Đội 1 ở cả Quasa & Mekong.

**KHÔNG bao gồm** trong GĐ1:
- Lương **cán bộ** (Quasa: 7 CB USD; Mekong: chưa tài liệu hoá)
- Lương **gián tiếp** / bảo vệ / tạp vụ
- **Thuế TNCN** — CN trực tiếp ở cả 2 công ty đều KHÔNG chịu thuế TNCN. Quasa có tính TNCN cho cán bộ → xử lý ở Giai đoạn 2.

Nội dung liên quan TNCN / cán bộ trong báo cáo được đánh dấu badge **"Phụ lục — GĐ2"** và vẫn giữ lại để chuẩn bị cho giai đoạn tiếp theo.

## 📒 Nguồn dữ liệu: Sổ mủ điện tử
Công ty đã triển khai xong **Sổ mủ điện tử (e-Latex Logbook)** — chứa toàn bộ **hồ sơ lao động** và **sản lượng mủ hằng ngày** theo từng công nhân. Phần mềm lương sẽ **đồng bộ dữ liệu từ Sổ mủ điện tử** (không nhập lại), rồi áp các công thức đã phân tích từ Excel.

- **Sổ mủ → Payroll (sync)**: hồ sơ CN, sản lượng mủ, chấm công ngày.
- **Payroll nhập thêm**: phụ cấp đặc thù, đơn giá/factor/DRC theo kỳ, các điều chỉnh cuối tháng.
- **Excel**: chỉ còn vai trò migration dữ liệu lịch sử + đối chiếu kết quả giai đoạn pilot.

## Cách mở báo cáo
1. Mở file `BAO_CAO_HOP_NHAT.html` bằng trình duyệt (Chrome / Edge / Firefox). Không cần server, không cần build. Các CDN (Tailwind, Mermaid, Chart.js) được load trực tiếp.
2. Sidebar trái có mục lục sticky — click để nhảy section.
3. Mermaid diagrams tự render khi cuộn tới; chart So sánh quy mô dùng Chart.js.

## Các điểm then chốt
- **Bản chất giống nhau**: cả 2 đều là payroll theo sản lượng mủ cao su, đều có chấm công + sản lượng → tính tiền → khấu trừ BHXH/TNCN → thực nhận.
- **Khác biệt cốt lõi**:
  - **Quasa**: công thức là `Tiền = Sản_lượng × Hệ_số_hạng × Tỷ_giá` (ra thẳng kip/USD). Tính theo **tiền**.
  - **Mekong**: công thức là `Công = Sản_lượng × Đơn_giá / 31.947`, sau đó `Tiền = Tổng_công × 31.947`. Tính theo **công quy đổi**.
- **Đơn vị**: Quasa dùng **kip Lào (LAK)** + USD + Bath; Mekong dùng **Riel Campuchia (KHR)** + VND (báo cáo công ty mẹ).
- **Kỳ lương**: Quasa **1 kỳ/tháng**; Mekong **2 đợt/tháng** (tạm ứng + quyết toán).
- **Giải pháp**: Thiết kế entity `WorkUnit` trung lập (có thể là VND, kg, công, USD) + `RuleSet` cấu hình theo công ty. Engine thực thi pipeline 5 bước (Normalize → Price → Allowance → Deduction → Net) dựa trên rule DSL.

## File trong thư mục
- `BAO_CAO_HOP_NHAT.html` — báo cáo đầy đủ, single-file, 12 section.
- `README.md` — tài liệu này.

## Bản quyền & nguồn
- Dữ liệu Quasa: `Document_Quasa/` (SRS, Mapping, Quy chế tham số).
- Dữ liệu Mekong: `Document_Mekong/BaoCao_Nghiep_Vu/` (6 file .md).
- Báo cáo tham khảo style cũ: `Bao_cao_excel_Quasa.html`, `Bao_cao_excel_Mekong.html`.
