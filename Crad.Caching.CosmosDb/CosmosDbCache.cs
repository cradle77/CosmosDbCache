using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crad.Caching.CosmosDb
{
    public class CosmosDbCache : IDistributedCache
    {
        private readonly CosmosDbCacheOptions _options;
        private readonly DocumentClient _client;

        public CosmosDbCache(IOptions<CosmosDbCacheOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;

            _client = new DocumentClient(
                new Uri(_options.ServiceUri), _options.AuthKey);

            _client.ConnectionPolicy.ConnectionProtocol = Protocol.Tcp;
            _client.ConnectionPolicy.ConnectionMode = ConnectionMode.Direct;

            foreach (var location in _options.PreferredLocations)
            {
                _client.ConnectionPolicy.PreferredLocations.Add(location);
            }
        }

        private Uri GetDocumentUri(string key)
        {
            return UriFactory.CreateDocumentUri(
                _options.DatabaseName,
                _options.CollectionName,
                key);
        }

        public byte[] Get(string key)
        {
            return this.GetAsync(key).GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var uri = this.GetDocumentUri(key);

            try
            {
                var result = await _client.ReadDocumentAsync<CosmosDbCacheItem>(uri, new RequestOptions() { });

                return result.Document.ToByteContent();
            }
            catch (DocumentClientException)
            {
                return null;
            }
        }

        public void Refresh(string key)
        {
            this.RefreshAsync(key).GetAwaiter().GetResult();
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var uri = this.GetDocumentUri(key);

            var result = await _client.ReadDocumentAsync<CosmosDbCacheItem>(uri);

            await _client.UpsertDocumentAsync(uri, result.Document);
        }

        public void Remove(string key)
        {
            this.RemoveAsync(key).GetAwaiter().GetResult();
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var uri = this.GetDocumentUri(key);

            await _client.DeleteDocumentAsync(uri);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            this.SetAsync(key, value, options).GetAwaiter().GetResult();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var uri = UriFactory.CreateDocumentCollectionUri(_options.DatabaseName, _options.CollectionName);

            var document = CosmosDbCacheItem.Build(key, (int)options.AbsoluteExpirationRelativeToNow.Value.TotalSeconds, value);

            await _client.UpsertDocumentAsync(uri, document);
        }
    }
}
