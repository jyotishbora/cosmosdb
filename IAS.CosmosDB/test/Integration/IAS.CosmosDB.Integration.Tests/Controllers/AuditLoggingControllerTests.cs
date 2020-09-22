using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IAS.CosmosDB.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IAS.CosmosDB.Integration.Tests.Controllers
{
    public class AuditLoggingControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public AuditLoggingControllerTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.BaseAddress = new Uri("http://localhost/api/auditlogging/");
            _httpClient = factory.CreateClient();

        }

        [Fact]
       
        public async Task RetrieveAuditDocumentsByEntity_Returns_Documents_With_Valid_Entity()
        {
            var model = await _httpClient.GetFromJsonAsync<AuditEventResultSet>("documents/entity/MasterDealer14");

            model.AuditEvents.Count.Should().BeGreaterThan(0); 
        }

    }
}
