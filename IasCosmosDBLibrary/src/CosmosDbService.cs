using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IasCosmosDBLibrary.Abstractions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace IasCosmosDBLibrary
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly ICosmosContainerFactory _containerFactory;


        public string DbId { get; }
        public string ContainerId { get; }

        public CosmosDbService(ICosmosContainerFactory containerFactory, string dbId, string containerId)
        {
            _containerFactory = containerFactory;
            DbId = dbId;
            ContainerId = containerId;
        }


        public async Task<ItemResponse<T>> AddDocumentAsync<T>(T item, string partitionKey)
        {
            var container = _containerFactory.CreateContainer(DbId, ContainerId);
            var response = await container.CreateItemAsync<T>(item, new PartitionKey(partitionKey));
            return response;
        }

        public async Task<(IEnumerable<T> Results, string ContinuationToken)> RetrieveDocumentsAsync<T>(Expression<Func<T, bool>> predicate, int maxRecords = 0, string partitionKey = "", string continuationToken = "")
        {

            var container = _containerFactory.CreateContainer(DbId, ContainerId);

            QueryRequestOptions options = new QueryRequestOptions();
            if (!string.IsNullOrEmpty(partitionKey))
            {
                options.PartitionKey = new PartitionKey(partitionKey);
            }

            if (maxRecords != 0)
            {
                options.MaxItemCount = maxRecords;
            }

            var feed = continuationToken == string.Empty
                ? container.GetItemLinqQueryable<T>(true, null, options).Where(predicate).ToFeedIterator()
                : container.GetItemLinqQueryable<T>(true, continuationToken, options).Where(predicate).ToFeedIterator();

            var feedResponse = await feed.ReadNextAsync();
            var token = feedResponse.ContinuationToken;
            var resultSet = feedResponse.ToList();

            return (resultSet, token);

        }

      
        public async Task<ItemResponse<T>> DeleteDocumentAsync<T>(string partitionKey, string documentId)
        {
            var container = _containerFactory.CreateContainer(DbId, ContainerId);

            var result = await container.DeleteItemAsync<T>(documentId, new PartitionKey(partitionKey));
            return result;
        }
    }
}
