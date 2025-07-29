namespace BookShop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        Task SaveAsync();
        Task ResetIdentityColumnsAsync();
        
        // Synchronous methods commented out to force async usage
        /*
        void Save();
        void ResetIdentityColumns();
        */
    }
}
