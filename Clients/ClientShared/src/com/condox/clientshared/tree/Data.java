package com.condox.clientshared.tree;

import com.condox.clientshared.document.I_JSON;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONValue;

public class Data {
	public enum DataType {
		Integer,
		Double,
		Boolean,
		String,
		JSON
	}
	
	public DataType Type;
	public String Value;

	public Data(Integer value) {
		Type = DataType.Integer;
		Value = value.toString();
	}
	
	public Data(Double value) {
		Type = DataType.Double;
		Value = value.toString();
	}
	
	public Data(Boolean value) {
		Type = DataType.Boolean;
		Value = value.toString();
	}
	
	public Data(String value) {
		Type = DataType.String;
		Value = value;
	}
	
	public Data(I_JSON object) {
		Type = DataType.JSON;
		JSONValue json = object.toJSONValue();
		Value = json.toString();
	}
	
	public int asInteger() {
		if (Type != DataType.Integer) return 0;

		return Integer.parseInt(Value);		
	}
	
	public double asDouble() {
		if (Type != DataType.Double) return 0.0;

		return Double.parseDouble(Value);		
	}
	
	public boolean asBoolean() {
		if (Type != DataType.Boolean) return false;

		return Boolean.parseBoolean(Value);		
	}
	
	public String asString() {
		if (Type != DataType.String) return null;

		return Value;
	}

	public JSONObject asJSONObject() {
		if (Type != DataType.JSON) return null;
		JSONObject obj = JSONParser.parseLenient(Value).isObject();
		return obj;
	}
}
