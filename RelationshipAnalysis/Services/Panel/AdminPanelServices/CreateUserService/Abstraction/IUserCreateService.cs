using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.CreateUserService.Abstraction;

public interface IUserCreateService
{
    Task<ActionResponse<MessageDto>> CreateUser(CreateUserDto createUserDto);
}