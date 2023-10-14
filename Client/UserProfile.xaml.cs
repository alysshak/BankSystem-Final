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
    /// <summary>
    /// Interaction logic for UserProfile.xaml
    /// </summary>
    public partial class UserProfile : Page
    {
        public UserProfile()
        {
            InitializeComponent();
        }

        private void CreateUserBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CreateUser());
        }

        private void RetrieveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RetrieveUser());
        }
    }

}
