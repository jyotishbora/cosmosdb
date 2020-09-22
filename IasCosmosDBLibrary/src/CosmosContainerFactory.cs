using IasCosmosDBLibrary.Abstractions;
using Microsoft.Azure.Cosmos;

namespace IasCosmosDBLibrary
{
    public class CosmosContainerFactory : ICosmosContainerFactory
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosContainerFactory(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public Container CreateContainer(string databaseId, string containerId)
        {
            var db = _cosmosClient.GetDatabase(databaseId);
            var container = db.GetContainer(containerId);
            return container;
        }
    }
}
