using BookShop.DataAccess.Repository.IRepository;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int PageSize = 10;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int page = 1, string search = null)
        {
            var categories = await _unitOfWork.Category.GetAllAsync(
                filter: c => !c.IsDeleted,
                orderBy: q => q.OrderBy(c => c.CatOrder).ThenByDescending(c => c.CatName)
            );

            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(c => 
                    c.CatName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var categoriesList = categories.ToList();
            var totalCategories = categoriesList.Count;
            var paginatedCategories = categoriesList
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCategories / PageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;
            ViewBag.SearchTerm = search;

            return View(paginatedCategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (category == null || category.IsDeleted)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _unitOfWork.Category.GetAsync(c => c.Id == id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }
                    
                    existingCategory.CatName = category.CatName;
                    existingCategory.CatOrder = category.CatOrder;
                    existingCategory.IsActive = category.IsActive;
                    
                    await _unitOfWork.SaveAsync();
                }
                catch (Exception)
                {
                    if (!await CategoryExistsAsync(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int page = 1)
        {
            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (category != null)
            {
                category.IsDeleted = true;
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, int page = 1)
        {
            var category = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (category != null && !category.IsDeleted)
            {
                category.IsActive = !category.IsActive;
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }

        private async Task<bool> CategoryExistsAsync(int id)
        {
            return await _unitOfWork.Category.GetAsync(c => c.Id == id && !c.IsDeleted) != null;
        }

        // Synchronous method commented out to force async usage
        /*
        private bool CategoryExists(int id)
        {
            return _unitOfWork.Category.Get(c => c.Id == id && !c.IsDeleted) != null;
        }
        */

        // Static pages
        [Route("Home")]
        [Route("")]
        public IActionResult Home()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View("~/Views/Home/Privacy.cshtml");
        }
    }
}
