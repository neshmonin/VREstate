using System;
using System.Net;
using System.Collections.Generic;

namespace Vre
{
	public class IPAddressRange
	{
		private byte[] _networkAddress;
		private int _networkMaskByteIdx, _maskLen;
		private byte _networkMaskByte;

		public IPAddressRange(string cidrRange)
		{
			var pos = cidrRange.IndexOf('/');
			if (pos < 0) throw new FormatException("The range argument does not follow CIDR notation.");

			var addr = IPAddress.Parse(cidrRange.Substring(0, pos));
			_maskLen = int.Parse(cidrRange.Substring(pos + 1));
			_networkAddress = addr.GetAddressBytes();

			if ((_maskLen < 0) || (_maskLen > _networkAddress.Length * 8))
				throw new FormatException("Network prefix is out of range.");

			_networkMaskByteIdx = _maskLen >> 3;
			if (_networkMaskByteIdx == _networkAddress.Length)
			{
				_networkMaskByteIdx--;
				_networkMaskByte = 0xFF;
			}
			else
			{
				_networkMaskByte = (byte)(0xFF << (8 - (_maskLen & 7)));
				_networkAddress[_networkMaskByteIdx] &= _networkMaskByte;
			}
			for (int idx = _networkMaskByteIdx + 1; idx < _networkAddress.Length; idx++)
				_networkAddress[idx] = 0;
		}

		public bool IsInRange(IPAddress addr)
		{
			var bytes = addr.GetAddressBytes();
			if (bytes.Length == _networkAddress.Length)  // same address type
			{
				for (int idx = _networkMaskByteIdx - 1; idx >= 0; idx--)
					if (_networkAddress[idx] != bytes[idx]) return false;
				if ((bytes[_networkMaskByteIdx] & _networkMaskByte) != _networkAddress[_networkMaskByteIdx])
					return false;
				return true;
			}
			return false;
		}

		public IPAddress RangeMin { get { return null; } }
		public IPAddress RangeMax { get { return null; } }

		public override string ToString()
		{
			return string.Format("{0}/{1}", new IPAddress(_networkAddress), _maskLen);
		}
	}

	public class IPRangeFilter
	{
		private readonly List<IPAddressRange> _inclusionRanges = new List<IPAddressRange>();
		private readonly List<IPAddressRange> _exclusionRanges = new List<IPAddressRange>();

		public void Clear()
		{
			_inclusionRanges.Clear();
			_exclusionRanges.Clear();
		}

		public void AddIncludeRange(IPAddressRange range)
		{
			_inclusionRanges.Add(range);
			test();
		}

		public void AddExcludeRange(IPAddressRange range)
		{
			_inclusionRanges.Add(range);
			test();
		}

		public void AddIncludeRanges(string ranges)
		{
			foreach (var rng in ranges.Split(','))
			{
				if (string.IsNullOrWhiteSpace(rng)) continue;
				_inclusionRanges.Add(new IPAddressRange(rng.Trim()));
			}
			test();
		}

		public void AddExcludeRanges(string ranges)
		{
			foreach (var rng in ranges.Split(','))
			{
				if (string.IsNullOrWhiteSpace(rng)) continue;
				_exclusionRanges.Add(new IPAddressRange(rng.Trim()));
			}
			test();
		}

		private void test()
		{
			foreach (var excl in _exclusionRanges)
			{
				var found = false;
				foreach (var incl in _inclusionRanges)
				{
					if (incl.IsInRange(excl.RangeMin))
					{
						if (!incl.IsInRange(excl.RangeMax))
							throw new ArgumentException(string.Format(
								"Exclusion range {0} does not completely fit into inclusion range {1}.",
								excl, incl));
						found = true;
					}
					else if (incl.IsInRange(excl.RangeMax))
					{
						throw new ArgumentException(string.Format(
							"Exclusion range {0} does not completely fit into inclusion range {1}.",
							excl, incl));
					}
				}
				if (!found)
					throw new ArgumentException(string.Format(
						"Exclusion range {0} does not belong to any inclusion range or hides inclusion range.",
						excl));
			}
		}

		public bool IsInRange(IPAddress addr)
		{
			var result = false;

			foreach (var range in _inclusionRanges)
				if (range.IsInRange(addr)) { result = true; break; }

			if (result)
			{
				foreach (var range in _exclusionRanges)
					if (range.IsInRange(addr)) { result = false; break; }
			}

			return result;
		}
	}
}