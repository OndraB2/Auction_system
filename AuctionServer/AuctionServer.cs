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
            Thread thread = new Thread(() => {
                TransactionPool.RemoveConfirmedTransactionFromPool();
                Thread.Sleep(60000);
            });
            thread.Start();
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
                if((sender as AuctionServerNewTransaction).Transaction is NewAuctionItemTransaction)
                {
                    ActiveAuctions.AddAuction(sender as NewAuctionItemTransaction);
                }
                else if((sender as AuctionServerNewTransaction).Transaction is NewItemBidTransaction)
                {
                    ActiveAuctions.NewBid(sender as NewItemBidTransaction);
                }
                else if((sender as AuctionServerNewTransaction).Transaction is EndOfAuctionTransaction)
                {
                    ActiveAuctions.EndOfAuctionByOwner(sender as EndOfAuctionTransaction);
                }
            }
        }

        private void AuctionServerSubscribeReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerSubscribe)
            {
                ActiveAuctions.AttachObserverToAuction((sender as AuctionServerSubscribe).AuctionId, (sender as AuctionServerSubscribe).SenderNode);
            }
        }

    }
}