using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kademlia
{
    // https://stackoverflow.com/questions/38679972/determine-type-during-json-deserialize
    public class MessageWrapper<T>
    {
        public string MessageType { get { return typeof(T).FullName; } }
        public T Message { get; set; }

        public KademliaNode SenderNode;
        public KademliaNode DestinationNode;
    }

    public class MessageWrapper
    {
        public string MessageType { get; set; }
        public object Message { get; set; }
        public KademliaNode SenderNode;
        public KademliaNode DestinationNode;
    }
} 