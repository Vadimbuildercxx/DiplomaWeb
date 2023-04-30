using Diploma.Models;
using Newtonsoft.Json;

namespace Diploma.Controllers
{
    public class ConfigurationManager
    {
        public ProjectConfiguration Config;

        public ConfigurationManager(string path) 
        {
            string json = System.IO.File.ReadAllText(path);
            Config = JsonConvert.DeserializeObject<ProjectConfiguration>(json);
        }

        public int GetCamId(string name)
        {
            int indexOfValue = Config.Sources.FindIndex(a => a.Contains(name));
            return Config.Cameras[indexOfValue];
        }

    }
}
