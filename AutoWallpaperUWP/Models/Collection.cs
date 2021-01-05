using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoWallpaperUWP.Models
{
    public class Collection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("total_photos")]
        public int Total_Photos { get; set; }
        [JsonProperty("preview_photos")]
        public List<Photo> Preview_Photos { get; set; }

        public bool Selected { get; set; } = false;

    }

    public class Photo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("urls")]
        public Dictionary<string, string> Urls { get; set; }
    }
    
}
