using BlockChainLedger;
using Kademlia;

namespace AuctionSystem
{
    class ApplicationNode
    {
        protected KademliaNode localNode;
        
        public ApplicationNode()
        {
            //string ipAddress = P2PUnit.Instance.IpAddress;
            //int port = P2PUnit.Instance.Port;
            //localNode = KademliaNode.CreateInstance(ipAddress, port);
            localNode = P2PUnit.Instance.NodeId;
        }

        public virtual void Start()
        {
            P2PUnit.Instance.Start();
            P2PUnit.Instance.RoutingTable.SetApplicationNode(this);
            
            FindValue.OnResponseReceiveRegistrations += FindValueResponseReceived;
            Ping.OnReceiveResponseRegistrations += PingResponseReceived;

            DataModule.dataModuleAPI = new DataModuleAPI(this);
        }

        protected Block? findValueReceived;
        protected ManualResetEvent findValueResetEvent = new ManualResetEvent(false);
        private void FindValueResponseReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is FindValue)
            {
                FindValue findValue = sender as FindValue;
                if(findValue.IsResponse && findValue.DataBlock == null)
                {
                    Console.WriteLine("find value response value not found");
                }
                // store to memory?
                else if(findValue.DataBlock != null)
                {
                    findValueReceived = findValue.DataBlock;
                    DataModule.Instance.Store(findValue.DataBlock, false);
                    findValueResetEvent.Set();
                }

                
            }
        }

        private void PingResponseReceived(object ?sender, EventArgs args) 
        {
            Console.WriteLine("ping response received");
            if(sender != null && sender is Ping)
            {
                Ping ping = sender as Ping;
                receivedPingResponses.Add(ping);
                pingResetEvent.Set();
            }
        }

        private List<Ping> receivedPingResponses = new List<Ping>();

        private ManualResetEvent pingResetEvent = new ManualResetEvent(false);
        public bool SendPing(KademliaNode node)
        {
            P2PUnit.Instance.Send(MessageFactory.GetPing(this.localNode, node));
            
            // int i = 1000;
            // Task.Delay(2000);
            // while(i > 0)
            // {
            //     Ping? response = receivedPingResponses.Find(x => x.SenderNode == node);
            //     if(response != null)
            //     {
            //         receivedPingResponses.Remove(response);
            //         return true;
            //     }
            //     Task.Delay(20);
            //     i--;
            // }

            pingResetEvent.WaitOne(2000);
            Ping? response = receivedPingResponses.Find(x => x.SenderNode.CompareNodeId(node));
            if(response != null)
            {
                receivedPingResponses.Remove(response);
                return true;
            }
            return false;
        }

    

        protected void FindNodeReceived(object ?sender, EventArgs args)  // RoutingTableReceived
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

        public bool SendFindValue(byte[] id, int n = 10)
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