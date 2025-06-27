using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace VSMS.Infrastructure.Hubs;

public class BaseHub(ILogger<BaseHub> logger) : Hub
{
    protected string GetConnectionId()
    {
        return Context.ConnectionId;
    }
    
    protected string? GetUserId()
    {
        return GetClaimValue(ClaimTypes.NameIdentifier);
    }

    protected string? GetClaimValue(string claimType)
    {
        return Context.User?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
    
    public override Task OnConnectedAsync()
    {
        logger.LogDebug($"User connected. Connection Id: {GetConnectionId()}. UserId: {GetUserId()}");
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogDebug($"User disconnected. Connection Id: {GetConnectionId()}. User: {GetClaimValue(ClaimTypes.Email)} {GetUserId()}");
        return Task.CompletedTask;
    }
}