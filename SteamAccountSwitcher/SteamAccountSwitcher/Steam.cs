﻿using System.Diagnostics;
using System.IO;

namespace SteamAccountSwitcher
{
    class Steam
    {
        string installDir;

        public Steam(string installDir)
        {
            this.installDir = installDir;
        }

        public string InstallDir
        {
            get { return installDir; }
            set { installDir = value; }
        }

        public bool IsSteamRunning()
        {
            Process[] pname = Process.GetProcessesByName("steam");
            if (pname.Length == 0)
                return false;
            else
                return true;
        }


        public bool StartSteamAccount(SteamAccount a)
        {
            bool finished = false;

            if (IsSteamRunning())
            {
                LogoutSteam();
            }

            while (finished == false)
            {
                if (IsSteamRunning() == false)
                {
                    Process p = new Process();
                    if (File.Exists(installDir))
                    {
                        p.StartInfo = new ProcessStartInfo(installDir, a.getStartParameters());
                        p.Start();
                        finished = true;
                        return true;
                    }
                }
            }
            return false;
        }


        public bool LogoutSteam()
        {
            Process p = new Process();
            if (File.Exists(installDir))
            {
                p.StartInfo = new ProcessStartInfo(installDir, "-shutdown");
                p.Start();
                return true;
            }
            return false;

        }
    }
}
