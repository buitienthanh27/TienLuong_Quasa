# Quy Định Nghiệp Vụ Quasa (Cập Nhật 04/2026)

## 1. Quy Tắc Làm Tròn & Định Dạng Số

### 1.1 Dấu Phân Cách Thập Phân
- **Quy định**: Sử dụng **dấu chấm (.)** làm dấu phân cách thập phân
- **Ví dụ**: 9.25 kg (KHÔNG phải 9,25 kg)
- **Code**: Sử dụng `CultureInfo.InvariantCulture` khi format số

### 1.2 Số Thập Phân (General)
- **Quy định**: Cắt lấy 2 số sau dấu thập phân (TRUNCATE, **không làm tròn**)
- **Ví dụ**: 9.256 → 9.25 (không phải 9.26)
- **Áp dụng**: Mủ quy khô, các phép tính trung gian

```csharp
public static decimal TruncateDecimal(decimal value, int decimals)
{
    var factor = (decimal)Math.Pow(10, decimals);
    return Math.Truncate(value * factor) / factor;
}

// Sử dụng:
var dryLatex = TruncateDecimal(freshLatex * drc, 2);  // 9.256 → 9.25
```

### 1.3 Cân Mủ Tươi
- **Quy định**: **Không có số lẻ** (cắt bỏ toàn bộ phần thập phân)
- **Ví dụ**: 9.3 kg → 9 kg
- **Lý do**: Đơn vị cân tại trạm chỉ hiển thị số nguyên

```csharp
var freshLatexRounded = Math.Truncate(freshLatex);  // 9.3 → 9
```

### 1.4 Tổng Tiền Lương
- **Quy định**: Làm tròn đến đơn vị **Ngàn** (3 số cuối = 000)
- **Ví dụ**: 10,768,456 → 10,768,000
- **Áp dụng**: Tổng lương cuối cùng của CN

```csharp
var netSalary = Math.Round(calculatedNet / 1000) * 1000;  // 10,768,456 → 10,768,000
```

---

## 2. Đánh Giá Kỹ Thuật (Technical Evaluation)

### 2.1 Quy Trình 2 Bước

```
┌─────────────────────────────────────────────────────────────┐
│                  ĐÁNH GIÁ KỸ THUẬT QUASA                    │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  BƯỚC 1: ĐỘI CHẤM (Default)                                │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ • Ai chấm: Đội trưởng / QLKT đội                    │   │
│  │ • Khi nào: Cuối mỗi tháng                           │   │
│  │ • Cách chấm: Đánh giá ngẫu nhiên các phần cây       │   │
│  │ • Kết quả: Điểm số → Xếp hạng A/B/C/D               │   │
│  └─────────────────────────────────────────────────────┘   │
│                        ↓                                   │
│  BƯỚC 2: CÔNG TY PHÚC TRA (Override - nếu có)              │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ • Ai chấm: Phòng KT Công ty                         │   │
│  │ • Khi nào: Đột xuất, không báo trước                │   │
│  │ • Kết quả: Điểm Công ty OVERRIDE điểm Đội           │   │
│  │            → FinalGrade = CompanyGrade               │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  LOGIC QUYẾT ĐỊNH:                                          │
│  if (CompanyReviewed)                                       │
│      FinalGrade = CompanyGrade  // Điểm Công ty             │
│  else                                                       │
│      FinalGrade = TeamGrade     // Điểm Đội                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Bảng Quy Đổi Điểm → Hạng

| Điểm | Hạng | Đơn giá Trạm 1 (Bath/kg) | Đơn giá Trạm 2 (Bath/kg) |
|------|------|---------------------------|---------------------------|
| >= 90 | **A** | 9.2 | 7.7 |
| >= 80 | **B** | 8.9 | 7.4 |
| >= 70 | **C** | 8.6 | 7.1 |
| < 70 | **D** | 8.3 | 6.8 |

**Lưu ý**: 
- Trạm 1 có đơn giá cao hơn Trạm 2 (vùng khó khăn)
- Đơn giá thường cố định 1 năm, do Phòng KT quy định

---

## 3. Tạm Ứng (Advance Payment)

### 3.1 Quy Định
- **DRC Tạm ứng**: **40%** (fixed)
- **Khi nào**: Giữa tháng, trước ngày lễ, hoặc khi CN có nhu cầu
- **Phương thức**: Trả tiền mặt trực tiếp
- **Trừ lại**: Cuối tháng, trừ vào bảng lương chính
- **Giới hạn**: Không quá 50% lương dự kiến tháng

### 3.2 Công Thức Tính Tiền Tạm Ứng

```csharp
// Khi CN tạm ứng dựa trên sản lượng mủ
const decimal DRC_ADVANCE = 0.40m;  // DRC 40%

var dryLatex = freshLatexKg * DRC_ADVANCE;
var advanceAmount = Math.Round(dryLatex * unitPricePerKg, 0);
```

### 3.3 Workflow

```
PENDING → APPROVED → DISBURSED → DEDUCTED
   │         │           │          │
   │         │           │          └── Đã trừ vào lương
   │         │           └── Đã chi tiền mặt
   │         └── Đội trưởng duyệt
   └── CN yêu cầu
```

---

## 4. Ký Hiệu Chấm Công

| Ký hiệu | Tên gọi | Mô tả |
|---------|---------|-------|
| **X** | Công thường | Ngày làm việc bình thường |
| **CN** | Chủ nhật | Làm việc ngày chủ nhật (hệ số cao hơn) |
| **L** | Nghỉ lễ | Nghỉ ngày lễ có lương |
| **P** | Nghỉ phép | Nghỉ phép năm có lương |
| **Ô** | Nghỉ ốm | Nghỉ ốm có BHXH (nếu áp dụng) |
| **CT** | Công tác | Đi công tác (tính công + phụ cấp) |
| **NB** | Nghỉ bù | Nghỉ bù ngày lễ/CN đã làm việc |
| **KP** | Không phép | Nghỉ không phép (không lương) |
| **TS** | Thai sản | Nghỉ thai sản (chế độ BHXH) |
| **HĐ** | Học/Đào tạo | Đi học/đào tạo do công ty cử |
| **/X** | Nửa công | Làm nửa ngày (0.5 công) |

---

## 5. Hệ Số Phụ Cấp (Factor)

### 5.1 Phần Hệ Số (× Factor $AD$4 ≈ 667.96)

| Loại công | Ô Excel | Hệ số | Đơn giá kip/công |
|-----------|---------|-------|------------------|
| Công thường | P16 | 46.1 | 30,793 |
| Công chủ nhật | Q16 | 76.9 | 51,366 |
| Vườn cây năm nhất | R16 | 30.7 | 20,506 |
| Cạo 2 lát/miệng thường | T16 | 46.1 × 2 | 61,586 |
| Cạo 2 lát/miệng CN | U16 | 76.9 × 2 | 102,732 |

### 5.2 Phần Cố Định (không dùng Factor)

| Loại công | Ô Excel | Đơn giá kip/công |
|-----------|---------|------------------|
| Khộp nặng | S16 | 20,000 |
| PC thêm cạo 2 lát/miệng | V16 | 100,000 |

---

## 6. DRC (Dry Rubber Content)

| Loại | Giá trị | Mô tả |
|------|---------|-------|
| DRC mủ tạp (AE6) | ~38.5% | Thay đổi theo trạm & tháng |
| DRC truy lĩnh (AE5) | ~7.48% | DRC của tháng trước (VD: DRC T6) |
| DRC mủ serum (AE9) | 55% | Cố định |
| DRC tạm ứng | **40%** | Fixed cho tính tạm ứng |

---

## 7. Tỷ Giá

| Loại | Giá trị | Nguồn | Cập nhật |
|------|---------|-------|----------|
| Bath → Kip | 600 | Vietinbank | Cuối tháng |
| USD → Kip | 640 | Vietinbank | Cuối tháng |

**Quy trình**: P.Kế toán cung cấp → P.Tổ chức ban hành xác nhận → Thông báo toàn công ty

---

## 8. BHXH & TNCN

### 8.1 Đối Tượng Áp Dụng

| Đối tượng | BHXH | TNCN |
|-----------|------|------|
| CN trực tiếp khai thác | **KHÔNG** | **KHÔNG** |
| Cán bộ đội (USD) | Có | Có (biểu Lào) |
| BV/Tạp vụ người Lào | Có (một số) | Có (một số) |

### 8.2 Tỷ Lệ BHXH (Lào)
- BHXH: 8%
- BHYT: 2.5%
- Chỉ áp dụng cho cán bộ và một số đối tượng đặc biệt

---

## 9. Công Thức Tổng Hợp

```
Thu nhập CN = Tiền sản lượng + Lương chăm sóc + Phụ cấp

Trong đó:
├── Tiền sản lượng = TRUNCATE(Quy khô × Hệ số hạng × Tỷ giá, 0)
│   └── Quy khô = TRUNCATE(Mủ tạp × DRC, 2)  // KHÔNG làm tròn
│
├── Lương chăm sóc = Công CS × 25,000 kip
│
└── Phụ cấp = (Hệ số × Factor) + Cố định
    ├── Hệ số: (P×46.1 + Q×76.9 + R×30.7 + T×92.2 + U×153.8) × 667.96
    └── Cố định: S×20,000 + V×100,000

Tổng lương cuối = ROUND(Thu nhập - Tạm ứng, -3)  // Làm tròn đến Ngàn
```

---

## 10. Version History

| Ngày | Phiên bản | Nội dung |
|------|-----------|----------|
| 13/04/2026 | 1.0 | Phiên bản đầu tiên |
| 22/04/2026 | 1.1 | Bổ sung quy tắc làm tròn, đánh giá KT 2 bước, tạm ứng DRC 40%, ký hiệu chấm công |
