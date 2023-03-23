using AuctionSystem;


namespace Kademlia
{
    class BootstrapNode : ApplicationNode
    {
        // start funkce spusteni p2p unit

        // v messages nadefinovat novou zpravu connect to network, a dalsi zpravu pro odpoved
        // z teto zpravy se bude volat metodat teto tridy ktera bude sbirat ip adresy sestavovat kademlia 
        // strom a distribuovat kademlia sousedy

        // v messages pridat nejaky navrhovy vzor observer ktery bude staticky - melo by jit https://refactoring.guru/design-patterns/observer/csharp/example
        // - zatim asi jen pro zpravu connect a do budoucna pro vsechny??
        
        private const int NumbetOfNeighbors = 5;

        public BootstrapNode() : base()
        {}

        public override void Start()
        {
            base.Start();

            SendIpToWebserver();
            // registrace k zprave connect
            Connect.OnReceiveRegistrations += NewClientConnected;
            
            // test add node
            //routingTable.AddNode(KademliaNode.CreateInstance("",2));

        }

        private void SendIpToWebserver()
        {
            BootstrapNodeIpAddressApi.SetBootstrapNodeIpAdress(localNode.IpAddress, localNode.Port);
        }

        private List<KademliaNode> GetClosestNeighbors(KademliaNode node)
        {
            return routingTable.GetClosestNodes(node, NumbetOfNeighbors);
        }

        private void NewClientConnected(object ?sender, EventArgs args)
        {
            // pridat jeho adresu
            if(sender != null && sender is Connect)
            {
                Connect connect = sender as Connect;
                routingTable.AddNode(connect.senderNode);

                List<KademliaNode> neighbors = GetClosestNeighbors(connect.senderNode);
                // send back
                RoutingTableResponse response = new RoutingTableResponse(this.localNode, connect.senderNode);
                P2PUnit.Instance.Send(response);
            }
        }
    }
}