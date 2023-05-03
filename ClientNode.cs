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

            Thread.Sleep(500);  // watiting for connection established
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
                    if(waitForNewNodes <= 0)
                        findNodeResetEvent.Set();
                }
                else  // is request
                {
                    List<KademliaNode> neighbours = P2PUnit.Instance.RoutingTable.GetNodeOrClosestNodes(findNode.WantedNode, findNode.NumberOfNeighbours);
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

        private ManualResetEvent findNodeResetEvent = new ManualResetEvent(false);
        public void SendFindNode(byte[] id, bool waiting = false)
        {
            var tmpDest = new KademliaNode(id, "", -1);
            P2PUnit.Instance.SendToClosestNeighbours(MessageFactory.GetFindNode(P2PUnit.Instance.NodeId, tmpDest, tmpDest), 3);
            
            if(waiting)
            {
                findNodeResetEvent.WaitOne(2000);
                waitForNewNodes = 0;
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

        public bool FindValue(byte[] id, int n = 10)
        {
            if(!P2PUnit.Instance.RoutingTable.Contains(id))
                SendFindNode(id, true);
            var tmpDest = new KademliaNode(id, "", -1);
            this.findValueReceived = null;
            P2PUnit.Instance.SendToClosestNeighbours(MessageFactory.GetFindValueRequest(P2PUnit.Instance.NodeId, tmpDest, id), 3);

            // if not found at first attempt
            // send find node and try again n times
            for(int i = 0 ; i < n; i++)
            {
                this.findValueResetEvent.WaitOne(2000);
                if(this.findValueReceived == null)
                {
                    Console.WriteLine("FindValue not received finding new nodes and sending again");
                    // var message = MessageFactory.GetFindNode(P2PUnit.Instance.NodeId, tmpDest, tmpDest);
                    // P2PUnit.Instance.SendToClosestNeighbours(message, 3);
                    this.SendFindNode(id, true);
                    P2PUnit.Instance.SendToClosestNeighbours(MessageFactory.GetFindValueRequest(P2PUnit.Instance.NodeId, tmpDest, id), 3);
                }
                else
                {
                    bool sameId = true;
                    for(int j = 0; j < this.findValueReceived.Rank.Length; j++)
                    {
                        if(this.findValueReceived.Rank[j] != id[j])
                        {
                            sameId = false;
                        }
                    }
                    if(sameId)
                        return true;
                    else
                        this.findValueReceived = null;
                }
            }

            return false;
        }

        
    }
}