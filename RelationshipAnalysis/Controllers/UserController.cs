
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController(
    IUserInfoManagerService userInfoManagerService,
    IUserUpdateManagerService userUpdateManagerService,
    IUserPasswordManagerService passwordManagerService)
    : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var result = await userInfoManagerService.GetUserAsync(User);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserUpdateInfoDto userUpdateInfoDto)
    {
        var result = await userUpdateManagerService.UpdateUserAsync(User, userUpdateInfoDto, Response);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdatePassword(UserPasswordInfoDto passwordInfoDto)
    {

        var result = await passwordManagerService.UpdatePasswordAsync(User, passwordInfoDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}