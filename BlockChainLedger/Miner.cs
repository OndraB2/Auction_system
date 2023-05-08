
using AuctionSystem;
using Kademlia;

namespace BlockChainLedger
{
    class Miner
    {
        private ApplicationNode Node;

        private DataModuleAPI dataModuleAPI;
        public Miner(ApplicationNode node)
        {
            Node = node;
            dataModuleAPI = new DataModuleAPI(node);
            AuctionServerTransactions.OnReceiveRegistrations += this.AuctionServerTransactionsReceived;
        }
        public List<Transaction>? LoadTransactions()
        {
            // send
            var message = MessageFactory.GetAuctionServerTransactions(P2PUnit.Instance.NodeId, P2PUnit.Instance.BootstrapNode);
            P2PUnit.Instance.SendMessageToBootstrapNode(message);
            
            AuctionServerTransactionsResetEvent.WaitOne(2000);
            // receive
            if(transactionsToProcess.Count == 0)
                return null;
            List<Transaction> transactions = new List<Transaction>();
            foreach(var t in transactionsToProcess)
            {
                transactions.Add(t);
            }
            transactionsToProcess.Clear();
            return transactions;
        }

        public void MineNewBlock()
        {
            List<Transaction>? transactions = LoadTransactions();
            if(transactions == null)
            {
                Console.WriteLine("No transactions from application server");
                return;
            }
            Console.WriteLine($"transactions from Application server loaded {transactions.Count}");
            Block previousBlock = dataModuleAPI.GetLastBlock();
            if(previousBlock != null)
            {
                Block b = new PowBlock(previousBlock, previousBlock.Difficulty,transactions, P2PUnit.Instance.NodeId);
                b.Mine();
                Node.SendStore(b);
                Console.WriteLine(b);
                Console.WriteLine("Block send to validation ###################################################");
            }

        }

        private ManualResetEvent AuctionServerTransactionsResetEvent = new ManualResetEvent(false);
        private List<Transaction> transactionsToProcess = new List<Transaction>();
        private void AuctionServerTransactionsReceived(object ?sender, EventArgs args)  // RoutingTableReceived
        {
            if(sender != null && sender is AuctionServerTransactions)
            {
                var response = sender as AuctionServerTransactions;
                if(response.Response && response.Transactions != null)
                {
                    foreach(var t in response.Transactions)
                    {
                        transactionsToProcess.Add(t);
                    }
                    AuctionServerTransactionsResetEvent.Set();
                }
            }
        }
    }
}