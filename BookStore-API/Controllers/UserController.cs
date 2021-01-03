
using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        public UserController(SignInManager<IdentityUser> signInManager , 
                                UserManager<IdentityUser> userManager ,
                                ILoggerService logger,
                                IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }
        /// <summary>
        /// User Login point
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>User details</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task <IActionResult> Login([FromBody] UserDTO userDTO)
        {
            
                var location = getControllerDetails();

                try
                {
                var username = userDTO.Username;
                var password = userDTO.Password;
                _logger.LogInfo($"{location} Call Started for getting login");
                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    _logger.LogInfo($"{location} Successfully completed");

                    var tockenString = await GenerateJSONWebToken(user);
                    //to return newly generated tocken if the login is success.
                    return Ok(new { token = tockenString });
                }
                _logger.LogWarn($"{location} No authorized details found");
                return Unauthorized(userDTO);
                }
                catch (Exception e)
                {

                    return internalError($"{location} - {e.Message} - {e.InnerException}");
                }
            
        
       
        }



        private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, $"Some error occured,Please contact administrator");
        }

        private string getControllerDetails()
        {
            var contollr = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $" {contollr} - {action}";
        }
        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim (JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultNameClaimType, r)));
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims ,
                null ,
                expires : DateTime.Now.AddMinutes(5),
                signingCredentials :credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
