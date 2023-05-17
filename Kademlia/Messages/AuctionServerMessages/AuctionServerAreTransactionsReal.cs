using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using BlockChainLedger;
using Newtonsoft.Json;

namespace Kademlia
{
    class AuctionServerAreTransactionsReal : Message
    {
        public List<Transaction> Transactions;

        public bool Response = false; 
        public bool Real = false;
        public static event EventHandler ?OnReceiveRegistrations;
        
        public AuctionServerAreTransactionsReal(KademliaNode senderNode, KademliaNode destinationNode, List<Transaction> transactions) : base(senderNode, destinationNode) 
        {
            this.Transactions = transactions;
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<AuctionServerAreTransactionsReal>(this);
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