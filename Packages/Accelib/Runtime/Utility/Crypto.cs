using System.Security.Cryptography;

namespace Accelib.Utility
{
    public static class Crypto
    {
        private static readonly string Cj4r83fk = "dlfi$(382fd##k72ldKDFIE$woekf@94";
        
        public static string Encrypt(string data)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);
            var rm = CreateRijndaelManaged();
            var ct = rm.CreateEncryptor();
            var results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Convert.ToBase64String(results, 0, results.Length);
        }
 
        public static string Decrypt(string data)
        {
 
            var bytes = System.Convert.FromBase64String(data);
            var rm = CreateRijndaelManaged();
            var ct = rm.CreateDecryptor();
            var resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(resultArray);
        }
 
        private static RijndaelManaged CreateRijndaelManaged()
        {
            var keyArray = System.Text.Encoding.UTF8.GetBytes(Cj4r83fk);
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