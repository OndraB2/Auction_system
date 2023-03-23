namespace Kademlia
{
    class RoutingTableResponse : Message
    {
        public List<KademliaNode> neighbours;
        public static event EventHandler ?OnReceiveRegistrations;
        public RoutingTableResponse(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<RoutingTableResponse>(this);
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
