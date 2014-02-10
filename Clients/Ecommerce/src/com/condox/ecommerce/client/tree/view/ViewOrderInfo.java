package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.document.I_JSON;
import com.google.gwt.json.client.JSONBoolean;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;

public class ViewOrderInfo implements I_JSON {
	
	// {"id":"c30232aa-0abe-43da-898c-00a04f440cb5",
	// "version":[0,0,0,0,0,52,160,173],
	// "ownerId":6,
	// "expiresOn":"\/Date(1385892000000)\/",
	// "enabled":true,
	// "imported":false,
	// "note":null,
	// "product":"Building3DLayout",
	// "options":"FloorPlan",
	// "mlsId":null,
	// "infoUrl":"\"http://www.michaelmarkovich.com/\"",
	// "vTourUrl":null,
	// "targetObjectType":"Building",
	// "targetObjectId":368,
	// "requestCounter":15,
	// "lastRequestTime":"\/Date(1385441890000)\/",
	// "viewOrder-url":"http://ref.3dcondox.com/go?id=VqjICw74K2kOJjACgT0QMtQ2",
	// "label":"8 Park Rd, Toronto, ON, M4W3S5, Canada"}

	private String id = null;
	private boolean enabled = true;
	private String label = "";
	private String mls = "";
	private String url = "";
	private String mi_URL = "";
	private String vt_URL = "";
	private int targetObjectId = -1;

	private JSONObject obj = null;

	public static ViewOrderInfo fromJSON(String json) {
		Log.write(json);
		ViewOrderInfo result = new ViewOrderInfo();
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		result.obj = obj;
		// Id
		if (obj.containsKey("id"))
			if (obj.get("id").isString() != null)
				result.id = obj.get("id").isString().stringValue()
						.replace("-", "");

		// Enabled
		if (obj.containsKey("enabled"))
			if (obj.get("enabled").isBoolean() != null)
				result.enabled = obj.get("enabled").isBoolean().booleanValue();

		result.label = obj.get("label").isString().stringValue();

		// MLS#
		if (obj.get("mlsId") != null)
			if (obj.get("mlsId").isString() != null)
				result.mls = obj.get("mlsId").isString().stringValue();

		// Url
		if (obj.get("viewOrder-url") != null)
			if (obj.get("viewOrder-url").isString() != null)
				result.url = obj.get("viewOrder-url").isString().stringValue();

		//
		if (obj.get("infoUrl") != null)
			if (obj.get("infoUrl").isString() != null)
				result.mi_URL = obj.get("infoUrl").isString().stringValue();

		//
		if (obj.get("vTourUrl") != null)
			if (obj.get("vTourUrl").isString() != null)
				result.vt_URL = obj.get("vTourUrl").isString().stringValue();

		return result;
	}

	public String getId() {
		return id;
	}

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean value) {
		obj.put("enabled", JSONBoolean.getInstance(value));
	}

	public String getMLS() {
		return mls;
	}

	public String getUrl() {
		return url;
	}

	public String getMoreInfoUrl() {
		return mi_URL;
	}

	public String getVirtualTourUrl() {
		return vt_URL;
	}

	public String getLabel() {
		return label;
	}

	public String getJSON() {
		return obj.toString();
	}

	@Override
	public JSONObject toJSONObject() {
		// TODO Auto-generated method stub
		return null;
	}

	public int getTargetObjectId() {
		return targetObjectId;
	}

	public void setTargetObjectId(int newId) {
		this.targetObjectId = newId;
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		targetObjectId = getInteger(obj, "targetObjectId");
	}
	
	// Utils
	
	private int getInteger(JSONObject obj, String key) {
		if (obj.get(key) != null)
			if (obj.get(key).isNumber() != null)
				if (obj.get(key).isNumber().doubleValue() > 0)
					return (int) obj.get(key).isNumber().doubleValue();
		return -1;
	}

}