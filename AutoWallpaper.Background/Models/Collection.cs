using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AutoWallpaper.Background.Models
{
    public sealed class Collection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("total_photos")]
        public int Total_Photos { get; set; }
        [JsonProperty("preview_photos")]
        public IEnumerable<Photo> Preview_Photos { get; set; }

        public bool Selected { get; set; } = false;

    }

    public sealed class Photo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("urls")]
        public IDictionary<String, String> Urls { get; set; }
    }
    
}
