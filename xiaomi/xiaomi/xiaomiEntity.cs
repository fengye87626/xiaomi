

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xiaomi.xiaomiEntityJsonTypes;

namespace xiaomi.xiaomiEntityJsonTypes
{

    public class Miphone
    {

        [JsonProperty("hdstart")]
        public bool Hdstart;

        [JsonProperty("hdstop")]
        public bool Hdstop;

        [JsonProperty("hdurl")]
        public string Hdurl;

        [JsonProperty("duration")]
        public object Duration;

        [JsonProperty("pmstart")]
        public bool Pmstart;
    }

    public class Mibox
    {

        [JsonProperty("hdstart")]
        public bool Hdstart;

        [JsonProperty("hdstop")]
        public bool Hdstop;

        [JsonProperty("hdurl")]
        public string Hdurl;

        [JsonProperty("duration")]
        public object Duration;

        [JsonProperty("pmstart")]
        public bool Pmstart;
    }

    public class Mitv
    {

        [JsonProperty("hdstart")]
        public bool Hdstart;

        [JsonProperty("hdstop")]
        public bool Hdstop;

        [JsonProperty("hdurl")]
        public string Hdurl;

        [JsonProperty("duration")]
        public object Duration;

        [JsonProperty("pmstart")]
        public bool Pmstart;
    }

    public class Status
    {

        [JsonProperty("allow")]
        public bool Allow;

        [JsonProperty("miphone")]
        public Miphone Miphone;

        [JsonProperty("mibox")]
        public Mibox Mibox;

        [JsonProperty("mitv")]
        public Mitv Mitv;
    }

}

namespace xiaomi
{

    public class xiaomiEntity
    {

        [JsonProperty("stime")]
        public int Stime;

        [JsonProperty("status")]
        public Status Status;
    }

}
