package com.condox.orders.client.document;

import java.util.ArrayList;

import com.condox.orders.client.Log;
import com.condox.orders.client.Options;
import com.condox.orders.client.Position;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;

public class Suite implements I_VRObject {

	public enum Status {
		Sold, 
		Available, 
		OnHold, 
		ResaleAvailable,
		Selected,
		Layout		
	}
	
	private int id = -1;
	// private version
	private int parent_id = -1;
	private Building parent = null;
	//private int level_number = -1;
	public String floor_name = "";
	private String name = "";
	private int ceiling_height_ft = 0;
	//private boolean show_panoramic_view = false;
	private Status status = null;
	private Position position = new Position();
	private SuiteType suite_type = null;
	private int price = 0;
	private ArrayList<Double> points = new ArrayList<Double>();
	private String vTourUrl = null;
	private String infoUrl = null;
	public int bedrooms = -1;
	public double bathrooms = -1;
	
	public void Parse(JSONValue value) {
//		Log.write(value.toString());
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		// TODO version

//		parent_id = (int) obj.get("buildingId").isNumber().doubleValue();
		bedrooms = (int) obj.get("bedrooms").isNumber().doubleValue();
		bathrooms = obj.get("bathrooms").isNumber().doubleValue();
		//JSONNumber _level_number = obj.get("levelNumber").isNumber();
		//level_number = (int) _level_number.doubleValue();

		JSONString str = obj.get("floorName").isString();
		if (str != null)
			floor_name = str.stringValue();
		else
			Log.write("!floorname is null." + obj.toString());

		JSONString _name = obj.get("name").isString();
		name = _name.stringValue();

		if (obj.containsKey("ceilingHeightFt"))
			ceiling_height_ft = (int) obj.get("ceilingHeightFt")
					.isNumber().doubleValue();
		// TODO show_panoramic_view
	
		if (obj.get("status").isString() != null) {
			String status = obj.get("status").isString().stringValue();
			if (status.equals("Available")) this.status = Status.Available;
			else if (status.equals("ResaleAvailable")) this.status = Status.ResaleAvailable;
			else if (status.equals("Sold")) this.status = Status.Sold;
			else if (status.equals("OnHold")) this.status = Status.OnHold;
			else this.status = Status.Selected;
		} else
			this.status = Status.Selected;
		
		// TODO position		
		if (obj.containsKey("position")) {
			JSONObject position = obj.get("position").isObject();
			double longitude = position.get("lon").isNumber().doubleValue();
			double latitude = position.get("lat").isNumber().doubleValue();
			double altitude = position.get("alt").isNumber().doubleValue();
			double heading = position.get("hdg").isNumber().doubleValue();
			double tilt = 45;
			double range = 100;

			this.position.setLongitude(longitude);
			this.position.setLatitude(latitude);
			this.position.setAltitude(altitude);

			this.position.setHeading(heading);
			this.position.setTilt(tilt);
			this.position.setRange(range);
		}
		
//		for (Building item : Document.get().getBuildings())
//			if (item.getId() == getParent_id())
//				this.parent = item;

//		int type_id = (int) obj.get("suiteTypeId").isNumber()
//				.doubleValue();
//		Log.write("suiteTypeId:" + type_id);
//		suite_type = SuiteType.get(type_id);
//		Log.write("suite_type:" + suite_type.toString());
		
//		for (SuiteType type : Document.get().getSuiteTypes()) {
////			Log.write("" + type.getId());
//			if (type.getId() == type_id) {
////				Log.write("OK");
//				suite_type = type;
//				break;
//			}
//		}
//		if (suite_type == null)
//			Log.write("++");
//		if (suite_type == null)
//			Log.write("suiteTypeId:" + type_id);
		
		price = -1;
		if ((obj.get("currentPrice") != null)&&
			(obj.get("currentPrice").isNumber() != null) &&
			obj.get("currentPrice").isNumber().doubleValue() != 0)
			price = (int) obj.get("currentPrice").isNumber().doubleValue();
		
		if (Options.DEBUG_MODE)
		{
			 // Workaround for prices	
			 price = (int) (500 + 500 * Math.random());
			 price *= 1000;
		}
	}
	
	public void CalcLineCoords() {
		double METERS_PER_DEGREES = 111111;
		int i = 0;
		// TODO Всю эту возню с типами квартир надо будет переделать. Но не
		// сейчас.
		SuiteType suite_type = this.getSuiteType();
		// Log.write("SuiteType: " + suite_type);
		// Log.write("getPoints().size(): " +

		if (suite_type == null)
			return;
		
		while (i < suite_type.getPoints().size()) {
			// Log.write("i: " + i);
			// X
			double lat = this.getPosition().getLatitude()
					+ (suite_type.getPoints().get(i) + 10 / 2.54)
					* 0.0254
					* Math.cos(Math.toRadians(this.getPosition()
							.getHeading())) / METERS_PER_DEGREES;
			double lon = this.getPosition().getLongitude()
					+ (suite_type.getPoints().get(i) + 10 / 2.54)
					* 0.0254
					* Math.sin(Math.toRadians(this.getPosition()
							.getHeading()))
					/ METERS_PER_DEGREES
					/ Math.cos(Math.toRadians(this.getPosition()
							.getLatitude()));
			i++;
			// Y
			lat += suite_type.getPoints().get(i)
					* 0.0254
					* Math.sin(Math.toRadians(this.getPosition()
							.getHeading())) / METERS_PER_DEGREES;
			lon -= suite_type.getPoints().get(i)
					* 0.0254
					* Math.cos(Math.toRadians(this.getPosition()
							.getHeading()))
					/ METERS_PER_DEGREES
					/ Math.cos(Math.toRadians(this.getPosition()
							.getLatitude()));
			i++;
			// Z
			double alt = this.getPosition().getAltitude()
					+ suite_type.getPoints().get(i) * 0.0254;
			Building parent = null;
//			for (Building item : Document.get().getBuildings())
//				if (item.getId() == this.getParent_id())
//					parent = item;

			
			if ((parent != null) && (parent.hasAltitudeAdjustment()))
				alt += parent.getAltitudeAdjustment();
			i++;
			points.add(lat);
			points.add(lon);
			points.add(alt);
		}
	}

	public String getFloor_name() {
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

	private boolean updated = false;
	private boolean visibility = false;

	// public boolean isVisible(double heading) {
	// heading += 180;
	// if (heading > 360)
	// heading -= 360;
	// double diff = Math.abs(heading - position.getHeading());
	// boolean visible = (diff < 50) || (diff > 310);
	// if (visible != visibility)
	// updated = false;
	// visibility = visible;
	// return visibility;
	// }

	public boolean isUpdated() {
		return updated;
	}

	public int getPrice() {
		return price;
	}

	public boolean isVisible() {
		return visibility;
	}

	public void setVisible(boolean visible) {
		visibility = visible;
	}

//	public static void test(String json) {
//		Log.write("Suite " + json);
//	}

	public int getId() {
		return id;
	}

	public int getParent_id() {
		return parent_id;
	}

//	public static ArrayList<Suite> get() {
//		ArrayList<Suite> result = new ArrayList<Suite>();
//		result.addAll(ids.values());
//		return result;
//	}

//	public static void Draw(View view) {
//		for (Suite suite : get())
//			view.Draw(suite);
//	}
//
//	@Override
//	public void Select() {
//		selection.clear();
//		selection.add(this);
//		Building building = Building.get(getParent_id());
//		selection.add(building);
//		Site site = Site.get(building.getParent_id());
//		selection.add(site);
//		Draw();
//	}

//	public static Suite get(int id) {
//		return ids.get(id);
//	}

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

	@Override
	public void setInfoUrl(String infoUrl) {
		this.infoUrl = infoUrl;
	}

	@Override
	public String getInfoUrl() {
		return infoUrl;
	}

	@Override
	public VRObjectType getType() {
		return VRObjectType.Suite;
	}

	
}
