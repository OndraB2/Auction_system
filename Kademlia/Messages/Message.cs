namespace Kademlia
{
    abstract class Message
    {
        // hlavicka - odesilatel, ..
        public string senderIp;
        public int senderPort;
        private string className; // pomocna informace pro deserializaci

        // odesilat jako plaintext nebo nejak serializovat na byty - slo by snadno zpet - poznalo by jaky je datovy typ?

        public Message(string senderIp, int senderPort)
        {
            this.senderIp = senderIp;
            this.senderPort = senderPort;
        }

        public abstract byte[] Serialize();
    }
}