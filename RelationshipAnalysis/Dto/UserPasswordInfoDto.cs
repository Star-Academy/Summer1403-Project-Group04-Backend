using System.ComponentModel.DataAnnotations;

namespace RelationshipAnalysis.Dto;

public class UserPasswordInfoDto
{
    [Required(ErrorMessageResourceName = "OldPasswordRequired", ErrorMessageResourceType = typeof(Resources))]
    public string OldPassword { get; set; }
    [Required(ErrorMessageResourceName = "NewPasswordRequired", ErrorMessageResourceType = typeof(Resources))]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessageResourceName = "InvalidPasswordMessage", ErrorMessageResourceType = typeof(Resources))]
    public string NewPassword { get; set; }
}