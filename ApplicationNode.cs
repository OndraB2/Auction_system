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
            
            FindValue.OnResponseReceiveRegistrations += FindValueResponseReceived;
            Ping.OnReceiveResponseRegistrations += PingResponseReceived;
        }

        protected bool findValueReceived;
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
                    findValueReceived = true;
                    DataModule.Instance.Store(findValue.DataBlock);
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
    }
}