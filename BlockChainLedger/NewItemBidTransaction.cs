namespace BlockChainLedger{
    public class NewItemBidTransaction : Transaction
    {
        public byte[] TransactionOwnerId;
        public double Amount;

        public NewItemBidTransaction(Guid tid, DateTime timestamp, Guid auctionItemId, byte[] auctionOwnerId, byte[] transactionOwnerId, double amount) : base(tid, timestamp, auctionItemId, auctionOwnerId)
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
        static public NewItemBidTransaction GetRandom(Guid auctionItemId, byte[] auctionOwnerId, double amount, byte[] bidderID)
        {
            Random rand = new Random();
            Guid tid = Guid.NewGuid();
            DateTime timestamp = DateTime.Now;
            System.Console.WriteLine("\tNew bid on auction: " + auctionItemId);
            return new NewItemBidTransaction(tid, timestamp, auctionItemId, auctionOwnerId, bidderID, amount);
        }
    }
}