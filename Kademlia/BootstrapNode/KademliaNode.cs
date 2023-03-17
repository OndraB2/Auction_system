namespace Kademlia
{
    public class KademliaNode : IComparable{
        public byte[] NodeId { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        
        public KademliaNode(byte[] nodeId, string ipAddress, int port) {
            NodeId = nodeId;
            IpAddress = ipAddress;
            Port = port;
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
    }
}