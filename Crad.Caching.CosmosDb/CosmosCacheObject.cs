using Newtonsoft.Json;
using System;

namespace Crad.Caching.CosmosDb
{
    internal class CosmosDbCacheItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeToLive { get; set; }

        public byte[] ToByteContent()
        {
            return Convert.FromBase64String(this.Content);
        }

        public static CosmosDbCacheItem Build(string key, int ttl, byte[] content)
        {
            return new CosmosDbCacheItem()
            {
                Id = key,
                TimeToLive = ttl,
                Content = Convert.ToBase64String(content)
            };
        }
    }
}
