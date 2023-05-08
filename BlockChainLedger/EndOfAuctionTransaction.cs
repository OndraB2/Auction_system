using System.Runtime.Intrinsics.Arm;

namespace BlockChainLedger{
    public class EndOfAuctionTransaction : Transaction
    {
        public byte[] TransactionOwnerId;
        public double Amount;

        public EndOfAuctionTransaction(Guid tid, DateTime timestamp, Guid auctionItemId, byte[] auctionOwnerId, byte[] transactionOwnerId, double amount) : base(tid, timestamp, auctionItemId, auctionOwnerId)
        {
            TransactionOwnerId = transactionOwnerId;
            Amount = amount;
        }

        public override byte[] ToByteArray()
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(base.ToByteArray());
                writer.Write(TransactionOwnerId);
                writer.Write(Amount);
                return stream.ToArray();
            }
        }

        public override byte[] GetHash()
        {
            return sha256.ComputeHash(this.ToByteArray());
        }

        public override void Print()
        {
            base.Print();
            System.Console.WriteLine(TransactionOwnerId);
            System.Console.WriteLine(Amount);
        }
<<<<<<< HEAD
        static public EndOfAuctionTransaction GetRandom(Guid auctionItemId, byte[] auctionOwnerId, double amount, byte[] bidderId)
        {
            Random rand = new Random();
=======
        static public EndOfAuctionTransaction CreateNew(Guid auctionItemId, byte[] auctionOwnerId, double amount, byte[] bidderId)
        {
>>>>>>> master
            Guid tid = Guid.NewGuid();
            DateTime timestamp = DateTime.Now;

            return new EndOfAuctionTransaction(tid, timestamp, auctionItemId, auctionOwnerId, bidderId, amount);
        }
    }
}