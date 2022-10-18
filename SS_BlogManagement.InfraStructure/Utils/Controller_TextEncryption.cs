using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.InfraStructure.Utils
{
    public class Controller_TextEncryption
    {
        private static readonly byte[] IV = new byte[8] { 240, 3, 45, 29, 0, 76, 173, 59 };
        
        public static string Encrypt(string s, string key)
        {
            
            if (s == null || s.Length == 0)
            {
                return string.Empty;
            }
            string result = string.Empty;

            try
            {
                byte[] buffer = Encoding.Unicode.GetBytes(s);

                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                des.Key = MD5.ComputeHash(Encoding.Unicode.GetBytes(key));
                des.IV = IV;
                result = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static string Decrypt(string s, string key)
        {
            try
            {

                //if (key == "") key = cryptoKey;
                if (s == null || s.Length == 0)
                {
                    return string.Empty;
                }

                string result = string.Empty;

                try
                {
                    byte[] buffer = Convert.FromBase64String(s);

                    TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                    MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                    //  des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
                    des.Key = MD5.ComputeHash(Encoding.Unicode.GetBytes(key));
                    des.IV = IV;
                    //  result = Encoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
                    result = Encoding.Unicode.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
                }
                catch
                {
                    throw;
                }

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
