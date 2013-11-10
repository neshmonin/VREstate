package com.condox.clientshared.document;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class BuildingInfo implements I_JSON{
	private static Map<Integer, BuildingInfo> ids = new HashMap<Integer, BuildingInfo>();
	
	private int id = -1;
	// private version
	private int parent_id = -1;
	private String name = "";
	private double max_suite_altitude = 0;
	private String address = "";
	private String street = "";
	private String city = "";
	private String postal = "";
	private Position position = null;
	private Double altitude_adjustment = null;
	
	@Override
	public JSONValue toJSONValue() {
		return null;
	}

	@Override
	public void fromJSONValue(JSONValue value) {
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
		
		if (obj.containsKey("address")) {
			address = obj.get("address").isString().stringValue();
			for (int i = 0; i < 3; i++)
				if (address.contains(","))
					address = address.substring(0, address.lastIndexOf(","));
		}
		
		if (obj.containsKey("addressLine1"))
			street = obj.get("addressLine1").isString().stringValue();
		
		if (obj.containsKey("city"))
			city = obj.get("city").isString().stringValue();
		
		if (obj.containsKey("postalCode"))
			postal = obj.get("postalCode").isString().stringValue();
		
		if (obj.containsKey("altitudeAdjustment")) {
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
	
	public String getStreet() {
		return street;
	}
	public String getCity() {
		return city;
	}
	public String getPostal() {
		return postal;
	}
	

	public int getId() {
		return id;
	}

	public int getParent_id() {
		return parent_id;
	}
	
	public static ArrayList<BuildingInfo> get() {
		ArrayList<BuildingInfo> result = new ArrayList<BuildingInfo>();
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

}
