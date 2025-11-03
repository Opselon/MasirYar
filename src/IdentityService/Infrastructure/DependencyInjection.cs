// مسیر: src/IdentityService/Infrastructure/DependencyInjection.cs
using IdentityService.Application.Contracts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJournalRepository, JournalRepository>();
        services.AddScoped<IJournalAnalyzer, JournalAnalyzer>();
        return services;
    }
}
