using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserUpdateManagerService
{
    Task<ActionResponse<MessageDto>> UpdateUserAsync(ClaimsPrincipal userClaims, UserUpdateInfoDto userUpdateInfoDto,
        HttpResponse response);
}