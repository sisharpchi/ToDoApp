using FluentValidation;
using System.Security.Claims;
using ToDoList.Bll.DTOs;
using ToDoList.Bll.Services.Helpers;
using ToDoList.Bll.Services.Helpers.Security;
using ToDoList.Core.Errors;
using ToDoList.Dal.Entity;
using ToDoList.Errors;
using ToDoList.Repository.ToDoItemRepository;

namespace ToDoList.Bll.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository UserRepository;
    private readonly ITokenService TokenService;
    private readonly IValidator<UserCreateDto> _userCreateDtoValidator;
    private readonly IValidator<UserLoginDto> _userLoginDtoValidator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(ITokenService tokenService, IUserRepository userRepository, IValidator<UserCreateDto> userCreateDtoValidator, IValidator<UserLoginDto> userLoginDtoValidator, IRefreshTokenRepository refreshTokenRepository)
    {
        TokenService = tokenService;
        UserRepository = userRepository;
        _userCreateDtoValidator = userCreateDtoValidator;
        _userLoginDtoValidator = userLoginDtoValidator;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto)
    {
        var validationResult = await _userLoginDtoValidator.ValidateAsync(userLoginDto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();

            throw new ValidationException(string.Join("; ", errors));
        }

        var user = await UserRepository.SelectUserByUserNameAsync(userLoginDto.UserName);

        var checkUserPassword = PasswordHasher.Verify(userLoginDto.Password, user.Password, user.Salt);

        if (checkUserPassword == false)
        {
            throw new UnauthorizedException("UserName or password incorrect");
        }

        var userGetDto = new UserGetDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = (UserRoleDto)user.Role,
        };

        var token = TokenService.GenerateToken(userGetDto);
        var refreshToken = TokenService.GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken()
        {
            Token = refreshToken,
            Expiration = DateTime.UtcNow.AddDays(21),
            UserId = user.UserId,
            IsRevoked = false,
        };

        await _refreshTokenRepository.InsertRefreshTokenAsync(refreshTokenEntity);

        var loginResponseDto = new LoginResponseDto()
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            Expires = 24
        };


        return loginResponseDto;
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto refreshRequestDto)
    {
        ClaimsPrincipal? principal = TokenService.GetPrincipalFromExpiredToken(refreshRequestDto.AccessToken);
        if (principal == null) throw new ForbiddenException("Invalid access token.");

        var userId = long.Parse(principal.FindFirst(c => c.Type == "UserId")!.Value);

        var refreshToken = await _refreshTokenRepository.SelectRefreshTokenAsync(refreshRequestDto.RefreshToken, userId);
        if (refreshToken == null || refreshToken.Expiration < DateTime.UtcNow || refreshToken.IsRevoked)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        refreshToken.IsRevoked = true;

        var user = await UserRepository.SelectUserByIdAsync(userId);

        var userGetDto = new UserGetDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = (UserRoleDto)user.Role,
        };

        var newAccessToken = TokenService.GenerateToken(userGetDto);
        var newRefreshToken = TokenService.GenerateRefreshToken();

        var refreshTokenToDB = new RefreshToken()
        {
            Token = newRefreshToken,
            Expiration = DateTime.UtcNow.AddDays(21),
            IsRevoked = false,
            UserId = user.UserId
        };

        await _refreshTokenRepository.InsertRefreshTokenAsync(refreshTokenToDB);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            Expires = 900
        };
    }

    public async Task<long> SignUpUserAsync(UserCreateDto userCreateDto)
    {
        var validationResult = await _userCreateDtoValidator.ValidateAsync(userCreateDto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();

            throw new ValidationException(string.Join("; ", errors));
        }

        var tupleFromHasher = PasswordHasher.Hasher(userCreateDto.Password);
        var user = new User()
        {
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            UserName = userCreateDto.UserName,
            Email = userCreateDto.Email,
            PhoneNumber = userCreateDto.PhoneNumber,
            Password = tupleFromHasher.Hash,
            Salt = tupleFromHasher.Salt,
            Role = UserRole.User,
        };

        return await UserRepository.InsertUserAsync(user);
    }
}
