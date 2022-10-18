using System;
using System.Collections.Generic;
using System.Text;

namespace SS_BlogManagement.Shared.Entities
{
    public class Appsettings
    {
        public string Secret { get; set; }
        public string Key { get; set; }
    }

    public class ResCodeMessage
    {
        public dynamic v_data { get; set; }
        public string v_token { get; set; }
        public string v_rescode { get; set; }
        public string v_resmessage { get; set; }

    }
    public class TokenLifeTime
    {
        public int Time { get; set; }
    }
    public class CryptoKey
    {
        public string Key { get; set; }
    }
}
