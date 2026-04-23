# Superpowers Activation — API_Sample & Tien_Luong

## Kích hoạt tự động

Superpowers được kích hoạt tự động cho mọi conversation trong dự án này. Agent PHẢI tuân thủ methodology Superpowers.

## Quy tắc bắt buộc

### 1. Trước khi viết code mới

**BẮT BUỘC** sử dụng skill `brainstorming` trước khi:
- Tạo feature mới
- Thêm module/component mới
- Thay đổi behavior quan trọng
- Implement yêu cầu từ user

```
Invoke: superpowers:brainstorming
```

### 2. Trước khi implement

**BẮT BUỘC** sử dụng skill `writing-plans` để tạo implementation plan:
- Mọi task >15 phút cần có plan
- Plan phải có bite-sized tasks (2-5 phút/task)
- Plan lưu vào `docs/superpowers/plans/YYYY-MM-DD-<feature>.md`

```
Invoke: superpowers:writing-plans
```

### 3. Test-Driven Development (TDD)

**BẮT BUỘC** tuân thủ TDD:
- Viết test TRƯỚC khi viết code
- Xem test FAIL trước
- Viết code tối thiểu để pass
- Refactor sau khi green

```
Invoke: superpowers:test-driven-development
```

### 4. Trước khi báo hoàn thành

**BẮT BUỘC** sử dụng skill `verification-before-completion`:
- Chạy tất cả tests
- Build không lỗi
- Verify output thực tế
- Evidence trước assertions

```
Invoke: superpowers:verification-before-completion
```

### 5. Debug có hệ thống

Khi gặp bug hoặc test fail, **BẮT BUỘC** sử dụng:
```
Invoke: superpowers:systematic-debugging
```

KHÔNG được đoán mò hoặc thử random fixes.

## Nguyên tắc cốt lõi

| Nguyên tắc | Mô tả |
|------------|-------|
| **TDD** | Test trước, code sau. Không ngoại lệ. |
| **YAGNI** | You Aren't Gonna Need It - không viết code không cần |
| **DRY** | Don't Repeat Yourself - tái sử dụng code |
| **Bite-sized** | Mỗi task 2-5 phút, có checkpoint |
| **Evidence first** | Chứng minh trước, tuyên bố sau |

## Workflow chuẩn

```
User request
    ↓
[brainstorming] → Spec document
    ↓
[writing-plans] → Implementation plan
    ↓
[test-driven-development] → Red-Green-Refactor
    ↓
[verification-before-completion] → Verified output
    ↓
[feature-documentation] → Feature docs
    ↓
Done
```

### 6. Sau khi implement xong

**BẮT BUỘC** sử dụng skill `feature-documentation` để tạo tài liệu:
- Tổng hợp tất cả business rules
- Document luồng xử lý
- Liệt kê files và code liên quan
- Lưu vào `docs/features/<feature-name>.md`

```
Invoke: feature-documentation
```

## Commands có sẵn

| Command | Mục đích |
|---------|----------|
| `/brainstorm` | Bắt đầu brainstorm tính năng |
| `/write-plan` | Viết implementation plan |
| `/execute-plan` | Thực thi plan từng bước |
| `/document-feature` | Tạo tài liệu tổng hợp feature |
| `/review` | Request code review |
| `/fix-issue` | Fix issue có hệ thống |
| `/deploy` | Deploy verification |

## Red Flags - DỪNG LẠI

Nếu thấy bất kỳ dấu hiệu nào sau, DỪNG và sử dụng skill phù hợp:

- Viết code trước test → Dùng `test-driven-development`
- Không có plan cho task lớn → Dùng `writing-plans`
- Đoán mò fix bug → Dùng `systematic-debugging`
- Claim done mà chưa verify → Dùng `verification-before-completion`
- Bắt đầu code mà chưa hiểu rõ yêu cầu → Dùng `brainstorming`

## Áp dụng cho dự án

Dự án này bao gồm:
- **Backend**: API_Sample (.NET 8, N-Tier, EF Core)
- **Frontend**: Tien_Luong (React TypeScript)

Superpowers áp dụng cho CẢ HAI phần. Mọi code mới phải qua workflow trên.
