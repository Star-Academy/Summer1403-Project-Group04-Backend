using System.Security.Claims;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;
using PermissionDto = RelationshipAnalysis.Dto.Panel.User.PermissionDto;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

public interface IPermissionService
{
    Task<ActionResponse<PermissionDto>> GetPermissionsAsync(User user);
}