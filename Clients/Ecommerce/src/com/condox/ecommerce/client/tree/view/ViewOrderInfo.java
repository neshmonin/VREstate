package com.condox.ecommerce.client.tree.view;

import com.condox.clientshared.abstractview.Log;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;

public class ViewOrderInfo {
//	{"id":"c30232aa-0abe-43da-898c-00a04f440cb5",
//	 "version":[0,0,0,0,0,52,160,173],
//	 "ownerId":6,
//	 "expiresOn":"\/Date(1385892000000)\/",
//	 "enabled":true,
//	 "imported":false,
//	 "note":null,
//	 "product":"Building3DLayout",
//	 "options":"FloorPlan",
//	 "mlsId":null,
//	 "infoUrl":"\"http://www.michaelmarkovich.com/\"",
//	 "vTourUrl":null,
//	 "targetObjectType":"Building",
//	 "targetObjectId":368,
//	 "requestCounter":15,
//	 "lastRequestTime":"\/Date(1385441890000)\/",
//	 "viewOrder-url":"http://ref.3dcondox.com/go?id=VqjICw74K2kOJjACgT0QMtQ2",
//	 "label":"8 Park Rd, Toronto, ON, M4W3S5, Canada"}
	
	private String label = "";
	private String mls = "";
	
	public static ViewOrderInfo fromJSON(String json) {
		Log.write(json);
		ViewOrderInfo result = new ViewOrderInfo();
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		result.label = obj.get("label").isString().stringValue();
		
		// MLS#
		if (obj.get("mlsId") != null)
			if (obj.get("mlsId").isString() != null)
				result.mls = obj.get("mlsId").isString().stringValue();
		
		return result;
	}
	
	public String getMLS() {
		return mls;
	}
	
	public String getLabel() {
		return label;
	}

}