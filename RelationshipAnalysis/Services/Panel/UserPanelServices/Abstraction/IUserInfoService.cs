using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

public interface IUserInfoService
{
    Task<ActionResponse<UserOutputInfoDto>> GetUser(User user);
}