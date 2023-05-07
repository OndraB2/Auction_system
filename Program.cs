using System;
using System.Text;
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
        Console.SetOut(new PrefixedWriter());

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
        
        Console.SetOut(new PrefixedWriter());
      // Console.WriteLine("Hello World!");
      // //PoWBlockTest();
      // //P2PWebClientTest();
      // //SerializaceTest();
      //   RoutingTableTest();
        ClientNode node = new ClientNode();
        node.Start();
        var testTransaction = new NewItemBidTransaction(Guid.NewGuid(),DateTime.Now, Guid.NewGuid(), P2PUnit.Instance.NodeId.NodeId, P2PUnit.Instance.NodeId.NodeId, 25);
        // Block b = new PowBlock(1, "", 3, new List<Transaction>(){ 
        //   new NewAuctionItemTransaction(Guid.NewGuid(),DateTime.Now, Guid.NewGuid(), P2PUnit.Instance.NodeId.NodeId,"item 1", 20,50),
        //   new NewAuctionItemTransaction(Guid.NewGuid(),DateTime.Now, Guid.NewGuid(), P2PUnit.Instance.NodeId.NodeId,"item 2", 20,50),
        //   testTransaction,
        //   new EndOfAuctionTransaction(Guid.NewGuid(),DateTime.Now, Guid.NewGuid(), P2PUnit.Instance.NodeId.NodeId, P2PUnit.Instance.NodeId.NodeId, 25)
        //   }, P2PUnit.Instance.NodeId);
        // b.Mine();
        // node.SendStore(b);
        Task.Delay(1000);
        byte[] id = new byte[20];
        id = Block.Increment(id);
        node.SendFindValue(id);
        //var xxxxx = DataModule.Instance.database;

        DataModuleAPI dmapi = new DataModuleAPI(node);
        var ttt = dmapi.FindLastBlockId();
        var tttt = dmapi.IsTransactionAlreadyInBlock(testTransaction);
        var ttttt = dmapi.FindActiveAuctions();

        var data = DataModule.Instance.database;

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
        await Task.Delay(new Random().Next(15000, 25000));
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

  class PrefixedWriter : TextWriter
  { 
    private TextWriter originalOut;

    public PrefixedWriter()
    {
        if (!Directory.Exists(Program.homeFolder))
        {
            Directory.CreateDirectory(Program.homeFolder);
        }  
        File.WriteAllText(Program.homeFolder + "log.txt", String.Empty);
        originalOut = Console.Out;
    }

    public override Encoding Encoding
    {
        get { return new System.Text.ASCIIEncoding(); }
    }
    public override void WriteLine(string message)
    {
      string str = String.Format("{0} {1}", DateTime.Now.ToString("hh:mm:ss.ffffff"), message);
      File.AppendAllText(Program.homeFolder + "log.txt", str + "\n");
      originalOut.WriteLine(str);
    }
    public override void Write(string message)
    {
      string str = String.Format("{0} {1}", DateTime.Now.ToString("hh:mm:ss.ffffff"), message);
      File.AppendAllText(Program.homeFolder + "log.txt", str + "\n");
      originalOut.Write(str);
    }
  }
}