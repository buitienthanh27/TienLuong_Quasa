---
description: Skill to verify and support deployment for the Transport Management SaaS .NET + Vue stack
---

# Deploy Skill - Transport Management SaaS

## Mục đích

Skill này hướng dẫn quy trình deploy và verify cho dự án Transport Management SaaS với stack:
- **Backend**: ASP.NET Core 8 Minimal API
- **Frontend**: Vue 3 + Vite + Tailwind v4
- **Database**: PostgreSQL 15

## Khi nào kích hoạt

- Khi user yêu cầu deploy
- Khi cần verify build trước production
- Khi cần kiểm tra migration database

## Pre-Deploy Checklist

### 1. Code Quality

```bash
# Chạy tất cả tests
dotnet test

# Build frontend
cd src/TransportManagement.WebUI
npm run build
```

### 2. Database Migration

```bash
# Kiểm tra pending migrations
dotnet ef migrations list -s src/TransportManagement.API

# Backup database TRƯỚC KHI migrate
pg_dump -h localhost -p 5433 -U postgres -d transport_db > backup_$(date +%Y%m%d_%H%M%S).sql

# Apply migration
dotnet ef database update -s src/TransportManagement.API
```

### 3. Environment Configuration

Kiểm tra các settings cho môi trường target:

| Setting | Development | Production |
|---------|-------------|------------|
| `RunStartupMigration` | true | **false** |
| `EnableDevAuth` | true | **false** |
| `CORS Origins` | localhost:5173 | production domain |

### 4. Build Production

```bash
# Backend
cd src/TransportManagement.API
dotnet publish -c Release -o ./publish

# Frontend
cd src/TransportManagement.WebUI
npm run build
# Output: dist/
```

## Post-Deploy Verification

### Health Check

```bash
# API health
curl -s https://your-domain/health | jq

# Kiểm tra OpenAPI
curl -s https://your-domain/openapi/v1.json | head -20
```

### Smoke Test

1. [ ] Login admin thành công
2. [ ] Tạo trip mới thành công
3. [ ] Xem báo cáo Dashboard
4. [ ] SignalR connection hoạt động
5. [ ] Driver login OTP hoạt động

### Monitoring

```bash
# Kiểm tra logs
grep -i "error" logs/app-$(date +%Y%m%d).log | tail -20

# Kiểm tra metrics (nếu có)
curl -s https://your-domain/metrics
```

## Rollback Procedure

```bash
# Rollback database
dotnet ef database update <PreviousMigrationName> -s src/TransportManagement.API

# HOẶC restore từ backup
psql -h localhost -p 5433 -U postgres -d transport_db < backup_YYYYMMDD_HHMMSS.sql

# Deploy lại version cũ
git checkout v1.x.x-1
dotnet publish -c Release -o ./publish
```

## Red Flags - DỪNG deploy nếu

| Vấn đề | Hành động |
|--------|-----------|
| Tests fail | Fix tests trước |
| Migration không có Down() | Thêm Down() method |
| Có uncommitted changes | Commit hoặc stash |
| ENV vars thiếu | Cập nhật config |
| Stack trace trong logs | Fix exceptions |

## Tham khảo

- Xem `docs/release-checklist.md` để biết chi tiết đầy đủ
- Xem `docs/database-indexes.md` nếu cần optimize performance
