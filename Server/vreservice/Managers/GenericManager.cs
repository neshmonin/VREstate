using System;
using NHibernate;
using System.Collections.Generic;
using Vre.Server.Dao;
using Vre.Server.RemoteService;

namespace Vre.Server.BusinessLogic
{
    public class GenericManager : IDisposable
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
            if (_initiatedSession) { _session.Disconnect(); _initiatedSession = true; }
        }
    }
}