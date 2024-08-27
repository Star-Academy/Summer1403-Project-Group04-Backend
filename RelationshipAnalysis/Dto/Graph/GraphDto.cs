namespace RelationshipAnalysis.Dto.Graph;

public class GraphDto
{
    public List<NodeDto> Nodes { get; set; } = new List<NodeDto>();
    public List<EdgeDto> Edges { get; set; } = new List<EdgeDto>();
}