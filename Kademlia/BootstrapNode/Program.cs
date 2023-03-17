using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace BootstrapNode
{
    // class Program
    // {
    //     // static void Main(string[] args)
    //     // {
    //     //     // How get the IP address of bootstrap node.
    //     //     string ipaddr = BootstrapNodeIpAddressApi.GetBootstrapIpAddress();

    //     //     // How set the IP address of bootstrap node.
    //     //     DemoSetIpAddress();
    //     // }

    //     public static void DemoSetIpAddress()
    //     {
    //         string[] ipaddresses = GetAllLocalIPv4(NetworkInterfaceType.Ethernet);
    //         if (ipaddresses.Length >= 1)
    //         {
    //             Console.WriteLine($"IP: {ipaddresses[0]}");
    //             BootstrapNodeIpAddressApi.SetBootstrapNodeIpAdress(ipaddresses[0]);
    //         }
    //         else
    //         {
    //             Console.WriteLine("no IP address found :(");
    //         }
    //     }

    //     public static string[] GetAllLocalIPv4(NetworkInterfaceType _type)
    //     {
    //         List<string> ipAddrList = new List<string>();
    //         foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
    //         {
    //             if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
    //             {
    //                 foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
    //                 {
    //                     if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
    //                     {
    //                         ipAddrList.Add(ip.Address.ToString());
    //                     }
    //                 }
    //             }
    //         }

    //         // foreach (var item in ipAddrList)
    //         // {
    //         //     Console.WriteLine(item.ToString());
    //         // }
    //         return ipAddrList.ToArray();
    //     }

        
    // }
}