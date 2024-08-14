using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserReceiver(ApplicationDbContext context) : IUserReceiver
{
    public async Task<User> ReceiveUserAsync(ClaimsPrincipal userClaims)
    {
        var currentId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == currentId);
        return user;
    }
    
    public async Task<User> ReceiveUserAsync(int id)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id);
        return user;
    }
    
}