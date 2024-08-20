using RelationshipAnalysis.Dto;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface ICsvValidatorService
{
    ActionResponse<MessageDto> Validate(IFormFile file, string uniqueHeaderName);
}