using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ToDoList.Bll.DTOs;
using ToDoList.Dal.Entity;
using ToDoList.Errors;
using ToDoList.Repository.ToDoItemRepository;

namespace ToDoList.Bll.Services;


public class ToDoItemService : IToDoItemService
{
    private readonly IToDoItemRepository _toDoItemRepository;
    private readonly IValidator<ToDoItemCreateDto> _toDoItemCreateDtoValidator;
    private readonly IValidator<ToDoItemUpdateDto> _toDoItemUpdateDtoValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<ToDoItemService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ToDoItemService(IToDoItemRepository toDoItemRepository, IValidator<ToDoItemCreateDto> validator, IMapper mapper, ILogger<ToDoItemService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _toDoItemRepository = toDoItemRepository;
        _toDoItemCreateDtoValidator = validator;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<long> AddToDoItemAsync(ToDoItemCreateDto toDoItem)
    {
        var validationResult = _toDoItemCreateDtoValidator.Validate(toDoItem);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        ArgumentNullException.ThrowIfNull(toDoItem);
        var covert = _mapper.Map<ToDoItem>(toDoItem);

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        covert.UserId = Int64.Parse(userId);

        var id = await _toDoItemRepository.InsertToDoItemAsync(covert);
        return id;
    }

    public async Task DeleteToDoItemByIdAsync(long id)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

        var item = await _toDoItemRepository.SelectToDoItemByIdAsync(id, Int64.Parse(userId!));
        if (item is null)
        {
            throw new NotFoundException($"ToDoItem with id {id} not found.");
        }
        await _toDoItemRepository.DeleteToDoItemByIdAsync(id, Int64.Parse(userId));
    }

    public async Task<GetAllResponseModel> GetAllToDoItemsAsync(int skip, int take)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

        var toDoItems = await _toDoItemRepository.SelectAllToDoItemsAsync(Int64.Parse(userId), skip, take);
        var totalCount = await _toDoItemRepository.SelectTotalCountAsync(Int64.Parse(userId));


        var toDoItemDtos = toDoItems
            .Select(item => _mapper.Map<ToDoItemGetDto>(item))
            .ToList();

        var getAllResponseModel = new GetAllResponseModel()
        {
            ToDoItemGetDtos = toDoItemDtos,
            TotalCount = totalCount,
        };

        return getAllResponseModel;
    }

    public async Task<List<ToDoItemGetDto>> GetByDueDateAsync(DateTime dueDate)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value; 

        var result = await _toDoItemRepository.SelectByDueDateAsync(dueDate, Int64.Parse(userId)); 
        return result.Select(item => _mapper.Map<ToDoItemGetDto>(item)).ToList();
    }

    public async Task<GetAllResponseModel> GetCompletedAsync(int skip, int take)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;   
        var completedItems = await _toDoItemRepository.SelectCompletedAsync(Int64.Parse(userId), skip, take); 
        var totalCount = await _toDoItemRepository.SelectTotalCountAsync(Int64.Parse(userId));

        var toDoItemDtos = completedItems
                   .Select(item => _mapper.Map<ToDoItemGetDto>(item))
                   .ToList();

        var getAllResponseModel = new GetAllResponseModel()
        {
            ToDoItemGetDtos = toDoItemDtos,
            TotalCount = totalCount,
        };

        return getAllResponseModel;
    }

    public async Task<GetAllResponseModel> GetIncompleteAsync(int skip, int take)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

        var incompleteItems = await _toDoItemRepository.SelectIncompleteAsync(Int64.Parse(userId), skip, take);

        var totalCount = await _toDoItemRepository.SelectTotalCountAsync(Int64.Parse(userId));
        var incompleteDtos = incompleteItems
            .Select(item => _mapper.Map<ToDoItemGetDto>(item))
            .ToList();

        var getAllResponseModel = new GetAllResponseModel()
        {
            ToDoItemGetDtos = incompleteDtos,
            TotalCount = totalCount,
        };

        return getAllResponseModel;
    }

    public async Task<ICollection<ToDoItemGetDto>> GetSearchedToDoItemAsync(string keyword)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;

        var result = await _toDoItemRepository.SearchToDoItemsAsync(keyword, Int64.Parse(userId));
        return result.Select(item => _mapper.Map<ToDoItemGetDto>(item)).ToList();
    }

    public async Task<ToDoItemGetDto> GetToDoItemByIdAsync(long id) 
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        var founded = await _toDoItemRepository.SelectToDoItemByIdAsync(id, Int64.Parse(userId));

        if (founded == null)
        {
            throw new NotFoundException($"ToDoItem with id {id} not found.");
        }
        return _mapper.Map<ToDoItemGetDto>(founded);
    }

    public async Task<int> GetTotalCountAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
        if (userId is null) throw new ArgumentNullException("UserId is null");
        var convert = Int64.Parse(userId);
        return await _toDoItemRepository.SelectTotalCountAsync(convert);
    }

    public async Task UpdateToDoItemAsync(ToDoItemUpdateDto newItem)
    {
        ArgumentNullException.ThrowIfNull(newItem);
        var convert = _mapper.Map<ToDoItem>(newItem);

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;  
        convert.UserId = Int64.Parse(userId);                                           

        var validationResult = _toDoItemUpdateDtoValidator.Validate(newItem);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _toDoItemRepository.UpdateToDoItemAsync(convert);
    }
}
