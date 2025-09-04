using BloggerAgent.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Dtos.A2ATaskDtos
{
    public class TextPart : ITaskPart
    {
        [JsonIgnore]
        public string Kind { get; set; } = "text";
        public string Text { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
      
    } 
    
    public class FilePart : ITaskPart
    {
        [JsonIgnore]
        public string Kind { get; set; } = "file";
        public FileContent File { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
      
    } 
    
    public class DataPart : ITaskPart
    {
        [JsonIgnore]
        public string Kind { get; set; } = "data";
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
      
    }
    
    public class FileContent 
    {
        public string? Name { get; set; }
        public string MimeType { get; set; }
        public string? Bytes { get; set; }
        public string? Url { get; set; }

      
    }
   
}
