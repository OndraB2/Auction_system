using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Kademlia
{
    public class BootstrapNodeIpAddressApi
    {
        public static (string, int) GetBootstrapIpAddress()
        {
            using var client = new HttpClient();
            var task = Task.Run(() => client.GetStringAsync("https://bootstrapnodeipaddressapi.azurewebsites.net/getNodeAddress"));
            task.Wait();
            string ipaddr = task.Result.Substring(task.Result.IndexOf("\"ipAddress\":\"")+"\"ipAddress\":\"".Length);
            ipaddr = ipaddr.Substring(0,ipaddr.IndexOf('\"'));

            var arr = ipaddr.Split(":");
            return (arr[0], Convert.ToInt32(arr[1]));
        }

        public static void SetBootstrapNodeIpAdress(string ipAddress, int port)
        {
            var task = Task.Run(() => SendIpToBootstrapAPIAsync(ipAddress + ":" + port));
            task.Wait();
        }

        private static async Task SendIpToBootstrapAPIAsync()
        {
                var json = "{\"ipAddress\" : \"123.123.222.0\"}";
                var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                // var url = "http://localhost:5269/updateNodeAddress";
                var url = "https://bootstrapnodeipaddressapi.azurewebsites.net//updateNodeAddress";
                using var client = new HttpClient();

                var response = await client.PostAsync(url, data);
                
                string result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
        }
        private static async Task SendIpToBootstrapAPIAsync(string ipAddress)
        {
            var json = $"{{\"ipAddress\" : \"{ipAddress}\" }}";
            var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            // var url = "http://localhost:5269/updateNodeAddress";
            var url = "https://bootstrapnodeipaddressapi.azurewebsites.net//updateNodeAddress";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);
            
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
    }
}