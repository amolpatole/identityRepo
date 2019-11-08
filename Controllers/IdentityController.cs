using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityApi.Helpers;
using IdentityApi.Infra;
using IdentityApi.Models;
using IdentityApi.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace IdentityApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private IdentityDbContext dbContext;
        private IConfiguration configuration;
        public IdentityController(IdentityDbContext identityDbContext, IConfiguration configuration)
        {
            this.dbContext = identityDbContext;
            this.configuration = configuration;
        }

        [HttpPost("register", Name = "RegisterUser")]
        public async Task<ActionResult<dynamic>> RegisterUser(User user)
        {
            TryValidateModel(user);
            if (ModelState.IsValid)
            {
                user.Status = "Not Verified";
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
                await SendVerificationMailAsync(user);
                return Created("", new
                {
                    user.Id,
                    user.Name,
                    user.UserName,
                    user.Email,
                    user.Role
                });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("token", Name = "GetToken")]
        public ActionResult<dynamic> GetToken(LoginModel loginModel)
        {
            TryValidateModel(loginModel);
            if (ModelState.IsValid)
            {
                var user = dbContext.Users.SingleOrDefault(s => s.UserName == loginModel.UserName && s.Password == loginModel.Password && s.Status == "Verified");
                if (user != null)
                {
                    var token = GernerateToken(user);
                    return Ok(
                        new {
                            user.Name,
                            user.Email,
                            user.UserName,
                            user.Role,
                            Token = token
                        });
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [NonAction]
        private string GernerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "catalogapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "orderapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "basketapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "paymentapi"));

            claims.Add(new Claim(ClaimTypes.Role, user.Role));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:issuer"),
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        
        [NonAction]
        private async Task SendVerificationMailAsync(User user)
        {
            var userObj = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UserName
            };
            var messageText = JsonConvert.SerializeObject(userObj);
            StorageAccountHelper storageAccountHelper = new StorageAccountHelper();
            storageAccountHelper.StorageConnectionString = configuration.GetConnectionString("StorageConnectionString");
            await storageAccountHelper.SendMessageToQueueAsync(messageText, "users");
        }
    }
}