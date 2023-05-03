using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace Kademlia
{
    public static class RSAFileHelper
    {
        public static void SaveRSAToFile(string filePath, RSA rsa)
        {
            RSAParameters publicKeyParameters = rsa.ExportParameters(false);
            RSAParameters privateKeyParameters = rsa.ExportParameters(true);

            string publicKeyJson = JsonSerializer.Serialize(publicKeyParameters);
            string privateKeyJson = JsonSerializer.Serialize(privateKeyParameters);

            File.WriteAllText(filePath, $"{publicKeyJson}\n{privateKeyJson}");
        }

        public static RSA LoadRSAFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            RSAParameters publicKeyParameters = JsonSerializer.Deserialize<RSAParameters>(lines[0]);
            RSAParameters privateKeyParameters = JsonSerializer.Deserialize<RSAParameters>(lines[1]);

            RSA rsa = RSA.Create();
            rsa.ImportParameters(publicKeyParameters);
            rsa.ImportParameters(privateKeyParameters);

            return rsa;
        }
    }
}