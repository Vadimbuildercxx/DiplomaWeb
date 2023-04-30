﻿using Diploma.Models;
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

        public string DetectedObjectName(int index)
        {
            if (Detections?.Length -1 < index)
            {
                throw new IndexOutOfRangeException(nameof(Detections));
            }
            return ClassNames[(int)Detections[index][^1]];
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
            Console.WriteLine(yamlText);
            YamlConfiguration yamlConfig = _deserializer.Deserialize<YamlConfiguration>(yamlText);
            Console.WriteLine("----" + yamlConfig.Device + " " + yamlConfig.DetectedObjectName(1));
            AddDataToLog();
            return yamlConfig;
        }

        // Create message data Tuple<message text, type>
        public Tuple<string, Models.Message> CreateMessage(string yamlText, float threshold = 0.0f)
        {
            YamlConfiguration yamlConfiguration = ParseYamlString(yamlText);

            int camId = GetCamIdBySource(yamlConfiguration.Device);
            string areaName = GetAreaNameByCamId(camId);

            StringBuilder message = new StringBuilder();
            message.AppendLine("На территории " + areaName + ", идентификатор" + camId);
            message.AppendLine("Обнаружены нарушения:");

            List<int> objectsList = yamlConfiguration.Detections.Where(x => x[0] > threshold).Select(x => (int)x[1]).Distinct().ToList();

            for (int i = 0; i < objectsList.Count; i++)
            {

                message.AppendLine();
                Console.WriteLine(yamlConfiguration.Detections[i][0] + " " +
                    yamlConfiguration.Detections[i][1]);
                message.Append("");
            }
            

            return Tuple.Create("На территории [текст] обнаржены нарушения не ношения" + yamlConfiguration.DetectedObjectName(1),
                Models.Message.Warning);
        }

        public async Task AddDataToLog()
        {
            using (var scope = _provider.CreateScope())
            {
                var dbHandler = scope.ServiceProvider.GetRequiredService<DBContext>();
                Console.WriteLine(dbHandler.Areas.FirstOrDefault()?.Name);
            }
        }

        public string GetAreaNameByCamId(int id)
        {
            using (var scope = _provider.CreateScope())
            {
                var dbHandler = scope.ServiceProvider.GetRequiredService<DBContext>();
                int areaId = dbHandler.Cameras.Where(x=>x.Id == id).FirstOrDefaultAsync().Id;
                string name = dbHandler.Areas.Where(x => x.Id == id).FirstOrDefault().Name;
                return name;
            }
            
        }

        public int GetCamIdBySource(string source)
        {
            return _configurationManager.GetCamId(source);
        }



    }

}