using Kademlia;
using BlockChainLedger;
using AuctionSystem;

namespace AuctionServer{
    public class AuctionServer
    {
        private Timer RemoveConfirmedTransactionsTimer;
        public void Start()
        {
            AuctionServerTransactions.OnReceiveRegistrations += this.AuctionServerTransactionsReceived;
            AuctionServerNewTransaction.OnReceiveRegistrations += this.AuctionServerNewTransactionReceived;
            AuctionServerSubscribe.OnReceiveRegistrations += this.AuctionServerSubscribeReceived;
            AuctionServerAreTransactionsReal.OnReceiveRegistrations += this.AuctionServerAreTransactionsRealReceived;
            
            // thread generovani transakci
            // Thread thread = new Thread(() => {
            //     TransactionPool.RemoveConfirmedTransactionFromPool();
            //     Thread.Sleep(60000);
            // });
            // thread.Start();
            RemoveConfirmedTransactionsTimer = new Timer(new TimerCallback(TransactionPool.RemoveConfirmedTransactionFromPool), null, 30000, 30000);
        }

        private void AuctionServerTransactionsReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerTransactions)
            {
                PrefixedWriter.WriteLineImprtant("Miner transactions request received");
                AuctionServerTransactions message = sender as AuctionServerTransactions;
                if(!message.Response)
                {
                    List<Transaction> transactions = TransactionPool.GetTransactions();
                    var responseMessage = MessageFactory.GetAuctionServerTransactionsResponse(message, transactions);
                    P2PUnit.Instance.SendMessageToSpecificNode(responseMessage);
                    PrefixedWriter.WriteLineImprtant($"Sending {transactions.Count} transactions from transactionpool");
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
                    PrefixedWriter.WriteLineImprtant($"New transaction received id - {message.Transaction.TID}, auction id - {message.Transaction.AuctionItemId}");
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
                PrefixedWriter.WriteLineImprtant("subscribe request received " + (sender as AuctionServerSubscribe).AuctionId);
                ActiveAuctions.AttachObserverToAuction((sender as AuctionServerSubscribe).AuctionId, (sender as AuctionServerSubscribe).SenderNode);
            }
        }

        private void AuctionServerAreTransactionsRealReceived(object ?sender, EventArgs args)
        {
            if(sender != null && sender is AuctionServerAreTransactionsReal)
            {
                AuctionServerAreTransactionsReal message = sender as AuctionServerAreTransactionsReal;
                bool AreTransactionsReal = TransactionPool.AreTransactionsReal(message.Transactions);
                if(!AreTransactionsReal)
                    TransactionPool.AddToBlackList(message.SenderNode);
                var response = MessageFactory.GetAuctionServerAreTransactionsRealResponse(message.DestinationNode, message.SenderNode, message.Transactions, AreTransactionsReal);
                P2PUnit.Instance.SendMessageToSpecificNode(response);
            }
        }

    }
}