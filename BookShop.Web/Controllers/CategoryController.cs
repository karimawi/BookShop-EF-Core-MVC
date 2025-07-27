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

        public IActionResult Index(int page = 1, string search = null)
        {
            var categories = _unitOfWork.Category.GetAll(
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
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null || category.IsDeleted)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = _unitOfWork.Category.Get(c => c.Id == id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }
                    
                    existingCategory.CatName = category.CatName;
                    existingCategory.CatOrder = category.CatOrder;
                    existingCategory.IsActive = category.IsActive;
                    
                    _unitOfWork.Save();
                }
                catch (Exception)
                {
                    if (!CategoryExists(category.Id))
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

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.Get(c => c.Id == id && !c.IsDeleted);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, int page = 1)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category != null)
            {
                category.IsDeleted = true;
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id, int page = 1)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category != null && !category.IsDeleted)
            {
                category.IsActive = !category.IsActive;
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index), new { page = page });
        }

        private bool CategoryExists(int id)
        {
            return _unitOfWork.Category.Get(c => c.Id == id && !c.IsDeleted) != null;
        }

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
