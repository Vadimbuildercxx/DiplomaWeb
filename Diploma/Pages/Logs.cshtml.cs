using Diploma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Diploma.Pages
{
    public class LogDTO
    {
        public int Id { get; set; }
        public Message MessageType { get; set; }
        public string? Text { get; set; }
        public DateTime DateTime { get; set; }
        public int? CameraId { get; set; }
        public string CameraName { get; set; } = string.Empty;
        public int? ObjectId { get; set; }
        public string ObjectName { get; set; } = string.Empty;
        public string DetectionPath { get; set; } = string.Empty;
        public int? PersonId { get; set; }
        public float? PersonConf { get; set; }
    }

    public class LogsModel : PageModel
    {
        private DBContext _dbContext;
        private IWebHostEnvironment Environment;

        public LogsModel(DBContext dBContext, IWebHostEnvironment _environment)
        {
            _dbContext = dBContext;
            this.Environment = _environment;
            //PersonViewModel = new PersonViewModel();
        }
        public void OnGet()
        {
        }
        public PartialViewResult OnGetLogListPartial()
        {
            List<LogDTO> logDTOs = new();
            var logs = _dbContext.Logs.ToList();
            foreach (var log in logs) 
            {
                logDTOs.Add(new LogDTO
                {
                    Id = log.Id,
                    MessageType = log.MessageType,
                    Text = log.Text,
                    DateTime = log.DateTime,
                    CameraId = log.CameraId,
                    CameraName = _dbContext.Cameras.Where(x=> x.Id == log.CameraId).Select(x=>x.Name).FirstOrDefault(),
                    ObjectId = log.PPEId,
                    ObjectName = _dbContext.PPEs.Where(x => x.Id == log.PPEId).Select(x => x.Name).FirstOrDefault(),
                    DetectionPath = log.DetectionPath,
                    PersonId = log.PersonId,
                    PersonConf = log.PersonConf,
                });
            }

            return new PartialViewResult
            {
                ViewName = "_LogListPartial",
                ViewData = new ViewDataDictionary<List<LogDTO>>(ViewData, logDTOs)
            };
        }
        public FileContentResult OnGetDownloadFile(string filePath)
        {
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/jpeg", Path.GetFileName(filePath));
        }
        //public FileResult OnGetDownloadFile(string fileName)
        //{
        //    fileName = fileName.Replace("__", "\\");
        //    Console.WriteLine("-----------"+ this.Environment.WebRootPath + " " + fileName);
        //    //Build the File Path.
        //    string path = Path.Combine(this.Environment.WebRootPath, "C:\\Users\\Vadim\\source\\repos\\DiplomaAI\\detections") + fileName;

        //    //Read the File data into Byte Array.
        //    byte[] bytes = System.IO.File.ReadAllBytes(fileName);

        //    //Send the File to Download.
        //    return File(bytes, "application/octet-stream", "download.jpg");
        //}
    }
}
