# Database — API_Sample

## ORM

- **EF Core** với `MainDbContext` duy nhất (`API_Sample.Data/EF/MainDbContext.cs`).
- Database provider: SQL Server (theo connection string).
- DI: `builder.Services.AddDbContext<MainDbContext>(opt => opt.UseSqlServer(...))`.

## Entity convention

```csharp
[Table("Product")]                           // PascalCase, số ít
public partial class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }              // PK luôn là Id (int)

    [Required, Column("code"), StringLength(20)]
    public string Code { get; set; }

    [Column("name_slug"), StringLength(100)]
    public string NameSlug { get; set; }     // slug song hành với Name

    [Column("status")]
    public short Status { get; set; }        // -1 = xoá mềm, 0 = inactive, 1 = active

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
    [Column("created_by")]
    public int CreatedBy { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }
    [Column("updated_by")]
    public int? UpdatedBy { get; set; }
}
```

Yêu cầu:
- Property C# **PascalCase**, column DB **snake_case** (gắn `[Column("snake_name")]`).
- Mọi entity nghiệp vụ phải có: `Id`, `Status`, `CreatedAt/By`, `UpdatedAt/By`.
- Field tracking: `CreatedAt`/`CreatedBy` not null; `UpdatedAt`/`UpdatedBy` nullable.
- Soft delete: KHÔNG xoá vật lý, set `Status = -1`.

## Query

- Read-only: BẮT BUỘC `.AsNoTracking()`.
- Map list sang DTO: `ProjectTo<MRes_X>(_mapper.ConfigurationProvider)`.
- Filter status: `.Where(x => x.Status != -1)` (loại bản ghi đã xoá mềm).
- Search slug: `x.NameSlug.StartsWith(StringHelper.ToUrlClean(searchText))`.
- Search code: `x.Code.StartsWith(searchText.ToUpper().Trim())`.
- `Contains` với array → dùng `EF.Constant(arr)` để tránh parameter sniffing (xem `S_Product.BuildFilterQuery`).

## Update / Delete

| Trường hợp | API EF Core |
|------------|-------------|
| Update toàn bộ field | `_context.X.FindAsync(id)` → gán field → `SaveChangesAsync()` |
| Update bulk vài field | `_context.X.Where(...).ExecuteUpdateAsync(s => s.SetProperty(...))` |
| Xoá mềm | `ExecuteUpdateAsync` set `Status = -1` (hoặc `UpdateStatus(id, -1, updatedBy)`) |
| Xoá cứng (hiếm) | `_context.X.Where(...).ExecuteDeleteAsync()` |

KHÔNG dùng `_context.Update(entity) + SaveChanges` cho bulk → tốn round-trip.

## Stored procedure

- Sử dụng helper `StoreProcedure.GetListAsync<T>(connectionString, spName, paramNames, paramValues)`.
- Tên SP: `sp_{entity}_{action}_by_{param}`. Ví dụ: `sp_product_getlist_by_fullparam`.
- Truyền connection: `_context.Database.GetConnectionString()`.

## Migration

- Code-first migration qua `dotnet ef migrations add {Name} --project API_Sample.Data --startup-project API_Sample.WebApi`.
- Apply: `dotnet ef database update --project API_Sample.Data --startup-project API_Sample.WebApi`.
- Script SQL cho production deploy: `dotnet ef migrations script`.

## Insert pattern (ID không identity)

Một số bảng quản lý ID thủ công (xem `S_Product.Create`):
```csharp
var maxId = await _context.Products.MaxAsync(x => (int?)x.Id) ?? 0;
data.Id = maxId + 1;
```
Chỉ áp dụng khi cột `id` không phải IDENTITY. Nếu có IDENTITY, bỏ logic gán Id.
