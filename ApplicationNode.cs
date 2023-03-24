using Kademlia;

namespace AuctionSystem
{
    class ApplicationNode
    {
        protected KademliaNode localNode;
        
        public ApplicationNode()
        {
            string ipAddress = P2PUnit.Instance.IpAddress;
            int port = P2PUnit.Instance.Port;
            localNode = KademliaNode.CreateInstance(ipAddress, port);
        }

        public virtual void Start()
        {
            P2PUnit.Instance.Start();
        }


    }
}