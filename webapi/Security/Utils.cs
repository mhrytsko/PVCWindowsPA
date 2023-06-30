using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace webapi.Security
{
    public static class Utils
    {
        //private static readonly byte[] additionalEntropy = Encoding.Unicode.GetBytes("Maksim");
        public static string Crypt(this string text)
        {
            return Encoding.Unicode.GetString(
                SHA256.Create().ComputeHash(Encoding.Unicode.GetBytes(text)));

            /*return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(text),
                    additionalEntropy,
                    DataProtectionScope.LocalMachine));*/
        }

        public static string Decrypt(this string text)
        {
            /*return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(
                     Convert.FromBase64String(text),
                     additionalEntropy,
                     DataProtectionScope.LocalMachine));*/
            return text;
        }

        public static bool Compare(string encryptedString, string toCompare)
        {
            return encryptedString == Crypt(toCompare);
        }
    }
}
