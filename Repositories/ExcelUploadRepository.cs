using System;
using System.Data;
using System.Data.Common;
using System.Security.Principal;
using Dapper;
using ExcelReportUpload.IRepositories;

namespace ExcelReportUpload.Repositories
{
	public class ExcelUploadRepository: IExcelUploadRepository
    {
        private readonly IDbConnection _connection;
        public ExcelUploadRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        public string ExcelUpload()
        {
            string query = "SELECT * from LABS for JSON PATH";
            string data =  _connection.QueryFirstOrDefault<string>(query);
            return data;
        }
    }
}

