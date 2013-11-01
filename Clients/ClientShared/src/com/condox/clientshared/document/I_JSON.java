package com.condox.clientshared.document;

import com.google.gwt.json.client.JSONValue;

public interface I_JSON {
	JSONValue toJSONValue();
	void fromJSONValue(JSONValue json);
}
