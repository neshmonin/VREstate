package com.condox.clientshared.document;

import java.util.ArrayList;

import com.condox.clientshared.abstractview.Log;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;

public class Suite implements I_VRObject {

	public enum Status {
		Sold, Available, OnHold, ResaleAvailable, AvailableRent, NotSupported
	}

	private int id = -1;
	private int parent_id = -1;
	private Building parent = null;
	private int level_number = -1;
	private String floor_name = "";
	private String name = "";
	private int ceiling_height_ft = 0;
	// private boolean show_panoramic_view = true;
	private Status status = null;
	private Position position = new Position();
	private SuiteType suite_type = null;
	private int price = 0;
	private String currency;
	private ArrayList<Double> points = new ArrayList<Double>();
	private String vTourUrl = null;
	private String infoUrl = null;
	private String floorplan_url = "";
	public enum Orientation {
		Normal,
		Horizontal
	}
	public Orientation orientation = Orientation.Normal;

	void ParseDynamic(JSONObject obj) {
		JSONString _name = obj.get("name").isString();
		name = _name.stringValue();

		if (obj.get("status").isString() != null) {
			String status = obj.get("status").isString().stringValue();
			if (status.equals("Available"))
				this.status = Status.Available;
			else if (status.equals("ResaleAvailable"))
				this.status = Status.ResaleAvailable;
			else if (status.equals("Sold"))
				this.status = Status.Sold;
			else if (status.equals("OnHold"))
				this.status = Status.OnHold;
			else if (status.equals("AvailableRent"))
				this.status = Status.AvailableRent;
			else
				this.status = Status.NotSupported;
		} else
			this.status = Status.NotSupported;
		
		price = -1;
		
		if ((obj.get("currentPrice") != null)
				&& (obj.get("currentPrice").isString() != null)
					&& obj.get("currentPrice").isString().stringValue() != null)
			price = Double.valueOf(obj.get("currentPrice").isString().stringValue()).intValue();
	}

	@Override
	public JSONObject toJSONObject() {
		return null;
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		id = (int) obj.get("id").isNumber().doubleValue();

		ParseDynamic(obj);
		parent_id = (int) obj.get("buildingId").isNumber().doubleValue();
		// JSONNumber _level_number = obj.get("levelNumber").isNumber();
		// level_number = (int) _level_number.doubleValue();

		JSONString str = obj.get("floorName").isString();
		if (str != null)
			floor_name = str.stringValue();

		if (obj.containsKey("currentPriceCurrency")) {
			str = obj.get("currentPriceCurrency").isString();
			if (str != null)
				currency = str.stringValue();
		}

		level_number = (int) obj.get("levelNumber").isNumber().doubleValue();

		if (obj.containsKey("ceilingHeightFt"))
			ceiling_height_ft = (int) obj.get("ceilingHeightFt").isNumber()
					.doubleValue();

		if (obj.containsKey("position")) {
			JSONObject position = obj.get("position").isObject();
			double longitude = position.get("lon").isNumber().doubleValue();
			double latitude = position.get("lat").isNumber().doubleValue();
			double altitude = position.get("alt").isNumber().doubleValue();
			double heading = position.get("hdg").isNumber().doubleValue();

			this.position.setLongitude(longitude);
			this.position.setLatitude(latitude);
			this.position.setAltitude(altitude);

			this.position.setHeading(heading);
		}

		this.parent = Document.get().getBuildings().get(getParent_id());
		int type_id = (int) obj.get("suiteTypeId").isNumber().doubleValue();

		suite_type = Document.get().getSuiteTypes().get(type_id);
		if (suite_type.getName().endsWith("-HORIZ-"))
			orientation = Orientation.Horizontal;
		
//		try {
//			price = Double.valueOf(obj.get("currentPrice").isString().stringValue()).intValue();
//		} catch (NumberFormatException e) {
//			e.printStackTrace();
//		} catch (NullPointerException e) {
//			e.printStackTrace();	
//		}
		
//		if ((obj.get("currentPrice") != null)
//				&& (obj.get("currentPrice").isNumber() != null)
//				&& obj.get("currentPrice").isNumber().doubleValue() != 0)
//			price = (int) obj.get("currentPrice").isNumber().doubleValue();

//		if (Options.DEBUG_MODE && price == -1) {
//			price = (int) (500 + 500 * Math.random());
//			price *= 1000;
//		}
	}

	public void CalcLineCoords() {
		double METERS_PER_DEGREES = 111111;
		int i = 0;
		SuiteType suite_type = this.getSuiteType();

		if (suite_type == null)
			return;

		while (i < suite_type.getPoints().size()) {
			// X
			double lat = this.getPosition().getLatitude()
					+ (suite_type.getPoints().get(i) + 10 / 2.54) * 0.0254
					* Math.cos(Math.toRadians(this.getPosition().getHeading()))
					/ METERS_PER_DEGREES;
			double lon = this.getPosition().getLongitude()
					+ (suite_type.getPoints().get(i) + 10 / 2.54)
					* 0.0254
					* Math.sin(Math.toRadians(this.getPosition().getHeading()))
					/ METERS_PER_DEGREES
					/ Math.cos(Math.toRadians(this.getPosition().getLatitude()));
			i++;
			// Y
			lat += suite_type.getPoints().get(i) * 0.0254
					* Math.sin(Math.toRadians(this.getPosition().getHeading()))
					/ METERS_PER_DEGREES;
			lon -= suite_type.getPoints().get(i)
					* 0.0254
					* Math.cos(Math.toRadians(this.getPosition().getHeading()))
					/ METERS_PER_DEGREES
					/ Math.cos(Math.toRadians(this.getPosition().getLatitude()));
			i++;
			// Z
			double alt = this.getPosition().getAltitude()
					+ suite_type.getPoints().get(i) * 0.0254;

			if ((parent != null) && (parent.hasAltitudeAdjustment()))
				alt += parent.getPosition().getAltitude() + parent.getAltitudeAdjustment();
			i++;
			points.add(lat);
			points.add(lon);
			points.add(alt);
		}
	}

	public int getLevelNumber() {
		return level_number;
	}

	public String getFloorName() {
		return floor_name;
	}

	public String getName() {
		return name;
	}

	public Status getStatus() {
		return status;
	}

	public void setStatus(Status newStatrus) {
		status = newStatrus;
	}

	public Position getPosition() {
		return position;
	}

	public int getPrice() {
		return price;
	}

	public String getCurrency() {
		return currency;
	}

	// public static void test(String json) {
	// Log.write("Suite " + json);
	// }

	public int getId() {
		return id;
	}

	public int getParent_id() {
		return parent_id;
	}

	public SuiteType getSuiteType() {
		return suite_type;
	}

	public int getCeiling_height_ft() {
		return ceiling_height_ft;
	}

	public ArrayList<Double> getPoints() {
		return points;
	}

	public Building getParent() {
		return parent;
	}

	public void setVTourUrl(String vTourUrl) {
		this.vTourUrl = vTourUrl;
	}

	public String getVTourUrl() {
		return vTourUrl;
	}

	public String getFloorplan_url() {
		return floorplan_url;
	}

	@Override
	public void setInfoUrl(String infoUrl) {
		Log.write("Suite.setInfoUrl: " + infoUrl);
		this.infoUrl = infoUrl;
	}

	@Override
	public String getInfoUrl() {
		Log.write("Suite.getInfoUrl: " + infoUrl);
		return infoUrl;
	}

	@Override
	public VRObjectType getType() {
		return VRObjectType.Suite;
	}

}
