using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Vre.Server
{
    internal class FloodPreventor : IDisposable
    {
		public const int TolerableUiDelayMs = 5000;

		class PeerInfo
		{
			public DateTime LastAccess;
			public int Counter;
			public PeerInfo() { LastAccess = DateTime.MinValue; Counter = -1; }
			public void Touch() { LastAccess = DateTime.UtcNow; Counter++; }
			public void Reset() { LastAccess = DateTime.MinValue; Counter = -1; }
		}

        private ConcurrentDictionary<string, PeerInfo> _peerInfo;

        private ManualResetEvent _cleanupThreadExit;
		private int _minAccessPeriodSec, _delayBaseMs;
        private int _cleanupTimeoutSec;

        public FloodPreventor()
        {
            _peerInfo = new ConcurrentDictionary<string, PeerInfo>();

			setup();
			Configuration.OnModified += new EventHandler((s, e) => setup());

            _cleanupThreadExit = new ManualResetEvent(false);
            new Thread(cleanupThread) { IsBackground = true }.Start();
        }

		private void setup()
		{
			var minAccessPeriodSec = Configuration.FloodPrevention.MinAccessPeriodSec.Value;
			var cleanupTimeoutSec = Configuration.FloodPrevention.AccessCleanupTimeoutSec.Value;

			// ensure some constraints
			//
			// cleanup period is large enough
			if (cleanupTimeoutSec < (minAccessPeriodSec * 2)) cleanupTimeoutSec = minAccessPeriodSec * 2;

			_minAccessPeriodSec = minAccessPeriodSec;
			_cleanupTimeoutSec = cleanupTimeoutSec;

			_delayBaseMs = _minAccessPeriodSec * 100; // (1/10-th)
		}

        public void Dispose()
        {
            _cleanupThreadExit.Set();
        }

        public void UpdatePeer(string peer)
        {
			var info = _peerInfo.GetOrAdd(peer, new PeerInfo());
            lock (info) info.Touch();
        }

		public int NeedDelay(string peer, bool touch)
		{
			var info = _peerInfo.GetOrAdd(peer, new PeerInfo());

			int delay;
			lock (info)
			{
				if (touch) info.Touch();

				delay = (int)(((Math.Exp(info.Counter) * (double)_delayBaseMs))
					- DateTime.UtcNow.Subtract(info.LastAccess).TotalMilliseconds);
				if (delay < 0) delay = 0;

				//	delay = (int)(Math.Exp(info.Counter) * (double)_delayBaseMs);
			}

			if (delay > 0)
			{
				int limit = _cleanupTimeoutSec * 1000;
				if (delay > limit) delay = limit;

				if (delay <= TolerableUiDelayMs)
				{
					Thread.Sleep(delay);
					delay = 0;
				}
			}
			return delay;
		}

		private void cleanupThread()
        {
            Thread.CurrentThread.Name = "FloodPreventor#" + Thread.CurrentThread.ManagedThreadId.ToString();
            while (!_cleanupThreadExit.WaitOne(60000))
            {
                //Thread.Sleep(60000);

                List<string> toremove = new List<string>();
                DateTime now = DateTime.UtcNow;

				foreach (var kvp in _peerInfo)
				{
					lock (kvp.Value)
					{
						if (now.Subtract(kvp.Value.LastAccess).TotalSeconds > _cleanupTimeoutSec)
						{
							PeerInfo deleted;
							_peerInfo.TryRemove(kvp.Key, out deleted);
						}
					}
				}
            }
        }
    }
}