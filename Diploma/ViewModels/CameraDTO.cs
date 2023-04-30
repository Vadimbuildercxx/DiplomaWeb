using Diploma.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diploma.ViewModels
{
    public class CameraDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AreaName { get; set; }

    }
}
