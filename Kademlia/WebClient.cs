using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kademlia
{
    class WebClient
    {
        private readonly UdpClient client;

        private int Port;

        public WebClient(int port)
        {
            // Create a new UdpClient object
            this.client = new UdpClient(port);
            this.Port = port;
        }

        public void Send(string ipAddressString, int port, Message message)
        {
            byte[] data = message.Serialize();
            Send(ipAddressString, port, data);
        }

        public void Send(string ipAddressString, int port, MessageWrapper wrapper)
        {
            byte[] data = Serializer.Serialize(wrapper);
            Send(ipAddressString, port, data);
        }

        public void Send(string ipAddressString, int port, byte[] data)
        {
            try
            {
                // Convert the IP address string to an IPAddress object
                IPAddress ipAddress = IPAddress.Parse(ipAddressString);
                // Create an IPEndPoint object with the IP address and port
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

                // Send the message to the endpoint without establishing a connection
                this.client.Send(data, data.Length, endPoint);

                Console.WriteLine($"send message to {ipAddressString}:{port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending data: " + ex.Message);
            }
        }

        public void Listen()
        {
            try
            {
                Console.WriteLine("Listening for incoming UDP data on port {0}...", Port);

                while (true)
                {
                    // Receive incoming data and store it in a byte array
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
                    byte[] data = this.client.Receive(ref endPoint);

                    //Message message = Serializer.Deserialize(data);
                    MessageWrapper wrapper = Serializer.Deserialize(data);
                    
                    if(IsForMe(wrapper))
                    {
                        try
                        {
                            Message message = Serializer.Deserialize(wrapper);
                            P2PUnit.Instance.RoutingTable.AddNode(message.SenderNode);
                            new Thread(() => message.OnReceive()).Start();
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("Listen Exception: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error listening for incoming UDP data: " + ex.Message);
            }
            finally
            {
                this.client.Close();
            }
        }

        private bool IsForMe(MessageWrapper wrapper)
        {
            if(wrapper.MessageType == typeof(Connect).FullName || wrapper.MessageType == typeof(FindNode).FullName)  // bootstrap node, 
                return true;

            // if(wrapper.MessageType == typeof(FindValue).FullName)
            // {
            //     return true;
            // }
            if(!P2PUnit.Instance.NodeId.CompareNodeId(wrapper.DestinationNode))
            {
                Console.WriteLine($"not for me me-{P2PUnit.Instance.NodeId}  recv {wrapper.DestinationNode}");
                //P2PUnit.Instance.Redirect(wrapper);
                return false;
            }
            return true;
        }

        public static int GetFreePort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public void Close()
        {
            // Close the UdpClient object
            this.client.Close();
        }
    }
}