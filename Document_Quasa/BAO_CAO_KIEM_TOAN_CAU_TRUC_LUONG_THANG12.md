# BÁO CÁO GIẢI MÃ CẤU TRÚC TÍNH LƯƠNG THÁNG 12 - ĐỘI 1

## 1. Phạm vi và cách đọc báo cáo

Phạm vi chỉnh lại lần này chỉ tập trung vào workbook:

- `2025/tháng 12/Đội 1/LƯƠNG ĐỘI 1 THÁNG 12.2025 2  BẢN.xlsx`

File:

- `2025/tháng 12/Đội 1/~$LƯƠNG ĐỘI 1 THÁNG 12.2025 2  BẢN.xlsx`

không phải file lương nghiệp vụ. Đây là file khóa tạm do Excel sinh ra khi workbook chính đang được mở. File này không chứa logic tính lương và không dùng làm căn cứ phân tích.

Mục tiêu của bản cập nhật này:

- không dừng ở việc chép công thức,
- mà giải thích từng cụm ô quan trọng trong báo cáo,
- chỉ rõ hệ số nào đang chi phối kết quả,
- chỉ rõ điều kiện nào ràng buộc hoặc khóa giá trị của từng ô.

---

## 2. Bức tranh logic của workbook Đội 1

Workbook này đang vận hành theo 5 lớp dữ liệu:

```text
Danh mục nhân sự
  -> DS CN MOI UP / MSNV 2025

Chấm công
  -> CHẤM CÔNG / CHẤM CÔNG Axit / Công cây non / Chấm công CB ĐỘI 1 / CC NGƯỜI LÀO

Sản lượng và quy khô
  -> sản lượng / MỦ DÂY / MỦ SIRUM / MỦ XIRUM / THCKT / bảng kê công

Bảng tính lương theo đơn vị
  -> TRẠM 1 / TRẠM 2 / Đội 3 / Đ3 - TC / Đ3 - XC / Đội 4 / Đội 5 / Đội 5 - KV71ha / Đội 6

Bảng xuất báo cáo
  -> CÁCH TÍNH LƯƠNG / LƯƠNG ĐỘI / BẢNG PHÂN TOÁN / BẢNG PHÂN TOÁN (2)
```

Luồng nghiệp vụ cốt lõi:

1. Mã người lao động được chuẩn hóa từ `MSNV 2025` hoặc `DS CN MOI UP`.
2. Công thực tế được lấy từ các sheet chấm công.
3. Sản lượng từng người được lấy từ `sản lượng`, sau đó quy qua DRC để ra mủ quy khô trả lương.
4. Hạng kỹ thuật A/B/C/D quyết định đơn giá sản lượng.
5. Phụ cấp được cộng thêm theo số công thường, công chủ nhật, cây non, khộp nặng, cạo 2 lát, cạo 2 miệng.
6. Bảng tổng hợp cuối chỉ kéo lại các khoản đã tính ở bảng trạm/đội, không tự tính mới.

---

## 3. Vai trò từng nhóm sheet trong Đội 1

### 3.1 Nhóm input gốc

- `MSNV 2025`, `DS CN MOI UP`
  - chuẩn hóa mã nhân viên, họ tên, phục vụ toàn bộ `VLOOKUP`
  - ràng buộc: nếu tên hoặc mã không khớp danh mục, toàn bộ chuỗi lookup sau đó sẽ đổ lỗi hoặc ra trống

- `CHẤM CÔNG `
  - là nguồn công khai thác chính
  - cấp dữ liệu cho công thường, công chủ nhật, cạo 2 lát, cạo 2 lát chủ nhật, khộp nặng

- `Công cây non`, `cây non`
  - cấp riêng số công vườn cây năm nhất
  - chỉ áp cho một số người, không phải cả đội

- `CC NGƯỜI LÀO`
  - cấp công cho bảng `LƯƠNG ĐỘI`

- `sản lượng`
  - là nguồn sản lượng trả lương chính
  - chứa mủ tạp, mủ xirum, hạng kỹ thuật và một số chỉ tiêu tổng hợp dùng ở `TRẠM 1`, `TRẠM 2`, `THCKT`

### 3.2 Nhóm xử lý trung gian

- `THCKT`
  - là sheet tổng hợp công việc, DRC, quy khô, sản lượng, phụ cấp
  - thực chất là “bảng quy đổi nghiệp vụ” để các bảng lương chi tiết bám theo

- `TRẠM 1`, `TRẠM 2`
  - là hai sheet tính lương chi tiết quan trọng nhất của Đội 1
  - mỗi dòng là một công nhân
  - mỗi dòng gom 3 lớp tiền:
    - lương chăm sóc
    - lương sản lượng
    - phụ cấp nghề

### 3.3 Nhóm báo cáo đầu ra

- `CÁCH TÍNH LƯƠNG`
  - dùng cho cán bộ đội, không phải công nhân khai thác

- `LƯƠNG ĐỘI`
  - chốt lương cho cán bộ, CB-CNV người Lào, bảo vệ, tạp vụ

- `BẢNG PHÂN TOÁN `
  - bảng phân loại quỹ lương theo khoản mục để chốt chi phí và ký duyệt

---

## 4. Giải thích chi tiết sheet `TRẠM 1`

`TRẠM 1` là sheet thể hiện rõ nhất cơ chế tính lương công nhân khai thác.

### 4.1 Các ô tham số đầu sheet

| Ô | Ý nghĩa nghiệp vụ | Hệ số/ràng buộc |
|---|---|---|
| `E8` | Tỷ giá Bath qua Kip | Đây là hệ số quy đổi tiền lương sản lượng sang Kip. Sheet đang dùng giá trị `600`. Nếu tỷ giá thay đổi cuối tháng, toàn bộ cột tiền sản lượng đổi theo. |
| `AE5` | DRC tháng trước hoặc DRC tham chiếu | Lấy từ sheet `sản lượng`, dùng để quy phần truy lĩnh. |
| `AE6` | DRC mủ tạp hiện hành | Tính bằng `AE8/AE7`, tức lấy mủ quy khô chia mủ tạp. Đây là hệ số khóa chất lượng quy đổi. |
| `AE9` | DRC mủ serum | Lấy từ sheet `MỦ SIRUM`, dùng riêng cho phần serum nếu có. |

Ý nghĩa quản trị:

- `E8` là tham số tài chính,
- `AE6` và `AE9` là tham số kỹ thuật,
- hai nhóm này cùng quyết định một kg mủ thực tế cuối cùng đổi thành bao nhiêu tiền.

### 4.2 Cấu trúc cột của bảng lương công nhân

Từ hàng `16` trở xuống, mỗi dòng là một công nhân. Các nhóm cột chia như sau:

| Cụm cột | Vai trò |
|---|---|
| `A:E` | nhận diện người lao động |
| `F:G` | lương chăm sóc theo ngày công |
| `H:O` | sản lượng, quy khô, hạng kỹ thuật, tiền sản lượng |
| `P:V` | số công dùng để tính phụ cấp |
| `Y:AA` | thành tiền phụ cấp, tổng lương, lương còn lại |

### 4.3 Giải thích từng nhóm ô quan trọng

#### Nhóm nhận diện

- `B16 = VLOOKUP(C16,'MSNV 2025'!...)`
  - mã nhân viên không nhập tay mà tra từ danh mục
  - ràng buộc: tên ở cột `C` phải khớp đúng danh mục `MSNV 2025`

#### Nhóm lương chăm sóc

- `F16`
  - là số công chăm sóc
  - nguồn thực tế được kéo từ bảng chấm công hoặc trung gian tương ứng

- `G16 = F16 * 25000`
  - đây là công thức nền của lương chăm sóc
  - hệ số cố định: `25.000 kip / 1 công`
  - ràng buộc:
    - nếu không có công thì lương chăm sóc bằng 0
    - không phụ thuộc hạng kỹ thuật
    - chỉ phụ thuộc số công được ghi nhận

Ý nghĩa:

- đây là lớp thu nhập nền,
- phản ánh quy chế “có công thì có tiền công nền”.

#### Nhóm sản lượng và quy khô

- `H16`
  - lấy mủ tạp theo từng người từ sheet `sản lượng`

- `J16 = H16 * $AE$6`
  - chuyển mủ tạp thực tế thành mủ quy khô bằng DRC mủ tạp
  - hệ số khóa: `AE6`

- `K16`
  - mủ tạp truy lĩnh hoặc phần được tính bổ sung

- `L16 = K16 * $AE$5`
  - quy đổi phần truy lĩnh bằng DRC tham chiếu ở `AE5`

- `M16 = SUM(J16, L16)`
  - tổng mủ trả lương
  - đây là số lượng sản phẩm cuối cùng dùng để nhân đơn giá

Ý nghĩa:

- công ty không trả trực tiếp theo kg mủ tươi,
- mà trả theo “kg quy lương” sau quy đổi DRC,
- nên cùng kg thực tế nhưng DRC thấp thì tiền thấp hơn.

#### Nhóm hạng kỹ thuật và đơn giá sản lượng

- `N16`
  - hạng kỹ thuật A/B/C/D của công nhân trong tháng
  - kéo từ `sản lượng`

- `O16`
  - tiền lương sản lượng
  - logic thực tế là:
    - nếu hạng A dùng hệ số `9.2`
    - nếu hạng B dùng hệ số `8.9`
    - nếu hạng C dùng hệ số `8.6`
    - nếu hạng D dùng hệ số `8.3`
    - sau đó nhân với tỷ giá/tỷ lệ tiền ở `E8`

Ràng buộc cứng của ô `O16`:

- nếu `N16` không phải `A`, `B`, `C`, `D` thì tiền sản lượng bằng 0
- nếu không có sản lượng quy đổi ở `M16` thì tiền sản lượng bằng 0
- nếu `E8` thay đổi thì toàn bộ tiền sản lượng thay đổi đồng loạt

Ý nghĩa nghiệp vụ:

- hạng kỹ thuật không chỉ để xếp loại,
- mà là “khóa mở đơn giá”.
- cuối tháng QLKT đánh giá hạng nào thì hệ số tháng đó chạy theo hạng đó.

#### Nhóm phụ cấp công khai thác

Các ô lấy công từ `CHẤM CÔNG `:

- `P16`: công thường
- `Q16`: công chủ nhật
- `R16`: công cây non hoặc công vườn cây năm nhất
- `S16`: công khộp nặng
- `T16`: cạo 2 lát
- `U16`: cạo 2 lát chủ nhật
- `V16 = SUM(T16:U16)`: tổng công cạo 2 lát để tính phụ cấp thêm

Ô `Y16` là ô cực kỳ quan trọng:

- nó gom toàn bộ phụ cấp nghề theo logic:
  - công thường x `46.1`
  - công chủ nhật x `76.9`
  - cây non x `30.7`
  - cạo 2 lát ngày thường x `46.1 x 2`
  - cạo 2 lát chủ nhật x `76.9 x 2`
  - toàn bộ phần trên nhân thêm một hệ số tiền/DRC ở cụm `AD`
  - khộp nặng cộng riêng `20.000 kip / công`
  - phụ cấp thêm cạo 2 lát cộng riêng `100.000 kip / công`

Ràng buộc của ô `Y16`:

- chỉ phát sinh nếu bảng chấm công ghi nhận đúng loại công
- hệ số `46.1`, `76.9`, `30.7` là hệ số chuẩn của doanh nghiệp
- `S16` chỉ xuất hiện ở người thuộc vùng khộp nặng
- `R16` chỉ xuất hiện ở người có phát sinh cây non
- `V16` càng cao thì phần cộng cố định `100.000` càng lớn

Ý nghĩa nghiệp vụ:

- đây là cơ chế bù điều kiện lao động,
- không phải ai cũng hưởng cùng một mặt bằng phụ cấp,
- mà phụ thuộc vào loại công phát sinh trong tháng.

#### Nhóm tổng lương

- `Z16 = O16 + G16 + Y16`
  - tổng lương trước khấu trừ

- `AA16 = Z16`
  - trong nhiều dòng đang để bằng tổng lương, tức chưa có logic trừ riêng ở cột này

Ý nghĩa:

- tổng lương công nhân tại `TRẠM 1` = lương chăm sóc + lương sản lượng + phụ cấp nghề

### 4.4 Các hệ số chính đang chi phối `TRẠM 1`

| Nhóm hệ số | Giá trị thấy trong file | Vai trò |
|---|---|---|
| Công chăm sóc | `25.000` | trả công nền |
| Hạng kỹ thuật A/B/C/D | `9.2 / 8.9 / 8.6 / 8.3` | khóa đơn giá sản lượng |
| Công thường | `46.1` | hệ số phụ cấp ngày thường |
| Chủ nhật | `76.9` | hệ số phụ cấp ngày CN |
| Cây non | `30.7` | hệ số phụ cấp năm nhất |
| Khộp nặng | `20.000` | cộng tiền cố định theo công |
| Cạo 2 lát thêm | `100.000` | cộng tiền cố định theo công đặc thù |
| Tỷ giá Bath/Kip | `E8` | quy đổi tiền lương sản lượng |
| DRC mủ tạp | `AE6` | quy đổi sản lượng thực sang quy khô |

### 4.5 Các ràng buộc kiểm soát của `TRẠM 1`

- Bảng này phụ thuộc hoàn toàn vào chất lượng lookup.
- Nếu tên người lao động lệch với `MSNV 2025` hoặc `sản lượng`, tiền sẽ sai hoặc bằng 0.
- Nếu `CHẤM CÔNG ` không ghi đúng loại công, phụ cấp sẽ mất.
- Nếu hạng kỹ thuật không ra A/B/C/D, cột tiền sản lượng sẽ bị triệt tiêu.
- Nếu DRC tháng bị nhập sai, toàn bộ sản lượng quy lương sai đồng loạt.

---

## 5. Giải thích chi tiết sheet `TRẠM 2`

`TRẠM 2` dùng cùng cấu trúc với `TRẠM 1` nhưng khác mặt bằng hệ số.

### 5.1 Điểm giống

- vẫn chia thành:
  - lương chăm sóc,
  - lương sản lượng,
  - phụ cấp nghề
- vẫn phụ thuộc:
  - mã nhân sự
  - chấm công
  - sản lượng
  - DRC
  - hạng kỹ thuật

### 5.2 Điểm khác bản chất

Hệ số hạng kỹ thuật của `TRẠM 2` thấp hơn `TRẠM 1`:

- A: `7.7`
- B: `7.4`
- C: `7.1`
- D: `6.8`

Ý nghĩa:

- cùng sản lượng quy khô và cùng hạng,
- công nhân `TRẠM 2` ra tiền sản lượng thấp hơn `TRẠM 1`.

Đây là dấu hiệu rất rõ rằng:

- công ty đang phân biệt mặt bằng đơn giá theo vùng/trạm,
- không dùng một đơn giá sản lượng cứng duy nhất cho tất cả địa bàn.

### 5.3 Hệ quả quản trị

- Nếu hỏi “vì sao cùng công mà hai người ra tiền khác”, phải kiểm tra trước người đó thuộc `TRẠM 1` hay `TRẠM 2`.
- Hệ số vùng khó khăn hoặc đặc thù địa bàn đang được mã hóa ngay trong mặt bằng đơn giá, không chỉ nằm ở phụ cấp riêng.

---

## 6. Giải thích chi tiết sheet `THCKT`

`THCKT` là sheet trung tâm để kiểm soát dữ liệu sản lượng, công thực hiện và DRC.

### 6.1 Vai trò

Sheet này không phải bảng lương cuối, nhưng là nơi chuẩn hóa dữ liệu đầu vào cho lương.

Các ô và cụm cột cho thấy:

- tổng hợp công việc kỹ thuật/chăm sóc,
- tổng hợp mủ tạp, mủ dây, mủ serum,
- tính quy khô,
- kéo công ngày thường, chủ nhật, khộp nặng, cây non, axit, cạo gia tăng.

### 6.2 Các ô tham số quan trọng

- `AN7 = AN9 / AN8`
  - đây là công thức xác định DRC chung

- `Z13 = W13 * $AN$13 + X13 * $AN$10`
  - cho thấy quy khô có ít nhất hai thành phần:
    - một phần nhân DRC chính
    - một phần nhân hệ số riêng của loại mủ khác

Ý nghĩa:

- `THCKT` là cầu nối từ sản lượng thô sang sản lượng trả lương.
- Nếu sheet này sai, các sheet trạm kéo theo sai hàng loạt.

### 6.3 Ràng buộc của `THCKT`

- phụ thuộc chặt vào `sản lượng` và `CHẤM CÔNG `
- có các vùng tổng cộng nhiều dòng theo đội/trạm
- là nơi rất dễ phát sinh sai nếu copy tháng cũ mà quên cập nhật vùng cộng

---

## 7. Giải thích chi tiết sheet `CÁCH TÍNH LƯƠNG`

Sheet này không tính cho công nhân khai thác trực tiếp. Nó tính cho cán bộ đội.

### 7.1 Bản chất nghiệp vụ

Lương cán bộ đội gồm 4 lớp:

- lương bậc/chức danh
- lương hiệu quả theo mức hoàn thành kế hoạch
- phụ cấp
- truy lĩnh hoặc phần bổ sung

### 7.2 Các ô logic quan trọng

- `O11 = 360/27*22`
  - lương bậc chuẩn của một chức danh
  - ràng buộc:
    - chuẩn tháng là `27 công`
    - thực trả theo số công thực tế `22`

- `I12 = 'TRẠM 2'!M76/1000`
  - lấy sản lượng thực hiện từ bảng trạm
  - nghĩa là lương cán bộ đội không tách rời sản lượng thực tế của đơn vị

- `J12 = I12/H12`
  - tỷ lệ hoàn thành = thực hiện / kế hoạch
  - đây là ô khóa logic hiệu quả

- `Q11 = (300*J11*G11)/27*22`
  - phần lương hiệu quả
  - bị chi phối đồng thời bởi:
    - mức khoán chuẩn `300`
    - tỷ lệ hoàn thành `J11`
    - hệ số vị trí hoặc hệ số quản lý `G11`
    - công thực tế `22/27`

- `S11 = O11 + Q11 + P11 + R11`
  - tổng thu nhập cán bộ đội

### 7.3 Ràng buộc

- nếu sản lượng thực hiện thấp hơn kế hoạch thì lương hiệu quả giảm
- nếu công thực tế thấp hơn công chuẩn thì cả lương bậc và lương hiệu quả cùng giảm
- khác hoàn toàn với công nhân khai thác, cán bộ đội chịu ràng buộc đồng thời bởi:
  - ngày công
  - chức danh
  - sản lượng hoàn thành của đơn vị

---

## 8. Giải thích chi tiết sheet `LƯƠNG ĐỘI`

`LƯƠNG ĐỘI` là bảng chốt thu nhập của CB-CNV người Lào, bảo vệ, tạp vụ và nhóm liên quan.

### 8.1 Logic chung

Sheet này không tự tính lại từ đầu. Nó kéo dữ liệu từ:

- `CC NGƯỜI LÀO`
- `CÁCH TÍNH LƯƠNG`

rồi quy ra lương tháng và thuế.

### 8.2 Các ô chính

- `E13 = VLOOKUP(C13,'CC NGƯỜI LÀO'!...,34,0)`
  - số công thực tế

- `F13 = VLOOKUP(C13,'CÁCH TÍNH LƯƠNG'!...,17,0)`
  - mức lương hoặc tổng thu nhập đã được xác định ở bảng cán bộ đội

- `I13 = (F13 + H13) / P7 * E13`
  - lương tháng quy theo công
  - ràng buộc:
    - `P7` là công chuẩn của nhóm
    - `E13` là công thực tế
    - `H13` là phần phụ cấp/bổ sung

- `J13`
  - tính thuế TNCN lũy tiến
  - có 5 nấc ngưỡng:
    - trên `1.300.000`
    - trên `5.000.000`
    - trên `15.000.000`
    - trên `25.000.000`
    - trên `65.000.000`

- `K13 = I13 - J13`
  - thu nhập sau thuế

### 8.3 Ràng buộc

- nếu công thực tế thấp, lương bị chia theo công chuẩn
- nhóm bảo vệ có công chuẩn riêng, có dòng dùng chuẩn `31 ngày`
- thuế chỉ phát sinh nếu thu nhập vượt ngưỡng

Ý nghĩa:

- `LƯƠNG ĐỘI` là lớp “thanh toán cuối” của nhóm không còn tính như công nhân khai thác trực tiếp,
- mà tính kiểu lương tháng chia theo công.

---

## 9. Giải thích chi tiết sheet `BẢNG PHÂN TOÁN `

Sheet này là bảng trình bày tài chính, không phải bảng tính nguồn.

### 9.1 Vai trò

Nó phân rã quỹ lương thành các khoản:

- lương công nhân trực tiếp
- lương kinh doanh
- tiền sản lượng theo hạng kỹ thuật
- phụ cấp công khai thác
- các khoản nghề nghiệp khác

### 9.2 Logic các ô quan trọng

- `G13 = G14 + G27 + G38`
  - lương CN trực tiếp = sản lượng + phụ cấp + phần khác

- `D15 = SUMIF(...,"D",...)`
  - cho thấy bảng đang tổng sản lượng theo hạng kỹ thuật

- `F15 = 5.2 * J9`
  - đơn giá hạng D được neo bởi hệ số `5.2`

- `G15 = D15 * F15`
  - thành tiền của hạng D

- `F26 = 6.1 * J9`
  - mủ xirum có đơn giá cố định theo hệ số `6.1`

- `G27 = SUM(G28:G37)`
  - tổng phụ cấp ngày công khai thác

Ý nghĩa:

- `BẢNG PHÂN TOÁN ` không quyết định ai được bao nhiêu,
- mà quyết định công ty đang ghi nhận quỹ lương theo khoản mục nào.

### 9.3 Ràng buộc

- nếu bảng trạm sai, bảng này sai theo
- nếu hạng kỹ thuật ở nguồn thay đổi, cơ cấu quỹ lương theo hạng thay đổi
- trong bảng này có các dấu hiệu cộng trừ điều chỉnh tay, nên rủi ro kiểm soát cao hơn các sheet lookup thuần

---

## 10. Từ các ô đang thấy, có thể kết luận gì về hệ số và quy chế

### 10.1 Hệ số sản lượng

- `TRẠM 1`: `9.2 / 8.9 / 8.6 / 8.3`
- `TRẠM 2`: `7.7 / 7.4 / 7.1 / 6.8`
- `BẢNG PHÂN TOÁN`: có mặt bằng chuẩn `6.1 / 5.8 / 5.5 / 5.2`

Kết luận:

- công ty có đơn giá chuẩn toàn cục,
- nhưng từng trạm/đội có thể được hỗ trợ mặt bằng hệ số khác nhau,
- đúng với thực tế người dùng mô tả về vùng khó khăn/rừng khộp.

### 10.2 Hệ số phụ cấp công

- công thường: `46.1`
- chủ nhật: `76.9`
- cây non: `30.7`
- khộp nặng: `20.000 kip / công`
- cạo 2 lát thêm: `100.000 kip / công`
- lương chăm sóc: `25.000 kip / công`

Kết luận:

- phần phụ cấp đang là “bảng chính sách ngầm” được mã hóa trực tiếp trong file,
- chưa được tách riêng thành sheet quy chế độc lập.

### 10.3 Hệ số kỹ thuật

- hạng A/B/C/D là khóa đơn giá
- không phải tham số mô tả
- nếu hạng thay đổi cuối tháng, tiền sản lượng thay đổi trực tiếp

### 10.4 DRC

- DRC là hệ số kỹ thuật quyết định quy đổi mủ thực sang mủ trả lương
- DRC đang ảnh hưởng trực tiếp đến:
  - sản lượng quy khô
  - thành tiền sản lượng

---

## 11. Các rủi ro kiểm soát thấy ngay trong workbook

### 11.1 Rủi ro link và công thức

Trong workbook còn thấy:

- link ngoài workbook cũ
- `#REF!` ở một số sheet trung gian
- tiêu đề tháng cũ ở một số sheet copy từ tháng trước

Ý nghĩa:

- logic nghiệp vụ vẫn đọc được,
- nhưng độ an toàn vận hành của file chưa cao.

### 11.2 Rủi ro lookup

Toàn bộ hệ thống phụ thuộc rất mạnh vào `VLOOKUP`.

Nếu:

- lệch tên,
- lệch mã,
- thiếu dòng trong danh mục,

thì:

- sản lượng mất,
- phụ cấp mất,
- lương đội sai,
- bảng phân toán sai theo.

### 11.3 Rủi ro tham số ngầm

Các hệ số như:

- `25.000`
- `46.1`
- `76.9`
- `30.7`
- `100.000`
- `9.2 / 8.9 / 8.6 / 8.3`

đang nằm thẳng trong công thức, không nằm ở một sheet chính sách riêng.

Điều này gây khó cho:

- giải trình,
- kiểm toán,
- thay đổi chính sách sau này.

---

## 12. Kết luận quản trị

Nếu diễn giải đơn giản, cơ chế tính lương Đội 1 đang là:

```text
Tổng lương công nhân
  = lương chăm sóc theo công
  + lương sản lượng sau quy đổi DRC
  + phụ cấp theo loại công và điều kiện lao động
```

Trong đó:

- DRC khóa phần quy đổi sản lượng,
- hạng kỹ thuật khóa phần đơn giá,
- công thực tế khóa phần chăm sóc và phần phụ cấp,
- điều kiện địa bàn khóa phần hỗ trợ thêm giữa các trạm.

Với cán bộ đội thì logic khác:

```text
Tổng lương cán bộ đội
  = lương bậc theo chức danh
  + lương hiệu quả theo mức hoàn thành kế hoạch
  + phụ cấp
  + truy lĩnh
```

Với `LƯƠNG ĐỘI` thì logic lại là:

```text
Lương thực trả
  = thu nhập theo ngạch/bảng cán bộ
  quy theo công thực tế
  rồi trừ thuế
```

Nói ngắn gọn:

- `TRẠM 1`, `TRẠM 2` là nơi lương được tạo ra,
- `CÁCH TÍNH LƯƠNG` là nơi lương cán bộ đội được tạo ra,
- `LƯƠNG ĐỘI` là nơi thu nhập nhóm người Lào/bảo vệ/tạp vụ được chốt,
- `BẢNG PHÂN TOÁN ` là nơi quỹ lương được trình bày để thanh toán.

---

## 13. Bước tiếp theo nên làm

Nếu cần đi sâu hơn nữa từ chính workbook này, nên làm tiếp 3 việc:

1. lập “từ điển cột” riêng cho từng sheet:
   - `TRẠM 1`
   - `TRẠM 2`
   - `LƯƠNG ĐỘI`
   - `CÁCH TÍNH LƯƠNG`
   - `BẢNG PHÂN TOÁN `

2. bóc toàn bộ hệ số ngầm ra một bảng “Quy chế tham số” riêng:
   - tên hệ số
   - giá trị
   - sheet đang dùng
   - khoản mục chịu tác động

3. đánh dấu toàn bộ ô có rủi ro:
   - `#REF!`
   - link ngoài
   - ô điều chỉnh tay
   - ô tham số tài chính và DRC

Nếu cần, bước kế tiếp tôi có thể chỉnh tiếp báo cáo này thành bản sâu hơn theo đúng format:

- `Ô/cụm ô`
- `Nguồn dữ liệu`
- `Hệ số chi phối`
- `Ràng buộc`
- `Giải thích nghiệp vụ`
