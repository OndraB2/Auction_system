using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Kademlia;
using Newtonsoft.Json;

namespace BlockChainLedger{
    class Block
    {
        public byte[] Rank {get; set;} = new byte[20];
        public KademliaNode MinerId {get; set;}
        public string Hash {get; set;}
        public string HashOfPrevious {get; set;}
        public int Nonce {get; set;}
        public int Difficulty {get; set;}
        public DateTime Timestamp {get; set;}
        public List<Transaction> Transactions {get; set;}

        public Block(int rank, string hashOfPrevious, int difficulty, List<Transaction> transactions, KademliaNode minerId)
        {
            BitConverter.GetBytes(rank).Reverse().ToArray().CopyTo(this.Rank, 16);
            this.HashOfPrevious = hashOfPrevious;
            this.Difficulty = difficulty;
            this.Transactions = transactions;
            this.Timestamp = DateTime.Now;
            this.Hash = "";
            this.MinerId = minerId;
        }

        public Block(Block previousBlock, int difficulty, List<Transaction> transactions, KademliaNode minerId)
        {
            byte[] rank = previousBlock.Rank.Clone() as byte[];
            this.Rank = Increment(rank);
            this.HashOfPrevious = previousBlock.Hash;
            this.Difficulty = difficulty;
            this.Transactions = transactions;
            this.Timestamp = DateTime.Now;
            this.Hash = "";
            this.MinerId = minerId;
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

        public static byte[] Decrement(byte[] arr)
        {
            bool borrow = true;
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (borrow)
                {
                    borrow = false;
                    if (arr[i] == 0x00)
                    {
                        arr[i] = 0xFF;
                        borrow = true;
                    }
                    else
                    {
                        arr[i]--;
                    }
                }
            }
            return arr;
        }


        public virtual Block Mine() { return this;}

        protected byte[] GetTransactionsHash()
        {
            if(this.Transactions.Count > 0)
                return GetTransactionsHash(0, this.Transactions.Count - 1);  // sha256.ComputeHash(
            return new byte[]{};
        }

        private byte[] GetTransactionsHash(int low, int high)
        {
            if(low == high)
                return this.Transactions[low].GetHash();
            int split = (high - low) / 2 + low;  
            byte[] left = GetTransactionsHash(low, split);
            byte[] right = GetTransactionsHash(split + 1, high);   
            return sha256.ComputeHash(left.Concat(right).ToArray());
        }

        public override string ToString()
        {
            string str = "";
            foreach(var b in Rank)
                str+=b+".";
            return $"{str} - {Hash} {HashOfPrevious} {Nonce} {Difficulty} {Timestamp} {Transactions.Count}";
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

        protected byte[] ToByteArray(int nonce, byte[] transactionsBytes)
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Rank);
                writer.Write(HashOfPrevious);
                writer.Write(Difficulty);
                writer.Write(transactionsBytes);
                writer.Write(nonce);
                writer.Write(MinerId.ToByteArray());
                //writer.Write(Encoding.UTF8.GetBytes(MyString));

                return stream.ToArray();
            }
        }

        public bool IsHashValid()
        {
            string hash = GetHash(ToByteArray(this.Nonce, GetTransactionsHash()));
            return hash == this.Hash;
        }
    }
}