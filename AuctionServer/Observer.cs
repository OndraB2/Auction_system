using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    public abstract class Observer 
    {
        public Auction subject;
<<<<<<< HEAD
        public abstract void UpdateNewBid();
        public abstract void UpdateEndOfAcution();
=======
        public abstract void UpdateNewBid(Transaction transaction);
        public abstract void UpdateEndOfAcution(Transaction transaction);
>>>>>>> master
    }
}