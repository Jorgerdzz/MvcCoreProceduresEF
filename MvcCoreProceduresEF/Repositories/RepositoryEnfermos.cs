using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

    //alter procedure SP_INSERT_ENFERMO
    //(@apellido nvarchar(50), @direccion nvarchar(50), @fechaNacimiento DateTime, @genero nvarchar(50), @nss nvarchar(50))
    //as
    //    declare @inscripcion int
    //    select @inscripcion = CAST(MAX(INSCRIPCION) AS INT) from ENFERMO;
    //    set @inscripcion = @inscripcion + 1
    //    insert into ENFERMO values(@inscripcion, @apellido, @direccion, @fechaNacimiento, @genero, @nss)
    //go

    //create procedure SP_UPDATE_ENFERMO
    //(@inscripcion nvarchar(50), @apellido nvarchar(50), @direccion nvarchar(50), @fechaNacimiento DateTime, @genero nvarchar(50), @nss nvarchar(50))
    //as
    //    update ENFERMO set APELLIDO = @apellido, DIRECCION = @direccion, FECHA_NAC = @fechaNacimiento,
    //    S = @genero, NSS = @nss where INSCRIPCION = @inscripcion
    //go
    #endregion
    public class RepositoryEnfermos
    {
        private HospitalContext context;

        public RepositoryEnfermos(HospitalContext context)
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

        public async Task<Enfermo> FindEnfermoAsync(string inscripcion)
        {
            //PARA LLAMAR A UN PROCEDIMIENTO QUE CONTIENE PARAMETROS
            //LA LLAMADA SE REALIZA MEDIANTE EL NOMBRE DEL PROCEDURE
            //Y CADA PARAMETRO A CONTINUAION EN LA DECLARACION DEL SQL:
            //SP_PROCEDURE @PAM1, @PAM2
            string sql = "SP_BUSCAR_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            //SI LOS DATOS QUE DEVUELVE EL PROCEDURE ESTAN MAPEADOS CON UN MODEL
            //PODEMOS UTILIZAR EL METODO FromSqlRaw PARA RECUPERAR DIRECTAMENTE EL MODEL/S
            //NO PODEMOS CONSULTAR Y EXTRAER A LA VEZ CON LINQ, SE DEBE REALIZAR
            //SIEMPRE EN DOS PASOS
            //var consulta = this.context.Enfermos.FromSqlRaw(sql, pamIns).ToList();
            var consulta = this.context.Enfermos.FromSqlRaw(sql, pamIns);
            Enfermo enfermo = await consulta.AsAsyncEnumerable().FirstOrDefaultAsync();
            //Enfermo enfermo = consulta.FirstOrDefault();
            return enfermo;
        }

        public async Task DeleteEnfermoAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(pamIns);
                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }
        
        public async Task DeleteEnfermoRawAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamIns);
        }

        public async Task CreateEnfermoAsync(string apellido, string direccion, DateTime fechaNac, string genero, string nss)
        {
            string sql = "SP_INSERT_ENFERMO @apellido, @direccion, @fechaNacimiento, @genero, @nss";
            SqlParameter pamApe = new SqlParameter("@apellido", apellido);
            SqlParameter pamDir = new SqlParameter("@direccion", direccion);
            SqlParameter pamFech = new SqlParameter("@fechaNacimiento", fechaNac);
            SqlParameter pamGen = new SqlParameter("@genero", genero);
            SqlParameter pamNs = new SqlParameter("@nss", nss);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamApe, pamDir, pamFech, pamGen, pamNs);
        }

        public async Task UpdateEnfermo(string inscripcion, string apellido, string direccion, DateTime fechaNac, string genero, string nss)
        {
            string sql = "SP_UPDATE_ENFERMO @inscripcion, @apellido, @direccion, @fechaNacimiento, @genero, @nss";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            SqlParameter pamApe = new SqlParameter("@apellido", apellido);
            SqlParameter pamDir = new SqlParameter("@direccion", direccion);
            SqlParameter pamFech = new SqlParameter("@fechaNacimiento", fechaNac);
            SqlParameter pamGen = new SqlParameter("@genero", genero);
            SqlParameter pamNs = new SqlParameter("@nss", nss);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamIns, pamApe, pamDir, pamFech, pamGen, pamNs);
        }

    }
}
