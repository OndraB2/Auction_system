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
    }
}