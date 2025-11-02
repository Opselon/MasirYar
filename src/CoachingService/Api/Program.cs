// مسیر: src/CoachingService/Api/Program.cs

using Api.Extensions;
using Application.Interfaces;
using Infrastructure.Clients;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// --- شروع بخش پیکربندی‌های ما ---

// ۱. خواندن رشته اتصال از فایل appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ۲. اضافه کردن و پیکربندی DbContext برای اتصال به PostgreSQL
builder.Services.AddDbContext<CoachingDbContext>(options =>
    options.UseNpgsql(connectionString));

// ۳. ثبت Repository ها و Clients
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IIdentityServiceClient, IdentityServiceGrpcClient>();

// ۴. پیکربندی MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

// --- پایان بخش پیکربندی‌های ما ---

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- شروع بخش اعمال خودکار Migration ---
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
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
