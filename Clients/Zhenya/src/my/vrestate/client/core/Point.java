package my.vrestate.client.core;

public class Point {

	private double Latitude = 0;
	private double Longitude = 0;
	private double Altitude = 0;

	public Point clone() {
		Point point = new Point();
		point.setLatitude(getLatitude());
		point.setLongitude(getLongitude());
		point.setAltitude(getAltitude());
		return null;
	}

	public double getLatitude() {
		return Latitude;
	}

	public double getLongitude() {
		return Longitude;
	}

	public double getAltitude() {
		return Altitude;
	}

	public void setLatitude(double latitude) {
		Latitude = latitude;
	}

	public void setLongitude(double longitude) {
		Longitude = longitude;
	}

	public void setAltitude(double altitude) {
		Altitude = altitude;
	}

}
