namespace Kademlia
{
    abstract class Message
    {
        // hlavicka - odesilatel, ..
        public int senderId;
        public int destinationId;

        // odesilat jako plaintext nebo nejak serializovat na byty - slo by snadno zpet - poznalo by jaky je datovy typ?

        public Message(int senderIp, int destinationId)
        {
            this.senderId = senderIp;
            this.destinationId = destinationId;
        }

        public abstract byte[] Serialize();

        public abstract void OnReceive();

        protected bool IsForMe()
        {
            if(destinationId != P2PUnit.Instance.Id)
            {
                P2PUnit.Instance.Redirect(this);
                return false;
            }
            return true;
        }
    }
}