using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Bll.DTOs;
using ToDoList.Bll.Services;

namespace ToDoList.Server.Controller;

[Authorize]
[Route("api/toDoList")]
[ApiController]
public class ToDoListController : ControllerBase
{
    private readonly IToDoItemService _toDoItemService;
    private readonly ILogger<ToDoListController> _logger;

    public ToDoListController(IToDoItemService toDoItemService, ILogger<ToDoListController> logger)
    {
        _toDoItemService = toDoItemService;
        _logger = logger;
    }

    [HttpPost("add")]
    public async Task<long> AddToDoItem(ToDoItemCreateDto toDoItemCreateDto)
    {
        _logger.LogInformation("AddToDoItem method worked");
        var result = await _toDoItemService.AddToDoItemAsync(toDoItemCreateDto);
        return result;
    }
    
    [HttpDelete("delete")]
    public async Task DeleteToDoItemByIdAsync(long id)
    {
        _logger.LogInformation("DeleteToDoItemByIdAsync method worked");

        await _toDoItemService.DeleteToDoItemByIdAsync(id);
    }

    [HttpGet("getCompleted")]
    public Task<GetAllResponseModel> GetCompletedAsync(int skip, int take)
    {
        return _toDoItemService.GetCompletedAsync(skip, take);
    }

    
    [HttpGet("getAll")]
    public async Task<GetAllResponseModel> GetAllToDoItemsAsync(int skip, int take)
    {
        _logger.LogInformation($"GetAllToDoItemsAsync method worked");
        var userId = Int64.Parse(User.FindFirst("UserId")?.Value!);
        return await _toDoItemService.GetAllToDoItemsAsync(userId, skip, take);
    }

    [HttpGet("getById")]
    public async Task<ToDoItemGetDto> GetToDoItemByIdAsync(long id)
    {
        return await _toDoItemService.GetToDoItemByIdAsync(id);
    }

    [HttpGet("getByDueDate")]
    public Task<List<ToDoItemGetDto>> GetByDueDateAsync(DateTime dueDate)
    {
        return _toDoItemService.GetByDueDateAsync(dueDate);
    }

    [HttpGet("getIncompleted")]
    public Task<GetAllResponseModel> GetIncompleteAsync(int skip, int take)
    {
        return _toDoItemService.GetIncompleteAsync(skip, take);
    }

    [HttpPut("update")]
    public async Task UpdateToDoItemAsync(ToDoItemUpdateDto toDoItemUpdateDto)
    {
        _logger.LogInformation("UpdateToDoItemAsync method worked");

        await _toDoItemService.UpdateToDoItemAsync(toDoItemUpdateDto);
    }
}
