# Git Workflow — API_Sample

> Repo hiện chưa init git. Khi init, áp dụng convention sau.

## Branching

- `main` — production-ready, chỉ merge qua PR.
- `develop` — integration branch.
- `feature/{ticket}-{slug}` — feature mới (vd `feature/PRD-12-product-import`).
- `bugfix/{ticket}-{slug}` — fix bug.
- `hotfix/{ticket}-{slug}` — fix gấp từ `main`.

## Commit message — Conventional Commits

```
{type}({scope}): {mô tả ngắn tiếng Việt}

[body chi tiết tuỳ chọn]
```

`type`: `feat`, `fix`, `refactor`, `perf`, `docs`, `chore`, `test`, `style`.
`scope`: tên project hoặc entity (`webapi`, `application`, `s-product`, `db`).

Ví dụ:
- `feat(s-product): thêm GetListBySpFullParam dùng stored procedure`
- `fix(webapi): sửa lỗi User.GetAccountId trả 0 khi token thiếu claim`
- `chore(db): bổ sung migration AddProductSlugIndex`

## .gitignore bắt buộc

- `bin/`, `obj/`
- `*.user`, `*.suo`
- `appsettings.Production.json` (nếu chứa secret)
- `API_Sample.WebApi/Logs/`
- `.vs/`, `.idea/`

## Pull request

- Title: cùng format commit.
- Mô tả: link ticket + checklist (build pass, test pass, đã update migration, đã review naming).
- Reviewer: tối thiểu 1 người trước khi merge.
- Squash merge để giữ history sạch.

## Cấm

- Không commit trực tiếp lên `main`/`develop`.
- Không force push branch chia sẻ.
- Không commit secret/connection string/JWT key.
