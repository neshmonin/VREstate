using System;
using System.Collections.Generic;

namespace Vre.Server.BusinessLogic
{
    /// <summary>
    /// Geometry list in model's terminology
    /// </summary>
    [Serializable]
    public class SuiteClass : IClientDataProvider
    {
        private string _className;
        private Wireframe[] _model;

        public SuiteClass(string className, Wireframe[] model)
        {
            _className = className;
            _model = model;
        }

        public ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("name", _className);

            int idx = 0;
            ClientData[] model = new ClientData[_model.Length];
            foreach (Wireframe wf in _model) model[idx++] = wf.GetClientData();
            result.Add("geometries", model);  // keep model's terminology

            return result;
        }

        public bool UpdateFromClient(ClientData data)
        {
            throw new NotImplementedException();
        }
    }
}
