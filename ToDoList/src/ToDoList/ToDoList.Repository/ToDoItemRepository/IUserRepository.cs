using ToDoList.Dal.Entity;

namespace ToDoList.Repository.ToDoItemRepository;

public interface IUserRepository
{
    Task<long> InsertUserAsync(User user);
    Task<User> SelectUserByIdAsync(long id);
    Task<User> SelectUserByUserNameAsync(string userName);
}