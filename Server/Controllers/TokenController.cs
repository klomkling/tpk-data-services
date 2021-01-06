using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Controllers
{
[ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;

        public TokenController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            try
            {
                var response = _userService.Authenticate(model);
                if (response == null)
                    return _userService.IsError
                        ? StatusCode(StatusCodes.Status500InternalServerError, _userService.Exception.Message)
                        : BadRequest(new {message = "Username or password does not match!"});

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new {message = e.Message});
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] AuthenticateResponse model)
        {
            try
            {
                var refreshToken = model.RefreshToken;
                var tokenRequest = new TokenRequest {RefreshToken = refreshToken};
                var response = _userService.RefreshToken(tokenRequest);
                if (response == null)
                    return _userService.IsError
                        ? StatusCode(StatusCodes.Status500InternalServerError, _userService.Exception.Message)
                        : Unauthorized(new {message = "Invalid token"});

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new {message = e.Message});
            }
        }

        [AuthorizeRequired]
        [HttpPost("logout")]
        public IActionResult RevokeToken()
        {
            try
            {
                // Get current user
                var user = _userService.CurrentUser;
                if (user == null) return BadRequest(new {message = "Not logged in!"});

                _userService.RevokeToken();

                return Ok(new {message = "Logged out"});
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new {message = e.Message});
            }
        }

        [AuthorizeRequired]
        [HttpPost("validate")]
        public IActionResult ValidateToken()
        {
            return Ok(new {message = "Valid"});
        }
    }
}