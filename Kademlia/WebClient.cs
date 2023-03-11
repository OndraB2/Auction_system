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

        public void Send(string ipAddressString, int port, string message)
        {
            try
            {
                // Convert the IP address string to an IPAddress object
                IPAddress ipAddress = IPAddress.Parse(ipAddressString);

                // Create an IPEndPoint object with the IP address and port
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

                // Convert the message to a byte array
                byte[] data = Encoding.UTF8.GetBytes(message);

                // Send the message to the endpoint without establishing a connection
                this.client.Send(data, data.Length, endPoint);
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

                    // Convert the received data to a string and display it
                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine("Received UDP data from {0}: {1}", endPoint.ToString(), message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error listening for incoming UDP data: " + ex.Message);
            }
            finally
            {
                // Close the UdpClient object when finished
                this.client.Close();
            }
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