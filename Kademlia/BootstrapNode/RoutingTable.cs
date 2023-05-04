using AuctionSystem;

namespace Kademlia
{
    class RoutingTable
    {
        private List<KademliaNode> nodes = new List<KademliaNode>();
        private const int BucketSize = 20;
        private const int NumLevels = 160;

        public int NumberOfNodes {get; private set;} = 0;

        private List<KademliaNode> []buckets;

        private KademliaNode localNode;
        private ApplicationNode applicationNode;
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

        private Timer PingTimer;
        public void SetApplicationNode(ApplicationNode applicationNode)
        {
            this.applicationNode = applicationNode;

            PingTimer = new Timer(new TimerCallback(TestActiveNodes), null, 10000, 60000);
        }

        private void TestActiveNodes(object ?state)
        {
            List<KademliaNode> ToRemove = new List<KademliaNode>();
            foreach(var bucket in buckets)
            {
                foreach(var node in bucket)
                {
                    if(!this.applicationNode.SendPing(node))
                    {
                        ToRemove.Add(node);
                    }
                }
            }
            foreach(var node in ToRemove)
            {
                RemoveNode(node);
                Console.WriteLine($"Removing Node no ping response {node.ToString()}");
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
            if(Contains(node))
            {
                Console.WriteLine("contains node");
                UpdateNode(node);
            }
            else
            {
                int distanceLevel = GetDistanceLevel(node);
                if(distanceLevel >= 0 && distanceLevel < NumLevels)
                {
                    if(buckets[distanceLevel].Count <= BucketSize)
                    {
                        if(!buckets[distanceLevel].Any(x => x.CompareNodeId(node))) // .Contains(node)
                        {
                            Console.WriteLine("Add node to bucket " + distanceLevel + " - " + node.ToString());
                            buckets[distanceLevel].Add(node);
                            NumberOfNodes++;
                        }
                    }
                    else
                        throw new Exception($"Bucket {distanceLevel} is full");
                }
            }
        }

        private void UpdateNode(KademliaNode node)
        {
            var nodes = GetNodeOrClosestNodes(node);
            if(nodes.Count != 1)
                return;
            KademliaNode nodeToUpdate = nodes.First();
            if(node.CompareTo(nodeToUpdate) == 0)
            {
                for(int i = 0; i < node.PublicKey.Length; i++)
                {
                    if(node.PublicKey[i] != nodeToUpdate.PublicKey[i])
                        return;
                }
                if(nodeToUpdate.IpAddress != node.IpAddress || nodeToUpdate.Port != node.Port)
                {
                    nodeToUpdate.IpAddress = node.IpAddress;
                    nodeToUpdate.Port = node.Port;
                    Console.WriteLine("Node updated " + nodeToUpdate.ToString());
                }
            }
        }

        public bool Contains(byte[] kademliaNode)
        {
            KademliaNode node = new KademliaNode(kademliaNode, "", -1);
            int distanceLevel = GetDistanceLevel(node);
            if(distanceLevel >= 0 && distanceLevel < NumLevels)
            {
                if(buckets[distanceLevel].Any(x => x.CompareNodeId(node)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(KademliaNode node) => Contains(node.NodeId);

        public void RemoveNode(KademliaNode node)
        {
            int distanceLevel = GetDistanceLevel(node);
            if(buckets[distanceLevel].Any(x => x.CompareNodeId(node)))
            {
                buckets[distanceLevel].Remove(node);
                NumberOfNodes--;
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

        public List<KademliaNode> GetClosestNodes(KademliaNode node, int numberOfNodes = 3)
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

        public List<KademliaNode> GetNodeOrClosestNodes(KademliaNode node, int numberOfNodes = 3)
        {
            var nodes = this.GetClosestNodes(node, numberOfNodes);
            if(nodes.Count > 0)
            if(nodes[0].CompareNodeId(node))  // if is node address is closest node on position 0
            {
                return new List<KademliaNode>(){nodes[0]};
            }
            return nodes;
        }
    }
}