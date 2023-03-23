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

            string bootstrapIp;
            int port;
            (bootstrapIp, port) = BootstrapNodeIpAddressApi.GetBootstrapIpAddress();

            // registrace k prijeti RoutingTableResponse
            RoutingTableResponse.OnReceiveRegistrations += RoutingTableReceived;

            KademliaNode bootstrapNode = new KademliaNode(new byte[20], bootstrapIp, port);
            Connect connect = new Connect(this.localNode, bootstrapNode);
            P2PUnit.Instance.Send(connect);
        }

        private void RoutingTableReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is RoutingTableResponse)
            {
                RoutingTableResponse r = sender as RoutingTableResponse;
                this.routingTable.AddNode(r.neighbours);
            }
        }
    }
}