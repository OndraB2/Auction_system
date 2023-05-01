using System.Security.Cryptography;
using System.Text;
using BlockChainLedger;
using Newtonsoft.Json;

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

        public override byte[] ComputeHash()
        {
            string jsonMessage  = JsonConvert.SerializeObject(new {s = this.SenderNode, b = Block}, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(jsonMessage));
        }
    }
}