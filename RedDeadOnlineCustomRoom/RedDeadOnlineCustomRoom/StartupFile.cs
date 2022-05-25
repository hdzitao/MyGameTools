using System.IO;

namespace RedDeadOnlineCustomRoom
{
    internal class StartupFile
    {
        ///
        internal SettingSave settingSave { get; set; }

        /// 卡单文件内容
        internal string Config { get; set; }
        /// 对应的房间
        internal Room Room { get; set; }

        public StartupFile(SettingSave settingSave)
        {
            this.settingSave = settingSave;
        }

        public StartupFile(SettingSave settingSave, Room room)
        {
            this.settingSave = settingSave;
            this.Room = room;
        }

        /// 保存卡单文件
        public void Save()
        {
            if (settingSave == null || Config == null || Room == null)
            {
                return;
            }

            string path = fileName();
            StreamWriter file = new StreamWriter(path);
            file.Write(Config);
            file.Close();

        }

        /// 删除卡单文件
        public void Clear()
        {
            string path = fileName();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// 复制卡单文件到大表哥
        public void CopyToRedDead()
        {
            if (settingSave == null || Room == null)
            {
                return;
            }
            string path = fileName();
            if (File.Exists(path))
            {
                File.Copy(path, settingSave.Setting.StartupPath(), true);
            }          
        }

        /// 读取卡单文件
        public string Read()
        {
            string path = fileName();
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                return null;
            }
        }

        /// 卡单文件路径
        private string fileName()
        {
            return settingSave.BasePath + "\\startup_" + Room.Id + ".meta";
        }
    }
}
