namespace Raygun.Owin
{
    using System;
    using System.IO;

    public class SeekableStreamWrapper : Stream
    {
        private readonly Stream _innerStream;

        public SeekableStreamWrapper(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _innerStream = stream;

            if (!_innerStream.CanSeek)
            {
                var sourceStream = _innerStream;

                _innerStream = new MemoryStream();

                sourceStream.CopyTo(this);
            }

            _innerStream.Position = 0;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return _innerStream.CanSeek; }
        }

        public override bool CanTimeout
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { return _innerStream.Length; }
        }

        public override long Position
        {
            get { return _innerStream.Position; }
            set
            {
                if (value < 0) throw new InvalidOperationException("The position of the stream cannot be set to less than zero.");
                if (value > Length) throw new InvalidOperationException("The position of the stream cannot exceed the length of the stream.");

                _innerStream.Position = value;
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerStream.Dispose();
            }

            base.Dispose(disposing);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _innerStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _innerStream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return _innerStream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }
    }
}