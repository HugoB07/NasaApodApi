using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace NasaApodApi.Model
{
    public class Apod
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Copyright { get; set; }
        public string Date { get; set; }
        public string Explanation { get ; set; }
        public string Hdurl { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
