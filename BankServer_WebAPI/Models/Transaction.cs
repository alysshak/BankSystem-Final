namespace BankServer_WebAPI.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; } // Primary key
        public int AccountId { get; set; }     // Foreign key to Account
        public string TransactionType { get; set; }       // e.g., "Deposit", "Withdrawal"
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; } // The date and time when the transaction occurred

        // Navigation property for Account (if using Entity Framework)
    }
}
