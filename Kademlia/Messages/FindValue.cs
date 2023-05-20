using System.Security.Cryptography;
using System.Text;
using AuctionSystem;
using BlockChainLedger;
using Newtonsoft.Json;

namespace Kademlia
{
    class FindValue : Message
    {
        public byte[] ValueId;

        public Block? DataBlock;

        public bool IsResponse;

        public static event EventHandler ?OnResponseReceiveRegistrations;

        public FindValue(KademliaNode senderNode, KademliaNode destinationNode, byte[] valueId) : base(senderNode, destinationNode) 
        {
            this.ValueId = valueId;
            DataBlock = null;
            IsResponse = false;
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<FindValue>(this);
        }

        public override void OnReceive()
        {
            if(!IsResponse)  // request
            {
                Console.WriteLine("FindValue request received");
                var block = DataModule.Instance.Get(ValueId);
                if(block != null)
                {
                    PrefixedWriter.WriteLineImprtant("FindValue sending block");
                    P2PUnit.Instance.Send(MessageFactory.GetFindValueResponse(this.DestinationNode, this.SenderNode, block));
                }
            }
            else  // response
            {
                Console.WriteLine("FindValue response received");
                //PrefixedWriter.WriteLineImprtant("FindValue response received");
                OnResponseReceiveRegistrations?.Invoke(this, new EventArgs());
            }
        }

        public override byte[] ComputeHash()
        {
            string jsonMessage  = JsonConvert.SerializeObject(new {v = ValueId, db = DataBlock, res = IsResponse}, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(jsonMessage));
        }
    }
}