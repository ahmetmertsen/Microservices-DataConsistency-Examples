using Coordinator.Models.Enums;

namespace Coordinator.Models.Entities
{
    public record NodeState(Guid TransactionId)
    {
        public Guid Id { get; set; }
        public Guid NodeId { get; set; }
        public ReadyType IsReady { get; set; }
        public TransactionState TransactionState { get; set; }

        public Node Node { get; set; }

    }
}
