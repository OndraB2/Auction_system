namespace BlockChainLedger{
    class Transaction
    {


        public byte[] ToByteArray()
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                // TODO zadat promenne

                return stream.ToArray();
            }
        }

        public string GetHash()
        {
            return Block.GetHash("");
        }
    }
}