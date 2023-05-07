using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    static class ActiveAuctions
    {
        static public List<Auction> AuctionsList = new List<Auction>();
        static public void NewBid(Auction auct, double amount, Byte[] bidderID)
        {
            Auction? auction = GetAuction(auct.AuctionTransaction);     
            if( auction is null )
            {
                System.Console.WriteLine("Not active auction: " + auct.AuctionTransaction.AuctionItemId);
                return;
            }
            auction.NewBid(amount, bidderID);
        }
        static public Auction AddAuction(NewAuctionItemTransaction auctionTransaction)
        {
            Auction auction = new Auction(auctionTransaction, auctionTransaction.GetStartingBid(), auctionTransaction.GetFinalBid());
            AuctionsList.Add(auction);
            TransactionPool.AddTransactionToPool(auctionTransaction);
            System.Console.WriteLine("New Auction: " + auctionTransaction.TID);
            System.Console.WriteLine("StartingBid: " + auctionTransaction.GetStartingBid() + ", FinalBid: " + auctionTransaction.GetFinalBid());

            return auction;
        }
        static public Auction? GetAuction(NewAuctionItemTransaction auctionTransaction)
        {
            return AuctionsList.FirstOrDefault(auction => auction.AuctionTransaction == auctionTransaction);
        }

        static public Auction? GetAuction(Guid auctionItemID)
        {
            return AuctionsList.FirstOrDefault(auction => auction.AuctionTransaction.AuctionItemId == auctionItemID);
        }
        static public bool IsAuctionActive(Auction auction)
        {
            if(GetAuction(auction.AuctionTransaction) is null) 
                return false;
            return true;
        }
        static public void RemoveAuction(NewAuctionItemTransaction auctionTransaction)
        {
            if( AuctionsList.RemoveAll(auction => auction.AuctionTransaction == auctionTransaction) == 1)
            {
                System.Console.WriteLine("Auction ended: " + auctionTransaction.AuctionItemId);
            }
        }
        static public void NewBid(NewItemBidTransaction NIBTransaction)
        {
            Auction? auction = GetAuction(NIBTransaction.AuctionItemId);
            if(auction is not null)
            {
                auction.NewBid(NIBTransaction);
            }
        }
        static public void EndOfAuctionByOwner(EndOfAuctionTransaction EOATransaction)
        {
            Auction? auction = GetAuction(EOATransaction.AuctionItemId);
            if(auction is not null)
            {
                auction.EndOfAuctionByOwner(EOATransaction);
            }
        }
        static public void AttachObserverToAuction(Guid auctionID, KademliaNode kNode)
        {
            Auction? auction = GetAuction(auctionID);
            if(auction is not null)
            {
                auction.AttachNewObserver(new User(kNode));
            }
        }
    }
}