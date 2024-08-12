using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; } // Store hashed password

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}