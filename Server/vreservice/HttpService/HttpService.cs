using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using Vre.Server.RemoteService;

namespace Vre.Server.HttpService
{
    internal class HttpServiceMain : HttpServiceBase
    {
        private RemoteServiceProvider _rsp;
        private long _maxRequestBodyLength;
		private RefererXref _refererXref;

        public HttpServiceMain() : base("HTTP Listener")
        {
            _rsp = new RemoteServiceProvider();
            _maxRequestBodyLength = ServiceInstances.Configuration.GetValue("MaxHttpRequestBodyLengthBytes", 10240);
			_refererXref = new RefererXref();
        }

        protected override IResponseData process(string browserKey, HttpListenerContext ctx, HttpServiceRequest.ProcessResponse proc)
        {
            HttpServiceRequest rq = new HttpServiceRequest(ctx, browserKey, 
				_refererXref.GetRealReferer(browserKey, ctx.Request.UrlReferrer), 
				_path, _maxRequestBodyLength, proc);

            if (_allowExtendedLogging) ServiceInstances.RequestLogger.Info(prepareCallerInfo(browserKey, ctx, rq));

            if (!rq.UserInfo.StaleSession)
            {
                if (rq.UserInfo.Session != null)
                {
                    lock (rq.UserInfo.Session)
                    {
                        try
                        {
							// TODO: Currently ANY request opens a DB session!
							// NHibernate's connection pooling should take care of this.
                            rq.UserInfo.Session.Resume();
                            _rsp.ProcessRequest(this, rq);
                            rq.UserInfo.Session.Disconnect(false);
                        }
                        catch (NHibernate.HibernateException)
                        {
                            rq.UserInfo.Session.Disconnect(true);
                            throw;
                        }
                        catch
                        {
                            rq.UserInfo.Session.Disconnect(false);
                            throw;
                        }
                    }
                }
                else
                {
                    _rsp.ProcessRequest(this, rq);
                }
            }

            return rq.Response;
        }
    }

	internal class RefererXref
	{
		private ConcurrentDictionary<string, Uri> _xref = new ConcurrentDictionary<string, Uri>();
		private ConcurrentDictionary<string, DateTime> _lastUsed = new ConcurrentDictionary<string, DateTime>();

		public RefererXref()
		{
			new Thread(cleanerThread) 
			{ IsBackground = true, Priority = ThreadPriority.BelowNormal }
			.Start();
		}

		public Uri GetRealReferer(string browserKey, Uri currentReferer)
		{
			Uri result = null;
			if (isInternalReferer(currentReferer))
			{
				if (_xref.TryGetValue(browserKey, out result))
					_lastUsed[browserKey] = DateTime.UtcNow;
				else
					result = null;
			}
			else if (currentReferer != null)
			{
				_xref[browserKey] = currentReferer;
				_lastUsed[browserKey] = DateTime.UtcNow;
				result = currentReferer;
			}
			return result;
		}

		private bool isInternalReferer(Uri referer)
		{
			return ((referer != null) && referer.Authority.EndsWith(".3dcondox.com"));
		}

		private DateTime _nextStat;
		private int _maxSize;

		private void cleanerThread()
		{
			Thread.CurrentThread.Name = "RefererXrefCleaner#" + Thread.CurrentThread.ManagedThreadId;
			int cleanupTimeoutSec = ServiceInstances.SessionStore.CleanupTimeoutSec + 10;
			_nextStat = DateTime.UtcNow.AddHours(1);
			_maxSize = 0;

			while (true)
			{
				Thread.Sleep(60000);
				var now = DateTime.UtcNow;

				int dbgrm = 0;
				var bks = _lastUsed.Keys.ToArray();
				foreach (var bk in bks)
				{
					DateTime t;
					Uri u;
					if (_lastUsed.TryGetValue(bk, out t))
					{
						if (now.Subtract(t).TotalSeconds > cleanupTimeoutSec)
						{
							_xref.TryRemove(bk, out u);
							_lastUsed.TryRemove(bk, out t);
							dbgrm++;
						}
					}
				}

				if (bks.Length > _maxSize) _maxSize = bks.Length;
				if (dbgrm > 0) ServiceInstances.Logger.Debug("RefererXref removed {0} items.", dbgrm);

				if (now >= _nextStat)
				{
					_nextStat = now.AddHours(1);
					ServiceInstances.Logger.Debug(
						"RefererXref: current store size is {0} records; max={1}.",
						_xref.Count, _maxSize);
				}
			}
		}
	}
}