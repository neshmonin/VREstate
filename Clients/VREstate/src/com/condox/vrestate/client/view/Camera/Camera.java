package com.condox.vrestate.client.view.Camera;

import com.condox.vrestate.client.ge.GE;
import com.google.gwt.user.client.Window;
import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;
import com.nitrous.gwt.earth.client.api.KmlCamera;
import com.nitrous.gwt.earth.client.api.KmlLookAt;

public class Camera {
    public enum Type
    {
        LookAt,
        Camera
    }

    public Type CameraType;
    public CameraAttributes attributes = new CameraAttributes();
    public double RangeMax_m = 5000;
    public double RangeMin_m = 20;
    public double HeadingMin_d = -70;
    public double HeadingMax_d = 70;
    public double TiltMin_d = 20;
    public double TiltMax_d = 120;

    protected double HeadingInitial_d;
    protected double TiltInitial_d;

    public Camera(Type cameraType,
                  double heading_d,
                  double tilt_d,
                  double roll_d,
                  double lat_d,
                  double lon_d,
                  double alt_m,
                  double range_m)
    {
        attributes = new CameraAttributes(heading_d,
                                       tilt_d,
                                       roll_d,
                                       lat_d,
                                       lon_d,
                                       alt_m,
                                       range_m);
        CameraType = cameraType;

        HeadingInitial_d = NormalizeHeading_d(heading_d);
        TiltInitial_d    = tilt_d;
    }

    public Camera(KmlLookAt lookAt)
    {
        attributes = new CameraAttributes(lookAt.getHeading(),
                                       lookAt.getTilt(),
                                       0d,
                                       lookAt.getLatitude(),
                                       lookAt.getLongitude(),
                                       lookAt.getAltitude(),
                                       lookAt.getRange());
        CameraType = Type.LookAt;

        HeadingInitial_d = attributes.Heading_d;
        TiltInitial_d = attributes.Tilt_d;
    }

    public Camera(KmlCamera camera)
    {
        attributes = new CameraAttributes(camera.getHeading(),
                                       camera.getTilt(),
                                       camera.getRoll(),
                                       camera.getLatitude(),
                                       camera.getLongitude(),
                                       camera.getAltitude(),
                                       0d);
        CameraType = Type.Camera;

        HeadingInitial_d = attributes.Heading_d;
        TiltInitial_d = attributes.Tilt_d;
    }

    public Camera(Camera copyFrom)
    {
        CameraType = copyFrom.CameraType;
        attributes = new CameraAttributes(copyFrom.attributes);
        HeadingMax_d = copyFrom.HeadingMax_d;
        HeadingMin_d = copyFrom.HeadingMin_d;
        RangeMax_m = copyFrom.RangeMax_m;
        RangeMin_m = copyFrom.RangeMin_m;
        TiltMax_d = copyFrom.TiltMax_d;
        TiltMin_d = copyFrom.TiltMin_d;

        HeadingInitial_d = copyFrom.HeadingInitial_d;
        TiltInitial_d = copyFrom.TiltInitial_d;
    }
    
    boolean moved = true;
    public boolean isMoved() {
    	
//    	if (CameraType == Type.LookAt) {
//    		KmlLookAt lookAt = GE.getView().copyAsLookAt(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
//    		moved = moved || (Math.abs(lookAt.getLatitude() - attributes.Lat_d) > 0.001d); 
//    		moved = moved || (Math.abs(lookAt.getLongitude() - attributes.Lon_d) > 0.001d); 
//    		moved = moved || (Math.abs(lookAt.getHeading() - attributes.Heading_d) > 0.001d); 
//    		moved = moved || (Math.abs(lookAt.getRange() - attributes.Range_m) > 0.001d); 
//    		moved = moved || (Math.abs(lookAt.getTilt() - attributes.Tilt_d) > 0.001d); 
//    		moved = moved || (Math.abs(lookAt.getAltitude() - attributes.Alt_m) > 0.001d); 
//    	}
//    	else 
//    	if (CameraType == Type.Camera) {
//    		KmlCamera camera = GE.getView().copyAsCamera(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);
//    		moved |= camera.getLatitude() != attributes.Lat_d;
//    		moved |= camera.getLongitude() != attributes.Lon_d;
//    		moved |= camera.getAltitude() != attributes.Alt_m;
//    		moved |= camera.getHeading() != attributes.Heading_d;
//    		moved |= camera.getRoll() != attributes.Roll_d;
//    		moved |= camera.getTilt() != attributes.Tilt_d;
//    	}
    	moved = !moved;
    	return moved;    		
    	
    }

    public void Apply()
    {
        if (CameraType == Type.LookAt)
        {
            KmlLookAt lookAt = GE.getView().copyAsLookAt(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);

            lookAt.setLatitude(attributes.Lat_d);
            lookAt.setLongitude(attributes.Lon_d);
            lookAt.setHeading(attributes.Heading_d);
            lookAt.setRange(attributes.Range_m);
            lookAt.setTilt(attributes.Tilt_d);
            lookAt.setAltitude(attributes.Alt_m);

            GE.getView().setAbstractView(lookAt);
        }
        else
        if (CameraType == Type.Camera)
        {
            KmlCamera camera = GE.getView().copyAsCamera(KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND);

            camera.setLatitude(attributes.Lat_d);
            camera.setLongitude(attributes.Lon_d);
            camera.setAltitude(attributes.Alt_m);
            camera.setHeading(attributes.Heading_d);
            camera.setRoll(attributes.Roll_d);
            camera.setTilt(attributes.Tilt_d);

            GE.getView().setAbstractView(camera);
        }
    }

    public void MoveLookAt(double deltaHeading_d,
                           double deltaTilt_d,
                           double deltaRange_m)
    {
        if (CameraType != Type.LookAt)
            return;

        double heading = attributes.Heading_d + deltaHeading_d;
        if (heading > 360)
            heading = heading - 360;
        if (heading < 0)
            heading = heading + 360;

        double tilt = attributes.Tilt_d + deltaTilt_d;
        if (tilt > 90)
            tilt = 90;
        if (tilt < 0)
            tilt = 0;

        double range = attributes.Range_m + deltaRange_m;
        if (range > RangeMax_m)
            range = RangeMax_m;
        if (range < RangeMin_m)
            range = RangeMin_m;

        attributes.Range_m = range;
        attributes.Heading_d = heading;
        attributes.Tilt_d = tilt;

        Apply();
    }

    public static double NormalizeHeading_d(double heading)
    {
        if (heading > 360)
        	heading = heading - 360;
        if (heading < 0)
        	heading = heading + 360;
        
        return heading;
    }
    
    public void MoveCamera(double deltaHeading_d,
                           double deltaTilt_d)
    {
        if (CameraType != Type.Camera)
            return;

        double newHeading = NormalizeHeading_d(attributes.Heading_d + deltaHeading_d);
        double newTilt = attributes.Tilt_d + deltaTilt_d;


        double absoluteDeltaHeading = HeadingInitial_d - newHeading;
        absoluteDeltaHeading -= (absoluteDeltaHeading >  180)? 360 : 0;
        absoluteDeltaHeading += (absoluteDeltaHeading < -180)? 360 : 0;
        if ((absoluteDeltaHeading < HeadingMax_d && absoluteDeltaHeading > HeadingMin_d)
        	&&
        	(newTilt < TiltMax_d && newTilt > TiltMin_d))
        {
            attributes.Heading_d = newHeading;
            attributes.Tilt_d = newTilt;

            Apply();
        }
    }

}
