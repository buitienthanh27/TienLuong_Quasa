---
name: document-feature
description: Tạo tài liệu tổng hợp cho feature đã implement
---

# Document Feature Command

Tạo file MD tổng hợp đầy đủ logic, business rules, và cấu trúc code cho feature.

## Usage

```
/document-feature <feature-name>
```

## Ví dụ

```
/document-feature attendance
/document-feature production
/document-feature payroll
```

## Action

**REQUIRED SKILL:** Sử dụng `feature-documentation` skill để tạo tài liệu.

1. Scan tất cả files liên quan đến feature
2. Phân tích business logic và rules
3. Tạo file MD theo template chuẩn
4. Lưu vào `docs/features/<feature-name>.md`
