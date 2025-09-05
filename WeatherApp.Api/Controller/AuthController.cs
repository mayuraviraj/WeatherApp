using Microsoft.AspNetCore.Mvc;
using WeatherApp.Application.DTOs;
using WeatherApp.Infrastructure.Security;

namespace WeatherApp.Api.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController(ITokenService _tokenService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequestDto request) {
        // Replace with real user validation
        if (request.Username == "mayura" && request.Password == "secure123") {
            var token = _tokenService.CreateToken(request.Username, "User");
            return Ok(new AuthResponse {
                Token = token,
                Username = request.Username,
                Role = "User"
            });
        }

        return Unauthorized("Invalid credentials");
    }
}