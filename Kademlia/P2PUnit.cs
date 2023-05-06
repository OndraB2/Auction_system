using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using AuctionSystem;

namespace Kademlia
{
    class P2PUnit{
        public KademliaNode NodeId { get; private set;}  // 160 bitu
        public KademliaNode BootstrapNode {get; set;}
        public RSA EncryptionKeys;
        public RoutingTable RoutingTable;
        public string IpAddress {get; private set;}
        public int Port {get; private set;}

        private bool IsConnected = false;

        private WebClient client;

        private Thread ?listener;

        private static P2PUnit? instance = null;
        public static P2PUnit Instance {
            get {
                if(instance == null)
                {
                    instance = new P2PUnit();
                }
                return instance;
            }
        }

        private P2PUnit()
        {
            IpAddress = GetIpAddress();
            Port = GetPort();
            client = new WebClient(Port);
            EncryptionKeys = GetEncryptionKeys();
            this.NodeId = KademliaNode.CreateInstance(IpAddress, Port, EncryptionKeys.ExportRSAPublicKey());
            RoutingTable = new RoutingTable(this.NodeId);
        }

        private static RSA GetEncryptionKeys()
        {
            RSA rsa;
            // load from file if exists
            if(File.Exists(Program.homeFolder + "keys.txt"))
            {
                rsa = RSAFileHelper.LoadRSAFromFile(Program.homeFolder + "keys.txt");
            }
            // or generate
            else
            {
                if (!Directory.Exists(Program.homeFolder))
                {
                    Directory.CreateDirectory(Program.homeFolder);
                }  
                rsa = RSA.Create();
                RSAFileHelper.SaveRSAToFile(Program.homeFolder + "keys.txt", rsa);
            }
            return rsa;
        }

        private string GetIpAddress() => Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();

        private int GetPort() => WebClient.GetFreePort();
        public void Start()
        {
            listener = new Thread(new ThreadStart(client.Listen));
            listener.Start();
            Console.WriteLine(IpAddress);
            Console.WriteLine(Port);
        }

        public void Send(Message message)   // send to neighbour - if not destination than is redirected
        {
            // find best options where send and send
            var destinations = this.RoutingTable.GetNodeOrClosestNodes(message.DestinationNode, 3);
            foreach(var destination in destinations)
            {
                client.Send(destination.IpAddress, destination.Port, message);
            }
        }

        public void Send(MessageWrapper wrapper)
        {
            // find best options where send and send
            var destinations = this.RoutingTable.GetNodeOrClosestNodes(wrapper.DestinationNode, 3);
            foreach(var destination in destinations)
            {
                client.Send(destination.IpAddress, destination.Port, wrapper);
            }
        }

        public void SendToClosestNeighbours(Message message, int count)  // send dirrectly to neighbour
        {
            var destinations = this.RoutingTable.GetNodeOrClosestNodes(message.DestinationNode, count);
            foreach(var destination in destinations)
            {
                message.DestinationNode = destination;
                client.Send(destination.IpAddress, destination.Port, message);
            }
        }

        public void ConnectToBootstrapNode(KademliaNode localNode)
        {
            Console.WriteLine($"my ip {localNode.IpAddress}:{localNode.Port} {localNode.NodeId[0]}.{localNode.NodeId[1]}");
            string bootstrapIp;
            int port;
            (bootstrapIp, port) = BootstrapNodeIpAddressApi.GetBootstrapIpAddress();
            BootstrapNode = new KademliaNode(new byte[20], bootstrapIp, port);
            Connect connect = new Connect(localNode, BootstrapNode);
            connect.CaptchaValidation();
            client.Send(bootstrapIp, port, connect);
        }

        public void SendMessageToBootstrapNode(Message message)
        {
            client.Send(BootstrapNode.IpAddress, BootstrapNode.Port, message);
        }

        public void SendMessageToSpecificNode(Message message)
        {
            client.Send(message.DestinationNode.IpAddress, message.DestinationNode.Port, message);
        }

        public void Redirect(MessageWrapper wrapper)
        {
            Send(wrapper);
        }
    }
}