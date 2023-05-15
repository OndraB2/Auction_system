using System.Text;
using AuctionSystem;
using BlockChainLedger;

namespace Kademlia
{
    class DataModule
    {
        public static DataModule Instance {
            get {
                if(instance == null)
                {
                    instance = new DataModule();
                }
                return instance;
            }
        }

        private static DataModule? instance;

        public Dictionary<byte[], Block> database;
        private DataModule()
        {
            database = new Dictionary<byte[], Block>(new ByteArrayComparer());
        }

        public static DataModuleAPI? dataModuleAPI;

        private static object _lock = new object();    
        public void Store(Block block, bool checkValidity = true)
        {
            // tmp debugging
            if(checkValidity && block.MinerId.NodeId.SequenceEqual(P2PUnit.Instance.NodeId.NodeId))
            {
                Console.WriteLine("store debugging break");
                return;
            }
            //
            lock(_lock)
            {
                if(!database.ContainsKey(block.Rank))
                {
                    Console.WriteLine("Store Check validity " + checkValidity);
                    if(!checkValidity || (checkValidity && dataModuleAPI.IsBlockValid(block)))  // true || 
                    {
                        
                        database.Add(block.Rank, block);
                        
                        StringBuilder builder = new StringBuilder();
                        foreach(var b in block.Rank)
                        {
                            builder.Append(b);
                            builder.Append('.');
                        }
                        //Console.WriteLine($"saving block " + builder.ToString());
                        PrefixedWriter.WriteLineImprtant($"saving block " + builder.ToString());
                        Console.WriteLine($"---------------------------------------------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine("Block is not valid");
                    }
                }
            }
        }

        public Block? Get(byte[] ValueId)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var b in ValueId)
            {
                builder.Append(b);
                builder.Append('.');
            }
            Console.WriteLine($"finding block " + builder.ToString());
            
            lock(_lock)
            {
                if(database.ContainsKey(ValueId))
                {
                    return database[ValueId];
                }
                Console.WriteLine("block not found");
                return null;
            }
        }

        public byte[] GetLastBlockId()
        {
            lock(_lock)
            {
                if(database.Count > 0)
                {
                    var blockIds = database.Keys.ToList();
                    blockIds.Sort(new ByteListComparer());

                    return blockIds.Last().Clone() as byte[];
                }
                return new byte[20] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            }
        }
    }

    // https://stackoverflow.com/questions/1440392/use-byte-as-key-in-dictionary
    public class ByteArrayComparer : IEqualityComparer<byte[]> {
        public bool Equals(byte[] left, byte[] right) {
            if ( left == null || right == null ) {
            return left == right;
            }
            if ( left.Length != right.Length ) {
            return false;
            }
            for ( int i= 0; i < left.Length; i++) {
            if ( left[i] != right[i] ) {
                return false;
            }
            }
            return true;
        }
        public int GetHashCode(byte[] key) {
            if (key == null)
            throw new ArgumentNullException("key");
            int sum = 0;
            foreach ( byte cur in key ) {
            sum += cur;
            }
            return sum;
        }
    }
}