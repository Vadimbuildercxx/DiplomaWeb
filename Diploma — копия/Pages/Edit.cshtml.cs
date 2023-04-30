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

namespace Diploma.Pages
{
    
    public class EditModel : PageModel
    {
        private class Element
        {
            public string? name { get; set; }
            public string? value { get; set; }
        }

        private DBContext _dbContext;

        [BindProperty]
        public PersonViewModel PersonViewModel {get;set;}
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
        public List<Camera> CameraList
        {
            get
            {
                return _dbContext.Cameras.ToList();
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
            PersonViewModel = new PersonViewModel();
        }



        #region AreaOperations

        public PartialViewResult OnGetAreaModalDeletePartial(int id)
        {
            return new PartialViewResult
            {
                ViewName = "_AreaDeleteWarningPartial",
                ViewData = new ViewDataDictionary<int>(ViewData, id)
            };
        }

        public async Task OnPostAreaModalDeletePartial(int id)
        {
            Area person = await _dbContext.Areas.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (person != null)
            {
                Console.WriteLine("Deleted id: " + id.ToString());
                _dbContext.Areas.Remove(person);
            }
            await _dbContext.SaveChangesAsync();
        }


        public async Task<PartialViewResult> OnGetAreaModalEditPartial(int id)
        {
            Area model = await _dbContext.Areas.Where(x => x.Id == id).FirstOrDefaultAsync();

            return new PartialViewResult
            {
                ViewName = "_AreaModalPartial",
                ViewData = new ViewDataDictionary<Area>(ViewData, model)
            };
        }

        public PartialViewResult OnGetAreaModalPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_AreaModalPartial",
                ViewData = new ViewDataDictionary<Area>(ViewData, new Area { })
            };
        }


        public async Task<PartialViewResult> OnPostAreaModalPartial(Area model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(model.Id);
                if (model.Id != 0)
                {
                    Area camera = await _dbContext.Areas.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                    if (camera != null)
                    {
                        _dbContext.Areas.Remove(camera);
                        await _dbContext.Areas.AddAsync(model);
                    }
                }
                else
                {
                    await _dbContext.Areas.AddAsync(model);
                }

                await _dbContext.SaveChangesAsync();
            }

            return new PartialViewResult
            {
                ViewName = "_AreaModalPartial",
                ViewData = new ViewDataDictionary<Area>(ViewData, model)
            };
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

        public JsonResult OnGetAreas()
        {
            List<Area> areaList = _dbContext.Areas.ToList();
            return new JsonResult(areaList);

        }

        public PartialViewResult OnGetCameraModalDeletePartial(int id)
        {
            return new PartialViewResult
            {
                ViewName = "_CameraDeleteWarningPartial",
                ViewData = new ViewDataDictionary<int>(ViewData, id)
            };
        }

        public async Task OnPostCameraModalDeletePartial(int id)
        {
            Camera person = await _dbContext.Cameras.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (person != null)
            {
                Console.WriteLine("Deleted id: " + id.ToString());
                _dbContext.Cameras.Remove(person);
            }
            await _dbContext.SaveChangesAsync();
        }


        public async Task<PartialViewResult> OnGetCameraModalEditPartial(int id)
        {
            Camera model = await _dbContext.Cameras.Where(x => x.Id == id).FirstOrDefaultAsync();

            return new PartialViewResult
            {
                ViewName = "_CameraModalPartial",
                ViewData = new ViewDataDictionary<Camera>(ViewData, model)
            };
        }

        public PartialViewResult OnGetCameraModalPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_CameraModalPartial",
                ViewData = new ViewDataDictionary<Camera>(ViewData, new Camera { })
            };
        }


        public async Task<PartialViewResult> OnPostCameraModalPartial(Camera model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(model.Id);
                if (model.Id != 0)
                {
                    Camera camera = await _dbContext.Cameras.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                    if (camera != null)
                    {
                        _dbContext.Cameras.Remove(camera);
                        await _dbContext.Cameras.AddAsync(model);
                    }
                }
                else
                {
                    await _dbContext.Cameras.AddAsync(model);
                }

                await _dbContext.SaveChangesAsync();
            }

            return new PartialViewResult
            {
                ViewName = "_CameraModalPartial",
                ViewData = new ViewDataDictionary<Camera>(ViewData, model)
            };
        }

        public PartialViewResult OnGetCameraListPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_CameraListPartial",
                ViewData = new ViewDataDictionary<List<Camera>>(ViewData, CameraList)
            };
        }
        #endregion


        #region PersonOperations
        public PartialViewResult OnGetPersonModalDeletePartial(int id)
        {
            return new PartialViewResult
            {
                ViewName = "_PersonDeleteWarningPartial",
                ViewData = new ViewDataDictionary<int>(ViewData, id)
            };
        }

        public async Task OnPostPersonModalDeletePartial(int id)
        {
            Person person = await _dbContext.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (person != null)
            {
                Console.WriteLine("Deleted id: " + id.ToString());
                _dbContext.Persons.Remove(person);
            }
            await _dbContext.SaveChangesAsync();
        }


        public async Task<PartialViewResult> OnGetPersonModalEditPartial(int id)
        {
            Person model = await _dbContext.Persons.Where(x => x.Id == id).FirstOrDefaultAsync();

            return new PartialViewResult
            {
                ViewName = "_PersonModalPartial",
                ViewData = new ViewDataDictionary<Person>(ViewData, model)
            };
        }

        public PartialViewResult OnGetPersonModalPartial()
        {
            Console.WriteLine("44444");
            return new PartialViewResult
            {
                ViewName = "_PersonModalPartial",
                ViewData = new ViewDataDictionary<Person>(ViewData, new Person { })
            };
        }


        public async Task<PartialViewResult> OnPostPersonModalPartial(Person model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(model.Id);
                if (model.Id != 0)
                {
                    Person person = await _dbContext.Persons.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                    if (person != null)
                    {
                        _dbContext.Persons.Remove(person);
                        await _dbContext.Persons.AddAsync(model);
                    }
                }
                else
                {
                    await _dbContext.Persons.AddAsync(model);
                }
                
                await _dbContext.SaveChangesAsync();
            }

            return new PartialViewResult
            {
                ViewName = "_PersonModalPartial",
                ViewData = new ViewDataDictionary<Person>(ViewData, model)
            };
        }

        public PartialViewResult OnGetPersonListPartial()
        {
            return new PartialViewResult
            {
                ViewName = "_PersonListPartial",
                ViewData = new ViewDataDictionary<List<Person>>(ViewData, PersonList)
            };
        }
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
