using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FilesCom
{
    /// <summary>
    /// The error body of a Files.com API error response, as its JSON gives it. <see cref="ApiException"/> keeps it as
    /// <see cref="ApiException.responseError"/>.
    /// </summary>
    public class ResponseError
    {
        /// <summary>
        /// The body's <c>detail</c> field, as it came.
        /// </summary>
        public string detail { get; set; }
        /// <summary>
        /// The error text the API gives in the body's <c>error</c> field, as it came
        /// (<see cref="ApiException.error"/> returns it).
        /// </summary>
        public string error { get; set; }
        /// <summary>
        /// The strings of the body's <c>errors</c> field, as they came.
        /// </summary>
        public string[] errors { get; set; }
        /// <summary>
        /// The HTTP status the body states (its <c>http-code</c> field).
        /// </summary>
        [JsonPropertyName("http-code")]
        public int httpCode { get; set; }
        /// <summary>
        /// The body's <c>instance</c> field, as it came.
        /// </summary>
        public string instance { get; set; }
        /// <summary>
        /// Error texts by the name of the attribute each is about (the body's <c>model_errors</c> field).
        /// </summary>
        [JsonPropertyName("model_errors")]
        public Dictionary<string, string[]> modelErrors { get; set; }
        /// <summary>
        /// The title the API gives the error type, such as <c>File Not Found</c>.
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// The error type, such as <c>not-found/file-not-found</c>. When the SDK reads an error response, it chooses the
        /// exception's class by this type.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// The body's <c>data</c> field, as it came: its members by name.
        /// </summary>
        public Dictionary<string, object> data { get; set; }
    }
}