using Microsoft.AspNetCore.Mvc;

namespace MagiRogue.WebClient.Server.Controllers
{
    public class ResearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
