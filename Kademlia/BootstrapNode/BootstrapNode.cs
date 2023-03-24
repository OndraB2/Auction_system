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

        private List<KademliaNode> GetClosestNeighbours(KademliaNode node)
        {
            var neighbours = P2PUnit.Instance.RoutingTable.GetClosestNodes(node, NumbetOfNeighbors+1);
            neighbours.Remove(node);
            return neighbours;
        }

        private void NewClientConnected(object ?sender, EventArgs args)
        {
            // pridat jeho adresu
            if(sender != null && sender is Connect)
            {
                Connect connect = sender as Connect;
                Console.WriteLine($"New Connection {connect.senderNode}");
                P2PUnit.Instance.RoutingTable.AddNode(connect.senderNode);

                List<KademliaNode> neighbours = GetClosestNeighbours(connect.senderNode);
                // send back
                FindNode response = MessageFactory.GetFindNodeResponse(this.localNode, connect.senderNode, connect.senderNode, neighbours);
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