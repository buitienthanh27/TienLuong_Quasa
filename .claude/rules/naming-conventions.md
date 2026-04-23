# Naming Conventions — API_Sample

Quy ước đặt tên BẮT BUỘC. Vi phạm = reject code.

## Prefix bắt buộc

| Loại | Prefix | Ví dụ | Vị trí |
|------|--------|-------|--------|
| Request DTO | `MReq_` | `MReq_Product`, `MReq_Account_Login` | `API_Sample.Models/Request/` |
| Request DTO (filter list) | `MReq_..._FullParam` | `MReq_Product_FullParam` | cùng file với `MReq_Product` |
| Response DTO | `MRes_` | `MRes_Product`, `MRes_Token` | `API_Sample.Models/Response/` |
| Service interface | `IS_` | `IS_Product`, `IS_Account` | `API_Sample.Application/Services/` |
| Service implementation | `S_` | `S_Product`, `S_Account` | cùng file với `IS_xxx` |
| Service field trong Controller | `_s_` (camelCase phần sau) | `_s_Product`, `_s_Account` | private readonly trong Controller |
| Stored procedure | `sp_{entity}_{action}_by_{param}` | `sp_product_getlist_by_fullparam` | DB |
| Constants class | `{Topic}Constants` | `MessageErrorConstants`, `TimeZoneConstants` | `API_Sample.Utilities/Constants/` |
| Config class | `Config_{Name}` | `Config_ApiSettings` | `API_Sample.Models/Common/` |

## C# code style

- **Class / Method / Property / Public field**: `PascalCase`.
- **Local variable / parameter**: `camelCase`.
- **Private readonly field**: prefix `_` + `camelCase` (`_context`, `_mapper`, `_logger`, `_s_Product`).
- **Constants**: `UPPER_SNAKE_CASE` (`CREATE_SUCCESS`, `DO_NOT_FIND_DATA`).
- **Async method**: KHÔNG cần hậu tố `Async` (dự án bỏ qua quy ước này — xem `Create`, `Update`, `GetById`).
- **Service interface**: dùng `IS_` (double prefix).
- **File**: 1 file 1 class chính. Service đặc biệt: 1 file chứa cả `IS_X` và `S_X`.

## Database / Entity

- **Table**: `[Table("Product")]` PascalCase, số ít.
- **Column**: `[Column("created_at")]` snake_case.
- **PK**: `Id` (int).
- **FK**: `{Entity}Id` (PascalCase property), column `{entity}_id` (snake_case).
- **Audit**: `CreatedAt`, `CreatedBy` (int, required); `UpdatedAt`, `UpdatedBy` (int?, nullable); `Status` (short).
- **Slug**: `{Field}Slug` (vd `NameSlug`).

## Controller

- Tên: `{Entity}Controller` (số ít), kế thừa `ControllerBase`.
- Action: PascalCase động từ — `Create`, `Update`, `UpdateStatus`, `UpdateStatusList`, `Delete`, `GetById`, `GetListByPaging`, `GetListByFullParam`, `GetListBySpFullParam`.
- Route mặc định: `[Route("[controller]/[action]")]`.

## Method trong Service

Bộ method chuẩn cho mỗi entity (theo `S_Product`):
- `Create(MReq_X request)`
- `Update(MReq_X request)`
- `UpdateStatus(int id, short status, int updatedBy)`
- `UpdateStatusList(string sequenceIds, short status, int updatedBy)`
- `Delete(int id)` *(xoá cứng — hiếm khi mở)*
- `GetById(int id)`
- `GetListByPaging(MReq_X_FullParam request)`
- `GetListByFullParam(MReq_X_FullParam request)`
- `GetListBySpFullParam(MReq_X_FullParam request)` *(nếu có stored proc)*

## Region

Cuối class Service, đặt `#region Common functions ... #endregion` cho hàm dùng chung như `BuildFilterQuery`.
