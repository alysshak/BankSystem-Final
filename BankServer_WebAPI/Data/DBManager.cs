using BankServer_WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;

namespace LocalDBWebAPI.Data
{
    public class DBManager
    {
        private static string connectionString = "Data Source=mydatabase.db;Version=3;";

        public static bool CreateTables()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        // Create UserProfiles table first since Accounts references it
                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS UserProfiles (
                        UserId INTEGER PRIMARY KEY,
                        FirstName TEXT,
                        LastName TEXT,
                        Username TEXT UNIQUE,  
                        Email TEXT,
                        Address TEXT,
                        Phone TEXT,
                        Picture BLOB,
                        Password TEXT
                    )";
                        command.ExecuteNonQuery();

                        // Create Accounts table
                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Accounts (
                        AccountId INTEGER PRIMARY KEY,
                        Balance DECIMAL,
                        UserId INTEGER,
                        FOREIGN KEY(UserId) REFERENCES UserProfiles(UserId)
                    )";
                        command.ExecuteNonQuery();

                        // Create Transactions table
                        command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Transactions (
                        TransactionId INTEGER PRIMARY KEY,
                        AccountId INTEGER,
                        Amount DECIMAL,
                        TransactionDate DATETIME,
                        TransactionType TEXT,
                        FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
                    )";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        // CRUD for UserProfiles
        public static bool InsertUserProfile(UserProfile userProfile)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        
                        command.CommandText = @"
                        INSERT INTO UserProfiles (FirstName, LastName, Username, Email, Address, Phone, Picture, Password, UserId)
                        VALUES (@FirstName, @LastName, @Username, @Email, @Address, @Phone, @Picture, @Password, @UserId)";

                        command.Parameters.AddWithValue("@FirstName", userProfile.FirstName);
                        command.Parameters.AddWithValue("@LastName", userProfile.LastName);
                        command.Parameters.AddWithValue("@Username", userProfile.Username);
                        command.Parameters.AddWithValue("@Email", userProfile.Email);
                        command.Parameters.AddWithValue("@Address", userProfile.Address);
                        command.Parameters.AddWithValue("@Phone", userProfile.Phone);
                        command.Parameters.Add("@Picture", DbType.Binary).Value = Convert.FromBase64String(userProfile.Picture);
                        command.Parameters.AddWithValue("@Password", userProfile.Password);
                        command.Parameters.AddWithValue("@UserId", userProfile.UserId);

                        int rowsInserted = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsInserted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static UserProfile GetUserProfile(string usernameOrEmail)
        {
            UserProfile userProfile = null;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT * FROM UserProfiles WHERE Username = @UsernameOrEmail OR Email = @UsernameOrEmail";
                        command.Parameters.AddWithValue("@UsernameOrEmail", usernameOrEmail);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userProfile = new UserProfile
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Phone = reader["Phone"].ToString(),
                                    Picture = Convert.ToBase64String((byte[])reader["Picture"]),
                                    Password = reader["Password"].ToString()
                                };
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return userProfile;
        }

        public static bool UpdateUserProfile(UserProfile userProfile)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        UPDATE UserProfiles 
                        SET FirstName = @FirstName, LastName = @LastName, Username = @Username, Email = @Email, Address = @Address, 
                            Phone = @Phone, Picture = @Picture, Password = @Password 
                        WHERE UserId = @UserId";

                        command.Parameters.AddWithValue("@FirstName", userProfile.FirstName);
                        command.Parameters.AddWithValue("@LastName", userProfile.LastName);
                        command.Parameters.AddWithValue("@Username", userProfile.Username);
                        command.Parameters.AddWithValue("@Email", userProfile.Email);
                        command.Parameters.AddWithValue("@Address", userProfile.Address);
                        command.Parameters.AddWithValue("@Phone", userProfile.Phone);
                       // command.Parameters.AddWithValue("@Picture", userProfile.Picture);
                        command.Parameters.Add("@Picture", DbType.Binary).Value = Convert.FromBase64String(userProfile.Picture);
                        command.Parameters.AddWithValue("@Password", userProfile.Password);
                        command.Parameters.AddWithValue("@UserId", userProfile.UserId);

                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsUpdated > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool DeleteUserProfile(int userId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM UserProfiles WHERE UserId = @UserId";
                        command.Parameters.AddWithValue("@UserId", userId);

                        int rowsDeleted = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsDeleted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public static List<UserProfile> GetAllUsers()
        {
            List<UserProfile> userList = new List<UserProfile>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM UserProfiles";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserProfile user = new UserProfile();
                                user.FirstName = reader["FirstName"].ToString();
                                user.LastName = reader["LastName"].ToString();
                                user.Email = reader["Email"].ToString();
                                user.Address = reader["Address"].ToString();
                                user.UserId = Convert.ToInt32(reader["UserId"]);
                                user.Password = reader["Password"].ToString();
                                user.Username = reader["Username"].ToString();
                                user.Phone = reader["Phone"].ToString();
                                user.Picture = Convert.ToBase64String((byte[])reader["Picture"]);
                                userList.Add(user);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return userList;

        }

        // CRUD for Accounts
        public static bool InsertAccount(Account account)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO Accounts (Balance, UserId)
                        VALUES (@Balance, @UserId)";

                        command.Parameters.AddWithValue("@Balance", account.Balance);
                        command.Parameters.AddWithValue("@UserId", account.UserId);

                        int rowsInserted = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsInserted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static Account GetAccountById(int accountId)
        {
            Account account = null;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT * FROM Accounts WHERE AccountId = @AccountId";
                        command.Parameters.AddWithValue("@AccountId", accountId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                account = new Account
                                {
                                    AccountId = Convert.ToInt32(reader["AccountId"]),
                                    Balance = Convert.ToDecimal(reader["Balance"]),
                                    UserId = Convert.ToInt32(reader["UserId"])
                                };
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return account;
        }

        public static bool UpdateAccount(Account account)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        UPDATE Accounts 
                        SET Balance = @Balance, UserId = @UserId
                        WHERE AccountId = @AccountId";

                        command.Parameters.AddWithValue("@Balance", account.Balance);
                        command.Parameters.AddWithValue("@UserId", account.UserId);
                        command.Parameters.AddWithValue("@AccountId", account.AccountId);

                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsUpdated > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool DeleteAccount(int accountId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Accounts WHERE AccountId = @AccountId";
                        command.Parameters.AddWithValue("@AccountId", accountId);

                        int rowsDeleted = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsDeleted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public static List<Account> GetAllAccounts()
        {
            List<Account> accList = new List<Account>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Accounts";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Account account = new Account();
                                account.AccountId = Convert.ToInt32(reader["AccountId"]);
                                account.Balance = Convert.ToDecimal(reader["Balance"]);
                                account.UserId = Convert.ToInt32(reader["UserId"]);

                                accList.Add(account);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return accList;

        }


        // CRUD for Transactions
        public static bool InsertTransaction(Transaction transaction)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                INSERT INTO Transactions (AccountId, TransactionType, Amount, TransactionDate)
                VALUES (@AccountId, @TransactionType, @Amount, @TransactionDate)";

                        command.Parameters.AddWithValue("@AccountId", transaction.AccountId);
                        command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                        command.Parameters.AddWithValue("@Amount", transaction.Amount);
                        command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);

                        int rowsInserted = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsInserted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static Transaction GetTransactionById(int transactionId)
        {
            Transaction transaction = null;
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                SELECT * FROM Transactions WHERE TransactionId = @TransactionId";
                        command.Parameters.AddWithValue("@TransactionId", transactionId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                transaction = new Transaction
                                {
                                    TransactionId = Convert.ToInt32(reader["TransactionId"]),
                                    AccountId = Convert.ToInt32(reader["AccountId"]),
                                    TransactionType = reader["TransactionType"].ToString(),
                                    Amount = Convert.ToDouble(reader["Amount"]),
                                    TransactionDate = Convert.ToDateTime(reader["TransactionDate"])
                                };
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return transaction;
        }

        public static bool UpdateTransaction(Transaction transaction)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                UPDATE Transactions 
                SET AccountId = @AccountId, TransactionType = @TransactionType, 
                    Amount = @Amount, TransactionDate = @TransactionDate
                WHERE TransactionId = @TransactionId";

                        command.Parameters.AddWithValue("@AccountId", transaction.AccountId);
                        command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                        command.Parameters.AddWithValue("@Amount", transaction.Amount);
                        command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                        command.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);

                        int rowsUpdated = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsUpdated > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public static bool DeleteTransaction(int transactionId)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Transactions WHERE TransactionId = @TransactionId";
                        command.Parameters.AddWithValue("@TransactionId", transactionId);

                        int rowsDeleted = command.ExecuteNonQuery();
                        connection.Close();
                        return rowsDeleted > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public static List<Transaction> GetAllTransactions()
        {
            List<Transaction> tList = new List<Transaction>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Transactions";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Transaction transaction = new Transaction();
                                transaction.TransactionType = reader["TransactionType"].ToString();
                                transaction.Amount = Convert.ToDouble(reader["Amount"]);
                                transaction.AccountId = Convert.ToInt32(reader["AccountId"]);
                                transaction.TransactionId = Convert.ToInt32(reader["TransactionId"]);

                                tList.Add(transaction);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return tList;

        }
        public static void DBInitialize()
        {
            Random random = new Random();
            if (CreateTables())
            {
                for (int i = 0; i < 10; i++)
                {
                    UserProfile userProfile = new UserProfile()
                    {
                        FirstName = GetFirstName(random),
                        LastName = GetLastName(random),
                        Username = GenerateRandomString(10, random),
                        Email = GenerateRandomString(10, random) + "@gmail.com",
                        Address = GenerateRandomString(12, random),
                        Phone = GenerateRandomNum(10, random),
                        //Picture = Convert.ToBase64String(GenerateRandomPicture(random)),
                        Picture = GenerateBlueImageAsBase64(64,64),
                        Password = GenerateRandomString(10, random),
                        UserId = i
                    };
                    InsertUserProfile(userProfile);
                }
                List<UserProfile> userProfiles = GetAllUsers();
                foreach (UserProfile userProfile in userProfiles)
                {
                    Account account = new Account
                    {
                        Balance = random.Next(100, 10000), // Random balance between 100 and 10000
                        UserId = userProfile.UserId,
                        AccountId = userProfile.UserId
                    };

                    InsertAccount(account);
                }
                List<Account> accounts = GetAllAccounts();
                foreach (Account account in accounts)
                {
                    Transaction transaction = new Transaction
                    {
                        AccountId = account.AccountId,
                        TransactionType = random.Next(2) == 0 ? "Deposit" : "Withdrawal",
                        Amount = random.Next(10, 1000),
                        TransactionDate = DateTime.Now.AddMinutes(-random.Next(1, 10000))
                    };
                    InsertTransaction(transaction);
                }
                // Seed data for User Profiles, Accounts and Transactions
                // I think we need to generate random data
            }
        }
        private static string GetFirstName(Random rnd)
        {
            int nameSelection = rnd.Next(1, 10);
            string firstName = "Rope";
            switch (nameSelection)
            {
                case 1:
                    firstName = "Randy";
                    break;

                case 2:
                    firstName = "Janet";
                    break;

                case 3:
                    firstName = "Cameron";
                    break;

                case 4:
                    firstName = "Alexander";
                    break;

                case 5:
                    firstName = "David";
                    break;

                case 6:
                    firstName = "Hassan";
                    break;

                case 7:
                    firstName = "May";
                    break;

                case 8:
                    firstName = "Carrie";
                    break;

                case 9:
                    firstName = "Jayden";
                    break;

                case 10:
                    firstName = "Theresa";
                    break;
            }
            return firstName;
        }
        private static string GetLastName(Random rnd)
        {
            int nameSelection = rnd.Next(1, 10);
            string lastName = "";
            switch (nameSelection)
            {
                case 1:
                    lastName = "Rorschach";
                    break;

                case 2:
                    lastName = "Jordans";
                    break;

                case 3:
                    lastName = "Harris";
                    break;

                case 4:
                    lastName = "Agreste";
                    break;

                case 5:
                    lastName = "Davidson";
                    break;

                case 6:
                    lastName = "Highlander";
                    break;

                case 7:
                    lastName = "Endaya";
                    break;

                case 8:
                    lastName = "Underwood";
                    break;

                case 9:
                    lastName = "Yuki";
                    break;

                case 10:
                    lastName = "Sharma";
                    break;
            }
            return lastName;

        }
        private static string GenerateRandomString(int length, Random rnd)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
        private static string GenerateRandomNum(int length, Random rnd)
        {
            string combinedNum = "";

            for (int i = 0; i < length; i++)
            {
                int newNum = rnd.Next(1, 10);
                combinedNum += newNum.ToString();
            }
            return combinedNum;
        }
        private static byte[] GenerateRandomPicture(Random rnd)
        {
            byte[] picture = new byte[100]; // Adjust the size as per your requirements

            rnd.NextBytes(picture);

            return picture;
        }

        private static string GetDefaultPictureAsBase64()
        {
            var imagePath = @"./62bb72a477c754d59691a2b1fc95e74c.png";
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageBytes);
        }

        private static string GenerateBlueImageAsBase64(int width, int height)
        {
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Blue); // Fill the entire image with a blue color
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}

