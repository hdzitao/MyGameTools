using System.Collections.Generic;

namespace SteamAccountSwitcher
{
    public class AccountList
    {
        string installDir;
        List<SteamAccount> accounts;

        public AccountList()
        {
            accounts = new List<SteamAccount>();
        }

        public string InstallDir
        {
            get { return installDir; }
            set { installDir = value; }
        }

        public List<SteamAccount> Accounts
        {
            get { return accounts; }
            set { accounts = value; }
        }


    }
}
