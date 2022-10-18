using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SS_BlogManagement.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.InfraStructure.Utils
{
    public class Controller_CheckAPIKey
    {
       
        public static bool checkAPIKey(string check_apiKey, string apikey, string cryptoKey)
        {

            if (Controller_TextEncryption.Decrypt(check_apiKey, cryptoKey) != apikey)
                return false;
            else 
                return true;

        }
    }
}
