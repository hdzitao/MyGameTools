using System;
using System.Windows;

namespace RedDeadOnlineCustomRoom
{
    /// <summary>
    /// EditRoom.xaml 的交互逻辑
    /// </summary>
    public partial class EditRoomWindow : Window
    {
        ///
        internal MainWindow mainWindow;
        /// 编辑的房间
        internal Room editRoom;
        /// 卡单文件内容
        internal string config;

        /// 新增
        public EditRoomWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
        }

        /// 修改
        public EditRoomWindow(Room editRoom, MainWindow mainWindow)
        {
            InitializeComponent();

            this.editRoom = editRoom;
            this.mainWindow = mainWindow;

            // 展示编辑房间信息
            roomNameTextBox.Text = editRoom.Name;
            roomIdTextBox.Text = editRoom.Id;
        }

        /// 确定按键
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            // 获取房间信息
            string name = roomNameTextBox.Text;
            string id = roomIdTextBox.Text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id))
            {
                MessageBox.Show("确认信息无误");
                return;
            }

            // 获取卡单文件
            StartupFile startupFile = new StartupFile(mainWindow.settingSave);
            // 卡单文件的内容,来自 自定义 ,如果空就用默认的卡单文件内容
            startupFile.Config = String.IsNullOrEmpty(config) ? AppResources.startup + id : config;

            if (editRoom != null)
            {/// 编辑
                // 先删除原来的卡单文件
                startupFile.Room = editRoom;
                startupFile.Clear();
                // 保存新的卡单文件
                editRoom.Name = name;
                editRoom.Id = id;
                startupFile.Save();
            }
            else
            {/// 新增
                // 添加到房间列表
                Room room = new Room();
                room.Name = name;
                room.Id = id;
                mainWindow.settingSave.Setting.Rooms.Add(room);
                // 保存卡单文件
                startupFile.Room = room;
                startupFile.Save();
            }

            // 重载配置
            mainWindow.SaveAndReloadSetting();

            // 关闭
            Close();
        }

        // 自定义
        private void Button_Config(object sender, RoutedEventArgs e)
        {
            EditRoomConfigWindow editRoomConfig = new EditRoomConfigWindow(this);
            editRoomConfig.ShowDialog();
        }
    }
}
