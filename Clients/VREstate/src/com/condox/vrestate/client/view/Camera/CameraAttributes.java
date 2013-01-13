package com.condox.vrestate.client.view.Camera;

import com.condox.vrestate.client.view._AbstractView;
import com.condox.vrestate.client.view.GeoItems.IGeoItem;

public class CameraAttributes {
    public double Heading_d;
    public double Tilt_d;
    public double Roll_d;
    public double Lat_d;
    public double Lon_d;
    public double Alt_m;
    public double Range_m;

    public CameraAttributes() { }

    public CameraAttributes(double heading_d,
                            double tilt_d,
                            double roll_d,
                            double lat_d,
                            double lon_d,
                            double alt_m,
                            double range_m)
    {
        Heading_d = heading_d;
        Tilt_d = tilt_d;
        Roll_d = roll_d;
        Lat_d = lat_d;
        Lon_d = lon_d;
        Alt_m = alt_m;
        Range_m = range_m;
    }

    private static double RadiansToDegrees(double radians)
    {
        return (180d / Math.PI) * radians;
    }

    public CameraAttributes(double x, double y, double z)
    {
    	IGeoItem currentGeoItem = _AbstractView.getCurrentGeoItem();
        if (currentGeoItem != null)
        	SetLonLatAlt(currentGeoItem);
        else
        {
            Lon_d = 0;
            Lat_d = 0;
            Alt_m = 0;
        }
        Roll_d = 0;

        double horizRange_m = Math.sqrt(x * x + y * y);
        Heading_d = 180 + RadiansToDegrees(Math.atan2(x, y));
        if (Heading_d >= 360)
            Heading_d = Heading_d - 360;

        Tilt_d = RadiansToDegrees(Math.atan2(horizRange_m, z));
        Range_m = Math.sqrt(horizRange_m * horizRange_m + z * z);
    }

    public CameraAttributes(CameraAttributes copyFrom)
    {
        Heading_d = copyFrom.Heading_d;
        Tilt_d = copyFrom.Tilt_d;
        Roll_d = copyFrom.Roll_d;
        Lat_d =  copyFrom.Lat_d;
        Lon_d =  copyFrom.Lon_d;
        Alt_m =  copyFrom.Alt_m;
        Range_m = copyFrom.Range_m;
    }

    public void SetLonLatAlt(IGeoItem geoItem)
    {
        Lat_d = geoItem.getPosition().getLatitude();
        Lon_d = geoItem.getPosition().getLongitude();
        Alt_m = geoItem.getPosition().getAltitude();
    }
}
