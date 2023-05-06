using System;
using BlockChainLedger;
using Kademlia;

namespace AuctionSystem
{
  class Program
  {
    public static string mode;
    public static string homeFolder;
    //static ManualResetEvent _quitEvent = new ManualResetEvent(false);
    static void Main(string[] args)
    {
      // Console.CancelKeyPress += (sender, eArgs) => {
      //   _quitEvent.Set();
      //   eArgs.Cancel = true;
      // };
      // _quitEvent.WaitOne();
      mode = "";
      homeFolder = "./aktualniInstance/";
      string typeOfNode = "";
      if(args.Length>3)
      {
        typeOfNode = args[3];
      }
      if(args.Length>2)
      {
        mode = args[1];
        homeFolder = args[2];
      }
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
          if(typeOfNode == "miner")
          {
            Console.WriteLine("Is miner");
            Miner miner = new Miner(node);
            Mining(miner);
          }
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
        Block b = new PowBlock(1, "", 3, new List<Transaction>(){
          new NewAuctionItemTransaction(1,DateTime.Now, 1, P2PUnit.Instance.NodeId.NodeId,"item 1", 20,50), 
          new NewAuctionItemTransaction(2,DateTime.Now, 2, P2PUnit.Instance.NodeId.NodeId,"item 2", 20,50),
          new NewItemBidTransaction(3,DateTime.Now, 1, P2PUnit.Instance.NodeId.NodeId, P2PUnit.Instance.NodeId.NodeId, 25),
          new EndOfAuctionTransaction(4,DateTime.Now, 1, P2PUnit.Instance.NodeId.NodeId, P2PUnit.Instance.NodeId.NodeId, 25)
          }, P2PUnit.Instance.NodeId);
        b.Mine();
        node.SendStore(b);
        Task.Delay(1000);
        byte[] id = new byte[20];
        id = Block.Increment(id);
        node.SendFindValue(id);
        //var xxxxx = DataModule.Instance.database;

        DataModuleAPI dmapi = new DataModuleAPI(node);
        var ttt = dmapi.FindLastBlockId();
        var tttt = dmapi.IsTransactionAlreadyInBlock(new NewItemBidTransaction(1,DateTime.Now, 1, P2PUnit.Instance.NodeId.NodeId, P2PUnit.Instance.NodeId.NodeId, 25));
        var ttttt = dmapi.FindActiveAuctions();

        // test ping
        var testNode = P2PUnit.Instance.RoutingTable.GetClosestNodes(P2PUnit.Instance.NodeId, 3).Last();
        
        Console.WriteLine(node.SendPing(testNode)? "ping ok" : "ping false");
      }
      Console.ReadLine();
    }

    static async void Mining(Miner miner)
    {
      while(true)
      {
        miner.MineNewBlock();
        await Task.Delay(new Random().Next(500, 10000));
      }
    }


    static void PoWBlockTest()
    {
      // Block block = new PowBlock(0, "", 4, new List<Transaction>() {new Transaction()});
      // List<Block> blockList = new List<Block>();
      // block.Mine();
      // blockList.Add(block);
      // Console.WriteLine(blockList.Last());
      // for(int i = 0; i < 10; i++)
      // {
      //   Block block2 = new PowBlock(blockList.Last(), 4, new List<Transaction>() {new Transaction(), new Transaction()});
      //   block2.Mine();
      //   blockList.Add(block2);
      //   Console.WriteLine(blockList.Last());
      // }
      // Console.WriteLine(blockList.Count);
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