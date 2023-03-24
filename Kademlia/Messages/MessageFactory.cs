namespace Kademlia
{
    static class MessageFactory
    {
        public static Ping GetPing(KademliaNode senderNode, KademliaNode destinationNode)
        {
            return new Ping(senderNode, destinationNode);
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

        public static Store GetStore(KademliaNode senderNode, KademliaNode destinationNode)
        {
            return new Store(senderNode, destinationNode);
        }
    }
}