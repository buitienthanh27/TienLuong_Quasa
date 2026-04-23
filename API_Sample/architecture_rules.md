# RESTful API Project Guidelines (API_Sample)

Tài liệu này mô tả chi tiết về Cấu trúc kiến trúc (Architecture), Quy tắc viết code (Coding Conventions), và Các chuẩn mực giao tiếp trong dự án `API_Sample`.

## 1. Cấu trúc Dự án (Project Structure)

Dự án áp dụng mô hình N-Tier (Kiến trúc phân tầng) nhằm đảm bảo Separation of Concerns (Phân tách mối quan tâm), giúp code dễ bảo trì, mở rộng và dễ test hơn.

*   **1. `API_Sample.WebApi` (Presentation Layer)**:
    *   Điểm đầu vào của ứng dụng. Chứa các `Controllers`, thiết lập cấu hình DI, Middlewares, xác thực (JWT), cấu hình Swagger và thiết lập thư viện giới hạn rate-limit (AspNetCoreRateLimit).
    *   **Nhiệm vụ**: Nhận Request từ HTTP, trích xuất danh tính người dùng qua JWT, gọi Application layer và trả về JSON chuẩn. Không chứa Business Logic.
*   **2. `API_Sample.Application` (Business Logic Layer)**:
    *   **Nhiệm vụ**: Chứa toàn bộ Business Logic, định nghĩa các Services nội bộ (`S_Product`, `S_Account`, ...).
    *   Các Services tại đây sẽ tương tác với `MainDbContext` để cập nhật Database và dùng AutoMapper (`AutoMapperProfile.cs`) để map DTO tới Entities.
    *   Kế thừa `BaseService<T>` để chuẩn hóa cách quản lý Transaction và xử lý ngoại lệ đồng nhất.
*   **3. `API_Sample.Data` (Data Access Layer)**:
    *   **Nhiệm vụ**: Quản lý truy cập Data bằng Entity Framework Core.
    *   Chứa `MainDbContext` và định nghĩa cấu trúc bảng dữ liệu (`Entities`).
*   **4. `API_Sample.Models` (Data Transfer Objects Layer)**:
    *   **Nhiệm vụ**: Tổ chức các Object/Model để trao đổi dữ liệu.
    *   Bao gồm: `Request` Models (Dữ liệu nhận từ Client), `Response` Models (Dữ liệu trả về cho Client), và `Common` (BaseModels, Chuẩn ResponseData).
*   **5. `API_Sample.Utilities` (Cross-cutting Concerns Layer)**:
    *   **Nhiệm vụ**: Cấu hình các đoạn mã có thể tái sử dụng ở bất kỳ tầng nào (Helper/Constants).
    *   Gồm: Chuỗi Constants, Mã hóa (Encryptor), Xử lý String (StringHelper).

---

## 2. Chuẩn hóa Phản hồi (Standard Response Data)

Mọi HTTP Request từ client đều sẽ nhận cùng một cấu trúc JSON trả về với lớp `ResponseData<T>`.

### Cấu trúc ResponseData
```csharp
public class ResponseData<T> {
    public int result { get; set; } // 0: Thất bại (Logic Error) | 1: Thành công | -1: System Exception
    public long time { get; set; }  // Thời gian Unix (Timestamp)
    public T data { get; set; }     // Payload chính
    public dynamic data2nd { get; set; } // Payload phụ (dùng cho Phân trang, meta info)
    public error error { get; set; } // Thông báo lỗi { code, message }
}
```

*   **Validation Request Middleware**: Trong `Program.cs`, tự động bắt lỗi `ModelState` và biến đổi nó thành chuỗi `ResponseData` với HTTP 400 và `Result = 0`.
*   **Controller Return**: Controller luôn trả về `Ok(res)` dựa trên kết quả của Service Application. Đảm bảo mã HTTP Request luôn trả về 200/201 (Ok) nhưng mã trạng thái xử lý thật sự sẽ bọc trong Result Model.

---

## 3. Quy tắc ở tầng Controller (WebApi)

*   **Thin Controllers**: Các Controller chỉ nên đóng vai trò là "Cửa ngõ dẫn đường". Mọi Logic điều hướng trực tiếp vào Service `_s_...` tương ứng.
*   **Authentication & Identity**: Bắt buộc gắn attribute `[Authorize]`. Sử dụng Extension Method `User.GetAccountId()` từ `Claims` (gắn trong JWT) để lấy định danh cá nhân ghi nhận cho các lệnh `CreatedBy`, `UpdatedBy`.
    ```csharp
    [HttpPost]
    public async Task<IActionResult> Create(MReq_Product request)
    {
        request.CreatedBy = User.GetAccountId();
        var res = await _s_Product.Create(request);
        return Ok(res);
    }
    ```

---

## 4. Quy tắc ở tầng Application (Services)

*   **Inheritance Constraint**: Tất cả Serves đều phải kế thừa class trừu tượng `BaseService<TService>` và tiêm phụ thuộc `IMapper`, `MainDbContext`, và `ILogger`.
*   **Error Handling (Xử lý lỗi Logic)**: Thay vì ném Exceptions, dùng hàm có sẵn `Error(HttpStatusCode statusCode, string message)` để thông báo lỗi Validate. Nếu có Transaction chưa rollback, hàm này sẽ tự động Rollback.
*   **System Exceptions (Xử lý ngoại lệ Server)**: Khi xảy ra exception ở tầng logic, bọc lại bằng catch và dùng hàm `CatchException(ex, Nameof(Function), parameterObject)`:
    *   Tự động ghi Log Lỗi.
    *   Tác dụng Rollback nếu có pending Transaction.
    *   Ghi vết tham số truyền vào bằng `Newtonsoft.Json` tiện dụng gỡ rối.
    ```csharp
    catch (Exception ex)
    {
        return CatchException(ex, nameof(Update), request);
    }
    ```
*   **Performance Conventions (Tối ưu hóa Truy Xuất CSDL)**:
    *   Dùng `.AsNoTracking()` cho bất kỳ câu lệnh `Get` / `Select` nào không cần `SaveChanges`.
    *   Dùng hàm `ProjectTo<T>(_mapper.ConfigurationProvider)` của AutoMapper trong query LINQ giúp Entity Framework tối ưu hóa mệnh đề SQL `SELECT` (chỉ convert những thuộc tính có sử dụng).
    *   Ưu tiên dùng `ExecuteUpdateAsync` / `ExecuteDeleteAsync` của EF Core thay vì `.Update()` + `.SaveChanges()` khi chỉ cần cập nhật dữ liệu hàng loạt không điều kiện (như update flag `Status = -1`).

---

## 5. Quy tắc Đặt Tên (Naming Conventions)

*   **Model Request**: Các Object dùng làm Input truyền vào qua API bắt buộc có tiền tố `MReq_` (ví dụ: `MReq_Product`, `MReq_Product_FullParam`). Nằm trong thư mục `API_Sample.Models/Request`.
*   **Model Response**: Dành cho Output DTO, có tiền tố `MRes_` (ví dụ: `MRes_Product`). Nằm trong thư mục `API_Sample.Models/Response`.
*   **Services**: Tiền tố với `S_` và interface `IS_`. (ví dụ: `IS_Product`, `S_Product`). Nằm bên trong dự án Application.
*   **Hàm Filter Dùng Chung**: Sẽ có khu vực `#region Common method` ở cuối các class Service (ví dụ: hàm `BuildFilterQuery(...)`) giúp tái sử dụng cấu trúc truy vấn ở `GetListByPaging`, `GetListFullParam`. Hàm này luôn trả về kiểu `IQueryable<TEntity>`.
