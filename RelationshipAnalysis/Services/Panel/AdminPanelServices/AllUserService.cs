using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices;

public class AllUserService(IUserReceiver userReceiver, IMapper mapper, IRoleReceiver rolesReceiver)
    : IAllUserService
{
    public async Task<ActionResponse<GetAllUsersDto>> GetAllUser(List<User> users)
    {
        if (users.IsNullOrEmpty()) return NotFoundResult();

        var usersList = await GetAllUsersList(users);
        var result = await GetAllUsersOutPut(usersList);

        return SuccessResult(result);
    }

    private async Task<GetAllUsersDto> GetAllUsersOutPut(List<UserOutputInfoDto> usersList)
    {
        return new GetAllUsersDto
        {
            Users = usersList,
            AllUserCount = await userReceiver.ReceiveAllUserCountAsync()
        };
    }

    private async Task<List<UserOutputInfoDto>> GetAllUsersList(List<User> users)
    {
        var userOutputs = new List<UserOutputInfoDto>();
        foreach (var user in users)
        {
            var data = new UserOutputInfoDto();
            mapper.Map(user, data);
            data.Roles = await rolesReceiver.ReceiveRoleNamesAsync(user.Id);

            userOutputs.Add(data);
        }

        return userOutputs;
    }

    private ActionResponse<GetAllUsersDto> SuccessResult(GetAllUsersDto outPut)
    {
        return new ActionResponse<GetAllUsersDto>
        {
            Data = outPut,
            StatusCode = StatusCodeType.Success
        };
    }

    private ActionResponse<GetAllUsersDto> NotFoundResult()
    {
        return new ActionResponse<GetAllUsersDto>
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}