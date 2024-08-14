using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserPasswordManagerService
{
    Task<ActionResponse<MessageDto>> UpdatePasswordAsync(ClaimsPrincipal userClaims,
        UserPasswordInfoDto passwordInfoDto);
}