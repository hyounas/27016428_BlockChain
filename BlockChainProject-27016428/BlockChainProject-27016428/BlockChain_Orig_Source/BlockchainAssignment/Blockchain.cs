using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Blockchain
    {
        public int maxBlock { get => this.Blocks.Count; }                                    // Max no. of transactions per block
        public List<Block> Blocks = new List<Block>();                                      // List of block objects forming the blockchain
        public List<Transaction> TransactionPool = new List<Transaction>();                // List of pending transactions to be mined

        public Blockchain()
        {
            Block genesis = new Block();
            Blocks.Add(genesis);
        }
        public string BlockString(int index)
        {
            return (Blocks.ElementAt(index).ReturnString());
        }


        public void add2TPool(Transaction Trans)
        {
            TransactionPool.Add(Trans);
        }
        public void add2Block(Block blck)
        {
            Blocks.Add(blck);
        }

        // The block at the specified index is printed to the user interface. 
        public String GetBlockAsString(int index)
        {
            // Check to see if the block that is being referenced exists. 
            if (index >= 0 && index < Blocks.Count)
                return Blocks[index].ToString(); // As a string, return block. 
            else
                return "No such block exists";
        }
        public void purgeTPool(List<Transaction> chosenT)
        {
            TransactionPool = TransactionPool.Except(chosenT).ToList();
        }
        public Block GetLastBlock()
        {
            return Blocks[Blocks.Count - 1];
        }

        /* The transaction pool is returned by this function. */
        public List<Transaction> retTPool() {
            return TransactionPool;
        }

        /* Function to return blockchain */ 
        public override string ToString()
        {
            return string.Join("\n", Blocks);
        }

        // Check the validity of a block's hash by recalculating it and comparing it to the value mined. 
        public static bool ValidateHash(Block b){
            string rehash =string.Empty ;
            rehash = b.Create256Hash();
            Console.WriteLine("Rehash: " + rehash + " --> Hash: " + b.hash);
            return rehash.Equals(b.hash);
        }



        // Based on the public key, assess the balance associated with a wallet. 
        public double GetBalance(String address)
        {
            // Accumulator value
            double balance = 0;

            // Loop through all approved transactions in order to assess account balance
            foreach (Block b in Blocks)
            {
                foreach (Transaction t in b.transactionList)
                {
                    if (t.RecipientAddress.Equals(address))
                    {
                        balance += t.Amount; // Credit funds recieved
                    }
                    if (t.SenderAddress.Equals(address))
                    {
                        balance -= (t.Amount + t.Fee); // Debit payments placed
                    }
                }
            }
            return balance;
        }

        // Recalculate the merkle root and compare it to the mined value to ensure its validity. 
        public static bool ValidateMerkleRoot(Block b){
            String reMerkle = Block.MerkleRoot(b.transactionList);
            return reMerkle.Equals(b.merkleRoot);
        }
    }
}
