// src/IdentityService/Tests/IdentityService.IntegrationTests/UsersControllerTests.cs
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api;
using Api.DTOs;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace IdentityService.IntegrationTests;

// این کلاس یک WebApplicationFactory سفارشی برای تست‌های集成 است.
// این کلاس یک کانتینر PostgreSQL با استفاده از Testcontainers راه‌اندازی می‌کند
// و سرویس‌های برنامه را برای استفاده از این پایگاه داده موقت پیکربندی می‌کند.
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // کانتینر پایگاه داده PostgreSQL که توسط Testcontainers مدیریت می‌شود.
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    // این متد برای پیکربندی WebHost قبل از ساخت برنامه استفاده می‌شود.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // رشته اتصال پیش‌فرض را با رشته اتصال کانتینر PostgreSQL جایگزین می‌کنیم.
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString()
            });
        });

        // سرویس‌های برنامه را برای استفاده از پایگاه داده موقت پیکربندی می‌کنیم.
        builder.ConfigureTestServices(services =>
        {
            // حذف پیکربندی قبلی DbContext.
            services.RemoveAll(typeof(DbContextOptions<IdentityDbContext>));
            // اضافه کردن DbContext با استفاده از رشته اتصال کانتینر PostgreSQL.
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
            // اطمینان از ایجاد پایگاه داده.
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<IdentityDbContext>();
                db.Database.EnsureCreated();
            }
        });
    }

    // این متد قبل از اجرای تست‌ها، کانتینر پایگاه داده را راه‌اندازی می‌کند.
    public async Task InitializeAsync() => await _dbContainer.StartAsync();
    // این متد پس از اجرای تست‌ها، کانتینر پایگاه داده را حذف می‌کند.
    public new async Task DisposeAsync() => await _dbContainer.DisposeAsync();
}

// این کلاس شامل تست‌های集成 برای UsersController است.
public class UsersControllerTests : IClassFixture<ApiFactory>
{
    // کلاینت HTTP برای ارسال درخواست به برنامه.
    private readonly HttpClient _client;

    // سازنده کلاس که ApiFactory را تزریق می‌کند.
    public UsersControllerTests(ApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }

    // این تست بررسی می‌کند که آیا ثبت‌نام کاربر با موفقیت انجام می‌شود یا خیر.
    [Fact]
    public async Task Register_ShouldReturnCreated_WhenPayloadIsValid()
    {
        // آماده‌سازی
        var request = new RegisterUserDto
        {
            Username = "integration_test_user",
            Email = "integration@test.com",
            Password = "Password123"
        };

        // اجرا
        var response = await _client.PostAsJsonAsync("/api/users/register", request);

        // تایید
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.UserId);
    }
}

public class RegisterUserResponse
{
    public Guid UserId { get; set; }
}
