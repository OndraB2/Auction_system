namespace Kademlia
{
    class BootstrapNode
    {
        // start funkce spusteni p2p unit

        // v messages nadefinovat novou zpravu connect to network, a dalsi zpravu pro odpoved
        // z teto zpravy se bude volat metodat teto tridy ktera bude sbirat ip adresy sestavovat kademlia 
        // strom a distribuovat kademlia sousedy

        // v messages pridat nejaky navrhovy vzor observer ktery bude staticky - melo by jit https://refactoring.guru/design-patterns/observer/csharp/example
        // - zatim asi jen pro zpravu connect a do budoucna pro vsechny??

        private List<KademliaNode> nodes = new List<KademliaNode>();

        public void Start()
        {
            string ipAddress = P2PUnit.GetIpAddress();
            int port = P2PUnit.GetPort();
            
            P2PUnit.Instance.Connect(false);
            SendIpToWebserver();

            // registrace k zprave connect
        }

        private void SendIpToWebserver()
        {

        }

        private List<KademliaNode> GetClosestNeighbors(KademliaNode node)
        {
            return nodes.OrderBy(z => z).Take(5).ToList();
        }
    }
}