package com.condox.vrestate.shared;

import java.util.HashMap;
import java.util.Map;

import com.google.gwt.http.client.Response;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;
import com.google.gwt.json.client.JSONValue;

public class Document implements IDocument, I_Progress 
{

	private static Document instance = new Document();
	public static I_Progress progressBar = null;
	public static I_AbstractView abstractView = null;

	private Document() {
	};

	public static IDocument get() {
		return instance;
	}

	private Map<Integer, Site> sites = new HashMap<Integer, Site>();
	private Map<Integer, Building> buildings = new HashMap<Integer, Building>();
	private Map<Integer, Suite> suites = new HashMap<Integer, Suite>();
	private Map<Integer, SuiteType> suite_types = new HashMap<Integer, SuiteType>();
	private Map<String, ViewOrder> viewOrders = new HashMap<String, ViewOrder>();

	public static ViewOrder targetViewOrder = null;

	@Override
	public boolean Parse(String json) {
		// Window.alert("Document start parsing");
		if (json.equals("")) {
			SetupProgress(I_Progress.ProgressType.Error);
			// Window.alert("Document end parsing");
			return false;
		} else {
			SetupProgress(I_Progress.ProgressType.Loading);
			UpdateProgress(0.0);
			ParseSuiteTypes(json);
			UpdateProgress(10.0);
			ParseBuildings(json);
			UpdateProgress(70.0);
			ParseSuites(json, false);
			UpdateProgress(80.0);
			ParseSites(json);
			UpdateProgress(90.0);
			ParseViewOrders(json);
			UpdateProgress(100.0);

			/*---------- JSON-Target Object -----------
			  "primaryViewOrderId": "edeaa071-9f22-4cc8-ada8-db9568e40e09", 
			  "initialView": "" 
			---------- JSON-Target Object -----------*/
			JSONObject obj = JSONParser.parseLenient(json).isObject();
			if (obj.get("primaryViewOrderId") != null) {
				JSONString jsonString = obj.get("primaryViewOrderId")
						.isString();
				String primaryViewOrderId = jsonString.stringValue();
				ViewOrder vo = viewOrders.get(primaryViewOrderId);
				if (vo != null)
					targetViewOrder = vo;
			}

			for (Suite suite : getSuites().values())
				suite.CalcLineCoords();

			CleanupProgress();
			// Window.alert("Document end parsing");

			return true;
		}
	}

	/*---------- JSON-SuiteType -----------
	{ 
	  "id": 2663, 
	  "version": [0, 0, 0, 0, 0, 21, 153, 225], 
	  "siteId": 21, 
	  "name": "MDR1.NT.NPH.N17-1Bax", 
	  "levels": [], 
	  "geometries": 
	  [
	    { 
	      "points": 
	      [
	        -141.30302649262981, 214.57383570114581, -4.5474735088646412E-13,
	         0, 0, 0, 
	        -141.30302966703451, 214.57383361071271, 118.110236220472, 
	        -2.1498145088116871E-05, 6.4849928094190554E-06, 118.1102362204722,
	        -25.605614068611771, -131.1999707624598, 118.1102362204722,
	        -25.60560036206255, -131.19998225627711, -2.2737367544323211E-13
	      ], 
	      "lines": [1, 0, 2, 0, 3, 2, 4, 3, 4, 5, 5, 1]
	    }
	  ], 
	  "area": 0, 
	  "areaUm": "sqFt", 
	  "bedrooms": 0, 
	  "dens": 0, 
	  "otherRooms": 1, 
	  "showerBathrooms": 0, 
	  "noShowerBathrooms": 0, 
	  "bathrooms": 0, 
	  "balconies": 0, 
	  "terraces": 0
	}
	---------- JSON-SuiteType -----------*/
	private void ParseSuiteTypes(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray JSONsuite_types = obj.get("suiteTypes").isArray();
		for (int index = 0; index < JSONsuite_types.size(); index++) {
			JSONObject JSONsuite_type = JSONsuite_types.get(index).isObject();
			int id = (int) JSONsuite_type.get("id").isNumber().doubleValue();
			SuiteType suite_type = this.suite_types.get(id);
			if (suite_type == null) {
				suite_type = new SuiteType();
				suite_type.Parse(JSONsuite_types.get(index));
				this.suite_types.put(suite_type.getId(), suite_type);
			}
		}
	}

	/*---------- JSON-Suite -----------
	{
	  "id": 7593, 
	  "version": [0, 0, 0, 0, 0, 21, 206, 10], 
	  "buildingId": 365, 
	  "levelNumber": -1, 
	  "floorName": "15", 
	  "name": "1507", 
	  "ceilingHeightFt": 9, 
	  "showPanoramicView": true, 
	  "status": "ResaleAvailable", 
	  "position": 
	  { 
	    "lon": -79.4835486612197, "lat": 43.619286911315818, "alt": 41.999999493262919, 
	    "hdg": 12.650000000000006, "vhdg": 0 
	  }, 
	  "suiteTypeId": 2663
	}
	---------- JSON-Suite -----------*/
	private Map<Integer, Suite> ParseSuites(String json, boolean redraw) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray JSONsuites = obj.get("suites").isArray();
		Map<Integer, Suite> changedSuites = new HashMap<Integer, Suite>();
		for (int index = 0; index < JSONsuites.size(); index++) {
			JSONObject JSONsuite = JSONsuites.get(index).isObject();
			int id = (int) JSONsuite.get("id").isNumber().doubleValue();

			Suite theSuite = this.suites.get(id);
			if (theSuite == null) {
				theSuite = new Suite();
				theSuite.Parse(JSONsuites.get(index));
				// ////////
				// if (!(theSuite.getName().contains("gf") ||
				// theSuite.getName().contains("ph") ||
				// theSuite.getName().contains("lph") ||
				// theSuite.getName().contains("10")))
				// continue;
				// ////////
				this.suites.put(theSuite.getId(), theSuite);

				changedSuites.put(theSuite.getId(), theSuite);
			} else {
				theSuite.ParseDynamic(JSONsuite);
				changedSuites.put(theSuite.getId(), theSuite);
			}
		}
		return changedSuites;
	}

	/*---------- JSON-Building -----------
	{ 
	  "id": 365, 
	  "version": [0, 0, 0, 0, 0, 17, 105, 137], 
	  "siteId": 21, 
	  "name": "Marina Del Rey-2", 
	  "status": "InProject", 
	  "openingDate": null, 
	  "altitudeAdjustment": 74.842002867821819, 
	  "maxSuiteAltitude": 47.99999943030582, 
	  "initialView": "", 
	  "position": 
	  { 
	    "lon": -79.483686816650689, "lat": 43.618763911927708, "alt": 8.3052178689808537E-10 
	  }, 
	  "center": 
	  { 
	    "lon": -79.483070225865276, "lat": 43.619017135713712, "alt": 18.317405925493432 
	  }, 
	  "addressLine1": "2261 Lake Shore Blvd W", 
	  "city": "Toronto", 
	  "stateProvince": "ON", 
	  "postalCode": "M8V3X1", 
	  "country": "Canada", 
	  "address": "2261 Lake Shore Blvd W, Toronto, ON, M8V3X1, Canada"
	}
	---------- JSON-Building -----------*/
	private void ParseBuildings(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray JSONbuildings = obj.get("buildings").isArray();
		for (int index = 0; index < JSONbuildings.size(); index++) {
			JSONObject JSONbuilding = JSONbuildings.get(index).isObject();
			int id = (int) JSONbuilding.get("id").isNumber().doubleValue();
			Building building = this.buildings.get(id);
			if (building == null) {
				building = new Building();
				building.Parse(JSONbuildings.get(index));
				this.buildings.put(building.getId(), building);
			}
		}
	}

	/*---------- JSON-Site -----------
	{ 
	  "id": 21, 
	  "version": [0, 0, 0, 0, 0, 17, 183, 45], 
	  "estateDeveloperId": 6, 
	  "name": "Mimico - Horst Richter", 
	  "initialView": ""
	}
	---------- JSON-Site -----------*/
	private void ParseSites(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray JSONsites = obj.get("sites").isArray();
		for (int index = 0; index < JSONsites.size(); index++) {
			JSONObject JSONsite = JSONsites.get(index).isObject();
			int id = (int) JSONsite.get("id").isNumber().doubleValue();
			Site site = this.sites.get(id);
			if (site == null) {
				site = new Site();
				site.Parse(JSONsites.get(index));
				this.sites.put(site.getId(), site);
			}
		}
	}

	/*---------- JSON-ViewOrder -----------
	{ 
	  "id": "ffda6616-daa1-4978-8dbf-4acd732de044", 
	  "targetObjectType": "Suite", 
	  "targetObjectId": 7678, 
	  "product": "PublicListing", 
	  "options": "FloorPlan", 
	  "infoUrl": "", 
	  "vTourUrl": null
	}, 
	---------- JSON-ViewOrder -----------*/
	private void ParseViewOrders(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray JSONviewOrders = obj.get("viewOrders").isArray();
		for (int index = 0; index < JSONviewOrders.size(); index++) {
			JSONObject JSONviewOrder = JSONviewOrders.get(index).isObject();
			String id = JSONviewOrder.get("id").isString().toString();
			ViewOrder viewOrder = this.viewOrders.get(id);
			if (viewOrder == null) {
				viewOrder = new ViewOrder();
				viewOrder.Parse(JSONviewOrders.get(index));
				this.viewOrders.put(viewOrder.getId(), viewOrder);
				switch (viewOrder.getTargetObjectType()) {
				case Suite:
					Suite targetSuite = this.suites.get(viewOrder
							.getTargetObjectId());
					viewOrder.setTargetObject(targetSuite);
					targetSuite.setInfoUrl(viewOrder.getInfoUrl());
					targetSuite.setVTourUrl(viewOrder.getVTourUrl());
					break;
				case Building:
					Building targetBuilding = this.buildings.get(viewOrder
							.getTargetObjectId());
					viewOrder.setTargetObject(targetBuilding);
					targetBuilding.setInfoUrl(viewOrder.getInfoUrl());
					break;
				case Site:
					Site targetSite = this.sites.get(viewOrder
							.getTargetObjectId());
					viewOrder.setTargetObject(targetSite);
					break;
				default:
					break;
				}
			}
		}
	}

	@Override
	public Map<Integer, Site> getSites() {
		return this.sites;
	}

	@Override
	public Map<Integer, Building> getBuildings() {
		return this.buildings;
	}

	@Override
	public Map<Integer, Suite> getSuites() {
		return this.suites;
	}

	@Override
	public Map<Integer, SuiteType> getSuiteTypes() {
		return this.suite_types;
	}

	@Override
	public Map<String, ViewOrder> getViewOrders() {
		return this.viewOrders;
	}

	@Override
	public Map<Integer, Suite> onCheckChanges(Response response) {
		String received = response.getText();
		if (received == null || received.length() == 0)
			return null;

		JSONValue JSONreceived = JSONParser.parseLenient(received);
		if (JSONreceived == null)
			return null;

		JSONObject obj = JSONreceived.isObject();
		JSONArray JSONsuites = obj.get("suites").isArray();
		if (JSONsuites.size() == 0)
			return null;
		
		SetupProgress(I_Progress.ProgressType.Loading);
		UpdateProgress(0.0);

		ParseSuiteTypes(received);
		UpdateProgress(70.0);
		Map<Integer, Suite> changedSuites = ParseSuites(received, true);
		
		CleanupProgress();
		
		return changedSuites;
	}

	@Override
	public void SetupProgress(ProgressType label) {
		if (progressBar != null)
			progressBar.SetupProgress(label);
	}

	@Override
	public void UpdateProgress(double percent) {
		if (progressBar != null)
			progressBar.UpdateProgress(percent);
	}

	@Override
	public void CleanupProgress() {
		if (progressBar != null)
			progressBar.CleanupProgress();
	}


}
