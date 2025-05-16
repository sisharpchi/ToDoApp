using Microsoft.EntityFrameworkCore;
using ToDoList.Dal;
using ToDoList.Dal.Entity;

namespace ToDoList.Repository.ToDoItemRepository;

public class ToDoItemRepository : IToDoItemRepository
{
    private readonly MainContext MainContext;

    public ToDoItemRepository(MainContext mainDbContext)
    {
        MainContext = mainDbContext;
    }

    public async Task DeleteToDoItemByIdAsync(long toDoItemID, long userId)
    {
        var toDoItem = await SelectToDoItemByIdAsync(toDoItemID, userId);
        MainContext.ToDoItems.Remove(toDoItem);
        await MainContext.SaveChangesAsync();
    }

    public Task<ICollection<ToDoItem>> GetUpcomingDeadlinesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<long> InsertToDoItemAsync(ToDoItem toDoItem)
    {
        await MainContext.ToDoItems.AddAsync(toDoItem);
        await MainContext.SaveChangesAsync();
        return toDoItem.ToDoItemId;
    }

    public async Task<ICollection<ToDoItem>> SearchToDoItemsAsync(string keyword, long userID)
    {
        return await MainContext.ToDoItems.Where(t => t.Title.Contains(keyword) && t.UserId == userID).ToListAsync();
    }

    public async Task<ICollection<ToDoItem>> SelectAllToDoItemsAsync(long userID, int skip, int take)
    {
        if (skip < 0 || take <= 0)
        {
            throw new ArgumentOutOfRangeException("Skip and take must be non-negative and take must be greater than zero.");
        }

        return await MainContext.ToDoItems
              .Where(t => t.UserId == userID)
              .Skip(skip)
              .Take(take)
              .ToListAsync();
    }

    public IQueryable<ToDoItem> SelectAllToDoItems()
    {
        return MainContext.ToDoItems;
    }

    public async Task<ICollection<ToDoItem>> SelectByDueDateAsync(DateTime dueTime, long userId)
    {
        var query = MainContext.ToDoItems
            .Where(t => t.DueDate.Date == dueTime && t.UserId == userId);
        return await query.ToListAsync();
    }

    public async Task<ICollection<ToDoItem>> SelectCompletedAsync(long userID, int skip, int take)
    {
        if (skip < 0 || take <= 0)
        {
            throw new ArgumentOutOfRangeException("Skip and take must be non-negative and take must be greater than zero.");
        }

        var query = MainContext.ToDoItems
            .Where(t => t.IsCompleted && t.UserId == userID)
            .Skip(skip)
            .Take(take);

        return await query.ToListAsync();
    }

    public async Task<ICollection<ToDoItem>> SelectIncompleteAsync(long userID, int skip, int take)
    {
        if (skip < 0 || take <= 0)
        {
            throw new ArgumentOutOfRangeException("Skip and take must be non-negative and take must be greater than zero.");
        }
        var query = MainContext.ToDoItems
            .Where(t => !t.IsCompleted && t.UserId == userID)
            .Skip(skip)
            .Take(take);

        return await query.ToListAsync();
    }

    public Task<ICollection<ToDoItem>> SelectOverdueItemsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ToDoItem> SelectToDoItemByIdAsync(long toDoItemID, long userId)
    {
        var toDoItem = await MainContext.ToDoItems.FirstOrDefaultAsync(x => x.ToDoItemId == toDoItemID && x.UserId == userId);

        return toDoItem;
    }

    public async Task<int> SelectTotalCountAsync(long userID)
    {
        return await MainContext.ToDoItems.Where(t => t.UserId == userID).CountAsync();
    }

    public async Task UpdateToDoItemAsync(ToDoItem toDoItem)
    {
        MainContext.ToDoItems.Update(toDoItem);
        await MainContext.SaveChangesAsync();
    }
}