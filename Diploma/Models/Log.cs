using Diploma.Models;
using System.Drawing.Printing;

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
        public int? PPEId { get; set; }
        public string? DetectionPath { get; set;}
        public int? PersonId { get; set; }
        public float? PersonConf { get; set; }
        public int? ObjCount { get; set; }
    }
}
