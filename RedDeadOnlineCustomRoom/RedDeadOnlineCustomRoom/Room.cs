namespace RedDeadOnlineCustomRoom
{
    public class Room
    {
        /// 房间名
        public string Name { get; set; }
        /// 房间Id
        public string Id { get; set; }

        override public string ToString()
        {
            return Name + " - " + Id;
        }


        /// 默认配置,未定义的房间
        public static Room Unknown()
        {
            Room room = new Room();
            room.Name = "未设定";
            room.Id = "";
            return room;
        }
    }
}
