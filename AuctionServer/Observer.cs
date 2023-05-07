using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    public abstract class Observer 
    {
        public Auction subject;
        public abstract void UpdateNewBid(Transaction transaction);
        public abstract void UpdateEndOfAcution(Transaction transaction);
    }
}