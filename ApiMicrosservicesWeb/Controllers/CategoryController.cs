using ApiMicrosservicesWeb.Models.MicrosservicesProduct;
using ApiMicrosservicesWeb.Services.MicrosservicesProduct.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiMicrosservicesWeb.Controllers
{
    public class CategoryController(ICategoryService categoryService) : Controller
    {
        private readonly ICategoryService _categoryService = categoryService;

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategories(await GetAccessToken());
            return View(categories);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(categoryViewModel, await GetAccessToken());
                return RedirectToAction(nameof(Index));
            }
            return View(categoryViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByCategoryIdAsync(id, await GetAccessToken());
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(categoryViewModel, await GetAccessToken());
                return RedirectToAction(nameof(Index));
            }
            return View(categoryViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByCategoryIdAsync(id, await GetAccessToken());
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id, await GetAccessToken());
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GetAccessToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
    }
}
