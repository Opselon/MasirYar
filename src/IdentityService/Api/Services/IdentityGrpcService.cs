// مسیر: src/IdentityService/Api/Services/IdentityGrpcService.cs

using Application.Interfaces;
using Application.UseCases.UserLogin;
using Application.UseCases.UserRegistration;
using Grpc.Core;
using Identity;
using MediatR;

namespace Api.Services;

/// <summary>
/// پیاده‌سازی سرویس gRPC برای Identity
/// این سرویس برای ارتباطات داخلی بین میکروسرویس‌ها استفاده می‌شود.
/// </summary>
public class IdentityGrpcService : IdentityGrpc.IdentityGrpcBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;

    public IdentityGrpcService(IMediator mediator, IUserRepository userRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
    }

    public override async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
    {
        try
        {
            var command = new RegisterUserCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            var userId = await _mediator.Send(command);
            return new RegisterUserResponse { UserId = userId.ToString() };
        }
        catch (InvalidOperationException ex)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"An error occurred: {ex.Message}"));
        }
    }

    public override async Task<LoginUserResponse> LoginUser(LoginUserRequest request, ServerCallContext context)
    {
        try
        {
            var command = new LoginUserCommand
            {
                Username = request.Username,
                Password = request.Password
            };

            var result = await _mediator.Send(command);
            return new LoginUserResponse
            {
                Token = result.Token,
                UserId = result.UserId.ToString(),
                Username = result.Username
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"An error occurred: {ex.Message}"));
        }
    }

    public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format."));
            }

            var user = await _userRepository.FindByIdAsync(userId, context.CancellationToken);
            
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found."));
            }

            return new GetUserByIdResponse
            {
                UserId = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"An error occurred: {ex.Message}"));
        }
    }

    public override async Task<GetUsersByIdsResponse> GetUsersByIds(GetUsersByIdsRequest request, ServerCallContext context)
    {
        try
        {
            var userIds = request.UserIds
                .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g!.Value)
                .ToList();

            if (!userIds.Any())
            {
                return new GetUsersByIdsResponse();
            }

            var users = await _userRepository.FindByIdsAsync(userIds, context.CancellationToken);

            var response = new GetUsersByIdsResponse();
            response.Users.AddRange(users.Select(u => new UserInfo
            {
                UserId = u.Id.ToString(),
                Username = u.Username,
                Email = u.Email
            }));

            return response;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"An error occurred: {ex.Message}"));
        }
    }
}

