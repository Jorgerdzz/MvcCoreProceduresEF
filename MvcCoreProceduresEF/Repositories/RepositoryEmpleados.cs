using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Diagnostics.Metrics;

#region VISTA EMPLEADOS
//alter view V_EMPLEADOS_DEPARTAMENTOS
//as
//	select cast(isnull(ROW_NUMBER() over (order by EMP.APELLIDO), 0) as int) as ID,
//    EMP.APELLIDO, EMP.OFICIO, EMP.SALARIO,
//    DEPT.DNOMBRE as DEPARTAMENTO,
//    DEPT.LOC as LOCALIDAD
//	from EMP
//	inner join DEPT
//	on EMP.DEPT_NO = DEPT.DEPT_NO
//go
#endregion

namespace MvcCoreProceduresEF.Repositories
{
    public class RepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<VistaEmpleado>> GetVistaEmpleadosAsync()
        {
            var consulta = from datos in this.context.VistaEmpleados
                           select datos;
            return await consulta.ToListAsync();
        }

    }
}
