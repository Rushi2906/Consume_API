using Microsoft.AspNetCore.Mvc;

namespace Fetch_API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
