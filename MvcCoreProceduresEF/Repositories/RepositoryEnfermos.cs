using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System;
using System.Data.Common;

namespace MvcCoreProceduresEF.Repositories
{
    #region STORED PROCEDURES
    //    create procedure SP_ALL_ENFERMOS
    //as
    //	select* from ENFERMO;
    //go

    //create procedure SP_BUSCAR_ENFERMO
    //(@inscripcion nvarchar(50))
    //as
    //	select* from ENFERMO where INSCRIPCION = @inscripcion
    //go

    //create procedure SP_DELETE_ENFERMO
    //(@inscripcion nvarchar(50))
    //as
    //	delete from ENFERMO where INSCRIPCION = @inscripcion
    //go
    #endregion
    public class RepositoryEnfermos
    {
        private EnfermosContext context;

        public RepositoryEnfermos(EnfermosContext context)
        {
            this.context = context;
        }

        public async Task<List<Enfermo>> GetEnfermosAsync()
        {
            //NECESITAMOS UN COMMAND, VAMOS A UTILIZAR UN using PARA TODO
            //EL COMMAND, EN SU CREACION, NECESITA DE UNA CADENA DE CONEXION (OBJETO)
            //EL OBJETO CONNECTION NOS LO OFRECE EF
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ALL_ENFERMOS";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                //ABRIMOS LA CONEXION A PARTIR DEL COMMAND
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Enfermo> enfermos = new List<Enfermo>();
                while (await reader.ReadAsync())
                {
                    Enfermo enf = new Enfermo
                    {
                        Inscripcion = reader["INSCRIPCION"].ToString(),
                        Apellido = reader["APELLIDO"].ToString(),
                        Direccion = reader["DIRECCION"].ToString(),
                        FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                        Genero = reader["S"].ToString(),
                        Nss = reader["NSS"].ToString(),
                    };
                    enfermos.Add(enf);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return enfermos;
            }
        }

    }
}
