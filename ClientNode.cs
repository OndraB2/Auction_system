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

            AuctionServerNewTransaction.OnReceiveRegistrations += NewTransactionReceived;

            P2PUnit.Instance.ConnectToBootstrapNode(this.localNode);

            Thread.Sleep(500);  // watiting for connection established

            StartTransactionSimulation();
        }

        public void StartTransactionSimulation()
        {
            new Thread(()=>{SimulateBuying();}).Start();
            new Thread(()=>{SimulateSelling();}).Start();
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

        Transaction? sellTransactions = null;
        public Transaction SellItem(string name, long low, long up)
        {
            Transaction t = new NewAuctionItemTransaction(Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), P2PUnit.Instance.NodeId.NodeId, name, low, up);
            var message = MessageFactory.GetAuctionServerNewTransaction(P2PUnit.Instance.NodeId, P2PUnit.Instance.BootstrapNode, t);
            P2PUnit.Instance.SendMessageToBootstrapNode(message);
            return t;
        }

        Guid? subscribedAuction = null;
        Transaction? lastSubscribedAuction = null;
        double lastPrice = -1;
        public void SubscribeToRandomAuction()
        {
            DataModuleAPI dataModuleAPI = new DataModuleAPI(this);
            var auctions = dataModuleAPI.FindActiveAuctions();
            if(auctions.Count > 0)
            {
                int i = new Random().Next(0, auctions.Count);
                var message = MessageFactory.GetAuctionServerSubscribe(P2PUnit.Instance.NodeId, P2PUnit.Instance.BootstrapNode, auctions[i].AuctionItemId);
                P2PUnit.Instance.SendMessageToBootstrapNode(message);
                subscribedAuction = auctions[i].AuctionItemId;
                lastSubscribedAuction = auctions[i];
            }
        }

        public void BidToAuction(Guid auctionId, double price)
        {
            Transaction t = new NewItemBidTransaction(Guid.NewGuid(), DateTime.Now, auctionId, this.lastSubscribedAuction.AuctionOwnerId, P2PUnit.Instance.NodeId.NodeId, price);
            var message = MessageFactory.GetAuctionServerNewTransaction(P2PUnit.Instance.NodeId, P2PUnit.Instance.BootstrapNode, t);
            P2PUnit.Instance.SendMessageToBootstrapNode(message);
        }

        protected ManualResetEvent SellingResetEvent = new ManualResetEvent(false);
        public void SimulateSelling()
        {
            while(true)
            {
                if(sellTransactions == null)
                {
                    string name = "name"+ new Random().Next();
                    sellTransactions = SellItem(name, 10, 50);
                    SellingResetEvent.WaitOne(60000);
                }
            }
        }

        protected ManualResetEvent BuyingResetEvent = new ManualResetEvent(false);
        public void SimulateBuying()
        {
            while(true)
            {
                if(subscribedAuction == null)
                {
                    SubscribeToRandomAuction();
                    Thread.Sleep(30000);
                }
                if(subscribedAuction != null)
                {
                    if(lastPrice != -1)
                        BidToAuction(subscribedAuction.Value, lastPrice + 5);
                    else
                        BidToAuction(subscribedAuction.Value, new Random().Next(10, 40));
                    BuyingResetEvent.WaitOne(60000);
                }
            }
        }

        List<Transaction> subscribedTransactions = new List<Transaction>();

        protected void NewTransactionReceived(object? sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerNewTransaction)
            {
                var message = sender as AuctionServerNewTransaction;
                if(message.Response)
                {
                    Console.WriteLine("new transaction response received");
                    var transaction = message.Transaction;
                    subscribedTransactions.Add(transaction);
                    if(transaction is NewItemBidTransaction)
                    {
                        Console.WriteLine("new bid transaction");
                        lastPrice = (transaction as NewItemBidTransaction).Amount;
                    }
                    if(transaction is EndOfAuctionTransaction)
                    {
                        Console.WriteLine("new end of auction transaction");
                        if(subscribedAuction!=null)
                        if(subscribedAuction == (transaction as EndOfAuctionTransaction).AuctionItemId)
                        { 
                            lastPrice = -1;
                            subscribedAuction = null;
                            lastSubscribedAuction = null;
                            BuyingResetEvent.Set();
                        }
                        if(sellTransactions != null)
                        if(sellTransactions.AuctionItemId == (transaction as EndOfAuctionTransaction).AuctionItemId)
                        {
                            sellTransactions = null;
                            SellingResetEvent.Set();
                        }
                    }
                }
            }
        }
    }
}