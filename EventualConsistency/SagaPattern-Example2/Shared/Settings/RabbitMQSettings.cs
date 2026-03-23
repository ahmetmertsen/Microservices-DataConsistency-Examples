using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Settings
{
    public static class RabbitMQSettings
    {
        public const string StateMachineQueue = "state-machine-queue";
        public const string Stock_OrderCreatedEventQueue = "stock-order-created-event-queue";
    }
}
