using System.Linq.Expressions;

namespace BookShop.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        Task<T> GetAsync(Expression<Func<T, bool>> filter, string includeProperties = "");
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        
        // Synchronous methods commented out to force async usage
        /*
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        T Get(Expression<Func<T, bool>> filter, string includeProperties = "");
        */
    }
}
