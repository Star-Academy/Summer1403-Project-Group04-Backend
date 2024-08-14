using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserInfoManagerService
{
    Task<ActionResponce<UserOutputInfoDto>> GetUserAsync(ClaimsPrincipal userClaim);
}