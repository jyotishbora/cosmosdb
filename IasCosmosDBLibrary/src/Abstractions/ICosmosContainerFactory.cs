using Microsoft.Azure.Cosmos;

namespace IasCosmosDBLibrary.Abstractions
{
    public interface ICosmosContainerFactory
    {
        Container CreateContainer(string db, string container);
    }
}
