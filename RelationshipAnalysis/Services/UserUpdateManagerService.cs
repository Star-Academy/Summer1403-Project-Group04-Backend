using System.Security.Claims;
using AutoMapper;
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
    public async Task<ActionResponce<MessageDto>> UpdateUserAsync(ClaimsPrincipal userClaims, UserUpdateInfoDto userUpdateInfoDto, HttpResponse response)
    {
        var result = new ActionResponce<MessageDto>();
        var user = await userReceiver.ReceiveUserAsync(userClaims);
        if (user is null)
        {
            result.Data = new MessageDto(Resources.UserNotFoundMessage);
            result.StatusCode = StatusCodeType.NotFound;
            return result;
        }
        mapper.Map(userUpdateInfoDto, user);
        context.Update(user);
        await context.SaveChangesAsync();
        
        var token = jwtTokenGenerator.GenerateJwtToken(user);
        cookieSetter.SetCookie(response, token);
        
        result.Data = new MessageDto(Resources.SuccessfulUpdateUserMessage);
        return result;
    }
}