using Kademlia;
using BlockChainLedger;

namespace AuctionServer{
    public class AuctionServer
    {
        public void Start()
        {
            AuctionServerTransactions.OnReceiveRegistrations += this.AuctionServerTransactionsReceived;
            AuctionServerNewTransaction.OnReceiveRegistrations += this.AuctionServerNewTransactionReceived;
            AuctionServerSubscribe.OnReceiveRegistrations += this.AuctionServerSubscribeReceived;
            
            // thread generovani transakci
            // Thread thread = new Thread(() => {
            //     TransactionPool.GenerateTraffic();
            // });
            // thread.Start();
        }

        private void AuctionServerTransactionsReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerTransactions)
            {
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
                // kontrola transakce viz ClientNode metoda NewTransactionReceived
            }
        }

        private void AuctionServerSubscribeReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerSubscribe)
            {

            }
        }

    }
}