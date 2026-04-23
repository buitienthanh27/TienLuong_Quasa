# Tech Stack & Dependencies

## 1. Framework & Runtime

| Component | Version | Mục đích |
|-----------|---------|----------|
| .NET | 8.0+ | Runtime |
| ASP.NET Core | 8.0+ | Web API Framework |
| Entity Framework Core | 8.0+ | ORM |

---

## 2. NuGet Packages

### API_Sample.WebApi
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="AspNetCoreRateLimit" />
<PackageReference Include="Swashbuckle.AspNetCore" />
```

### API_Sample.Application
```xml
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
<PackageReference Include="AutoMapper.Extensions.QueryableExtensions" />
<PackageReference Include="Newtonsoft.Json" />
<PackageReference Include="BCrypt.Net-Next" />
```

### API_Sample.Data
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
```

---

## 3. Database

| Component | Giá trị |
|-----------|---------|
| DBMS | SQL Server |
| Provider | Microsoft.EntityFrameworkCore.SqlServer |
| Connection | appsettings.json → ConnectionStrings:MainConnection |

### Entity Framework Commands
```bash
# Add migration
dotnet ef migrations add {Name} --project API_Sample.Data --startup-project API_Sample.WebApi

# Update database
dotnet ef database update --project API_Sample.Data --startup-project API_Sample.WebApi

# Generate SQL script
dotnet ef migrations script --project API_Sample.Data --startup-project API_Sample.WebApi
```

---

## 4. Authentication

### JWT Configuration
```json
// appsettings.json
{
  "JWT": {
    "Key": "your-secret-key-minimum-256-bits",
    "Issuer": "API_Sample",
    "Audience": "API_Sample_Users",
    "DurationInMinutes": 30
  }
}
```

### Token Structure
```
Header:
  alg: HS256
  typ: JWT

Payload:
  AccountId: "1"
  email: "admin@example.com"
  http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone: "0123456789"
  http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier: "admin"
  exp: 1734567890
  iss: "API_Sample"
  aud: "API_Sample_Users"
```

### Password Hashing
```csharp
// Hash
BCrypt.Net.BCrypt.HashString(password, SaltRevision.Revision2Y)

// Verify
BCrypt.Net.BCrypt.Verify(password, hashedPassword)
```

---

## 5. Rate Limiting

### Configuration (appsettings.json)
```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1s",
        "Limit": 10
      },
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  }
}
```

---

## 6. Swagger/OpenAPI

### Configuration (Program.cs)
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_Sample", Version = "v1" });
    
    // JWT Bearer configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

### Access
- Development: `https://localhost:5001/swagger`
- Có nút "Authorize" để nhập JWT token

---

## 7. Logging

### Configuration
```csharp
// ILogger<TService> injection through BaseService
_logger.LogInformation("Message");
_logger.LogWarning("Warning");
_logger.LogError(ex, "{Class}.{Method} Exception", className, methodName);
```

### Output
- Console (Development)
- File: `API_Sample.WebApi/Logs/`

### Log Levels (appsettings.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

---

## 8. AutoMapper

### Registration (Program.cs)
```csharp
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
```

### Profile Location
`API_Sample.Application/Mapper/AutoMapperProfile.cs`

### Usage
```csharp
// Map single
var entity = _mapper.Map<Entity>(request);

// Map to existing
_mapper.Map(request, entity);

// ProjectTo for queries
query.ProjectTo<MRes_Entity>(_mapper.ConfigurationProvider).ToListAsync();
```

---

## 9. Middleware Pipeline (Program.cs)

```csharp
// Order matters!
app.UseSecurityHeadersMiddleware();  // Custom: X-Frame-Options, CSP, etc.
app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseTimezoneMiddleware();          // Custom: timezone handling
app.MapControllers();
```

---

## 10. Dependency Injection

### Lifetimes
```csharp
// Singleton (1 instance toàn app)
builder.Services.AddSingleton<IJwtHelper, JwtHelper>();

// Scoped (1 instance per request)
builder.Services.AddScoped<IS_Account, S_Account>();
builder.Services.AddScoped<IS_Employee, S_Employee>();
builder.Services.AddScoped<IS_Payroll, S_Payroll>();
// ... all services

// Transient (new instance mỗi lần inject)
builder.Services.AddTransient<ISendMailSMTP, SendMailSMTP>();

// DbContext (scoped by default)
builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(connectionString));
```

---

## 11. Project Dependencies

```
API_Sample.WebApi
├── API_Sample.Application
├── API_Sample.Models
└── API_Sample.Utilities

API_Sample.Application
├── API_Sample.Data
├── API_Sample.Models
└── API_Sample.Utilities

API_Sample.Data
├── API_Sample.Models (nếu cần)
└── API_Sample.Utilities (nếu cần)

API_Sample.Models
└── (không dependency)

API_Sample.Utilities
└── (không dependency)
```

---

## 12. Environment Configuration

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "MainConnection": "Server=...;Database=...;..."
  },
  "JWT": {
    "Key": "...",
    "Issuer": "...",
    "Audience": "...",
    "DurationInMinutes": 30
  },
  "IpRateLimiting": { ... },
  "Logging": { ... }
}
```

### Environment Files
- `appsettings.json` - Base config
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production (DO NOT commit secrets!)

### Reading Config
```csharp
// In Program.cs
var connectionString = builder.Configuration.GetConnectionString("MainConnection");
var jwtKey = builder.Configuration["JWT:Key"];
```

---

## 13. CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://yourdomain.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// In pipeline
app.UseCors();
```

---

## 14. Folder Structure Conventions

```
API_Sample.WebApi/
├── Controllers/         # {Entity}Controller.cs
├── Middlewares/         # Custom middleware
├── Lib/                 # Extensions (ClaimsPrincipalExtensions)
├── Logs/                # Log files
├── Properties/          # launchSettings.json
├── Program.cs           # Entry point, DI, middleware
└── appsettings*.json    # Configuration

API_Sample.Application/
├── Services/            # S_{Entity}.cs (interface + class)
├── Ultilities/          # BaseService, JwtHelper, StoreProcedure, etc.
├── Mapper/              # AutoMapperProfile.cs
├── ServiceExternal/     # External API wrappers
└── ExtensionMethods/    # Query extensions

API_Sample.Data/
├── EF/                  # MainDbContext.cs
├── Entities/            # {Entity}.cs
├── Migrations/          # EF Core migrations
└── Seeders/             # Data seeders

API_Sample.Models/
├── Common/              # ResponseData, BaseModel, PagingRequestBase
├── Request/             # MReq_{Entity}.cs
├── Response/            # MRes_{Entity}.cs
└── Enums/               # EN_{Name}.cs

API_Sample.Utilities/
├── Constants/           # MessageErrorConstants, etc.
├── Encryptor.cs
├── StringHelper.cs
├── Utilities.cs
└── DataException.cs
```
