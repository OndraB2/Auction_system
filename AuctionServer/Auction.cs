using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    public class Auction
    {
        public NewAuctionItemTransaction AuctionTransaction;
        public double HighestBid { get; set; } = 0;
        public Byte[] HighestBidderID = BitConverter.GetBytes(0);
        public double StartingBid;
        public double FinalBid;
        private List<User> observers = new List<User>();

        public Auction(NewAuctionItemTransaction auctionTransaction, double startingBid, double finalBid)
        {
            AuctionTransaction = auctionTransaction;
            HighestBid = StartingBid; // testovací učely
            StartingBid = startingBid;
            FinalBid = finalBid;
        }
        public void AttachNewObserver(User observer){
            observers.Add(observer);		
        }

        public void NotifyAllObserversAboutNewBid(){
            foreach (User observer in observers) {
                observer.UpdateNewBid();
            }
        } 

        public void NotifyAllObserversAboutEndOfAuction(){
            foreach (User observer in observers) {
                observer.UpdateEndOfAcution();
            }
        } 		

        public void GenerateNew()
        {
            AuctionTransaction = NewAuctionItemTransaction.GetRandom();

            HighestBid = 0;
            HighestBidderID = BitConverter.GetBytes(0);
            StartingBid = AuctionTransaction.GetStartingBid();
            FinalBid = AuctionTransaction.GetFinalBid();
            observers = new List<User>();
        }

        public void NewBid(NewItemBidTransaction NIBtransaction)
        {
            if( NIBtransaction.Amount > HighestBid && NIBtransaction.Amount > StartingBid)
            {
                HighestBid = NIBtransaction.Amount;
                HighestBidderID = NIBtransaction.TransactionOwnerId;
                TransactionPool.AddTransactionToPool(NIBtransaction);
                System.Console.WriteLine("\tNew highest bid by: " + new Guid(NIBtransaction.TransactionOwnerId) + ", value: " + NIBtransaction.Amount);
                NotifyAllObserversAboutNewBid();
            }
            if( NIBtransaction.Amount >= HighestBid && NIBtransaction.Amount > FinalBid )
            {
                HighestBid = NIBtransaction.Amount;
                HighestBidderID = NIBtransaction.TransactionOwnerId;
                EndOfAuctionTransaction EOATransaction = EndOfAuctionTransaction.CreateNew(AuctionTransaction.AuctionItemId, AuctionTransaction.AuctionOwnerId, NIBtransaction.Amount, NIBtransaction.TransactionOwnerId);
                EndAuction(EOATransaction);
            }
        }
        public void EndOfAuctionByOwner(EndOfAuctionTransaction EOATransaction)
        {
            if( EOATransaction.TransactionOwnerId == AuctionTransaction.AuctionOwnerId)
            {
                EndAuction(EOATransaction);
            }
        }
        public void EndAuction(EndOfAuctionTransaction EOATransaction)
        {
            TransactionPool.AddTransactionToPool(EOATransaction);    
            System.Console.WriteLine("End, Sold to: " + new Guid(EOATransaction.TransactionOwnerId) + ", sold for: " + EOATransaction.Amount + "\n");
            ActiveAuctions.RemoveAuction(AuctionTransaction);
            NotifyAllObserversAboutEndOfAuction();
        }

        public void NewBid(double amount, Byte[] bidderID)
        {
            if( amount > HighestBid && amount > StartingBid)
            {
                HighestBid = amount;
                HighestBidderID = bidderID;
                NewItemBidTransaction NIBtransaction = NewItemBidTransaction.GetRandom(AuctionTransaction.AuctionItemId, AuctionTransaction.AuctionOwnerId, amount, bidderID);
                TransactionPool.AddTransactionToPool(NIBtransaction);
                System.Console.WriteLine("\tNew highest bid by: " + new Guid(bidderID) + ", value: " + amount);
                NotifyAllObserversAboutNewBid();
            }
            if( amount >= HighestBid && amount > FinalBid )
            {
                HighestBid = amount;
                HighestBidderID = bidderID;
                EndOfAuctionTransaction EOAtransaction = EndOfAuctionTransaction.CreateNew(AuctionTransaction.AuctionItemId, AuctionTransaction.AuctionOwnerId, amount, bidderID);
                TransactionPool.AddTransactionToPool(EOAtransaction);
                System.Console.WriteLine("End, Sold to: " + new Guid(bidderID) + ", sold for: " + amount + "\n");
                ActiveAuctions.RemoveAuction(AuctionTransaction);
                NotifyAllObserversAboutEndOfAuction();
            }
        }
    }
}