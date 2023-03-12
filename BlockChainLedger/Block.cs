namespace BlockChainLedger{
    abstract class Block
    {
        public int Rank {get; protected set;}
        public string Hash {get; protected set;}
        public string HashOfPrevious {get; protected set;}
        public int Nonce {get; protected set;}
        public int Difficulty {get; protected set;}
        public DateTime Timestamp {get; protected set;}
        public List<Transaction> Transactions {get; protected set;}

        public Block(int rank, string hashOfPrevious, int difficulty, List<Transaction> transactions)
        {
            this.Rank = rank;
            this.HashOfPrevious = hashOfPrevious;
            this.Difficulty = difficulty;
            this.Transactions = transactions;
            this.Timestamp = DateTime.Now;
            this.Hash = "";
        }

        public Block(Block previousBlock, int difficulty, List<Transaction> transactions)
        {
            this.Rank = previousBlock.Rank + 1;
            this.HashOfPrevious = previousBlock.Hash;
            this.Difficulty = difficulty;
            this.Transactions = transactions;
            this.Timestamp = DateTime.Now;
            this.Hash = "";
        }

        public abstract Block ValidateBlock();

        public override string ToString()
        {
            return $"{Rank} {Hash} {HashOfPrevious} {Nonce} {Difficulty} {Timestamp} {Transactions.Count}";
        }
    }
}