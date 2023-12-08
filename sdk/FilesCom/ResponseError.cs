using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FilesCom
{
    public class ResponseError
    {
        public string detail { get; set; }
        public string error { get; set; }
        public string[] errors { get; set; }
        [JsonPropertyName("http-code")]
        public int httpCode { get; set; }
        public string instance { get; set; }
        [JsonPropertyName("model_errors")]
        public Dictionary<string, string[]> modelErrors { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public Dictionary<string, object> data { get; set; }
    }
}