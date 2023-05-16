using Diploma.Controllers;
using Diploma.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Diploma.Pages
{
    public class DashboardModel : PageModel
    {
        public List<String> colors;
        public string title;

        public string description;

        public List<int?> ViolationDates;
        public List<int?> ViolationCountByDate;

        public int NumberOfDetectionsByWeek {
            get
            {
                return _dbContext.Logs.Where(x=>x.DateTime >  DateTime.Now.AddDays(-7)).Count();
            }
        }

        public int NumberOfWorkers
        {
            get
            {
                return _dbContext.Persons.Count();
            }
        }

        public int NumberOfCams
        {
            get
            {
                return _dbContext.Cameras.Count();
            }
        }

        public float PercentOfDetectedViolations
        {
            get
            {
                float numOfViolations = _dbContext.Logs.Where(
                    x => x.MessageType == Message.Warning || x.MessageType == Message.Alert).Count();
                return numOfViolations / _dbContext.Logs.Count();
            }
        }


        //public DateTime LRDateTimeEnd = DateTime.Now;

        public DateTime LRDateTimeStart = DateTime.Now;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "yyyy-MM-dd")]
        public DateTime YearDOB { get; set; } = DateTime.Now;


        private DBContext _dbContext;

        public DashboardModel(DBContext dBContext)
        {
            this.colors = new List<String>() { "Red", "Blue", "Yellow" };
            this.title = "My First Dataset";
            _dbContext = dBContext;
        }

        public async Task<IActionResult> OnGetImagesCountCount()
        {
            List<int> ids = _dbContext.Persons.Select(x => x.Id).ToList();
            List<string> labels = _dbContext.Persons.Select(x => x.LastName + " " + x.LastName[0] + ". " + (x.Patronymic == null? "": x.Patronymic[0] +".")).ToList();

            string text = System.IO.File.ReadAllText(@"Files/startup_config.json");
            var parsedObject = JObject.Parse(text);
            string path = parsedObject["PersonsFacesPath"].ToString();
            List<int> imageCount = new List<int>();
            foreach (var id in ids)
            {
                string dir = Path.Combine(path, "person_" + id.ToString());
                if (Directory.Exists(dir))
                {
                    int fCount = Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Length;
                    imageCount.Add(fCount);
                }
                else
                {
                    imageCount.Add(0);
                }
               
            }
            

            return new JsonResult(new { labels = labels, data = imageCount});
        }

        public async Task<IActionResult> OnGetDetectedObjectsCount()
        {
            DateTime fromData = DateTime.Now.AddDays(-6);

            var dates = new List<DateTime>();
            for (var dt = fromData ; dt <= DateTime.Now; dt = dt.AddDays(1))
            {
                dates.Add(new DateTime(dt.Year, dt.Month, dt.Day));
            }

            

            var resultsThisWeek = from l in _dbContext.Logs
                          where  l.DateTime > fromData && l.DateTime < DateTime.Now && l.PPEitem != null
                          group l by new
                          {
                              l.PPEitem,
                              date = new DateTime(l.DateTime.Year, l.DateTime.Month, l.DateTime.Day),
                          } into g
                          select new { g.Key.date, g.Key.PPEitem, count = g.Sum(e => e.ObjCount) };


            DateTime fromDataLastWeek = fromData.AddDays(-7);

            var datesL = new List<DateTime>();
            for (var dt = fromDataLastWeek; dt < fromData; dt = dt.AddDays(1))
            {
                datesL.Add(new DateTime(dt.Year, dt.Month, dt.Day));
            }

            var resultsLastWeek = from l in _dbContext.Logs
                        where fromData > l.DateTime && l.DateTime > fromDataLastWeek && l.PPEitem != null
                                  group l by new
                        {
                            l.PPEitem,
                            date = new DateTime(l.DateTime.Year, l.DateTime.Month, l.DateTime.Day),
                        } into g
                        select new {g.Key.date, g.Key.PPEitem, count = g.Sum(e => e.ObjCount)};


            var uObjects = _dbContext.Logs.Where(x => x.PPEitem != null).Select(x=>x.PPEitem).Distinct().ToList();

            List<string> labelDates = new();
            List<List<int?>> thisWeekCount = new();
            List<List<int?>> lastWeekCount = new();
            foreach (string ppe in uObjects)
            {
                List<int?> ppeItems = new();
                foreach (DateTime date in dates)
                {
                    bool flag = false;
                    foreach (var item in resultsThisWeek)
                    {
                        if (item.date == date && item.PPEitem == ppe)
                        {
                            ppeItems.Add(item.count);
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        ppeItems.Add(0);

                    }
                }
                thisWeekCount.Add(ppeItems);
            }

            foreach (string ppe in uObjects)
            {
                List<int?> ppeItems = new();
                foreach (DateTime date in datesL)
                {

                    bool flag = false;
                    foreach (var item in resultsLastWeek)
                    {
                        if (item.date == date && item.PPEitem == ppe)
                        {
                            ppeItems.Add(item.count);
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        ppeItems.Add(0);

                    }

                }
                lastWeekCount.Add(ppeItems);
            }

            foreach (DateTime date in datesL)
            {
                labelDates.Add(date.ToString("dddd", new CultureInfo("ru-RU")));
            }

            return new JsonResult(
                new 
                {
                    ppeLabels = uObjects,
                    labels = labelDates, 
                    countThisWeek = thisWeekCount,
                    countLastWeek = lastWeekCount,

                });
        }

        public async Task<IActionResult> OnGetViolationsByPerson(string type)
        {
            DateTime fromData;
            if (type == "week")
            {
                fromData = DateTime.Now.AddDays(-7);
            }
            else
            {
                fromData = DateTime.Now.AddMonths(-1);
            }
            var results = from l in _dbContext.Logs
                          where l.DateTime < DateTime.Now && l.DateTime > fromData && l.PersonId != 0
                          group l by  l.PersonId into g
                          select new { personId = g.Key, count = g.Count() };

            Random rnd = new Random();
            var areasAndCounts = new
            {
                Label = results.Select(g => _dbContext.Persons.Where(x=>x.Id == g.personId).Select(
                    x=>x.LastName + " " + x.FirstName).FirstOrDefault()).ToArray(),
                Total = results.Select(g => g.count).ToArray(),
                Color = results.Select(g => $"rgb({rnd.Next(0, 256)}, {rnd.Next(0, 256)}, {rnd.Next(0, 256)})").ToArray(),
            };
            
            return new JsonResult(new { labels = areasAndCounts.Label, data = areasAndCounts.Total, color = areasAndCounts.Color });
        }


        public async Task<IActionResult> OnGetViolationByAreas(string type)
        {
            DateTime fromData;
            if (type == "week")
            {
                fromData = DateTime.Now.AddDays(-7);
            }
            else
            {
                fromData = DateTime.Now.AddMonths(-1);
            }
            var results = from l in _dbContext.Logs
                          where l.DateTime < DateTime.Now && l.DateTime > fromData
                          group l by new
                          {
                              areaId = _dbContext.Cameras.Where(x=> x.Id == l.CameraId).Select(x=> x.AreaId).FirstOrDefault()
                          } into g
                          select new { areaId = g.Key, count = g.Sum(e => e.ObjCount) };

           
            var areasAndCounts = new
            {
                Label = results.Select(g => g.areaId).ToArray(),
                Total = results.Select(g => g.count).ToArray(),
            };
            List<string> labels = new List<string>();
            List<string> colors = new List<string>();
            foreach (var item in areasAndCounts.Label)
            {
                if (item.areaId!=null)
                {
                    labels.Add(_dbContext.Areas.Where(x => x.Id == item.areaId).Select(x => x.Name).FirstOrDefault());
                }
                else
                {
                    labels.Add("Не указана");
                }
            }

            return new JsonResult(new { labels = labels, data = areasAndCounts.Total });
        }

        public async Task<IActionResult> OnGetViolationCountByDate(string type , DateTime fromDate)
        {
            if (type == "minute")
            {
                DateTime to = fromDate.AddMinutes(1);
                var results = from p in _dbContext.Logs
                              where p.DateTime > fromDate && p.DateTime <= to
                              group p by new
                              {
                                  p.DateTime.Year,
                                  p.DateTime.Month,
                                  p.DateTime.Day,
                                  p.DateTime.Hour,
                                  p.DateTime.Minute,
                                  p.DateTime.Second
                              } into g
                              select new { dt = g.Key, count = g.Sum(e => e.ObjCount) };

                var orders = new
                {
                    Time = results.Select(g => new DateTime(
                                  g.dt.Year, g.dt.Month,
                                  g.dt.Day, g.dt.Hour, g.dt.Minute, g.dt.Second)).ToArray(),
                    Total = results.Select(g => g.count).ToArray(),
                };
                var zipped = orders.Time.Zip(orders.Total);

                var dates = new List<string>();
                var values = new List<int?>();
                for (var dt = fromDate; dt <= to; dt = dt.AddSeconds(1))
                {
                    values.Add(zipped.Where(x => x.First == dt).Select(x => x.Second).FirstOrDefault(0));
                    dates.Add(dt.ToString("T"));
                }

                return new JsonResult(new { labels = dates, data = values });
            }
            else if (type == "hour")
            {
                DateTime to = fromDate.AddHours(1);
                var results = from p in _dbContext.Logs
                              where p.DateTime > fromDate && p.DateTime <= to
                              group p by new
                              {
                                  p.DateTime.Year,
                                  p.DateTime.Month,
                                  p.DateTime.Day,
                                  p.DateTime.Hour,
                                  p.DateTime.Minute
                              } into g
                              select new { dt = g.Key, count = g.Sum(e => e.ObjCount) };
                var orders = new
                {
                    Time = results.Select(g => new DateTime(
                                  g.dt.Year, g.dt.Month,
                                  g.dt.Day, g.dt.Hour, 
                                  g.dt.Minute, 0)).ToArray(),
                    Total = results.Select(g => g.count).ToArray(),
                };

                var zipped = orders.Time.Zip(orders.Total);

                var dates = new List<string>();
                var values = new List<int?>();
                for (var dt = fromDate; dt <= to; dt = dt.AddMinutes(1))
                {
                    values.Add(zipped.Where(x => x.First == dt).Select(x => x.Second).FirstOrDefault(0));
                    dates.Add(dt.ToString("t"));
                }

                return new JsonResult(new { labels = dates, data = values });
            }
            else if (type == "day")
            {
                DateTime to = fromDate.AddDays(1);
                var results = from p in _dbContext.Logs
                              where p.DateTime > fromDate && p.DateTime <= to
                              group p by new
                              {
                                  p.DateTime.Year,
                                  p.DateTime.Month,
                                  p.DateTime.Day,
                                  p.DateTime.Hour
                              } into g
                              select new { dt = g.Key, count = g.Sum(e => e.ObjCount) };
                var orders = new
                {
                    Time = results.Select(g => new DateTime(
                                  g.dt.Year, g.dt.Month,
                                  g.dt.Day, g.dt.Hour,
                                  0, 0)).ToArray(),
                    Total = results.Select(g => g.count).ToArray(),
                };

                var zipped = orders.Time.Zip(orders.Total);

                var dates = new List<string>();
                var values = new List<int?>();
                for (var dt = fromDate; dt <= to; dt = dt.AddHours(1))
                {
                    values.Add(zipped.Where(x => x.First == dt).Select(x => x.Second).FirstOrDefault(0));
                    dates.Add(dt.ToString("HH:mm"));
                }

                return new JsonResult(new { labels = dates, data = values });
            }
            else if (type == "month")
            {
                DateTime to = fromDate.AddMonths(1);
                var results = from p in _dbContext.Logs
                              where p.DateTime > fromDate && p.DateTime <= to
                              group p by new
                              {
                                  p.DateTime.Year,
                                  p.DateTime.Month,
                                  p.DateTime.Day
                              } into g
                              select new { dt = g.Key, count = g.Sum(e => e.ObjCount) };
                var orders = new
                {
                    Time = results.Select(g => new DateTime(
                                  g.dt.Year, g.dt.Month,
                                  g.dt.Day)).ToArray(),
                    Total = results.Select(g => g.count).ToArray(),
                };

                var zipped = orders.Time.Zip(orders.Total);

                var dates = new List<string>();
                var values = new List<int?>();
                for (var dt = fromDate; dt <= to; dt = dt.AddDays(1))
                {
                    values.Add(zipped.Where(x => x.First == dt).Select(x => x.Second).FirstOrDefault(0));
                    dates.Add(dt.ToString("D"));
                }

                return new JsonResult(new { labels = dates, data = values });
            }
            //else if (type == "year")
            //{
            //    DateTime to = fromDate.AddYears(1);
            //    var results = from p in _dbContext.Logs
            //                  where p.DateTime > fromDate && p.DateTime <= to
            //                  group p by new
            //                  {
            //                      p.DateTime.Year,
            //                      p.DateTime.Month
            //                  } into g
            //                  select new { dt = g.Key, count = g.Sum(e => e.ObjCount) };
            //    var orders = new
            //    {
            //        Time = results.Select(g => new DateTime(
            //                      g.dt.Year, g.dt.Month, 0)).ToArray(),
            //        Total = results.Select(g => g.count).ToArray(),
            //    };

            //    var zipped = orders.Time.Zip(orders.Total);

            //    var dates = new List<DateTime>();
            //    var values = new List<int?>();
            //    for (var dt = fromDate; dt <= to; dt = dt.AddMonths(1))
            //    {
            //        values.Add(zipped.Where(x => x.First == dt).Select(x => x.Second).FirstOrDefault(0));
            //        dates.Add(dt);
            //    }

            //    return new JsonResult(new { labels = dates, data = values });
            //}
            else
            {
                throw new Exception("Wrong parameters type");
            }




        }
    }
}
