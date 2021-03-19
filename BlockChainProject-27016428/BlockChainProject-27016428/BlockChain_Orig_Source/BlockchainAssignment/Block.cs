using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Block
    {
        public Boolean THREADING { get; set; } = true;                                
        public List<Transaction> transactionList { get; set; }                
        public DateTime timeStamp { get; set; }                                 
        public int index { get; set; }                                          
        public int difficulty { get; set; }                                 
        public long nonce { get; set; } = 0;                                    
        public string hash { get; set; }                                     
        public string prevHash { get; set; }                                  
        public string merkleRoot { get; set; } = "0";                              
        public string minerAddress { get; set; }                               
        public double reward { get; set; } = 1;                                     

// private int nonce = 0;
        public int nonce0 { get; set; } = 0;
        public int nonce1 { get; set; } = 1;
        public string finalHash { get; set; }
        public string finalHash0 { get; set; }
        public string finalHash1 { get; set; }

        private bool th1Fin = false, th2Fin = false;




        // Constructor which is passed the previous block
        public Block(Block lastBlock) {
            this.timeStamp = DateTime.Now;
            this.nonce = 0;
            this.index = lastBlock.index + 1;
            this.prevHash = lastBlock.hash;
            if (THREADING == true){
                ThreadedMine();
                this.hash = this.finalHash;
            }
            else this.hash = this.Create256Hash();
            //    Create hash from index, prevhash and time
            this.transactionList = new List<Transaction>();
           
        }

        public Block(Block lastBlock, List<Transaction> TPool)
        {
            this.transactionList = new List<Transaction>();
            this.nonce = 0;
            this.timeStamp = DateTime.Now;
            this.index = lastBlock.index + 1;
            this.prevHash = lastBlock.hash;
            this.addFromPool(TPool, 2, "!");

            
        }
        // Constructor which is passed the index & hash of previous block
        public Block(int lastIndex, string lastHash) {
            this.nonce = 0;
            this.timeStamp = DateTime.Now;                      
            this.index = lastIndex + 1;                        
            this.prevHash = lastHash;                            
            this.hash = this.Create256Hash();                           
        }

        // Constructor which is not passed anything
        public Block() {
            // This generates the Genesis Block 
            this.transactionList = new List<Transaction>();
            this.timeStamp = DateTime.Now;                                   
            this.index = 0;                                                
            this.prevHash = string.Empty;                                   
            this.hash = this.Create256Mine();                              
            this.difficulty = 4;
        }

        public Block(Block lastBlock, List<Transaction> TPool, string MinerAddress, int setting, string address )
        {
            this.transactionList = new List<Transaction>();
            this.nonce = 0;
            this.timeStamp = DateTime.Now;
            this.difficulty = lastBlock.difficulty;
            this.adjustdiff(lastBlock.timeStamp); 
            this.index = lastBlock.index + 1;
            this.prevHash = lastBlock.hash;
            this.minerAddress = MinerAddress;
            TPool.Add(createRewardTransaction(TPool)); // Create and append the reward transaction
            this.addFromPool(TPool, setting, address );
            this.merkleRoot = MerkleRoot(transactionList); // Calculate merkleRoot of block transaction

            if (THREADING == true)
            {
                this.ThreadedMine();
                this.hash = this.finalHash;
            }
            else this.hash = this.Create256Mine();
            Console.WriteLine("Here");
        }

        public override string ToString()
        {
            return ("\n\n\t\t[BLOCK START]"
                + "\nIndex: " + this.index
                + "\tTimestamp: " + this.timeStamp
                + "\nPrevious Hash: " + this.prevHash
                + "\n\t\t-- PoW --"
                + "\nDifficulty Level: " + this.difficulty
                + "\nNonce: " + this.nonce
                + "\nHash: " + this.hash + " " + this.finalHash
                + "\n\t\t-- Rewards --"
                + "\nReward: " + this.reward
                + "\nMiners Address: " + this.minerAddress
                + "\n\t\t-- " + this.transactionList.Count + " Transactions --"
                + "\nMerkle Root: " + this.merkleRoot
                + "\n" + String.Join("\n", this.transactionList)
                + "\n\t\t[BLOCK END]");
        }
        public string ReturnString() {
            return ("\n\n\t\t[BLOCK START]"
                + "\nIndex: " + this.index
                + "\tTimestamp: " + this.timeStamp
                + "\nPrevious Hash: " + this.prevHash
                + "\n\t\t-- PoW --"
                + "\nDifficulty Level: " + this.difficulty
                + "\nNonce: " + this.nonce
                + "\nHash: " + this.hash
                + "\n\t\t-- Rewards --"
                + "\nReward: " + this.reward
                + "\nMiners Address: " + this.minerAddress
                + "\n\t\t-- " + this.transactionList.Count + " Transactions --"
                + "\nMerkle Root: " + this.merkleRoot
                + "\n" + String.Join("\n", this.transactionList)
                + "\n\t\t[BLOCK END]");
        }

        public string readblock()
        {
            string s = "";
            s += this.ToString();

            foreach (Transaction T in transactionList)
            {
                s += ("\n" + T.ToString());
            }
            return s;

        }

        public void add2TList(Transaction T)
        {
            this.transactionList.Add(T);
        }

        public void addFromPool(List<Transaction> TP, int mode, string address)
        {
            int LIMIT = 5;
            int idx =0 ;

            
            while (transactionList.Count < LIMIT && TP.Count > 0 ) {
                if (mode == 0 ) {
                    
                    for (int i = 0; ((i < TP.Count)); i++)
                    {
                        if (TP.ElementAt(i).Fee > TP.ElementAt(idx).Fee)
                        {
                            idx = i;
                        }
                    }
                    this.transactionList.Add(TP.ElementAt(idx));
                } 
                else if (mode == 1) {// altruistic
                    for (int i = 0; ((i < TP.Count) && (i < LIMIT)); i++)
                    {
                        this.transactionList.Add(TP.ElementAt(i));
                    }
                } 
                else if (mode == 2 ) {  //rand   
                    Random random = new Random();
                    idx = random.Next(0, TP.Count);
                    this.transactionList.Add(TP.ElementAt(idx));
                }       
                else if (mode == 3) {
                    
                   
                    for (int i = 0; i < TP.Count && (transactionList.Count < LIMIT); i++)
                    {                       
                        if (TP.ElementAt(i).SenderAddress == address)
                        {
                            this.transactionList.Add(TP.ElementAt(i));
                        }
                        else if (TP.ElementAt(i).RecipientAddress == address)
                        {
                            this.transactionList.Add(TP.ElementAt(i));
                        }
                        else
                        {
                            
                        }
                        
                        
                    }
                    Console.WriteLine("Endless loop");
                }
                else
                { 
                    mode = 1; 
                }
                TP = TP.Except(this.transactionList).ToList();
                
            }

        }

       
        public string Create256Hash()
        {
            SHA256 hasher;
            hasher = SHA256Managed.Create();
            String input = this.index.ToString() + this.timeStamp.ToString() + this.prevHash + this.nonce + this.merkleRoot + this.reward.ToString();
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));

            String hash = string.Empty;

            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);
            }
            return hash;


        }

        private string Create256Mine()
        {             
            string hash =  "";         
            string diffString = new string('0', this.difficulty);
            while (hash.StartsWith(diffString)== false)
            {
                hash = this.Create256Hash();
                this.nonce++;
            }
            this.nonce--;
            if (hash is null){
                throw new IndexOutOfRangeException("No hash generated"); }
            return hash;
        }

        public static string MerkleRoot(List<Transaction> transactionList) {

            // X => f(X) means given X return f(X)
            List<String> hashes = transactionList.Select(t => t.Hash).ToList(); // Get a list of transaction hashes for "combining"
            // Handle Blocks with...
            if (hashes.Count == 0) // No transactions
            {
                return String.Empty;
            }
            else if (hashes.Count == 1) // One transaction - hash with "self"
            {
                return HashCode.HashTools.combineHash(hashes[0], hashes[0]);
            }
            while (hashes.Count != 1) // Repeat until the tree has been traversed several times. 
            {
                List<String> merkleLeaves = new List<String>(); // Keep track of the tree's current "level." 

                for (int i = 0; i < hashes.Count; i += 2) // Step over the adjacent pair, merging them. 
                {
                    if (i == hashes.Count - 1)
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashes[i], hashes[i])); // Handle an unusually large number of leaves 
                    }
                    else
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashes[i], hashes[i + 1])); // Hash neighbours leaves
                    }
                }
                hashes = merkleLeaves; // Update the "layer" that is currently in use. 
            }
            return hashes[0]; // returns roots node
        }

        // Create a reward to encourage the mining of blocks. 
        public Transaction createRewardTransaction(List<Transaction> transactions)
        {
            double fees = transactions.Aggregate(0.0, (acc, t) => acc + t.Fee); // Add up all the processing costs. 
            return new Transaction("Mine Rewards", "", minerAddress, (this.reward + fees), 0); // In the new block, issue the incentive as a transaction. 
        }
        private string ArchivedCreate256Hash() {
            SHA256 hasher;
            hasher = SHA256Managed.Create();
            String input = this.index.ToString() + this.timeStamp.ToString() + this.prevHash;
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));

            String hash = string.Empty;

            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);
            }
            return hash;


        }

        public void ThreadedMine(){
            Thread th1 = new Thread(this.Mine0);
            Thread th2 = new Thread(this.Mine1);

            th1.Start();
            th2.Start();

            while (th1.IsAlive == true || th2.IsAlive == true){Thread.Sleep(1);}

           
            if (this.finalHash1 is null) { 
                this.nonce = this.nonce0;
                this.finalHash = this.finalHash0;
            }
            else{
                this.nonce = this.nonce1;
                this.finalHash = this.finalHash1;
            }
            if (this.finalHash is null)
            {
                Console.WriteLine(this.ReturnString());
                throw new Exception("NULL finalhash" + 
                    " Nonce0: " + this.nonce0 + 
                    " Nonce1: "+ this.nonce1 + 
                    " Nonce: " + this.nonce +
                    " finalhash0 " + this.finalHash0 +
                    " finalhash1: " + this.finalHash1 +
                    " NewHash: " + this.Create256Hash());
               
            }
            
        }

        public void Mine0(){
            th1Fin = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Boolean check = false;
            String newHash;
            string diffString = new string('0', this.difficulty);

            while (check == false)
            {
                newHash = Create256Hash(this.nonce0);
                if (newHash.StartsWith(diffString) == true){
                    check = true;
                    this.finalHash0 = newHash;
                    Console.WriteLine("Block index: " + this.index);
                    Console.WriteLine("Thread 1 closed: Thread 1 found: " + this.finalHash0);
                    th1Fin = true;

                    Console.WriteLine(nonce0);
                    sw.Stop();
                    Console.WriteLine("Th1 mine:");
                    Console.WriteLine(sw.Elapsed);

                    return;
                }
                else if (th2Fin == true){
                    Console.WriteLine("Thread 1 closed: Thread 2 found: " + this.finalHash1 );
                    Thread.Sleep(1);
                    return;
                }
                else{
                    check = false;
                    this.nonce0 += 2;
                }
            }
            return;
        }

        public void Mine1()
        {
            th2Fin = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Boolean check = false;
            String newHash;
            string diffString = new string('0', this.difficulty);
            while (check == false){
                newHash = Create256Hash(this.nonce1);
                if (newHash.StartsWith(diffString) == true){
                    check = true;
                    this.finalHash1 = newHash;
                    Console.WriteLine("Block index: " + this.index);
                   Console.WriteLine("Thread 2 closed: Thread 2 found: " + this.finalHash1);
                    th2Fin = true;

                    Console.WriteLine(this.nonce1);
                    sw.Stop();
                    Console.WriteLine("Th2 mine:");
                    Console.WriteLine(sw.Elapsed);

                    return;
                }
                else if (th1Fin == true){
                    Console.WriteLine("Thread 2 closed: Thread 1 found: " + this.finalHash0);
                    Thread.Sleep(1);
                    return;
                }
                else{
                    check = false;
                    this.nonce1 += 2;
                }
            }
            return;
        }

        //Nonce is now a parameter in the above process. 
        public String Create256Hash(int inNonce)
        {
            SHA256 hasher;
            hasher = SHA256Managed.Create();
            String input = this.index.ToString() + this.timeStamp.ToString() + this.prevHash + inNonce + this.merkleRoot + this.reward.ToString();
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));
            String hash = string.Empty;

            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);
            }
            return hash;
        }

        //Adjusting difficulty
        public void adjustdiff(DateTime lastTime)
        {
            //Gets the amount of time that has passed since the last block was mined. 
            DateTime startTime = DateTime.UtcNow;
            TimeSpan timeDiff = startTime - lastTime;

            //If the gap between the two times is less than 5 seconds, the complexity is raised in order to maximise the time. 
            if (timeDiff < TimeSpan.FromSeconds(5))
            {
                this.difficulty++;
                Console.WriteLine("Time since last mine");
                Console.WriteLine(timeDiff);
                Console.WriteLine("New Difficulty:");
                Console.WriteLine(this.difficulty);
            }
            //If the time delay is greater than 5 seconds, the difficulty level is raised in order to reduce the time. 
            else if (timeDiff > TimeSpan.FromSeconds(5))
            {
                difficulty--;
                Console.WriteLine("Time since last mine");
                Console.WriteLine(timeDiff);
                Console.WriteLine("New Difficulty:");
                Console.WriteLine(this.difficulty);
            }

            //Difficulty will never reach 5 or fall below 0. 
            if (this.difficulty <= 0)
            {
                this.difficulty = 0;
                Console.WriteLine("Difficulty too low, new difficulty:");
                Console.WriteLine(this.difficulty);
            }
            else if (this.difficulty >= 6)
            {
                this.difficulty = 4;
                Console.WriteLine("Difficulty too high, new difficulty:");
                Console.WriteLine(this.difficulty);
            }
        }




    }

}
