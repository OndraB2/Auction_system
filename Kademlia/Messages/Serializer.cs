using Newtonsoft.Json;
using System.Text;

namespace Kademlia
{
    class Serializer
    {
        public static byte[] Serialize<T>(T message)
        {
            MessageWrapper<T> wrapper = new MessageWrapper<T>();
            wrapper.Message = message; 
            wrapper.DestinationNode = (message as Message).DestinationNode;
            wrapper.SenderNode = (message as Message).SenderNode;
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
            //string json = Encoding.UTF8.GetString(bytes);
            //MessageWrapper deserialized  = JsonConvert.DeserializeObject<MessageWrapper>(json);
            
            var messageType = Type.GetType(wrapper.MessageType);
            var message = JsonConvert.DeserializeObject(Convert.ToString(wrapper.Message), messageType);

            return message as Message;
        }

        public static MessageWrapper Deserialize(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            MessageWrapper deserialized  = JsonConvert.DeserializeObject<MessageWrapper>(json);
            return deserialized;
            // var messageType = Type.GetType(deserialized.MessageType);
            // var message = JsonConvert.DeserializeObject(Convert.ToString(deserialized.Message), messageType);

            // return message as Message;
        }
    }
}