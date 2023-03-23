using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionSystem
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");
      //PoWBlockTest();
      //P2PWebClientTest();
      //SerializaceTest();
      RoutingTableTest();
    }


    static void PoWBlockTest()
    {
      Block block = new PowBlock(0, "", 4, new List<Transaction>() {new Transaction()});
      List<Block> blockList = new List<Block>();
      block.ValidateBlock();
      blockList.Add(block);
      Console.WriteLine(blockList.Last());
      for(int i = 0; i < 10; i++)
      {
        Block block2 = new PowBlock(blockList.Last(), 4, new List<Transaction>() {new Transaction(), new Transaction()});
        block2.ValidateBlock();
        blockList.Add(block2);
        Console.WriteLine(blockList.Last());
      }
      Console.WriteLine(blockList.Count);
    }

    static void P2PWebClientTest()
    {
      P2PUnit.Instance.Connect(KademliaNode.CreateInstance("",1));
      KademliaNode sender = new KademliaNode(new byte[10], "", 1);
      KademliaNode destination = new KademliaNode(new byte[10], "", 1);
      Message m = new Ping(sender,destination);
      
      while(true)
      P2PUnit.Instance.Send(m);
    }

    static void SerializaceTest()
    {
      KademliaNode sender = new KademliaNode(new byte[10], "", 1);
      KademliaNode destination = new KademliaNode(new byte[10], "", 1);
      Message x = new Ping(sender, destination);
      var bytes = x.Serialize();
      Message x2 = Serializer.Deserialize(bytes);
      Console.WriteLine(x.GetType());
      Console.WriteLine(x2.GetType());
    }

    static void RoutingTableTest()
    {
      BootstrapNode node = new BootstrapNode();
      node.Start();

    }
  }
}