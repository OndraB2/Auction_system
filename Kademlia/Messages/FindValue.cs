namespace Kademlia
{
    class FindValue : Message
    {
        public FindValue(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<FindValue>(this);
        }

        public override void OnReceive()
        {
            Console.WriteLine("FindValue received");
        }
    }
}