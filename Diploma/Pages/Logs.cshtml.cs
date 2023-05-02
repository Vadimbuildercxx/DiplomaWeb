using Diploma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

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
    }

    public class LogsModel : PageModel
    {
        private DBContext _dbContext;

        public LogsModel(DBContext dBContext)
        {
            _dbContext = dBContext;
            //PersonViewModel = new PersonViewModel();
        }
        public void OnGet()
        {
        }
        public PartialViewResult OnGetAreaListPartial()
        {
            List<LogDTO> logDTOs = new();
            List<Log> logs = _dbContext.Logs.ToList();
            foreach (var log in logs) 
            {
                logDTOs.Add(new LogDTO
                {
                    Id = log.Id,
                    MessageType = log.MessageType,
                    Text = log.Text,
                    DateTime = log.DateTime,
                    CameraId = log.CameraId,
                    CameraName = _dbContext.Cameras.Where(x=> x.Id == log.CameraId).Select(x=>x.Name).FirstOrDefault()
                });
            }

            return new PartialViewResult
            {
                ViewName = "_LogListPartial",
                ViewData = new ViewDataDictionary<List<LogDTO>>(ViewData, logDTOs)
            };
        }
    }
}
