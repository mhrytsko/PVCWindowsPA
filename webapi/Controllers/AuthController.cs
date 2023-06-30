using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using webapi.Database;
using webapi.Models;
using webapi.Security;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly UserManager<Models.User> _userManager;
        private readonly SignInManager<Models.User> _signInManager;

        public AuthController(IConfiguration config, AppDbContext context, UserManager<Models.User> userManager, SignInManager<Models.User> signInManager, IMapper mapper, ILogger<AuthController> logger) 
            : base(config, context, mapper, logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public class LoginRequest
        {
            [JsonPropertyName("username"), Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
            public string Username { get; set; } = null!;

            [JsonPropertyName("password"), Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            public string Password { get; set; } = null!;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginData)
        {
            // validate username/password
            if (!ModelState.IsValid 
                || string.IsNullOrWhiteSpace(loginData.Username) 
                || string.IsNullOrWhiteSpace(loginData.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "As credenciais fornecidas não são válidas. Por favor, tente novamente.",
                    errors = ModelState.Values.SelectMany(x => x.Errors)
                });
            }

            var auth = new Security.Authentification(_context, _userManager);
            // Check if user exists in database and verify password
            var user = await auth.GetUserByUsernameAndPassword(loginData.Username, loginData.Password);
            if (user == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "As credenciais fornecidas não são válidas. Por favor, tente novamente."
                });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { 
                success = true,
                token,
                id = user.Id,
                userName = user.UserName,
                role = user.Role,
                isAuthenticated = true,
                firstName = user.PersonalDetail?.FirstName,
                lastName = user.PersonalDetail?.LastName,
            });
        }

        private string? GenerateJwtToken(Models.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _config.GetValue<string>("Jwt:Key") ?? Authentification.TEMP_TOKEN;
            var audience = _config.GetValue<string>("Jwt:Audience") ?? Authentification.TEMP_AUDIENCE;
            var issuer = _config.GetValue<string>("Jwt:Issuer") ?? Authentification.TEMP_ISSUER;
            var expires = _config.GetValue<int?>("Jwt:Expires") ?? Authentification.TEMP_EXPIRES;

            var key = Encoding.ASCII.GetBytes(jwtSecret);

            // Create JWT token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //issuer: _configuration["Jwt:Issuer"],
                //audience: _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName ?? Models.User.USER_GUEST),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Audience = audience,
                Issuer = issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public class RegistrationRequest
        {
            //[Required]
            public string? Email { get; set; } = null!;
            [Required]
            public string Username { get; set; } = null!;
            [Required, MinLength(5)]
            public string Password { get; set; } = null!;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);


            var user = new Models.User { 
                UserName = request.Username, 
                Email = request.Email,
                PersonalDetail = new PersonalDetail() { 
                    Email = request.Email, 
                    State = Framework.BaseEnums.RowState.Valid 
                },
                Role = Framework.BaseEnums.PermissionLevel.Client,
                State = Framework.BaseEnums.RowState.Valid 
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // Atualizar o carimbo de segurança
                await _userManager.UpdateSecurityStampAsync(user);

                request.Password = "";
                return Ok(new { success = true });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);

            return ValidationProblem(ModelState);
        }

    }
}
