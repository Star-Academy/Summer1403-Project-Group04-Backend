using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.UserUpdateInfoService.Abstraction;

public interface IUserUpdateInfoService
{
    Task<ActionResponse<MessageDto>> UpdateUserAsync(User user, UserUpdateInfoDto userUpdateInfoDto,
        HttpResponse response);
}