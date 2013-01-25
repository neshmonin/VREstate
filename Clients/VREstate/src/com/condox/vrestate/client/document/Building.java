package com.condox.vrestate.client.document;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import com.condox.vrestate.client.Position;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class Building implements I_VRObject {

	private static Map<Integer, Building> ids = new HashMap<Integer, Building>();
	
	private int id = -1;
	// private version
	private int parent_id = -1;
	private String name = "";
	private double max_suite_altitude = 0;
	private String address = "";
	private Position position = null;
	private Double altitude_adjustment = null;
	
	void Parse(JSONValue value) {
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		// TODO version
		parent_id = (int) obj.get("siteId").isNumber().doubleValue();
		name = obj.get("name").isString().stringValue();
		
		if (obj.containsKey("center")) {	// ?? TODO 
			JSONObject position = obj.get("center").isObject();
			double longitude = position.get("lon").isNumber().doubleValue();
			double latitude = position.get("lat").isNumber().doubleValue();
			double altitude = position.get("alt").isNumber().doubleValue();
			double heading = 0;
			double tilt = 45;
			double range = 200;
			this.position = new Position();
			this.position.setLongitude(longitude);
			this.position.setLatitude(latitude);
			this.position.setAltitude(altitude);
			this.position.setHeading(heading);
			this.position.setTilt(tilt);
			this.position.setRange(range);
		}
		max_suite_altitude = obj.get("maxSuiteAltitude").isNumber().doubleValue();
		address = obj.get("address").isString().stringValue();
		for (int i = 0; i < 3; i++)
			if (address.contains(","))
				address = address.substring(0, address.lastIndexOf(","));
		
		if (obj.containsKey("AltitudeAdjustment")) {
			altitude_adjustment = obj.get("altitudeAdjustment").isNumber().doubleValue();
		}
			
	}
	
	public boolean hasAltitudeAdjustment() {
		return altitude_adjustment != null;
	}
	
	public double getAltitudeAdjustment() {
		return altitude_adjustment;
	}

	public String getName() {
		return name;
	}

	public double getMax_suite_altitude() {
		return max_suite_altitude;
	}

	public String getAddress() {
		return address;
	}

	public int getId() {
		return id;
	}

	public int getParent_id() {
		return parent_id;
	}
	
	public static ArrayList<Building> get() {
		ArrayList<Building> result = new ArrayList<Building>();
		result.addAll(ids.values());
		return result;
	}
	
//	public static void Draw(View view) {
//		for (Building building : get())
//			view.Draw(building);
//	}
//	
//	public static Building get(int id) {
//		return ids.get(id);
//	}
//
//	@Override
//	public void Select() {
//		selection.clear();
//		selection.add(this);
//		Site site = Site.get(this.getParent_id());
//		selection.add(site);
//		Draw();
//	}

	public Position getPosition() {
		return position;
	}
	
	private Object extended_data = null;

	public Object getExtendedData() {
		return extended_data;
	}

	public void setExtendedData(Object extended_data) {
		this.extended_data = extended_data;
	}

	@Override
	public VRObjectType getType() {
		return VRObjectType.Building;
	}
	
	
	
	

}
