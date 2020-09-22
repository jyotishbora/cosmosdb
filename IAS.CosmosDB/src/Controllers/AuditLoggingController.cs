using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IAS.Audit;
using IAS.CosmosDB.DI;
using IAS.CosmosDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IAS.CosmosDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLoggingController : ControllerBase
    {
        private readonly CosmosDbServiceFactory _serviceFactory;
        private readonly ILogger<AuditLoggingController> _logger;

        public AuditLoggingController(CosmosDbServiceFactory serviceFactory, ILogger<AuditLoggingController> logger)
        {
            _serviceFactory = serviceFactory;
            _logger = logger;
            
        }

        /// <summary>
        /// This method will add a audit document to configured Cosmos DB container
        /// </summary>
        /// <param name="auditEvent">Audit Event, which wil be serialized to json</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]

        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(201)]
        public async Task<ActionResult> AddAuditDocumentToContainer(AuditEvent auditEvent)
        {
            try
            {
                var service = _serviceFactory.Invoke(Containers.AuditEventContainerId);
                auditEvent.Id ??= Guid.NewGuid().ToString();
                auditEvent.Target.FinalState = auditEvent.Target.FinalState.ToString();
                auditEvent.Target.InitialState = auditEvent.Target.InitialState.ToString();
                var response = await service.AddDocumentAsync(auditEvent, auditEvent.AuditKey);
                return StatusCode((int) response.StatusCode, response.Resource);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error has occurred while writing to cosmos db");
                return StatusCode((int) HttpStatusCode.InternalServerError, e.Message);
            }

        }
        
        /// <summary>
        /// This method queries a document by entity id  passed in the required key field
        /// This method will return all the documents if max count is not specified
        /// </summary>
        /// <param name="key">audit entity id</param>
        /// <param name="maxCount">maximum number of records to be returned in one batch</param>
        /// <param name="continuationToken">If a continuation token is passed, it will use that
        /// reference point to return the next batch of records</param>
        /// <returns>AuditDocuments and Continuation token if any</returns>
        [HttpGet]
        [ProducesResponseType(typeof(AuditEventResultSet), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [Route("documents/entity/{key}")]
        public async Task<ActionResult<AuditEventResultSet>> RetrieveAuditDocumentsByEntity(string key, [FromQuery]int maxCount, [FromQuery] string continuationToken)
        {
            try
            {
                //check for alphanumeric key 
                if (!Regex.IsMatch(key, "^[a-zA-Z0-9]+$"))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Search key must be alphanumeric");
                }
                Expression<Func<AuditEvent, bool>> predicate = _ => true;

                var service = _serviceFactory.Invoke(Containers.AuditEventContainerId);
                var (results, token) = await service.RetrieveDocumentsAsync<AuditEvent>(predicate, maxCount, key, continuationToken);
                return Ok(new AuditEventResultSet
                {
                    AuditEvents = results.ToList(),
                    ContinuationToken = token
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred while retrieving documents from cosmos db");
                return StatusCode((int)HttpStatusCode.InternalServerError,e.Message);
            }
           
        }
        ///<summary>
        ///This method queries a document by a specific user specified by the in the
        /// required userid field. This method will return all the documents if max count is not specified
        /// </summary>
        /// <param name="userid">User id of audit events being queried </param>
        /// <param name="maxCount">maximum number of records to be returned in one batch</param>
        /// <param name="continuationToken">If a continuation token is passed, it will use that
        /// reference point to return the next batch of records</param>
        /// <returns>AuditDocuments and Continuation token if any</returns>
        [HttpGet]
        [ProducesResponseType(typeof(AuditEventResultSet), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [Route("documents/user/{userid}")]
        public async Task<ActionResult<AuditEventResultSet>> RetrieveAuditDocumentsByUser(string userid, [FromQuery] int maxCount, [FromQuery] string continuationToken)
        {
            try
            {
                //check for alphanumeric key 
                if (!Regex.IsMatch(userid, "^[0-9]+$"))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Search key must be alphanumeric");
                }
                Expression<Func<AuditEvent, bool>> predicate = _ => true;

                var service = _serviceFactory.Invoke(Containers.AuditEventByUserContainerId);
                var (results, token) = await service.RetrieveDocumentsAsync<AuditEvent>(predicate, maxCount, userid, continuationToken);
                return Ok(new AuditEventResultSet
                {
                    AuditEvents = results.ToList(),
                    ContinuationToken = token
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred while retrieving documents from cosmos db");
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }

        }


    }

}
