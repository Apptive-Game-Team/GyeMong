using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GyeMong.DataSystem
{
    internal static class SavedSectionCodec
    {
        // Preserve the existing storage contract; format migration is a separate concern.
        private const string encryptionKey = "GyemongFighting!";

        // AES 암호화
        public static string Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aes.IV = new byte[16]; // 16-byte 초기화 벡터 (0으로 초기화)
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray()); // 암호화된 데이터를 Base64로 인코딩
        }

        // AES 복호화
        public static string Decrypt(string cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
            aes.IV = new byte[16]; // 16-byte 초기화 벡터 (0으로 초기화)
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);
            return reader.ReadToEnd(); // 복호화된 데이터를 반환
        }
    }
}
