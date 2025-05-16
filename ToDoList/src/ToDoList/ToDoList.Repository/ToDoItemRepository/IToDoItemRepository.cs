using ToDoList.Dal.Entity;

namespace ToDoList.Repository.ToDoItemRepository;

public interface IToDoItemRepository
{
    Task<long> InsertToDoItemAsync(ToDoItem toDoItem); 
    Task DeleteToDoItemByIdAsync(long toDoItemID, long userId); 
    Task UpdateToDoItemAsync(ToDoItem toDoItem); 
    Task<ICollection<ToDoItem>> SelectAllToDoItemsAsync(long userID, int skip, int take);
    Task<ToDoItem> SelectToDoItemByIdAsync(long toDoItemID, long userId); 
    Task<ICollection<ToDoItem>> SelectByDueDateAsync(DateTime dueDate, long userId); 
    Task<ICollection<ToDoItem>> SelectCompletedAsync(long userID, int skip, int take); 
    Task<ICollection<ToDoItem>> SelectIncompleteAsync(long userID, int skip, int take);
    Task<ICollection<ToDoItem>> SearchToDoItemsAsync(string keyword, long userID); 
    Task<ICollection<ToDoItem>> SelectOverdueItemsAsync();
    Task<ICollection<ToDoItem>> GetUpcomingDeadlinesAsync();
    Task<int> SelectTotalCountAsync(long userID);

    IQueryable<ToDoItem> SelectAllToDoItems();
}


