using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.StockEvents
{
    public class StockNotReservedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; }

        public StockNotReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string Message { get; set; }
    }
}
