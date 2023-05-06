namespace BlockChainLedger{
    public class NewAuctionItemTransaction : Transaction
    {
        public string ItemName;
        public double StartingBid;
        public double FinalBid;

        public NewAuctionItemTransaction(Guid tid, DateTime timestamp, Guid auctionItemId, byte[] auctionOwnerId, string itemName, double startingBid, double finalBid) : base(tid, timestamp, auctionItemId, auctionOwnerId)
        {
            ItemName = itemName;
            StartingBid = startingBid;
            FinalBid = finalBid;
        }

        public override byte[] ToByteArray()
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(base.ToByteArray());
                writer.Write(ItemName);
                writer.Write(StartingBid);
                writer.Write(FinalBid);
                return stream.ToArray();
            }
        }
        public override byte[] GetHash()
        {
            return sha256.ComputeHash(this.ToByteArray());
        }
        public double GetStartingBid()
        {
            return StartingBid;
        }
        public double GetFinalBid()
        {
            return FinalBid;
        }
        public override void Print()
        {
            base.Print();
            System.Console.WriteLine(ItemName);
            System.Console.WriteLine("Starting price: " + StartingBid);
            System.Console.WriteLine("Final price: \t" + FinalBid);
        }
        static public NewAuctionItemTransaction GetRandom()
        {
            Random rand = new Random();
            Guid tid = Guid.NewGuid();
            DateTime timestamp = DateTime.Now;
            Guid auctionItemId = Guid.NewGuid();
            byte[] auctionOwnerId = BitConverter.GetBytes(rand.Next());

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string itemName = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[rand.Next(s.Length)]).ToArray());
            // double startingBid = rand.Next(0, 1000000);
            // double finalBid = rand.Next(Convert.ToInt32(startingBid), Int32.MaxValue - Convert.ToInt32(startingBid) * 10 );
            
            double startingBid = 10000;
            double finalBid = rand.Next(Convert.ToInt32(startingBid), 25000 );

            return new NewAuctionItemTransaction(tid, timestamp, auctionItemId, auctionOwnerId, itemName, startingBid, finalBid);
        }
    }
}