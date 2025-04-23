using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace lab4quiz
{
    public class AESCipher
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("kochamdotNET2115");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("2137211513376902");

        public static void EncryptToFile<T>(T obj, string path)
        {
            var json = JsonSerializer.Serialize(obj);
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using var fs = new FileStream(path, FileMode.Create);
            using var cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(json);
        }

        public static T DecryptFromFile<T>(string path)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using var fs = new FileStream(path, FileMode.Open);
            using var cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            var json = sr.ReadToEnd();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
