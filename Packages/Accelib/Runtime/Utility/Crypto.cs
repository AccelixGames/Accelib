using System.Security.Cryptography;

namespace Accelib.Utility
{
    public static class Crypto
    {
        public static string Encrypt(string str, string key)
        {
            var bytes = EncryptToBytes(str, key);
            return System.Convert.ToBase64String(bytes, 0, bytes.Length);
        }
        
        public static byte[] EncryptToBytes(string str, string key)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            var rm = CreateRijndaelManaged(key);
            var ct = rm.CreateEncryptor();
            return ct.TransformFinalBlock(bytes, 0, bytes.Length);
        }
 
        public static string Decrypt(string str, string key)
        {
            var bytes = System.Convert.FromBase64String(str);
            return DecryptFromBytes(bytes, key);
        }

        public static string DecryptFromBytes(byte[] str, string key)
        {
            var rm = CreateRijndaelManaged(key);
            var ct = rm.CreateDecryptor();
            var bytes = ct.TransformFinalBlock(str, 0, str.Length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
 
        private static RijndaelManaged CreateRijndaelManaged(string key)
        {
            var keyArray = System.Text.Encoding.UTF8.GetBytes(key);
            var result = new RijndaelManaged();
 
            var newKeysArray = new byte[16];
            System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);
 
            result.Key = newKeysArray;
            result.Mode = CipherMode.ECB;
            result.Padding = PaddingMode.PKCS7;
            return result;
        }
    }
}