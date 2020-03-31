using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestApiUsingCore.Contracts.v1;
using RestApiUsingCore.Contracts.v1.Requests;
using RestApiUsingCore.Contracts.v1.Responses;
using RestApiUsingCore.Services;

namespace RestApiUsingCore.Controllers.v1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _identityService.RegisterAsync(registrationRequest.Email, registrationRequest.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthErrorResponse
                {
                    Errors = authResponse.Error
                });
            }

            return Ok(new AuthSuccessResponse
            {
                JWTToken = authResponse.JWTToken,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            var loginResponse = await _identityService.LoginAsync(userLoginRequest.Email, userLoginRequest.Password);
            if (!loginResponse.Success)
            {
                return BadRequest(new AuthErrorResponse
                {
                    Errors = loginResponse.Error
                });
            }
            return Ok(new AuthSuccessResponse { 
                JWTToken = loginResponse.JWTToken,
                RefreshToken = loginResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest _refreshTokenRequest)
        {
            var refreshTokenResponse = await _identityService.RefreshTokenAsync(_refreshTokenRequest.JWTToken, _refreshTokenRequest.RefreshToken);
            if (!refreshTokenResponse.Success)
            {
                return BadRequest(new AuthErrorResponse
                {
                    Errors = refreshTokenResponse.Error
                });
            }
            return Ok(new AuthSuccessResponse
            {
                JWTToken = refreshTokenResponse.JWTToken,
                RefreshToken = refreshTokenResponse.RefreshToken
            });
        }
    }
}