package com.condox.orders.client;

import java.util.ArrayList;

import com.nitrous.gwt.earth.client.api.KmlAltitudeMode;

public class Point {

	private double latitude = 0;
	private double longitude = 0;
	private double altitude = 0;
	private KmlAltitudeMode altitude_mode = KmlAltitudeMode.ALTITUDE_RELATIVE_TO_GROUND;
	
	public void CalcCenter(ArrayList<Point> points) {
		double lat = 0;
		double lon = 0;
		double alt = 0;
		int count = 0;
		for (Point point : points) {
			lat += point.latitude;
			lon += point.longitude;
			alt += point.altitude;
			count++;
		}
		if (count > 0) {
			latitude = lat / count;
			longitude = lon / count;
			altitude = alt / count;
		}
	}

	public double getLatitude() {
		return latitude;
	}

	public void setLatitude(double latitude) {
		this.latitude = latitude;
	}

	public double getLongitude() {
		return longitude;
	}

	public void setLongitude(double longitude) {
		this.longitude = longitude;
	}

	public double getAltitude() {
		return altitude;
	}

	public void setAltitude(double altitude) {
		this.altitude = altitude;
	}

	public KmlAltitudeMode getAltitudeMode() {
		return altitude_mode;
	}

	public void setAltitudeMode(KmlAltitudeMode altitude_mode) {
		this.altitude_mode = altitude_mode;
	}

}
