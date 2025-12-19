using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MewVivor.Data;
using UnityEngine;

namespace MewVivor
{
    public static class AesEncryptionUtil
    {
        // 반드시 32바이트 (256비트)
        private static byte[] Key;
        // 반드시 16바이트 (128비트)
        private static byte[] IV;

        public static void Initialize()
        {
            var configSetting = Manager.I.Resource.Load<ConfigurationSetting>("ConfigurationSetting");
            Key = Convert.FromBase64String(configSetting.AesKey);
            IV = Convert.FromBase64String(configSetting.AesIV);
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return null;
            }

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor();

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                    sw.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return null;
            }
            
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

                using (MemoryStream ms = new MemoryStream(cipherTextBytes))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        
        // public static void GenerateKeyAndIV()
        // {
        //     byte[] key = new byte[32]; // 256-bit key
        //     byte[] iv = new byte[16];  // 128-bit IV
        //
        //     using (var rng = RandomNumberGenerator.Create())
        //     {
        //         rng.GetBytes(key);
        //         rng.GetBytes(iv);
        //     }
        //
        //     string base64Key = Convert.ToBase64String(key);
        //     string base64IV = Convert.ToBase64String(iv);
        //
        //     Debug.Log("AES 256 Key (Base64): " + base64Key);
        //     Debug.Log("AES IV (Base64): " + base64IV);
        // }
    }
}