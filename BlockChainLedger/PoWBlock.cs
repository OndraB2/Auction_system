using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

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

        [JsonConstructor]
        public PowBlock() : base()
        {}

        public override Block Mine()
        {
            ComputeNonce();
            return this;
        }

        private void ComputeNonce()
        {
            byte[] transactionsBytes = Encoding.UTF8.GetBytes(GetTransactionsHash());
            int nonce = 0;
            string hash = "";
            string endValue = new string('0', Difficulty);

            do
            {
                nonce++;
                hash = GetHash(ToByteArray(nonce, transactionsBytes));
            }while(!hash.StartsWith(endValue));

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

        
    }
}