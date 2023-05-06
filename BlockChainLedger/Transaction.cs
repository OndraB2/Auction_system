using System.Security.Cryptography;

namespace BlockChainLedger{
    public abstract class Transaction
    {
        public Guid TID;
        public DateTime TimeStamp;
        public Guid AuctionItemId;
        public byte[] AuctionOwnerId;

        public Transaction(Guid tid, DateTime timestamp, Guid auctionItemId, byte[] auctionOwnerId)
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
                writer.Write(TID.ToByteArray());
                writer.Write(TimeStamp.ToBinary());
                writer.Write(AuctionItemId.ToByteArray());
                writer.Write(AuctionOwnerId);
                return stream.ToArray();
            }
        }

        protected static readonly SHA256 sha256 = SHA256.Create();
        public abstract byte[] GetHash();
        public virtual void Print()
        {
            System.Console.WriteLine(TID);
            System.Console.WriteLine(TimeStamp);
            System.Console.WriteLine(AuctionItemId);
            System.Console.WriteLine(AuctionOwnerId);
        }
    }
}