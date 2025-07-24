using Microsoft.AspNetCore.Mvc;

namespace BookShop.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public new IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return RedirectToAction("NotFound");
                default:
                    return View("Error");
            }
        }
    }
}
