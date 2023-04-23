using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;


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

        public virtual Block Mine() { return this;}

        protected string GetTransactionsHash()
        {
            if(this.Transactions.Count > 0)
                return GetHash(GetTransactionsHash(0, this.Transactions.Count - 1));
            return "";
        }

        private string GetTransactionsHash(int low, int high)
        {
            if(low == high)
                return this.Transactions[low].GetHash();
            int split = (high - low) / 2 + low;     
            return GetHash(GetTransactionsHash(low, split)) + GetHash(GetTransactionsHash(split + 1, high));
        }

        public override string ToString()
        {
            return $"{Rank} {Hash} {HashOfPrevious} {Nonce} {Difficulty} {Timestamp} {Transactions.Count}";
        }

        private static readonly SHA256 sha256 = SHA256.Create();
        public static string GetHash(byte[] data)
        {
            byte[] hashData = sha256.ComputeHash(data);

            var sBuilder = new StringBuilder();
            // format each as a hexadecimal string.
            for (int i = 0; i < hashData.Length; i++)
            {
                sBuilder.Append(hashData[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public static string GetHash(string data) => GetHash(Encoding.UTF8.GetBytes(data));
    }
}