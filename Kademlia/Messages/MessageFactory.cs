using BlockChainLedger;

namespace Kademlia
{
    static class MessageFactory
    {
        public static Ping GetPing(KademliaNode senderNode, KademliaNode destinationNode)
        {
            return new Ping(senderNode, destinationNode);
        }
        public static Ping GetPingResponse(Ping request)
        {
            var ping = new Ping(request.DestinationNode, request.SenderNode);
            ping.Response = true;
            return ping;
        }

        public static FindNode GetFindNode(KademliaNode senderNode, KademliaNode destinationNode, KademliaNode wantedNode)
        {
            return new FindNode(senderNode, destinationNode, wantedNode);
        }

        public static FindNode GetFindNodeResponse(KademliaNode senderNode, KademliaNode destinationNode, KademliaNode wantedNode, List<KademliaNode> neighbours)
        {
            var message = new FindNode(senderNode, destinationNode, wantedNode);
            message.Neighbours = neighbours;
            return message;
        }

        public static FindNode GetFindNodeResponse(FindNode request, List<KademliaNode> neighbours)
        {
            var message = new FindNode(request.DestinationNode, request.SenderNode, request.WantedNode);
            message.Neighbours = neighbours;
            return message;
        }

        public static Store GetStore(KademliaNode senderNode, KademliaNode destinationNode, Block block)
        {
            return new Store(senderNode, destinationNode, block);
        }

        public static FindValue GetFindValueRequest(KademliaNode senderNode, KademliaNode destinationNode, byte[] valueId)
        {
            return new FindValue(senderNode, destinationNode, valueId);
        }

        public static FindValue GetFindValueResponse(KademliaNode senderNode, KademliaNode destinationNode, Block block)
        {
            var message = new FindValue(senderNode, destinationNode, block.Rank);
            message.DataBlock = block;
            message.IsResponse = true;
            return message;
        }

        public static AuctionServerTransactions GetAuctionServerTransactions(KademliaNode senderNode, KademliaNode destinationNode)
        {
            return new AuctionServerTransactions(senderNode, destinationNode);
        }

        public static AuctionServerTransactions GetAuctionServerTransactionsResponse(AuctionServerTransactions request, List<Transaction> transactions)
        {
            var message = new AuctionServerTransactions(request.DestinationNode, request.SenderNode);
            message.Transactions = transactions;
            message.Response = true;
            return message;
        }

        public static AuctionServerNewTransaction GetAuctionServerNewTransaction(KademliaNode senderNode, KademliaNode destinationNode, Transaction transaction)
        {
            return new AuctionServerNewTransaction(senderNode, destinationNode, transaction);
        }

        public static AuctionServerNewTransaction GetAuctionServerNewTransactionResponse(KademliaNode senderNode, KademliaNode destinationNode, Transaction transaction)
        {
            var message = new AuctionServerNewTransaction(senderNode, destinationNode, transaction);
            message.Response = true;
            return message;
        }

        public static AuctionServerSubscribe GetAuctionServerSubscribe(KademliaNode senderNode, KademliaNode destinationNode, Guid auctionId)
        {
            return new AuctionServerSubscribe(senderNode, destinationNode, auctionId);
        }

        public static AuctionServerAreTransactionsReal GetAuctionServerAreTransactionsReal(KademliaNode senderNode, KademliaNode destinationNode, List<Transaction> transactions)
        {
            return new AuctionServerAreTransactionsReal(senderNode, destinationNode, transactions);
        }

        public static AuctionServerAreTransactionsReal GetAuctionServerAreTransactionsRealResponse(KademliaNode senderNode, KademliaNode destinationNode, List<Transaction> transactions, bool response)
        {
            var message = new AuctionServerAreTransactionsReal(senderNode, destinationNode, transactions);
            message.Real = response;
            message.Response = true;
            return message;
        }
    }
}