using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Kademlia
{
    class P2PUnit{
        //private ... RoutingTable;
        public int Id { get; private set;}
        private string IpAddress;
        private int Port;

        private bool IsConnected = false;

        private WebClient Client;

        private Thread Listener;

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
            IpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            Port = WebClient.GetFreePort();
            Client = new WebClient(Port);
        }

        public bool Connect()
        {
            // autentizace, pripojeni k centralnimu prvku, obdrzeni a ulozeni routing table, zrizeni listeneru na zpravy v novem vlakne
            
            Id = 1;

            Listener = new Thread(new ThreadStart(Client.Listen));
            Listener.Start();
            Console.WriteLine(IpAddress);
            Console.WriteLine(Port);
            IsConnected = true;
            return IsConnected;
        }

        public void Send(Message message)
        {
            // vypocist na jakou ip dle id v message

            Client.Send(Console.ReadLine(), Convert.ToInt32(Console.ReadLine()), message);
        }

        public void Redirect(Message message)
        {
            //Console.WriteLine(message.GetType());
        }
    }
}