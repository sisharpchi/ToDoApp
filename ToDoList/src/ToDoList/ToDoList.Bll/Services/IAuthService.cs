using ToDoList.Bll.DTOs;

namespace ToDoList.Bll.Services;

public interface IAuthService
{
    Task<long> SignUpUserAsync(UserCreateDto userCreateDto);
    Task<LoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto);
}