using System.Text;
using AuctionSystem;
using Newtonsoft.Json;

namespace Kademlia
{
    public class KademliaNode : IComparable{
        public byte[] NodeId { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public byte[] PublicKey { get; set; }
        
        // public KademliaNode(byte[] nodeId, string ipAddress, int port) {
        //     NodeId = nodeId;
        //     IpAddress = ipAddress;
        //     Port = port;
        // }

        public KademliaNode(byte[] nodeId, string ipAddress, int port, byte[] publicKey = null) {
            NodeId = nodeId;
            IpAddress = ipAddress;
            Port = port;
            PublicKey = publicKey;
        }

        public static KademliaNode CreateInstance(string ipAddress, int port)
        {
            byte[] id = new byte[20];
            Random rnd = new Random();
            rnd.NextBytes(id);
            return new KademliaNode(id, ipAddress, port);
        }

        public static KademliaNode CreateInstance(string ipAddress, int port, byte[] publicKey)
        {
            byte[] id = new byte[20];
            KademliaNode node;
            // if file exists load from file
            if(File.Exists(Program.homeFolder + "kademliaNode.txt"))
            {
                node = LoadFromFile(Program.homeFolder + "kademliaNode.txt");
                node.IpAddress = ipAddress;
                node.Port = port;
                for(int i = 0; i < publicKey.Length; i++)
                {
                    if(publicKey[i] != node.PublicKey[i])
                        throw new Exception("public key incompatibility between files");
                }
            }
            // else generate
            else
            {
                if (!Directory.Exists(Program.homeFolder))
                {
                    Directory.CreateDirectory(Program.homeFolder);
                }  
                Random rnd = new Random();
                rnd.NextBytes(id);
                node = new KademliaNode(id, ipAddress, port, publicKey);
                SaveToFile(Program.homeFolder + "kademliaNode.txt", node);
            }
            return node;
        }

        private static void SaveToFile(string filePath, KademliaNode node)
        {
            string json = JsonConvert.SerializeObject(node, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            File.WriteAllText(filePath, json);
        }

        private static KademliaNode LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            KademliaNode node = JsonConvert.DeserializeObject(json, typeof(KademliaNode), settings) as KademliaNode;
            return node;
        }

        public byte[] ToByteArray()
        {
            using(MemoryStream stream = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Port);
                writer.Write(PublicKey);
                writer.Write(NodeId);
                writer.Write(Encoding.UTF8.GetBytes(IpAddress));

                return stream.ToArray();
            }
        }
        
        public bool CompareNodeId(KademliaNode node)
        {
            return node.NodeId.SequenceEqual(this.NodeId);
        }

        public bool ComparePublicKey(KademliaNode node)
        {
            return node.PublicKey.SequenceEqual(this.PublicKey);
        }

        public byte[] CalculateXorDistance(KademliaNode node)
        {
            byte[] distance = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                distance[i] = (byte)(this.NodeId[i] ^ node.NodeId[i]);
            }
            return distance;
        } 

        public int CompareTo(object? other) => CompareTo(other as KademliaNode);
        private int CompareTo(KademliaNode other)
        {
            for(int i = 0; i < 20; i++)
            {
                if(this.NodeId[i] < other.NodeId[i])
                    return 1;
                if(this.NodeId[i] > other.NodeId[i])
                    return -1;
            }
            return 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach(var b in NodeId)
            {
                builder.Append(b);
                builder.Append('.');
            }
            builder.Append(" - ");
            builder.Append(IpAddress);
            builder.Append(":");
            builder.Append(Port);
            return builder.ToString();
        }
    }




    public class ByteListComparer : IComparer<IList<byte>>  // https://stackoverflow.com/questions/30422655/sorting-list-of-list-of-bytes-or-list-of-byte-arrays
    {
        public int Compare(IList<byte> x, IList<byte> y)
        {
            int result;
            for(int index = 0; index<Math.Min(x.Count, y.Count); index++)
            {
                result = x[index].CompareTo(y[index]);
                if (result != 0) return result;
            }
            return x.Count.CompareTo(y.Count);
        }
    }
}