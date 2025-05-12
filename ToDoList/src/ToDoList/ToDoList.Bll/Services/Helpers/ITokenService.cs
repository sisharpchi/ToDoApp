
using ToDoList.Bll.DTOs;

namespace ToDoList.Bll.Services.Helpers;

public interface ITokenService
{
    public string GenerateToken(UserGetDto user);
}
