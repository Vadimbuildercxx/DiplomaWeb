using Diploma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;

namespace Diploma.Pages
{
    [Authorize(Roles = "Admin")]
    public class UploadModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> Files { get; set; }
        public int Num { get; set; } = 367;

        public List<SelectListItem> PersonsListSelect {
            get
            {
                return _dbContext.Persons.Select( x => new SelectListItem 
                {
                    Value = x.Id.ToString(), 
                    Text = x.LastName + " " + x.FirstName + " " + x.Patronymic
                }).ToList();
            }
        }
        const string _wwwroot = "wwwroot";
        const string _path = "person_faces";

        private DBContext _dbContext;

        public UploadModel(DBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task OnPostUploadFile(ICollection<IFormFile> files, int id)
        {
            //var directoryPath = Path.Combine(_appSettings.UploadFolder, user.SessionID);
            //Directory.CreateDirectory(directoryPath);
            Console.WriteLine("--------" + id.ToString());
            if (id != 0)
            {
                string dirPath = Path.Combine(_path, "person_" + id.ToString());
                Console.WriteLine($"{dirPath}");
                if (!Directory.Exists(dirPath))
                {
                    Console.WriteLine($"{dirPath}");
                    Directory.CreateDirectory(Path.Combine(_wwwroot, dirPath));
                }

                foreach (var file in files)
                {
                    Console.WriteLine(file.FileName);

                    using FileStream fileStream = new(Path.Combine(Path.Combine(_wwwroot, dirPath), file.FileName), FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    _dbContext.PersonsXPaths.Add(new PersonsXPaths
                    {
                        PersonId = id,
                        Path = Path.Combine(dirPath, file.FileName),
                    });
                    await _dbContext.SaveChangesAsync();

                }
            }
            
        }
    }
}
