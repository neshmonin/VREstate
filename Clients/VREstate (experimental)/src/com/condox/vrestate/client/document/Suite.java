package com.condox.vrestate.client.document;

import java.lang.annotation.ElementType;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.Position;
import com.google.gwt.json.client.JSONNumber;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;

public class Suite {

	private int id = -1;
	// private version
	private int parent_id = -1;
	private int level_number = -1;
	private String floor_name = "";
	private String name = "";
	private int ceiling_height_ft = 0;
	private boolean show_panoramic_view = false;
	private SuiteStatus status = null;
	private Position position = new Position();
	private SuiteType suite_type = null;
	private int price = 0;

	void Parse(JSONValue value) {
//		Log.write(value.toString());
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		// TODO version

		parent_id = (int) obj.get("buildingId").isNumber().doubleValue();
		JSONNumber _level_number = obj.get("levelNumber").isNumber();
		level_number = (int) _level_number.doubleValue();

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
		// TODO status
		if (obj.get("status").isString() != null) {
			String status = obj.get("status").isString().stringValue();
			this.status = (status.equals("Available")) ? SuiteStatus.STATUS_AVAILABLE
					: this.status;
			this.status = (status.equals("ResaleAvailable")) ? SuiteStatus.STATUS_RESALE_AVAILABLE
					: this.status;
			this.status = (status.equals("Sold")) ? SuiteStatus.STATUS_SOLD
					: this.status;
			this.status = (status.equals("OnHold")) ? SuiteStatus.STATUS_ON_HOLD
					: this.status;
			
		} else
//			Log.write("Error: status == null at the " + obj.toString());
		this.status = SuiteStatus.STATUS_AVAILABLE;
		
		// TODO position
		if (obj.containsKey("position")) {
			JSONObject position = obj.get("position").isObject();
			double longitude = position.get("lon").isNumber().doubleValue();
			double latitude = position.get("lat").isNumber().doubleValue();
			double altitude = position.get("alt").isNumber().doubleValue();
			double heading = position.get("hdg").isNumber().doubleValue();
			double tilt = 45;
			double range = 100;
//			suite.position = new Position();
			this.position.setLongitude(longitude);
			this.position.setLatitude(latitude);
			this.position.setAltitude(altitude);
//			Log.write("alt: " + this.position.getAltitude());
			this.position.setHeading(heading);
			this.position.setTilt(tilt);
			this.position.setRange(range);
		}

		int type_id = (int) obj.get("suiteTypeId").isNumber()
				.doubleValue();
		for (SuiteType type : Document.get().getSuiteTypes())
			if (type.getId() == type_id) {
				suite_type = type;
				break;
			}
		// TODO Сделать нормальную обработку цен
		// price = params.getInteger("currentPrice");
		price = (int) (500 + 500 * Math.random());
		price *= 1000;
	}

	public String getFloor_name() {
		return floor_name;
	}

	public String getName() {
		return name;
	}

	public SuiteStatus getStatus() {
		return status;
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
	
	private Object extended_data = null;

	public Object getExtendedData() {
		return extended_data;
	}

	public void setExtendedData(Object extended_data) {
		this.extended_data = extended_data;
	}
	
	
}
