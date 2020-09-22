using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IasCosmosDBLibrary;
using IasCosmosDBLibrary.Abstractions;
using IasCosmosDBLibrary.Configurations;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace IAS.CosmosDB.DI
{
    public static class CosmosDbSetup
    {
        public static void AddCosmosDb(this IServiceCollection collection, Action<CosmosDbOptions> optionsAction)
        {
            var options = new CosmosDbOptions();
            optionsAction.Invoke(options);

            //Create an instance of the Cosmos Client
            var cosmosClient = new CosmosClient(options.EndpointUri, options.PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDbApi" });
            collection.AddSingleton<CosmosClient>(_ => cosmosClient);
            collection.AddScoped<ICosmosContainerFactory, CosmosContainerFactory>();
            collection.AddScoped<CosmosDbServiceFactory>(provider => container=>
            {
                var factory = provider.GetRequiredService<ICosmosContainerFactory>();
                return new CosmosDbService(factory, options.DbId, container);
            });
        }
    }
}
