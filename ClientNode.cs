using Kademlia;
using BlockChainLedger;

namespace AuctionSystem
{
    class ClientNode : ApplicationNode
    {        
        public ClientNode() : base()
        {}

        public override void Start()
        {
            base.Start();

            FindNode.OnReceiveRegistrations += FindNodeReceived;

            P2PUnit.Instance.ConnectToBootstrapNode(this.localNode);
        }

        private void FindNodeReceived(object ?sender, EventArgs args)  // RoutingTableReceived
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
                    waitForNewNodes--;
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

        private int waitForNewNodes = 0;
        public void SendFindNode(byte[] id, bool waiting = false)
        {
            var tmpDest = new KademliaNode(id, "", -1);
            P2PUnit.Instance.SendToClosestNeighbours(MessageFactory.GetFindNode(P2PUnit.Instance.NodeId, tmpDest, tmpDest), 3);
            
            if(waiting)
            {
                waitForNewNodes = 3;
                int i = 1000;
                while(waitForNewNodes > 0 && i > 0)
                {
                    Task.Delay(20);
                    i--;
                }
            }
        }

        public void Store(Block block)
        {
            if(!P2PUnit.Instance.RoutingTable.Contains(block.Rank))
                SendFindNode(block.Rank, true);
            var tmpDest = new KademliaNode(block.Rank, "", -1);
            P2PUnit.Instance.SendToClosestNeighbours(MessageFactory.GetStore(P2PUnit.Instance.NodeId, tmpDest, block), 3);

            // timer and send again
        }

        public void FindValue(byte[] id)
        {
            if(!P2PUnit.Instance.RoutingTable.Contains(id))
                SendFindNode(id, true);
            var tmpDest = new KademliaNode(id, "", -1);
            P2PUnit.Instance.SendToClosestNeighbours(MessageFactory.GetFindValueRequest(P2PUnit.Instance.NodeId, tmpDest, id), 3);
        }

    }
}