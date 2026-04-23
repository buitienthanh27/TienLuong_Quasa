# Code Style — API_Sample

## Format

- Indent 4 space (C#). UTF-8, CRLF (Windows).
- Brace mới dòng (Allman) — khớp code hiện tại.
- 1 file 1 class chính (ngoại lệ: file Service chứa cả `IS_X` và `S_X`; file `MReq_X.cs` chứa cả `MReq_X` và `MReq_X_FullParam`).

## Using

- Sắp xếp using ngoài namespace, group hợp lý (Microsoft.* → System.* → bên thứ 3 → project nội bộ).

## Namespace

- Khớp đường dẫn folder: `API_Sample.{Project}.{Folder}`.
- Ưu tiên block-scoped namespace `namespace X { ... }` (xem code hiện tại). Entity dùng file-scoped `namespace X;` cũng được — đồng nhất theo file lân cận.

## XML doc comment (tiếng Việt)

Method service public bắt buộc có summary tiếng Việt mô tả mục đích nghiệp vụ:

```csharp
/// <summary>
/// Hàm tạo mới bản ghi, kiểm tra trùng mã trước khi insert.
/// </summary>
public async Task<ResponseData<MRes_Product>> Create(MReq_Product request) { ... }
```

## Region

- Cuối class Service: `#region Common functions ... #endregion` cho hàm private dùng chung (vd `BuildFilterQuery`).
- Không lạm dụng region cho method ngắn.

## LINQ & async

- `await` mọi async — không `.Result`, `.Wait()`.
- Đặt `.AsNoTracking()` ngay sau `_context.X` cho query read-only.
- `ProjectTo<MRes_X>(_mapper.ConfigurationProvider)` cho query trả DTO list — KHÔNG `ToListAsync()` rồi `_mapper.Map`.
- Filter động: dùng helper `BuildFilterQuery(request)` trả `IQueryable<TEntity>`, gọi `.CountAsync()` rồi `.Skip().Take()`.

## Strings & helpers

- Trim + ToUpper cho mã (`request.Code = request.Code?.Trim().ToUpper()`) trước khi check trùng.
- Slug: `StringHelper.ToUrlClean(request.Name)`.
- Time: dùng `DateTime.UtcNow` cho `CreatedAt/UpdatedAt`. Hiển thị timezone xử lý ở client hoặc middleware.
- Unix timestamp: `Utilities.Utilities.CurrentTimeSeconds()`.

## DTO ↔ Entity

- AutoMapper. Mọi `CreateMap<MReq_X, X>()` / `CreateMap<X, MRes_X>()` đặt trong `AutoMapperProfile.cs`.
- KHÔNG manual map từng property trong Service nếu AutoMapper đủ — chỉ override field đặc biệt sau khi `_mapper.Map`.

## Cấm

- Không dùng `var` cho biến primitive nếu giảm rõ ràng (giữ đúng style hiện tại — `int count = ...`, `List<T> data = ...`).
- Không `Console.WriteLine` để debug — dùng `_logger`.
- Không dùng `dynamic` ngoài trường `data2nd` của `ResponseData`.
- Không nuốt exception bằng `catch {}` rỗng.
