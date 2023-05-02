using Diploma.Models;

namespace Diploma.Models
{
    public enum Message
    {
        Info,
        Log,
        Warning,
        Alert,
    }

    public class Log
    {
        public int Id {  get; set; }
        public Message MessageType { get; set; }
        public string? Text { get; set; }
        public DateTime DateTime { get; set; }
        public int? CameraId { get; set; }
    }
}
