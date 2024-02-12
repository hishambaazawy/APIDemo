using ApiData.Entities;
using ApiData.Entities.Identity;
using ApiData.Shared;
using APIDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace APIDemo.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "V1")]
    [Route("api/public/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SmartChargingContext context;
        private readonly ILogger<AccountController> logger;
        private readonly LogsService logsService;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AccountController(SmartChargingContext smart, LogsService logs, IOptions<JwtSettings> options, ILogger<AccountController> llogger) 
        {
            context = smart;
            logger = llogger;
            _jwtSettings = options;
            logsService = logs;
        }

        [HttpPost("Login")]
        public async Task<GenericResponse<string>> LoginAsync([FromBody] AccountLoginForm accountLoginForm)
        {
            try {
                var user = await context.Users
                   .Where(us => us.UserEmail == accountLoginForm.email)
                   .Where(us => us.PasswordHash ==ApiData.Shared.Utility.GenerateHash(accountLoginForm.password))
                   .Include(x => x.UserRoles).ThenInclude(y => y.Role)
                   .AsNoTracking().FirstOrDefaultAsync();
                if (user == null)
                {
                    return new GenericResponse<string>() { Success = false, StatusCode = StatusCodeEnum.Error, Response = "*", Message = "Invalid Login" };
                }
                else
                {
                  var token= await GenerateAccessToken(user);
                    if (!string.IsNullOrEmpty(token))
                    {
                        return new GenericResponse<string> { Success = true, Message = "Success", Response = "Bearer" + token, StatusCode = StatusCodeEnum.Success };
                    }
                    else
                    {
                        return new GenericResponse<string>() { Success = false, StatusCode = StatusCodeEnum.Error, Response = "*", Message = "Invalid Account" };
                    }
                }

            }
            catch(Exception ex) 
            {
                logsService.TraceError(ex);
                logger.LogError("Error",ex);
                return new GenericResponse<string>() { Success = false, StatusCode = StatusCodeEnum.Error, Response = "*", Message = "System Error" };
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
                foreach(var item in user.UserRoles) 
                {
                    subject.AddClaim(new Claim(ClaimTypes.Role,item.Role.RoleName));
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
