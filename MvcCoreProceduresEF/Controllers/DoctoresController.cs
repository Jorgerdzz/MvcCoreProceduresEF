using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;

namespace MvcCoreProceduresEF.Controllers
{
    public class DoctoresController : Controller
    {
        private RepositoryDoctores repo;

        public DoctoresController(RepositoryDoctores repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ESPECIALIDADES"] = await this.repo.GetEspecialidadesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string especialidad, int incremento, string accion)
        {
            ViewData["ESPECIALIDADES"] = await this.repo.GetEspecialidadesAsync();
            if(accion == "raw")
            {
                await this.repo.UpdateDoctorSalarioRawAsync(especialidad, incremento);
            }
            else
            {
                await this.repo.UpdateDoctorSalarioSinProcedureAsync(especialidad, incremento);
            }
            List<Doctor> doctores = await this.repo.GetDoctoresPorEspecialidadAsync(especialidad);
            return View(doctores);
        }

    }
}
