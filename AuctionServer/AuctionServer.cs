using Kademlia;
using BlockChainLedger;

namespace AuctionServer{
    public class AuctionServer
    {
        private Timer RemoveConfirmedTransactionsTimer;
        public void Start()
        {
            AuctionServerTransactions.OnReceiveRegistrations += this.AuctionServerTransactionsReceived;
            AuctionServerNewTransaction.OnReceiveRegistrations += this.AuctionServerNewTransactionReceived;
            AuctionServerSubscribe.OnReceiveRegistrations += this.AuctionServerSubscribeReceived;
            
            // thread generovani transakci
            // Thread thread = new Thread(() => {
            //     TransactionPool.RemoveConfirmedTransactionFromPool();
            //     Thread.Sleep(60000);
            // });
            // thread.Start();
            RemoveConfirmedTransactionsTimer = new Timer(new TimerCallback(TransactionPool.RemoveConfirmedTransactionFromPool), null, 60000, 60000);
        }

        private void AuctionServerTransactionsReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerTransactions)
            {
                Console.WriteLine("Transactions request received");
                AuctionServerTransactions message = sender as AuctionServerTransactions;
                if(!message.Response)
                {
                    List<Transaction> transactions = TransactionPool.GetTransactions();
                    var responseMessage = MessageFactory.GetAuctionServerTransactionsResponse(message, transactions);
                    P2PUnit.Instance.SendMessageToSpecificNode(responseMessage);
                }
            }
        }

        private void AuctionServerNewTransactionReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerNewTransaction)
            {
                var message = (sender as AuctionServerNewTransaction);
                if(message.Transaction != null)
                {
                    Console.WriteLine("new transaction received");
                    // kontrola transakce viz ClientNode metoda NewTransactionReceived
                    if(message.Transaction is NewAuctionItemTransaction)
                    {
                        ActiveAuctions.AddAuction(message.Transaction as NewAuctionItemTransaction);
                    }
                    else if(message.Transaction is NewItemBidTransaction)
                    {
                        ActiveAuctions.NewBid(message.Transaction as NewItemBidTransaction);
                    }
                    else if(message.Transaction is EndOfAuctionTransaction)
                    {
                        ActiveAuctions.EndOfAuctionByOwner(message.Transaction as EndOfAuctionTransaction);
                    }
                }
            }
        }

        private void AuctionServerSubscribeReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerSubscribe)
            {
                Console.WriteLine("subscribe request received");
                ActiveAuctions.AttachObserverToAuction((sender as AuctionServerSubscribe).AuctionId, (sender as AuctionServerSubscribe).SenderNode);
            }
        }

    }
}