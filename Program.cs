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
      P2PWebClientTest();
      //SerializaceTest();
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
      P2PUnit.Instance.Connect();
      Message m = new Ping(1,1);
      
      while(true)
      P2PUnit.Instance.Send(m);
    }

    static void SerializaceTest()
    {
      Message x = new Ping(1, 45646);
      var bytes = x.Serialize();
      Message x2 = Serializer.Deserialize(bytes);
      Console.WriteLine(x.GetType());
      Console.WriteLine(x2.GetType());
    }

  }
}