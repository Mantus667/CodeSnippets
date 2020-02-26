/*
Crypto service based on RijndaelManaged
*/
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoService
{
    public static class EncryptionHelper
    {
        private readonly IRandomService _randomService;

        public CryptoService(IRandomService randomService)
        {
            _randomService = randomService;
        }

        /// <summary>
        /// Generates a random string using only alpha-numeric ASCII characters
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_randomService.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generates a fixed-length string with possible leading zeros (e.g. "052047")
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_randomService.Value.Next(s.Length)]).ToArray());
        }

        public static byte[] GetNewIv()
        {
            using (var aes = new RijndaelManaged())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }

        private static void CheckKeyLength(byte[] keyBytes)
        {
            if (keyBytes.Length != 32)
            {
                throw new Exception($"Your key is {keyBytes.Length} long but should be 32 long");
            }
        }

        public static byte[] Encrypt(string key, string plainText, byte[] ivBytes)
        {
            var keyBytes = Encoding.Default.GetBytes(key);
            CheckKeyLength(keyBytes);
            return EncryptStringToBytes(plainText, keyBytes, ivBytes);
        }

        public static string Decrypt(string key, byte[] cipherBytes, byte[] ivBytes)
        {
            var keyBytes = Encoding.Default.GetBytes(key);
            CheckKeyLength(keyBytes);
            return DecryptStringFromBytes(cipherBytes, keyBytes, ivBytes);
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            byte[] encrypted;

            using (var aes = new RijndaelManaged())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            string plaintext = null;

            using (var aes = new RijndaelManaged())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var msDecrypt = new MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }

            return plaintext;
        }

        public static string GetSha256Hash(string input)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}