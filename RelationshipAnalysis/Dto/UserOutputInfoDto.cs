namespace RelationshipAnalysis.Dto;

public class UserOutputInfoDto
{
    public string Username { get; set; }
    public string Email { get; set; }

    public UserOutputInfoDto(string username, string email)
    {
        Username = username;
        Email = email;
    }
}