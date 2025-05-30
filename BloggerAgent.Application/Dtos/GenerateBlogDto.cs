﻿using System.Text.Json.Serialization;
using BloggerAgent.Domain.Commons;

namespace BloggerAgent.Application.Dtos
{
    public class GenerateBlogDto
    {
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } 
        
        [JsonPropertyName("thread_id")]
        public string? ThreadId { get; set; }

        [JsonPropertyName("org_id")]
        public string OrganizationId { get; set; }

        [JsonPropertyName("settings")]
        public List<Setting>? Settings { get; set; } = new();
    }
}
