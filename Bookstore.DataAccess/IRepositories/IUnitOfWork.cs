namespace Bookstore.DataAccess.IRepositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        Task SaveChangesAsync();
    }
}
