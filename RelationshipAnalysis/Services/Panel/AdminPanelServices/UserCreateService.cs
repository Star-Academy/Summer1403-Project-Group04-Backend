using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.Admin;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;
using CreateUserDto = RelationshipAnalysis.Dto.Panel.Admin.CreateUserDto;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices;

public class UserCreateService(
    IUserAdder userAdder,
    IRoleReceiver roleReceiver,
    IUserRolesAdder userRolesAdder,
    IMessageResponseCreator messageResponseCreator,
    IServiceProvider serviceProvider,
    IPasswordHasher passwordHasher) : IUserCreateService
{
    public async Task<ActionResponse<MessageDto>> CreateUser(CreateUserDto createUserDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var isUserExist = context.Users.Select(x => x.Username).ToList().Contains(createUserDto.Username);
        if (isUserExist)
            return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.UsernameExistsMessage);

        var isEmailExist = context.Users.Select(x => x.Email).ToList().Contains(createUserDto.Email);
        if (isEmailExist)
            return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.EmailExistsMessage);

        if (createUserDto.Roles.IsNullOrEmpty())
            return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.EmptyRolesMessage);

        var invalidRoles = createUserDto.Roles.FindAll(r => !context.Roles.Select(role => role.Name)
            .Contains(r));
        if (invalidRoles.Any())
            return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidRolesListMessage);

        var user = await AddUser(createUserDto);
        await AddUserRoles(createUserDto, user);


        return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.SucceddfulCreateUser);
    }

    private async Task AddUserRoles(CreateUserDto createUserDto, User user)
    {
        var roleNames = await roleReceiver.ReceiveRolesListAsync(createUserDto.Roles);
        await userRolesAdder.AddUserRoles(roleNames, user);
    }

    private async Task<User> AddUser(CreateUserDto createUserDto)
    {
        var user = new User
        {
            Username = createUserDto.Username,
            PasswordHash = passwordHasher.HashPassword(createUserDto.Password),
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };

        await userAdder.AddUserAsync(user);
        return user;
    }
}