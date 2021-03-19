using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Transaction
    {
        public string Hash{get; set;} // The Hash
        public string Signature{get; set;} // Hash signed with private key of sender
        public string SenderAddress{get; set;} // Public key of sender
        public string RecipientAddress{get; set;} // public key of reciever 
        public DateTime TimeStamp{get; set;} // Time of transaction
        public double Amount{get; set;} // amount sent  // decimal Amount = 2.1M (suffix M needed )
        public double Fee{get; set;} // the fee added to transaction 

        public Transaction()
        {

        }
        public Transaction(string senderPublic, string senderPrivate, string recipientPublic, double amount, double fee)
        {
            this.TimeStamp = DateTime.Now;

            this.SenderAddress = senderPublic;

            this.RecipientAddress = recipientPublic;

            this.Amount = amount;

            this.Fee = fee;

            this.Hash = Create256Hash();

            this.Signature = Wallet.Wallet.CreateSignature(SenderAddress, senderPrivate, Hash);
          
        }

        private string Create256Hash()
        {

            SHA256 hasher;
            hasher = SHA256Managed.Create();

            /* combine all transactions */
            String input = this.SenderAddress + this.RecipientAddress + this.TimeStamp.ToString() + this.Amount.ToString() + this.Fee.ToString();
            /* Apply the hash function to the "input" string */
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));

            String hash = string.Empty;


            /* Convert to string  */
            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);
            }
            return hash;

        }

        /* To print the contents of a transaction, there are two related implementations: and that makes use of the ToString Override */
        public override string ToString()
        {
            return ("[TRANSACTION START]"
                + "\n\t  Timestamp: " + this.TimeStamp
                + "\n  -- Verification --"
                + "\n\t  Hash: " + this.Hash
                + "\n\t  Signature: " + this.Signature
                + "\n  -- Quantities --"
                + "\n\t  Transferred: " + this.Amount + " Assignment Coin"
                + "\t  Fee: " + this.Fee
                + "\n  -- Participants --"
                + "\n\t  Sender: " + this.SenderAddress
                + "\n\t  Reciever: " + this.RecipientAddress
                + "\n  [TRANSACTION END]");
        }
        public string ReturnString()
        {
            return ("[TRANSACTION START]"
                + "\n\t  Timestamp: " + this.TimeStamp
                + "\n  -- Verification --"
                + "\n\t  Hash: " + this.Hash
                + "\n\t  Signature: " + this.Signature
                + "\n  -- Quantities --"
                + "\n\t  Transferred: " + this.Amount + " Assignment Coin"
                + "\t  Fee: " + this.Fee
                + "\n  -- Participants --"
                + "\n\t  Sender: " + this.SenderAddress
                + "\n\t  Reciever: " + this.RecipientAddress
                + "\n  [TRANSACTION END]");

        }

    }
}
