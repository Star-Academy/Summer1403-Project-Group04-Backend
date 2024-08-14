using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserUpdateManagerService(ApplicationDbContext context, IUserReceiver userReceiver, IMapper mapper, ICookieSetter cookieSetter,
    IJwtTokenGenerator jwtTokenGenerator) : IUserUpdateManagerService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserAsync(ClaimsPrincipal userClaims, UserUpdateInfoDto userUpdateInfoDto, HttpResponse response)
    {
        var result = new ActionResponse<MessageDto>();
        var user = await userReceiver.ReceiveUserAsync(userClaims);
        if (user is null)
        {
            return NotFoundResult();
        }
        mapper.Map(userUpdateInfoDto, user);
        context.Update(user);
        await context.SaveChangesAsync();
        SetCookie(user, response);
        return SuccessResult();
    }

    private ActionResponse<MessageDto> NotFoundResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.UserNotFoundMessage),
            StatusCode = StatusCodeType.NotFound
        };
    }

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulUpdateUserMessage),
            StatusCode = StatusCodeType.Success
        };
    }

    private void SetCookie(User user, HttpResponse response)
    {
        var token = jwtTokenGenerator.GenerateJwtToken(user);
        cookieSetter.SetCookie(response, token);
    }
}