using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Kademlia
{
    class P2PUnit{
        //private ... RoutingTable;
        public KademliaNode NodeId { get; private set;}  // 160 bitu
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
            this.NodeId = KademliaNode.CreateInstance(IpAddress, Port);
            RoutingTable = new RoutingTable(this.NodeId);
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

        public void Send(Message message)
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

        public void ConnectToBootstrapNode(KademliaNode localNode)
        {
            string bootstrapIp;
            int port;
            (bootstrapIp, port) = BootstrapNodeIpAddressApi.GetBootstrapIpAddress();
            KademliaNode bootstrapNode = new KademliaNode(new byte[20], bootstrapIp, port);
            Connect connect = new Connect(localNode, bootstrapNode);
            client.Send(bootstrapIp, port, connect);
            // posle se????
        }

        public void Redirect(MessageWrapper wrapper)
        {
            Send(wrapper);
        }
    }
}