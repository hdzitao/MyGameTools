using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteamAccountSwitcher
{
    /// ****
    /// SteamAccountSwitcher
    /// Copyright by Christoph Wedenig
    /// ****

    public partial class MainWindow : Window
    {
        internal Steam steam;
        internal SettingSave settingSave;

        internal SwitcherNotifyIcon notifyIcon;

        public MainWindow()
        {
            IsSwitcherRunning();

            InitializeComponent();

            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            this.settingSave = new SettingSave();

            //Get directory of Executable
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).TrimStart(@"file:\\".ToCharArray());
            path += "\\accounts.ini";
            settingSave.Path = path;

            Auth auth = new Auth(settingSave);
            auth.ShowDialog();
            if (auth.PW == null || auth.PW == "")
            {
                Process.GetCurrentProcess().Kill();
            }
            settingSave.Crypt = new Crypto(auth.PW);

            this.buttonInfo.ToolTip = "Build Version: " + Assembly.GetEntryAssembly().GetName().Version.ToString();

            try
            {
                settingSave.ReadAccountsFromFile();
            }
            catch (FileNotFoundException ignore)
            {
                //Maybe create file?
            }
            catch (Exception e)
            {
                MessageBox.Show("解密失败,可能密码错误", "即将退出", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }

            AccountList accountList = settingSave.AccountLis;
            if (accountList == null)
            {
                accountList = new AccountList();
                settingSave.AccountLis = accountList;
            }

            listBoxAccounts.ItemsSource = accountList.Accounts;
            listBoxAccounts.Items.Refresh();

            if (accountList.InstallDir == "" || (accountList.InstallDir == null))
            {
                accountList.InstallDir = SelectSteamFile(@"C:\Program Files (x86)\Steam");
                if (accountList.InstallDir == null)
                {
                    MessageBox.Show("请选择您的Steam程序", "缺少Steam程序", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            steam = new Steam(accountList.InstallDir);

            notifyIcon = new SwitcherNotifyIcon(this);

        }

        private string SelectSteamFile(string initialDirectory)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "Steam |steam.exe";
            dialog.InitialDirectory = initialDirectory;
            dialog.Title = "选择您的Steam";
            return (dialog.ShowDialog() == true)
               ? dialog.FileName : null;
        }

        private void buttonLogout_Click(object sender, RoutedEventArgs e)
        {
            steam.LogoutSteam();
        }

        private void buttonPassword_Click(object sender, RoutedEventArgs e)
        {
            Auth auth = new Auth(this.settingSave);
            auth.ShowDialog();
            string ps = auth.PW;
            if (ps != null && ps != "")
            {
                this.settingSave.Crypt.update(ps);
                this.settingSave.WriteAccountsToFile();
            }
        }

        private void buttonAddAccount_Click(object sender, RoutedEventArgs e)
        {
            AddAccount newAccWindow = new AddAccount();
            newAccWindow.Owner = this;
            newAccWindow.ShowDialog();

            if (newAccWindow.Account != null)
            {
                settingSave.AccountLis.Accounts.Add(newAccWindow.Account);

                refreshAccountList();
            }
        }


        private void listBoxAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SteamAccount selectedAcc = (SteamAccount)listBoxAccounts.SelectedItem;
            steam.StartSteamAccount(selectedAcc);
        }


        private void buttonEditAccount_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAccounts.SelectedItem != null)
            {
                AddAccount newAccWindow = new AddAccount((SteamAccount)listBoxAccounts.SelectedItem);
                newAccWindow.Owner = this;
                newAccWindow.ShowDialog();

                if (newAccWindow.Account.Username != "" && newAccWindow.Account.Password != "")
                {
                    settingSave.AccountLis.Accounts[listBoxAccounts.SelectedIndex] = newAccWindow.Account;

                    refreshAccountList();
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image itemClicked = (Image)e.Source;

            SteamAccount selectedAcc = (SteamAccount)itemClicked.DataContext;
            MessageBoxResult dialogResult = MessageBox.Show("确认删除'" + selectedAcc.Name + "'?", "删除账号", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                settingSave.AccountLis.Accounts.Remove((SteamAccount)listBoxAccounts.SelectedItem);
                refreshAccountList();
            }
            else if (dialogResult == MessageBoxResult.No)
            {
                //do something else
            }
        }

        private void IsSwitcherRunning()
        {
            Process[] pname = Process.GetProcessesByName("SteamAccountSwitcher");
            int currentId = Process.GetCurrentProcess().Id;
            foreach (Process process in pname)
            {
                if (process.Id != currentId)
                {
                    MessageBoxResult dialogResult = MessageBox.Show("SteamAccountSwitcher已启动，确认是否强制重启?", "重启或取消", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dialogResult == MessageBoxResult.OK)
                    {
                        process.Kill();
                    }
                    else
                    {
                        System.Environment.Exit(-1);
                    }
                }
            }
        }

        private void refreshAccountList()
        {
            listBoxAccounts.Items.Refresh();
            notifyIcon.BuildMenu();
            settingSave.WriteAccountsToFile();
        }

    }
}
