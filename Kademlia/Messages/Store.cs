namespace Kademlia
{
    class Store : Message
    {
        public Store(string senderIp, int senderPort) : base(senderIp, senderPort) {}

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Store>(this);
        }
    }
}