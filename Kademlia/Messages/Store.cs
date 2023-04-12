using BlockChainLedger;

namespace Kademlia
{
    class Store : Message
    {
        public Block Block;
        public Store(KademliaNode senderNode, KademliaNode destinationNode, Block block) : base(senderNode, destinationNode) 
        {
            this.Block = block;
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<Store>(this);
        }

        public override void OnReceive()
        {
            Console.WriteLine("Store received");
            DataModule.Instance.Store(Block);
        }
    }
}