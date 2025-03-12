using System.Text.Json;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Services
{
   
    public class TelexIntegrationService : ITelexIngegrationService
    {
        private string _configurationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Integration.json");


        public string LoadIntegration()
        {
            if (!File.Exists(_configurationFilePath))
            {
                throw new FileNotFoundException("The configuration file was not found.");
            }

            var json = File.ReadAllText(_configurationFilePath);
            using JsonDocument doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc.RootElement);

        }
    }
}
   




