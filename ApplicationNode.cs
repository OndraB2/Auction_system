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
        }

        private void FindValueResponseReceived(object ?sender, EventArgs args)
        {
            Console.WriteLine("find value response received");
            if(sender != null && sender is FindValue)
            {
                FindValue findValue = sender as FindValue;
                // store to memory?
                if(findValue.DataBlock != null)
                {
                    DataModule.Instance.Store(findValue.DataBlock);
                }
            }
        }
    }
}