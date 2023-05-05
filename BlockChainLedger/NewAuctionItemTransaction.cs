namespace BlockChainLedger{
    class NewAuctionItemTransaction : Transaction
    {
        public string ItemName;
        public double StartingBid;
        public double FinalBid;

        public NewAuctionItemTransaction(double tid, DateTime timestamp, double auctionItemId, byte[] auctionOwnerId, string itemName, double startingBid, double finalBid) : base(tid, timestamp, auctionItemId, auctionOwnerId)
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
    }
}