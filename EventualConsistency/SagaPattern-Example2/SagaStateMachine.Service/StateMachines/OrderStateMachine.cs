using MassTransit;
using SagaStateMachine.Service.StateIstances;
using Shared.OrderEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachine.Service.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateIstance>
    {
        public Event<OrderStartedEvent> OrderStartedEvent { get; set; }

        public OrderStateMachine()
        {
            InstanceState(instance => instance.CurrentState);
        }
    }
}
