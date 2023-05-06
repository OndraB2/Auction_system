using Kademlia;
using BlockChainLedger;

namespace AuctionSystem
{
    class ClientNode : ApplicationNode
    {        
        public ClientNode() : base()
        {}

        public override void Start()
        {
            base.Start();

            FindNode.OnReceiveRegistrations += FindNodeReceived;

            P2PUnit.Instance.ConnectToBootstrapNode(this.localNode);

            Thread.Sleep(500);  // watiting for connection established
        }

        private bool FirstFindNodeResponse = true;

        protected override void FindNodeReceived(object? sender, EventArgs args)
        {
            base.FindNodeReceived(sender, args);
            // save bootstrap node
            if(FirstFindNodeResponse)
            if(sender != null && sender is FindNode)
            {
                FindNode findNode = sender as FindNode;
                if(findNode.Neighbours != null)
                {
                    P2PUnit.Instance.BootstrapNode = findNode.SenderNode;
                }
                FirstFindNodeResponse = false;
            }
        }
    }
}