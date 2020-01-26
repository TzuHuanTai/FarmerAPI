using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FarmerAPI.Models.Weather;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FarmerAPI.ViewModels;
using System.Web;
using Microsoft.AspNetCore.Authorization;

namespace FarmerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private IConfiguration _config;
        private readonly WeatherContext _context;
        private readonly IHttpContextAccessor _accessor;
        
        public AuthController(IConfiguration config, WeatherContext context, IHttpContextAccessor accessor)
        {
            _config = config;
            _context = context;
            _accessor = accessor;
        }
               
        [HttpPost("[action]")]
        public async Task<IActionResult> Authenticate([FromBody]AuthRequest AuthRequest) //, string Account, string Password
        {
            bool HasUser = _context.FindUser(AuthRequest.Account, AuthRequest.Password);
            bool HasToken = _context.FindToken(AuthRequest.Account);
            
            if (HasUser)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                var authTime = DateTime.UtcNow.ToLocalTime();//ToLocalTime變UTC+8時區
                var expiresAt = authTime.AddDays(7);
                
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtClaimTypes.Audience, _config["Jwt:Audience"]),
                        new Claim(JwtClaimTypes.Issuer, _config["Jwt:Issuer"]),
                        new Claim(JwtClaimTypes.Id, AuthRequest.Account),
                        //new Claim(JwtClaimTypes.RoleId, RoleID.ToString()), //停止在jwt加入角色資訊，統一用id(帳號)判斷
                        //new Claim(JwtClaimTypes.Email, user.Email),
                        //new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber)
                    }),
                    Expires = expiresAt,
                    NotBefore = authTime,
                    IssuedAt = authTime,
                    SigningCredentials = new SigningCredentials
                    (
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                try
                {
                    Token SaveInfo = new Token
                    {
                        Account = AuthRequest.Account,
                        TokenCode = tokenString,
                        AuthTime = authTime,
                        ExpiredTime = expiresAt,
                        Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
                    };
                    //將Token資訊加入Database
                    if (HasToken)
                    {
                        //若過去已有建立過Token，刷新資料
                        Token existInfo = _context.Token.Where(x => x.Account == AuthRequest.Account).FirstOrDefault();
                        _context.Entry(existInfo).State = EntityState.Modified;
                        _context.Entry(existInfo).CurrentValues.SetValues(SaveInfo);
                    }
                    else
                    {
                        _context.Token.Add(SaveInfo);
                    } 

                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {                    
                    return BadRequest("Could not create token \n" + ex);
                }               

                return Ok(new
                {
                    access_token = tokenString,
                    token_type = "Bearer",
                    profile = new
                    {
                        //sid = user.Id,
                        //name = user.Name,
                        auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                        expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                    }
                });
            }
            else
            {
                //紀錄System Log
                _context.SystemLog.Add(new SystemLog
                {
                    LogTime = DateTime.Now,
                    Account = AuthRequest.Account,
                    Action = ControllerContext.ActionDescriptor.ActionName,
                    Detail = "Failure to authorize ",
                    Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
                });
                await _context.SaveChangesAsync();
                return Unauthorized();
            }            
        }
    }

    public static class Extension
    {
        public static bool FindUser(this WeatherContext context, string Account, string Password)
        {
            //用string.Compare區分大小寫判斷帳號密碼
            //若單純用x.Account == Account && x.Password == Password，無法區分大小寫
            return context.Member.Any(x =>
                string.Compare(x.Account, Account, false) == 0 &&
                string.Compare(x.Password, Password, false) == 0
            );
        }

        public static bool FindToken(this WeatherContext context, string Account)
        {
            //用string.Compare區分大小寫判斷帳號密碼
            //若單純用x.Account == Account && x.Password == Password，無法區分大小寫
            return context.Token.Any(x =>
                string.Compare(x.Account, Account, false) == 0                 
            );
        }

    }
    
}