namespace Coordinator.Models.Entities
{
    public class Node(string name)
    {
        public Guid Id { get; set; }
        public ICollection<NodeState> NodeStates { get; set; }
    }
}
