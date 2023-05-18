using System.Security.Cryptography;
using System.Text;
using Kademlia;
using Newtonsoft.Json;

namespace BlockChainLedger
{
    class PoSBlock : Block
    {
        public int CoverMoney;
        public PoSBlock(int rank, string hashOfPrevious, int difficulty, List<Transaction> transactions, KademliaNode minerId) : base(rank, hashOfPrevious, difficulty, transactions, minerId)
        {
        }

        public PoSBlock(Block previousBlock, int difficulty, List<Transaction> transactions, KademliaNode minerId) : base(previousBlock, difficulty, transactions, minerId)
        {
        }

        [JsonConstructor]
        public PoSBlock() : base()
        {}

        public override Block Mine()
        {
            CoverMoney = 50;
            ComputeNonce();
            return this;
        }

        private void ComputeNonce()
        {
            byte[] transactionsBytes = GetTransactionsHash();
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
    }
}