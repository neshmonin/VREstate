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

            _minAccessPeriodSec = 30;
            _minAccessPeriodSec = ServiceInstances.Configuration.GetValue("MinAccessPeriodSec", _minAccessPeriodSec);
            _cleanupTimeoutSec = 600;
            _cleanupTimeoutSec = ServiceInstances.Configuration.GetValue("AccessCleanupTimeoutSec", _cleanupTimeoutSec);

            // ensure some constraints
            //
            // minimal reaccess time is 1 sec
            if (_minAccessPeriodSec < 1) _minAccessPeriodSec = 1;
            // reaccess time is no more than 30 minutes
            if (_minAccessPeriodSec > 900) _minAccessPeriodSec = 900;
            // cleanup period is large enough
            if (_cleanupTimeoutSec < (_minAccessPeriodSec * 2)) _cleanupTimeoutSec = _minAccessPeriodSec * 2;

            _cleanupThreadExit = new ManualResetEvent(false);
            new Thread(cleanupThread) { IsBackground = true }.Start();
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