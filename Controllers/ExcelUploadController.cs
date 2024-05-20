using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Linq;
using ExcelReportUpload.Models;
using ExcelReportUpload.IRepositories;
using System.Net;
using System.Net.Mail;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExcelReportUpload.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelUploadController : Controller
    {
        private readonly IExcelUploadRepository excelUpload;
        public ExcelUploadController(IExcelUploadRepository _excelRepo)
        {
            excelUpload = _excelRepo;
        }
        [HttpPost("ExcelUpload")]
        public IActionResult ExcelUpload(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                return BadRequest("No file uploaded");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(formFile.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return BadRequest("No worksheet found in the Excel file.");

                var headers = GetHeaders(worksheet);
                var data = GetData(worksheet, headers);
                excelUpload.ExcelUpload();
                return Ok(data);
            }
        }
        [HttpPost("ExcelUploadMultipleSheets")]
        public IActionResult ExcelUploadMultipleSheets(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                return BadRequest("No file uploaded");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(formFile.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets;
                List<Dictionary<string, object>>[] JSONArray = new List<Dictionary<string, object>>[worksheet.Count];
                for(int i=0;i< worksheet.Count; i++)
                {
                    var headers = GetHeaders(worksheet[i]);
                    var SheetInfo= GetData(worksheet[i], headers);
                    if(SheetInfo != null)
                    {
                        JSONArray[i] = SheetInfo;
                    }
                }
                return Ok(JSONArray);
            }
        }
        private List<string> GetHeaders(ExcelWorksheet worksheet)
        {
            var headers = new List<string>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var cellValue = worksheet.Cells[1, col].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(cellValue))
                    headers.Add(cellValue);
            }
            return headers;
        }

        private List<Dictionary<string, object>> GetData(ExcelWorksheet worksheet, List<string> headers)
        {
            var data = new List<Dictionary<string, object>>();
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var rowData = new Dictionary<string, object>();
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var header = headers[col - 1];
                    var cellValue = worksheet.Cells[row, col].Value;
                    rowData[header] = cellValue;
                }
                data.Add(rowData);
            }
            return data;
        }
        [HttpGet("GetDetails")]
        public IActionResult GetDetails()
        {
            var data = excelUpload.ExcelUpload();

            return Ok(data);
        }
        [HttpPost("SendMail")]
        public IActionResult SendMail()
        {
            SendEmail("maheshmahi1816@gmail.com", "Testing", "Testing the application");
            return Ok("");
        }
        //private void SendEmail(string toAddress, string subject, string body)
        //{
        //    // Set up the SMTP client
        //    using (SmtpClient smtpClient = new SmtpClient("localhost", 2500))
        //    {
        //        // Set the credentials (if needed)
        //        smtpClient.Credentials = new NetworkCredential("mahesh.ekambaram@talentpace.com", "MaheshYash@18");

        //        // Create a MailMessage object
        //        MailMessage mailMessage = new MailMessage();
        //        mailMessage.From = new MailAddress("mahesh.ekambaram@talentpace.com");
        //        mailMessage.To.Add(toAddress);
        //        mailMessage.Subject = subject;
        //        mailMessage.Body = body;

        //        // You can set additional properties like CC, BCC, etc., if needed

        //        // Send the email
        //        smtpClient.Send(mailMessage);
        //    }
        //}

        private void SendEmail(string toAddress, string subject, string body)
        {
            try
            {
                // Set up the SMTP client
                using (SmtpClient smtpClient = new SmtpClient("localhost", 2500))
                {
                    // Set the credentials (if needed)
                    smtpClient.Credentials = new NetworkCredential("mahesh.ekambaram@talentpace.com", "MaheshYash@18");

                    // Create a MailMessage object
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("mahesh.ekambaram@talentpace.com");
                    mailMessage.To.Add(toAddress);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    // You can set additional properties like CC, BCC, etc., if needed

                    // Send the email
                    smtpClient.Send(mailMessage);

                    Console.WriteLine("Email sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the email: " + ex.Message);
            }
        }


    }

}

