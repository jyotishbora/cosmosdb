using System.Linq;
using FluentAssertions;
using IAS.CosmosDB.DI;
using IAS.CosmosDB.Unit.Tests.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IAS.CosmosDB.Unit.Tests
{
    public class ServiceCollectionExtensionsTests : IClassFixture<ConfigurationFixture>
    {
        private readonly ConfigurationFixture _configurationFixture;
        public ServiceCollectionExtensionsTests(ConfigurationFixture configurationFixture)
        {
            _configurationFixture = configurationFixture;
        }
        [Fact]
        public void AddCosmosDB_Should_Register_CosmosClient_As_Singleton()
        {
            //Arrange
            var services= new ServiceCollection();
            services.AddOptions();

            //Act
            services.AddCosmosDb(dbOptions =>
            {
                _configurationFixture.Configuration.GetSection("CosmosDbConfig").Bind(dbOptions);
            } );

            //Assert

            var registration = services.FirstOrDefault(s => s.ServiceType.Name.Equals("CosmosClient"));

            registration.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }

        [Fact]
        public void AddCosmosDB_Should_Register_CosmosDbService_With_Configured_Settings()
        {
            //Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            //Act
            services.AddCosmosDb(dbOptions => { _configurationFixture.Configuration.GetSection("CosmosDbConfig").Bind(dbOptions); });
            var provider = services.BuildServiceProvider();
            var cosmosDbServiceFactory = provider.GetService<CosmosDbServiceFactory>();
            var cosmosDbService = cosmosDbServiceFactory.Invoke("audit-events");
            //Assert
            cosmosDbService.DbId.Should().Be("ias-audit-logs");
            cosmosDbService.ContainerId.Should().Be("audit-events");

        }
    }
}
