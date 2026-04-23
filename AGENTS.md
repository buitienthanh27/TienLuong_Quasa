# Codex AI Configuration — API_Sample (EcoTech2A)

## Superpowers Methodology (BẮT BUỘC)

Dự án này sử dụng **Superpowers** methodology. Agent PHẢI tuân thủ workflow:

1. **Brainstorming** → Hiểu rõ yêu cầu trước khi code
2. **Writing Plans** → Tạo implementation plan chi tiết
3. **TDD** → Test trước, code sau
4. **Verification** → Verify trước khi claim done

**Commands:** `/brainstorm`, `/write-plan`, `/execute-plan`, `/review`

**Chi tiết:** Xem `.Codex/rules/superpowers.md`

---

## Karpathy Guidelines (BẮT BUỘC)

Áp dụng 4 nguyên tắc từ [Andrej Karpathy](https://x.com/karpathy/status/2015883857489522876) để tránh lỗi LLM coding phổ biến.

### 1. Suy nghĩ trước khi code (Think Before Coding)

**Không giả định. Không che giấu sự nhầm lẫn. Nêu rõ tradeoffs.**

- Nêu rõ assumptions trước khi implement. Nếu không chắc → **HỎI**.
- Nếu có nhiều cách hiểu → trình bày tất cả, không tự chọn im lặng.
- Nếu có cách đơn giản hơn → đề xuất. Push back khi cần.
- Nếu có gì không rõ → **DỪNG LẠI**, nêu rõ điểm chưa hiểu và hỏi.

### 2. Đơn giản là nhất (Simplicity First)

**Code tối thiểu để giải quyết vấn đề. Không viết code đoán trước.**

- **KHÔNG** thêm feature ngoài yêu cầu.
- **KHÔNG** tạo abstraction cho code chỉ dùng 1 lần.
- **KHÔNG** thêm "flexibility" hay "configurability" không được yêu cầu.
- **KHÔNG** xử lý error cho scenarios không thể xảy ra.
- Nếu viết 200 dòng mà có thể 50 dòng → **viết lại**.

**Kiểm tra:** "Senior engineer có nói code này phức tạp quá không?" Nếu có → đơn giản hoá.

### 3. Sửa đổi chính xác (Surgical Changes)

**Chỉ sửa những gì cần thiết. Chỉ dọn dẹp mess của chính mình.**

Khi sửa code có sẵn:
- **KHÔNG** "improve" code, comment, formatting xung quanh.
- **KHÔNG** refactor thứ không bị hỏng.
- **Match existing style**, dù bạn sẽ làm khác đi.
- Nếu thấy dead code không liên quan → **đề cập**, không xoá.

Khi thay đổi của bạn tạo orphans:
- Xoá imports/variables/functions mà **THAY ĐỔI CỦA BẠN** làm unused.
- **KHÔNG** xoá dead code có sẵn trừ khi được yêu cầu.

**Kiểm tra:** Mỗi dòng thay đổi phải trace trực tiếp về yêu cầu của user.

### 4. Thực thi hướng mục tiêu (Goal-Driven Execution)

**Định nghĩa success criteria. Loop cho đến khi verify.**

Chuyển đổi task thành mục tiêu có thể verify:

| Thay vì... | Chuyển thành... |
|------------|-----------------|
| "Thêm validation" | "Viết test cho invalid inputs, sau đó làm pass" |
| "Fix bug" | "Viết test reproduce bug, sau đó làm pass" |
| "Refactor X" | "Đảm bảo tests pass trước và sau" |

Với multi-step tasks, nêu brief plan:
```
1. [Bước] → verify: [kiểm tra]
2. [Bước] → verify: [kiểm tra]
3. [Bước] → verify: [kiểm tra]
```

**Strong success criteria** → có thể loop độc lập. **Weak criteria** ("làm cho nó work") → cần clarification liên tục.

---

## Project Overview
Dự án **API_Sample** — RESTful API backend theo mô hình **N-Tier (Layered Architecture)** dùng .NET / ASP.NET Core.

- **Architecture**: 5 project N-Tier (WebApi → Application → Data, dùng chung Models + Utilities).
- **Backend Stack**: ASP.NET Core, EF Core (`MainDbContext`), AutoMapper, JWT Authentication, AspNetCoreRateLimit, Swagger, Newtonsoft.Json, Serilog/ILogger.
- **Pattern**: Controller (thin) → Service (`S_xxx` : `BaseService<T>`) → DbContext / Entities.
- **Response**: Mọi API trả về `ResponseData<T>` chuẩn hoá (`result`, `time`, `data`, `data2nd`, `error`).
- **Language**: Giao tiếp với user và comment code bằng **Tiếng Việt**, code identifier tiếng Anh.

## Project Layout
| Project | Vai trò |
|---------|---------|
| `API_Sample.WebApi` | Presentation: Controllers, Program.cs, Middlewares, JWT, Swagger, RateLimit |
| `API_Sample.Application` | Business logic: `Services/S_xxx.cs`, `BaseService<T>`, `Mapper/AutoMapperProfile.cs`, `Ultilities/` (StoreProcedure, JwtHelper, CallApi, SendMailSMTP) |
| `API_Sample.Data` | Data access: `EF/MainDbContext.cs`, `Entities/` |
| `API_Sample.Models` | DTO: `Request/MReq_*`, `Response/MRes_*`, `Common/` (ResponseData, BaseModel, PagingRequestBase), `Enums/` |
| `API_Sample.Utilities` | Cross-cutting: `Constants/`, `Encryptor.cs`, `StringHelper.cs`, `Utilities.cs`, `DataException.cs` |

## Core Instructions
- Luôn phản hồi user bằng **Tiếng Việt**.
- Luôn đọc **tất cả file trong `.Codex/rules/`** trước khi sinh code mới hoặc sửa code.
- Tuân thủ tuyệt đối tài liệu gốc [architecture_rules.md](../architecture_rules.md) — đây là source of truth.
- **KHÔNG** viết business logic trong Controller — chỉ gọi service `_s_Xxx`.
- **KHÔNG** dùng Minimal API — dự án dùng `[ApiController]` + `ControllerBase`.
- **KHÔNG** ném `throw new Exception` ở tầng Service — dùng `Error(HttpStatusCode, message)` cho lỗi nghiệp vụ và `CatchException(ex, nameof(Func), param)` cho exception.
- **KHÔNG** trả về `BadRequest()`, `NotFound()` trong Controller — luôn `return Ok(res)` với `ResponseData<T>` bên trong.
- **KHÔNG** đặt tên class/file sai prefix (`MReq_`, `MRes_`, `S_`, `IS_`).
- **KHÔNG** quên `.AsNoTracking()` cho query read-only và `ProjectTo<T>(_mapper.ConfigurationProvider)` khi map list.
- **KHÔNG** dùng `.Update()` + `SaveChanges()` cho cập nhật hàng loạt — dùng `ExecuteUpdateAsync` / `ExecuteDeleteAsync`.
- **KHÔNG** hardcode `CreatedBy`/`UpdatedBy` — luôn lấy từ `User.GetAccountId()` ở Controller rồi gán vào request DTO.

## Mandatory Rules (`.Codex/rules/`)

### Architecture & Structure
- `project-structure.md` — Cấu trúc 5-project N-Tier và dependency direction.
- `tech-stack.md` — Stack đã được phê duyệt (ASP.NET Core, EF Core, AutoMapper, JWT, AspNetCoreRateLimit).
- `system-design.md` — Pattern Controller → Service → DbContext, `ResponseData<T>`, transaction/rollback flow.
- `api-conventions.md` — Thin controller, `[Route("[controller]/[action]")]`, `[Authorize]`, HTTP verbs, paging.

### Naming & Style
- `naming-conventions.md` — Prefix `MReq_`, `MRes_`, `S_`, `IS_`, field service `_s_Xxx`, file 1 class chính.
- `code-style.md` — PascalCase / camelCase, namespace theo folder, region `#region Common functions`, XML doc tiếng Việt cho method service.

### Data
- `database.md` — EF Core, `MainDbContext`, snake_case column với `[Column]`, `Status` short (-1 = xoá mềm), `CreatedAt/By`/`UpdatedAt/By`, stored procedure qua `StoreProcedure.GetListAsync`.

### Quality & Operations
- `clean-code.md` — SOLID, DRY, KISS áp dụng cho service.
- `error-handling.md` — `BaseService.Error()`, `CatchException()`, `MessageErrorConstants`, validation middleware.
- `security.md` — JWT bắt buộc, `[Authorize]` mặc định, `Encryptor`, rate-limit, security headers middleware.
- `monitoring.md` — `ILogger<TService>` thông qua `BaseService`, log có serialize parameter.
- `testing.md` — Conventions test (nếu có) — hiện chưa setup unit test framework.
- `git-workflow.md` — Quy ước branch / commit (chưa init git).

## Key Conventions Cheatsheet

### Controller mẫu
```csharp
[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IS_Product _s_Product;
    public ProductController(IS_Product s_Product) => _s_Product = s_Product;

    [HttpPost]
    public async Task<IActionResult> Create(MReq_Product request)
    {
        request.CreatedBy = User.GetAccountId();
        var res = await _s_Product.Create(request);
        return Ok(res);
    }
}
```

### Service mẫu
```csharp
public class S_Product : BaseService<S_Product>, IS_Product
{
    private readonly IMapper _mapper;
    public S_Product(MainDbContext context, IMapper mapper, ILogger<S_Product> logger)
        : base(context, logger) => _mapper = mapper;

    public async Task<ResponseData<MRes_Product>> Create(MReq_Product request)
    {
        try
        {
            // 1. Validate trùng lặp
            if (await _context.Products.AnyAsync(x => x.Code == request.Code && x.Status != -1))
                return Error(HttpStatusCode.Conflict, "Mã trùng lặp!");

            // 2. Map + set audit
            var data = _mapper.Map<Product>(request);
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = request.CreatedBy;

            _context.Products.Add(data);
            if (await _context.SaveChangesAsync() == 0)
                return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

            return new ResponseData<MRes_Product>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
            { data = _mapper.Map<MRes_Product>(data) };
        }
        catch (Exception ex) { return CatchException(ex, nameof(Create), request); }
    }
}
```

## Agent Behavior
- Trước khi tạo entity / DTO / service / controller mới: đọc lại các rule liên quan trong `.Codex/rules/`.
- Khi thêm Service mới: đăng ký DI trong `Program.cs` (`builder.Services.AddScoped<IS_Xxx, S_Xxx>()`).
- Khi thêm Entity mới: thêm `DbSet<T>` vào `MainDbContext` và profile mapping trong `AutoMapperProfile.cs`.
- Khi thêm Request DTO: kế thừa `BaseModel.History` (có `Status`, `CreatedAt/By`, `UpdatedAt/By`) hoặc `PagingRequestBase` (cho query list).
- Mọi method service `Get*` phải dùng `.AsNoTracking()`. Mọi list response nên dùng `ProjectTo<MRes_X>`.
- Cập nhật hàng loạt → `ExecuteUpdateAsync`. Xoá mềm → set `Status = -1`. Xoá cứng (hiếm) → `ExecuteDeleteAsync`.

## Dấu hiệu Guidelines đang hoạt động

Khi tuân thủ Superpowers + Karpathy Guidelines, bạn sẽ thấy:

- **Ít thay đổi thừa trong diff** — chỉ có changes được yêu cầu
- **Ít phải viết lại do phức tạp hoá** — code đơn giản ngay từ đầu
- **Câu hỏi clarifying đến TRƯỚC implementation** — không phải sau khi mắc lỗi
- **PRs sạch, tối thiểu** — không có drive-by refactoring hay "improvements"
- **Evidence trước assertions** — verify xong mới claim done
