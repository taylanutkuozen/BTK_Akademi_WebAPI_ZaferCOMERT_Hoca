 using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Services
{
    public class AuthenticationManager : IAuthenticationService
    {
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;/*Burası Framework tarafından tanımlanmaktadır.*/
        private readonly IConfiguration _configuration;
        private User? _user;
        public AuthenticationManager(ILoggerService logger, IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<TokenDto> CreateToken(bool populateExpire)
        {
            var signingCredentials = GetSignInCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;
            if(populateExpire)
            {
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            }
            await _userManager.UpdateAsync(_user);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto 
            { 
                AccessToken = accessToken, 
                RefreshToken = refreshToken 
            };
        }
        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
        {
            var user = _mapper.Map<User>(userForRegistrationDto);
            /*Bize bir USER lazım. Nihai olarak userForRegistrationDto'dan gelecek.*/
            var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);
            if(result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, userForRegistrationDto.Roles);
            }
            return result;
            /*User nesneleri verilecek bunları alacağız.*/
        }
        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuthDto)
        {
            _user= await _userManager.FindByNameAsync(userForAuthDto.UserName);
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuthDto.Password));
            if (!result)
            {
              _logger.LogWarning($"{nameof(ValidateUser)} : Authentication failed. Wrong username or password.");
            }
            return result;
        }
        private SigningCredentials GetSignInCredentials()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
            var secret=new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            /*Amaç kimlik bilgilerini doğrulamak*/
        }
        private async Task<List<Claim>> GetClaims() //Claims burada hak,ihtar veya roller olarak tanımlanmaktadır.
        {
            var claims= new List<Claim>()
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };
            var roles = await _userManager.GetRolesAsync(_user);//GetRolesAsync bir user ister.List<string> olarak geliyor.
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims : claims,
                expires : DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signingCredentials
            );
            return tokenOptions;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator=RandomNumberGenerator.Create())/*Masraflı bir iş yapıldığı zaman using ifadesini bu şekilde kullanırız. GarbageCollector devreye girecek !!! */
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)/*Principal gördüğümüzde kullanıcı bilgilerinin istendiğini anlamamız gerekiyor*/
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];
            /*Aşağıdaki komut token'ı doğrulamak için parametre tanımlarını içerir.*/
            var tokenValidationParameters = new TokenValidationParameters
            { /*ServiceExtensions içerisinde açıklamalar mevcut*/
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
            var tokenHandler=new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal=tokenHandler.ValidateToken(token,tokenValidationParameters, out securityToken);/*Token validate ederken ilk parametre token, ikinci parametre token'ı biz mi ürettik kontrolü sağlanması için gerekli parametreleri içeren bir nesne, üçüncü parametre bir out değişkene ata. ref,out,params=>parametre düzenleyicileri. securityToken değeri ValidateToken metodu çalıştıktan sonra set edilmiş olacaktır.*/
            var jwtSecurityToken =  securityToken as JwtSecurityToken;/*cast işlemi JwtSecurityToken'a çevir. Convert işlemi başarılı olmazsa null başarılı olursa değer alacaktır.*/
            if(jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token.");
            }
            return principal;
        }
        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if(user is null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime<=DateTime.Now)
            {
                throw new RefreshTokenBadRequestException();
            }
            _user = user;
            return await CreateToken(populateExpire: false);
        }
    }
}