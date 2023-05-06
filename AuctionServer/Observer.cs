using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    public abstract class Observer 
    {
        public Auction subject;
        public abstract void UpdateNewBid();
        public abstract void UpdateEndOfAcution();
    }
}