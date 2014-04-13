using System;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Vre
{
	public class VirtualMemoryStream : Stream
	{
		private byte[] _field;
		private long _headOffset, _length;

		public VirtualMemoryStream(byte[] field, long headOffset, long length)
		{
			_field = field;
			_headOffset = headOffset;
			_length = length;
			Position = 0L;
		}

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return true; } }
		[ComVisible(false)]
		public override bool CanTimeout { get { return false; } }
		public override bool CanWrite { get { return false; } }
		public override long Length { get { return _length; } }
		public override long Position { get; set; }

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException();
		}

		public override void Close() { }

		public override void Flush() { }

		public new void CopyTo(Stream destination)
		{
			long remaining = (_length - Position);
			destination.Write(_field, (int)(_headOffset + Position), (int)remaining);
			Position = _length;
		}

		public new void CopyTo(Stream destination, int bufferSize)
		{
			long remaining;
			do
			{
				remaining = _length - Position;
				int toCopy = (remaining > bufferSize) ? bufferSize : (int)remaining;
				destination.Write(_field, (int)(_headOffset + Position), toCopy);
				Position += toCopy;
			} while (remaining > 0);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			// TODO: ???
			throw new NotSupportedException();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			// TODO: ???
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			long remaining = _length - Position;
			int result = (remaining > count) ? count : (int)remaining;
			Buffer.BlockCopy(_field, (int)(_headOffset + Position), buffer, offset, result);
			//Array.Copy(_field, _headOffset + Position, buffer, offset, result);
			Position += result;
			return result;
		}

		[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		public override int ReadByte()
		{
			if (Position >= _length) return -1;
			int result = _field[_headOffset + Position];
			Position++;
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			long t;
			switch (origin)
			{
				case SeekOrigin.Begin:
					t = offset;
					break;

				case SeekOrigin.Current:
					t = Position + offset;
					break;

				case SeekOrigin.End:
					t = _length + offset;
					break;

				default:
					throw new InvalidOperationException();
			}

			if (t <= 0) Position = 0L;
			else if (t > _length) Position = _length;
			else Position = t;

			return Position;
		}
	}
}