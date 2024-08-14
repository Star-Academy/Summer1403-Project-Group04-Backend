
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class UserController(
    IUserInfoManagerService userInfoManagerService,
    IUserUpdateManagerService userUpdateManagerService,
    IUserPasswordManagerService passwordManagerService,
    IUserReceiver userReceiver)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var user = await userReceiver.ReceiveUserAsync(User);
        var result = await userInfoManagerService.GetUserAsync(user);
        return StatusCode((int)result.StatusCode, result.Data);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserUpdateInfoDto userUpdateInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(User);
        var result = await userUpdateManagerService.UpdateUserAsync(user, userUpdateInfoDto, Response);
        return StatusCode((int)result.StatusCode, result.Data);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdatePassword(UserPasswordInfoDto passwordInfoDto)
    {
        var user = await userReceiver.ReceiveUserAsync(User);
        var result = await passwordManagerService.UpdatePasswordAsync(user, passwordInfoDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}