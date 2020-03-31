using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestApiUsingCore.Data;
using RestApiUsingCore.Domain;
using RestApiUsingCore.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestApiUsingCore.Services
{
    public class IdentityService : IIdentityService
    {
        //identityuser is auto generted by ef it is class which contians all the registraion columns
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JWTSettings _jWTSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _context;

        public IdentityService(UserManager<IdentityUser> userManager, JWTSettings jWTSettings, ApplicationDbContext context, TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _jWTSettings = jWTSettings;
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<AuthanticationResult> LoginAsync(string email, string password)
        {
            var userExist = await _userManager.FindByEmailAsync(email);
            if(userExist == null)
            {
                return new AuthanticationResult
                {
                    Error = new[] { "User didn't found" }
                };
            }
            var getpassword = await _userManager.CheckPasswordAsync(userExist,password);
            if (!getpassword)
            {
                return new AuthanticationResult
                {
                    Error = new[] { "Password not correct" }
                };
            }

            return await generateJWTTokenAsync(userExist);


        }

        public async Task<AuthanticationResult> RegisterAsync(string email, string password)
        {
            var userExist = await _userManager.FindByEmailAsync(email);
            if (userExist != null)
                return new AuthanticationResult
                {
                    Error = new[] { "user with email already exist" },
                };
            var newuser = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newuser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthanticationResult
                {
                    Error = createdUser.Errors.Select(x => x.Description)
                };
            }

            #region JWT token handler
            /* Define first token handler object then key that is secret you described in jwtsettings
                 then all the claims in subject of secuirtytokendescription*/

            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTSettings.Secret);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    //claims define the bunch of property of our jwt tokens
                    new Claim(JwtRegisteredClaimNames.Sub, newuser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//jwt token identity it is unique identity of our token
                    new Claim(JwtRegisteredClaimNames.Email, newuser.Email),
                    new Claim("id", newuser.Id) //custom claim define id of user
                }),
                //define the expiration time
                Expires = DateTime.UtcNow.AddHours(2),
                //define signing credentials for what key and what algoritham we will use to make security key
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            //now create your token by above fields
            var token = tokenhandler.CreateToken(securityTokenDescriptor);
            #endregion

            return new AuthanticationResult
            {
                Success = true,
                JWTToken = tokenhandler.WriteToken(token)
            };
        }

        public async Task<AuthanticationResult> generateJWTTokenAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTSettings.Secret);
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.Add(_jWTSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //create jwt token
            var token = tokenHandler.CreateToken(securityTokenDescriptor);

            //create refresh token
            var refreshToken = new RefreshToken
            {
                JWTToken = token.Id,
                UserId = user.Id,
                RTokenCreationDate = DateTime.UtcNow,
                RTokenExpiryDate = DateTime.UtcNow.AddMonths(2)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthanticationResult
            {
                Success = true,
                JWTToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.RToken 
            };
        }

        public async Task<AuthanticationResult> RefreshTokenAsync(string jWTToken, string refreshToken)
        {
            var validatedToken = getClaimsPrincipalsfromToken(jWTToken);

            if (validatedToken == null)
            {
                return new AuthanticationResult
                {
                    Error = new[] { "Refresh Token Not found" }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                        .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthanticationResult
                {
                    Error = new[] { "your Refresh Token hasen't exired yet" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.JWTToken == refreshToken);

            if(storedRefreshToken == null)
            {
                return new AuthanticationResult { Error = new[] { "This Token hasen't expired yet" } };
            }

            if(DateTime.UtcNow > storedRefreshToken.RTokenExpiryDate)
            {
                return new AuthanticationResult { Error = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.inValidated)
            {
                return new AuthanticationResult { Error = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.isUsedToken)
            {
                return new AuthanticationResult { Error = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JWTToken != jti)
            {
                return new AuthanticationResult { Error = new[] { "Refresh token does not match the jwttoken" } };
            }

            storedRefreshToken.isUsedToken = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

            return await generateJWTTokenAsync(user);
        }

        //get claims to find out that the token is same as we assign to the client or different
        private ClaimsPrincipal getClaimsPrincipalsfromToken(string JWTToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(JWTToken, _tokenValidationParameters, out var validateToken);
                if (!isJWTTokenSecurityAlogSame(validateToken))
                {
                    return null;
                }
                return principal;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        //get claims to find out that the token security algoritham is also same or not
        private bool isJWTTokenSecurityAlogSame(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
