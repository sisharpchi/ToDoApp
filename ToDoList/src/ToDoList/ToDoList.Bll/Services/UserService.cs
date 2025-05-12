﻿using ToDoList.Bll.DTOs;
using ToDoList.Dal.Entity;
using ToDoList.Errors;
using ToDoList.Repository.ToDoItemRepository;

namespace ToDoList.Bll.Services;

public class UserService : IUserService
{
    private readonly IUserRepository UserRepository;


    public UserService(IUserRepository userRepository)
    {
        UserRepository = userRepository;
    }

    public async Task DeleteUserByIdAsync(long userId, string userRole)
    {
        if (userRole == "SuperAdmin")
        {
            await UserRepository.DeleteUserByIdAsync(userId);
        }
        else if (userRole == "Admin")
        {
            var user = await UserRepository.SelectUserByIdAsync(userId);
            if(user.Role == UserRole.User)
            {
                await UserRepository.DeleteUserByIdAsync(userId);
            }
            else
            {
                throw new NotAllowedException("Admin can not delete Admin or SuperAdmin");
            }
        }
    }

    public async Task UpdateUserRoleAsync(long userId, UserRoleDto userRoleDto)
    {
        await UserRepository.UpdateUserRoleAsync(userId, (Dal.Entity.UserRole)userRoleDto);
    }
}
