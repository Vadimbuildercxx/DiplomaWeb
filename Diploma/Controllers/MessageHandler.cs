using Diploma.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public MessageHandler(IServiceProvider provider) 
        {
            _provider = provider;
            _configurationManager = new ConfigurationManager(@"Files/startup_config.json");

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
        public async Task<Tuple<string, Models.Message>> CreateMessage(string yamlText,
            float probThreshold = 0.0f, int maxDetCount = 0)
        {
            YamlConfiguration yamlConfiguration = ParseYamlString(yamlText);

            int camId = GetCamIdBySource(yamlConfiguration.Device);
            string areaName = await GetAreaNameByCamId(camId);

            StringBuilder message = new StringBuilder();
            message.AppendLine("На территории " + areaName + ", идентификатор" + camId);
            message.AppendLine("Обнаружены нарушения:");

            List<int> objectsList = yamlConfiguration.Detections.Where(x => x[0] > probThreshold).Select(x => (int)x[1]).Distinct().ToList();

            for (int i = 0; i < objectsList.Count; i++)
            {
                int objCount = yamlConfiguration.Detections.Where(x => x[1] == objectsList[i]).Select(x => x[1]).ToList().Count;
                string detObjName = yamlConfiguration.DetectedObjectName(objectsList[i]);
                int ppeItemId = await GetIdByPPE(detObjName);
                if (detObjName[..2] == "NO")
                {
                    if (maxDetCount < objCount)
                    {

                        Console.WriteLine(yamlConfiguration.Detections.Length + " " + i);

                        message.AppendLine("Не ношение предмета [" + detObjName + "]"
                        + ", в количестве - " + objCount);
                        await AddDataToLog(Models.Message.Alert, message.ToString(),
                            DateTime.Now, camId, ppeItemId, yamlConfiguration.ImagePath, objCount);
                        return Tuple.Create(message.ToString(), Models.Message.Alert);
                    }

                    else
                    {
                        message.AppendLine("Не ношение предмета [" + detObjName + "]"
                            + ", в количестве - " + objCount);

                    }
                }
                message.AppendLine("Предмет [" + detObjName + "]"
                                    + ", в количестве - " + objCount);
                await AddDataToLog(Models.Message.Alert, message.ToString(),
                    DateTime.Now, camId, ppeItemId, yamlConfiguration.ImagePath, objCount);

            }


            return Tuple.Create(message.ToString(), Models.Message.Warning);
        }

        public async Task AddDataToLog(Models.Message mType, string? text,
            DateTime dt, int camId, int PPEid, string detectionPath, int objCount)
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

        public int GetCamIdBySource(string source)
        {
            return _configurationManager.GetCamId(source);
        }



    }

}
