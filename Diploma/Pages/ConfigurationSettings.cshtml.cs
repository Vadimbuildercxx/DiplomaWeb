using Diploma.Models;
using Diploma.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diploma.Pages
{
    public class ConfigurationSettingsModel : PageModel
    {
        public ProjectConfiguration ProjectConfiguration { get; set; }

        private Controllers.ConfigurationManager _configurationManager { get; set; }

        public ConfigurationSettingsModel() {
            _configurationManager = new Controllers.ConfigurationManager(@"Files/startup_config.json");
        }

        public IActionResult OnGet()
        {
            ProjectConfiguration = _configurationManager.Config;
            
            return Page();
        }
    }
}
