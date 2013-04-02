package com.condox.vrestate.client.document;

//import java.lang.annotation.ElementType;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.condox.vrestate.client.Position;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class Site implements I_VRObject {

	public Position getPosition() {
		return position;
	}

	private static Map<Integer, Site> ids = new HashMap<Integer, Site>();

	private int id = -1;
	// private version
	private int parent_id = -1;
	private String name = "";
	private String displayModelUrl = "";
	private String infoUrl = null;
//	private String moreInfoUrl = null;
	private Position position = new Position();
	private String bubbleTemplateUrl = new String();

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

		position.setLongitude(lon);
		position.setLatitude(lat);
		position.setAltitude(alt);
		position.setHeading(heading);

		if (obj.get("displayModelUrl") != null)
		displayModelUrl = obj.get("displayModelUrl").isString()
				.stringValue();
		
		if (obj.get("bubbleTemplateUrl") != null)
			if (obj.get("bubbleTemplateUrl").isString() != null)
				bubbleTemplateUrl = obj.get("bubbleTemplateUrl").isString()
			.stringValue();
		// KIOSK
//		bubbleTemplateUrl = "template(musee).html";
//		if (bubbleTemplateUrl.isEmpty())
//			bubbleTemplateUrl = "template.html";
//		
//		if (!bubbleTemplateUrl.isEmpty()) {
//			Window.alert("Using " + bubbleTemplateUrl);
//			Options.SUITE_INFO.setSrc(bubbleTemplateUrl);
//		}
	}

	public String getName() {
		return name;
	}

	public String getDisplayModelUrl() {
		return displayModelUrl;
	}
	
	public String getBubbleTemplateUrl() {
		return bubbleTemplateUrl;
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

	@Override
	public VRObjectType getType() {
		return VRObjectType.Site;
	}

	@Override
	public void setInfoUrl(String infoUrl) {
		this.infoUrl = infoUrl;
	}

	@Override
	public String getInfoUrl() {
		return infoUrl;
	}

}
