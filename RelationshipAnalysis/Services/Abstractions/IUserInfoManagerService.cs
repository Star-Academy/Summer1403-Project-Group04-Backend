using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserInfoManagerService
{
    Task<ActionResponse<UserOutputInfoDto>> GetUserAsync(ClaimsPrincipal userClaim);
}