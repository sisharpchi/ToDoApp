using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ToDoList.Errors;
using ToDoList.Bll.DTOs;
using ToDoList.Dal.Entity;
using ToDoList.Repository.ToDoItemRepository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ToDoList.Bll.Services
{
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
            covert.UserId = Int64.Parse(userId!);
            //toDoItem
            var id = await _toDoItemRepository.InsertToDoItemAsync(covert);
            return id;
        }

        public async Task DeleteToDoItemByIdAsync(long id)
        {
            var item = await _toDoItemRepository.SelectToDoItemByIdAsync(id);
            if (item is null)
            {
                throw new NotFoundException($"ToDoItem with id {id} not found.");
            }
            await _toDoItemRepository.DeleteToDoItemByIdAsync(id);
        }

        public async Task<GetAllResponseModel> GetAllToDoItemsAsync(long userId, int skip, int take)
        {
            var toDoItems = await _toDoItemRepository.SelectAllToDoItemsAsync(userId, skip, take);
            var totalCount = await _toDoItemRepository.SelectTotalCountAsync();
          

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
            var result = await _toDoItemRepository.SelectByDueDateAsync(dueDate);
            return result.Select(item => _mapper.Map<ToDoItemGetDto>(item)).ToList();
        }

        public async Task<GetAllResponseModel> GetCompletedAsync(int skip, int take)
        {
            var completedItems = await _toDoItemRepository.SelectCompletedAsync(skip, take);
            var totalCount = await _toDoItemRepository.SelectTotalCountAsync();

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
            var incompleteItems = await _toDoItemRepository.SelectIncompleteAsync(skip, take);
            
            var totalCount = await _toDoItemRepository.SelectTotalCountAsync();
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

        public async Task<ToDoItemGetDto> GetToDoItemByIdAsync(long id)
        {
            var founded = await _toDoItemRepository.SelectToDoItemByIdAsync(id);
            if (founded == null)
            {
                throw new NotFoundException($"ToDoItem with id {id} not found.");
            }
            return _mapper.Map<ToDoItemGetDto>(founded);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _toDoItemRepository.SelectTotalCountAsync();
        }

        public async Task UpdateToDoItemAsync(ToDoItemUpdateDto newItem)
        {
            var validationResult = _toDoItemUpdateDtoValidator.Validate(newItem);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _toDoItemRepository.UpdateToDoItemAsync(_mapper.Map<ToDoItem>(newItem));
        }
    }
}
