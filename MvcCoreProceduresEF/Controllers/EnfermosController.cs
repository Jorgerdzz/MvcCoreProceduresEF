using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;
using System.Threading.Tasks;

namespace MvcCoreProceduresEF.Controllers
{
    public class EnfermosController : Controller
    {
        private RepositoryEnfermos repo;
        public EnfermosController(RepositoryEnfermos repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<Enfermo> enfermos = await this.repo.GetEnfermosAsync();
            return View(enfermos);
        }

        public async Task<IActionResult> Details(string id)
        {
            Enfermo enfermo = await this.repo.FindEnfermoAsync(id);
            return View(enfermo);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.repo.DeleteEnfermoRawAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Enfermo enf)
        {
            await this.repo.CreateEnfermoAsync(enf.Apellido, enf.Direccion, enf.FechaNacimiento, enf.Genero, enf.Nss);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string id)
        {
            Enfermo enf = await this.repo.FindEnfermoAsync(id);
            return View(enf);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Enfermo enf)
        {
            await this.repo.UpdateEnfermo(enf.Inscripcion, enf.Apellido, enf.Direccion, enf.FechaNacimiento, enf.Genero, enf.Nss);
            return RedirectToAction("Index");
        }

    }
}
