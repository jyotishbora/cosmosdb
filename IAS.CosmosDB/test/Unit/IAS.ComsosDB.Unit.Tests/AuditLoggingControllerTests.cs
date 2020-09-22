using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using IAS.Audit;
using IAS.CosmosDB.Controllers;
using IAS.CosmosDB.DI;
using IasCosmosDBLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IAS.CosmosDB.Unit.Tests
{
    public class AuditLoggingControllerTests
    {

        [Fact]
        public async Task AddDocumentToContainer_Returns_OK_With_Valid_Document()
        {
            var cosmosService = new Mock<ICosmosDbService>();
            var logger = new Mock<ILogger<AuditLoggingController>>();
            var mockItemResponse = new Mock<ItemResponse<AuditEvent>>();
            var fixture= new Fixture();
            
            mockItemResponse.Setup(x => x.StatusCode)
                .Returns(HttpStatusCode.OK);

            cosmosService.Setup(s => s.AddDocumentAsync(It.IsAny<AuditEvent>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockItemResponse.Object));

            var mockCosmosDbServiceFactory = new Mock<CosmosDbServiceFactory>();
            mockCosmosDbServiceFactory.Setup(_ => _(It.IsAny<string>())).Returns(cosmosService.Object);

            var sut= new AuditLoggingController(mockCosmosDbServiceFactory.Object, logger.Object);
            var auditEvent = fixture.Create<AuditEvent>();
            var result =await sut.AddAuditDocumentToContainer(auditEvent);
            var objectResult = Assert.IsType<ObjectResult>(result);
            objectResult.StatusCode.Should().Be(200);

        }


    }
}
