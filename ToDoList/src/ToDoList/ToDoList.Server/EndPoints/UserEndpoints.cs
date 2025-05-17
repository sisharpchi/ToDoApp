using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Bll.DTOs;
using ToDoList.Bll.Services;

namespace ToDoList.Server.EndPoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/userMinimalApi")
            .RequireAuthorization()
            .WithTags("UserMinimalApi");

        // DELETE /api/user/delete
        userGroup.MapDelete("/delete", [Authorize(Roles = "Admin,SuperAdmin")] 
        async (long userId, HttpContext httpContext, IUserService userService) =>
        {
            var role = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            await userService.DeleteUserByIdAsync(userId, role!);
            return Results.Ok();
        })
        .WithName("DeleteUser");

        // PATCH /api/user/updateRole
        userGroup.MapPatch("/updateRole", [Authorize(Roles = "SuperAdmin")] 
        async ( long userId, UserRoleDto userRoleDto, IUserService userService) =>
        {
            await userService.UpdateUserRoleAsync(userId, userRoleDto);
            return Results.Ok();
        })
        .WithName("UpdateUserRole");
    }
}
