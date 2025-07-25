using BloggerAgent.Application.Dtos.A2ATaskDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Helpers
{
    public class ValidationHelper
    {
        public static void ValidateRequest(A2aTaskRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Jsonrpc != "2.0")
                throw new ArgumentException("Invalid JSON-RPC version");

            if (string.IsNullOrWhiteSpace(request.Id))
                throw new ArgumentException("Id is required");

            if (string.IsNullOrEmpty(request.Method))
                throw new ArgumentException("Method required");

            var message = request.Params?.Message;
            if (message == null)
                throw new ArgumentException("Message params are required");

            //if (message.Role?.ToLower() != "user")
            //    throw new ArgumentException("Role must be 'user'");

            if (message.Parts == null || !message.Parts.Any())
                throw new ArgumentException("Message parts cannot be empty");

            foreach (var part in message.Parts)
            {
                if (part.Kind != "text")
                    throw new ArgumentException("Only 'text' type supported in message parts");

                if (string.IsNullOrWhiteSpace(part.Text))
                    throw new ArgumentException("Text cannot be empty");
            }

            // Validate IDs are GUIDs (optional but recommended)
            if (string.IsNullOrEmpty(message.ContextId))
                throw new ArgumentException("Invalid ContextId format");

            //if (message.TaskId != null && !Guid.TryParse(message.TaskId, out _))
            //    throw new ArgumentException("Invalid TaskId format");

            if (string.IsNullOrEmpty(message.MessageId))
                throw new ArgumentException("Message Id is required");

            // Optional: Validate push notification config if present
            var config = request?.Params?.Configuration;
            if (config?.PushNotificationConfig != null)
            {
                if (string.IsNullOrWhiteSpace(config.PushNotificationConfig.Url))
                    throw new ArgumentException("PushNotification Url is required");

                Authentication auth = config.PushNotificationConfig.Authentication;
                if (auth.Schemes.Count == 0 || !auth.Schemes.Contains("TelexApiKey"))
                    throw new ArgumentException("Invalid auth scheme");
                
                if (string.IsNullOrWhiteSpace(auth.Credentials))
                    throw new ArgumentException("Auth credentials is required");
            }
        }

       
    }
}


