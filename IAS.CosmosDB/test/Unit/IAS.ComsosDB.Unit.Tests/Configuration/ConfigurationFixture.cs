using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;

namespace IAS.CosmosDB.Unit.Tests.Configuration
{
    public class ConfigurationFixture : IDisposable
    {
        public IConfiguration Configuration { get; private set; }
        public List<KeyValuePair<string, string>> SettingsKeyValuePairs { get; private set; }

        public ConfigurationFixture()
        {
            var settings = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("CosmosDbConfig:EndpointUri", "https://ias-audit.documents.azure.com:443/"),
                new KeyValuePair<string, string>("CosmosDbConfig:PrimaryKey", "Q4jwmlrdAUkE72hZ3Y6ZZcelTbisjEQYdLnVS9cnTrG70UXRY6790oBq4b4Cpr5BTiEYdlEAVMjvh8l3eiiaFg=="),
                new KeyValuePair<string, string>("CosmosDbConfig:DbId", "ias-audit-logs"),
                new KeyValuePair<string, string>("CosmosDbConfig:ContainerId", "audit-events")
             
            };
            SettingsKeyValuePairs = settings;
            Configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        }

        public void Dispose() => Expression.Empty();

    }
}
