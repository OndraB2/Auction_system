using System.Text.Json.Serialization;

namespace Kademlia
{
    class FindNode : Message
    {
        public List<KademliaNode> ?Neighbours;
        public KademliaNode WantedNode;
        public static event EventHandler ?OnReceiveRegistrations;

        public int NumberOfNeighbours = 3;
        
        public FindNode(KademliaNode senderNode, KademliaNode destinationNode, KademliaNode wantedNode) : base(senderNode, destinationNode) 
        {
            this.WantedNode = wantedNode;
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<FindNode>(this);
        }

        public override void OnReceive()
        {
            OnReceiveRegistrations?.Invoke(this, new EventArgs());
        }
    }
}
