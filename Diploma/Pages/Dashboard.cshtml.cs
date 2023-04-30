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
        public DashboardModel()
        {
            this.colors = new List<String>(){"Red","Blue","Yellow"};
            this.title = "My First Dataset";
        }

        public void OnGet()
        {
        }
    }
}
