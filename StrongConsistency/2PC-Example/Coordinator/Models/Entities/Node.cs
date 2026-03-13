namespace Coordinator.Models.Entities
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<NodeState> NodeStates { get; set; }
    }
}
