using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RelationshipAnalysis.Models.Auth;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(50)] public required string Username { get; set; }

    [Required] [StringLength(256)] public required string PasswordHash { get; set; }

    [Required] public required  string FirstName { get; set; }

    [Required] public required string LastName { get; set; }

    [Required] [EmailAddress] public required string Email { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}