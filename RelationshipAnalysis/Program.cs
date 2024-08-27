using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Middlewares;
using RelationshipAnalysis.Services.AdminPanelServices;
using RelationshipAnalysis.Services.AdminPanelServices.Abstraction;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory.Abstraction;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory.Abstraction;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices.Abstraction;
using RelationshipAnalysis.Settings.JWT;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql( builder.Configuration["CONNECTION_STRING"]).UseLazyLoadingProxies());

builder.Services.AddSingleton<ICookieSetter, CookieSetter>()
    .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()
    .AddSingleton<ILoginService, LoginService>()
    .AddSingleton<IPermissionService, PermissionService>()
    .AddSingleton<IPasswordHasher, CustomPasswordHasher>()
    .AddSingleton<IAllUserService, AllUserService>()
    .AddSingleton<IUserUpdateInfoService, UserUpdateInfoService>()
    .AddSingleton<IUserDeleteService, UserDeleteService>()
    .AddSingleton<IUserReceiver, UserReceiver>()
    .AddSingleton<IUserPasswordService, UserPasswordService>()
    .AddSingleton<IUserInfoService, UserInfoService>()
    .AddSingleton<IPasswordVerifier, PasswordVerifier>()
    .AddSingleton<IRoleReceiver, RoleReceiver>()
    .AddSingleton<ILogoutService, LogoutService>()
    .AddSingleton<IUserCreateService, UserCreateService>()
    .AddSingleton<IUserUpdateRolesService, UserUpdateRolesService>()
    .AddSingleton<INodeCategoryReceiver, NodeCategoryReceiver>()
    .AddSingleton<IEdgeCategoryReceiver, EdgeCategoryReceiver>()
    .AddSingleton<ICreateNodeCategoryService, CreateNodeCategoryService>()
    .AddSingleton<ICreateEdgeCategoryService, CreateEdgeCategoryService>()
    .AddSingleton<IGraphReceiver, GraphReceiver>()
    .AddSingleton<IUserUpdateRolesService, UserUpdateRolesService>()
    .AddSingleton<IGraphReceiver, GraphReceiver>()
    .AddSingleton<INodesAdditionService, NodesAdditionService>()
    .AddSingleton<ISingleNodeAdditionService, SingleNodeAdditionService>()
    .AddSingleton<ICsvProcessorService, CsvProcessorService>()
    .AddSingleton<ISingleEdgeAdditionService, SingleEdgeAdditionService>()
    .AddSingleton<IEdgesAdditionService, EdgesAdditionService>()
    .AddSingleton<ICsvValidatorService, CsvValidatorService>()
    .AddSingleton<IExpansionGraphReceiver, ExpansionGraphReceiver>()
    .AddSingleton<IGraphDtoCreator, GraphDtoCreator>()
    .AddKeyedSingleton<IInfoReceiver, NodeInfoReceiver>("node")
    .AddKeyedSingleton<IInfoReceiver, EdgeInfoReceiver>("edge")
    .AddKeyedSingleton<IAttributesReceiver, NodeAttributesReceiver>("node")
    .AddKeyedSingleton<IAttributesReceiver, EdgeAttributesReceiver>("edge");
    


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAutoMapper(typeof(UserUpdateInfoMapper));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var cookie = context.Request.Cookies[jwtSettings.CookieName];
                if (!string.IsNullOrEmpty(cookie))
                {
                    context.Token = cookie;
                }
                return Task.CompletedTask;
            }
        };
    });


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors(x => x.AllowCredentials().AllowAnyHeader().AllowAnyMethod()
    .SetIsOriginAllowed(x => true));
app.UseMiddleware<SanitizationMiddleware>();
app.Run();

namespace RelationshipAnalysis
{
    public partial class Program
    {
    }
}