using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp;

namespace TestTelegramBotApi
{
    namespace GetUpdate
    {
        public enum EntitiesType
        {
            bot_command = 0,
        }

        public class GetUpdate
        {
            [JsonProperty("ok")]
            public bool ok { get; set; }
            [JsonProperty("result")]
            public List<Result> result { get; set; }
        }

        public class Result
        {
            [JsonProperty("update_id")]
            public long update_id { get; set; }
            [JsonProperty("message")]
            public Message message { get; set; }
        }

        public class Message
        {
            [JsonProperty("message_id")]
            public long message_id { get; set; }
            [JsonProperty("from")]
            public From from { get; set; }
            [JsonProperty("chat")]
            public Chat chat { get; set; }
            [JsonProperty("date"/*, ItemConverterType = typeof(JavaScriptDateTimeConverter)*/)]
            public long date { get; set; }
            [JsonProperty("text")]
            public string text { get; set; }
            [JsonProperty("entities")]
            public List<Entities> entities { get; set; }
        }

        public class From
        {
            [JsonProperty("id")]
            public long id { get; set; }
            [JsonProperty("is_bot")]
            public bool is_bot { get; set; }
            [JsonProperty("first_name")]
            public string first_name { get; set; }
            [JsonProperty("last_name")]
            public string last_name { get; set; }
            [JsonProperty("username")]
            public string username { get; set; }
            [JsonProperty("language_code")]
            public string language_code { get; set; }
        }

        public class Chat 
        {
            [JsonProperty("id")]
            public long id { get; set; }
            [JsonProperty("first_name")]
            public string first_name { get; set; }
            [JsonProperty("last_name")]
            public string last_name { get; set; }
            [JsonProperty("username")]
            public string username { get; set; }
            [JsonProperty("type")]
            public string type { get; set; }
        }

        public class Entities
        {
            [JsonProperty("offset")]
            public byte offset { get; set; }
            [JsonProperty("length")]
            public uint length { get; set; }
            [JsonProperty("type")]
            public string type { get; set; }
        }
    }
}