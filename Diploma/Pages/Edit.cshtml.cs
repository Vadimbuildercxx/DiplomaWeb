using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diploma.Models;
using Microsoft.AspNetCore.Identity;
using Diploma.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Web;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using System;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;

namespace Diploma.Pages
{
    public record class CameraTmp(Camera Camera, List<SelectListItem>? Selects);

    public class EditModel : PageModel
    {
        private class Element
        {
            public string? name { get; set; }
            public string? value { get; set; }
        }

        private DBContext _dbContext;

        //[BindProperty]
        //public _PersonViewModel PersonViewModel {get;set;}
        [BindProperty]
        public Area RoomModel { get; set; }

        [BindProperty]
        public List<Person> PersonList 
        { 
            get
            {
                return _dbContext.Persons.ToList();
            }
            private set { }

        }

        [BindProperty]
        public List<CameraDTO> CameraList
        {
            get
            {
                List<CameraDTO> dtoCameras = new List<CameraDTO>();
                List<Camera> cameras = _dbContext.Cameras.ToList();
                foreach (Camera cam in cameras)
                {
                    CameraDTO cameraDTO = new CameraDTO
                    {
                        Id = cam.Id,
                        Name = cam.Name,
                        Description = cam.Description,
                        AreaName = _dbContext.Areas.Where(x => x.Id == cam.AreaId).Select(x => x.Name).FirstOrDefault()
                    };
                    dtoCameras.Add(cameraDTO);
                }
                return dtoCameras;
            }
            private set { }

        }

        [BindProperty]
        public List<Area> AreaList
        {
            get
            {
                return _dbContext.Areas.ToList();
            }
            private set { }

        }

        [BindProperty]
        public List<IdentityUser> UserList
        {
            get
            {
                return _dbContext.Users.ToList();
            }
            private set { }

        }

        public EditModel(DBContext dBContext)    
        {
            _dbContext = dBContext;
            //PersonViewModel = new PersonViewModel();
        }



        #region AreaOperations
        public PartialViewResult OnGetAreaDelete(int id)
        {
            return new PartialViewResult
            {
                ViewName = "_AreaDeleteWarningPartial",
                ViewData = new ViewDataDictionary<int>(ViewData, id)
            };
        }

        public async Task<PartialViewResult> OnPostAreaDelete(int id)
        {
            Area area = await _dbContext.Areas.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (area != null)
            {
                _dbContext.Remove(area);
                await _dbContext.SaveChangesAsync();

            }
            return new PartialViewResult
            {
                ViewName = "_AreaListPartial",
                ViewData = new ViewDataDictionary<List<Area>>(ViewData, AreaList)
            };

        }

        public async Task<PartialViewResult> OnGetAreaCreateOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return new PartialViewResult
                {
                    ViewName = "_AreaModalPartial",
                    ViewData = new ViewDataDictionary<Area>(ViewData, new Area())
                };
            }
            else
            {
                Area area = await _dbContext.Areas.Where(x => x.Id == id).FirstOrDefaultAsync();
                return new PartialViewResult
                {
                    ViewName = "_AreaModalPartial",
                    ViewData = new ViewDataDictionary<Area>(ViewData, area)
                };
            }
        }
        public async Task<PartialViewResult> OnPostAreaCreateOrEdit(int id, Area area)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    await _dbContext.AddAsync(area);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    Area qArea = (from ar in _dbContext.Areas
                                  where ar.Id == id
                                  select ar).First();
                    qArea.Name = area.Name;
                    qArea.Description = area.Description;
                    qArea.Level = area.Level;

                    await _dbContext.SaveChangesAsync();
                }
                return new PartialViewResult
                {
                    ViewName = "_AreaListPartial",
                    ViewData = new ViewDataDictionary<List<Area>>(ViewData, AreaList)
                };
            }
            else
            {
                return new PartialViewResult
                {
                    ViewName = "_AreaModalPartial",
                    ViewData = new ViewDataDictionary<Area>(ViewData, area)
                };
            }
        }
        public PartialViewResult OnGetAreaListPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_AreaListPartial",
                ViewData = new ViewDataDictionary<List<Area>>(ViewData, AreaList)
            };
        }
        #endregion



        #region CameraOperations

        public PartialViewResult OnGetCameraDelete(int id)
        {
            return new PartialViewResult
            {
                ViewName = "_CameraDeleteWarningPartial",
                ViewData = new ViewDataDictionary<int>(ViewData, id)
            };
        }

        public async Task<PartialViewResult> OnPostCameraDelete(int id)
        {
            Camera camera = await _dbContext.Cameras.Where(x=> x.Id == id).FirstOrDefaultAsync();

            if (camera != null) 
            {
                _dbContext.Remove(camera);
                await _dbContext.SaveChangesAsync();
                    
            }
            return new PartialViewResult
            {
                ViewName = "_CameraListPartial",
                ViewData = new ViewDataDictionary<List<CameraDTO>>(ViewData, CameraList)
            };

        }

        public async Task<PartialViewResult> OnGetCameraCreateOrEdit(int id = 0)
        {
            List<SelectListItem> areas = _dbContext.Areas.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();
            if (id == 0)
            {
                return new PartialViewResult
                {
                    ViewName = "_CameraModalPartial",
                    ViewData = new ViewDataDictionary<CameraTmp>(ViewData, new CameraTmp(new Camera(), areas))
                };
            }
            else
            {
                Camera camera = await _dbContext.Cameras.Where(x => x.Id == id).FirstOrDefaultAsync();
                return new PartialViewResult
                {
                    ViewName = "_CameraModalPartial",
                    ViewData = new ViewDataDictionary<CameraTmp>(ViewData, new CameraTmp(camera, areas))
                };
            }
        }
        public async Task<PartialViewResult> OnPostCameraCreateOrEdit(int id, Camera camera)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    await _dbContext.AddAsync(camera);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    Camera qcam = (from cam in _dbContext.Cameras
                                  where cam.Id == id
                                  select cam).First();
                    qcam.Name = camera.Name;
                    qcam.AreaId = camera.AreaId;
                    qcam.Description = camera.Description;

                    await _dbContext.SaveChangesAsync();
                }
                return new PartialViewResult
                {
                    ViewName = "_CameraListPartial",
                    ViewData = new ViewDataDictionary<List<CameraDTO>>(ViewData, CameraList)
                };
            }
            else
            {
                return new PartialViewResult
                {
                    ViewName = "_CameraModalPartial",
                    ViewData = new ViewDataDictionary<Camera>(ViewData, camera)
                };
            }
        }
        
        public PartialViewResult OnGetCameraListPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_CameraListPartial",
                ViewData = new ViewDataDictionary<List<CameraDTO>>(ViewData, CameraList)
            };
        }
        #endregion


        #region PersonOperations

        public PartialViewResult OnGetPersonDelete(int id)
        {
            return new PartialViewResult
            {
                ViewName = "_PersonDeleteWarningPartial",
                ViewData = new ViewDataDictionary<int>(ViewData, id)
            };
        }

        public async Task<PartialViewResult> OnPostPersonDelete(int id)
        {
            Person person = await _dbContext.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (person != null)
            {
                _dbContext.Remove(person);
                await _dbContext.SaveChangesAsync();

            }
            return new PartialViewResult
            {
                ViewName = "_PersonListPartial",
                ViewData = new ViewDataDictionary<List<Person>>(ViewData, PersonList)
            };

        }

        public async Task<PartialViewResult> OnGetPersonCreateOrEdit(int id = 0)
        {
            if (id == 0)
            {
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++");
                return new PartialViewResult
                {
                    ViewName = "_PersonModalPartial",
                    ViewData = new ViewDataDictionary<Person>(ViewData, new Person())
                };
            }
            else
            {
                Person person = await _dbContext.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();
                return new PartialViewResult
                {
                    ViewName = "_PersonModalPartial",
                    ViewData = new ViewDataDictionary<Person>(ViewData, person)
                };
            }
        }
        public async Task<PartialViewResult> OnPostPersonCreateOrEdit(int id, Person person)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    await _dbContext.AddAsync(person);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    Person qperson = (from per in _dbContext.Persons
                                   where per.Id == id
                                   select per).First();

                    qperson.FirstName = person.FirstName;
                    qperson.LastName = person.LastName;
                    qperson.Patronymic = person.Patronymic;
                    qperson.Age = person.Age;
                    qperson.Gender = person.Gender;
                    qperson.Email = person.Email;
                    qperson.Phone = person.Phone;

                    await _dbContext.SaveChangesAsync();
                }
                return new PartialViewResult
                {
                    ViewName = "_PersonListPartial",
                    ViewData = new ViewDataDictionary<List<Person>>(ViewData, PersonList)
                };
            }
            else
            {
                return new PartialViewResult
                {
                    ViewName = "_PersonModalPartial",
                    ViewData = new ViewDataDictionary<Person>(ViewData, person)
                };
            }
        }

        public PartialViewResult OnGetPersonListPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_PersonListPartial",
                ViewData = new ViewDataDictionary<List<Person>>(ViewData, PersonList)
            };
        }
        //public PartialViewResult OnGetPersonModalDeletePartial(int id)
        //{
        //    return new PartialViewResult
        //    {
        //        ViewName = "_PersonDeleteWarningPartial",
        //        ViewData = new ViewDataDictionary<int>(ViewData, id)
        //    };
        //}

        //public async Task OnPostPersonModalDeletePartial(int id)
        //{
        //    Person person = await _dbContext.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (person != null)
        //    {
        //        Console.WriteLine("Deleted id: " + id.ToString());
        //        _dbContext.Persons.Remove(person);
        //    }
        //    await _dbContext.SaveChangesAsync();
        //}


        //public async Task<PartialViewResult> OnGetPersonModalEditPartial(int id)
        //{
        //    Person model = await _dbContext.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();

        //    return new PartialViewResult
        //    {
        //        ViewName = "_PersonModalPartial",
        //        ViewData = new ViewDataDictionary<Person>(ViewData, model)
        //    };
        //}

        //public PartialViewResult OnGetPersonModalPartial()
        //{
        //    Console.WriteLine("44444");
        //    return new PartialViewResult
        //    {
        //        ViewName = "_PersonModalPartial",
        //        ViewData = new ViewDataDictionary<Person>(ViewData, new Person { })
        //    };
        //}


        //public async Task<PartialViewResult> OnPostPersonModalPartial(Person model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Console.WriteLine(model.Id);
        //        if (model.Id != 0)
        //        {
        //            Person person = await _dbContext.Persons.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
        //            if (person != null)
        //            {
        //                _dbContext.Persons.Remove(person);
        //                await _dbContext.Persons.AddAsync(model);
        //            }
        //        }
        //        else
        //        {
        //            await _dbContext.Persons.AddAsync(model);
        //        }

        //        await _dbContext.SaveChangesAsync();
        //    }

        //    return new PartialViewResult
        //    {
        //        ViewName = "_PersonModalPartial",
        //        ViewData = new ViewDataDictionary<Person>(ViewData, model)
        //    };
        //}

        //public PartialViewResult OnGetPersonListPartial()
        //{
        //    return new PartialViewResult
        //    {
        //        ViewName = "_PersonListPartial",
        //        ViewData = new ViewDataDictionary<List<Person>>(ViewData, PersonList)
        //    };
        //}
        #endregion
        //public async Task<JsonResult> OnGetPersonList() => new JsonResult(PersonList);


        //public async Task<JsonResult> OnPostEditPerson([FromBody] dynamic json)
        //{

        //    string jsonStr = Convert.ToString(json);
        //    if (!string.IsNullOrEmpty(jsonStr))
        //    {
        //        JsonNode data = JsonNode.Parse(jsonStr);
        //        int id = (int)data["id"];
        //        Person person = _dbContext.Persons.Where(p => p.Id == id).Single();
        //        _dbContext.Persons.Remove(person);
        //        await _dbContext.SaveChangesAsync();
        //    }

        //    return new JsonResult(PersonList);
        //}

        //public async Task<JsonResult> OnPostDeletePerson([FromBody]dynamic json)
        //{

        //    string jsonStr = Convert.ToString(json);
        //    if (!string.IsNullOrEmpty(jsonStr))
        //    {
        //        JsonNode data = JsonNode.Parse(jsonStr);
        //        int id = (int)data["id"];
        //        Person person = _dbContext.Persons.Where(p => p.Id == id).Single();
        //        _dbContext.Persons.Remove(person);
        //        await _dbContext.SaveChangesAsync();
        //    }

        //    return new JsonResult(PersonList);
        //}

        //public async Task<JsonResult> OnPostCreatePerson([FromBody]dynamic data)
        //{
        //    string jsonStr = Convert.ToString(data);

        //    List<Element> json = JsonConvert.DeserializeObject<List<Element>>(jsonStr);

        //    var rgx = new Regex("PersonViewModel\\.");
        //    foreach (var element in json)
        //    {
        //        if (element.name != null) {
        //            element.name = rgx.Replace(element.name, "", 1);
        //        }
        //    }

        //    PersonViewModel viewModel = new PersonViewModel()
        //    {
        //        FirstName = json.Where(x=> x.name == "FirstName").Select(x=> x.value).FirstOrDefault(),
        //        LastName = json.Where(x => x.name == "LastName").Select(x => x.value).FirstOrDefault(),
        //        Patronymic = json.Where(x => x.name == "Patronymic").Select(x => x.value).FirstOrDefault(),
        //        Email = json.Where(x => x.name == "Email").Select(x => x.value).FirstOrDefault(),
        //        Age = int.Parse(json.Where(x => x.name == "Age").Select(x => x.value).FirstOrDefault()),
        //        Phone = json.Where(x => x.name == "Phone").Select(x => x.value).FirstOrDefault(),
        //        Gender = json.Where(x => x.name == "Gender").Select(x => x.value).FirstOrDefault(),
        //    };

        //    Person person = new Person();
        //    //await _dbContext.Persons.AddAsync(person);
        //    //await _dbContext.SaveChangesAsync();
        //    return new JsonResult(PersonList);
        //}

        //public JsonResult OnGetPersonList()
        //{
        //    List<PersonViewModel> viewModels = _dbContext.Persons.Select(x => ConvertPersonToPersonViewModel(x)).ToList();
        //    foreach (var item in viewModels)
        //    {
        //        Console.WriteLine(item.Info());
        //    }
        //    return new JsonResult(viewModels);
        //}

    }
}
