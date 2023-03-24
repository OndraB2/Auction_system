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

            FindNode.OnReceiveRegistrations += FineNodeReceived;

            P2PUnit.Instance.ConnectToBootstrapNode(this.localNode);
        }

        private void FineNodeReceived(object ?sender, EventArgs args)  // RoutingTableReceived
        {
            if(sender != null && sender is FindNode)
            {
                FindNode r = sender as FindNode;
                P2PUnit.Instance.RoutingTable.AddNode(r.Neighbours);
                Console.WriteLine("FindNode recived, content:");
                foreach(var n in r.Neighbours)
                {
                    Console.WriteLine(n);
                }
            }
        }
    }
}