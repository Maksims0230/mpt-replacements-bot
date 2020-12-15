using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TestTelegramBotApi.GetUpdate;

namespace TestTelegramBotApi
{
    namespace Users
    {
        [Serializable]
        public class Users
        {
            public static List <User> users { get { return Firebase.Firebase.SelectUsers().users; } set { Firebase.Firebase.InsertUsers(value);  } }
            public static List<User> _users { get { return GetUsers(); } set { SetUsers(value); } }
            private static List<User> GetUsers()
            {
                try
                {
                    using (FileStream fs = new FileStream("Users.xml", FileMode.OpenOrCreate))
                        return (List<User>)new XmlSerializer(typeof(List<User>)).Deserialize(fs);
                }
                catch { return new List<User>(); }
            }
            private static void SetUsers(object obj)
            {
                using (FileStream fs = new FileStream("Users.xml", FileMode.Create))
                    new XmlSerializer(typeof(List<User>)).Serialize(fs, obj);
            }
        }

        [Serializable]
        public class User
        {
            public string group { get; set; }
            public bool notif_on_off { get; set; }
            public bool replace_on_off { get; set; }
            public From from { get; set; }
        }
    }
}