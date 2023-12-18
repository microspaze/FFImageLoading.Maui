using System;
using System.Text;
using System.IO;

namespace FFImageLoading.Helpers
{
    public class MD5Helper : IMD5Helper
    {
        public string MD5(Stream stream)
        {
            using (var hashProvider = System.Security.Cryptography.MD5.Create())
            {
                var bytes = hashProvider.ComputeHash(stream);
                return BitConverter.ToString(bytes)?.ToSanitizedKey();
            }
        }

        public string MD5(string input)
        {
            using (var hashProvider = System.Security.Cryptography.MD5.Create())
            {
                var bytes = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes)?.ToSanitizedKey();
            }
        }
    }
}
