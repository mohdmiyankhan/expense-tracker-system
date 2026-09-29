using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ExpenseTrackerSystem.Helpers
{
    public static class UrlEncryptionHelper
    {
        private static readonly string SecretKey = "MySecretKey12345MySecretKey12345";

        public static string Encrypt(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes(SecretKey);
            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(plainText);

                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }

                    // URL-safe Base64
                    return Convert.ToBase64String(ms.ToArray())
                        .Replace("+", "-")
                        .Replace("/", "_")
                        .Replace("=", "");
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            encryptedText = encryptedText
                .Replace("-", "+")
                .Replace("_", "/");

            while (encryptedText.Length % 4 != 0)
            {
                encryptedText += "=";
            }

            byte[] cipherText = Convert.FromBase64String(encryptedText);

            byte[] key = Encoding.UTF8.GetBytes(SecretKey);
            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}