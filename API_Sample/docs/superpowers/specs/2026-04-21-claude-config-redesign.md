# Design: Thiết kế lại .claude/ cho API_Sample

**Ngày**: 2026-04-21  
**Trạng thái**: Approved  
**Tác giả**: Claude AI + User

---

## 1. Bối cảnh

Các file trong `.claude/agents/` và `.claude/commands/` đang mô tả cho dự án **Transport Management SaaS** (Minimal API, MediatR, CQRS, PostgreSQL, Vue) — không khớp với codebase **API_Sample** (Controllers, Service pattern, AutoMapper, SQL Server, N-Tier).

User cần học và tuân thủ codebase API_Sample của công ty, do đó cần thiết kế lại các file này để phản ánh đúng stack và pattern thực tế.

---

## 2. Phạm vi

### Trong phạm vi
- 8 files trong `.claude/agents/`
- 6 files trong `.claude/commands/`

### Ngoài phạm vi
- `.claude/rules/` — đã chính xác với codebase
- `.claude/skills/` — giữ nguyên vì là quy trình chung
- `.claude/CLAUDE.md` — đã chính xác

---

## 3. Quyết định thiết kế

### 3.1 Agents

| File | Hành động | Lý do |
|------|-----------|-------|
| `backend.md` | Viết lại hoàn toàn | Sai stack (Minimal API → Controllers) |
| `systems-architect.md` | Viết lại hoàn toàn | Sai architecture (Clean 4-layer → N-Tier 5-project) |
| `qa.md` | Viết lại hoàn toàn | Sai test paths và stack |
| `project-manager.md` | Viết lại hoàn toàn | Sai domain và breakdown pattern |
| `code-reviewer.md` | Bổ sung checklist | Generic nhưng cần thêm checklist cụ thể cho API_Sample |
| `frontend.md` | Đánh dấu N/A | API_Sample chỉ có backend |
| `copywriter-seo.md` | Đánh dấu N/A | API_Sample không có UI |
| `ui-ux-designer.md` | Đánh dấu N/A | API_Sample không có UI |

### 3.2 Commands

| File | Hành động | Lý do |
|------|-----------|-------|
| `deploy.md` | Viết lại | Sai paths và commands |
| `fix-issue.md` | Viết lại | Sai paths và checklist |
| `review.md` | Giữ nguyên | Generic, đã reference đúng rules |
| `brainstorm.md` | Giữ nguyên | Deprecated wrapper |
| `execute-plan.md` | Giữ nguyên | Deprecated wrapper |
| `write-plan.md` | Giữ nguyên | Deprecated wrapper |

---

## 4. Chi tiết nội dung mới

### 4.1 `agents/backend.md`

**Stack thực tế:**
- ASP.NET Core với `[ApiController]` + `ControllerBase`
- EF Core + SQL Server (`MainDbContext`)
- AutoMapper
- JWT Authentication
- AspNetCoreRateLimit

**Pattern bắt buộc:**
- Controller (thin) → Service (`S_xxx : BaseService<T>`) → DbContext
- `ResponseData<T>` cho mọi response
- `Error(HttpStatusCode, message)` cho lỗi logic
- `CatchException(ex, nameof(Method), params)` cho exception

**Boundary cấm:**
- Không Minimal API
- Không MediatR/CQRS
- Không `throw new Exception`
- Không `BadRequest()`/`NotFound()` — luôn `Ok(ResponseData<T>)`

### 4.2 `agents/systems-architect.md`

**Kiến trúc N-Tier 5 project:**
```
WebApi → Application → Data
         ↑              ↑
      Models ←――――――― Utilities
```

**Nguyên tắc:**
- Dependency direction: WebApi → Application → Data
- Models và Utilities được dùng chung
- Không thêm project mới
- Không Repository pattern generic

### 4.3 `agents/qa.md`

**Hiện trạng:** Chưa có project test

**Stack test dự kiến:**
- xUnit + FluentAssertions + Moq
- EF Core InMemory cho test

**Nhóm rủi ro cần ưu tiên:**
- Soft delete (`Status = -1`) không nhất quán
- Sai `ResponseData.result` (1/0/-1)
- Thiếu `.AsNoTracking()` cho query read-only
- Sai audit fields (`CreatedBy`/`UpdatedBy`)

### 4.4 `agents/project-manager.md`

**Domain hiện tại:** Product, Account, Image (CRUD mẫu)

**Template breakdown:**
1. Entity + `DbSet` trong `MainDbContext`
2. AutoMapper profile trong `AutoMapperProfile.cs`
3. Service (`IS_X` + `S_X`) trong `Application/Services/`
4. Controller trong `WebApi/Controllers/`
5. DTO (`MReq_X`, `MRes_X`) trong `Models/`
6. DI registration trong `Program.cs`

### 4.5 `agents/code-reviewer.md`

**Bổ sung checklist cho API_Sample:**
- Prefix naming: `MReq_`, `MRes_`, `S_`, `IS_`, `_s_`
- Error handling: `Error()`/`CatchException()`, không `throw`
- Query: `.AsNoTracking()` cho read-only
- Response: `ResponseData<T>`, không `BadRequest()`/`NotFound()`
- Audit: `CreatedBy`/`UpdatedBy` từ `User.GetAccountId()`
- Bulk update: `ExecuteUpdateAsync`/`ExecuteDeleteAsync`

### 4.6 Files N/A

Thêm banner ở đầu file:
```markdown
> ⚠️ **N/A**: Dự án API_Sample hiện chỉ có backend API. Agent này sẽ được cập nhật khi có source UI.
```

### 4.7 `commands/deploy.md`

**Lệnh chính:**
```bash
dotnet build API_Sample.sln
dotnet test  # khi có project test
dotnet publish -c Release -o ./publish
dotnet ef migrations add {Name} --project API_Sample.Data --startup-project API_Sample.WebApi
dotnet ef database update --project API_Sample.Data --startup-project API_Sample.WebApi
```

### 4.8 `commands/fix-issue.md`

**Quy trình:**
1. Tái hiện lỗi
2. Xác định tầng: Controller / Service / DbContext
3. Tìm root cause
4. Sửa với phạm vi nhỏ nhất
5. Verify: `dotnet build`

**Checklist:**
- Giữ đúng `ResponseData<T>` pattern
- Giữ đúng `Error()`/`CatchException()`
- Kiểm tra `.AsNoTracking()` cho query read-only
- Kiểm tra audit fields

---

## 5. Implementation plan

1. Viết lại `agents/backend.md`
2. Viết lại `agents/systems-architect.md`
3. Viết lại `agents/qa.md`
4. Viết lại `agents/project-manager.md`
5. Bổ sung `agents/code-reviewer.md`
6. Đánh dấu N/A: `agents/frontend.md`
7. Đánh dấu N/A: `agents/copywriter-seo.md`
8. Đánh dấu N/A: `agents/ui-ux-designer.md`
9. Viết lại `commands/deploy.md`
10. Viết lại `commands/fix-issue.md`

---

## 6. Verification

- Mỗi file phải reference đúng paths trong codebase (`API_Sample.WebApi/Controllers/`, `API_Sample.Application/Services/`, etc.)
- Mọi ví dụ code phải match với `S_Product.cs`, `ProductController.cs`, `BaseService.cs`
- Không còn reference tới Transport Management, MediatR, Minimal API, PostgreSQL, Vue
