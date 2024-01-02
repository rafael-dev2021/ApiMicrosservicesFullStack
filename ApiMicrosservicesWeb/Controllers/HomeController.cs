using ApiMicrosservicesWeb.Models;
using ApiMicrosservicesWeb.Models.MicrosservicesProduct;
using ApiMicrosservicesWeb.Services.MicrosservicesProduct.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ApiMicrosservicesWeb.Controllers
{
    public class HomeController(ILogger<HomeController> logger,
            IProductService productService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IProductService _productService = productService;


        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProducts(string.Empty);

            if (products is null)
            {
                return View("Error");
            }

            return View(products);
        }

        [HttpGet]
        public async Task<ActionResult<ProductViewModel>> ProductDetails(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var product = await _productService.GetByProductIdAsync(id, token);

            if (product is null)
                return View("Error");

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
