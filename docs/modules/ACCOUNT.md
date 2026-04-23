# Module: ACCOUNT (Quản Lý Tài Khoản & Xác Thực)

## 1. Tổng Quan

**Mục đích**: Quản lý tài khoản người dùng, xác thực JWT, refresh token.

**Entity**: `Account`

**Features**:
- Login với username/email/phone
- JWT Access Token (30 phút)
- Refresh Token (7 ngày)
- Password hashing với BCrypt
- Revoke token (logout)

---

## 2. Entity Structure

```csharp
[Table("Account")]
public partial class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("use_name")]
    [StringLength(20)]
    public string UserName { get; set; }       // Tên đăng nhập (unique)

    [Required]
    [Column("password")]
    [StringLength(100)]
    public string Password { get; set; }        // BCrypt hashed

    [Required]
    [Column("first_name")]
    [StringLength(10)]
    public string FirstName { get; set; }

    [Required]
    [Column("last_name")]
    [StringLength(10)]
    public string LastName { get; set; }

    [Required]
    [Column("account_type")]
    [StringLength(10)]
    public string AccountType { get; set; }     // Admin, HR, Accountant, Viewer

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string Phone { get; set; }

    [Column("status")]
    public short Status { get; set; }           // -1=deleted, 0=locked, 1=active

    // Refresh Token
    [Column("refresh_token")]
    [StringLength(500)]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry_time", TypeName = "datetime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Audit
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

---

## 3. Service Interface

```csharp
public interface IS_Account
{
    // Authentication
    Task<ResponseData<MRes_Token>> Login(MReq_Account_Login request);
    Task<ResponseData<MRes_Token>> RefreshToken(MReq_Token_Refresh request);
    Task<ResponseData<bool>> RevokeToken(string userName);
    
    // CRUD
    Task<ResponseData<MRes_Account>> Create(MReq_Account request);
    Task<ResponseData<MRes_Account>> UpdateStatus(int id, short status, int updatedBy);
    Task<ResponseData<List<MRes_Account>>> UpdateStatusList(string sequenceIds, short status, int updatedBy);
    Task<ResponseData<int>> Delete(int id);
    Task<ResponseData<MRes_Account>> GetById(int id);
    Task<ResponseData<List<MRes_Account>>> GetListByPaging(MReq_Account_FullParam request);
    Task<ResponseData<List<MRes_Account>>> GetListByFullParam(MReq_Account_FullParam request);
    Task<ResponseData<List<MRes_Account>>> GetListBySpFullParam(MReq_Account_FullParam request);
}
```

---

## 4. API Endpoints

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| POST | `/Account/Login` | No | Đăng nhập |
| POST | `/Account/RefreshToken` | No | Làm mới token |
| POST | `/Account/RevokeToken` | Yes | Đăng xuất |
| POST | `/Account/Create` | Yes | Tạo tài khoản |
| PUT | `/Account/UpdateStatus` | Yes | Cập nhật trạng thái |
| PUT | `/Account/UpdateStatusList` | Yes | Cập nhật nhiều |
| DELETE | `/Account/Delete/{id}` | Yes | Xóa tài khoản |
| GET | `/Account/GetById/{id}` | Yes | Xem chi tiết |
| GET | `/Account/GetListByPaging` | Yes | Danh sách phân trang |
| GET | `/Account/GetListByFullParam` | Yes | Danh sách full |
| GET | `/Account/GetListBySpFullParam` | Yes | Danh sách qua SP |

---

## 5. Authentication Flow

### 5.1 Login Flow
```
┌──────────────────────────────────────────────────────────────────────┐
│                          LOGIN FLOW                                  │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  1. Client gửi POST /Account/Login                                   │
│     { "userName": "admin", "password": "123456" }                    │
│                                                                      │
│  2. Server xử lý:                                                    │
│     a. Tìm account theo userName/email/phone                         │
│        WHERE (UserName = X OR Email = X OR Phone = X)                │
│              AND Status IN (Active, Locked)                          │
│                                                                      │
│     b. Verify password với BCrypt                                    │
│        BCrypt.Verify(request.Password, account.Password)             │
│                                                                      │
│     c. Generate JWT Access Token (30 phút)                           │
│        Claims: AccountId, Email, Phone, UserName                     │
│                                                                      │
│     d. Generate Refresh Token (random 32 bytes base64)               │
│        Lưu vào DB: RefreshToken, RefreshTokenExpiryTime (+7 days)    │
│                                                                      │
│  3. Response                                                         │
│     { "accessToken": "eyJ...", "refreshToken": "abc123..." }         │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### 5.2 Refresh Token Flow
```
┌──────────────────────────────────────────────────────────────────────┐
│                       REFRESH TOKEN FLOW                             │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  1. Client gửi POST /Account/RefreshToken                            │
│     { "accessToken": "eyJ... (expired)", "refreshToken": "abc..." }  │
│                                                                      │
│  2. Server xử lý:                                                    │
│     a. Extract claims từ expired token                               │
│        GetPrincipalFromExpiredToken(accessToken)                     │
│        → Lấy UserName từ ClaimTypes.NameIdentifier                   │
│                                                                      │
│     b. Tìm account, validate refresh token                           │
│        WHERE UserName = X                                            │
│              AND RefreshToken = request.RefreshToken                 │
│              AND RefreshTokenExpiryTime > NOW                        │
│                                                                      │
│     c. Generate new Access Token (30 phút)                           │
│        Generate new Refresh Token                                    │
│        Lưu new Refresh Token vào DB                                  │
│                                                                      │
│  3. Response                                                         │
│     { "accessToken": "eyJ...(new)", "refreshToken": "xyz...(new)" }  │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### 5.3 Logout (Revoke) Flow
```
┌──────────────────────────────────────────────────────────────────────┐
│                         REVOKE TOKEN FLOW                            │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  1. Client gửi POST /Account/RevokeToken?userName=admin              │
│     Header: Authorization: Bearer eyJ...                             │
│                                                                      │
│  2. Server xử lý:                                                    │
│     a. Tìm account theo userName                                     │
│     b. Set RefreshToken = null                                       │
│     c. SaveChanges                                                   │
│                                                                      │
│  3. Effect:                                                          │
│     - Access Token vẫn valid cho đến khi expire (30 phút)            │
│     - Refresh Token không còn dùng được                              │
│     - Client cần login lại sau khi access token hết hạn              │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

---

## 6. JWT Configuration

### 6.1 Claims Structure
```csharp
var claims = new[]
{
    new Claim("AccountId", account.Id.ToString()),
    new Claim(ClaimTypes.Email, account.Email ?? ""),
    new Claim(ClaimTypes.MobilePhone, account.Phone ?? ""),
    new Claim(ClaimTypes.NameIdentifier, account.UserName)
};
```

### 6.2 Token Settings (appsettings.json)
```json
{
  "JWT": {
    "Key": "your-secret-key-at-least-256-bits",
    "Issuer": "API_Sample",
    "Audience": "API_Sample_Users",
    "DurationInMinutes": 30
  }
}
```

### 6.3 Lấy AccountId trong Controller
```csharp
// Extension method: User.GetAccountId()
public static int GetAccountId(this ClaimsPrincipal user)
{
    var claim = user.FindFirst("AccountId");
    return claim != null ? int.Parse(claim.Value) : 0;
}

// Usage
var accountId = User.GetAccountId();
request.CreatedBy = accountId;
```

---

## 7. Request/Response Models

### 7.1 MReq_Account_Login
```csharp
public class MReq_Account_Login
{
    [Required]
    public string UserName { get; set; }  // Can be username, email, or phone
    
    [Required]
    public string Password { get; set; }
}
```

### 7.2 MReq_Token_Refresh
```csharp
public class MReq_Token_Refresh
{
    [Required]
    public string AccessToken { get; set; }   // Expired token
    
    [Required]
    public string RefreshToken { get; set; }  // Valid refresh token
}
```

### 7.3 MRes_Token
```csharp
public class MRes_Token
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
```

### 7.4 MReq_Account (Create)
```csharp
public class MReq_Account : BaseModel.History
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(20)]
    public string UserName { get; set; }
    
    [Required]
    [StringLength(50)]
    [JsonIgnore]                           // Don't log password
    public string Password { get; set; }
    
    [Required]
    [StringLength(10)]
    public string FirstName { get; set; }
    
    [Required]
    [StringLength(10)]
    public string LastName { get; set; }
    
    [Required]
    [StringLength(10)]
    public string AccountType { get; set; }  // Admin, HR, Accountant, Viewer
    
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }
    
    [StringLength(20)]
    public string? Phone { get; set; }
}
```

---

## 8. Password Hashing

```csharp
// Hash password on create
data.Password = BCrypt.Net.BCrypt.HashString(
    request.Password.Trim(), 
    SaltRevision.Revision2Y
);

// Verify on login
bool isValid = BCrypt.Net.BCrypt.Verify(
    request.Password, 
    storedHash
);
```

---

## 9. Account Status Enum

```csharp
public enum EN_Account_Status
{
    Deleted = -1,    // Soft deleted
    Locked = 0,      // Cannot login (but can still refresh token check)
    Active = 1       // Normal active
}

// Login only allows Active or Locked (để kiểm tra và thông báo)
WHERE Status = Active OR Status = Locked
```

---

## 10. Business Rules

### 10.1 Create Account
```csharp
// 1. UserName must be unique (case-insensitive)
request.UserName = request.UserName?.Trim().ToLower();
if (await _context.Accounts.AnyAsync(x => x.UserName == request.UserName && x.Status != -1))
    return Error(HttpStatusCode.Conflict, "Trùng lặp tên đăng nhập");

// 2. Password is hashed before storage
data.Password = BCrypt.Net.BCrypt.HashString(request.Password.Trim(), SaltRevision.Revision2Y);
```

### 10.2 Login Rules
```csharp
// 1. Find by userName OR email OR phone
var findUser = await _context.Accounts.FirstOrDefaultAsync(x => 
    (x.UserName == request.UserName || 
     x.Phone == request.UserName || 
     x.Email == request.UserName) && 
    (x.Status == Active || x.Status == Locked));

// 2. Return same error for not found OR wrong password (security)
return Error(HttpStatusCode.NotFound, "Tài khoản hoặc mật khẩu không đúng");
```

---

## 11. Stored Procedure Example

```csharp
// GetListBySpFullParam calls: sp_account_getlist_by_fullparam
string[] arrParams = { "@SequenceStatus", "@SearchText", "@Page", "@Record" };
object[] arrValues = { request.SequenceStatus, request.SearchText, request.Page, request.Record };
var result = await StoreProcedure.GetListAsync<MRes_Account>(
    _context.Database.GetConnectionString(), 
    "sp_account_getlist_by_fullparam", 
    arrParams, 
    arrValues
);
```

---

## 12. Security Considerations

1. **Password**: Không log, dùng `[JsonIgnore]` trên DTO
2. **Refresh Token**: Lưu DB, có expiry time
3. **Token Rotation**: Mỗi refresh sẽ tạo cặp token mới
4. **Revoke**: Clear refresh token từ DB
5. **Rate Limit**: Áp dụng cho endpoint Login

---

## 13. Code Examples

### Login
```javascript
POST /Account/Login
Content-Type: application/json

{
    "userName": "admin",
    "password": "123456"
}
```

### Response
```json
{
    "result": 1,
    "data": {
        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "refreshToken": "dGVzdC1yZWZyZXNoLXRva2VuLWJhc2U2NA=="
    },
    "error": {
        "code": 200,
        "message": "Đăng nhập thành công"
    }
}
```

### Sử dụng Token trong Request
```javascript
GET /Employee/GetListByPaging
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Refresh khi Token hết hạn
```javascript
POST /Account/RefreshToken
Content-Type: application/json

{
    "accessToken": "eyJ... (expired token)",
    "refreshToken": "dGVzdC1yZWZyZXNoLXRva2VuLWJhc2U2NA=="
}
```
