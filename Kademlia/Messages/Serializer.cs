using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Kademlia
{
    class Serializer
    {
        private static readonly bool EncriptionActivated = true;
        public static byte[] Serialize<T>(T message)
        {
            MessageWrapper<T> wrapper = new MessageWrapper<T>();
            wrapper.DestinationNode = (message as Message).DestinationNode;
            wrapper.SenderNode = (message as Message).SenderNode;
            if(EncriptionActivated && wrapper.MessageType != typeof(Connect).FullName) // && wrapper.DestinationNode.PublicKey != null
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    byte[] hash = (message as Message).ComputeHash();
                    byte[] signature = P2PUnit.Instance.EncryptionKeys.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    wrapper.Signature = signature;
                }
            }
            wrapper.Message = message; 

            string json = JsonConvert.SerializeObject(wrapper, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] Serialize(MessageWrapper wrapper)
        {
            string json = JsonConvert.SerializeObject(wrapper, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return Encoding.UTF8.GetBytes(json);
        }

        public static Message Deserialize(MessageWrapper wrapper)
        {
            Type messageType = Type.GetType(wrapper.MessageType);
            object message;
            
            Console.WriteLine(messageType.ToString());

            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            message = JsonConvert.DeserializeObject(Convert.ToString(wrapper.Message), messageType, settings);
            
            //if(!P2PUnit.Instance.RoutingTable.GetNodeOrClosestNodes(message.SenderNode).First().ComparePublicKey())
            // return;

            if(EncriptionActivated && wrapper.MessageType != typeof(Connect).FullName && P2PUnit.Instance.RoutingTable.NumberOfNodes > 1)
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    //rsa.ImportRSAPublicKey(wrapper.SenderNode.PublicKey, out int _);
                    rsa.ImportRSAPublicKey(P2PUnit.Instance.RoutingTable.GetNodeOrClosestNodes(wrapper.SenderNode).First().PublicKey, out int _);
                    byte[] hash = (message as Message).ComputeHash();
                    bool valid = rsa.VerifyHash(hash, wrapper.Signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    if(!valid)
                        throw new Exception("Invalid message signature");
                }
            }
            return message as Message;
        }

        public static MessageWrapper Deserialize(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            MessageWrapper deserialized = JsonConvert.DeserializeObject<MessageWrapper>(json);
            return deserialized;
            // var messageType = Type.GetType(deserialized.MessageType);
            // var message = JsonConvert.DeserializeObject(Convert.ToString(deserialized.Message), messageType);

            // return message as Message;
        }
    }
}