using AuctionSystem;
using AuctionServer;


namespace Kademlia
{
    class BootstrapNode : ApplicationNode
    {        
        private const int NumbetOfNeighbors = 5;

        public BootstrapNode() : base()
        {}

        private AuctionServer.AuctionServer auctionServer;

        public override void Start()
        {
            base.Start();

            SendIpToWebserver();
            P2PUnit.Instance.BootstrapNode = P2PUnit.Instance.NodeId;
            // registrace k zprave connect
            Connect.OnReceiveRegistrations += NewClientConnected;
            TransactionPool.DataModuleAPIinstance = new DataModuleAPI(this);
            auctionServer = new AuctionServer.AuctionServer();
            auctionServer.Start();
        }

        private void SendIpToWebserver()
        {
            BootstrapNodeIpAddressApi.SetBootstrapNodeIpAdress(localNode.IpAddress, localNode.Port);
        }

        private List<KademliaNode> GetClosestNeighbours(KademliaNode node)
        {
            var neighbours = P2PUnit.Instance.RoutingTable.GetClosestNodes(node, NumbetOfNeighbors+1);
            neighbours.Remove(node);
            return neighbours;
        }

        private void NewClientConnected(object ?sender, EventArgs args)
        {
            if(sender != null && sender is Connect)
            {
                Connect connect = sender as Connect;
                Console.WriteLine($"New Connection {connect.SenderNode}");
                if(!connect.CaptchaCheck())
                {
                    Console.WriteLine("Invalid captcha hash");
                    return;
                }
                P2PUnit.Instance.RoutingTable.AddNode(connect.SenderNode);

                List<KademliaNode> neighbours = GetClosestNeighbours(connect.SenderNode);
                // send back
                FindNode response = MessageFactory.GetFindNodeResponse(this.localNode, connect.SenderNode, connect.SenderNode, neighbours);
                P2PUnit.Instance.Send(response);

                Console.WriteLine("Sending, content:");
                foreach(var n in response.Neighbours)
                {
                    Console.WriteLine(n);
                }
            }
        }
    }
}