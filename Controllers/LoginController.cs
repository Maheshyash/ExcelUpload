using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExcelReportUpload.Models;
using ExcelReportUpload.IRepositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.PortableExecutable;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExcelReportUpload.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IConfiguration _configuration;

        public LoginController(ILoginRepository loginRepository, IConfiguration configuration)
        {
            _loginRepository = loginRepository;
            _configuration = configuration;
        }
        [Authorize]
        [HttpPost("GetUserDetailsFromToken")]
        public IActionResult GetUserDetailsFromToken(string Token)
        {
            // Extract the token from the Authorization header
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            // Parse the token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Extract claims
            var loginIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "LoginId").Value;
            var claims = jwtToken.Claims.ToList();
            if (claims == null || !claims.Any())
            {
                return Unauthorized();
            }

            var claimDetails = claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(claimDetails);
            // Other claims can be extracted similarly
            // var otherClaim = jwtToken.Claims.First(claim => claim.Type == "customClaimType").Value;

            // Return the extracted information
            //return Ok(new
            //{
            //    Username = loginIdClaim
            //    // OtherClaim = otherClaim
            //});
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            var data = _loginRepository.LoginCheck1(user);
            var key = _configuration["JWT:Key"];
            var Issuer = _configuration["JWT:Issuer"];
            var Audience = _configuration["JWT:Audience"];
            var token1 = GenerateToken(user, key, Issuer, Audience);
            return Ok(new { data, token=token1});
        }
        //[HttpPost]
        //public async Task<IActionResult> LogindAsync(User user)
        //{
        //    //var data = await _loginRepository.LoginCheckAsync(user);
        //    return Ok("");
        //}

        
        private string GenerateToken(User user, string key, string Issuer, string Audience)
        {
            List<Claim> claims = new List<Claim>
            {

                new Claim("LoginId", user.UserName.ToString()),

            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var siginCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(Issuer, Audience, claims: claims, expires: DateTime.Now.AddMinutes(10), signingCredentials: siginCredential);
            var token1 = new JwtSecurityTokenHandler().WriteToken(token);
            return token1;
        }
        



    }
}

