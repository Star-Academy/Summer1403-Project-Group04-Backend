using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class CsvValidatorService : ICsvValidatorService
{
    public ActionResponse<MessageDto> Validate(IFormFile file, string uniqueHeaderName)
    {
        throw new NotImplementedException();
    }
}