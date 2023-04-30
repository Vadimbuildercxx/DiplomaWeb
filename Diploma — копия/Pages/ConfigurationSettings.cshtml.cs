using Diploma.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diploma.Pages
{
    public class ConfigurationSettingsModel : PageModel
    {
        public ProjectConfiguration ProjectConfiguration { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string json = await System.IO.File.ReadAllTextAsync(@"Files/startup_config.json");
            ProjectConfiguration = JsonConvert.DeserializeObject<ProjectConfiguration>(json);
            
            return Page();
        }
    }
}
