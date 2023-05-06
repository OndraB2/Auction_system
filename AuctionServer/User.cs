using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    public class User
    {
        public List<Transaction> Transactions = new List<Transaction>();
        public KademliaNode KNode;

        public User(KademliaNode knode)
        {
            KNode = knode;
        }
    }
}