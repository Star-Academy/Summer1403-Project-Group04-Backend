using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

public interface IUserPasswordService
{
    Task<ActionResponse<MessageDto>> UpdatePasswordAsync(User user,
        UserPasswordInfoDto passwordInfoDto);
}