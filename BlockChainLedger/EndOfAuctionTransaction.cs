using System.Runtime.Intrinsics.Arm;

namespace BlockChainLedger{
    class EndOfAuctionTransaction : Transaction
    {
        public byte[] TransactionOwnerId;
        public double Amount;

        public EndOfAuctionTransaction(double tid, DateTime timestamp, double auctionItemId, byte[] auctionOwnerId, byte[] transactionOwnerId, double amount) : base(tid, timestamp, auctionItemId, auctionOwnerId)
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
    }
}