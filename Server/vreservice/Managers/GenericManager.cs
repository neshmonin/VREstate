using System;
using NHibernate;
using System.Collections.Generic;
using Vre.Server.Dao;
using Vre.Server.RemoteService;

namespace Vre.Server.BusinessLogic
{
    internal class GenericManager : IDisposable
    {
        protected ClientSession _session;
        private bool _initiatedSession;

        public GenericManager(ClientSession clientSession)
        {
            _initiatedSession = clientSession.Resume();
            _session = clientSession;
        }

        public void Dispose()
        {
            if (_initiatedSession) { _session.Disconnect(false); _initiatedSession = false; }
        }
    }
}