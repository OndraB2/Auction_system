using System;
using BlockChainLedger;
using Kademlia;
using System.Threading;

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
            System.Console.WriteLine(index);
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
            foreach(SentGroupOfTransactions group in CurrentlyBeingConfirmedTransactionsGroups)
            {
                System.Console.WriteLine(group);
                System.Console.WriteLine(group.count);
                System.Console.WriteLine();
            }
        }

        static public void PrintTransactions()
        {
            foreach (object t in ActiveTransactionsList)
            {
                (t as Transaction).Print();
            }
        }
        static public void AddTransactionToPool(object transaction)
        {
            ActiveTransactionsList.Add(transaction as Transaction);
        }
        static public List<Transaction> GetTransactions()
        {
            // user.Transactions = ActiveTransactionsList;
            // UsersList.Add(user);
            AddTransactionsGroupToBeingConfirmedList(ActiveTransactionsList);
            return ActiveTransactionsList;
        }

        static public List<Transaction> GetTransactions(int n)
        {
            List<Transaction> N_FirstTransactions = ActiveTransactionsList.Take(n).ToList();
            // user.Transactions = N_FirstTransactions;
            // UsersList.Add(user);
            AddTransactionsGroupToBeingConfirmedList(N_FirstTransactions);

            return N_FirstTransactions;
        }

        static public void RemoveUser(KademliaNode KNode)
        {

        }

        static public void GenerateTraffic()
        {
            NewAuctionItemTransaction auction1 = NewAuctionItemTransaction.GetRandom();
            ActiveAuctions.AddAuction(auction1);
            NewAuctionItemTransaction auction2 = NewAuctionItemTransaction.GetRandom();
            ActiveAuctions.AddAuction(auction2);
            Random rand = new Random();
            while(true)
            {   
                int a1_Bbid = rand.Next(Convert.ToInt32(auction1.GetStartingBid()), Convert.ToInt32(auction1.GetFinalBid() + auction1.GetStartingBid()));
                int a2_Bbid = rand.Next(Convert.ToInt32(auction2.GetStartingBid()), Convert.ToInt32(auction2.GetFinalBid() + auction2.GetStartingBid()));

                ActiveAuctions.NewBid(auction1, a1_Bbid, Guid.NewGuid().ToByteArray());
                if(ActiveAuctions.GetAuction(auction1) is null)
                {
                    auction1 = NewAuctionItemTransaction.GetRandom();   
                    ActiveAuctions.AddAuction(auction1);
                }

                Thread.Sleep(10000);

                ActiveAuctions.NewBid(auction2, a2_Bbid, Guid.NewGuid().ToByteArray());
                if(ActiveAuctions.GetAuction(auction2) is null)
                {
                    auction2 = NewAuctionItemTransaction.GetRandom();  
                    ActiveAuctions.AddAuction(auction2);
                }

                Thread.Sleep(1000);
                System.Console.WriteLine("Transactin count: " + TransactionPool.GetTransactions().Count);

            }
        }
    }
}