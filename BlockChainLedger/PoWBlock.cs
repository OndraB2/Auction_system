using System.Security.Cryptography;
using System.Text;

namespace BlockChainLedger
{
    class PowBlock : Block
    {
        public PowBlock(int rank, string hashOfPrevious, int difficulty, List<Transaction> transactions) : base(rank, hashOfPrevious, difficulty, transactions)
        {
        }

        public PowBlock(Block previousBlock, int difficulty, List<Transaction> transactions) : base(previousBlock, difficulty, transactions)
        {
        }

        public override Block ValidateBlock()
        {
            ComputeNonce();
            return this;
        }

        private void ComputeNonce()
        {
            byte[] transactionsBytes = TransactionsToByteArray();
            int nonce = 0;
            string hash = "";
            string endValue = new string('0', Difficulty);

            using (SHA256 sha256Hash = SHA256.Create())
            {
                do
                {
                    nonce++;
                    hash = GetHash(sha256Hash, ToByteArray(nonce, transactionsBytes));
                }while(!hash.StartsWith(endValue));
            }

            this.Hash = hash;
            this.Nonce = nonce;
        }

        private byte[] ToByteArray(int nonce, byte[] transactionsBytes)
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Rank);
                writer.Write(HashOfPrevious);
                writer.Write(Difficulty);
                writer.Write(transactionsBytes);
                writer.Write(nonce);
                //writer.Write(Encoding.UTF8.GetBytes(MyString));

                return stream.ToArray();
            }
        }

        private byte[] TransactionsToByteArray()
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                foreach(var transaction in Transactions)
                    writer.Write(transaction.ToByteArray());

                return stream.ToArray();
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, byte[] data)
        {
            byte[] hashData = hashAlgorithm.ComputeHash(data);

            var sBuilder = new StringBuilder();
            // format each as a hexadecimal string.
            for (int i = 0; i < hashData.Length; i++)
            {
                sBuilder.Append(hashData[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}