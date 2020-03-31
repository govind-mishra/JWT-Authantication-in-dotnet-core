using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RestApiUsingCore.Options;
using RestApiUsingCore.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace RestApiUsingCore.Installer
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<IIdentityService, IdentityService>();
            #region JWT settings
            var jwtsettings = new JWTSettings();
            configuration.Bind(nameof(jwtsettings), jwtsettings); // nameof give you the variable name or object name in string format, used to avoid hardcoding
            services.AddSingleton(jwtsettings);
            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                //here we define the last parameter of jwt token that is the signature made by the header,payload and secretkey
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtsettings.Secret)),
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddSingleton(tokenValidationParameters);//so that we can use it anywhere with single object
            #endregion
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info { Title = "Tweetbook API", Version = "v1" });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer",new string[0] }
                };

                x.AddSecurityDefinition(
                      "Bearer",
                      new ApiKeyScheme
                      {
                          Description = "JWT authorization header using Bearer Scheme",
                          Name = "Authorization",
                          In = "header",
                          Type = "apiKey"
                      }
                    );
                });
        }
    }
}
