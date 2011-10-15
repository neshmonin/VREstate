package my.vrestate.client.core;


public class LookAt {
//	private LatLonAlt Position;
	private double Latitude = 0;
	private double Longitude = 0;
	private double Altitude = 0;
	private double Heading;
	private double Tilt;
	private double Range;


//	public void setLookAt() {
//		Events.FireEvent(EventTypes.SET_LOOKAT, this);
//	}

//	public void get() {
//		GEWrapper.getLookAt(this);
//	}
//	
//	public void set() {
//		GEWrapper.setLookAt(this);
//	}
//
//	public LookAt() {
//		// TODO Auto-generated constructor stub
//	}

//	public LookAt(double Lat, double Lon, double Alt, double Heading,
//			double Tilt, double Range) {
//		Position = new LatLonAlt(Lat, Lon, Alt);
//		this.Heading = Heading;
//		this.Tilt = Tilt;
//		this.Range = Range;
//	}

//	public LookAt(LookAt Src) {
//		Position = new LatLonAlt(Src.getPosition());
//		this.Heading = Src.getHeading();
//		this.Tilt = Src.getTilt();
//		this.Range = Src.getRange();
//	}

	public double getLatitude() {
//		GWT.log("Getting Latitude");
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

	public double getRange() {
		return Range;
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

	public void setRange(double range) {
		Range = range;
	}
}
