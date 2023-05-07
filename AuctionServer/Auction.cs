using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    public class Auction
    {
        public NewAuctionItemTransaction AuctionTransaction;
        public double HighestBid { get; set; } = 0;
        public Byte[] HighestBidderID = BitConverter.GetBytes(0);
        double StartingBid;
        double FinalBid;
        private List<KademliaNode> observers = new List<KademliaNode>();

        public Auction(NewAuctionItemTransaction auctionTransaction, double startingBid, double finalBid)
        {
            AuctionTransaction = auctionTransaction;
            HighestBid = StartingBid; // testovací učely
            StartingBid = startingBid;
            FinalBid = finalBid;
        }
        public void AttachNewObserver(KademliaNode observer){
            observers.Add(observer);		
        }

        public void NotifyAllObserversAboutNewBid(){
            foreach (KademliaNode observer in observers) {
                observer.UpdateNewBid();
            }
        } 

        public void NotifyAllObserversAboutEndOfAuction(){
            foreach (KademliaNode observer in observers) {
                observer.UpdateEndOfAcution();
            }
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
                EndOfAuctionTransaction EOAtransaction = EndOfAuctionTransaction.GetRandom(AuctionTransaction.AuctionItemId, AuctionTransaction.AuctionOwnerId, amount, bidderID);
                TransactionPool.AddTransactionToPool(EOAtransaction);
                System.Console.WriteLine("End, Sold to: " + new Guid(bidderID) + ", sold for: " + amount + "\n");
                ActiveAuctions.RemoveAuction(AuctionTransaction);
                NotifyAllObserversAboutEndOfAuction();
            }
        }
    }
}