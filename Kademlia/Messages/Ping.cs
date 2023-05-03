using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Kademlia
{
    class Ping : Message
    {
        public Ping(KademliaNode senderNode, KademliaNode destinationNode) : base(senderNode, destinationNode) {}

        public static event EventHandler ?OnReceiveResponseRegistrations;

        public bool Response = false;
        public override byte[] Serialize()
        {
            return Serializer.Serialize<Ping>(this);
        }

        public override void OnReceive()
        {
            //Console.WriteLine("Ping received");
            if(!Response)
            {
                //Console.WriteLine("Sending ping response");
                Ping pingResponse = MessageFactory.GetPingResponse(this);
                P2PUnit.Instance.Send(pingResponse);
            }
            else
            {
                OnReceiveResponseRegistrations?.Invoke(this, new EventArgs());
            }
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