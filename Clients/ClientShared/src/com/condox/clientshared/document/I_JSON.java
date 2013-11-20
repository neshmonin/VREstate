package com.condox.clientshared.document;

import com.google.gwt.json.client.JSONObject;

public interface I_JSON {
	JSONObject toJSONObject();
	void fromJSONObject(JSONObject json);
}
