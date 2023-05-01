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

        // public bool IsEncrypted = false;
        public byte[]? Signature;
    }

    public class MessageWrapper
    {
        public string MessageType { get; set; }
        public object Message { get; set; }
        public KademliaNode SenderNode;
        public KademliaNode DestinationNode;
        // public bool IsEncrypted;
        public byte[]? Signature;
    }
} 




    // public class MessageAndType<T>
    // {
    //     public string MessageType { get { return typeof(T).FullName; } }
    //     public T ?Message { get; set; }
    // }

    // public class MessageWrapper<T>
    // {
    //     public MessageAndType<T> ?MessageAndType;

    //     public KademliaNode ?SenderNode;
    //     public KademliaNode ?DestinationNode;

    //     bool IsEncrypted = false;

    // }

    // public class MessageAndType
    // {
    //     public string MessageType { get; set; }
    //     public object Message { get; set; }
    // }

    // public class MessageWrapper
    // {
    //     public object MessageAndType;
    //     public KademliaNode SenderNode;
    //     public KademliaNode DestinationNode;
    //     public string MessageType => (!IsEncrypted)?(MessageAndType as MessageAndType).MessageType : "";
    //     public object Message => (!IsEncrypted)?(MessageAndType as MessageAndType).Message : null;
    //     bool IsEncrypted = false;
    // }