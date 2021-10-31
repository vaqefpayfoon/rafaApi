using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using API.Models;
using System.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SamanYazdController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SamanYazdController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            string query = @"SELECT [EMP_NO]
                            ,[PERS_NO]
                            ,[NAME]
                            ,[FAMILY]
                            ,[EMP_DATE]
                            ,[EMP_TYPE]
                            ,[SYS_ACTIVE]
                            ,[GRADE_NO]
                            ,[PersonelType]
                            ,[SEX]
                            ,[IsCut]
                            ,[CUT_REASON]
                            ,[END_DATE]
                        FROM [pwkara].[dbo].[EMPLOYEE]";
            string connectionString = _configuration.GetConnectionString("SamanYazd");
            SqlConnection cnn = new SqlConnection(connectionString);
            if (cnn.State == ConnectionState.Closed)
                await cnn.OpenAsync();
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = query;
            SqlDataReader reader = await command.ExecuteReaderAsync();
            List<Employee> lstEmployee = new List<Employee>();
            while (await reader.ReadAsync())
            {
                Employee employee = new Employee();
                employee.EMP_NO = reader["EMP_NO"].ToString();
                employee.CUT_REASON = reader["CUT_REASON"].ToString();
                employee.EMP_DATE = reader["EMP_DATE"].ToString();
                employee.EMP_TYPE = reader["EMP_TYPE"].ToString();
                employee.END_DATE = reader["END_DATE"].ToString();
                employee.FAMILY = reader["FAMILY"].ToString();
                employee.GRADE_NO = reader["GRADE_NO"].ToString();
                employee.IsCut = reader["IsCut"].ToString();
                employee.NAME = reader["NAME"].ToString();
                employee.PERS_NO = reader["PERS_NO"].ToString();
                employee.PersonelType = reader["PersonelType"].ToString();
                employee.SEX = reader["SEX"].ToString();
                employee.SYS_ACTIVE = reader["SYS_ACTIVE"].ToString();
                lstEmployee.Add(employee);
            }

            cnn.Close();

            return Ok(new { lstEmployee });
        }
        [HttpPost]
        public async Task<IActionResult> GetClockInOut(Between between)
        {
            string query = $@"SELECT [STATUS],[EMP_NO],DATEADD(day, -2, CONVERT (datetime,DATE_)) As Date1, TIME_ As Time1,
                            [MODIFY],[Clock_No] FROM [pwkara].[dbo].[DataFile] Where (CONVERT (datetime,DATE_) BETWEEN '{between.FromDate}' and '{between.ToDate}')
                            Order By date1 desc";
            string connectionString = _configuration.GetConnectionString("SamanYazd");
            SqlConnection cnn = new SqlConnection(connectionString);
            if (cnn.State == ConnectionState.Closed)
                await cnn.OpenAsync();
            SqlCommand command = new SqlCommand();
            command.Connection = cnn;
            command.CommandText = query;
            SqlDataReader reader = await command.ExecuteReaderAsync();
            List<InOut> lstInout = new List<InOut>();
            while (await reader.ReadAsync())
            {
                InOut inout = new InOut();
                inout.STATUS = reader["STATUS"].ToString();
                inout.EMP_NO = reader["EMP_NO"].ToString();
                inout.Date1 = Convert.IsDBNull(reader["Date1"]) ? null : Convert.ToDateTime(reader["Date1"]);
                inout.Time1 = Convert.IsDBNull(reader["Time1"]) ? null : GetTime(reader["Time1"].ToString());
                inout.Time2 = reader["Time1"].ToString();
                inout.MODIFY = reader["MODIFY"].ToString();
                inout.Clock_No = reader["Clock_No"].ToString();
                lstInout.Add(inout);
            }

            cnn.Close();

            return Ok(new { lstInout });
        }
        private string GetTime(string Time)
        {
            if (Time.Length >= 3)
            {
                string splited;
                splited = Time.Substring(Time.Length - 2);
                Time = Time.Remove(Time.Length - 2);
                return Time + ":" + splited;
            }
            return Time;
        }
    }
}