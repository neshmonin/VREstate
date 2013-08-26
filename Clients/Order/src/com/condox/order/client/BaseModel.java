package com.condox.order.client;

import java.util.HashMap;
import java.util.Map;

import com.condox.clientshared.abstractview.Log;

public abstract class BaseModel extends Node implements I_Node {

	abstract ModelType getModelType();

	private Map<String, String> data = new HashMap<String, String>();

	public String getValue(ModelType type, String key) {
		if (type != null)
			if (type.equals(getModelType()))
				return data.get(key);
		return null;
	}

	public void setValue(String key, String value) {
		data.put(key, value);
	}
	
	public void log() {
		Log.write("" + getValue(getModelType(), "value"));
		if (getPrev() != null)
			getPrev().log();
	}
}
