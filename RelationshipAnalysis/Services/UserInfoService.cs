using System.Security.Claims;
using AutoMapper;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserInfoService(IUserReceiver userReceiver, IUserRolesReceiver rolesReceiver) : IUserInfoService
{
    public async Task<ActionResponse<UserOutputInfoDto>> GetUserAsync(User user)
    {
        if (user is null)
        {
            return NotFoundResult();
        }
        return SuccessResult(user);
    }

    private ActionResponse<UserOutputInfoDto> SuccessResult(User user)
    {
        return new ActionResponse<UserOutputInfoDto>()
        {
            Data = new UserOutputInfoDto()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Roles = rolesReceiver.ReceiveRoles(user.Id)
            },
            StatusCode = StatusCodeType.Success
        };
    }
    
    private ActionResponse<UserOutputInfoDto> NotFoundResult()
    {
        return new ActionResponse<UserOutputInfoDto>()
        {
            StatusCode = StatusCodeType.NotFound
        };
    }
}