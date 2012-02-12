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

        private ValueWithUM _area;
        private int _bedroomCount, _denCount, _bathroomCount, _balconyCount, _terraceCount;

        public SuiteClass(string className, Wireframe[] model)
        {
            _className = className;
            _model = model;
            _area = ValueWithUM.EmptyArea;
            _bedroomCount = -1;
            _denCount = -1;
            _balconyCount = -1;
            _bathroomCount = -1;
            _terraceCount = -1;
        }

        public bool NeedUpdateFromType { get { return (_bedroomCount >= 0); } }

        public void UpdateFromType(SuiteType suiteType)
        {
            _area = suiteType.FloorArea;
            _bathroomCount = suiteType.BathroomCount;
            _denCount = suiteType.DenCount;
            _bedroomCount = suiteType.BedroomCount;
            _balconyCount = suiteType.BalconyCount;
            _terraceCount = suiteType.TerraceCount;
        }

        public ClientData GetClientData()
        {
            ClientData result = new ClientData();

            result.Add("name", _className);

            int idx = 0;
            ClientData[] model = new ClientData[_model.Length];
            foreach (Wireframe wf in _model) model[idx++] = wf.GetClientData();
            result.Add("geometries", model);  // keep model's terminology

            result.Add("area", 1200);
            result.Add("areaUm", "sqFt");

            result.Add("bedrooms", 2);
            result.Add("dens", 0);
            result.Add("bathrooms", 2);
            result.Add("balconies", 1);
            result.Add("terraces", 0);

            return result;
        }

        public bool UpdateFromClient(ClientData data)
        {
            throw new NotImplementedException();
        }
    }
}
