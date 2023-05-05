using System.Security.Cryptography;

namespace BlockChainLedger{
    abstract class Transaction
    {
        public double TID;
        public DateTime TimeStamp;
        public double AuctionItemId;
        public byte[] AuctionOwnerId;

        public Transaction(double tid, DateTime timestamp, double auctionItemId, byte[] auctionOwnerId)
        {
            TID = tid;
            TimeStamp = timestamp;
            AuctionItemId = auctionItemId;
            AuctionOwnerId = auctionOwnerId;
        }

        public virtual byte[] ToByteArray()
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(TID);
                //writer.Write(TimeStamp.ToBinary());
                writer.Write(AuctionItemId);
                writer.Write(AuctionOwnerId);
                return stream.ToArray();
            }
        }

        protected static readonly SHA256 sha256 = SHA256.Create();
        public abstract byte[] GetHash();
    }
}