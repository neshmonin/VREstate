package com.condox.vrestate.client.document;

import java.lang.annotation.ElementType;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.condox.vrestate.client.JSONParams;
import com.google.gwt.core.client.JsArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONValue;

public class SuiteType {
	//====================================
	private static Map<Integer, SuiteType> ids = new HashMap<Integer, SuiteType>();
	
//	static void Parse(String json) {
//		ids.clear();
//		JSONObject obj = JSONParser.parseLenient(json).isObject();
//		JSONArray types = obj.get("suiteTypes").isArray();
//		for (int index = 0; index < types.size(); index++) {
//			SuiteType type = Create(types.get(index).isObject());
//			if (type != null)
//				ids.put(type.getId(), type);
//		}
//	}
	
	private int id = -1;
	// private version
	private int parent_id = -1;
	private String name = "";
	private ArrayList<Double> points = new ArrayList<Double>();
	
	private int bedrooms = 0;
	private int bathrooms = 0;
	private int balconies = 0;
	private double area = 0;
	private String floorPlanUrl = null;
	
	void Parse(JSONValue value) {
//		Log.write(value.toString());
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		parent_id = (int) obj.get("siteId").isNumber().doubleValue();
		name = obj.get("name").isString().stringValue();
		JSONParams params = JSONParams.parse(obj.toString());
		JsArray<JSONParams> geometries = params.getArray("geometries");
		if (geometries != null)
			for (int i = 0; i < geometries.length(); i++) {
				JSONParams points = geometries.get(i).getParams("points");
				JSONParams lines = geometries.get(i).getParams("lines");
				for (int index = 0; index < lines.getLength(); index++) {
					int point_index = (int) lines.getDouble(index);
					this.points.add(points.getDouble(point_index * 3));
					this.points.add(points.getDouble(point_index * 3 + 1));
					this.points.add(points.getDouble(point_index * 3 + 2));
				}
			}
		bedrooms = params.getInteger("bedrooms");
		bathrooms = params.getInteger("bathrooms");
		balconies = params.getInteger("balconies");
		area = params.getDouble("area");
		floorPlanUrl = params.getString("floorPlanUrl");
	}
	//====================================

	public ArrayList<Double> getPoints() {
		return points;
	}

	public int getBedrooms() {
		return bedrooms;
	}

	public int getBathrooms() {
		return bathrooms;
	}

	public double getArea() {
		return area;
	}

	public int getBalconies() {
		return balconies;
	}

	public int getId() {
		return id;
	}

	public int getParent_id() {
		return parent_id;
	}
	
	public static SuiteType get(int id) {
		return ids.get(id);
	}

	public String getFloorPlanUrl() {
//		return null;
		return floorPlanUrl;
	}

	
	
	
	
	
}
