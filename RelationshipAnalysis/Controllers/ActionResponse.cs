using RelationshipAnalysis.Enums;

namespace RelationshipAnalysis.Controllers;

public class ActionResponce<T>
{
    public T Data { get; set; }
    
    public StatusCodeType StatusCode { get; set; }
    
    public ActionResponce()
    {
        StatusCode = StatusCodeType.Success;
    }
}