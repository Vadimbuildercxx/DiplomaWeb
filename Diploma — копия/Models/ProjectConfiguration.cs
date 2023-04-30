using System.Text.Json.Serialization;

namespace Diploma.Models
{
    public class ProjectConfiguration
    {
        [JsonPropertyName("Cameras")]
        public List<int>? Cameras { get; set; }
        [JsonPropertyName("Sources")]
        public List<string>? Sources { get; set; }
    }
}
