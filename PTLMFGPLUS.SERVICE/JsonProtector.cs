using System;
using System.Security.Cryptography;
using System.Text;

namespace PTLMFGPLUS.SERVICE
{
    public static class JsonProtector
    {
        public static string Decrypt(string encryptedText, string password)
        {
            byte[] allBytes = Convert.FromBase64String(encryptedText);

            byte[] salt = allBytes[..16];
            byte[] iv = allBytes[16..32];
            byte[] cipherBytes = allBytes[32..];

            using var key = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();

            aes.KeySize = 256;
            aes.Key = key.GetBytes(32);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }

}
