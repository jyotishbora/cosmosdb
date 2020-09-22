using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAS.Audit;

namespace IAS.CosmosDB.Models
{
    public class AuditEventResultSet
    {
        public List<AuditEvent> AuditEvents { get; set; }
        public string ContinuationToken { get; set; }


    }
}
