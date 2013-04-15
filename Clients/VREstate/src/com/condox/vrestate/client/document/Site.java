package com.condox.vrestate.client.document;

//import java.lang.annotation.ElementType;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.condox.vrestate.client.Options;
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
	// private String moreInfoUrl = null;
	private Position position = new Position();
	// private String bubbleTemplateUrl = new String();
	// --
	private String bubbleWebTemplateUrl = null;
	private String bubbleKioskTemplateUrl = null;

	// --

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

		// if (obj.get("bubbleTemplateUrl") != null)
		// if (obj.get("bubbleTemplateUrl").isString() != null)
		// bubbleTemplateUrl = obj.get("bubbleTemplateUrl").isString()
		// .stringValue();
		// KIOSK
		// bubbleTemplateUrl = "template(musee).html";
		// if (bubbleTemplateUrl.isEmpty())
		// bubbleTemplateUrl = "template.html";
		//
		// if (!bubbleTemplateUrl.isEmpty()) {
		// Window.alert("Using " + bubbleTemplateUrl);
		// Options.SUITE_INFO.setSrc(bubbleTemplateUrl);
		// }
		// --
		if (obj.containsKey("bubbleWebTemplateUrl"))
			bubbleWebTemplateUrl = obj.get("bubbleWebTemplateUrl").isString()
					.stringValue();
		if (obj.containsKey("bubbleKioskTemplateUrl"))
			bubbleKioskTemplateUrl = obj.get("bubbleKioskTemplateUrl")
					.isString().stringValue();

		// Rest of the code is for backward compatibility
		if (obj.containsKey("bubbleTemplateUrl"))
			bubbleKioskTemplateUrl = obj.get("bubbleTemplateUrl")
					.isString().stringValue();
		
		if (bubbleWebTemplateUrl != null) {
		    if (bubbleWebTemplateUrl.startsWith(Options.URL_MODEL))	
			    bubbleWebTemplateUrl = bubbleWebTemplateUrl.substring(Options.URL_MODEL.length());
		    else if (bubbleWebTemplateUrl.startsWith(Options.URL_VRT))	
			    bubbleWebTemplateUrl = bubbleWebTemplateUrl.substring(Options.URL_VRT.length());
		    else if (bubbleWebTemplateUrl.startsWith(Options.URL_STATIC))	
			    bubbleWebTemplateUrl = bubbleWebTemplateUrl.substring(Options.URL_STATIC.length());
		}

		if (bubbleKioskTemplateUrl != null) {
			if (bubbleKioskTemplateUrl.startsWith(Options.URL_MODEL))
			    bubbleKioskTemplateUrl = bubbleKioskTemplateUrl.substring(Options.URL_MODEL.length());
			else if (bubbleKioskTemplateUrl.startsWith(Options.URL_VRT))
			    bubbleKioskTemplateUrl = bubbleKioskTemplateUrl.substring(Options.URL_VRT.length());
			else if (bubbleKioskTemplateUrl.startsWith(Options.URL_STATIC))
			    bubbleKioskTemplateUrl = bubbleKioskTemplateUrl.substring(Options.URL_STATIC.length());
		}

		// --
	}

	public String getName() {
		return name;
	}

	public String getDisplayModelUrl() {
		return displayModelUrl;
	}

	// --
	public String getBubbleWebTemplateUrl() {
		if (bubbleWebTemplateUrl == null || bubbleWebTemplateUrl.isEmpty())
			return null;
		return bubbleWebTemplateUrl;
	}

	public String getBubbleKioskTemplateUrl() {
		if (bubbleKioskTemplateUrl == null || bubbleKioskTemplateUrl.isEmpty())
			return null;
		return bubbleKioskTemplateUrl;
	}

	// --

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
