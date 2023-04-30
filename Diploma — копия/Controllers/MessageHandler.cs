using Diploma.Models;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
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

        public MessageHandler(IServiceProvider provider) 
        {
            _provider = provider;

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
        public Tuple<string, Message> CreateMessage(YamlConfiguration yamlConfig)
        {
            return Tuple.Create("На территории [текст] обнаржены нарушения не ношения" + yamlConfig.DetectedObjectName(1),
                Message.Warning);
        }

        public async Task AddDataToLog()
        {
            using (var scope = _provider.CreateScope())
            {
                var dbHandler = scope.ServiceProvider.GetRequiredService<DBContext>();
                Console.WriteLine(dbHandler.Areas.FirstOrDefault()?.Name);
            }
        }



    }

}
