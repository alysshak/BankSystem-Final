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
    /// Interaction logic for SystemOptions.xaml
    /// </summary>
    public partial class SystemOptions : Page
    {
        public SystemOptions()
        {
            InitializeComponent();
        }

        private void TransactionBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Transactions());
        }

        private void AccountsBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Accounts());
        }

        private void UsersBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new UserProfile());
        }
    }
}
