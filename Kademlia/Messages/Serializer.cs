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
            string json = JsonConvert.SerializeObject(wrapper, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return Encoding.UTF8.GetBytes(json);
        }

        public static Message Deserialize(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            MessageWrapper deserialized  = JsonConvert.DeserializeObject<MessageWrapper>(json);
            
            var messageType = Type.GetType(deserialized.MessageType);
            var message = JsonConvert.DeserializeObject(Convert.ToString(deserialized.Message), messageType);

            return message as Message;
        }
    }
}