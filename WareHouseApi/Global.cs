namespace WareHouseApi
{
    public class Global
    {
        public static string MSSqlConnectionString { get; set; } = "Data Source=RKSQL.shzhleb.ru\\SQL2019; Initial Catalog=RKNET; User ID=rk7; Password=wZSbs6NKl2SF; TrustServerCertificate=True";

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
    }
}
