using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using ExcelReportUpload.IRepositories;
using ExcelReportUpload.Models;

namespace ExcelReportUpload.Repositories
{
	public class LoginRepository: ILoginRepository
    {
        private readonly IDbConnection _connection;
        public LoginRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        //public LoginResponse LoginCheck(User User)
        //{
        //    try
        //    {
        //        DynamicParameters parameters = new DynamicParameters();
        //        parameters.Add("UserName", User.UserName);
        //        parameters.Add("Password", User.Password);
        //        parameters.Add("OutPut")
        //    }
        //    catch(Exception e)
        //    {
        //        throw e;
        //    }
        //}
        public async Task<LoginResponse> LoginCheckAsync(User user)
        {
            try
            {
                using (var connection = _connection)
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", user.UserName, DbType.String);
                    parameters.Add("@Password", user.Password, DbType.String);
                    parameters.Add("@OutPut", dbType: DbType.Int16, direction: ParameterDirection.Output);
                    parameters.Add("@OutPutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync("LOGIN_CHECK", parameters, commandType: CommandType.StoredProcedure);

                    return new LoginResponse
                    {
                        Status = parameters.Get<int>("@OutPut"),
                        StatusMessage = parameters.Get<string>("@OutPutMessage")
                    };
                }
            }
            catch (Exception e)
            {
                // Handle or log the exception
                throw; // Re-throwing the exception to propagate it up the call stack
            }
        }


        public LoginResponse LoginCheck1(User user)
        {
            using (var connection = _connection)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserName", user.UserName, DbType.String);
                parameters.Add("@Password", user.Password, DbType.String);
                parameters.Add("@OutPut", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@OutPutMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

                connection.Execute("LOGIN_CHECK", parameters, commandType: CommandType.StoredProcedure);

                return new LoginResponse
                {
                    Status = parameters.Get<int>("@OutPut"),
                    StatusMessage = parameters.Get<string>("@OutPutMessage")
                };
            }
                
        }
    }
}

