using Diploma.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;

namespace Diploma.Pages
{
    public class DashboardModel : PageModel
    {
        public List<String> colors;
        public string title;

        private DBContext _dbContext;

        public DashboardModel(DBContext dBContext)
        {
            this.colors = new List<String>(){"Red","Blue","Yellow"};
            this.title = "My First Dataset";
            _dbContext = dBContext;
            
        }

        public void OnGet()
        {

        }

        public void OnGetqs() { }
    }
}
