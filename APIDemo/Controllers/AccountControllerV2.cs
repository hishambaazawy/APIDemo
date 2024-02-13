using ApiData.Entities;
using ApiData.Entities.Identity;
using ApiData.Shared;
using APIDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIDemo.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "V2")]
    [Route("api/public/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin,User")]
    public class AccountController2 : ControllerBase
    {
        private readonly SmartChargingContext context;
        private readonly ILogger<AccountController2> logger;
        private readonly LogsService logsService;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AccountController2(SmartChargingContext smart, LogsService logs, IOptions<JwtSettings> options, ILogger<AccountController2> llogger)
        {
            context = smart;
            logger = llogger;
            _jwtSettings = options;
            logsService = logs;
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<GenericResponse<User>> LoginAsync([FromBody] AccountLoginForm accountLoginForm)
        {
            try
            {
                var user = await context.Users
                   .Where(us => us.UserEmail == accountLoginForm.email)
                   .Where(us => us.PasswordHash == ApiData.Shared.Utility.GenerateHash(accountLoginForm.password))
                   .Include(x => x.UserRoles).ThenInclude(y => y.Role)
                   .AsNoTracking().FirstOrDefaultAsync();
                if (user == null)
                {
                    return new GenericResponse<User>() { Success = false, StatusCode = StatusCodeEnum.Error, Response = null, Message = "Invalid Login" };
                }
                else
                {
                    var token = await GenerateAccessToken(user);
                    if (!string.IsNullOrEmpty(token))
                    {
                        user.PasswordHash = "";
                        return new GenericResponse<User> { Success = true, Message = "Bearer " + token, Response =user , StatusCode = StatusCodeEnum.Success };
                    }
                    else
                    {
                        return new GenericResponse<User>() { Success = false, StatusCode = StatusCodeEnum.Error, Response =null, Message = "Invalid Account" };
                    }
                }

            }
            catch (Exception ex)
            {
                logsService.TraceError(ex);
                logger.LogError("Error", ex);
                return new GenericResponse<User>() { Success = false, StatusCode = StatusCodeEnum.Error, Response = null, Message = "System Error" };
            }
        }
        protected async Task<string> GenerateAccessToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.Key);
                var subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name,Convert.ToString( user.UserEmail)),
                });
                foreach (var item in user.UserRoles)
                {
                    subject.AddClaim(new Claim(ClaimTypes.Role, item.Role.RoleName));
                }
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = subject,
                    Expires = DateTime.UtcNow.AddMinutes(ApiData.Shared.Utility.GetLifeTimeInMinites()),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _jwtSettings.Value.Issuer,
                    Audience = _jwtSettings.Value.Audience

                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception exp)
            {
                logsService.TraceError(exp);
                logger.LogError("Error", exp);
                return null;
            }

        }
    }
}
