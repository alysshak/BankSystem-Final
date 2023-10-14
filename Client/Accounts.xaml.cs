using Client.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class Accounts : Page
    {
        RestClient client = new RestClient("http://localhost:5235/");
        Models.Account account;

        public Accounts()
        {
            InitializeComponent();
        }

        private bool TryParseInt(string input, out int result, string errorMessage)
        {
            if (!int.TryParse(input, out result))
            {
                MessageBox.Show(errorMessage);
                return false;
            }
            return true;
        }

        //Update Account Button
        private void UpdateAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseInt(EditBalance.Text, out int balance, "Invalid balance entered.")) return;

            account.Balance = balance;
            RestRequest request = new RestRequest("api/bank/accounts", Method.Put);
            request.AddJsonBody(account);

            try
            {
                RestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Account data updated successfully.");
                }
                else
                {
                    MessageBox.Show($"Failed to update account data. Error: {response.Content ?? response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //Delete Account Button
        private void DeleteAccBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseInt(AccountIdInput.Text, out int retrieveAccountId, "Invalid account ID entered.")) return;

            RestRequest request = new RestRequest($"api/bank/accounts/{retrieveAccountId}", Method.Delete);

            try
            {
                RestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Account deleted successfully.");
                }
                else
                {
                    MessageBox.Show($"Failed to delete account data. Error: {response.Content ?? response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //Retrieve User Details Button
        private void RetrieveAccDetailsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseInt(AccountIdInput.Text, out int retrieveAccountId, "Invalid account ID entered.")) return;

            RestRequest accountRequest = new RestRequest($"api/bank/accounts/{retrieveAccountId}", Method.Get);

            try
            {
                RestResponse<Models.Account> accountResponse = client.Execute<Models.Account>(accountRequest);

                if (accountResponse.StatusCode == System.Net.HttpStatusCode.OK && accountResponse.Data != null)
                {
                    int retrievedUserId = accountResponse.Data.UserId;

                    RestRequest userRequest = new RestRequest("api/bank/users", Method.Get);
                    var userListResponse = client.Execute<List<Models.UserProfile>>(userRequest);

                    if (userListResponse.StatusCode == System.Net.HttpStatusCode.OK && userListResponse.Data != null)
                    {
                        var matchedUser = userListResponse.Data.FirstOrDefault(u => u.UserId == retrievedUserId);

                        if (matchedUser != null)
                        {
                            DisplayAccountHolderBox.Text = $"{matchedUser.FirstName} {matchedUser.LastName}";
                            DisplayBalanceBox.Text = accountResponse.Data.Balance.ToString();
                            DisplayAccountIdBox.Text = retrieveAccountId.ToString();

                            account = new Account
                            {
                                AccountId = accountResponse.Data.AccountId,
                                UserId = retrievedUserId,
                                Balance = accountResponse.Data.Balance
                            };
                        }
                        else
                        {
                            MessageBox.Show("No user found for the given account.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Failed to retrieve user list. Error: {userListResponse.Content ?? userListResponse.ErrorMessage}");
                    }
                }
                else
                {
                    MessageBox.Show($"Account ID does not exist. Error: {accountResponse.Content ?? accountResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //Create New Account Button
        private void CreateNewAcc_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseInt(UserIdInput.Text, out int userId, "Invalid user ID entered.")) return;
            if (!TryParseInt(BalanceInput.Text, out int balance, "Invalid balance entered.")) return;

            RestRequest userRequest = new RestRequest("api/bank/users", Method.Get);

            try
            {
                var userResponse = client.Execute<List<Models.UserProfile>>(userRequest);

                if (userResponse.Data != null && userResponse.Data.Any(u => u.UserId == userId))
                {
                    RestRequest accountRequest = new RestRequest("api/bank/accounts", Method.Get);
                    var accountResponse = client.Execute<List<Account>>(accountRequest);
                    int nextAccountId = (accountResponse.Data?.Count ?? 0) + 1;

                    Account newAccount = new Account
                    {
                        AccountId = nextAccountId,
                        UserId = userId,
                        Balance = balance
                    };

                    RestRequest createRequest = new RestRequest("api/bank/accounts", Method.Post);
                    createRequest.AddJsonBody(newAccount);
                    var createResponse = client.Execute(createRequest);

                    if (createResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show($"Successfully created account with AccountId: {nextAccountId}");
                    }
                    else
                    {
                        MessageBox.Show($"Failed to create account. Error: {createResponse.Content ?? createResponse.ErrorMessage}");
                    }
                }
                else
                {
                    MessageBox.Show($"User with UserId {userId} does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
