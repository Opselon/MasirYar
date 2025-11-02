// فایل: src/IdentityService/Api/Program.cs
using Api.Extensions;
using Api.Filters;
using Api.Services;
using Application.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- شروع بخش پیکربندی‌های ما ---

// ۱. خواندن رشته اتصال از فایل appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ۲. اضافه کردن و پیکربندی DbContext برای اتصال به PostgreSQL
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(connectionString));

// ۳. ثبت سرویس‌های دامنه (Domain Services)
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJournalRepository, JournalRepository>();
builder.Services.AddScoped<IJournalAnalyzer, JournalAnalyzer>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// ۴. پیکربندی MediatR برای استفاده از Handlers در لایه Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

// ۶. پیکربندی Hangfire برای کارهای پس‌زمینه
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
    {
        SchemaName = "hangfire" // نام Schema برای جداول Hangfire
    }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1; // تعداد Worker های همزمان
});

// ۵. پیکربندی JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// --- پایان بخش پیکربندی‌های ما ---

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ۷. پیکربندی gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// --- شروع بخش اعمال خودکار Migration ---
// این کد را فقط در محیط توسعه اجرا می‌کنیم تا در پروداکشن کنترل بیشتری داشته باشیم.
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}
// --- پایان بخش اعمال خودکار Migration ---

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // داشبورد Hangfire برای نظارت بر کارهای پس‌زمینه (فقط در محیط Development)
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() } // در Production باید کنترل دسترسی اضافه شود
    });
}

app.UseHttpsRedirection();

// اعمال Authentication و Authorization
app.UseAuthentication();
app.UseAuthorization();

// Mapping gRPC Service
app.MapGrpcService<IdentityGrpcService>();

app.MapControllers();

app.MapGet("/healthz", () => "OK");

app.Run();

public partial class Program { }
