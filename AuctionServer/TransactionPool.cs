using System;
using BlockChainLedger;
using Kademlia;
using System.Threading;
using AuctionClient;

namespace AuctionServer
{
    public class SentGroupOfTransactions
    {
        public List<Transaction> Transactions = new List<Transaction>();
        public int count;

        public SentGroupOfTransactions(List<Transaction> transactions)
        {
            Transactions = transactions;
            count = 1;
        }

        public void IncrementCount()
        {
            count += 1;
        }

    }
    static class TransactionPool
    {

        static public List<Transaction> ActiveTransactionsList = new List<Transaction>();
        // static public List<User> UsersList = new List<User>();
        static public List<SentGroupOfTransactions> CurrentlyBeingConfirmedTransactionsGroups = new List<SentGroupOfTransactions>();
        static public DataModuleAPI DataModuleAPIinstance;

        static public bool CompareTwoTransactionsGroups(List<Transaction> group1, List<Transaction> group2)
        {
            if (group1.Count != group2.Count)
            {
                // If the two groups have different number of transactions, they are not the same.
                return false;
            }

            // Sort both groups by their transaction IDs to ensure that they are in the same order.
            group1.Sort((t1, t2) => t1.TID.CompareTo(t2.TID));
            group2.Sort((t1, t2) => t1.TID.CompareTo(t2.TID));

            // Compare each transaction in the two groups.
            for (int i = 0; i < group1.Count; i++)
            {
                if (group1[i].TID != group2[i].TID)
                {
                    // If any transaction is different, the two groups are not the same.
                    return false;
                }
            }

            // All transactions are the same, so the two groups are the same.
            return true;
        }

        static private int FindTransactionsGroupInBeingConfirmedList(List<Transaction> transactionsGroup)
        {
            for (int i = 0; i < CurrentlyBeingConfirmedTransactionsGroups.Count; i++)
            {
                if (CompareTwoTransactionsGroups(transactionsGroup, CurrentlyBeingConfirmedTransactionsGroups[i].Transactions))
                {
                    return i;
                }
            }
            return -1;
        }

        static public void AddTransactionsGroupToBeingConfirmedList(List<Transaction> transactionsGroup)
        {
            int index = FindTransactionsGroupInBeingConfirmedList(transactionsGroup);
            System.Console.WriteLine("Index: " + index);
            if (index >= 0)
            {
                CurrentlyBeingConfirmedTransactionsGroups[index].IncrementCount();
            }
            else
            {
                CurrentlyBeingConfirmedTransactionsGroups.Add(new SentGroupOfTransactions(transactionsGroup));
            }
        }

        static public void PrintBeingConfirmedTransactionsList()
        {
            System.Console.WriteLine("Count of Transaction Groups: " + CurrentlyBeingConfirmedTransactionsGroups.Count);
            // foreach(SentGroupOfTransactions group in CurrentlyBeingConfirmedTransactionsGroups)
            // {
            //     System.Console.WriteLine(group);
            //     System.Console.WriteLine(group.count);
            //     System.Console.WriteLine();
            // }
        }

        static public void RemoveConfirmedTransactionFromPool(object ?state)
        {
            Console.WriteLine("RemoveConfirmedTransactionFromPool start");
            // List<Transaction> ActiveTransactionsList_Copy = new List<Transaction>(ActiveTransactionsList);
            // for(int i = 0; i < ActiveTransactionsList_Copy.Count; i++)
            // {
            //     if(DataModuleAPIinstance.IsTransactionAlreadyInBlock(ActiveTransactionsList_Copy[i]))
            //     {
            //         Console.WriteLine("RemoveConfirmedTransactionFromPool Remove at index " + i);
            //         ActiveTransactionsList.RemoveAt(i);
            //     }

            // }

            List<Transaction> toRemove = new List<Transaction>();
            foreach(var t in ActiveTransactionsList)
            {
                toRemove.Add(t);
            }
            foreach(var t in toRemove)
            {
                AuctionSystem.PrefixedWriter.WriteLineImprtant("RemoveConfirmedTransactionFromPool Remove " + t.TID);
                ActiveTransactionsList.Remove(t);
            }
        }

        static public void PrintTransactions()
        {
            foreach (object t in ActiveTransactionsList)
            {
                (t as Transaction).Print();
            }
        }
        static public void AddTransactionToPool(Transaction transaction)
        {
            ActiveTransactionsList.Add(transaction);
        }
        static public List<Transaction> GetTransactions()
        {
            // user.Transactions = ActiveTransactionsList;
            // UsersList.Add(user);
            AddTransactionsGroupToBeingConfirmedList(new List<Transaction>(ActiveTransactionsList));
            return ActiveTransactionsList;
        }

        static public List<Transaction> GetTransactions(int n)
        {
            List<Transaction> N_FirstTransactions = ActiveTransactionsList.Take(n).ToList();
            // user.Transactions = N_FirstTransactions;
            // UsersList.Add(user);
            // AddTransactionsGroupToBeingConfirmedList(N_FirstTransactions);

            return N_FirstTransactions;
        }

        static public void RemoveConfirmedTransactionsFromPool()
        {
            
        }

        static public void GenerateTraffic()
        {
            KademliaNode kn = KademliaNode.CreateInstance("125.12.41.24", 1232);
            AuctionSeller seller1 = new AuctionSeller(kn);
            AuctionSeller seller2 = new AuctionSeller(kn);
            User u1 = new User(kn);
            User u2 = new User(kn);
            // NewAuctionItemTransaction auction1 = NewAuctionItemTransaction.GetRandom();
            // Auction auction1 = ActiveAuctions.AddAuction(auction1);

            Auction auction1 = seller1.CreateAuction();
            Auction auction2 = seller2.CreateAuction();

            auction1.AttachNewObserver(u1);
            // auction1.AttachNewObserver(u2);
            Random rand = new Random();
            while(true)
            {   
                int auction1_Bbid = rand.Next(Convert.ToInt32(auction1.StartingBid), Convert.ToInt32(auction1.FinalBid + auction1.StartingBid));
                int auction2_Bbid = rand.Next(Convert.ToInt32(auction2.StartingBid), Convert.ToInt32(auction2.FinalBid + auction2.StartingBid));

                ActiveAuctions.NewBid(auction1, auction1_Bbid, Guid.NewGuid().ToByteArray());
                if(!ActiveAuctions.IsAuctionActive(auction1))
                {
                    auction1 = seller1.CreateAuction();

                }

                Thread.Sleep(3000);

                ActiveAuctions.NewBid(auction2, auction2_Bbid, Guid.NewGuid().ToByteArray());
                if(!ActiveAuctions.IsAuctionActive(auction2))
                {
                    auction2 = seller2.CreateAuction();
                }

                Thread.Sleep(1000);
                TransactionPool.GetTransactions();
                TransactionPool.PrintBeingConfirmedTransactionsList();
            }
        }
    }
}