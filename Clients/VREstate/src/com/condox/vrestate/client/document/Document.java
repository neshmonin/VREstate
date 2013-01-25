package com.condox.vrestate.client.document;

import java.util.Collection;
import java.util.HashMap;
import java.util.Map;
import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.document.Suite.Status;
import com.condox.vrestate.client.view.ProgressBar;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.json.client.JSONString;

public class Document implements IDocument {

	private static IDocument instance = new Document();
	public static ProgressBar progressBar;

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
		progressBar = new ProgressBar();
		if (json.equals("")) {
			progressBar.Update(-1.0);
			progressBar.Update(ProgressBar.ProgressLabel.Error);
			return false;
		} else {
			progressBar.Update(ProgressBar.ProgressLabel.Loading);
			progressBar.Update(0.0);
			ParseSuiteTypes(json);
			progressBar.Update(10.0);
			ParseBuildings(json);
			progressBar.Update(70.0);
			ParseSuites(json);
			progressBar.Update(80.0);
			ParseSites(json);
			progressBar.Update(90.0);
			ParseViewOrders(json);
			progressBar.Update(100.0);

			/*---------- JSON-Target Object -----------
			  "primaryViewOrderId": "edeaa071-9f22-4cc8-ada8-db9568e40e09", 
			  "initialView": "" 
			---------- JSON-Target Object -----------*/
			JSONObject obj = JSONParser.parseLenient(json).isObject();
			if (obj.get("primaryViewOrderId") != null) {
				JSONString jsonString = obj.get("primaryViewOrderId").isString();
				String primaryViewOrderId = jsonString.stringValue();
				ViewOrder vo = viewOrders.get(primaryViewOrderId);
				if (vo != null) {
					targetViewOrder = vo;
					if (vo.getProductType() == ViewOrder.ProductType.PrivateListing ||
						vo.getProductType() == ViewOrder.ProductType.PublicListing) {
						Suite targetSuite = (Suite) vo.getTargetObject();
						targetSuite.setStatus(Suite.Status.Selected);

						String vTourUrl =  vo.getVTourUrl();
						if (vTourUrl != null)
							targetSuite.setVTourUrl(vTourUrl);
					}
					else if (vo.getProductType() == ViewOrder.ProductType.Building3DLayout){
						for (Suite suite : this.suites.values())
							suite.setStatus(Status.Layout);
					}
						
				}
			}		

			for (Suite suite : getSuites())
				suite.CalcLineCoords();

			progressBar.Cleanup();
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
		Log.write(json);
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray suite_types = obj.get("suiteTypes").isArray();
		for (int index = 0; index < suite_types.size(); index++) {
			SuiteType suite_type = new SuiteType();
			suite_type.Parse(suite_types.get(index));
			this.suite_types.put(suite_type.getId(), suite_type);
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
	private void ParseSuites(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray suites = obj.get("suites").isArray();
		for (int index = 0; index < suites.size()/*10*/; index++) {
			Suite suite = new Suite();
			suite.Parse(suites.get(index));
			this.suites.put(suite.getId(), suite);
		}
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
		JSONArray buildings = obj.get("buildings").isArray();
		for (int index = 0; index < buildings.size(); index++) {
			Building building = new Building();
			building.Parse(buildings.get(index));
			this.buildings.put(building.getId(), building);
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
		JSONArray sites = obj.get("sites").isArray();
		for (int index = 0; index < sites.size(); index++) {
			Site site = new Site();
			site.Parse(sites.get(index));
			this.sites.put(site.getId(), site);
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
		JSONArray viewOrders = obj.get("viewOrders").isArray();
		for (int index = 0; index < viewOrders.size(); index++) {
			ViewOrder viewOrder = new ViewOrder();
			viewOrder.Parse(viewOrders.get(index));
			this.viewOrders.put(viewOrder.getId(), viewOrder);
			switch (viewOrder.getTargetObjectType())
			{
			case Suite:
				Suite targetSuite = this.suites.get(viewOrder.getTargetObjectId());
				viewOrder.setTargetObject(targetSuite);
				targetSuite.setInfoUrl(viewOrder.getInfoUrl());

				if (viewOrder.getProductType() == ViewOrder.ProductType.Building3DLayout)
					targetSuite.setStatus(Suite.Status.Layout);
				break;
			case Building:
				Building targetBuilding = this.buildings.get(viewOrder.getTargetObjectId());
				viewOrder.setTargetObject(targetBuilding);
				targetBuilding.setInfoUrl(viewOrder.getInfoUrl());
				break;
			case Site:
				Site targetSite = this.sites.get(viewOrder.getTargetObjectId());
				viewOrder.setTargetObject(targetSite);
				break;
			}
		}
	}

	@Override
	public Collection<Site> getSites() {
		return this.sites.values();
	}

	@Override
	public Collection<Building> getBuildings() {
		return this.buildings.values();
	}

	@Override
	public Collection<Suite> getSuites() {
		return this.suites.values();
	}
	
	@Override
	public Collection<SuiteType> getSuiteTypes() {
		return this.suite_types.values();
	}

	@Override
	public Collection<ViewOrder> getViewOrders() {
		return this.viewOrders.values();
	}

}
