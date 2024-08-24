using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

public interface IUserDeleteService
{
    Task<ActionResponse<MessageDto>> DeleteUser(User user);
}