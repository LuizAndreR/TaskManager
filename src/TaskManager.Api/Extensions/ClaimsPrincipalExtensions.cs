using FluentResults;
using System.Security.Claims;

namespace TaskManager.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Result<int> GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Result.Fail<int>("User ID claim is missing or invalid");
        }

        return Result.Ok(userId);
    }
}
