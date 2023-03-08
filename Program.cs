using System;
using BlockChainLedger;

namespace AuctionSystem
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");
      PoWBlockTest();
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
  }
}