namespace Client.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }  // Foreign Key

        // Navigation property for UserProfile
    }

}
