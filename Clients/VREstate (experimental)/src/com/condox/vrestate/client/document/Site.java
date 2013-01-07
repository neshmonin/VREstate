package com.condox.vrestate.client.document;

import java.lang.annotation.ElementType;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.condox.vrestate.client.Position;
import com.condox.vrestate.client.view.View;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class Site {

	public Position getPosition() {
		return position;
	}

	private static Map<Integer, Site> ids = new HashMap<Integer, Site>();

	private int id = -1;
	// private version
	private int parent_id = -1;
	private String name = "";
	private String displayModelUrl = "";

	private Position position = new Position();

	// private double max_suite_altitude = 0;

	void Parse(JSONValue value) {
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		// TODO version
		// parent_id = (int) obj.get("siteId").isNumber().doubleValue();
		name = obj.get("name").isString().stringValue();

		// Calculate position

		double lat = 0;
		double lon = 0;
		double alt = 0;
		int count = 0;

		for (Building building : Document.get().getBuildings()) {
			Position position = building.getPosition();
			lat += position.getLatitude();
			lon += position.getLongitude();
			alt += position.getAltitude();
			count++;
		}

		lat /= count;
		lon /= count;
		alt /= count;
		double heading = 0;
		double tilt = 45;
		double range = 1000;

		position .setLongitude(lon);
		position.setLatitude(lat);
		position.setAltitude(alt);
		position.setHeading(heading);
		position.setTilt(tilt);
		position.setRange(range);
		if (obj.get("displayModelUrl") != null)
		displayModelUrl = obj.get("displayModelUrl").isString()
				.stringValue();
	}

	public String getName() {
		return name;
	}

	public String getDisplayModelUrl() {
		return displayModelUrl;
	}

	public int getId() {
		return id;
	}

	public int getParent_id() {
		return parent_id;
	}
	
	public static ArrayList<Site> get() {
		ArrayList<Site> result = new ArrayList<Site>();
		result.addAll(ids.values());
		return result;
	}
	
	public static Site get(int id) {
		return ids.get(id);
	}
}
