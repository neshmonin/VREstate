package com.condox.clientshared.document;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.condox.clientshared.communication.Options;
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
	private String street = "";
	private String city = "";
	private String postal = "";
	private Position center = null;
	private Position position = null;
	private Double altitude_adjustment = null;
	private String displayModelUrl = "";
	private String poiModelUrl = "";
	private String overlayModelUrl = "";
	private String infoUrl = null;
	// ---------------------------------------------
	private String bubbleWebTemplateUrl = null;
	private String bubbleKioskTemplateUrl = null;

	// ---------------------------------------------

	public void Parse(JSONValue value) {
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		parent_id = (int) obj.get("siteId").isNumber().doubleValue();
		name = obj.get("name").isString().stringValue();

		if (obj.containsKey("center")) {
			JSONObject center = obj.get("center").isObject();
			double longitude = center.get("lon").isNumber().doubleValue();
			double latitude = center.get("lat").isNumber().doubleValue();
			double altitude = center.get("alt").isNumber().doubleValue();
			double heading = 0;

			this.center = new Position();
			this.center.setLongitude(longitude);
			this.center.setLatitude(latitude);
			this.center.setAltitude(altitude);
			this.center.setHeading(heading);
		}
		if (obj.containsKey("position")) {
			JSONObject position = obj.get("position").isObject();
			double longitude = position.get("lon").isNumber().doubleValue();
			double latitude = position.get("lat").isNumber().doubleValue();
			double altitude = position.get("alt").isNumber().doubleValue();
			double heading = 0;

			this.position = new Position();
			this.position.setLongitude(longitude);
			this.position.setLatitude(latitude);
			this.position.setAltitude(altitude);
			this.position.setHeading(heading);
		}
		max_suite_altitude = obj.get("maxSuiteAltitude").isNumber()
				.doubleValue();
		address = obj.get("address").isString().stringValue();
		for (int i = 0; i < 3; i++)
			if (address.contains(","))
				address = address.substring(0, address.lastIndexOf(","));

		if (obj.containsKey("addressLine1"))
			street = obj.get("addressLine1").isString().stringValue();
		
		if (obj.containsKey("city"))
			city = obj.get("city").isString().stringValue();
		
		if (obj.containsKey("postalCode"))
			postal = obj.get("postalCode").isString().stringValue();
		
		
		if (obj.containsKey("displayModelUrl"))
			displayModelUrl = obj.get("displayModelUrl").isString()
					.stringValue();

		if (obj.containsKey("poiModelUrl"))
			poiModelUrl = obj.get("poiModelUrl").isString().stringValue();

		if (obj.containsKey("overlayModelUrl"))
			overlayModelUrl = obj.get("overlayModelUrl").isString()
					.stringValue();

		if (obj.containsKey("altitudeAdjustment"))
			altitude_adjustment = obj.get("altitudeAdjustment").isNumber()
					.doubleValue();

		if (obj.containsKey("bubbleTemplateUrl"))
			infoUrl = obj.get("bubbleTemplateUrl").isString().stringValue();

		// ---------------------------------------------
		if (obj.containsKey("bubbleWebTemplateUrl"))
			bubbleWebTemplateUrl = obj.get("bubbleWebTemplateUrl").isString()
					.stringValue();
		if (obj.containsKey("bubbleKioskTemplateUrl"))
			bubbleKioskTemplateUrl = obj.get("bubbleKioskTemplateUrl")
					.isString().stringValue();
		// ---------------------------------------------

		// Rest of the code is for backward compatibility
		if (obj.containsKey("bubbleTemplateUrl"))
			bubbleKioskTemplateUrl = obj.get("bubbleTemplateUrl").isString()
					.stringValue();

		if (bubbleWebTemplateUrl != null) {
			if (bubbleWebTemplateUrl.startsWith(Options.URL_MODEL))
				bubbleWebTemplateUrl = bubbleWebTemplateUrl
						.substring(Options.URL_MODEL.length());
			else if (bubbleWebTemplateUrl.startsWith(Options.URL_VRT))
				bubbleWebTemplateUrl = bubbleWebTemplateUrl
						.substring(Options.URL_VRT.length());
			else if (bubbleWebTemplateUrl.startsWith(Options.URL_STATIC))
				bubbleWebTemplateUrl = bubbleWebTemplateUrl
						.substring(Options.URL_STATIC.length());
		}

		if (bubbleKioskTemplateUrl != null) {
			if (bubbleKioskTemplateUrl.startsWith(Options.URL_MODEL))
				bubbleKioskTemplateUrl = bubbleKioskTemplateUrl
						.substring(Options.URL_MODEL.length());
			else if (bubbleKioskTemplateUrl.startsWith(Options.URL_VRT))
				bubbleKioskTemplateUrl = bubbleKioskTemplateUrl
						.substring(Options.URL_VRT.length());
			else if (bubbleKioskTemplateUrl.startsWith(Options.URL_STATIC))
				bubbleKioskTemplateUrl = bubbleKioskTemplateUrl
						.substring(Options.URL_STATIC.length());
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

	public Site getParent() {
		return Document.get().getSites().get(getParent_id());
	}

	public static ArrayList<Building> get() {
		ArrayList<Building> result = new ArrayList<Building>();
		result.addAll(ids.values());
		return result;
	}

	// public static void Draw(View view) {
	// for (Building building : get())
	// view.Draw(building);
	// }
	//
	// public static Building get(int id) {
	// return ids.get(id);
	// }
	//
	// @Override
	// public void Select() {
	// selection.clear();
	// selection.add(this);
	// Site site = Site.get(this.getParent_id());
	// selection.add(site);
	// Draw();
	// }

	public Position getCenter() {
		return center;
	}

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

	@Override
	public void setInfoUrl(String infoUrl) {
		this.infoUrl = infoUrl;
	}

	@Override
	public String getInfoUrl() {
		return infoUrl;
	}

	public String getDisplayModelUrl() {
		return displayModelUrl;
	}

	public String getPOIUrl() {
		return poiModelUrl;
	}

	public String getOverlayUrl() {
		return overlayModelUrl;
	}

	// --
	public String getBubbleWebTemplateUrl() {
		if (bubbleWebTemplateUrl == null || bubbleWebTemplateUrl.isEmpty())
			return null;
		return bubbleWebTemplateUrl;
	}

	public String getBubbleKioskTemplateUrl() {
		if (bubbleKioskTemplateUrl == null || bubbleKioskTemplateUrl.isEmpty())
			return infoUrl;
		return bubbleKioskTemplateUrl;
	}
	// --
}
