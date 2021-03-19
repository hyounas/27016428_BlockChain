using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlockchainAssignment
{

    public partial class BlockchainApp : Form
    {
        Blockchain blockchain;
        public BlockchainApp()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            outputToRichTextBox1("Initilising new Blockchain by 27016428");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
               
        private void printBtn_Click(object sender, EventArgs e)
        {
            
            if (Int32.TryParse(indexTBox.Text, out int index))
                outputToRichTextBox1(blockchain.GetBlockAsString(index));
            else
                outputToRichTextBox1("Invalid Block No.");
           
        }

        private void outputToRichTextBox1(string toBePrinted) { richTextBox1.Text = toBePrinted; }
        private void outputToTextBox(TextBox TBox, string toBePrinted) { TBox.Text = toBePrinted; }

        private void GenWalletBtn_Click(object sender, EventArgs e)
        {
            
            Wallet.Wallet myNewWallet = new Wallet.Wallet(out string privKey);
            String publicKey = myNewWallet.publicID;
            outputToTextBox(privKeyTBox, privKey);
            outputToTextBox(pubKeyTBox, publicKey) ;
        }

        private void ValKeysBtn_Click(object sender, EventArgs e)
        {
            Color color; 
           color =  Wallet.Wallet.ValidatePrivateKey(privKeyTBox.Text, pubKeyTBox.Text) ? Color.Green : Color.Red;
            valKeysBtn.BackColor = color;
        }

        private void CreateTransBtn_Click(object sender, EventArgs e)
        {
            Transaction transaction = new Transaction(pubKeyTBox.Text, privKeyTBox.Text, recieverKeyTBox.Text, Convert.ToSingle(amountTBox.Text), Convert.ToSingle(feeTBox.Text));
            blockchain.add2TPool(transaction);
            outputToRichTextBox1(transaction.ReturnString());
        }

        private void BlockGenBtn_Click(object sender, EventArgs e)
        {
            Block block = new Block(blockchain.GetLastBlock());
            blockchain.add2Block(block);
        }
        private void PrintAllBtn_Click(object sender, EventArgs e)
        {
            string printall = "";
            for (int i = 0; i < blockchain.maxBlock; i++)
            {
                printall += (blockchain.BlockString(Convert.ToInt32(i)) + "\n \n");
            }
            outputToRichTextBox1(printall);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Block block = new Block(blockchain.GetLastBlock(), blockchain.retTPool(), pubKeyTBox.Text, comboBox1.SelectedIndex, addressFind.Text);
            blockchain.purgeTPool(block.transactionList);
            blockchain.add2Block(block);
            Console.WriteLine("added new block to chain - with " + block.transactionList.Count + " transactions");
        }

        private void ReadPendTrandBtn_Click(object sender, EventArgs e)
        {string s = "";
            foreach (Transaction T in blockchain.retTPool()) {
               s+= T +": \n "+T.ReturnString() + "\n \n ";
            }
            outputToRichTextBox1(s);
        }


        /*If any of the text in the key boxes changes, Validation should be reset.
         * 
         *It might be possible to automate this in the future, eliminating the need for a button.  */
        private void PubKeyTBox_TextChanged(object sender, EventArgs e)
        {
            valKeysBtn.BackColor = Color.AntiqueWhite;
        }
        private void PrivKeyTBox_TextChanged(object sender, EventArgs e)
        {
            valKeysBtn.BackColor = Color.AntiqueWhite;
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            outputToRichTextBox1(blockchain.ToString());
        }

        private void BlockchainApp_Load(object sender, EventArgs e)
        {

        }

          private void IndexInput_TextChanged(object sender, EventArgs e)
        {

        }
    

        private void Validate_Click(object sender, EventArgs e)
        {
            // CASE: Genesis Block - Check only the hash since there are currently no transactions. 
            if (blockchain.Blocks.Count == 1)
            {
                if (!Blockchain.ValidateHash(blockchain.Blocks[0])) // Check the validity of Hash by recalculating it. 
                    outputToRichTextBox1("Blockchain is invalid - Hash ");
                else
                    outputToRichTextBox1("Blockchain is valid");
                return;
            }

            Console.WriteLine(" NewBlock: " + (blockchain.Blocks.Count - 1)); 
            for (int i = 1; i < blockchain.Blocks.Count - 1; i++)
            {
                Console.WriteLine("Hash for block " + i);
                if (
                    blockchain.Blocks[i].prevHash != blockchain.Blocks[i - 1].hash || // Check the "chain" hash. 
                    !Blockchain.ValidateHash(blockchain.Blocks[i]) ||  // Check the hash of each Block. 
                    !Blockchain.ValidateMerkleRoot(blockchain.Blocks[i]) // Merkle Root can be used to check transaction integrity. 
                )
                {
                    outputToRichTextBox1("Blockchain is invalid " + (blockchain.Blocks[i].prevHash != blockchain.Blocks[i - 1].hash).ToString() + "  " +
                    !Blockchain.ValidateHash(blockchain.Blocks[i]) + "  " + // Check the hash of each Block. 
                    !Blockchain.ValidateMerkleRoot(blockchain.Blocks[i]) + " " + blockchain.Blocks[i].nonce);
                    return;
                }
            }
            outputToRichTextBox1("Blockchain is valid");
        }

        // Check current user balance
        private void CheckBalance_Click(object sender, EventArgs e)
        {
            outputToRichTextBox1(blockchain.GetBalance(pubKeyTBox.Text).ToString() + " Assignment Coin");
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void infoLbl_Click(object sender, EventArgs e)
        {

        }
    }
}
