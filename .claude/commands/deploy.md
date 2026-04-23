# Deploy Command

## Muc dich

Trien khai hoac chuan bi trien khai cho API_Sample theo workflow .NET N-Tier hien tai.

## Cach xu ly

1. Xac dinh environment va cach deploy that su duoc user yeu cau.
2. Kiem tra thay doi co anh huong database (migration), config, hay code.
3. Chay cac lenh verify phu hop.

## Pre-deploy checklist

- [ ] Solution build thanh cong: `dotnet build API_Sample.sln`
- [ ] Tests pass (khi co): `dotnet test`
- [ ] Migration da san sang neu co thay doi schema
- [ ] Config/secrets cua environment da duoc xac nhan
- [ ] `appsettings.Production.json` KHONG chua secrets that

## Lenh thuong dung

### Build va test

```bash
# Build solution
dotnet build API_Sample.sln

# Build Release
dotnet build API_Sample.sln -c Release

# Chay test (khi co project test)
dotnet test API_Sample.sln

# Publish cho deployment
dotnet publish API_Sample.WebApi/API_Sample.WebApi.csproj -c Release -o ./publish
```

### Database migration

```bash
# Xem danh sach migrations
dotnet ef migrations list --project API_Sample.Data --startup-project API_Sample.WebApi

# Tao migration moi
dotnet ef migrations add {MigrationName} --project API_Sample.Data --startup-project API_Sample.WebApi

# Apply migration
dotnet ef database update --project API_Sample.Data --startup-project API_Sample.WebApi

# Tao SQL script cho production
dotnet ef migrations script --project API_Sample.Data --startup-project API_Sample.WebApi -o migration.sql
```

## Sau khi deploy

### Verify endpoints

- [ ] Swagger UI hoat dong: `https://{domain}/swagger`
- [ ] Health check (neu co): `https://{domain}/health`
- [ ] JWT authentication hoat dong
- [ ] Rate limiting hoat dong

### Smoke test

- [ ] Login endpoint tra ve token
- [ ] Endpoint co `[Authorize]` tu choi request khong co token
- [ ] CRUD co ban hoat dong (Create, Read, Update, Delete)
- [ ] Paging hoat dong dung

### Kiem tra logs

```bash
# Xem log files
ls API_Sample.WebApi/Logs/

# Kiem tra loi
grep -i "error\|exception" API_Sample.WebApi/Logs/*.log | tail -20
```

## Rollback procedure

### Rollback migration

```bash
# Rollback ve migration truoc do
dotnet ef database update {PreviousMigrationName} --project API_Sample.Data --startup-project API_Sample.WebApi
```

### Rollback code

```bash
# Deploy lai version cu
git checkout {previous-tag-or-commit}
dotnet publish API_Sample.WebApi/API_Sample.WebApi.csproj -c Release -o ./publish
```

## Red flags - DUNG deploy neu

| Van de | Hanh dong |
|--------|-----------|
| Build fail | Fix build errors truoc |
| Tests fail | Fix tests truoc |
| Migration khong co rollback plan | Them Down() method hoac backup DB |
| Co uncommitted changes | Commit hoac stash |
| Connection string trong source | Di chuyen sang env var hoac secret manager |

## Luu y

- **KHONG** commit `appsettings.Production.json` chua connection string/JWT secret that
- **KHONG** chay migration production khi chua backup database
- **KHONG** tu suy dien script deploy neu repo chua co CI/CD setup
- Neu can migration database, xac nhan voi user truoc khi chay lenh thay doi schema production
