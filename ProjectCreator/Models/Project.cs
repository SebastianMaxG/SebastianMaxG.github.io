using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ProjectCreator.Models
{    internal class Project
    {
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("image")]
        public string ImagePath { get; set; } = string.Empty;

        [JsonProperty("alt")]
        public string ImageAlt { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("link")]
        public string? Link { get; set; } = null;

        [JsonProperty("Tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
} 
