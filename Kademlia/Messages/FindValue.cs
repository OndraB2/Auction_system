using BlockChainLedger;

namespace Kademlia
{
    class FindValue : Message
    {
        public byte[] ValueId;

        public Block? DataBlock;

        public static event EventHandler ?OnResponseReceiveRegistrations;

        public FindValue(KademliaNode senderNode, KademliaNode destinationNode, byte[] valueId) : base(senderNode, destinationNode) 
        {
            this.ValueId = valueId;
            DataBlock = null;
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<FindValue>(this);
        }

        public override void OnReceive()
        {
            Console.WriteLine("FindValue received");

            if(DataBlock == null)  // request
            {
                var block = DataModule.Instance.Get(ValueId);
                if(block != null)
                {
                    Console.WriteLine("FindValue sending block");
                    P2PUnit.Instance.Send(MessageFactory.GetFindValueResponse(this.DestinationNode, this.SenderNode, block));
                }
            }
            else  // response
            {
                OnResponseReceiveRegistrations?.Invoke(this, new EventArgs());
            }
        }
    }
}