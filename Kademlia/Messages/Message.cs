namespace Kademlia
{
    abstract class Message
    {
        // hlavicka - odesilatel, ..
        public KademliaNode SenderNode;
        public KademliaNode DestinationNode;

        // odesilat jako plaintext nebo nejak serializovat na byty - slo by snadno zpet - poznalo by jaky je datovy typ?

        public Message(KademliaNode senderNode, KademliaNode destinationNode)
        {
            this.SenderNode = senderNode;
            this.DestinationNode = destinationNode;
        }

        public abstract byte[] Serialize();

        public abstract void OnReceive();
    }
}