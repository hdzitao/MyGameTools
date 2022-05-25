using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace RedDeadOnlineCustomRoom
{
    /// <summary>
    /// EditRoomConfig.xaml 的交互逻辑
    /// </summary>
    public partial class EditRoomConfigWindow : Window
    {
        /// 编辑房间界面
        private EditRoomWindow editRoomWindow;

        public EditRoomConfigWindow(EditRoomWindow editRoomWindow)
        {
            InitializeComponent();

            this.editRoomWindow = editRoomWindow;

            // 获取编辑的房间
            Room editRoom;
            if (!string.IsNullOrEmpty(editRoomWindow.config))
            {
                // 第二次点击自定义时，拿上一次配置好的内容
                configTextBox.Text = editRoomWindow.config;
            }
            else if ((editRoom = editRoomWindow.editRoom) != null)
            {   // 编辑房间就读取原来的卡单文件
                StartupFile startupFile = new StartupFile(editRoomWindow.mainWindow.settingSave, editRoom);
                configTextBox.Text = startupFile.Read();
            }
            else
            {
                // 默认卡单文件内容
                configTextBox.Text = AppResources.startup;
            }
        }

        // 关闭时调用
        private void Save_Config(object sender, EventArgs e)
        {
            // 读取内容
            string text = configTextBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                // 直接使用自定义内容
                editRoomWindow.config = text;
                // 自动获取房间ID
                int index = text.LastIndexOf('>');
                editRoomWindow.roomIdTextBox.Text = Regex.Replace(text.Substring(index + 1), @"[^0-9a-zA-Z]", "");
            }
        }
    }
}
