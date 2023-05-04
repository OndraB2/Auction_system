using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Kademlia
{
    class FindNode : Message
    {
        public List<KademliaNode> ?Neighbours; // test
        public KademliaNode WantedNode;
        public static event EventHandler ?OnReceiveRegistrations;

        public int NumberOfNeighbours = 3;
        
        public FindNode(KademliaNode senderNode, KademliaNode destinationNode, KademliaNode wantedNode) : base(senderNode, destinationNode) 
        {
            this.WantedNode = wantedNode;
        }

        public override byte[] Serialize()
        {
            return Serializer.Serialize<FindNode>(this);
        }

        public override void OnReceive()
        {
            OnReceiveRegistrations?.Invoke(this, new EventArgs());
        }

        public override byte[] ComputeHash()
        {
            string jsonMessage  = JsonConvert.SerializeObject(new {n = Neighbours, wn = WantedNode}, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(jsonMessage));
        }
    }
}
