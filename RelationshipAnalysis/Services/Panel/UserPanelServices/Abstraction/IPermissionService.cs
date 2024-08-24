using System.Security.Claims;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

public interface IPermissionService
{
    Task<ActionResponse<PermissionDto>> GetPermissionsAsync(ClaimsPrincipal userClaims);
}