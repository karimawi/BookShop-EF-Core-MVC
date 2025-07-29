using BookShop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
        }

        // Async methods
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task ResetIdentityColumnsAsync()
        {
            // Reset identity for Products table
            await _db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('MasterSchema.Products', RESEED, 0)");
            
            // Reset identity for Categories table
            await _db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('MasterSchema.Categories', RESEED, 0)");
        }

        // Synchronous methods (commented out to force async usage)
        /*
        public void Save()
        {
            _db.SaveChanges();
        }

        public void ResetIdentityColumns()
        {
            // Reset identity for Products table
            _db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('MasterSchema.Products', RESEED, 0)");
            
            // Reset identity for Categories table
            _db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('MasterSchema.Categories', RESEED, 0)");
        }
        */
    }
}
