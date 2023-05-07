using System;
using BlockChainLedger;
using Kademlia;
using AuctionServer;

namespace AuctionClient{
    public class AuctionSeller : User
    {
        public AuctionSeller(KademliaNode kNode) : base(kNode) {}
        public Auction CreateAuction()
        {
            NewAuctionItemTransaction auction = NewAuctionItemTransaction.GetRandom(KNode.NodeId);
            TransactionPool.AddTransactionToPool(auction);
            return ActiveAuctions.AddAuction(auction);
        }

        // public KademliaNode KNode;

        // public User(KademliaNode knode)
        // {
        //     KNode = knode;
        // }

        // public override void UpdateNewBid()
        // {
        //     System.Console.WriteLine("update new bid");
        // }

        // public override void UpdateEndOfAcution()
        // {
        //     System.Console.WriteLine("update end of auction");
        // }

        // public void Register(string ipAddress, int port)
        // {
        //     KNode = KademliaNode.CreateInstance(ipAddress, port);
        // }

        // public void Register(string ipAddress, int port, byte[] publicKey)
        // {
        //     KNode = KademliaNode.CreateInstance(ipAddress, port, publicKey);
        // }
    }
}