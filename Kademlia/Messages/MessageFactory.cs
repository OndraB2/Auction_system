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

        public static Store GetStore(KademliaNode senderNode, KademliaNode destinationNode)
        {
            return new Store(senderNode, destinationNode);
        }
    }
}