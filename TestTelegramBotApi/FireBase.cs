using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTelegramBotApi
{
    namespace Firebase
    {
        public class Firebase
        {
            private static IFirebaseConfig _firebaseConfig = new FirebaseConfig
            {
                AuthSecret = "3wbwpmXoUwcFxj6sGohDgdzWzrnEdptFkSXGfVNJ",
                BasePath = "https://mpt-replaceent-s-bot-default-rtdb.firebaseio.com/",
            };

            public static long _update_id = 255812930;

            public static long? set_get_id { get { return JsonConvert.DeserializeObject<long>(_firebaseClient.Get("/update_id").Body.ToString()) as long?; } set { _firebaseClient.Set($"/update_id", value); } }

            private static IFirebaseClient _firebaseClient = new FirebaseClient(_firebaseConfig);

            (IFirebaseConfig config, bool ok) firebaseConfig { get { return (_firebaseConfig, _firebaseConfig != null); } set { _firebaseConfig = value.config; } }

            (IFirebaseClient client, bool ok) firebaseClient { get { return (_firebaseClient, _firebaseClient != null); } set { _firebaseClient = value.client; } }

            public static void InsertUsers(List<Users.User> users)
            {
                var data = users;

                foreach (var user in data)
                    _firebaseClient.Set($"Users/{user.from.id}", user);
            }
            
            public static void InsertUser(Users.User user) => _firebaseClient.Set($"Users/{user.from.id}", user);

            public static (bool ok, Users.User user) SelectUser(long from_id)
            {
                var result = _firebaseClient.Get($"Users/{from_id}");
                Users.User user = result.ResultAs<Users.User>();
                if (user != null)
                    return (true, user);
                else
                    return (false, user);
            }

            public static (bool ok, List<Users.User> users) SelectUsers()
            {
                var result = _firebaseClient.Get("Users");
                Dictionary<string, Users.User> data = JsonConvert.DeserializeObject<Dictionary<string, Users.User>>(result.Body.ToString());
                List<Users.User> users = new List<Users.User>();

                foreach (var item in data)
                    users.Add(item.Value);

                if (users.Count != 0)
                    return (true, users);
                else
                    return (false, users);
            }
            async public static Task<(bool ok, List<Users.User> users)> AsyncSelectUsers()
            {
                var result = await _firebaseClient.GetAsync("Users");
                Dictionary<string, Users.User> data = JsonConvert.DeserializeObject<Dictionary<string, Users.User>>(result.Body.ToString());
                List<Users.User> users = new List<Users.User>();

                foreach (var item in data)
                    users.Add(item.Value);

                if (users.Count != 0)
                    return (true, users);
                else
                    return (false, users);
            }

            public static void UpdateUsers()
            {
                var data = Users.Users.users;

                foreach (var user in data)
                    _firebaseClient.Update($"Users/{user.from.id}", user);
            }

            public static bool UpdateUser(long from_id)
            {
                var data = Users.Users.users;
                if (data.Where(u => u.from.id == from_id).ToList().Count > 0)
                {
                    var user = data.Where(u => u.from.id == from_id).First();
                    _firebaseClient.Update($"Users/{from_id}", user);
                    return true;
                }
                else
                    return false;
            }

            public static bool UpdateUser(Users.User user)
            {
                _firebaseClient.Update($"Users/{user.from.id}", user);
                return true;
            }

            public static void DeleteUser(long from_id) => _firebaseClient.Delete($"Users/{from_id}");
        }
    }

}
