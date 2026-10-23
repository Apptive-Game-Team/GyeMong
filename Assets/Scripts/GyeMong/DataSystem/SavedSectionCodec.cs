using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GyeMong.DataSystem
{
    internal static class SavedSectionCodec
    {
        private const string Prefix = "GYEMONG1:";

        // Client-side obfuscation only; this embedded key is not a security boundary.
        private static byte[] CreateKey()
        {
            using var hash = SHA256.Create();
            return hash.ComputeHash(Encoding.UTF8.GetBytes("GyemongFighting!"));
        }

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = CreateKey();
            aes.GenerateIV();
            using var output = new MemoryStream();
            output.Write(aes.IV, 0, aes.IV.Length);
            using (var crypto = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
            using (var writer = new StreamWriter(crypto, new UTF8Encoding(false)))
                writer.Write(plainText);
            return Prefix + Convert.ToBase64String(output.ToArray());
        }

        public static string Decrypt(string storedText)
        {
            if (string.IsNullOrWhiteSpace(storedText)) throw new FormatException("Saved section is empty.");
            string value = storedText.Trim();
            // Editor JSON remains readable in builds and is migrated on the next save.
            if (value.StartsWith("{", StringComparison.Ordinal)) return value;
            if (!value.StartsWith(Prefix, StringComparison.Ordinal))
                throw new FormatException("Unsupported saved section format.");
            byte[] payload = Convert.FromBase64String(value.Substring(Prefix.Length));
            if (payload.Length < 32 || (payload.Length - 16) % 16 != 0)
                throw new FormatException("Saved section payload is truncated.");
            using var aes = Aes.Create();
            aes.Key = CreateKey();
            var iv = new byte[16];
            Buffer.BlockCopy(payload, 0, iv, 0, iv.Length);
            aes.IV = iv;
            using var input = new MemoryStream(payload, 16, payload.Length - 16);
            using var crypto = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(crypto);
            return reader.ReadToEnd();
        }
    }
}
