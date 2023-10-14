using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class CreateUser : Page
    {
        Models.UserProfile newUser;

        public CreateUser()
        {
            InitializeComponent();
            newUser = new Models.UserProfile();
        }

        private bool AreAllFieldsFilled()
        {
            return !string.IsNullOrWhiteSpace(FirstNameBox.Text) &&
                   !string.IsNullOrWhiteSpace(LastNameBox.Text) &&
                   !string.IsNullOrWhiteSpace(EmailBox.Text) &&
                   !string.IsNullOrWhiteSpace(PasswordBox.Text) &&
                   !string.IsNullOrWhiteSpace(AddressBox.Text) &&
                   !string.IsNullOrWhiteSpace(PhoneBox.Text) &&
                   !string.IsNullOrWhiteSpace(UsernameBox.Text);
        }

        private void MainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SystemOptions());
        }

        private void RetrieveUserBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RetrieveUser());
        }

        private void CreateNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AreAllFieldsFilled())
            {
                MessageBox.Show("Please fill all fields before proceeding.");
                return;
            }

            RestClient restClient = new RestClient("http://localhost:5235");

            try
            {
                RestRequest getUsersRequest = new RestRequest("/api/bank/users", Method.Get);
                RestResponse<List<Models.UserProfile>> getUsersResponse = restClient.Execute<List<Models.UserProfile>>(getUsersRequest);

                if (getUsersResponse.StatusCode != System.Net.HttpStatusCode.OK || getUsersResponse.Data == null)
                {
                    MessageBox.Show("Error fetching user data. Please try again.");
                    return;
                }

                if (getUsersResponse.Data.Any(user => user.Username == UsernameBox.Text))
                {
                    MessageBox.Show("Username already exists");
                    return;
                }

                newUser.UserId = getUsersResponse.Data.Count + 1;
                newUser.FirstName = FirstNameBox.Text;
                newUser.LastName = LastNameBox.Text;
                newUser.Email = EmailBox.Text;
                newUser.Password = PasswordBox.Text;
                newUser.Address = AddressBox.Text;
                newUser.Phone = PhoneBox.Text;
                newUser.Username = UsernameBox.Text;

                RestRequest restRequest = new RestRequest("/api/bank/adduser", Method.Post);
                restRequest.AddJsonBody(newUser);
                RestResponse restResponse = restClient.Execute(restRequest);

                if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show($"Successfully created user {newUser.FullName}\nUser id is: " + newUser.UserId);
                }
                else
                {
                    MessageBox.Show($"Failed to create user. Error: {restResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void UploadDP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Added New Profile Picture to User Profile");

                newUser.Picture = GenerateRedImageAsBase64(64, 64);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new System.IO.MemoryStream(System.Convert.FromBase64String(newUser.Picture));
                bi.EndInit();
                ProfilePicture.Source = bi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while uploading the profile picture: {ex.Message}");
            }
        }

        private static string GenerateRedImageAsBase64(int width, int height)
        {
            try
            {
                using (Bitmap bmp = new System.Drawing.Bitmap(width, height))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.Clear(System.Drawing.Color.Red);
                    }
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}