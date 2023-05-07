using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using BlockChainLedger;
using Newtonsoft.Json;

namespace Kademlia
{
    class AuctionServerSubscribe : Message
    {
        public Guid AuctionId;
        public static event EventHandler ?OnReceiveRegistrations;
        
        public AuctionServerSubscribe(KademliaNode senderNode, KademliaNode destinationNode, Guid auctionId) : base(senderNode, destinationNode) 
        {
            this.AuctionId = auctionId;
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<AuctionServerSubscribe>(this);
        }

        public override void OnReceive()
        {
            OnReceiveRegistrations?.Invoke(this, new EventArgs());
        }

        public override byte[] ComputeHash()
        {
            string jsonMessage  = JsonConvert.SerializeObject(new {s = this.SenderNode, auid = AuctionId}, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(jsonMessage));
        }
    }
}