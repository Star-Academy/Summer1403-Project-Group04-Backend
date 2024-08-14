using System.Security.Claims;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserInfoManagerService(IUserReceiver userReceiver) : IUserInfoManagerService
{
    public async Task<ActionResponce<UserOutputInfoDto>> GetUserAsync(ClaimsPrincipal userClaim)
    {
        var result = new ActionResponce<UserOutputInfoDto>();
        var user = await userReceiver.ReceiveUserAsync(userClaim);
        if (user is null)
        {
            result.Data = null;
            result.StatusCode = StatusCodeType.NotFound;
            return result;
        }

        result.Data = new UserOutputInfoDto()
        {
            Username = user.Username, Email = user.Email, 
            FirstName = user.FirstName, LastName = user.LastName,
            Id = user.Id
        };
        result.StatusCode = StatusCodeType.Success;
        return result;
    }
}