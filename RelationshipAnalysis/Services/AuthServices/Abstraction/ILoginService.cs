using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Auth;

namespace RelationshipAnalysis.Services.AuthServices.Abstraction;

public interface ILoginService
{
    Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response);
}