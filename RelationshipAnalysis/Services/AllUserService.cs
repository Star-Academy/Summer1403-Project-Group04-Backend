using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class AllUserService(IRoleReceiver rolesReceiver) : IAllUserService
{
    public ActionResponse<List<UserOutputInfoDto>> GetAllUser(List<User> users)
    {
        if (users.IsNullOrEmpty())
        {
            return NotFoundResult();
        }

        var userOutputs = GetAllUserOutputs(users);

        return SuccessResult(userOutputs);
    }

    private List<UserOutputInfoDto> GetAllUserOutputs(List<User> users)
    {
        var userOutputs = new List<UserOutputInfoDto>();
        foreach (var user in users)
        {
            var data = new UserOutputInfoDto()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Roles = rolesReceiver.ReceiveRoles(user.Id)
            };
            userOutputs.Add(data);
        }

        return userOutputs;
    }

    private ActionResponse<List<UserOutputInfoDto>> SuccessResult(List<UserOutputInfoDto> users)
    {
        return new ActionResponse<List<UserOutputInfoDto>>()
        {
            Data = users,
            StatusCode = StatusCodeType.Success
        };
    }
    
    private ActionResponse<List<UserOutputInfoDto>> NotFoundResult()
    {
        return new ActionResponse<List<UserOutputInfoDto>>()
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}