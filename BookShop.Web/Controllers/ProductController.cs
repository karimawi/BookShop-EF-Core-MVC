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

        public IActionResult Index(int page = 1, int? categoryFilter = null, string search = null)
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category");

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
            ViewBag.CategoryList = GetCategorySelectListForFilter();

            return View(paginatedProducts);
        }

        public IActionResult Create()
        {
            ViewBag.CategoryList = GetCategorySelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Price < 1 || product.Price > 1000)
                {
                    ModelState.AddModelError("Price", "Price must be between 1 and 1000");
                    ViewBag.CategoryList = GetCategorySelectList();
                    return View(product);
                }

                _unitOfWork.Product.Add(product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryList = GetCategorySelectList();
            return View(product);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            
            ViewBag.CategoryList = GetCategorySelectList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
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
                    ViewBag.CategoryList = GetCategorySelectList();
                    return View(product);
                }

                var existingProduct = _unitOfWork.Product.Get(p => p.Id == id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.Title = product.Title;
                existingProduct.Description = product.Description;
                existingProduct.Author = product.Author;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.CategoryList = GetCategorySelectList();
            return View(product);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _unitOfWork.Product.Get(p => p.Id == id, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Product.Get(p => p.Id == id);
            if (product != null)
            {
                _unitOfWork.Product.Remove(product);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

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

        // Development reset route
        [Route("reset")]
        public IActionResult ResetDatabase()
        {
            try
            {
                // Delete all products first (due to foreign key constraint)
                var products = _unitOfWork.Product.GetAll().ToList();
                foreach (var product in products)
                {
                    _unitOfWork.Product.Remove(product);
                }

                // Delete all categories
                var categories = _unitOfWork.Category.GetAll().ToList();
                foreach (var category in categories)
                {
                    _unitOfWork.Category.Remove(category);
                }

                _unitOfWork.Save();

                // Reset identity columns
                _unitOfWork.ResetIdentityColumns();

                return Json(new { success = true, message = "Database has been reset successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error resetting database: {ex.Message}" });
            }
        }
    }
}
