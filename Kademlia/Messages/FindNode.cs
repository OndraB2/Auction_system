using System.Text.Json.Serialization;

namespace Kademlia
{
    class FindNode : Message
    {
        public List<KademliaNode> ?Neighbours;
        public KademliaNode ?WantedNode;
        public static event EventHandler ?OnReceiveRegistrations;
        
        // [JsonConstructor]
        // public FindNode(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public FindNode(KademliaNode senderNode, KademliaNode destinationNode, KademliaNode wantedNode) : base(senderNode, destinationNode) 
        {
            this.WantedNode = wantedNode;
        }

        
        // public FindNode(KademliaNode senderNode, KademliaNode destinationNode, KademliaNode wantedNode, List<KademliaNode> neighbours) : base(senderNode, destinationNode)
        // {
        //     this.WantedNode = wantedNode;
        //     this.Neighbours = neighbours;
        // }


        public override byte[] Serialize()
        {
            return Serializer.Serialize<FindNode>(this);
        }

        public override void OnReceive()
        {
            if(IsForMe())
            {
                OnReceiveRegistrations?.Invoke(this, new EventArgs());
            }
        }
    }
}
