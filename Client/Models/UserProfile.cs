namespace Client.Models
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Picture { get; set; }
        public string Password { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }


}
