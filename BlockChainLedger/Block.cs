using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;

namespace BlockChainLedger{
    class Block
    {
        public byte[] Rank {get; set;} = new byte[20];
        public string Hash {get; set;}
        public string HashOfPrevious {get; set;}
        public int Nonce {get; set;}
        public int Difficulty {get; set;}
        public DateTime Timestamp {get; set;}
        public List<Transaction> Transactions {get; set;}

        public Block(int rank, string hashOfPrevious, int difficulty, List<Transaction> transactions)
        {
            var tt = BitConverter.GetBytes(rank);
            BitConverter.GetBytes(rank).Reverse().ToArray().CopyTo(this.Rank, 16);
            this.HashOfPrevious = hashOfPrevious;
            this.Difficulty = difficulty;
            this.Transactions = transactions;
            this.Timestamp = DateTime.Now;
            this.Hash = "";
        }

        public Block(Block previousBlock, int difficulty, List<Transaction> transactions)
        {
            this.Rank = Increment(previousBlock.Rank);
            this.HashOfPrevious = previousBlock.Hash;
            this.Difficulty = difficulty;
            this.Transactions = transactions;
            this.Timestamp = DateTime.Now;
            this.Hash = "";
        }

        // [JsonConstructor]
        // public Block(byte[] Rank, string Hash, string HashOfPrevious, int Nonce, int Difficulty, DateTime Timestamp, List<Transaction> Transactions)
        // {
        //     this.Rank = Rank;
        //     this.Hash = Hash;
        //     this.HashOfPrevious = HashOfPrevious;
        //     this.Nonce = Nonce;
        //     this.Difficulty = Difficulty;
        //     this.Timestamp = Timestamp;
        //     this.Transactions = Transactions;
        // }
        [JsonConstructor]
        public Block()
        {}

        public static byte[] Increment(byte[] arr)
        {
            bool carry = true;
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (carry)
                {
                    carry = false;
                    if (arr[i] == 0xFF)
                    {
                        arr[i] = 0x00;
                        carry = true;
                    }
                    else
                    {
                        arr[i]++;
                    }
                }
            }
            return arr;
        }

        public virtual Block ValidateBlock() { return this;}

        public override string ToString()
        {
            return $"{Rank} {Hash} {HashOfPrevious} {Nonce} {Difficulty} {Timestamp} {Transactions.Count}";
        }
    }
}