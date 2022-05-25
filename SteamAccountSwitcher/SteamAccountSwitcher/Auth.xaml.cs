using System.Windows;
using System.Windows.Input;

namespace SteamAccountSwitcher
{
    /// <summary>
    /// Auth.xaml 的交互逻辑
    /// </summary>
    public partial class Auth : Window
    {
        SettingSave settingsSave;
        string pw;

        public string PW
        {
            get { return pw; }
        }

        public Auth(SettingSave settingsSave)
        {
            this.settingsSave = settingsSave;
            InitializeComponent();
            pwBoxPasswd.Focus();
        }

        private void entry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonEntry_Click(null, null);
            }
        }

        private void buttonEntry_Click(object sender, RoutedEventArgs e)
        {
            pw = pwBoxPasswd.Password;
            if (pw != null && pw != "")
            {
                Close();
            }
            else
            {
                pwBoxPasswd.Focus();
            }
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("重置配置文件?", "重置", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    System.IO.File.Delete(settingsSave.Path);
                    MessageBox.Show("重置配置文件成功", "重置成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show("重置配置文件失败", "重置失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
