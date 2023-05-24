using Diploma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Diploma.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, DBContext dbContext)
        {
            _logger = logger;
            Console.WriteLine(dbContext.UserRoles.FirstOrDefault().RoleId + dbContext.UserRoles.FirstOrDefault().UserId);
        }

        public IActionResult OnGet()
        {

            return Page();
        }

    }
}