using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RedDeadOnlineCustomRoom
{

    public class SettingSave
    {
        /// 程序目录
        public string BasePath
        {
            get;
            set;
        }
        /// 配置文件路径
        public string Path
        {
            get;
            set;
        }
        /// 配置
        public Setting Setting { get; set; }

        /// 写配置
        public void WriteAccountsToFile()
        {
            string xml = this.ToXML<Setting>(Setting);
            StreamWriter file = new System.IO.StreamWriter(Path);
            file.Write(xml);
            file.Close();
        }

        /// 读配置
        public void ReadAccountsFromFile()
        {
            string text = System.IO.File.ReadAllText(Path);
            Setting = FromXML<Setting>(text);
        }

        private T FromXML<T>(string xml)
        {
            using (StringReader stringReader = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }

        private string ToXML<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        /// 初始化
        internal void Init()
        {
            Setting = new Setting();
            Setting.Init();
        }
    }
}
