using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionServer{
    public class User : Observer
    {
        public KademliaNode KNode;

        public User(KademliaNode knode)
        {
            KNode = knode;
        }

        public override void UpdateNewBid(Transaction transaction)
        {
            System.Console.WriteLine("update new bid");
            var message = MessageFactory.GetAuctionServerNewTransactionResponse(P2PUnit.Instance.BootstrapNode, KNode, transaction);
            P2PUnit.Instance.SendMessageToSpecificNode(message);
        }

        public override void UpdateEndOfAcution(Transaction transaction)
        {
            System.Console.WriteLine("update end of auction");
            var message = MessageFactory.GetAuctionServerNewTransactionResponse(P2PUnit.Instance.BootstrapNode, KNode, transaction);
            P2PUnit.Instance.SendMessageToSpecificNode(message);
        }

        public void Register(string ipAddress, int port)
        {
            KNode = KademliaNode.CreateInstance(ipAddress, port);
        }

        public void Register(string ipAddress, int port, byte[] publicKey)
        {
            KNode = KademliaNode.CreateInstance(ipAddress, port, publicKey);
        }
    }
}