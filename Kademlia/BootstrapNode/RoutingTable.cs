namespace Kademlia
{
    class RoutingTable
    {
        private List<KademliaNode> nodes = new List<KademliaNode>();
        private const int BucketSize = 20;
        private const int NumLevels = 160;

        private List<KademliaNode> []buckets;

        private KademliaNode localNode;
        public RoutingTable(KademliaNode localNode)
        {
            this.localNode = localNode;
            buckets = new List<KademliaNode>[NumLevels];
            for(int i = 0; i < NumLevels; i++)
            {
                buckets[i] = new List<KademliaNode>();
            }
        }

        public void AddNode(List<KademliaNode> nodes)
        {
            foreach(var node in nodes)
            {
                AddNode(node);
            }
        }
        public void AddNode(KademliaNode node)
        {
            int distanceLevel = GetDistanceLevel(node);
            if(buckets[distanceLevel].Count >= BucketSize)
                buckets[distanceLevel].Add(node);
            else
                throw new Exception($"Bucket {distanceLevel} is full");
        }

        public void RemoveNode(KademliaNode node)
        {
            int distanceLevel = GetDistanceLevel(node);
            if(buckets[distanceLevel].Contains(node))
            {
                buckets[distanceLevel].Remove(node);
            }
        }

        private int GetDistanceLevel(KademliaNode node) 
        {
            byte[] XORDistance = this.localNode.CalculateXorDistance(node);
            
            // number of leading zeros in xor distance
            int i;
            for(i=0; i < NumLevels; i++)
            {
                if(((XORDistance[(i/8)] >> (7 - i%8)) & 1) == 1)
                {
                    break;
                }
            }
            return NumLevels - 1 - i;
        }

        public List<KademliaNode> GetClosestNodes(KademliaNode node, int numberOfNodes)
        {
            List<KademliaNode> closestNodes = new List<KademliaNode>();
            int distanceLevel = GetDistanceLevel(node);
            closestNodes.AddRange(buckets[distanceLevel]);
            int i = 1;
            while((closestNodes.Count < numberOfNodes + 1) && i < NumLevels)
            {
                if(distanceLevel - i > 0)
                {
                    closestNodes.AddRange(buckets[distanceLevel - i]);
                }
                if(distanceLevel + i < NumLevels)
                {
                    closestNodes.AddRange(buckets[distanceLevel + i]);
                }
                i++;
            }
            closestNodes.Remove(node);
            return closestNodes.OrderBy(x => x.CalculateXorDistance(node), new ByteListComparer()).Take(numberOfNodes).ToList();
        }
    }
}