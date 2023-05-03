using System.Transactions;

namespace Kademlia
{
    class DataModuleAPI
    {
        public byte[] FindLastBlockId()
        {
            throw new NotImplementedException();
        }

        public bool TransactionExists(Transaction transaction)
        {
            // projit x predchozich bloku
            throw new NotImplementedException();
        }

        public List<Transaction> FindActiveAuctions()
        {
            throw new NotImplementedException();
        }
    }
}