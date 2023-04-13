using System.Text;
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

        private Dictionary<byte[], Block> database;
        private DataModule()
        {
            database = new Dictionary<byte[], Block>(new ByteArrayComparer());
        }

        private static object _lock = new object();    
        public void Store(Block block)
        {
            lock(_lock)
            {
                if(!database.ContainsKey(block.Rank))
                {

                    StringBuilder builder = new StringBuilder();
                    foreach(var b in block.Rank)
                    {
                        builder.Append(b);
                        builder.Append('.');
                    }
                    Console.WriteLine($"saving block " + builder.ToString());
                    
                    database.Add(block.Rank, block);
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

            if(database.ContainsKey(ValueId))
            {
                return database[ValueId];
            }
            Console.WriteLine("block not found");
            return null;
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