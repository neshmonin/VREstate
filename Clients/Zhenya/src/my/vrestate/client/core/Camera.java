package my.vrestate.client.core;

public class Camera {
	// public LatLonAlt Position = new LatLonAlt();
	private double Latitude = 0;
	private double Longitude = 0;
	private double Altitude = 0;

	private double Heading = 0;
	private double Tilt = 0;
	private double Roll = 0;

	// public void MoveTo(double[] args) {
	// try {
	// Position.setLat(args[0]);
	// Position.setLon(args[1]);
	// Position.setAlt(args[2]);
	// Heading = args[3];
	// Tilt = args[4];
	// Roll = args[5];
	// } catch (Exception e) {
	//
	// }
	// }

	// public Camera(double[] args) {
	// Position.setLat(args[0]);
	// Position.setLon(args[1]);
	// Position.setAlt(args[2]);
	// Heading = args[3];
	// Tilt = args[4];
	// Roll = args[5];
	// }

	// public Camera(double Lat, double Lon, double Alt, double Heading,
	// double Tilt, double Roll) {
	// Position = new LatLonAlt();
	// Position.setLat(Lat);
	// Position.setLon(Lon);
	// Position.setAlt(Alt);
	// this.Heading = Heading;
	// this.Tilt = Tilt;
	// this.Roll = Roll;
	// }

	public Camera() {
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

	public double getHeading() {
		return Heading;
	}

	public double getTilt() {
		return Tilt;
	}

	public double getRoll() {
		return Roll;
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

	public void setHeading(double heading) {
		Heading = heading;
	}

	public void setTilt(double tilt) {
		Tilt = tilt;
	}

	public void setRoll(double roll) {
		Roll = roll;
	};

	public void setPosition(Point Position) {
//		Window.alert("1");
		Latitude = Position.getLatitude();
//		Window.alert("2");
		Longitude = Position.getLongitude();
//		Window.alert("3");
		Altitude = Position.getAltitude();
	};

}
