using System;
using BlockChainLedger;

namespace AuctionServer{
    static class ActiveAuctions
    {
        static public List<Auction> AuctionsList = new List<Auction>();
        static public void NewBid(NewAuctionItemTransaction auctionTransaction, double amount, Byte[] bidderID)
        {
            Auction auction = GetAuction(auctionTransaction);     
            if( auction is null )
            {
                System.Console.WriteLine("Not active auction: " + auctionTransaction.AuctionItemId);
                return;
            }
            auction.NewBid(amount, bidderID);
        }
        static public void AddAuction(NewAuctionItemTransaction auctionTransaction)
        {
            Auction auction = new Auction(auctionTransaction, auctionTransaction.GetStartingBid(), auctionTransaction.GetFinalBid());
            AuctionsList.Add(auction);
            System.Console.WriteLine("New Auction: " + auctionTransaction.TID);
            System.Console.WriteLine("StartingBid: " + auctionTransaction.GetStartingBid() + ", FinalBid: " + auctionTransaction.GetFinalBid());
        }
        static public Auction GetAuction(NewAuctionItemTransaction auctionTransaction)
        {
            return AuctionsList.FirstOrDefault(auction => auction.AuctionTransaction == auctionTransaction);
        }
        static public void RemoveAuction(NewAuctionItemTransaction auctionTransaction)
        {
            if( AuctionsList.RemoveAll(auction => auction.AuctionTransaction == auctionTransaction) == 1)
            {
                System.Console.WriteLine("Auction ended: " + auctionTransaction.AuctionItemId);
            }
        }
    }
}