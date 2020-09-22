using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace WebStore.ChangeFeedFunctions
{
    public static class ReplicateAuditEventsFunction
    {
        private static readonly Lazy<CosmosClient> _cosmosClient =
            new Lazy<CosmosClient>(() =>
            {
                var connStr = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
                return new CosmosClient(connStr);
            });

        [FunctionName("ReplicateAuditEventsFunction")]
        public static async Task Run([CosmosDBTrigger(
                databaseName: "ias-audit-logs",
                collectionName: "audit-events",
                ConnectionStringSetting = "CosmosDBConnectionString",
                LeaseCollectionName = "lease",
                LeaseCollectionPrefix = "ReplicateAuditEvents",
                StartFromBeginning = true)]
            IReadOnlyList<Document> documents, ILogger log)
        {

            var container = _cosmosClient.Value.GetContainer("ias-audit-logs", "audit-events-by-userid");
            foreach (var document in documents)
            {
                if (document.TimeToLive == null)
                {
                    await container.UpsertItemAsync(document);
                    log.LogInformation($"Document with id {document.Id} inserted in audit-events-by-userid container");
                }
                else
                {
                    var item = document.GetPropertyValue<string>("UserId");
                    await container.DeleteItemAsync<Document>(document.Id, new Microsoft.Azure.Cosmos.PartitionKey(item));
                    log.LogWarning($"Document with id {document.Id} deleted from audit-events-by-userid container");
                }
            }

        }
    }
}
