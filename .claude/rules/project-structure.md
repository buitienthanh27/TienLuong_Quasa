# Project Structure — API_Sample

Kiến trúc N-Tier 5 project. Dependency theo chiều: `WebApi → Application → Data`. `Models` và `Utilities` được tham chiếu chéo bởi mọi tầng.

```
API_Sample.sln
├── API_Sample.WebApi          (Presentation - entry point)
│   ├── Controllers/           Thin controllers (ProductController, AccountController)
│   ├── Middlewares/           SecurityHeadersMiddleware, Timezone/
│   ├── Lib/                   ClaimsPrincipalExtensions (User.GetAccountId), DataAnnotationExtensionMethod
│   ├── Logs/                  File log output
│   ├── Properties/            launchSettings.json
│   ├── Program.cs             DI, JWT, Swagger, AspNetCoreRateLimit, ValidationMiddleware, CORS
│   ├── appsettings*.json
│   └── web.config
│
├── API_Sample.Application     (Business Logic)
│   ├── Services/              S_Xxx.cs (mỗi file 1 interface IS_Xxx + 1 class S_Xxx)
│   ├── Ultilities/            BaseService<T>, JwtHelper, CallApi, SendMailSMTP, StoreProcedure, QueryableExtensions
│   ├── Mapper/                AutoMapperProfile.cs (toàn bộ CreateMap)
│   ├── ServiceExternal/       Wrapper gọi service ngoài
│   └── ExtensionMethods/
│
├── API_Sample.Data            (Data Access)
│   ├── EF/MainDbContext.cs    DbContext duy nhất, chứa toàn bộ DbSet<>
│   └── Entities/              Mỗi entity 1 file (Product.cs, Account.cs, Image.cs)
│
├── API_Sample.Models          (DTO Layer)
│   ├── Common/                ResponseData<T>, BaseModel, PagingRequestBase, Config_ApiSettings, Res_CustomIdName
│   ├── Request/               MReq_Xxx.cs (kèm class con MReq_Xxx_FullParam)
│   ├── Response/              MRes_Xxx.cs
│   └── Enums/
│
└── API_Sample.Utilities       (Cross-cutting)
    ├── Constants/             MessageErrorConstants, TimeZoneConstants
    ├── Encryptor.cs
    ├── StringHelper.cs        ToUrlClean, slug helper
    ├── Utilities.cs           CurrentTimeSeconds, helper chung
    └── DataException.cs
```

## Quy tắc khi thêm file mới

- **Controller** → `API_Sample.WebApi/Controllers/{Entity}Controller.cs` + register service trong `Program.cs`.
- **Service** → `API_Sample.Application/Services/S_{Entity}.cs` (interface + class trong cùng file, namespace `API_Sample.Application.Services`).
- **Entity** → `API_Sample.Data/Entities/{Entity}.cs`, thêm `DbSet<{Entity}>` vào `MainDbContext` và `CreateMap` trong `AutoMapperProfile`.
- **Request DTO** → `API_Sample.Models/Request/MReq_{Entity}.cs`. Class chính `MReq_{Entity}` kế thừa `BaseModel.History`. Class lọc đặt cùng file: `MReq_{Entity}_FullParam : PagingRequestBase`.
- **Response DTO** → `API_Sample.Models/Response/MRes_{Entity}.cs`.
- **Constants** → `API_Sample.Utilities/Constants/`.
- **Helper dùng nhiều tầng** → `API_Sample.Utilities/`. Helper chỉ tầng Application → `API_Sample.Application/Ultilities/`.

## Cấm

- Không tạo project mới (Domain/Infrastructure...) — giữ đúng 5 project.
- Không cho `Data` tham chiếu `Application` hoặc `WebApi`.
- Không đặt entity vào `Models`, không đặt DTO vào `Data`.
- Không tạo folder `Controllers/` trong project khác ngoài `WebApi`.
