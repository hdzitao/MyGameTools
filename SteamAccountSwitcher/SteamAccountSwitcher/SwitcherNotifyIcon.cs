using System.Windows.Forms;

namespace SteamAccountSwitcher
{
    public class SwitcherNotifyIcon
    {
        private MainWindow main;
        private NotifyIcon notify;

        public bool Visible
        {
            get => notify.Visible;
            set => notify.Visible = value;
        }

        public SwitcherNotifyIcon(MainWindow main)
        {
            this.main = main;
            notify = new NotifyIcon();
            notify.Text = "Steam账号管理器";
            notify.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notify.Visible = true;
            notify.ContextMenuStrip = new ContextMenuStrip();
            notify.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            BuildMenu();
            notify.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    main.Show();
                }
            };
        }

        public void BuildMenu()
        {
            notify.ContextMenuStrip.Items.Clear();
            foreach (SteamAccount account in main.settingSave.AccountLis.Accounts)
            {
                notify.ContextMenuStrip.Items.Add(account.Name, null, (sender, enentArgs) =>
                {
                    main.steam.StartSteamAccount(account);
                });
            }
            notify.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            var t = notify.ContextMenuStrip.Items.Add("显示", null, (sender, enentArgs) =>
            {
                main.Show();
            });
            notify.ContextMenuStrip.Items.Add("关闭", null, (sender, enentArgs) =>
            {
                main.settingSave.WriteAccountsToFile();


                Properties.Settings.Default.Top = main.Top;
                Properties.Settings.Default.Left = main.Left;
                Properties.Settings.Default.Height = main.Height;
                Properties.Settings.Default.Width = main.Width;
                Properties.Settings.Default.Maximized = false;

                Properties.Settings.Default.Save();

                notify.Dispose();
                main.Close();
                System.Environment.Exit(0);
            });
        }
    }
}
