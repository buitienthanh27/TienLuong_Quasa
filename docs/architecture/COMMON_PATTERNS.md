# Common Patterns & Components

## 1. ResponseData<T> - Chuẩn Response

### Cấu Trúc
```csharp
public class ResponseData<T>
{
    public int result { get; set; }           // 1=success, 0=logic error, -1=exception
    public long time { get; set; }            // Unix timestamp (seconds)
    public string dataDescription { get; set; }
    public T? data { get; set; }              // Main payload
    public dynamic? data2nd { get; set; }     // Secondary (total count, metadata)
    public ErrorResponseBase error { get; set; }
}

public class ErrorResponseBase
{
    public int code { get; set; }             // HTTP status code
    public string message { get; set; }       // Error message
}
```

### Constructors
```csharp
// Success with message
new ResponseData<MRes_Entity>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
{
    data = mappedEntity
}

// Simple success
new ResponseData<MRes_Entity>
{
    data = mappedEntity,
    result = 1
}

// Success with secondary data (paging)
new ResponseData<List<MRes_Entity>>
{
    data = items,
    data2nd = totalCount,
    result = 1
}
```

### Result Values
| Value | Meaning | When to Use |
|-------|---------|-------------|
| 1 | Success | Operation completed successfully |
| 0 | Logic Error | Business rule violation (duplicate, not found, validation) |
| -1 | Exception | System error (DB error, unhandled exception) |

---

## 2. BaseService<T> - Service Base Class

### Cấu Trúc
```csharp
public abstract class BaseService<TService>
{
    protected readonly MainDbContext _context;
    protected readonly ILogger<TService> _logger;

    protected BaseService(MainDbContext context, ILogger<TService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Trả về lỗi logic (result = 0), tự động rollback transaction nếu có
    /// </summary>
    protected ErrorResponseBase Error(HttpStatusCode statusCode, string message)
    {
        if (_context.Database.CurrentTransaction != null)
            _context.Database.RollbackTransaction();

        return new ErrorResponseBase
        {
            code = (int)statusCode,
            message = message
        };
    }

    /// <summary>
    /// Bắt exception (result = -1), ghi log, tự động rollback
    /// </summary>
    protected ErrorResponseBase CatchException(Exception ex, string methodName, object parameters)
    {
        if (_context.Database.CurrentTransaction != null)
            _context.Database.RollbackTransaction();

        var paramJson = JsonConvert.SerializeObject(parameters);
        _logger.LogError(ex, "{Class}.{Method} Exception_Logger. Parameters: {Params}",
            typeof(TService).Name, methodName, paramJson);

        return new ErrorResponseBase
        {
            code = (int)HttpStatusCode.InternalServerError,
            message = "Đã xảy ra lỗi hệ thống!"
        };
    }
}
```

### Cách Sử Dụng
```csharp
public class S_Entity : BaseService<S_Entity>, IS_Entity
{
    private readonly IMapper _mapper;

    public S_Entity(MainDbContext context, IMapper mapper, ILogger<S_Entity> logger)
        : base(context, logger)
    {
        _mapper = mapper;
    }

    public async Task<ResponseData<MRes_Entity>> Create(MReq_Entity request)
    {
        try
        {
            // Validate
            if (await _context.Entities.AnyAsync(x => x.Code == request.Code && x.Status != -1))
                return Error(HttpStatusCode.Conflict, "Mã đã tồn tại!");

            // Map & save
            var data = _mapper.Map<Entity>(request);
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = request.CreatedBy;
            
            _context.Entities.Add(data);
            var save = await _context.SaveChangesAsync();
            
            if (save == 0)
                return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);

            return new ResponseData<MRes_Entity>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
            {
                data = _mapper.Map<MRes_Entity>(data)
            };
        }
        catch (Exception ex)
        {
            return CatchException(ex, nameof(Create), request);
        }
    }
}
```

---

## 3. BaseModel.History - Request Base Class

### Cấu Trúc
```csharp
public class BaseModel
{
    public class History
    {
        public short Status { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
```

### Sử Dụng
```csharp
public class MReq_Entity : BaseModel.History
{
    public int Id { get; set; }
    
    [Required]
    public string Code { get; set; }
    
    [Required]
    public string Name { get; set; }
    // ...
}
```

---

## 4. PagingRequestBase - Paging Base

### Cấu Trúc
```csharp
public class PagingRequestBase
{
    public string? SequenceStatus { get; set; }  // "1,0" hoặc "1" hoặc "-1,0,1"
    public int Page { get; set; } = 1;
    public int Record { get; set; } = 20;
}
```

### Sử Dụng
```csharp
public class MReq_Entity_FullParam : PagingRequestBase
{
    public string? SearchText { get; set; }
    public int? CategoryId { get; set; }
    // ... other filters
}
```

### Pattern Xử Lý Status
```csharp
private IQueryable<Entity> BuildFilterQuery(MReq_Entity_FullParam request)
{
    // Parse SequenceStatus từ string sang array
    var status = request.SequenceStatus?
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(short.Parse)
        .ToArray() ?? Array.Empty<short>();

    var query = _context.Entities.AsNoTracking();

    // Apply status filter
    if (status.Length > 0)
        query = query.Where(x => EF.Constant(status).Contains(x.Status));

    return query;
}
```

---

## 5. MessageErrorConstants - Thông Báo Chuẩn

### Định Nghĩa
```csharp
public static class MessageErrorConstants
{
    public const string CREATE_SUCCESS = "Tạo mới thành công!";
    public const string UPDATE_SUCCESS = "Cập nhật thành công!";
    public const string DELETE_SUCCESS = "Xoá thành công!";
    
    public const string DO_NOT_FIND_DATA = "Không tìm thấy dữ liệu!";
    
    public const string EXCEPTION_DO_NOT_CREATE = "Không thể tạo mới, vui lòng thử lại!";
    public const string EXCEPTION_DO_NOT_UPDATE = "Không thể cập nhật, vui lòng thử lại!";
}
```

### Cách Dùng
```csharp
// Success
return new ResponseData<MRes_Entity>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS);

// Not found
return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

// Save failed
if (save == 0)
    return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);
```

---

## 6. CRUD Method Templates

### 6.1 Create
```csharp
public async Task<ResponseData<MRes_Entity>> Create(MReq_Entity request)
{
    try
    {
        // 1. Normalize input
        request.Code = request.Code?.Trim().ToUpper();
        
        // 2. Validate unique
        var isExists = await _context.Entities.AnyAsync(x => 
            x.Code == request.Code && x.Status != -1);
        if (isExists)
            return Error(HttpStatusCode.Conflict, "Mã đã tồn tại!");
        
        // 3. Validate foreign keys (optional)
        if (!await _context.Categories.AnyAsync(x => x.Id == request.CategoryId && x.Status != -1))
            return Error(HttpStatusCode.BadRequest, "Danh mục không tồn tại!");
        
        // 4. Map DTO to Entity
        var data = _mapper.Map<Entity>(request);
        
        // 5. Set audit fields
        data.CreatedAt = DateTime.UtcNow;
        data.CreatedBy = request.CreatedBy;
        
        // 6. Save
        _context.Entities.Add(data);
        var save = await _context.SaveChangesAsync();
        
        if (save == 0)
            return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_CREATE);
        
        // 7. Load navigation (optional)
        await _context.Entry(data).Reference(x => x.Category).LoadAsync();
        
        // 8. Return response
        return new ResponseData<MRes_Entity>(1, (int)HttpStatusCode.Created, MessageErrorConstants.CREATE_SUCCESS)
        {
            data = _mapper.Map<MRes_Entity>(data)
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(Create), request);
    }
}
```

### 6.2 Update
```csharp
public async Task<ResponseData<MRes_Entity>> Update(MReq_Entity request)
{
    try
    {
        // 1. Normalize
        request.Code = request.Code?.Trim().ToUpper();
        
        // 2. Check unique (exclude current)
        var isExists = await _context.Entities.AnyAsync(x => 
            x.Code == request.Code && x.Status != -1 && x.Id != request.Id);
        if (isExists)
            return Error(HttpStatusCode.Conflict, "Mã đã tồn tại!");
        
        // 3. Find existing
        var data = await _context.Entities.FindAsync(request.Id);
        if (data == null)
            return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);
        
        // 4. Map changes
        _mapper.Map(request, data);
        
        // 5. Set audit
        data.UpdatedAt = DateTime.UtcNow;
        data.UpdatedBy = request.UpdatedBy;
        
        // 6. Save
        var save = await _context.SaveChangesAsync();
        if (save == 0)
            return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);
        
        return new ResponseData<MRes_Entity>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
        {
            data = _mapper.Map<MRes_Entity>(data)
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(Update), request);
    }
}
```

### 6.3 UpdateStatus (Single)
```csharp
public async Task<ResponseData<MRes_Entity>> UpdateStatus(int id, short status, int updatedBy)
{
    try
    {
        var data = await _context.Entities.FindAsync(id);
        if (data == null)
            return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

        data.Status = status;
        data.UpdatedAt = DateTime.UtcNow;
        data.UpdatedBy = updatedBy;

        var save = await _context.SaveChangesAsync();
        if (save == 0)
            return Error(HttpStatusCode.InternalServerError, MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE);

        return new ResponseData<MRes_Entity>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
        {
            data = _mapper.Map<MRes_Entity>(data)
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(UpdateStatus), new { id, status, updatedBy });
    }
}
```

### 6.4 UpdateStatusList (Bulk)
```csharp
public async Task<ResponseData<List<MRes_Entity>>> UpdateStatusList(string sequenceIds, short status, int updatedBy)
{
    try
    {
        // Parse JSON array
        List<int> ids = JsonConvert.DeserializeObject<List<int>>(sequenceIds);
        if (ids == null || !ids.Any())
            return Error(HttpStatusCode.BadRequest, MessageErrorConstants.DO_NOT_FIND_DATA);

        var now = DateTime.UtcNow;
        
        // Bulk update với ExecuteUpdateAsync
        var updatedCount = await _context.Entities
            .Where(x => ids.Contains(x.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, status)
                .SetProperty(p => p.UpdatedBy, updatedBy)
                .SetProperty(p => p.UpdatedAt, now)
            );

        if (updatedCount == 0)
            return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

        // Fetch updated records
        var datas = await _context.Entities.AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();

        return new ResponseData<List<MRes_Entity>>(1, (int)HttpStatusCode.OK, MessageErrorConstants.UPDATE_SUCCESS)
        {
            data = _mapper.Map<List<MRes_Entity>>(datas)
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(UpdateStatusList), new { sequenceIds, status, updatedBy });
    }
}
```

### 6.5 Delete (Hard)
```csharp
public async Task<ResponseData<int>> Delete(int id)
{
    try
    {
        // Check dependencies
        var hasChildren = await _context.ChildEntities.AnyAsync(x => x.ParentId == id);
        if (hasChildren)
            return Error(HttpStatusCode.BadRequest, "Không thể xoá vì đang có dữ liệu liên quan!");

        var deletedCount = await _context.Entities
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();

        if (deletedCount == 0)
            return Error(HttpStatusCode.NotFound, MessageErrorConstants.DO_NOT_FIND_DATA);

        return new ResponseData<int>(1, (int)HttpStatusCode.OK, MessageErrorConstants.DELETE_SUCCESS)
        {
            data = deletedCount
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(Delete), new { id });
    }
}
```

### 6.6 GetById
```csharp
public async Task<ResponseData<MRes_Entity>> GetById(int id)
{
    try
    {
        var data = await _context.Entities.AsNoTracking()
            .Include(x => x.Category)          // Include navigation
            .FirstOrDefaultAsync(x => x.Id == id);

        return new ResponseData<MRes_Entity>
        {
            data = _mapper.Map<MRes_Entity>(data),
            result = 1
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(GetById), new { id });
    }
}
```

### 6.7 GetListByPaging
```csharp
public async Task<ResponseData<List<MRes_Entity>>> GetListByPaging(MReq_Entity_FullParam request)
{
    try
    {
        var query = BuildFilterQuery(request);

        int count = await query.CountAsync();
        List<MRes_Entity> data = new List<MRes_Entity>();

        if (count > 0)
        {
            data = await query
                .OrderBy(x => x.Code)                              // Default sort
                .Skip((request.Page - 1) * request.Record)
                .Take(request.Record)
                .ProjectTo<MRes_Entity>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        return new ResponseData<List<MRes_Entity>>
        {
            data = data,
            data2nd = count,    // Total count for pagination
            result = 1
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(GetListByPaging), request);
    }
}
```

### 6.8 GetListByFullParam (No Paging)
```csharp
public async Task<ResponseData<List<MRes_Entity>>> GetListByFullParam(MReq_Entity_FullParam request)
{
    try
    {
        var query = BuildFilterQuery(request);

        var data = await query
            .OrderBy(x => x.Code)
            .ProjectTo<MRes_Entity>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new ResponseData<List<MRes_Entity>>
        {
            data = data,
            result = 1
        };
    }
    catch (Exception ex)
    {
        return CatchException(ex, nameof(GetListByFullParam), request);
    }
}
```

### 6.9 BuildFilterQuery (Common)
```csharp
#region Common functions
private IQueryable<Entity> BuildFilterQuery(MReq_Entity_FullParam request)
{
    // Parse status
    var status = request.SequenceStatus?
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(short.Parse)
        .ToArray() ?? Array.Empty<short>();

    var query = _context.Entities.AsNoTracking()
        .Include(x => x.Category)
        .AsQueryable();

    // Filter by status
    if (status.Length > 0)
        query = query.Where(x => EF.Constant(status).Contains(x.Status));

    // Search text
    if (!string.IsNullOrWhiteSpace(request.SearchText))
    {
        var searchText = request.SearchText.Trim();
        query = query.Where(x => x.Code.Contains(searchText) || x.Name.Contains(searchText));
    }

    // Filter by FK
    if (request.CategoryId.HasValue)
        query = query.Where(x => x.CategoryId == request.CategoryId.Value);

    return query;
}
#endregion
```

---

## 7. Controller Pattern

```csharp
[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class EntityController : ControllerBase
{
    private readonly IS_Entity _s_Entity;

    public EntityController(IS_Entity s_Entity)
    {
        _s_Entity = s_Entity;
    }

    [HttpPost]
    public async Task<IActionResult> Create(MReq_Entity request)
    {
        request.CreatedBy = User.GetAccountId();
        var res = await _s_Entity.Create(request);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> Update(MReq_Entity request)
    {
        request.UpdatedBy = User.GetAccountId();
        var res = await _s_Entity.Update(request);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStatus(int id, short status)
    {
        var res = await _s_Entity.UpdateStatus(id, status, User.GetAccountId());
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStatusList(string sequenceIds, short status)
    {
        var res = await _s_Entity.UpdateStatusList(sequenceIds, status, User.GetAccountId());
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _s_Entity.Delete(id);
        return Ok(res);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var res = await _s_Entity.GetById(id);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> GetListByPaging([FromQuery] MReq_Entity_FullParam request)
    {
        var res = await _s_Entity.GetListByPaging(request);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> GetListByFullParam([FromQuery] MReq_Entity_FullParam request)
    {
        var res = await _s_Entity.GetListByFullParam(request);
        return Ok(res);
    }
}
```

---

## 8. AutoMapper Configuration

```csharp
// Request → Entity (ignore audit fields)
CreateMap<MReq_Entity, Entity>()
    .ForMember(d => d.Status, opt => opt.Ignore())
    .ForMember(d => d.CreatedAt, opt => opt.Ignore())
    .ForMember(d => d.CreatedBy, opt => opt.Ignore())
    .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
    .ForMember(d => d.UpdatedBy, opt => opt.Ignore());

// Entity → Response (include navigation data)
CreateMap<Entity, MRes_Entity>()
    .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null));
```
