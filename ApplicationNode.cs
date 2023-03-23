using Kademlia;

namespace AuctionSystem
{
    class ApplicationNode
    {
        protected KademliaNode localNode;
        protected RoutingTable routingTable;

        public ApplicationNode()
        {
            string ipAddress = P2PUnit.GetIpAddress();
            int port = P2PUnit.GetPort();
            localNode = KademliaNode.CreateInstance(ipAddress, port);
            routingTable = new RoutingTable(localNode);
        }

        public virtual void Start()
        {
            P2PUnit.Instance.Connect(this.localNode);
        }


    }
}