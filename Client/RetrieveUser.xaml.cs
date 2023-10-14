using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class RetrieveUser : Page
    {
        RestClient client = new RestClient("http://localhost:5235/");
        string picture;
        Models.UserProfile profile;

        public RetrieveUser()
        {
            InitializeComponent();
        }
        
        //Retrieve User Button
        private void RetrieveUserBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string identifier = SearchBox.Text;

                RestRequest request = new RestRequest($"api/bank/users/{identifier}", Method.Get);
                RestResponse<Models.UserProfile> response = client.Execute<Models.UserProfile>(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data == null)
                {
                    MessageBox.Show($"Failed to retrieve user data. Error: {response.ErrorMessage}");
                    return;
                }

                FirstNameBox.Text = response.Data.FirstName;
                LastNameBox.Text = response.Data.LastName;
                UsernameBox.Text = response.Data.Username;
                EmailBox.Text = response.Data.Email;
                AddressBox.Text = response.Data.Address;
                PhoneBox.Text = response.Data.Phone;
                PasswordBox.Text = response.Data.Password;
                picture = response.Data.Picture;
                profile = response.Data;
                UserIdBox.Text = "User ID: " + response.Data.UserId.ToString();

                string base64Image = response.Data.Picture;
                if (string.IsNullOrEmpty(base64Image) || !IsBase64String(base64Image))
                {
                    MessageBox.Show("Invalid image data retrieved.");
                    return;
                }

                UserPicture.Source = ConvertBase64ToImage(picture);

                MessageBox.Show("User data retrieved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //Update User Details Button
        private void UpdateUserDetailsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Models.UserProfile updatedUser = new Models.UserProfile
                {
                    FirstName = FirstNameBox.Text,
                    LastName = LastNameBox.Text,
                    Username = UsernameBox.Text,
                    Email = EmailBox.Text,
                    Address = AddressBox.Text,
                    Phone = PhoneBox.Text,
                    Password = PasswordBox.Text,
                    Picture = picture,
                    UserId = profile.UserId
                };

                RestRequest request = new RestRequest("api/bank", Method.Put);
                request.AddJsonBody(updatedUser);
                RestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("User data updated successfully.");
                }
                else
                {
                    MessageBox.Show($"Failed to update user data. Error: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        //Go to Create User UI
        private void CreateUserBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CreateUser());
        }

        //Go to main menu UI
        private void MainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SystemOptions());
        }

        //Conversion for Profile Picture
        private BitmapImage ConvertBase64ToImage(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to convert base64 string to image.");
                return null;
            }
        }

        private bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0
                || base64.Contains(" ") || base64.Contains("\t") || base64.Contains("\r") || base64.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Delete Account Button 
        private void DeleteAccBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (profile == null || string.IsNullOrEmpty(profile.Username))
                {
                    MessageBox.Show("Please retrieve a user profile before attempting to delete.");
                    return;
                }

                MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you want to delete the user: {profile.Username}?", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    return;
                }

                RestRequest request = new RestRequest($"api/bank/users/{profile.UserId}", Method.Delete);
                RestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("User deleted successfully.");
                }
                else
                {
                    MessageBox.Show($"Failed to delete user. Error: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}