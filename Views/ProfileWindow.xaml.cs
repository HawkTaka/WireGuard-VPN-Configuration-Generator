using System.Windows;
using VPNConfigGen.Models;

namespace VPNConfigGen.Views
{
    public partial class ProfileWindow : Window
    {
        public ServerProfile Profile { get; set; }
        
        public ProfileWindow(ServerProfile profile = null)
        {
            InitializeComponent();
            
            Profile = profile ?? new ServerProfile();
            DataContext = Profile;
        }
        
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}