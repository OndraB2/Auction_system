using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using BlockChainLedger;
using Newtonsoft.Json;

namespace Kademlia
{
    class AuctionServerTransactions : Message
    {
        public List<Transaction> ?Transactions;

        public bool Response = false; 
        public static event EventHandler ?OnReceiveRegistrations;
        
        public AuctionServerTransactions(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) 
        {
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<AuctionServerTransactions>(this);
        }

        public override void OnReceive()
        {
            OnReceiveRegistrations?.Invoke(this, new EventArgs());
        }

        public override byte[] ComputeHash()
        {
            string jsonMessage  = JsonConvert.SerializeObject(new {s = this.SenderNode, res = Response}, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(jsonMessage));
        }
    }
}
