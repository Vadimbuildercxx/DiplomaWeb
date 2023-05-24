using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Data;

namespace Diploma.Pages
{
    [Authorize(Roles = "Admin")]
    public class RolesModel : PageModel
    {

        RoleManager<IdentityRole> roleManager;
        public List<IdentityRole> Roles { get; set; }

        public RolesModel(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = roleManager.Roles.ToList();
            return Page();
        }

        //public PartialViewResult Index()
        //{
        //    var roles = roleManager.Roles.ToList();
        //    return new PartialViewResult
        //    {
        //        ViewName = "_RoleListPartial",
        //        ViewData = new ViewDataDictionary<List<IdentityRole>>(ViewData, roles)
        //    };
        //}
        public PartialViewResult OnGetCreate()
        {
            return new PartialViewResult
            {
                ViewName = "_RoleCreatePartial",
                ViewData = new ViewDataDictionary<IdentityRole>(ViewData, new IdentityRole())
            };
        }
        public async void OnPostCreate(IdentityRole role)
        {
            await roleManager.CreateAsync(role);
        }


        public async Task<IActionResult> Create(IdentityRole role)
        {
            await roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }
    }
}
