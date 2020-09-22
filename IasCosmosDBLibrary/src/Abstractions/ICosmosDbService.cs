using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace IasCosmosDBLibrary.Abstractions
{
    public interface ICosmosDbService
    {
        string DbId { get; }
        string ContainerId { get; }
        Task<ItemResponse<T>> AddDocumentAsync<T>(T item, string partitionKey);
        Task<(IEnumerable<T> Results, string ContinuationToken)> RetrieveDocumentsAsync<T>(Expression<Func<T, bool>> predicate, int maxRecords, string partitionKey, string continuationToken);
        Task<ItemResponse<T>> DeleteDocumentAsync<T>(string partitionKey, string documentId);

    }
}
