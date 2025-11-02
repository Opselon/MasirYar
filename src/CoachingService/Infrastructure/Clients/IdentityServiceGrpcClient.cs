// مسیر: src/CoachingService/Infrastructure/Clients/IdentityServiceGrpcClient.cs

using Application.Interfaces;
using Grpc.Net.Client;
using Identity;
using GrpcUserInfo = Identity.UserInfo;
using AppUserInfo = Application.Interfaces.UserInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Clients;

/// <summary>
/// پیاده‌سازی Client برای ارتباط با IdentityService از طریق gRPC
/// این کلاس از Batch Get استفاده می‌کند تا مشکل N+1 Query حل شود.
/// </summary>
public class IdentityServiceGrpcClient : IIdentityServiceClient
{
    private readonly IdentityGrpc.IdentityGrpcClient _client;
    private readonly ILogger<IdentityServiceGrpcClient> _logger;

    public IdentityServiceGrpcClient(IConfiguration configuration, ILogger<IdentityServiceGrpcClient> logger)
    {
        _logger = logger;
        var identityServiceUrl = configuration["IdentityService:Url"] 
            ?? "http://identity-service:8080";
        
        var channel = GrpcChannel.ForAddress(identityServiceUrl);
        _client = new IdentityGrpc.IdentityGrpcClient(channel);
    }

    public async Task<AppUserInfo?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetUserByIdRequest { UserId = userId.ToString() };
            var response = await _client.GetUserByIdAsync(request, cancellationToken: cancellationToken);

            return new AppUserInfo
            {
                Id = Guid.Parse(response.UserId),
                Username = response.Username,
                Email = response.Email
            };
        }
        catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            _logger.LogWarning("User {UserId} not found", userId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", userId);
            return null;
        }
    }

    public async Task<Dictionary<Guid, AppUserInfo>> GetUsersByIdsAsync(
        List<Guid> userIds, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!userIds.Any())
            {
                return new Dictionary<Guid, AppUserInfo>();
            }

            var request = new GetUsersByIdsRequest();
            request.UserIds.AddRange(userIds.Select(id => id.ToString()));

            var response = await _client.GetUsersByIdsAsync(request, cancellationToken: cancellationToken);

            var result = new Dictionary<Guid, AppUserInfo>();
            foreach (var userInfo in response.Users)
            {
                if (Guid.TryParse(userInfo.UserId, out var userId))
                {
                    result[userId] = new AppUserInfo
                    {
                        Id = userId,
                        Username = userInfo.Username,
                        Email = userInfo.Email
                    };
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users batch. UserIds: {UserIds}", string.Join(", ", userIds));
            return new Dictionary<Guid, AppUserInfo>();
        }
    }
}

