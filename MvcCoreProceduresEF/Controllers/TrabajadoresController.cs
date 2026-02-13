using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;
using System.Threading.Tasks;

namespace MvcCoreProceduresEF.Controllers
{
    public class TrabajadoresController : Controller
    {
        private RepositoryEmpleados repo;

        public TrabajadoresController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["OFICIOS"] = await this.repo.GetOficiosAsync();
            TrabajadoresModel model = await this.repo.GetTrabajadoresModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string oficio)
        {
            ViewData["OFICIOS"] = await this.repo.GetOficiosAsync();
            TrabajadoresModel model = await this.repo.GetTrabajadoresModelOficioAsync(oficio);
            return View(model);
        }
    }
}
