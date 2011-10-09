using System;

namespace Vre.Server.BusinessLogic
{
    [Serializable]
    public class ViewPoint : GeoPoint, IClientDataProvider
    {
        public double HorizontalHeading { get; set; }
        public double VerticalHeading { get; set; }

        protected ViewPoint() { }

        public ViewPoint(double longitude, double latitude, double altitude, 
            double horizontalHeading, double verticalHeading)
            : base(longitude, latitude, altitude)
        {
            HorizontalHeading = horizontalHeading;
            VerticalHeading = verticalHeading;
        }

        public ViewPoint(double longitude, double latitude, double altitude,
            double horizontalHeading)
            : base(longitude, latitude, altitude)
        {
            HorizontalHeading = horizontalHeading;
            VerticalHeading = 0.0;
        }

        public ViewPoint(double longitude, double latitude, double horizontalHeading)
            : base(longitude, latitude)
        {
            HorizontalHeading = horizontalHeading;
            VerticalHeading = 0.0;
        }

        public new ClientData GetClientData()
        {
            ClientData result = base.GetClientData();

            result.Add("hdg", HorizontalHeading);
            result.Add("vhdg", VerticalHeading);

            return result;
        }

        public new bool UpdateFromClient(ClientData data)
        {
            bool changed = base.UpdateFromClient(data);

            HorizontalHeading = data.UpdateProperty("hdg", HorizontalHeading, ref changed);
            VerticalHeading = data.UpdateProperty("vhdg", VerticalHeading, ref changed);

            return changed;
        }

        public static ViewPoint Empty { get { return new ViewPoint(0.0, 0.0, 0.0); } }
    }
}