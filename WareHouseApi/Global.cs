using System.Security.Cryptography;
using System.Text;

namespace WareHouseApi
{
    public class Global
    {
        public static string MSSqlConnectionString { get; set; } = "Data Source=RKSQL.shzhleb.ru\\SQL2019; Initial Catalog=RKNET; User ID=rk7; Password=wZSbs6NKl2SF; TrustServerCertificate=True";

        public static string SecretKey { get; set; } = "kjdgfskldgfkljdgfsdgfsdgfsdgfsdgfsdgfsdgfsdgfdgfkljdgkljdgfdfghghghghghghghghghghghghghghghghghghdfkjsfldhgfdshglfdhgsfldkjg";
        public static byte[] ToCode(string code)
        {
            byte[] charArray = new byte[code.Length / 2];
            for (int i = 0; i < code.Length; i += 2)
            {
                string hexPair = code.Substring(i, 2);
                charArray[i / 2] = (byte)Convert.ToInt32(hexPair, 16);
            }
            return charArray;
        }

        public static string FromCode(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }     


        //public static string Encrypt(string password)
        //{
        //    string salt = "Kosher";
        //    string hashAlgorithm = "SHA1";
        //    int passwordIterations = 2;
        //    string initialVector = "OFRna73m*aze01xY";
        //    int keySize = 256;
        //    if (string.IsNullOrEmpty(password))
        //        return "";
        //    byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
        //    byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
        //    byte[] plainTextBytes = Encoding.UTF8.GetBytes(password);
        //    PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(SecretKey, saltValueBytes, hashAlgorithm, passwordIterations);
        //    byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
        //    RijndaelManaged symmetricKey = new RijndaelManaged();
        //    symmetricKey.Mode = CipherMode.CBC;
        //    byte[] cipherTextBytes = null;
        //    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
        //    {
        //        using (MemoryStream memStream = new MemoryStream())
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
        //            {
        //                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        //                cryptoStream.FlushFinalBlock();
        //                cipherTextBytes = memStream.ToArray();
        //                memStream.Close();
        //                cryptoStream.Close();
        //            }
        //        }
        //    }
        //    symmetricKey.Clear();
        //    return Convert.ToBase64String(cipherTextBytes);
        //}



        //public static string Decrypt(string password)
        //{
        //    string salt = "Kosher";
        //    string hashAlgorithm = "SHA1";
        //    int passwordIterations = 2;
        //    string initialVector = "OFRna73m*aze01xY";
        //    int keySize = 256;
        //    if (string.IsNullOrEmpty(password))
        //        return "";
        //    byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
        //    byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
        //    byte[] cipherTextBytes = Convert.FromBase64String(password);
        //    PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(SecretKey, saltValueBytes, hashAlgorithm, passwordIterations);
        //    byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
        //    RijndaelManaged symmetricKey = new RijndaelManaged();
        //    symmetricKey.Mode = CipherMode.CBC;
        //    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        //    int byteCount = 0;
        //    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
        //    {
        //        using (MemoryStream memStream = new MemoryStream(cipherTextBytes))
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
        //            {
        //                byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        //                memStream.Close();
        //                cryptoStream.Close();
        //            }
        //        }
        //    }
        //    symmetricKey.Clear();
        //    return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        //}

    }
}
