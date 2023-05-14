using Diploma.Models;
using Microsoft.CodeAnalysis.Text;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;



namespace Diploma.Controllers
{
    public class YamlConfiguration
    {
        public string? Device { get; set; }
        public float[][]? Detections { get; set; }
        public string[]? ClassNames { get; set; }
        public string[]? Persons { get; set; }
        public float[]? PersonsConf { get; set; }
        public string? ImagePath{ get; set; }

        public string DetectedObjectName(int index)
        {
            if (ClassNames?.Length <= index)
            {

                throw new IndexOutOfRangeException(nameof(ClassNames));

            }
            return ClassNames[index];
        }
    }

    public class MessageHandler
    {
        private IDeserializer _deserializer;
        private readonly IServiceProvider _provider;
        private readonly ConfigurationManager _configurationManager;
        private string _faceClass;

        public MessageHandler(IServiceProvider provider) 
        {
            _provider = provider;
            _configurationManager = new ConfigurationManager(@"Files/startup_config.json");

            string text = File.ReadAllText(@"Files/startup_config.json");
            var parsedObject = JObject.Parse(text);
            _faceClass = parsedObject["FaceClass"].ToString();

            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        }

        public YamlConfiguration ParseYamlString(string yamlText)
        {
            //Console.WriteLine(yamlText);
            YamlConfiguration yamlConfig = _deserializer.Deserialize<YamlConfiguration>(yamlText);
            //Console.WriteLine("----" + yamlConfig.Device + " " + yamlConfig.DetectedObjectName(1));
            //AddDataToLog();
            return yamlConfig;
        }

        // Create message data Tuple<message text, type>
        public async Task<Tuple<string, Models.Message>>? CreateMessage(string yamlText,
            float probThreshold = 0.0f, int maxDetCount = 0)
        {
            YamlConfiguration yamlConfiguration = ParseYamlString(yamlText);

            int camId = GetCamIdBySource(yamlConfiguration.Device);
            string areaName = await GetAreaNameByCamId(camId);

            StringBuilder message = new StringBuilder();
            message.AppendLine("На территории " + areaName + ", id камеры - " + camId);
            message.AppendLine("Обнаружены нарушения:");

            //List<int> objectsList = yamlConfiguration.Detections.Where(x => x[0] > probThreshold).Select(x => (int)x[1]).Distinct().ToList();
            List<int> validIds = new List<int>();

            if(yamlConfiguration.Detections == null)
            {
                return null;
            }
            for (int i = 0; i < yamlConfiguration.Detections.Length; i++)
            {
                if (yamlConfiguration.Detections[i][0] > probThreshold)
                {
                    validIds.Add(i);
                }
            }

            //Detect faces to logs
            foreach (int valId in validIds)
            {
                int objIndex =      (int)yamlConfiguration.Detections[valId][1];
                float objConf =     (int)yamlConfiguration.Detections[valId][0];
                string detObjName = yamlConfiguration.ClassNames[objIndex];
                int ppeItemId =     await GetIdByPPE(detObjName);

                if (_faceClass == detObjName)
                {
                    int personId = Int32.Parse(yamlConfiguration.Persons[valId].Replace("person_", ""));
                    string name = await GetFullNameById(personId);
                    message.AppendLine("Не ношение СИЗ"
                        + ", работником - " + name + " ");
                    await AddDataToLog(Models.Message.Warning, message.ToString(),
                        DateTime.Now, camId, ppeItemId, yamlConfiguration.ImagePath, 1, personId, objConf);
                }
            }
            //List<string> uniqueObject = yamlConfiguration.Detections.Where(
            //    x => yamlConfiguration.ClassNames.Contains(yamlConfiguration.DetectedObjectName((int)x[1])) &&
            //    yamlConfiguration.DetectedObjectName((int)x[1]) != _faceClass).Select(x => yamlConfiguration.DetectedObjectName((int)x[1])).ToList();

            List<string> uniqueObject = yamlConfiguration.ClassNames.Where(x => x != _faceClass).ToList();


            foreach (var item in uniqueObject)
            {
                int ppeItemId = await GetIdByPPE(item);
                int count = yamlConfiguration.Detections.Where(x => yamlConfiguration.ClassNames[(int)x[1]] == item).ToList().Count;
                message.AppendLine("Обнаржен предмет [" + item + "]"
                    + ", в количестве - " + count+ " ");
                await AddDataToLog(Models.Message.Info, message.ToString(),
                    DateTime.Now, camId, ppeItemId, yamlConfiguration.ImagePath, 1, 0, 0);
            }

            return Tuple.Create(message.ToString(), Models.Message.Warning);
        }

        public async Task AddDataToLog(Models.Message mType, string? text,
            DateTime dt, int camId, int PPEid, string detectionPath, int objCount, int personId, float faceConf)
        {
            using (var scope = _provider.CreateScope())
            {
                var dbHandler = scope.ServiceProvider.GetRequiredService<DBContext>();
                Log log = new()
                {
                    CameraId = camId,
                    DateTime = dt,
                    MessageType = mType,
                    Text = text,
                    PPEId = PPEid,
                    DetectionPath = detectionPath,
                    ObjCount = objCount,
                    PersonId = personId,
                    PersonConf = faceConf
                };
                await dbHandler.AddAsync(log);
                await dbHandler.SaveChangesAsync();
            }
        }

        public async Task<string> GetAreaNameByCamId(int id)
        {
            using (var scope = _provider.CreateScope())
            {
                var dbHandler = scope.ServiceProvider.GetRequiredService<DBContext>();

                int areaId = await dbHandler.Cameras.Where(x=>x.Id == id).Select(x=>x.Id).FirstOrDefaultAsync();
                string name = await dbHandler.Areas.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync();
                return name;
            }
            
        }

        public async Task<int> GetIdByPPE(string name)
        {
            using (var scope = _provider.CreateScope())
            {
                var dbHandler = scope.ServiceProvider.GetRequiredService<DBContext>();

                int id = await dbHandler.PPEs.Where(x => x.Name == name).Select(x => x.Id).FirstOrDefaultAsync();
                return id;
            }

        }

        public async Task<string> GetFullNameById(int id)
        {
            using (var scope = _provider.CreateScope())
            {
                var dbHandler = scope.ServiceProvider.GetRequiredService<DBContext>();

                string fullName = await dbHandler.Persons.Where(x => x.Id == id).Select(x => 
                    x.LastName + " " + x.FirstName[0]+"."
                ).FirstOrDefaultAsync();
                return fullName;
            }

        }

        public int GetCamIdBySource(string source)
        {
            return _configurationManager.GetCamId(source);
        }



    }

}
