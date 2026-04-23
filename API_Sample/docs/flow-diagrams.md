# API_Sample — Sơ đồ luồng code & Hướng dẫn test API

> Tài liệu này giúp bạn **hình dung** cách một request đi xuyên qua hệ thống và **test thực tế** để thấy dữ liệu di chuyển.

---

## 1. Kiến trúc tổng quan (Component diagram)

```mermaid
flowchart LR
    Client[🌐 Client<br/>Postman / Swagger / FE]

    subgraph WebApi["📦 API_Sample.WebApi (Presentation)"]
        MW[Middleware Pipeline<br/>SecurityHeaders → RateLimit → JWT → Validation]
        CTRL[Controllers<br/>ProductController, AccountController]
        EXT[Lib/ClaimsPrincipalExtensions<br/>User.GetAccountId]
    end

    subgraph App["⚙️ API_Sample.Application (Business Logic)"]
        SVC[Services<br/>S_Product, S_Account : BaseService&lt;T&gt;]
        BASE[BaseService<br/>Error / CatchException]
        MAP[AutoMapperProfile]
        JWT[JwtHelper]
        SP[StoreProcedure helper]
    end

    subgraph Data["💾 API_Sample.Data (Data Access)"]
        CTX[MainDbContext]
        ENT[Entities<br/>Product, Account, Image]
    end

    subgraph Models["📨 API_Sample.Models (DTO)"]
        REQ[Request/MReq_*]
        RES[Response/MRes_*]
        COMMON[Common/ResponseData&lt;T&gt;<br/>BaseModel, PagingRequestBase]
    end

    subgraph Util["🛠 API_Sample.Utilities"]
        CONST[Constants]
        HELP[StringHelper, Encryptor]
    end

    DB[(🗄 SQL Server)]

    Client -->|HTTP + JWT| MW
    MW --> CTRL
    CTRL --> SVC
    CTRL -.uses.-> EXT
    CTRL -.nhận.-> REQ
    CTRL -.trả.-> COMMON

    SVC --> BASE
    SVC --> MAP
    SVC --> CTX
    SVC -.dùng.-> JWT
    SVC -.dùng.-> SP
    SVC -.map.-> RES

    CTX --> ENT
    CTX --> DB

    App -.dùng.-> Util
    WebApi -.dùng.-> Util
```

---

## 2. Middleware pipeline (Request đi từ client vào)

```mermaid
flowchart TD
    A[🌐 HTTP Request] --> B[SecurityHeadersMiddleware<br/>Set X-Frame-Options, CSP...]
    B --> C[AspNetCoreRateLimit<br/>Check IP quota]
    C -->|Vượt quota| CX[❌ 429 Too Many Requests]
    C -->|OK| D[Authentication<br/>Validate JWT Bearer]
    D -->|Token invalid| DX[❌ 401 Unauthorized]
    D -->|OK| E[Authorization<br/>Check Authorize attr]
    E -->|Không quyền| EX[❌ 403 Forbidden]
    E -->|OK| F[ValidationMiddleware<br/>Check ModelState]
    F -->|ModelState invalid| FX[⚠️ 400 + ResponseData result=0]
    F -->|OK| G[✅ Controller Action]
```

---

## 3. Luồng POST /Product/Create (Sequence — trường hợp thành công)

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant MW as Middleware Pipeline
    participant CTRL as ProductController
    participant EXT as ClaimsPrincipalExtensions
    participant SVC as S_Product
    participant BASE as BaseService
    participant MAP as IMapper
    participant CTX as MainDbContext
    participant DB as SQL Server

    Client->>MW: POST /Product/Create<br/>Header: Bearer JWT<br/>Body: MReq_Product
    MW->>MW: SecurityHeaders → RateLimit<br/>→ JWT validate → Authorization<br/>→ ModelState check
    MW->>CTRL: Invoke Create(request)

    CTRL->>EXT: User.GetAccountId()
    EXT-->>CTRL: accountId (int, từ JWT claim)
    CTRL->>CTRL: request.CreatedBy = accountId

    CTRL->>SVC: await _s_Product.Create(request)

    Note over SVC: try { ... }
    SVC->>SVC: request.Code = Code?.Trim().ToUpper()
    SVC->>CTX: Products.AnyAsync(x => x.Code == ... && x.Status != -1)
    CTX->>DB: SELECT 1 FROM Product WHERE code=@p AND status<>-1
    DB-->>CTX: false
    CTX-->>SVC: isExistsCode = false

    SVC->>MAP: Map<Product>(request)
    MAP-->>SVC: Product entity

    SVC->>CTX: Products.MaxAsync(x => x.Id)
    CTX->>DB: SELECT MAX(id) FROM Product
    DB-->>SVC: maxId
    SVC->>SVC: data.Id = maxId + 1<br/>data.NameSlug = ToUrlClean(Name)<br/>data.CreatedAt = UtcNow<br/>data.CreatedBy = request.CreatedBy

    SVC->>CTX: Products.Add(data)
    SVC->>CTX: SaveChangesAsync()
    CTX->>DB: INSERT INTO Product(...)
    DB-->>CTX: 1 row affected
    CTX-->>SVC: save = 1

    SVC->>MAP: Map<MRes_Product>(data)
    MAP-->>SVC: MRes_Product dto

    SVC-->>CTRL: ResponseData<MRes_Product><br/>{ result:1, data: dto, error:{code:201,...} }
    CTRL-->>MW: return Ok(res)
    MW-->>Client: HTTP 200<br/>JSON ResponseData
```

---

## 4. Luồng lỗi nghiệp vụ — Mã trùng (Error branch)

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant CTRL as ProductController
    participant SVC as S_Product
    participant BASE as BaseService
    participant CTX as MainDbContext
    participant DB as SQL Server

    Client->>CTRL: POST /Product/Create<br/>Code="P01" (đã tồn tại)
    CTRL->>SVC: Create(request)
    SVC->>CTX: AnyAsync(Code == "P01")
    CTX->>DB: SELECT 1 ...
    DB-->>SVC: true (đã có)

    SVC->>BASE: Error(HttpStatusCode.Conflict, "Mã trùng lặp!")
    Note over BASE: Check CurrentTransaction<br/>→ rollback nếu có
    BASE-->>SVC: ErrorResponseBase<br/>{ IsException:false, Status:409, Message }

    Note over SVC: implicit cast ErrorResponseBase → ResponseData<T>
    SVC-->>CTRL: ResponseData<MRes_Product><br/>{ result:0, error:{code:409,...} }
    CTRL-->>Client: HTTP 200<br/>result=0 → FE hiện toast lỗi
```

---

## 5. Luồng exception hệ thống (Catch branch)

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant CTRL as ProductController
    participant SVC as S_Product
    participant BASE as BaseService
    participant LOG as ILogger
    participant CTX as MainDbContext
    participant DB as SQL Server

    Client->>CTRL: POST /Product/Create
    CTRL->>SVC: Create(request)
    SVC->>CTX: SaveChangesAsync()
    CTX->>DB: INSERT ...
    DB--xCTX: ❌ SqlException (DB down / constraint)
    CTX--xSVC: throw

    Note over SVC: catch (Exception ex)
    SVC->>BASE: CatchException(ex, nameof(Create), request)

    BASE->>CTX: CurrentTransaction?.Rollback()
    BASE->>BASE: JsonConvert.SerializeObject(request)
    BASE->>LOG: LogError(ex, "S_Product.Create Exception_Logger. Parameters: {...}")
    BASE-->>SVC: ErrorResponseBase<br/>{ IsException:true, Status:500 }

    SVC-->>CTRL: ResponseData<br/>{ result:-1, error:{code:500,...} }
    CTRL-->>Client: HTTP 200<br/>result=-1
```

---

## 6. Luồng GET /Product/GetListByPaging (query + paging)

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant CTRL as ProductController
    participant SVC as S_Product
    participant CTX as MainDbContext
    participant DB as SQL Server

    Client->>CTRL: GET /Product/GetListByPaging<br/>?Page=1&Record=10&SearchText=abc
    CTRL->>SVC: GetListByPaging(request)

    SVC->>SVC: BuildFilterQuery(request)
    Note over SVC: query = Products.AsNoTracking()<br/>+ filter Status<br/>+ filter NameSlug/Code

    SVC->>CTX: query.CountAsync()
    CTX->>DB: SELECT COUNT(*) ... WHERE ...
    DB-->>SVC: count = 37

    SVC->>CTX: query.OrderBy(Sort)<br/>.Skip(0).Take(10)<br/>.ProjectTo<MRes_Product>()
    CTX->>DB: SELECT (chỉ field MRes) ... ORDER BY sort OFFSET 0 FETCH 10
    DB-->>SVC: List<MRes_Product> (10 bản ghi)

    SVC-->>CTRL: ResponseData<List<MRes_Product>><br/>{ result:1, data: [...], data2nd: 37 }
    CTRL-->>Client: HTTP 200<br/>data = 10 items, data2nd = tổng count
```

> **Chú ý**: `data2nd = 37` → FE dùng tính tổng số trang = `Math.ceil(37/10) = 4`.

---

## 7. Luồng PUT /Product/UpdateStatusList (bulk update)

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant CTRL as ProductController
    participant SVC as S_Product
    participant CTX as MainDbContext
    participant DB as SQL Server

    Client->>CTRL: PUT /Product/UpdateStatusList<br/>?sequenceIds=[1,2,3]&status=0
    CTRL->>CTRL: updatedBy = User.GetAccountId()
    CTRL->>SVC: UpdateStatusList("[1,2,3]", 0, updatedBy)

    SVC->>SVC: JsonConvert.DeserializeObject<List<int>>(sequenceIds)
    Note over SVC: ids = [1, 2, 3]

    SVC->>CTX: Products.Where(ids.Contains(Id))<br/>.ExecuteUpdateAsync(SetProperty Status, UpdatedBy, UpdatedAt)
    CTX->>DB: UPDATE Product SET status=0, updated_by=X, updated_at=Y<br/>WHERE id IN (1,2,3)
    DB-->>SVC: updatedCount = 3

    SVC->>CTX: Products.AsNoTracking().Where(ids.Contains(Id)).ToListAsync()
    CTX->>DB: SELECT * FROM Product WHERE id IN (1,2,3)
    DB-->>SVC: List<Product>

    SVC-->>CTRL: ResponseData<List<MRes_Product>><br/>{ result:1, data:[3 items] }
    CTRL-->>Client: HTTP 200
```

---

## 8. Luồng xác thực JWT (login → gọi API có token)

```mermaid
sequenceDiagram
    autonumber
    actor Client
    participant CTRL as AccountController
    participant SVC as S_Account
    participant CTX as MainDbContext
    participant JWT as JwtHelper
    participant ENC as Encryptor

    Note over Client,ENC: === Giai đoạn 1: LOGIN ===
    Client->>CTRL: POST /Account/Login<br/>Body: { username, password }<br/>[AllowAnonymous]
    CTRL->>SVC: Login(request)
    SVC->>ENC: HashPassword(request.Password)
    ENC-->>SVC: hash
    SVC->>CTX: Accounts.FirstOrDefault(Username && PasswordHash)
    CTX-->>SVC: account hoặc null

    alt Sai mật khẩu
        SVC-->>CTRL: Error(401, "Sai tài khoản/mật khẩu")
        CTRL-->>Client: result=0
    else OK
        SVC->>JWT: GenerateToken(account.Id, roles...)
        JWT-->>SVC: accessToken + refreshToken
        SVC-->>CTRL: ResponseData<MRes_Token>
        CTRL-->>Client: result=1, data={token,...}
    end

    Note over Client,ENC: === Giai đoạn 2: GỌI API CÓ [Authorize] ===
    Client->>CTRL: GET /Product/GetById/1<br/>Header: Authorization: Bearer {token}
    Note over CTRL: Middleware JWT validate token<br/>→ gán HttpContext.User với claims
    CTRL->>CTRL: User.GetAccountId() → đọc claim "AccountId"
    CTRL-->>Client: ResponseData<MRes_Product>
```

---

## 9. Cheatsheet: Map code ↔ sơ đồ

| Trên sơ đồ | File code |
|------------|-----------|
| Middleware pipeline | [API_Sample.WebApi/Program.cs](../API_Sample.WebApi/Program.cs) |
| SecurityHeaders | [API_Sample.WebApi/Middlewares/SecurityHeadersMiddleware.cs](../API_Sample.WebApi/Middlewares/SecurityHeadersMiddleware.cs) |
| `User.GetAccountId()` | [API_Sample.WebApi/Lib/ClaimsPrincipalExtensions.cs](../API_Sample.WebApi/Lib/ClaimsPrincipalExtensions.cs) |
| Controller | [API_Sample.WebApi/Controllers/ProductController.cs](../API_Sample.WebApi/Controllers/ProductController.cs) |
| Service | [API_Sample.Application/Services/S_Product.cs](../API_Sample.Application/Services/S_Product.cs) |
| `Error` / `CatchException` | [API_Sample.Application/Ultilities/BaseService.cs](../API_Sample.Application/Ultilities/BaseService.cs) |
| AutoMapper | [API_Sample.Application/Mapper/AutoMapperProfile.cs](../API_Sample.Application/Mapper/AutoMapperProfile.cs) |
| DbContext | [API_Sample.Data/EF/MainDbContext.cs](../API_Sample.Data/EF/MainDbContext.cs) |
| Entity | [API_Sample.Data/Entities/Product.cs](../API_Sample.Data/Entities/Product.cs) |
| DTO REQ / RES | [API_Sample.Models/Request/MReq_Product.cs](../API_Sample.Models/Request/MReq_Product.cs) / [API_Sample.Models/Response/MRes_Product.cs](../API_Sample.Models/Response/MRes_Product.cs) |
| Response wrapper | [API_Sample.Models/Common/ResponseData.cs](../API_Sample.Models/Common/ResponseData.cs) |
| JWT helper | [API_Sample.Application/Ultilities/JwtHelper.cs](../API_Sample.Application/Ultilities/JwtHelper.cs) |

---

## 10. Hướng dẫn test API để thấy code chạy

### 10.1. Chuẩn bị
1. Mở SQL Server, chạy migration hoặc attach DB:
   ```bash
   dotnet ef database update --project API_Sample.Data --startup-project API_Sample.WebApi
   ```
2. Chỉnh `appsettings.Development.json` → connection string đúng.
3. Chạy WebApi:
   ```bash
   dotnet run --project API_Sample.WebApi
   ```
4. Mở Swagger: `https://localhost:{port}/swagger`.

### 10.2. Test theo kịch bản (theo đúng thứ tự)

#### Bước 1 — Login lấy token
- Swagger → `POST /Account/Login` → Try it out → nhập username/password → Execute.
- Copy giá trị `data.token` từ response.
- Bấm nút **Authorize** (🔒) ở góc trên Swagger → dán `Bearer {token}` → Authorize.

#### Bước 2 — Test Create (happy path)
- `POST /Product/Create` với body:
  ```json
  {
    "code": "TEST01",
    "name": "Sản phẩm test",
    "sort": 1,
    "ratioTransfer": 1.5,
    "remark": "Test flow"
  }
  ```
- Kỳ vọng: `result: 1`, `data.id` là số mới, `data.nameSlug = "san-pham-test"`.
- Check DB: `SELECT * FROM Product WHERE code='TEST01'` → thấy record mới có `created_by` = id của account login.

#### Bước 3 — Test Create (error — mã trùng)
- Lặp lại request bước 2 với cùng `code=TEST01`.
- Kỳ vọng: HTTP 200 nhưng `result: 0`, `error.code: 409`, `error.message: "Mã trùng lặp!"`.

#### Bước 4 — Test GetListByPaging
- `GET /Product/GetListByPaging?Page=1&Record=5&SequenceStatus=1&SearchText=test`.
- Kỳ vọng: `data` = array tối đa 5, `data2nd` = tổng số bản ghi match.

#### Bước 5 — Test UpdateStatusList (bulk)
- `PUT /Product/UpdateStatusList?sequenceIds=[1,2,3]&status=0`.
- Check DB: 3 record có `status=0`, `updated_at` mới.

#### Bước 6 — Test Delete (xoá mềm)
- `DELETE /Product/Delete?id=X` → thực chất gọi `UpdateStatus(id, -1, ...)`.
- Check DB: `status = -1`.

#### Bước 7 — Test exception (giả lập)
- Tắt SQL Server → gọi lại `GET /Product/GetById/1`.
- Kỳ vọng: `result: -1`, log xuất hiện trong `API_Sample.WebApi/Logs/` với format `S_Product.GetById Exception_Logger. Parameters: {...}`.

### 10.3. Debug bằng Visual Studio / Rider để **thấy** flow

1. Đặt breakpoint ở 4 chỗ:
   - `ProductController.Create` dòng `request.CreatedBy = ...`
   - `S_Product.Create` dòng đầu tiên của `try`
   - `S_Product.Create` dòng `_context.Products.Add(data)`
   - `BaseService.Error` hoặc `CatchException`
2. Chạy debug (F5) → gọi API từ Swagger.
3. Dùng **F10** (step over) / **F11** (step into) để đi qua từng dòng.
4. Cửa sổ **Locals/Watch** xem giá trị `request`, `data`, `save`.
5. Khi đến `SaveChangesAsync`, mở **SQL Server Profiler** hoặc bật EF logging để thấy câu SQL thực thi.

### 10.4. Bật EF Core log SQL (dev)
Trong `Program.cs` (hoặc `appsettings.Development.json`):
```csharp
builder.Services.AddDbContext<MainDbContext>(opt =>
    opt.UseSqlServer(connStr)
       .LogTo(Console.WriteLine, LogLevel.Information)
       .EnableSensitiveDataLogging()); // chỉ dev
```
→ Mỗi request sẽ in câu SQL ra console, giúp bạn đối chiếu với sơ đồ ở mục 3–7.

### 10.5. Postman collection (gợi ý)
Tạo collection với biến môi trường `{{baseUrl}}` và `{{token}}`. Thứ tự chạy:
1. Login → **Tests** tab tự set `pm.environment.set("token", pm.response.json().data.token)`.
2. Các request sau dùng header `Authorization: Bearer {{token}}`.
3. Chạy Collection Runner để test toàn bộ luồng CRUD tự động.

---

## 11. Bài tập tự luyện

1. **Vẽ lại sequence** cho `AccountController.Login` dựa trên code thật → so sánh với mục 8.
2. **Trace log**: cố ý truyền `sequenceIds = "abcxyz"` (không parse được JSON) → xem log + response trả về đi qua nhánh nào (`Error` hay `CatchException`?). Giải thích vì sao.
3. **Thêm entity mới** `Category`: tạo 5 file (Entity, MReq, MRes, Service, Controller), chạy migration, test CRUD → bạn đã nắm trọn pattern.

---

> Sau khi quen, bạn chỉ cần nhìn 1 API là hình dung được chuỗi: **Middleware → Controller → Service → DbContext → DB → ResponseData<T>**.
