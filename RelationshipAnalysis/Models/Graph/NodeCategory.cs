using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph;

[Index(nameof(NodeCategoryName), IsUnique = true)]

public class NodeCategory
{
    [Key]
    public int NodeCategoryId { get; set; }
    
    public string NodeCategoryName { get; set; }

    
    public virtual ICollection<Node> Nodes { get; set; }
}