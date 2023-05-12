using Diploma.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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


        //public DateTime LRDateTimeEnd = DateTime.Now;

        public DateTime LRDateTimeStart = DateTime.Now;


        private DBContext _dbContext;

        public DashboardModel(DBContext dBContext)
        {
            this.colors = new List<String>() { "Red", "Blue", "Yellow" };
            this.title = "My First Dataset";
            _dbContext = dBContext;




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
                //colors.Add()
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
            else if (type == "year")
            {
                DateTime to = fromDate.AddYears(1);
                var results = from p in _dbContext.Logs
                              where p.DateTime > fromDate && p.DateTime <= to
                              group p by new
                              {
                                  p.DateTime.Year,
                                  p.DateTime.Month
                              } into g
                              select new { dt = g.Key, count = g.Sum(e => e.ObjCount) };
                var orders = new
                {
                    Time = results.Select(g => new DateTime(
                                  g.dt.Year, g.dt.Month, 0)).ToArray(),
                    Total = results.Select(g => g.count).ToArray(),
                };

                var zipped = orders.Time.Zip(orders.Total);

                var dates = new List<DateTime>();
                var values = new List<int?>();
                for (var dt = fromDate; dt <= to; dt = dt.AddMonths(1))
                {
                    values.Add(zipped.Where(x => x.First == dt).Select(x => x.Second).FirstOrDefault(0));
                    dates.Add(dt);
                }

                return new JsonResult(new { labels = dates, data = values });
            }
            else
            {
                throw new Exception("Wrong parameters type");
            }




        }
    }
}
