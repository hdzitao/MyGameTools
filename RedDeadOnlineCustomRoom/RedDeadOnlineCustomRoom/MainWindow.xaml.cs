using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace RedDeadOnlineCustomRoom
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        internal SettingSave settingSave;

        public MainWindow()
        {
            InitializeComponent();

            settingSave = new SettingSave();

            // 获取执行文件目录
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).TrimStart(@"file:\\".ToCharArray());
            settingSave.BasePath = path;
            // 配置文件
            path += "\\setting.ini";
            settingSave.Path = path;
            // 载入配置
            ReloadSetting();

        }

        /// 载入配置
        public void ReloadSetting()
        {
            try
            {
                // 读文件
                settingSave.ReadAccountsFromFile();
            }
            catch (FileNotFoundException ignore)
            {
                // 没有配置，初始化
                settingSave.Init();
            }

            Setting setting = settingSave.Setting;
            if (setting != null)
            {
                // 大表哥路径
                string redDeadPath = setting.RedDeadPath;
                if (redDeadPath != null)
                {
                    redDeadPathTextBox.Text = redDeadPath;
                }
                // 当前房间
                Room currentRoom = setting.CurrentRoom;
                if (currentRoom != null)
                {
                    currentRoomTextBox.Text = currentRoom.ToString();
                }
                // 房间列表
                List<Room> rooms = setting.Rooms;
                if (rooms != null)
                {
                    // 之前选择的房间
                    int selectedIndex = roomComboBox.SelectedIndex;
                    // 更新房间列表
                    roomComboBox.ItemsSource = rooms;
                    roomComboBox.Items.Refresh();
                    // 选中之前选择的房间
                    roomComboBox.SelectedIndex = selectedIndex;
                }
            }
        }

        /// 保存并重载配置
        public void SaveAndReloadSetting()
        {
            settingSave.WriteAccountsToFile();
            ReloadSetting();
        }

        /// 编辑大表哥路径
        private void Button_EditRedDeadPath(object sender, RoutedEventArgs e)
        {
            // 选择大表哥目录
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            string redDeadPath = dialog.SelectedPath;
            // 选择后保存重载配置
            if (!string.IsNullOrEmpty(redDeadPath))
            {
                settingSave.Setting.RedDeadPath = redDeadPath;
                SaveAndReloadSetting();
            }
        }

        /// 查看大表哥路径
        private void Button_ShowRedDeadPath(object sender, RoutedEventArgs e)
        {
            // 先检查大表哥路径
            if (!checkRedDeadPath())
            {
                System.Windows.MessageBox.Show("未选择大表哥路径！");
                return;
            }

            Process.Start(settingSave.Setting.RedDeadPath);
        }

        /// 添加房间
        private void Button_Add_Room(object sender, RoutedEventArgs e)
        {
            // 弹出编辑房间的弹窗,并由其完成添加房间的功能
            new EditRoomWindow(this).ShowDialog();
        }

        /// 删除房间
        private void Button_Del_Room(object sender, RoutedEventArgs e)
        {
            // 获取当前选择的房间
            Room selectedRoom = (Room) roomComboBox.SelectedItem;
            if (selectedRoom == null)
            {
                System.Windows.MessageBox.Show("未选择房间！");
                return;
            }
            // 弹窗确认
            MessageBoxResult dialogResult = System.Windows.MessageBox.Show("确认删除房间：" + selectedRoom.ToString());
            if (dialogResult == MessageBoxResult.OK)
            {
                // 删除配置中的房间
                settingSave.Setting.Rooms.Remove(selectedRoom);
                // 删除对应的卡单文件
                StartupFile startupFile = new StartupFile(settingSave, selectedRoom);
                startupFile.Clear();
                // 取消选择
                roomComboBox.SelectedIndex = -1;
                // 更新
                SaveAndReloadSetting();
            }
        }

        /// 编辑房间
        private void Button_Edit_Room(object sender, RoutedEventArgs e)
        {
            // 获取当前选择的房间
            Room selectedRoom = (Room)roomComboBox.SelectedItem;
            if (selectedRoom == null)
            {
                System.Windows.MessageBox.Show("未选择房间！");
                return;
            }
            // 弹出编辑房间的弹窗,并由其完成编辑房间的功能
            new EditRoomWindow(selectedRoom, this).ShowDialog();
        }

        /// 应用房间
        private void Button_Apply(object sender, RoutedEventArgs e)
        {
            // 先检查大表哥路径
            if (!checkRedDeadPath())
            {
                System.Windows.MessageBox.Show("未选择大表哥路径！");
                return;
            }
            // 获取当前选择的房间
            Room selectedRoom = (Room)roomComboBox.SelectedItem;
            if (selectedRoom == null)
            {
                System.Windows.MessageBox.Show("未选择房间！");
                return;
            }
            // 拷贝卡单文件到大表哥目录下
            StartupFile startupFile = new StartupFile(settingSave, selectedRoom);
            startupFile.CopyToRedDead();
            // 更新当前应用的房间
            settingSave.Setting.CurrentRoom = selectedRoom;
            SaveAndReloadSetting();

            System.Windows.MessageBox.Show("成功应用房间：" + selectedRoom + "，请重启游戏。");
        }

        /// 取消卡单
        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            // 先检查大表哥路径
            if (!checkRedDeadPath())
            {
                System.Windows.MessageBox.Show("未选择大表哥路径！");
                return;
            }
            // 获取卡单文件路径
            string startupPath = settingSave.Setting.StartupPath();
            // 删除卡单文件
            if (File.Exists(startupPath))
            {
                File.Delete(startupPath);
            }
            // 更新
            settingSave.Setting.CurrentRoom = Room.Unknown();
            SaveAndReloadSetting();
        }

        // 检查大表哥路径
        private bool checkRedDeadPath()
        {
            return !string.IsNullOrEmpty(settingSave.Setting.RedDeadPath);
        }
    }
}
