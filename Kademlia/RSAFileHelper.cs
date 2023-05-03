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
            string xmlString = rsa.ToXmlString(true);
            File.WriteAllText(filePath, xmlString);
        }

        public static RSA LoadRSAFromFile(string filePath)
        {
            string xmlString = File.ReadAllText(filePath);
            RSA rsa = RSA.Create();
            rsa.FromXmlString(xmlString);
            return rsa;
        }
    }
}