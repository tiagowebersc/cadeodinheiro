using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Infrastructure.Util
{
    public class Encrypter
    {
        public static string EncryptPass(String text)
        {
            StringBuilder password = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] entry = Encoding.ASCII.GetBytes(text);
            byte[] hash = md5.ComputeHash(entry);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                password.Append(hash[i].ToString("X2"));
            }
            return password.ToString();
        }
    }
}