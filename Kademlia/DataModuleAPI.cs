using AuctionSystem;
using BlockChainLedger;

namespace Kademlia
{
    class DataModuleAPI
    {
        private ApplicationNode node;

        public DataModuleAPI(ApplicationNode node)
        {
            this.node = node;
        }

        public byte[] FindLastBlockId()
        {
            byte[] localLastId = DataModule.Instance.GetLastBlockId();
            do
            {
                localLastId = Block.Increment(localLastId);
            }while(node.SendFindValue(localLastId, 4));
            Console.WriteLine("found last block Id");
            return DataModule.Instance.GetLastBlockId();
        }

        public bool IsTransactionAlreadyInBlock(Transaction transaction, int n = 10, List<byte[]>? lastNBlocksIds = null)
        {
            // test last n blocks
            if(lastNBlocksIds == null)
                lastNBlocksIds = this.GetLastNBlockIds(n);
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

        public bool IsTransactionAlreadyInBlock(List<Transaction> transactions, int n = 10)
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
                        foreach(var transaction in transactions)
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

        public List<Transaction> FindFinishedAuctions(int n = 10)
        {
            List<Transaction> finishedAuctions = new List<Transaction>();
            List<byte[]> lastNBlocksIds = this.GetLastNBlockIds(n);
            foreach(var blockId in lastNBlocksIds)
            {
                Block? block = DataModule.Instance.Get(blockId);
                if(block != null)
                {
                    foreach(var t in block.Transactions)
                    {
                        if(t.GetType() == typeof(EndOfAuctionTransaction))
                        {
                            if(!finishedAuctions.Any(item => item.AuctionItemId == t.AuctionItemId))
                            {
                                finishedAuctions.Add(t);
                            }
                        }
                    }
                }
            }
            return finishedAuctions;
        }

        private static object _lock = new object();  
        public bool IsBlockValid(Block block)
        {
            //lock(_lock)
            //{
                // check hash
                if(!block.IsHashValid())
                {
                    Console.WriteLine("hash invalid");
                    return false;
                }

                // check block not exist
                byte[] lastBlockId = FindLastBlockId();
                if(lastBlockId.SequenceEqual(block.Rank))
                {
                    Console.WriteLine("Block with same id already exists");
                    return false;
                }

                // check transactions not in existing blocks
                Console.WriteLine("Checking previous blocks start");
                if(IsTransactionAlreadyInBlock(block.Transactions))
                {
                    Console.WriteLine("transaction already in block");
                    return false;
                }
                Console.WriteLine("Checking previous blocks end");
                return true;
            //}
        }

        public Block GetLastBlock()
        {
            byte[] lastBlockId = FindLastBlockId();
            if(!IsFirstBlock(lastBlockId))
                return DataModule.Instance.Get(lastBlockId);
            else return new Block(0, "", 5, new List<Transaction>(), P2PUnit.Instance.NodeId);
        }

        public bool AreTransactionsReal(List<Transaction> transactions) => this.node.AreTansactionsReal(transactions);

        private bool IsFirstBlock(byte[] lastBlockId)
        {
            for(int i = 0; i < lastBlockId.Count(); i++)
            {
                if(lastBlockId[i] != 0)
                    return false;
            }
            return true;
        }

        public List<byte[]> GetLastNBlockIds(int n)
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