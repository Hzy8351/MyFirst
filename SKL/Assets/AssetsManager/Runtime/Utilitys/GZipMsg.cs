using ICSharpCode.SharpZipLib.GZip;
using System.IO;

namespace Assets
{
    public class GZip
    {
        /// 压缩字节数组
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipOutputStream gzip = new GZipOutputStream(ms))
                {
                    gzip.Write(inputBytes, 0, inputBytes.Length);
                    gzip.Close();

                    return ms.ToArray();
                }
            }
        }

        /// 解压缩字节数组
        public static byte[] Decompress(byte[] inputBytes, int offset = 0)
        {
            using (GZipInputStream gzi = new GZipInputStream(new MemoryStream(inputBytes, offset, inputBytes.Length - offset)))
            {
                MemoryStream re = new MemoryStream();
                int count = 0;
                byte[] data = new byte[4096];
                while ((count = gzi.Read(data, 0, data.Length)) != 0)
                {
                    re.Write(data, 0, count);
                }
                return re.ToArray();
            }
        }
    }
}

