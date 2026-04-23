# Error Handling — API_Sample

Lỗi chia 3 loại — KHÔNG dùng `throw` ở Service.

## 1. Logic error (Validate, business rule) — `result = 0`

Dùng `Error(HttpStatusCode, message)` của `BaseService`:

```csharp
if (await _context.Products.AnyAsync(x => x.Code == request.Code && x.Status != -1))
    return Error(HttpStatusCode.Conflict, "Mã trùng lặp!");

var data = await _context.Products.FindAsync(id);
if (data == null)
    return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);
```

`Error()` tự rollback transaction nếu đang mở. Implicit cast sang `ResponseData<T>` nên dùng trực tiếp trong `return`.

## 2. System exception — `result = -1`

Mọi method service BẮT BUỘC bọc `try/catch`, catch tổng dùng `CatchException`:

```csharp
public async Task<ResponseData<MRes_Product>> Update(MReq_Product request)
{
    try
    {
        // ... logic
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(Update), request);
    }
}
```

`CatchException` tự:
- Rollback transaction nếu có pending.
- Serialize parameter bằng `Newtonsoft.Json`.
- Log qua `_logger.LogError(ex, "{Class}.{Method} Exception_Logger. Parameters: ...")`.
- Trả về `ErrorResponseBase` với HTTP 500.

## 3. Validation request (ModelState)

Tự động bắt bởi middleware trong `Program.cs`. KHÔNG check `ModelState.IsValid` thủ công trong Controller.

## Save changes

- Sau `SaveChangesAsync`: kiểm tra `if (save == 0) return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE/UPDATE)`.
- Với bulk `ExecuteUpdateAsync`/`ExecuteDeleteAsync`: kiểm tra giá trị trả về `if (updatedCount == 0) return Error(HttpStatusCode.NotFound, ...)`.

## Constants thông báo

Sử dụng `MessageErrorConstants` (trong `API_Sample.Utilities/Constants/`):
- `CREATE_SUCCESS`, `UPDATE_SUCCESS`, `DELETE_SUCCESS`
- `DO_NOT_FIND_DATA`
- `EXCEPTION_DO_NOT_CREATE`, `EXCEPTION_DO_NOT_UPDATE`

Thêm constant mới vào `MessageErrorConstants` thay vì hardcode chuỗi (ngoại trừ message nghiệp vụ tiếng Việt cụ thể như `"Mã trùng lặp!"`).

## Transaction

- Dùng `_context.Database.BeginTransactionAsync()` khi cần atomic nhiều bước.
- KHÔNG cần rollback thủ công trong catch — `Error()` và `CatchException()` đã tự rollback nếu `CurrentTransaction != null`.
- Nhớ `await transaction.CommitAsync()` ở cuối nhánh thành công.
