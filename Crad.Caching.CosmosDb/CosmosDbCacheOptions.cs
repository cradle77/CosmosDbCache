using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Crad.Caching.CosmosDb
{
    public class CosmosDbCacheOptions : IOptions<CosmosDbCacheOptions>
    {
        public string ServiceUri { get; set; }
        public string AuthKey { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }

        public List<string> PreferredLocations { get; private set; }

        CosmosDbCacheOptions IOptions<CosmosDbCacheOptions>.Value => this;

        public CosmosDbCacheOptions()
        {
            this.PreferredLocations = new List<string>();
        }
    }
}