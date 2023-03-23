namespace Kademlia
{
    class Connect : Message
    {
        public static event EventHandler ?OnReceiveRegistrations;
        public Connect(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Connect>(this);
        }

        public override void OnReceive()
        {
            // cant test if is for me - it is message for bootstrap node while dont know his id
            OnReceiveRegistrations?.Invoke(this, new EventArgs());
        }
    }
}