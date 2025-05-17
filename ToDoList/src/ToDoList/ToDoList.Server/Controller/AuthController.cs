using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Bll.DTOs;
using ToDoList.Bll.Services;

namespace ToDoList.Server.Controller;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService AuthService;
    public AuthController(IAuthService authService)
    {
        AuthService = authService;
    }

    [HttpPost("signUp")]
    public async Task<long> SignUp(UserCreateDto userCreateDto)
    {
        return await AuthService.SignUpUserAsync(userCreateDto);
    }

    [HttpPost("login")]
    public async Task<LoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        return await AuthService.LoginUserAsync(userLoginDto);
    }

    [HttpPost("refreshToken")]
    public async Task<LoginResponseDto> RefreshToken(RefreshRequestDto refreshRequestDto)
    {
        return await AuthService.RefreshTokenAsync(refreshRequestDto);
    }

    [HttpDelete("logout")]
    public async Task LogOut(string token)
    {
        await AuthService.LogOut(token);
    }
}
