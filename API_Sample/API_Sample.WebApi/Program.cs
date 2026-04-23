using API_Sample.Application.Mapper;
using API_Sample.Application.Services;
using API_Sample.Application.Ultilities;
using API_Sample.Data.EF;
using API_Sample.Data.Seed;
using API_Sample.Utilities.Constants;
using API_Sample.WebApi.Middlewares;
using API_Sample.WebApi.Middlewares.Timezone;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


void GetDefaultHttpClient(IServiceProvider serviceProvider, HttpClient httpClient, string hostUri)
{
    if (!string.IsNullOrEmpty(hostUri))
        httpClient.BaseAddress = new Uri(hostUri);
    //client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
    httpClient.Timeout = TimeSpan.FromMinutes(2);
    httpClient.DefaultRequestHeaders.Clear();
    httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml+json");
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}


HttpClientHandler GetDefaultHttpClientHandler()
{
    return new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        UseCookies = false,
        AllowAutoRedirect = false,
        UseDefaultCredentials = true,
        ClientCertificateOptions = ClientCertificateOption.Manual,
        //ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true,
    };
}

static void InitUtilitiesService(IServiceCollection services)
{
    services.AddSingleton<IJwtHelper, JwtHelper>();
    services.AddScoped<ISendMailSMTP, SendMailSMTP>();
}

static void InitApplicationService(IServiceCollection services)
{
    services.AddScoped<IS_Image, S_Image>();
    services.AddScoped<IS_Account, S_Account>();
    services.AddScoped<IS_Product, S_Product>();

    #region Payroll Services
    services.AddScoped<IS_Tram, S_Tram>();
    services.AddScoped<IS_Employee, S_Employee>();
    services.AddScoped<IS_SystemParameter, S_SystemParameter>();
    services.AddScoped<IS_SalaryScale, S_SalaryScale>();
    services.AddScoped<IS_CostCenter, S_CostCenter>();
    services.AddScoped<IS_Attendance, S_Attendance>();
    services.AddScoped<IS_Payroll, S_Payroll>();
    services.AddScoped<IS_Production, S_Production>();
    services.AddScoped<IS_DrcRate, S_DrcRate>();
    #endregion

    #region Config Services (No Hardcode)
    services.AddScoped<IS_EmployeeType, S_EmployeeType>();
    services.AddScoped<IS_TechnicalGrade, S_TechnicalGrade>();
    services.AddScoped<IS_RubberUnitPrice, S_RubberUnitPrice>();
    services.AddScoped<IS_ExchangeRate, S_ExchangeRate>();
    services.AddScoped<IS_WorkType, S_WorkType>();
    services.AddScoped<IS_Holiday, S_Holiday>();
    services.AddScoped<IS_AdvancePayment, S_AdvancePayment>();
    services.AddScoped<IS_TechnicalEvaluation, S_TechnicalEvaluation>();
    #endregion

    #region Payroll Extended Services
    services.AddScoped<IS_CareAdjustment, S_CareAdjustment>();
    services.AddScoped<IS_PayrollReconciliation, S_PayrollReconciliation>();
    services.AddScoped<IS_ZoneSupport, S_ZoneSupport>();
    services.AddScoped<IS_EmployeeHistory, S_EmployeeHistory>();
    #endregion

    #region Config-Driven Payroll Services
    services.AddScoped<IS_TaxBracket, S_TaxBracket>();
    services.AddScoped<IS_PayrollPolicy, S_PayrollPolicy>();
    services.AddScoped<IS_Position, S_Position>();
    #endregion
}

// Add services to the container.

// CORS configuration - Allow frontend origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "https://localhost:3000",
                "https://localhost:3001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainConnectString"),
        sqlOptions => {
            // SQL Server 2008 R2 = compatibility level 100
            // KHÔNG hỗ trợ OFFSET/FETCH - code đã sử dụng paging in-memory
            sqlOptions.UseCompatibilityLevel(100);
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        }));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

InitUtilitiesService(builder.Services);
InitApplicationService(builder.Services);

// Timezone services
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUserTimeZoneProvider, UserTimeZoneProvider>();
builder.Services.AddTransient<IConfigureOptions<JsonOptions>, ConfigureJsonOptions>();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressInferBindingSourcesForParameters = true; //Disable inference rules
    
    // Tự động hứng lỗi Validation và trả về Cấu trúc ResponseData chuẩn (Trả HTTP 200 kèm Result 0)
    options.InvalidModelStateResponseFactory = context =>
    {
        // Gộp tất cả các thông báo lỗi hoặc lấy thông báo lỗi đầu tiên
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)    
            .FirstOrDefault(); // Hoặc string.Join("; ", ...)

        var res = new API_Sample.Models.Common.ResponseData<object>(0, (int)HttpStatusCode.BadRequest, errors ?? MessageErrorConstants.REQUEST_DATA_INVALID);
        return new OkObjectResult(res); 
    };
});

//builder.Services.AddHttpClient("image")
//    .ConfigureHttpClient((serviceProvider, httpClient) => GetDefaultHttpClient(serviceProvider, httpClient, builder.Configuration.GetSection("ApiSettings:UrlApiImage").Value))
//    .SetHandlerLifetime(TimeSpan.FromMinutes(5)) //Default is 2 min
//    .ConfigurePrimaryHttpMessageHandler(x => GetDefaultHttpClientHandler());
//builder.Services.AddSingleton<IBase_CallApi, Base_CallApi>();
//builder.Services.AddSingleton<ICallApi, CallApi>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.OperationFilter<API.H2ADBSite.Portal.Variables.AddAuthorizationHeaderOperationHeader>();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_Sample.WebApi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.RequireHttpsMetadata = true;
    option.SaveToken = true;
    option.IncludeErrorDetails = true;
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
        ValidateLifetime = true,
        RequireExpirationTime = true, //Expired or not 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key"))),
        ClockSkew = TimeSpan.Zero, //TimeSpan.Zero // new System.TimeSpan(0,0,30);
    };
    option.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                context.Response.Headers.Add("Token-Expired", "true");
            return Task.CompletedTask;
        }
    };
});

//Config IpRateLimit https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


var path = Directory.GetCurrentDirectory();
builder.Logging.AddFile($"{path}\\Logs\\Logs.txt");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelExpandDepth(2);
        c.DefaultModelRendering(ModelRendering.Model);
        c.DefaultModelsExpandDepth(-1);
        c.DisplayOperationId();
        c.DisplayRequestDuration();
        c.DocExpansion(DocExpansion.None);
        c.EnableDeepLinking();
        c.EnableFilter();
        //c.MaxDisplayedTags(5);
        c.ShowExtensions();
        c.ShowCommonExtensions();
        c.EnableValidator();
        //c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head);
        c.UseRequestInterceptor("(request) => { return request; }");
    });
}
else
{
    app.UseHsts();
    if (builder.Configuration.GetValue<bool>("Swagger:Active"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DefaultModelExpandDepth(2);
            c.DefaultModelRendering(ModelRendering.Model);
            c.DefaultModelsExpandDepth(-1);
            c.DisplayOperationId();
            c.DisplayRequestDuration();
            c.DocExpansion(DocExpansion.None);
            c.EnableDeepLinking();
            c.EnableFilter();
            //c.MaxDisplayedTags(5);
            c.ShowExtensions();
            c.ShowCommonExtensions();
            c.EnableValidator();
            //c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head);
            c.UseRequestInterceptor("(request) => { return request; }");
        });
    }
}

app.UseIpRateLimiting(); //Apply IpRateLimit in middleware

// CORS must be before other middlewares
app.UseCors("AllowFrontend");

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseUserTimeZone(); //Timezone middleware
app.MapControllers();

// Seed payroll data on startup (Development only)
if (app.Environment.IsDevelopment())
{
    // Use RealDataSeeder with actual Excel data from November 2025
    await RealDataSeeder.SeedAsync(app.Services);
}

app.Run();
