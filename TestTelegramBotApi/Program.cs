using MPTLib.Replaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TestTelegramBotApi
{
    public static class DataBot
    {
        private const string botNum = "1459518290";
        private const string token = "AAHLGFWgljMCgR8KqldmoitU1kTQQTKYhoU";
        private static string link = $"https://api.telegram.org/bot{botNum}:{token}";

        // https://api.telegram.org/bot1459518290:AAHLGFWgljMCgR8KqldmoitU1kTQQTKYhoU/getUpdates

        /// <summary>
        /// Получить ответ на запрос в json формате.
        /// </summary>
        /// <param name="link">Запрос.</param>
        /// <returns>json in string</returns>
        public static string GetResponce_in_Json(long ID_Offset = 0) => new WebClient().DownloadString($"{DataBot.link}/getUpdates?offset={ID_Offset}");
        /// <summary>
        /// Десереализация JSONObject содержащий ответ на запрос getUpdate в класс с заранее определённые полями.
        /// </summary>
        /// <returns>GetUpdate</returns>
        public static GetUpdate.GetUpdate getUpdates(long ID_Offset)
        {
            var json_str = GetResponce_in_Json(ID_Offset);
            if (json_str.Trim() != "")
                return JsonConvert.DeserializeObject<GetUpdate.GetUpdate>(json_str);
            else
                return new GetUpdate.GetUpdate() { ok = false, result = null };
        }


        public static IEnumerable<string> Split(this string text, int size)
        {
            for (var i = 0; i < text.Length; i += size)
                yield return text.Substring(i, Math.Min(size, text.Length - i));
        }

        public static void SendMessage(this string text, long chat_id, bool WrappingText = true, bool DoubleSpace = false)
        {
            text = !WrappingText ? (!DoubleSpace ? new Regex(@"\s+").Replace(text.Replace("\r\n", "").Replace("\n", "").Replace("\r", ""), " ") : text.Replace("\r\n", "").Replace("\n", "").Replace("\r", "")) : (DoubleSpace ? new Regex(@"\s+").Replace(text.Trim(), " ") : text.Trim());
            using (var webClient = new WebClient())
            {
                try
                {
                    if (text.Length > 4095)
                    {
                        text.Split(4095).ToList().ForEach(x =>
                        {
                            var parts = new System.Collections.Specialized.NameValueCollection();
                            parts.Add("text", x);
                            parts.Add("chat_id", chat_id.ToString());
                            webClient.UploadValues($"{DataBot.link}/sendMessage", parts);
                        });
                    }
                    else if (text.Trim() != "")
                    {
                        var parts = new System.Collections.Specialized.NameValueCollection();
                        parts.Add("text", text);
                        parts.Add("chat_id", chat_id.ToString());
                        webClient.UploadValues($"{DataBot.link}/sendMessage", parts);
                    }
                    else
                    {
                        var parts = new System.Collections.Specialized.NameValueCollection();
                        parts.Add("text", "Пустой ответ!");
                        parts.Add("chat_id", chat_id.ToString());
                        webClient.UploadValues($"{DataBot.link}/sendMessage", parts);
                    }
                }
                catch (Exception e) { $"Ошибка при отпраки ответа.\r\n{e.ToString()}".SendMessage(chat_id); }
            }
        }

    }


    class Program
    {

        static private string Win1251ToUTF8(string source)
        {
            string text = source;

            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf8Bytes = win1251.GetBytes(text);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);

            return win1251.GetString(win1251Bytes);
        }

        static string UTF8ToWin1251(string sourceStr)
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] utf8Bytes = utf8.GetBytes(sourceStr);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            return win1251.GetString(win1251Bytes);
        }

        static void Main(string[] args)
        {

            //var t = Firebase.Firebase.SelectUsers();
            start:
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                start2:
                try
                {
                    string g = new WebClient().DownloadString(@"https://www.mpt.ru/");
                    if (g == "" && g == null) goto start2;
                }
                catch { goto start2; }

                Console.CancelKeyPress += (s, e) =>
                {

                };


                var MPT = MPTInfoReplaces.GetReplacesInfo();
                List<MPTLib.Replaces.Group> Groups = MPT.Groups;
                MPTLib.Raspisanie.RaspisanieInfo Rasp = MPTLib.Raspisanie.MPTInfoRaspisanie.GetRaspisanieInfo();

                Thread notifications = new Thread(() =>
                {
                    while (true)
                    {
                        var TGroups = MPTInfoReplaces.GetReplacesInfo().Groups;
                        if (TGroups?.Count != 0)
                        {
                            var time = long.Parse($"{DateTime.Now.Hour.ToString("00")}{DateTime.Now.Minute.ToString("00")}{DateTime.Now.Second.ToString("00")}");
                            var users = Users.Users.users;
                            users.Where(x => x.notif_on_off).AsParallel().ToList().ForEach(x =>
                            {
                                MPTLib.Replaces.Group group = null;
                                if ((time > (long)060000 && time < (long)061000) || (time > (long)195800 && time < (long)200900) || (time > (long)215400 && time < (long)220500))
                                    group = TGroups.Any(y => Win1251ToUTF8(y.name) == x.group) ? TGroups.Where(y => Win1251ToUTF8(y.name) == x.group).First() : default(MPTLib.Replaces.Group);
                                if (((time > (long)060000 && time < (long)061000) || (time > (long)195900 && time < (long)200900) || (time > (long)215500 && time < (long)220500)) && group != default(MPTLib.Replaces.Group) && group != null && group?.Replaces?.Count > 0)
                                    String.Join("\r\n\r\n", group.Replaces.Select(z => $"| {Win1251ToUTF8(z.lesson_number)} | {Win1251ToUTF8(z.from)} | {Win1251ToUTF8(z.to)} |").ToArray()).SendMessage(x.from.id);
                                else if ((time > (long)060000 && time < (long)061000) || (time > (long)195900 && time < (long)200900) || (time > (long)215500 && time < (long)220500))
                                    "Замен нет".SendMessage(x.from.id);
                            });
                        }
                        GC.Collect();
                        Thread.Sleep(599000);
                    }
                })
                /*{ IsBackground = true }*/;

                //if (!(notifications.ThreadState == ThreadState.Background) || notifications.ThreadState == ThreadState.Unstarted)
                notifications.Start();

                var updates = DataBot.getUpdates(0);

                long id_offset = Firebase.Firebase.set_get_id ?? 0;
                //(updates.ok == true && updates.result.Count > 0) ? updates.result.First().update_id : 0;
                updates = DataBot.getUpdates(id_offset);
                while (true)
                {
                    updates = DataBot.getUpdates(id_offset);

                    if (updates != null && updates.ok && updates.result.Count > 0)
                    {
                        var Replaces = MPTInfoReplaces.GetReplacesInfo();
                        Groups = Replaces.Groups;

                        updates.result.ToList().ForEach(x =>
                        {
                            if (x.message != null)
                            {
                                Console.WriteLine($"(wait) OffsetID: {id_offset}; UpdateID: {x.update_id}; ChatID: {x.message.chat.id}; -> {x.message.text}");
                                try
                                {
                                    if (x.message.entities != null && x.message.entities.Last().type == GetUpdate.EntitiesType.bot_command.ToString())
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        var str = x.message.text.StartsWith("/") ? new string(x.message.text.Skip(1).ToArray()) : x.message.text;
                                        if (str.Contains("setgroup"))
                                        {
                                            var users = Users.Users.users;
                                            var gr1 = new string(x.message.text.Skip(10).ToArray());
                                            List<string> gr0 = new List<string>();
                                            foreach (var item1 in Rasp.Otdelenies.Select(u => u.Groups))
                                                foreach (var item0 in item1)
                                                    gr0.Add(Win1251ToUTF8(item0.name));

                                            if (gr0.Any(z => z == gr1))
                                            {
                                                if (users.All(u => u.from.id != x.message.from.id))
                                                {
                                                    //users.Add(new Users.User() { from = x.message.from, group = new string(x.message.text.Skip(10).ToArray()), notif_on_off = false });
                                                    Firebase.Firebase.InsertUser(new Users.User() { from = x.message.from, group = new string(x.message.text.Skip(10).ToArray()) });
                                                    "Вызарегистрировались".SendMessage(x.message.chat.id);
                                                }
                                                else
                                                {
                                                    users.Where(u => u.from.id == x.message.from.id).First().group = new string(x.message.text.Skip(10).ToArray());
                                                    Firebase.Firebase.UpdateUser(users.Where(u => u.from.id == x.message.from.id).First());
                                                    "Группа изменена".SendMessage(x.message.chat.id);
                                                }
                                                //Users.Users.users = users;
                                            }
                                            else
                                            {
                                                $"Такой группы не существует, пожалуйста, выбирите из следующего списка: \r\n{String.Join("\r\n", Rasp.Otdelenies.Select(y => $"{Win1251ToUTF8(y.name)}\r\n{String.Join("\r\n", y.Groups.Select(u => Win1251ToUTF8(u.name)).ToArray())}").ToArray())}".SendMessage(x.message.chat.id);
                                            }
                                        }
                                        else if (str.Contains("lessonsweek") && str.Trim().Length > 13)
                                        {
                                            var users = Users.Users.users;
                                            var gr1 = new string(x.message.text.Skip(13).ToArray());
                                            List<string> gr0 = new List<string>();
                                            foreach (var item1 in Rasp.Otdelenies.Select(u => u.Groups))
                                                foreach (var item0 in item1)
                                                    gr0.Add(Win1251ToUTF8(item0.name));

                                            if (gr0.Any(z => z == gr1))
                                            {
                                                string group = new string(x.message.text.Skip(13).ToArray());

                                                foreach (var item in Rasp.Otdelenies)
                                                {
                                                    foreach (var item2 in item.Groups)
                                                    {
                                                        if (Win1251ToUTF8(item2.name) == group)
                                                        {
                                                            List<string> Days = new List<string>();
                                                            var typeofweek = ((int)DateTime.Now.DayOfWeek == 0) ? ((Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() == "знаменатель") ? "числитель" : "знаменатель") : Win1251ToUTF8(Rasp.typeWeek).ToLower();

                                                            foreach (var item3 in item2.Days)
                                                            {
                                                                List<string> Lessons = new List<string>();
                                                                foreach (var item4 in item3.LessonsTables)
                                                                {
                                                                    string num = Win1251ToUTF8(item4.num);
                                                                    string subject = "";
                                                                    string teacher = Win1251ToUTF8(item4.teacher);

                                                                    string sub_subject = item4.subject;
                                                                    if (new Regex(@".*?:.*?\r\n.*?:.*?").IsMatch(sub_subject))
                                                                    {
                                                                        var groups = new Regex(@".*?:(.*?)\r\n.*?:(.*?) r").Match($"{sub_subject} r").Groups;
                                                                        if (typeofweek != "знаменатель")
                                                                        {
                                                                            subject = Win1251ToUTF8(groups[1].Value).Trim();
                                                                            if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[1].Value.Trim();
                                                                        }
                                                                        else if (typeofweek != "числитель")
                                                                        {
                                                                            subject = Win1251ToUTF8(groups[2].Value).Trim();
                                                                            if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value.Trim();
                                                                        }
                                                                    }
                                                                    else
                                                                        subject = Win1251ToUTF8(sub_subject);
                                                                    Lessons.Add($"| {num} | {subject} | {teacher} |");
                                                                }
                                                                Days.Add($"{Win1251ToUTF8(item3.name)}\r\n{Win1251ToUTF8(item3.position)}\r\n\r\n{String.Join("\r\n", Lessons)}");
                                                            }

                                                            if ((int)DateTime.Now.DayOfWeek == 0)
                                                                $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\nРасписание на: {typeofweek.First().ToString().ToUpper() + String.Join("", typeofweek.Skip(1))}\r\n\r\n{String.Join("\r\n\r\n", Days)}".SendMessage(x.message.chat.id);
                                                            else
                                                                $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\n\r\n{String.Join("\r\n\r\n", Days)}".SendMessage(x.message.chat.id);
                                                            goto end;
                                                        }
                                                    }
                                                }
                                                "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                end: { }
                                            }
                                            else
                                            {
                                                $"Такой группы не существует, пожалуйста, выбирите из следующего списка: \r\n{String.Join("\r\n", Rasp.Otdelenies.Select(y => $"{Win1251ToUTF8(y.name)}\r\n{String.Join("\r\n", y.Groups.Select(u => Win1251ToUTF8(u.name)).ToArray())}").ToArray())}".SendMessage(x.message.chat.id);
                                            }
                                        }
                                        else
                                            switch (str)
                                            {
                                                case "start":
                                                    "Привет, я бот, который будет помогать отслеживать замены с сайта mpt.ru для студентов.".SendMessage(x.message.chat.id);
                                                    break;
                                                case "replaces":
                                                    var _users = Users.Users.users;
                                                    if (_users.All(u => u.from.id != x.message.from.id))
                                                    {
                                                        String.Join("\r\n\r\n", Groups.Select(u => $"{Win1251ToUTF8(u.name)}\r\n{String.Join("\r\n", u.Replaces.Select(o => $"| {Win1251ToUTF8(o.lesson_number)} | {Win1251ToUTF8(o.from)} | {Win1251ToUTF8(o.to)} |"))}")).SendMessage(x.message.chat.id);
                                                        "Чтобы получить информацию только о своей группе авторизируйтесь командой /setgroup Группа.\r\nПолучить список групп - /groups.".SendMessage(x.message.chat.id);
                                                    }
                                                    else
                                                    {
                                                        var gr0 = Groups.Select(o => Win1251ToUTF8(o.name)).ToList();
                                                        var replaces = Groups.Where(u => Win1251ToUTF8(u.name) == _users.Where(o => o.from.id == x.message.from.id).First().group).ToList();
                                                        if (replaces.Count == 0)
                                                            "Замен на завтра нет".SendMessage(x.message.chat.id);
                                                        else
                                                            String.Join("\r\n\r\n", replaces.First().Replaces.Select(z => $"| {Win1251ToUTF8(z.lesson_number)} | {Win1251ToUTF8(z.from)} | {Win1251ToUTF8(z.to)} |")).SendMessage(x.message.chat.id);
                                                    }
                                                    break;
                                                case "replacesall":
                                                    String.Join("\r\n\r\n", Groups.Select(u => $"{Win1251ToUTF8(u.name)}\r\n{String.Join("\r\n", u.Replaces.Select(o => $"| {Win1251ToUTF8(o.lesson_number)} | {Win1251ToUTF8(o.from)} | {Win1251ToUTF8(o.to)} |"))}")).SendMessage(x.message.chat.id);
                                                    break;
                                                case "typeweek":
                                                    if ((int)DateTime.Now.DayOfWeek != 0)
                                                        Win1251ToUTF8(Rasp.typeWeek).SendMessage(x.message.chat.id);
                                                    else
                                                    {
                                                        if (Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() == "знаменатель")
                                                            $"Сейчас - {Win1251ToUTF8(Rasp.typeWeek)}\r\nСледующая неделя - Числитель".SendMessage(x.message.chat.id);
                                                        else if (Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() == "числитель")
                                                            $"Сейчас - {Win1251ToUTF8(Rasp.typeWeek)}\r\nСледующая неделя - Знаменатель".SendMessage(x.message.chat.id);
                                                    }
                                                    break;
                                                case "notifon":
                                                    if (Users.Users.users.Any(z => x.message.from.id == z.from.id))
                                                    {
                                                        var users = Users.Users.users;
                                                        "Уведомления включены.\r\nПримерное время прихода уведомления 20:00".SendMessage(x.message.chat.id);
                                                        users.Where(z => x.message.from.id == z.from.id).First().notif_on_off = true;

                                                        Firebase.Firebase.UpdateUser(users.Where(z => x.message.from.id == z.from.id).First());

                                                        //Users.Users.users = users;
                                                    }
                                                    else "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                    break;
                                                case "notifoff":
                                                    if (Users.Users.users.Any(z => x.message.from.id == z.from.id))
                                                    {
                                                        var users = Users.Users.users;
                                                        "Уведомления выключены.".SendMessage(x.message.chat.id);
                                                        users.Where(z => x.message.from.id == z.from.id).First().notif_on_off = false;

                                                        Firebase.Firebase.UpdateUser(users.Where(z => x.message.from.id == z.from.id).First());
                                                        //Users.Users.users = users;
                                                    }
                                                    else "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                    break;

                                                case "replacesinlessonson":
                                                    if (Users.Users.users.Any(z => x.message.from.id == z.from.id))
                                                    {
                                                        var users = Users.Users.users;
                                                        "Автозамена занятий в расписании на заменённые занятия включена.".SendMessage(x.message.chat.id);
                                                        users.Where(z => x.message.from.id == z.from.id).First().replace_on_off = true;

                                                        Firebase.Firebase.UpdateUser(users.Where(z => x.message.from.id == z.from.id).First());

                                                        //Users.Users.users = users;
                                                        break;
                                                    }
                                                    else "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                    break;

                                                case "replacesinlessonsoff":
                                                    if (Users.Users.users.Any(z => x.message.from.id == z.from.id))
                                                    {
                                                        var users = Users.Users.users;
                                                        "Автозамена занятий в расписании на заменённые занятия выключена.".SendMessage(x.message.chat.id);
                                                        users.Where(z => x.message.from.id == z.from.id).First().replace_on_off = false;

                                                        Firebase.Firebase.UpdateUser(users.Where(z => x.message.from.id == z.from.id).First());

                                                        //Users.Users.users = users;
                                                        break;
                                                    }
                                                    else "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                    break;

                                                case "groups":
                                                    $"Группы:\r\n{String.Join("\r\n", Rasp.Otdelenies.Select(y => $"{Win1251ToUTF8(y.name)}\r\n{String.Join("\r\n", y.Groups.Select(u => Win1251ToUTF8(u.name)).ToArray())}").ToArray())}".SendMessage(x.message.chat.id);
                                                    break;
                                                //Доделать
                                                case "lessonstoday":
                                                    {
                                                        if ((int)DateTime.Now.DayOfWeek == 0)
                                                            "На сегодня нет занятий.".SendMessage(x.message.chat.id);
                                                        else
                                                        {
                                                            var users = Users.Users.users;
                                                            var user = users.Where(z => x.message.from.id == z.from.id).First();

                                                            foreach (var item in Rasp.Otdelenies)
                                                            {
                                                                foreach (var item2 in item.Groups)
                                                                {
                                                                    //var users = Users.Users.users;
                                                                    //var user = users.Where(z => x.message.from.id == z.from.id).First();
                                                                    if (Win1251ToUTF8(item2.name) == user.group)
                                                                    {

                                                                        var Datetime = DateTime.Now;
                                                                        var datetomorrow = $"{Datetime.Day.ToString("00")}{Datetime.Month.ToString("00")}{Datetime.Year}";
                                                                        var date = Replaces.date.Replace(".", "");

                                                                        if (!(user.replace_on_off && datetomorrow == date))
                                                                        {
                                                                            List<string> Lessons = new List<string>();
                                                                            foreach (var item4 in item2.Days[(int)DateTime.Now.DayOfWeek - 1].LessonsTables)
                                                                            {
                                                                                string num = Win1251ToUTF8(item4.num);
                                                                                string subject = "";
                                                                                string teacher = Win1251ToUTF8(item4.teacher);

                                                                                string sub_subject = item4.subject;
                                                                                if (new Regex(@".*?:.*?\r\n.*?:.*?").IsMatch(sub_subject))
                                                                                {
                                                                                    var groups = new Regex(@".*?:(.*?)\r\n.*?:(.*?) r").Match($"{sub_subject} r").Groups;
                                                                                    if (Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() != "знаменатель")
                                                                                    {
                                                                                        subject = Win1251ToUTF8(groups[1].Value).Trim();
                                                                                        if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                            teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[1].Value.Trim();
                                                                                    }
                                                                                    else if (Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() != "числитель")
                                                                                    {
                                                                                        subject = Win1251ToUTF8(groups[2].Value).Trim();
                                                                                        if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                        {
                                                                                            var g = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value;
                                                                                            teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value.Trim();
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                    subject = Win1251ToUTF8(sub_subject);
                                                                                Lessons.Add($"| {num} | {subject} | {teacher} |");
                                                                            }
                                                                            $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\n\r\n{user.group}\r\n{Win1251ToUTF8(item2.Days[(int)DateTime.Now.DayOfWeek - 1].name)}\r\n\r\n{String.Join("\r\n", Lessons.ToArray())}".SendMessage(x.message.chat.id);
                                                                            goto end;
                                                                        }
                                                                        else
                                                                        {

                                                                            List<Replace> _replaces = new List<Replace>();
                                                                            List<(int num, string subject, string teacher)> replaces = new List<(int num, string subject, string teacher)>();

                                                                            if (Groups.Where(u => Win1251ToUTF8(u.name) == user.group).ToList().Count != 0)
                                                                            {
                                                                                _replaces = Groups.Where(u => Win1251ToUTF8(u.name) == user.group).First().Replaces;
                                                                                replaces = _replaces.Select(it => ((int.Parse(it.lesson_number)), ($">{Win1251ToUTF8(it.to)}<".Contains("отменено") ? "" : String.Join("", new Regex(@"<.*? .*? (.*?)>").Match(String.Join("", ($">{Win1251ToUTF8(it.to)}<").Reverse().ToList())).Groups[1].Value.Reverse().ToList())), (new Regex(@">.*? (.... .*?)<").Match($">{Win1251ToUTF8(it.to)}<").Groups[1].Value))).ToList();
                                                                            }

                                                                            List<(int num, string subject, string teacher)> lessons = new List<(int num, string subject, string teacher)>();
                                                                            foreach (var item4 in item2.Days[(int)DateTime.Now.DayOfWeek - 1].LessonsTables)
                                                                            {
                                                                                string num = Win1251ToUTF8(item4.num);
                                                                                string subject = "";
                                                                                string teacher = Win1251ToUTF8(item4.teacher);

                                                                                string sub_subject = item4.subject;
                                                                                if (new Regex(@".*?:.*?\r\n.*?:.*?").IsMatch(sub_subject))
                                                                                {
                                                                                    var groups = new Regex(@".*?:(.*?)\r\n.*?:(.*?) r").Match($"{sub_subject} r").Groups;
                                                                                    if (Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() != "знаменатель")
                                                                                    {
                                                                                        subject = Win1251ToUTF8(groups[1].Value).Trim();
                                                                                        if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                            teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[1].Value.Trim();
                                                                                    }
                                                                                    else if (Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() != "числитель")
                                                                                    {
                                                                                        subject = Win1251ToUTF8(groups[2].Value).Trim();
                                                                                        if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                        {
                                                                                            var g = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value;
                                                                                            teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value.Trim();
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                    subject = Win1251ToUTF8(sub_subject);
                                                                                lessons.Add((int.Parse(num), subject, teacher));
                                                                            }

                                                                            var rep = replaces;

                                                                            rep.AddRange(lessons.Where(u => replaces.All(o => o.num != u.num)).ToList());

                                                                            var Lessons = rep.OrderBy(u => u.num).Where(u => u.subject.Trim() != "").ToList().Select(u => $"| {u.num} | {u.subject} | {u.teacher} |");

                                                                            $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\n\r\n{user.group}\r\n{Win1251ToUTF8(item2.Days[(int)DateTime.Now.DayOfWeek - 1].name)}\r\n\r\n{String.Join("\r\n", Lessons.ToArray())}".SendMessage(x.message.chat.id);
                                                                            goto end;

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                            end:
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                case "lessonstomorrow":
                                                    if ((int)DateTime.Now.DayOfWeek == 6)
                                                        "На завтра нет занятий.".SendMessage(x.message.chat.id);
                                                    else
                                                    {

                                                        // изменено
                                                        var users = Users.Users.users;
                                                        var user = users.Where(z => x.message.from.id == z.from.id).First();
                                                        var typeofweek = ((int)DateTime.Now.DayOfWeek == 0) ? ((Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() == "знаменатель") ? "числитель" : "знаменатель") : Win1251ToUTF8(Rasp.typeWeek).ToLower();

                                                        foreach (var item in Rasp.Otdelenies)
                                                        {
                                                            foreach (var item2 in item.Groups)
                                                            {

                                                                //var users = Users.Users.users;
                                                                //var user = users.Where(z => x.message.from.id == z.from.id).First();
                                                                //var typeofweek = ((int)DateTime.Now.DayOfWeek == 0) ? ((Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() == "знаменатель") ? "числитель" : "знаменатель") : Win1251ToUTF8(Rasp.typeWeek).ToLower();

                                                                if (Win1251ToUTF8(item2.name) == user.group)
                                                                {
                                                                    var Datetime = DateTime.Now.AddDays(1);
                                                                    var datetomorrow = $"{Datetime.Day.ToString("00")}{Datetime.Month.ToString("00")}{Datetime.Year}";
                                                                    var date = Replaces.date.Replace(".", "");

                                                                    if (!(user.replace_on_off && datetomorrow == date))
                                                                    {

                                                                        List<string> Lessons = new List<string>();
                                                                        foreach (var item4 in item2.Days[(int)DateTime.Now.DayOfWeek == 0 ? 0 : (int)DateTime.Now.DayOfWeek].LessonsTables)
                                                                        {
                                                                            string num = Win1251ToUTF8(item4.num);
                                                                            string subject = "";
                                                                            string teacher = Win1251ToUTF8(item4.teacher);

                                                                            string sub_subject = item4.subject;
                                                                            if (new Regex(@".*?:.*?\r\n.*?:.*?").IsMatch(sub_subject))
                                                                            {
                                                                                var groups = new Regex(@".*?:(.*?)\r\n.*?:(.*?) r").Match($"{sub_subject} r").Groups;
                                                                                if (typeofweek != "знаменатель")
                                                                                {
                                                                                    subject = Win1251ToUTF8(groups[1].Value).Trim();
                                                                                    if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                        teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[1].Value.Trim();
                                                                                }
                                                                                else if (typeofweek != "числитель")
                                                                                {
                                                                                    subject = Win1251ToUTF8(groups[2].Value).Trim();
                                                                                    if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                    {
                                                                                        var g = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value;
                                                                                        teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value.Trim();
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                                subject = Win1251ToUTF8(sub_subject);
                                                                            Lessons.Add($"| {num} | {subject} | {teacher} |");
                                                                        }

                                                                        if ((int)DateTime.Now.DayOfWeek == 0)
                                                                            $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\nРасписание на: {typeofweek.First().ToString().ToUpper() + String.Join("", typeofweek.Skip(1))}\r\n\r\n{user.group}\r\n{Win1251ToUTF8(item2.Days[(int)DateTime.Now.DayOfWeek == 0 ? 0 : (int)DateTime.Now.DayOfWeek].name)}\r\n\r\n{String.Join("\r\n", Lessons.ToArray())}".SendMessage(x.message.chat.id);
                                                                        else
                                                                            $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\n\r\n{user.group}\r\n{Win1251ToUTF8(item2.Days[(int)DateTime.Now.DayOfWeek == 0 ? 0 : (int)DateTime.Now.DayOfWeek].name)}\r\n\r\n{String.Join("\r\n", Lessons.ToArray())}".SendMessage(x.message.chat.id);
                                                                        goto end;
                                                                    }
                                                                    else
                                                                    {
                                                                        List<Replace> _replaces = new List<Replace>();
                                                                        List<(int num, string subject, string teacher)> replaces = new List<(int num, string subject, string teacher)>();

                                                                        if (Groups.Where(u => Win1251ToUTF8(u.name) == user.group).ToList().Count != 0)
                                                                        {
                                                                            _replaces = Groups.Where(u => Win1251ToUTF8(u.name) == user.group).First().Replaces;
                                                                            replaces = _replaces.Select(it => ((int.Parse(it.lesson_number)), ($">{Win1251ToUTF8(it.to)}<".Contains("отменено") ? "" : String.Join("", new Regex(@"<.*? .*? (.*?)>").Match(String.Join("", ($">{Win1251ToUTF8(it.to)}<").Reverse().ToList())).Groups[1].Value.Reverse().ToList())), (new Regex(@">.*? (.... .*?)<").Match($">{Win1251ToUTF8(it.to)}<").Groups[1].Value))).ToList();
                                                                        }

                                                                        List<(int num, string subject, string teacher)> lessons = new List<(int num, string subject, string teacher)>();
                                                                        foreach (var item4 in item2.Days[(int)DateTime.Now.DayOfWeek == 0 ? 0 : (int)DateTime.Now.DayOfWeek].LessonsTables)
                                                                        {
                                                                            string num = Win1251ToUTF8(item4.num);
                                                                            string subject = "";
                                                                            string teacher = Win1251ToUTF8(item4.teacher);

                                                                            string sub_subject = item4.subject;
                                                                            if (new Regex(@".*?:.*?\r\n.*?:.*?").IsMatch(sub_subject))
                                                                            {
                                                                                var groups = new Regex(@".*?:(.*?)\r\n.*?:(.*?) r").Match($"{sub_subject} r").Groups;
                                                                                if (typeofweek != "знаменатель")
                                                                                {
                                                                                    subject = Win1251ToUTF8(groups[1].Value).Trim();
                                                                                    if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                        teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[1].Value.Trim();
                                                                                }
                                                                                else if (typeofweek != "числитель")
                                                                                {
                                                                                    subject = Win1251ToUTF8(groups[2].Value).Trim();
                                                                                    if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                    {
                                                                                        var g = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value;
                                                                                        teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value.Trim();
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                                subject = Win1251ToUTF8(sub_subject);
                                                                            lessons.Add((int.Parse(num), subject, teacher));
                                                                        }

                                                                        var rep = replaces;

                                                                        rep.AddRange(lessons.Where(u => replaces.All(o => o.num != u.num)).ToList());

                                                                        var Lessons = rep.OrderBy(u => u.num).Where(u => u.subject.Trim() != "").ToList().Select(u => $"| {u.num} | {u.subject} | {u.teacher} |");

                                                                        if ((int)DateTime.Now.DayOfWeek == 0)
                                                                            $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\nРасписание на: {typeofweek.First().ToString().ToUpper() + String.Join("", typeofweek.Skip(1))}\r\n\r\n{user.group}\r\n{Win1251ToUTF8(item2.Days[(int)DateTime.Now.DayOfWeek == 0 ? 0 : (int)DateTime.Now.DayOfWeek].name)}\r\n\r\n{String.Join("\r\n", Lessons.ToArray())}".SendMessage(x.message.chat.id);
                                                                        else
                                                                            $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\n\r\n{user.group}\r\n{Win1251ToUTF8(item2.Days[(int)DateTime.Now.DayOfWeek == 0 ? 0 : (int)DateTime.Now.DayOfWeek].name)}\r\n\r\n{String.Join("\r\n", Lessons.ToArray())}".SendMessage(x.message.chat.id);
                                                                        goto end;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                        end:
                                                        break;
                                                    }
                                                    break;
                                                case "lessonsweek":
                                                    {
                                                        var users = Users.Users.users;
                                                        var user = users.Where(z => x.message.from.id == z.from.id).First();
                                                        var typeofweek = ((int)DateTime.Now.DayOfWeek == 0) ? ((Win1251ToUTF8(Rasp.typeWeek).ToLower().Trim() == "знаменатель") ? "числитель" : "знаменатель") : Win1251ToUTF8(Rasp.typeWeek).ToLower();

                                                        foreach (var item in Rasp.Otdelenies)
                                                        {
                                                            foreach (var item2 in item.Groups)
                                                            {

                                                                if (Win1251ToUTF8(item2.name) == user.group)
                                                                {
                                                                    List<string> Days = new List<string>();
                                                                    foreach (var item3 in item2.Days)
                                                                    {
                                                                        List<string> Lessons = new List<string>();
                                                                        foreach (var item4 in item3.LessonsTables)
                                                                        {
                                                                            string num = Win1251ToUTF8(item4.num);
                                                                            string subject = "";
                                                                            string teacher = Win1251ToUTF8(item4.teacher);

                                                                            string sub_subject = item4.subject;
                                                                            if (new Regex(@".*?:.*?\r\n.*?:.*?").IsMatch(sub_subject))
                                                                            {
                                                                                var groups = new Regex(@".*?:(.*?)\r\n.*?:(.*?) r").Match($"{sub_subject} r").Groups;
                                                                                if (typeofweek != "знаменатель")
                                                                                {
                                                                                    subject = Win1251ToUTF8(groups[1].Value).Trim();
                                                                                    if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                        teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[1].Value.Trim();
                                                                                }
                                                                                else if (typeofweek != "числитель")
                                                                                {
                                                                                    subject = Win1251ToUTF8(groups[2].Value).Trim();
                                                                                    if (new Regex(@".*?\r\n.*?").IsMatch(teacher))
                                                                                        teacher = new Regex(@"(.*?)\r\n(.*?) r").Match($"{teacher} r").Groups[2].Value.Trim();
                                                                                }
                                                                            }
                                                                            else
                                                                                subject = Win1251ToUTF8(sub_subject);
                                                                            Lessons.Add($"| {num} | {subject} | {teacher} |");
                                                                        }
                                                                        Days.Add($"{Win1251ToUTF8(item3.name)}\r\n{Win1251ToUTF8(item3.position)}\r\n\r\n{String.Join("\r\n", Lessons)}");
                                                                    }
                                                                    if ((int)DateTime.Now.DayOfWeek == 0)
                                                                        $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\nРасписание на: {typeofweek.First().ToString().ToUpper() + String.Join("", typeofweek.Skip(1))}\r\n\r\n{String.Join("\r\n\r\n", Days)}".SendMessage(x.message.chat.id);
                                                                    else
                                                                        $"Сейчас: {Win1251ToUTF8(Rasp.typeWeek)}\r\n\r\n{String.Join("\r\n\r\n", Days)}".SendMessage(x.message.chat.id);

                                                                    goto end;
                                                                }
                                                            }
                                                        }
                                                        "Вы ещё не зарегистрировались.\r\nВведите команду: /setgroup.".SendMessage(x.message.chat.id);
                                                        end:
                                                        break;
                                                    }
                                                default:
                                                    "Такой команды не существует.".SendMessage(x.message.chat.id);
                                                    break;
                                            }
                                        Console.WriteLine($"(ok) OffsetID: {id_offset}; UpdateID: {x.update_id}; ChatID: {x.message.chat.id}; -> {x.message.text}");
                                        Console.ResetColor();
                                        goto endall;

                                    }
                                    //Console.WriteLine($"(message) OffsetID: {id_offset}; UpdateID: {x.update_id}; ChatID: {x.message.chat.id}; -> {x.message.text}");
                                    //Console.ResetColor();
                                }
                                catch (Exception e)
                                {
                                    e.ToString().SendMessage(x.message.chat.id);
                                }
                            }
                            endall: { }
                        });
                        if (updates.result.Count > 0)
                        {
                            id_offset = updates.result.First().update_id + 1;
                            Firebase.Firebase.set_get_id = id_offset;
                        }
                    }
                    GC.Collect();
                    Thread.Sleep(1000);
                }
                Console.ReadKey();

            }
            else goto start;
        }
    }
}