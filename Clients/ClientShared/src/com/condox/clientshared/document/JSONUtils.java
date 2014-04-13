package com.condox.clientshared.document;

import com.google.gwt.json.client.JSONObject;

public class JSONUtils {
	
	public static String getString(JSONObject obj, String key) {
		if (obj.get(key) != null)
			if (obj.get(key).isString() != null)
				return obj.get(key).isString().stringValue();
		return null;
	}
}
