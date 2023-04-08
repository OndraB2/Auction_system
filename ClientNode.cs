using Kademlia;

namespace AuctionSystem
{
    class ClientNode : ApplicationNode
    {        
        public ClientNode() : base()
        {}

        public override void Start()
        {
            base.Start();

            FindNode.OnReceiveRegistrations += FineNodeReceived;

            P2PUnit.Instance.ConnectToBootstrapNode(this.localNode);
        }

        private void FineNodeReceived(object ?sender, EventArgs args)  // RoutingTableReceived
        {
            if(sender != null && sender is FindNode)
            {
                FindNode findNode = sender as FindNode;
                if(findNode.Neighbours != null)  // is response with neighbours
                {
                    P2PUnit.Instance.RoutingTable.AddNode(findNode.Neighbours);
                    Console.WriteLine("FindNode response recived, content:");
                    foreach(var n in findNode.Neighbours)
                    {
                        Console.WriteLine(n);
                    }
                }
                else  // is request
                {
                    List<KademliaNode> neighbours = P2PUnit.Instance.RoutingTable.GetClosestNodes(findNode.WantedNode, findNode.NumberOfNeighbours);
                    // send back
                    FindNode response = MessageFactory.GetFindNodeResponse(findNode, neighbours);
                    P2PUnit.Instance.Send(response);
                    Console.WriteLine("FindNode request recived, sending:");
                    foreach(var n in neighbours)
                    {
                        Console.WriteLine(n);
                    }
                }
            }
        }
    }
}