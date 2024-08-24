using AutoMapper;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices;

public class UserUpdateInfoService(
    IUserUpdater userUpdater,
    IMessageResponseCreator messageResponseCreator,
    IServiceProvider serviceProvider,
    IMapper mapper) : IUserUpdateInfoService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserAsync(User user, UserUpdateInfoDto userUpdateInfoDto,
        HttpResponse response)
    {
        if (user == null) return messageResponseCreator.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage);

        if (!IsUsernameUnique(user.Username, userUpdateInfoDto.Username))
            return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.UsernameExistsMessage);

        if (!IsEmailUnique(user.Email, userUpdateInfoDto.Email)) 
            return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.EmailExistsMessage);

        mapper.Map(userUpdateInfoDto, user);

        await userUpdater.UpdateUserAsync(user);
        
        return messageResponseCreator.Create(StatusCodeType.Success, Resources.SuccessfulUpdateUserMessage);
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
}