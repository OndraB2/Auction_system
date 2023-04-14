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
            AddNode(localNode);
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
            if(distanceLevel >= 0 && distanceLevel < NumLevels)
            {
                if(buckets[distanceLevel].Count <= BucketSize)
                {
                    if(!buckets[distanceLevel].Any(x => x.CompareNodeId(node))) // .Contains(node)
                    {
                        Console.WriteLine("Add node to bucket " + distanceLevel);
                        buckets[distanceLevel].Add(node);
                    }
                }
                else
                    throw new Exception($"Bucket {distanceLevel} is full");
            }
        }

        public bool Contains(byte[] kademliaNode)
        {
            KademliaNode node = new KademliaNode(kademliaNode, "", -1);
            int distanceLevel = GetDistanceLevel(node);
            if(distanceLevel >= 0 && distanceLevel < NumLevels)
            {
                if(!buckets[distanceLevel].Any(x => x.CompareNodeId(node)))
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveNode(KademliaNode node)
        {
            int distanceLevel = GetDistanceLevel(node);
            if(buckets[distanceLevel].Any(x => x.CompareNodeId(node)))
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
            if(i==NumLevels)
                i--;
            return NumLevels - 1 - i;
        }

        public List<KademliaNode> GetClosestNodes(KademliaNode node, int numberOfNodes)
        {
            List<KademliaNode> closestNodes = new List<KademliaNode>();
            int distanceLevel = GetDistanceLevel(node);
            closestNodes.AddRange(buckets[distanceLevel]);
            int i = 1;
            while((closestNodes.Count < numberOfNodes) && i < NumLevels)
            {
                if(distanceLevel - i >= 0)
                {
                    closestNodes.AddRange(buckets[distanceLevel - i]);
                }
                if(distanceLevel + i < NumLevels)
                {
                    closestNodes.AddRange(buckets[distanceLevel + i]);
                }
                i++;
            }
            return closestNodes.OrderBy(x => x.CalculateXorDistance(node), new ByteListComparer()).Take(numberOfNodes).ToList();
        }

        public List<KademliaNode> GetNodeOrClosestNodes(KademliaNode node, int numberOfNodes)
        {
            var nodes = this.GetClosestNodes(node, numberOfNodes);
            if(nodes[0].CompareNodeId(node))  // if is node address is closest node on position 0
            {
                return new List<KademliaNode>(){nodes[0]};
            }
            return nodes;
        }
    }
}