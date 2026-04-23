# Fix Issue Command

## Muc dich

Phan tich va sua loi mot cach he thong, dung voi codebase .NET N-Tier cua API_Sample.

## Quy trinh

### 1. Tai hien loi

- Tu mo ta, log, request, hoac Swagger test
- Xac dinh input gay loi
- Xac dinh expected vs actual behavior

### 2. Xac dinh tang gay loi

| Tang | Dau hieu | Files lien quan |
|------|----------|-----------------|
| Controller | Request khong vao duoc Service | `*Controller.cs` |
| Service | Logic sai, `result = 0` hoac `-1` | `S_*.cs` |
| Database | Query sai, data khong dung | `MainDbContext.cs`, Entity files |
| Mapping | DTO khong map dung | `AutoMapperProfile.cs` |
| Validation | ModelState errors | `MReq_*.cs` (DataAnnotations) |
| Auth | 401/403 errors | `Program.cs`, `JwtHelper.cs` |

### 3. Tim root cause

- Doc code lien quan, khong doan mo
- Kiem tra flow: Controller → Service → DbContext
- Kiem tra `Error()` va `CatchException()` returns
- Kiem tra log files trong `API_Sample.WebApi/Logs/`

### 4. Sua voi pham vi nho nhat

- Chi sua code lien quan truc tiep den bug
- Khong refactor code khong lien quan
- Giu dung pattern hien tai

### 5. Verify

```bash
# Build kiem tra syntax
dotnet build API_Sample.sln

# Test (khi co project test)
dotnet test API_Sample.sln

# Chay va test thu cong qua Swagger
dotnet run --project API_Sample.WebApi
```

## Checklist khi fix

### Pattern bat buoc

- [ ] Giu dung `ResponseData<T>` pattern
- [ ] Giu dung `Error(HttpStatusCode, message)` cho loi logic
- [ ] Giu dung `CatchException(ex, nameof(Method), params)` cho exception
- [ ] Khong them `throw new Exception` trong Service
- [ ] Khong them `BadRequest()`, `NotFound()` trong Controller

### Query checks

- [ ] Co `.AsNoTracking()` cho query read-only
- [ ] Co `.Where(x => x.Status != -1)` de filter soft delete
- [ ] Dung `ProjectTo<T>()` thay vi `ToList()` roi `Map()`

### Audit checks

- [ ] `CreatedBy`/`UpdatedBy` lay tu `User.GetAccountId()` o Controller
- [ ] `CreatedAt`/`UpdatedAt` set trong Service

### Naming checks

- [ ] Giu dung prefix: `MReq_`, `MRes_`, `S_`, `IS_`, `_s_`

## Cac loi thuong gap va cach fix

### Loi: "Khong tim thay du lieu" nhung data co trong DB

**Nguyen nhan co the:**
- Query filter `Status != -1` nhung data co `Status = -1`
- Query dung `Id` sai
- Thieu `.AsNoTracking()` gay cache issue

**Cach fix:**
```csharp
// Kiem tra query co filter dung khong
var data = await _context.Products
    .AsNoTracking()
    .FirstOrDefaultAsync(x => x.Id == id && x.Status != -1);
```

### Loi: `result = -1` (exception)

**Nguyen nhan co the:**
- Database connection fail
- Null reference trong mapping
- Invalid query (EF Core translation fail)

**Cach fix:**
- Kiem tra log file de thay exception message
- Kiem tra connection string
- Kiem tra null checks truoc khi map

### Loi: Duplicate code khong detect

**Nguyen nhan co the:**
- Query khong filter `Status != -1`
- So sanh case-sensitive

**Cach fix:**
```csharp
request.Code = request.Code?.Trim().ToUpper();
var isExists = await _context.Products.AnyAsync(x => 
    x.Code == request.Code && 
    x.Status != -1 && 
    x.Id != request.Id);  // exclude chinh no khi update
```

### Loi: Paging tra ve sai count

**Nguyen nhan co the:**
- `CountAsync()` va `ToListAsync()` dung khac query
- Filter khong nhat quan

**Cach fix:**
```csharp
var query = BuildFilterQuery(request);  // dung chung query
int count = await query.CountAsync();
var data = await query.Skip(...).Take(...).ToListAsync();
```

### Loi: Audit fields khong duoc set

**Nguyen nhan co the:**
- Controller khong gan `CreatedBy`/`UpdatedBy`
- Service khong set `CreatedAt`/`UpdatedAt`

**Cach fix:**
```csharp
// Controller
request.CreatedBy = User.GetAccountId();

// Service
data.CreatedAt = DateTime.UtcNow;
data.CreatedBy = request.CreatedBy;
```

## Commit guidance

Tuan thu `.claude/rules/git-workflow.md`:

```
fix({scope}): {mo ta ngan tieng Viet}

[body chi tiet tuy chon]
```

Vi du:
- `fix(s-product): sua loi khong detect duplicate code khi update`
- `fix(webapi): sua loi User.GetAccountId tra ve 0`
