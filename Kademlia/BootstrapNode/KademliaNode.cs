using System.Text;

namespace Kademlia
{
    public class KademliaNode : IComparable{
        public byte[] NodeId { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public byte[] PublicKey { get; set; }
        
        // public KademliaNode(byte[] nodeId, string ipAddress, int port) {
        //     NodeId = nodeId;
        //     IpAddress = ipAddress;
        //     Port = port;
        // }

        public KademliaNode(byte[] nodeId, string ipAddress, int port, byte[] publicKey = null) {
            NodeId = nodeId;
            IpAddress = ipAddress;
            Port = port;
            PublicKey = publicKey;
        }

        public static KademliaNode CreateInstance(string ipAddress, int port)
        {
            byte[] id = new byte[20];
            Random rnd = new Random();
            rnd.NextBytes(id);
            return new KademliaNode(id, ipAddress, port);
        }

        public static KademliaNode CreateInstance(string ipAddress, int port, byte[] publicKey)
        {
            byte[] id = new byte[20];
            Random rnd = new Random();
            rnd.NextBytes(id);
            return new KademliaNode(id, ipAddress, port, publicKey);
        }
        
        public bool CompareNodeId(KademliaNode node)
        {
            return node.NodeId.SequenceEqual(this.NodeId);
        }

        public byte[] CalculateXorDistance(KademliaNode node)
        {
            byte[] distance = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                distance[i] = (byte)(this.NodeId[i] ^ node.NodeId[i]);
            }
            return distance;
        } 

        public int CompareTo(object? other) => CompareTo(other as KademliaNode);
        private int CompareTo(KademliaNode other)
        {
            for(int i = 0; i < 20; i++)
            {
                if(this.NodeId[i] < other.NodeId[i])
                    return 1;
                if(this.NodeId[i] > other.NodeId[i])
                    return -1;
            }
            return 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach(var b in NodeId)
            {
                builder.Append(b);
                builder.Append('.');
            }
            builder.Append(" - ");
            builder.Append(IpAddress);
            builder.Append(":");
            builder.Append(Port);
            return builder.ToString();
        }
    }




    public class ByteListComparer : IComparer<IList<byte>>  // https://stackoverflow.com/questions/30422655/sorting-list-of-list-of-bytes-or-list-of-byte-arrays
    {
        public int Compare(IList<byte> x, IList<byte> y)
        {
            int result;
            for(int index = 0; index<Math.Min(x.Count, y.Count); index++)
            {
                result = x[index].CompareTo(y[index]);
                if (result != 0) return result;
            }
            return x.Count.CompareTo(y.Count);
        }
    }
}