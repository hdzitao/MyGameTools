using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SteamAccountSwitcher
{
    public class SettingSave
    {
        public string Path
        {
            get;
            set;
        }

        public Crypto Crypt
        {
            get;
            set;
        }

        public AccountList AccountLis
        {
            get;
            set;
        }

        public void WriteAccountsToFile()
        {
            string xmlAccounts = this.ToXML<AccountList>(AccountLis);
            StreamWriter file = new System.IO.StreamWriter(Path);
            file.Write(Crypt.Encrypt(xmlAccounts));
            file.Close();
        }

        public AccountList ReadAccountsFromFile()
        {
            string text = System.IO.File.ReadAllText(Path);
            AccountLis = FromXML<AccountList>(Crypt.Decrypt(text));
            return AccountLis;
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
    }
}
