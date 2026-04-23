# Clean Code — API_Sample

Áp dụng SOLID/DRY/KISS trong context dự án N-Tier service-based C#.

## SOLID

- **SRP**: Mỗi `S_X` chỉ phục vụ 1 entity. Logic cross-entity → tạo service riêng (vd `S_OrderFlow`) hoặc compose nhiều service trong service orchestrator.
- **OCP**: Thêm tính năng → thêm method mới vào `IS_X` + `S_X`, tránh sửa method cũ đã ổn định.
- **LSP**: Mọi `S_X` kế thừa `BaseService<S_X>` — không override hành vi `Error`/`CatchException`.
- **ISP**: Interface `IS_X` chỉ chứa method thật sự dùng. Không gom service khổng lồ.
- **DIP**: Controller phụ thuộc `IS_X`, không phụ thuộc `S_X` cụ thể.

## DRY

- Filter list logic → `#region Common functions` → hàm `BuildFilterQuery`.
- Constants thông báo lỗi → `MessageErrorConstants`.
- Slug/string/encrypt → `StringHelper`, `Encryptor`.
- AutoMapper config → `AutoMapperProfile.cs` duy nhất.

## KISS

- Không thêm pattern phức tạp (CQRS, Mediator, Repository) — dự án dùng Service trực tiếp với DbContext.
- Không tạo abstract layer trên `MainDbContext`.
- Method service ngắn gọn — pattern `validate → map → save → return`. Tách helper khi >50 dòng.

## Method guidelines

- 1 method = 1 mục đích nghiệp vụ.
- Tham số ≤ 4. Nhiều hơn → dùng DTO.
- Tránh boolean flag parameter — tách thành 2 method (vd `Delete` vs `UpdateStatus(id, -1, ...)`).

## Comment

- XML doc tiếng Việt cho method service public — mô tả mục đích nghiệp vụ, không lặp tên hàm.
- Inline comment chỉ khi giải thích **tại sao**, không phải **làm gì**.
- Bỏ code chết/comment cũ — không để code commented rải rác.

## Async

- `async` xuyên suốt từ Controller → Service → EF Core. Không trộn sync/async.
- `await` mọi Task — không `.Result`, `.Wait()`, `.GetAwaiter().GetResult()`.

## Error path

- Early return cho validation (`if (...) return Error(...)`) — không lồng `else`.
- Try/catch BAO QUANH toàn bộ method body, không nhỏ giọt nhiều try.

## Tránh

- Magic number — đưa vào constants hoặc enum.
- Nullable abuse — dùng `[Required]` ở DTO để ModelState bắt sớm.
- Trả về `null` từ service — luôn trả `ResponseData<T>` (data có thể null bên trong).
