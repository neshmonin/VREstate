using System;
using System.Collections.Generic;
using System.Threading;

namespace Vre.Server
{
    internal class FloodPreventor : IDisposable
    {
        private Dictionary<string, DateTime> _peerInfo;
        private ManualResetEvent _cleanupThreadExit;
        private int _minAccessPeriodSec;
        private int _cleanupTimeoutSec;

        public FloodPreventor()
        {
            _peerInfo = new Dictionary<string, DateTime>();

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
		}

        public void Dispose()
        {
            _cleanupThreadExit.Set();
        }

        public void UpdatePeer(string peer)
        {
            DateTime now = DateTime.UtcNow;
            DateTime last = DateTime.MinValue;
            bool exists;

            lock (_peerInfo)
            {
                exists = _peerInfo.TryGetValue(peer, out last);
                if (exists) _peerInfo[peer] = now;
                else _peerInfo.Add(peer, now);
            }

            if (exists)
            {
                int diff = _minAccessPeriodSec - (int)now.Subtract(last).TotalSeconds;
                if (diff > 0) Thread.Sleep(diff * 1000);
            }
        }

        private void cleanupThread()
        {
            Thread.CurrentThread.Name = "FloodPreventor#" + Thread.CurrentThread.ManagedThreadId.ToString();
            while (!_cleanupThreadExit.WaitOne(60000))
            {
                //Thread.Sleep(60000);

                List<string> toremove = new List<string>();
                DateTime now = DateTime.UtcNow;

                lock (_peerInfo)
                {
                    foreach (KeyValuePair<string, DateTime> kvp in _peerInfo)
                        if (now.Subtract(kvp.Value).TotalSeconds > _cleanupTimeoutSec) toremove.Add(kvp.Key);

                    foreach (string peer in toremove) _peerInfo.Remove(peer);
                }
            }
        }
    }
}