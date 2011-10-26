using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace SteveSloka.WebSyncNLogTarget
{
    [DataContract]
    public class Payload
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "time")]
        public DateTime Time { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }
    }
}
