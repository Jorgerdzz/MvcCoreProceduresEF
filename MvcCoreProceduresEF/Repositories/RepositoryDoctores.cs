using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MvcCoreProceduresEF.Repositories
{
    #region STORED PROCEDURES

    //create procedure SP_ESPECIALIDADES_DOCTOR
    //as
    //    select distinct ESPECIALIDAD from DOCTOR;
    //go

    //create procedure SP_INCREMENTAR_SALARIOS_POR_ESPECIALIDAD
    //(@especialidad nvarchar(50), @incremento int)
    //as
    //    update DOCTOR set SALARIO = SALARIO + @incremento where ESPECIALIDAD = @especialidad
    //go

    //create procedure SP_DOCTORES_ESPECIALIDAD
    //(@especialidad nvarchar(50))
    //as
    //    select* from DOCTOR where ESPECIALIDAD = @especialidad;
    //go

    #endregion
    public class RepositoryDoctores
    {
        private EnfermosContext context;

        public RepositoryDoctores(EnfermosContext context)
        {
            this.context = context;
        }

        public async Task<List<Doctor>> GetDoctoresPorEspecialidadAsync(string especialidad)
        {
            string sql = "SP_DOCTORES_ESPECIALIDAD @especialidad";
            SqlParameter pamEsp = new SqlParameter("@especialidad", especialidad);
            var doctores = await this.context.Doctores.FromSqlRaw(sql, pamEsp).ToListAsync();
            return doctores;
        }

        public async Task<List<string>> GetEspecialidadesAsync()
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ESPECIALIDADES_DOCTOR";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<string> especialidades = new List<string>();
                while(await reader.ReadAsync())
                {
                    string especialidad = reader["ESPECIALIDAD"].ToString();
                    especialidades.Add(especialidad);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return especialidades;
            }
        }

        public async Task UpdateDoctorSalarioRawAsync(string especialidad, int incremento)
        {
            string sql = "SP_INCREMENTAR_SALARIOS_POR_ESPECIALIDAD @especialidad, @incremento";
            SqlParameter pamEsp = new SqlParameter("@especialidad", especialidad);
            SqlParameter pamIn = new SqlParameter("@incremento", incremento);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamEsp, pamIn);
        }

        public async Task UpdateDoctorSalarioSinProcedureAsync(string especialidad, int incremento)
        {
            List<Doctor> doctores = await GetDoctoresPorEspecialidadAsync(especialidad);
            foreach(Doctor doc in doctores)
            {
                doc.Salario = doc.Salario + incremento;
            }
            await this.context.SaveChangesAsync();
            //using(DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            //{
            //    string sql = "SP_INCREMENTAR_SALARIOS_POR_ESPECIALIDAD";
            //    SqlParameter pamEsp = new SqlParameter("@especialidad", especialidad);
            //    SqlParameter pamIn = new SqlParameter("@incremento", incremento);
            //    com.Parameters.Add(pamEsp);
            //    com.Parameters.Add(pamIn);
            //    com.CommandType = System.Data.CommandType.StoredProcedure;
            //    com.CommandText = sql;
            //    await com.Connection.OpenAsync();
            //    await com.ExecuteNonQueryAsync();
            //    await com.Connection.CloseAsync();
            //    com.Parameters.Clear();
            //}
            
        }

    }
}
