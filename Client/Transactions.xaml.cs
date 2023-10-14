using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    public partial class Transactions : Page
    {
        RestClient client = new RestClient("http://localhost:5235/");

        public Transactions()
        {
            InitializeComponent();
        }

        //Deposit Button
        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(AccountNumberBox.Text, out int retrieveAccountId))
                {
                    MessageBox.Show("Invalid account number format.");
                    return;
                }

                if (!decimal.TryParse(TransactionAmountBox.Text, out decimal depositAmount))
                {
                    MessageBox.Show("Invalid deposit amount format.");
                    return;
                }

                if (depositAmount <= 0)
                {
                    MessageBox.Show("Invalid deposit amount.");
                    return;
                }

                RestRequest accountRequest = new RestRequest($"api/bank/accounts/{retrieveAccountId}", Method.Get);
                RestResponse<Models.Account> accountResponse = client.Execute<Models.Account>(accountRequest);

                if (accountResponse.StatusCode != System.Net.HttpStatusCode.OK || accountResponse.Data == null)
                {
                    MessageBox.Show($"Failed to retrieve account data. Error: {accountResponse.ErrorMessage}");
                    return;
                }

                Models.Account retrievedAccount = accountResponse.Data;
                InitialBalanceBox.Text = retrievedAccount.Balance.ToString();

                retrievedAccount.Balance += depositAmount;

                RestRequest updateRequest = new RestRequest($"api/bank/accounts", Method.Put);
                updateRequest.AddJsonBody(retrievedAccount);
                RestResponse updateResponse = client.Execute(updateRequest);

                if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Models.Transaction newTransaction = new Models.Transaction
                    {
                        AccountId = retrieveAccountId,
                        TransactionType = "Deposit",
                        Amount = (double)depositAmount,
                        TransactionDate = DateTime.Now
                    };

                    RestRequest transactionRequest = new RestRequest("api/bank/transactions", Method.Post);
                    transactionRequest.AddJsonBody(newTransaction);
                    RestResponse transactionResponse = client.Execute(transactionRequest);

                    if (transactionResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Transaction recorded successfully.");
                        LoadTransactions(retrieveAccountId);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to record transaction. Error: {transactionResponse.ErrorMessage}");
                    }

                    BalanceBox.Text = retrievedAccount.Balance.ToString("C");
                    MessageBox.Show($"Successfully deposited {depositAmount:C}. New balance: {retrievedAccount.Balance:C}");
                }
                else
                {
                    MessageBox.Show($"Failed to update account after deposit. Error: {updateResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //Withdraw Button
        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(AccountNumberBox.Text, out int retrieveAccountId))
                {
                    MessageBox.Show("Invalid account number format.");
                    return;
                }

                if (!decimal.TryParse(TransactionAmountBox.Text, out decimal withdrawAmount))
                {
                    MessageBox.Show("Invalid withdrawal amount format.");
                    return;
                }

                RestRequest accountRequest = new RestRequest($"api/bank/accounts/{retrieveAccountId}", Method.Get);
                RestResponse<Models.Account> accountResponse = client.Execute<Models.Account>(accountRequest);

                if (accountResponse.StatusCode != System.Net.HttpStatusCode.OK || accountResponse.Data == null)
                {
                    MessageBox.Show($"Failed to retrieve account data. Error: {accountResponse.ErrorMessage}");
                    return;
                }

                Models.Account retrievedAccount = accountResponse.Data;

                if (withdrawAmount <= 0)
                {
                    MessageBox.Show("Invalid withdrawal amount.");
                    return;
                }

                if (withdrawAmount > retrievedAccount.Balance)
                {
                    MessageBox.Show("Insufficient funds for withdrawal.");
                    return;
                }

                retrievedAccount.Balance -= withdrawAmount;

                RestRequest updateRequest = new RestRequest($"api/bank/accounts", Method.Put);
                updateRequest.AddJsonBody(retrievedAccount);
                RestResponse updateResponse = client.Execute(updateRequest);

                if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Models.Transaction newTransaction = new Models.Transaction
                    {
                        AccountId = retrieveAccountId,
                        TransactionType = "Withdrawal",
                        Amount = (double)withdrawAmount,
                        TransactionDate = DateTime.Now
                    };

                    RestRequest transactionRequest = new RestRequest("api/bank/transactions", Method.Post);
                    transactionRequest.AddJsonBody(newTransaction);
                    RestResponse transactionResponse = client.Execute(transactionRequest);

                    if (transactionResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Transaction recorded successfully.");
                        LoadTransactions(retrieveAccountId);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to record transaction. Error: {transactionResponse.ErrorMessage}");
                    }

                    BalanceBox.Text = retrievedAccount.Balance.ToString("C");
                    MessageBox.Show($"Successfully withdrew {withdrawAmount:C}. New balance: {retrievedAccount.Balance:C}");
                }
                else
                {
                    MessageBox.Show($"Failed to update account after withdrawal. Error: {updateResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //Load Transactions onto Group Box
        private void LoadTransactions(int accountId)
        {
            try
            {
                RestRequest request = new RestRequest($"api/bank/transactions/account/{accountId}", Method.Get);
                RestResponse<List<Models.Transaction>> response = client.Execute<List<Models.Transaction>>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TransactionListView.ItemsSource = response.Data;
                }
                else
                {
                    MessageBox.Show($"Failed to load transactions for account {accountId}. Error: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}