using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IasCosmosDBLibrary.Abstractions;

namespace IAS.CosmosDB.DI
{
    public delegate ICosmosDbService CosmosDbServiceFactory(string container);

}
