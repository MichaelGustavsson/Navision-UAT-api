using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace navision.api.Controllers
{
  public class Fallback: Controller
    {
        [AllowAnonymous]
        public IActionResult Index(){
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "index.html"), "text/html");
        }
    }
}