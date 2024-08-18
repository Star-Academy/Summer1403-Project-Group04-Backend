using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Graph;

[Index(nameof(EdgeCategoryName), IsUnique = true)]

public class EdgeCategory
{
    [Key]
    public int EdgeCategoryId { get; set; }
    
    public string EdgeCategoryName { get; set; }

    
    public virtual ICollection<Edge> Edges { get; set; }
}
