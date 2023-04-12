using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionSystem
{
  class Program
  {
    //static ManualResetEvent _quitEvent = new ManualResetEvent(false);
    static void Main(string[] args)
    {
      // Console.CancelKeyPress += (sender, eArgs) => {
      //   _quitEvent.Set();
      //   eArgs.Cancel = true;
      // };
      // _quitEvent.WaitOne();
      string mode = "";
      if(args.Length>1)
        mode = args[1];
      else if(args.Length>0)
        mode = args[0];
      if(mode != "")
      {
        Console.WriteLine($"Running {mode}");
        if(mode=="BootstrapNode")
        {
          BootstrapNode node = new BootstrapNode();
          node.Start();
        }
        else if(mode=="Client")
        {
          ClientNode node = new ClientNode();
          node.Start();
        }
      }
      else
      {
      // Console.WriteLine("Hello World!");
      // //PoWBlockTest();
      // //P2PWebClientTest();
      // //SerializaceTest();
      //   RoutingTableTest();
        ClientNode node = new ClientNode();
        node.Start();
        Block b = new PowBlock(1, "", 3, new List<Transaction>());
        b.ValidateBlock();
        node.Store(b);
        Task.Delay(1000);
        byte[] id = new byte[20];
        id = Block.Increment(id);
        node.FindValue(id);
      }
      Console.ReadLine();
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
      P2PUnit.Instance.Start();
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
      Message x2 = Serializer.Deserialize(Serializer.Deserialize(bytes));
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