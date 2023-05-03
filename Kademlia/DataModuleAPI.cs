using AuctionSystem;
using BlockChainLedger;

namespace Kademlia
{
    class DataModuleAPI
    {
        private ClientNode node;

        public DataModuleAPI(ClientNode node)
        {
            this.node = node;
        }
        public byte[] FindLastBlockId()
        {
            byte[] localLastId = DataModule.Instance.GetLastBlockId();
            do
            {
                localLastId = Block.Increment(localLastId);
            }while(node.FindValue(localLastId));
            return DataModule.Instance.GetLastBlockId();
        }

        public bool IsTransactionAlreadyInBlock(Transaction transaction, int n = 10)
        {
            // test last n blocks
            List<byte[]> lastNBlocksIds = this.GetLastNBlockIds(n);
            foreach(var blockId in lastNBlocksIds)
            {
                Block? block = DataModule.Instance.Get(blockId);
                if(block!=null)
                {
                    foreach(var t in block.Transactions)
                    {
                        if(t.TID == transaction.TID)
                            return true;
                    }
                }
            }
            return false;
        }

        public List<Transaction> FindActiveAuctions(int n = 10)
        {
            List<Transaction> activeAuctions = new List<Transaction>();
            List<byte[]> lastNBlocksIds = this.GetLastNBlockIds(n);
            foreach(var blockId in lastNBlocksIds)
            {
                Block? block = DataModule.Instance.Get(blockId);
                if(block != null)
                {
                    foreach(var t in block.Transactions)
                    {
                        if(t.GetType() != typeof(EndOfAuctionTransaction))
                        {
                            if(!activeAuctions.Any(item => item.AuctionItemId == t.AuctionItemId))
                            {
                                activeAuctions.Add(t);
                            }
                        }
                        else if(t.GetType() == typeof(EndOfAuctionTransaction))
                        {
                            if(activeAuctions.Any(item => item.AuctionItemId == t.AuctionItemId))
                            {
                                activeAuctions.RemoveAll(item => item.AuctionItemId == t.AuctionItemId);
                            }
                        }
                    }
                }
            }
            return activeAuctions;
        }


        

        private List<byte[]> GetLastNBlockIds(int n)
        {
            List<byte[]> lastIds = new List<byte[]>();
            byte[] lastBlockId = FindLastBlockId();
            byte[] minId = new byte[20];
            for(int i = 0; i < n; i++)
            {
                if(lastBlockId.All(x => x == 0))  // end at 0
                {
                    break;
                }
                lastIds.Add(lastBlockId.Clone() as byte[]);
                lastBlockId = Block.Decrement(lastBlockId);
            }
            return lastIds;
        }
    }
}