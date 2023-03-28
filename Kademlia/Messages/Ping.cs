namespace Kademlia
{
    class Ping : Message
    {
        public Ping(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Ping>(this);
        }

        public override void OnReceive()
        {
            Console.WriteLine("Ping received");
        }
    }
}