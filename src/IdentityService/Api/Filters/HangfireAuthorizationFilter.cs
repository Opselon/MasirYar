// مسیر: src/IdentityService/Api/Filters/HangfireAuthorizationFilter.cs

using Hangfire.Dashboard;
using Microsoft.AspNetCore.Hosting;

namespace Api.Filters;

/// <summary>
/// فیلتر دسترسی برای داشبورد Hangfire
/// در محیط Development، دسترسی آزاد است اما در Production باید کنترل دسترسی پیاده‌سازی شود.
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // در محیط Development، دسترسی آزاد است
        // در Production، اینجا باید بررسی احراز هویت و نقش کاربر انجام شود
        var httpContext = context.GetHttpContext();
        var environment = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        
        return environment.IsDevelopment();
    }
}

