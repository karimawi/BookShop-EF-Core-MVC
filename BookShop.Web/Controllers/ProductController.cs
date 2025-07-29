using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PageSize = 10;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int page = 1, int? categoryFilter = null, string search = null)
        {
            var products = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => 
                    p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Category.CatName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryFilter.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryFilter.Value);
            }

            var productsList = products.ToList();
            var totalProducts = productsList.Count;
            var paginatedProducts = productsList
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;
            ViewBag.CategoryFilter = categoryFilter;
            ViewBag.SearchTerm = search;
            ViewBag.CategoryList = await GetCategorySelectListForFilterAsync();

            return View(paginatedProducts);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Price < 1 || product.Price > 1000)
                {
                    ModelState.AddModelError("Price", "Price must be between 1 and 1000");
                    ViewBag.CategoryList = await GetCategorySelectListAsync();
                    return View(product);
                }

                _unitOfWork.Product.Add(product);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            
            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (product.Price < 1 || product.Price > 1000)
                {
                    ModelState.AddModelError("Price", "Price must be between 1 and 1000");
                    ViewBag.CategoryList = await GetCategorySelectListAsync();
                    return View(product);
                }

                var existingProduct = await _unitOfWork.Product.GetAsync(p => p.Id == id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.Title = product.Title;
                existingProduct.Description = product.Description;
                existingProduct.Author = product.Author;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.CategoryList = await GetCategorySelectListAsync();
            return View(product);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            if (product != null)
            {
                _unitOfWork.Product.Remove(product);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
        {
            var categories = await _unitOfWork.Category.GetAllAsync(filter: c => !c.IsDeleted && c.IsActive);
            return categories.Select(c => new SelectListItem
            {
                Text = c.CatName,
                Value = c.Id.ToString()
            });
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListForFilterAsync()
        {
            var categories = await _unitOfWork.Category.GetAllAsync(filter: c => !c.IsDeleted && c.IsActive);
            var categoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CatName,
                Value = c.Id.ToString()
            }).ToList();

            // Add "All Categories" option at the beginning
            categoryList.Insert(0, new SelectListItem { Text = "All Categories", Value = "" });
            return categoryList;
        }

        // Synchronous helper methods commented out to force async usage
        /*
        private IEnumerable<SelectListItem> GetCategorySelectList()
        {
            return _unitOfWork.Category.GetAll(filter: c => !c.IsDeleted && c.IsActive)
                .Select(c => new SelectListItem
                {
                    Text = c.CatName,
                    Value = c.Id.ToString()
                });
        }

        private IEnumerable<SelectListItem> GetCategorySelectListForFilter()
        {
            var categories = _unitOfWork.Category.GetAll(filter: c => !c.IsDeleted && c.IsActive)
                .Select(c => new SelectListItem
                {
                    Text = c.CatName,
                    Value = c.Id.ToString()
                }).ToList();

            // Add "All Categories" option at the beginning
            categories.Insert(0, new SelectListItem { Text = "All Categories", Value = "" });
            return categories;
        }
        */

        // Development reset route
        [Route("reset")]
        public async Task<IActionResult> ResetDatabase()
        {
            try
            {
                // Delete all products first (due to foreign key constraint)
                var products = (await _unitOfWork.Product.GetAllAsync()).ToList();
                foreach (var product in products)
                {
                    _unitOfWork.Product.Remove(product);
                }

                // Delete all categories
                var categories = (await _unitOfWork.Category.GetAllAsync()).ToList();
                foreach (var category in categories)
                {
                    _unitOfWork.Category.Remove(category);
                }

                await _unitOfWork.SaveAsync();

                // Reset identity columns
                await _unitOfWork.ResetIdentityColumnsAsync();

                return Json(new { success = true, message = "Database has been reset successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error resetting database: {ex.Message}" });
            }
        }
    }
}
