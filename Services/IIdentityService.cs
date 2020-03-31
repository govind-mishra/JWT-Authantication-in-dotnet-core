using Microsoft.AspNetCore.Identity;
using RestApiUsingCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Services
{
    public interface IIdentityService
    {
       
        Task<AuthanticationResult> RegisterAsync(string email, string password);

        Task<AuthanticationResult> LoginAsync(string email, string password);
        Task<AuthanticationResult> RefreshTokenAsync(string jWTToken, string refreshToken);
    }
}
