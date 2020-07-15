using Microsoft.AspNetCore.Mvc;

namespace Notebook.WebClient.Controllers
{
    public class NotebookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
