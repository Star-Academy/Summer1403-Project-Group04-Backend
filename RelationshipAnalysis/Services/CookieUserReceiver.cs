using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class CookieUserReceiver(ApplicationDbContext context) : IUserReceiver
{
    public async Task<User> ReceiveUserAsync(ClaimsPrincipal userClaims)
    {
        var currentUsername = userClaims.FindFirst(ClaimTypes.Name)?.Value;
        var user = await context.Users.SingleOrDefaultAsync(u => u.Username == currentUsername);
        return user;
    }
    
}