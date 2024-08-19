﻿using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;

namespace RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;

public interface ILoginService
{
    Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response,
        [FromServices] ApplicationDbContext context);
}