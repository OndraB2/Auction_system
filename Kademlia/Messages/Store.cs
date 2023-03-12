namespace Kademlia
{
    class Store : Message
    {
        public Store(int senderId, int destinationId) : base(senderId, destinationId) {}

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