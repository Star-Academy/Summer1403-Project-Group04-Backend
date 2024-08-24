using AutoMapper;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices;

public class UserUpdateInfoService(IServiceProvider serviceProvider, IMapper mapper) : IUserUpdateInfoService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserAsync(User user, UserUpdateInfoDto userUpdateInfoDto,
        HttpResponse response)
    {
        if (user == null) return NotFoundResult();

        if (!IsUsernameUnique(user.Username, userUpdateInfoDto.Username))
            return BadRequestResult(Resources.UsernameExistsMessage);

        if (!IsEmailUnique(user.Email, userUpdateInfoDto.Email)) return BadRequestResult(Resources.EmailExistsMessage);

        mapper.Map(userUpdateInfoDto, user);

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Update(user);
            await context.SaveChangesAsync();
        }

        return SuccessResult();
    }

    private bool IsUsernameUnique(string currentValue, string newValue)
    {
        if (currentValue == newValue) return true;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return !context.Users.Any(u => u.Username == newValue);
        }
    }

    private bool IsEmailUnique(string currentValue, string newValue)
    {
        if (currentValue == newValue) return true;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return !context.Users.Any(u => u.Email == newValue);
        }
    }

    private ActionResponse<MessageDto> BadRequestResult(string message)
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.BadRequest
        };
    }

    private ActionResponse<MessageDto> NotFoundResult()
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.UserNotFoundMessage),
            StatusCode = StatusCodeType.NotFound
        };
    }

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.SuccessfulUpdateUserMessage),
            StatusCode = StatusCodeType.Success
        };
    }
}