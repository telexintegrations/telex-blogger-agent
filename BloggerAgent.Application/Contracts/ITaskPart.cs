using BloggerAgent.Application.Dtos.A2ATaskDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Contracts
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
    [JsonDerivedType(typeof(TextPart), "text")]
    [JsonDerivedType(typeof(FilePart), "file")]
    [JsonDerivedType(typeof(DataPart), "data")]
    public interface ITaskPart
    {
    }
}
