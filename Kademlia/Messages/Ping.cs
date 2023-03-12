namespace Kademlia
{
    class Ping : Message
    {
        public Ping(int senderId, int destinationId) : base(senderId, destinationId) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Ping>(this);
        }

        public override void OnReceive()
        {
            if(IsForMe())
            {
                Console.WriteLine("Ping received");
            }
        }
    }
}