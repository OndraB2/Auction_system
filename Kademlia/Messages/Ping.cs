namespace Kademlia
{
    class Ping : Message
    {
        public Ping(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public static event EventHandler ?OnReceiveResponseRegistrations;

        public bool Response = false;
        public override byte[] Serialize()
        {
            return Serializer.Serialize<Ping>(this);
        }

        public override void OnReceive()
        {
            Console.WriteLine("Ping received");
            if(!Response)
            {
                Ping pingResponse = MessageFactory.GetPingResponse(this);
            }
            else
            {
                OnReceiveResponseRegistrations?.Invoke(this, new EventArgs());
            }
        }
    }
}