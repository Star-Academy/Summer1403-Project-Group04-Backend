using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto;

public class UserUpdateInfoDto
{
    
    [Required(ErrorMessageResourceName = "UsernameRequired", ErrorMessageResourceType = typeof(Resources))]
    public string Username { get; set; }
    [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(Resources))]
    [EmailAddress]
    public string Email { get; set; }
}