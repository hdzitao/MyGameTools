using System.Collections.Generic;

namespace RedDeadOnlineCustomRoom
{
    public class Setting
    {
        /// 大表哥路径
        public string RedDeadPath { get; set; }
        /// 当前房间
        public Room CurrentRoom { get; set; }
        /// 房间列表
        public List<Room> Rooms { get; set; }
        
        /// 初始化
        public void Init()
        {
            RedDeadPath = "";
            Rooms = new List<Room>();
            CurrentRoom = Room.Unknown();
        }

        /// 大表哥目录下卡单文件位置
        public string StartupPath()
        {
            return RedDeadPath + "\\x64\\data\\startup.meta";
        }
    }
}
