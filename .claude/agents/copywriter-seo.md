---
name: copywriter-seo
description: "Chua ap dung — API_Sample hien chi co backend API, khong co UI. Agent nay se duoc cap nhat khi co giao dien nguoi dung."
---

> **N/A**: Du an API_Sample hien chi co backend API, khong co giao dien nguoi dung. Agent nay se duoc cap nhat khi co source UI.

# Copywriter and SEO Agent

## Trang thai

Agent nay tam thoi khong ap dung vi du an API_Sample hien tai chi bao gom backend API, khong co giao dien nguoi dung.

## Ap dung han che

Mot so nguyen tac co the ap dung cho **error messages** trong `MessageErrorConstants`:

- Viet bang tieng Viet tu nhien, ngan gon
- Giam mo ho: noi ro van de va cach xu ly
- Uu tien thong bao cu the hon la chung chung

Vi du:
```csharp
// Tot
public const string DO_NOT_FIND_DATA = "Khong tim thay du lieu!";
public const string DUPLICATE_CODE = "Ma da ton tai trong he thong!";

// Tranh
public const string ERROR = "Da co loi xay ra";  // qua mo ho
```

## Khi nao cap nhat day du

- Khi co source code frontend/UI
- Khi can viet microcopy cho buttons, labels, placeholders
- Khi can viet empty states, success/error toasts
- Khi can toi uu SEO cho trang cong khai (neu co)

## Lien he

Lien he team lead hoac architect de biet ke hoach frontend cho du an.
