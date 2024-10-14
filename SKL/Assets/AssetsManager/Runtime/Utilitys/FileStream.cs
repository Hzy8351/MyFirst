
using System;
using System.IO;

namespace Assets
{
    public class ABStream : Stream
    {
        Stream mStream;
        
        public static ABStream Create( BundleInfo info)
        {
            var stream = Manager.OpenRead(info);
            if (stream != null)
                return new ABStream(stream);
            return null;
        }

        ABStream( Stream stream)
        {
            mStream = stream;
        }

        public override bool CanRead => mStream.CanRead;

        public override bool CanSeek => mStream.CanSeek;

        public override bool CanWrite => mStream.CanWrite;

        public override long Length => mStream.Length;

        public override long Position { get => mStream.Position; set => mStream.Position = value; }

        public override void Flush()
        {
            mStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long pos = mStream.Position;
            // 读取数据
            int v = mStream.Read(buffer, offset, count);
            // 解密
            int len = ENRES.ENCRIPT_LEN - (int)pos;
            if (len < 0)
                return v;
            int enlen = Math.Min(v, len);
            for (int i = 0; i < enlen; i++)
                buffer[i] = (byte)(buffer[i] ^ ENRES.XORTABLE[(pos + i) % 1024]);
            return v;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var ret = mStream.Seek(offset, origin);
            return ret;
        }

        public override void SetLength(long value)
        {
            mStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            mStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            mStream?.Dispose();
            mStream = null;
        }

    }
}
