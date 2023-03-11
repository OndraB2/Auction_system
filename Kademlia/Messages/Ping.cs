namespace Kademlia
{
    class Ping : Message
    {
        public Ping(string senderIp, int senderPort) : base(senderIp, senderPort) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Ping>(this);
        }
    }
}