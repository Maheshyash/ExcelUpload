using System;
using ExcelReportUpload.Models;

namespace ExcelReportUpload.IRepositories
{
	public interface ILoginRepository
	{
        Task<LoginResponse> LoginCheckAsync(User user);
        LoginResponse LoginCheck1(User user);
    }
}

