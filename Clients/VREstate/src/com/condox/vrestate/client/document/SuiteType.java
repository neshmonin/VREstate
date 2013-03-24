package com.condox.vrestate.client.document;

//import java.lang.annotation.ElementType;
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
	//private String name = "";
	private ArrayList<Double> points = new ArrayList<Double>();
	
	private int bedrooms = 0;
	private int dens = 0;
	private int otherRooms = 0;
	private double bathrooms = 0;
	private int balconies = 0;
	private int terraces = 0;
	private double area = 0;
	private String roomsStr = "";
	private String floorPlanUrl = null;

	void Parse(JSONValue value) {
		/*=======================================================
		{
			"id":1393,"version":[0,0,0,0,0,1,236,30],
			"siteId":15,
			"name":"25Esplanade/D",
			"floorPlanUrl":"https://vrt.3dcondox.com/models/1393_fp.jpg",
			"levels":[],
			"geometries":
			[
				{
					"points":
					[
						0,-128.06228579081539,0,
						2.2737367544323211E-13,88.47314728005108,0,
						2.2737367544323211E-13,-128.06228579081559,118.1102362204724,
						0,88.47314728005108,118.1102362204724
					],
					"lines":[1,0,0,2,3,2,3,1]
				}
			],
			"area":641,
			"areaUm":"sqFt",
			"bedrooms":0,
			"dens":1,
			"bathrooms":1,
			"balconies":0,
			"terraces":0
		}
		===========================================================*/
		JSONObject obj = value.isObject();
		id = (int) obj.get("id").isNumber().doubleValue();
		parent_id = (int) obj.get("siteId").isNumber().doubleValue();
		//name = obj.get("name").isString().stringValue();
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
		dens = params.getInteger("dens");
		//otherRooms = params.getInteger("otherRooms");
		roomsStr += bedrooms == 0? "St" : bedrooms;
		if (dens == 1)
			roomsStr += "+D";
		else if  (dens > 1)
			roomsStr += "+" + dens + "D";
			
		bathrooms = params.getDouble("bathrooms").doubleValue();
		balconies = params.getInteger("balconies");
		terraces = params.getInteger("terraces");
		area = params.getDouble("area");
		area = (area > 100)? area : 100;
		
		floorPlanUrl = params.getString("floorPlanUrl");
	}
	//====================================

	public ArrayList<Double> getPoints() {
		return points;
	}

	public int getBedrooms() {
		return bedrooms;
	}

	public int getOtherRooms() {
		return otherRooms;
	}

	public int getDens() {
		return dens;
	}

	public double getBathrooms() {
		return bathrooms;
	}
	
	public double getArea() {
		return area;
	}

	public String getRoomsStr() {
		return roomsStr;
	}

	public int getBalconies() {
		return balconies;
	}

	public int getTerraces() {
		return terraces;
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
		return floorPlanUrl;
	}

	
	
	
	
	
}
