using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsersService.Application.Common;
using UsersService.Application.Exceptions;
using UsersService.Application.Interfaces;

namespace UsersService.Application.Services
{
    public class JwtAuthenticationService : IAuthenticationService<JwtTokens>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHashService _passwordHashService;
        private readonly ILogger<JwtAuthenticationService> _logger;

        public JwtAuthenticationService(IUsersRepository usersRepository,
            IConfiguration configuration,
            IPasswordHashService passwordHashService,
            ILogger<JwtAuthenticationService> logger)
        {
            _usersRepository = usersRepository;
            _configuration = configuration;
            _passwordHashService = passwordHashService;
            _logger = logger;
        }

        public async Task<Result<JwtTokens>> Authenticate(string login, string password)
        {
            var user = await _usersRepository.GetUser(login);
            if(user == null || !_passwordHashService.VerifyPassword(password, user.Password))
            {
                var ex = new AuthenticationException();
                _logger.LogError(ex, "An error occurred while authenticating user with login: {Login}", login);
                return new Result<JwtTokens> { IsSuccess = false, Exception = ex};
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.IsAdmin? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = new JwtTokens(tokenHandler.WriteToken(token));
            return new Result<JwtTokens> { IsSuccess = true, Value = jwtToken};
        }
    }
}
