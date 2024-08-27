using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.AllUserService.Abstraction;

public interface IAllUserService
{
    Task<ActionResponse<GetAllUsersDto>> GetAllUser(List<User> users);
}