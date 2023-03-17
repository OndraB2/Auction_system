namespace Kademlia
{
    abstract class Message
    {
        // hlavicka - odesilatel, ..
        public KademliaNode senderNode;
        public KademliaNode destinationNode;

        // odesilat jako plaintext nebo nejak serializovat na byty - slo by snadno zpet - poznalo by jaky je datovy typ?

        public Message(KademliaNode senderNode, KademliaNode destinationNode)
        {
            this.senderNode = senderNode;
            this.destinationNode = destinationNode;
        }

        public abstract byte[] Serialize();

        public abstract void OnReceive();

        protected bool IsForMe()
        {
            if(P2PUnit.Instance.NodeId.CompareNodeId(destinationNode))
            {
                P2PUnit.Instance.Redirect(this);
                return false;
            }
            return true;
        }
    }
}