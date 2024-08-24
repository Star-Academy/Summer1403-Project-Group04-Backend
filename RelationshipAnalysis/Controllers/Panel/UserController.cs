using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Controllers.Panel;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(
    IUserInfoService userInfoService,
    IUserUpdateInfoService userUpdateInfoService,
    IUserPasswordService passwordService,
    IUserReceiver userReceiver,
    ILogoutService logoutService,
    IPermissionService permissionService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var user = await userReceiver.ReceiveUserAsync(User);
        var result = await userInfoService.GetUser(user);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserUpdateInfoDto userUpdateInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(User);
        var result = await userUpdateInfoService.UpdateUserAsync(user, userUpdateInfoDto, Response);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPatch("password")]
    public async Task<IActionResult> UpdatePassword(UserPasswordInfoDto passwordInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(User);
        var result = await passwordService.UpdatePasswordAsync(user, passwordInfoDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        logoutService.Logout(Response);
        return Ok(new MessageDto(Resources.SuccessfulLogoutMessage));
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        var user = await userReceiver.ReceiveUserAsync(User);
        var response = await permissionService.GetPermissionsAsync(user);
        return StatusCode((int)response.StatusCode, response.Data);
    }
}