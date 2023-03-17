namespace Kademlia
{
    class Store : Message
    {
        public Store(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Store>(this);
        }

        public override void OnReceive()
        {
            if(IsForMe())
            {
                Console.WriteLine("Store received");
            }
        }
    }
}