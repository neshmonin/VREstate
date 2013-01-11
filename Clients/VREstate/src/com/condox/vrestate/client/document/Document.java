package com.condox.vrestate.client.document;

import java.util.Collection;
import java.util.HashMap;
import java.util.Map;
import com.condox.vrestate.client.Log;
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

	public static Suite targetSuite = null;
	
	@Override
	public void Parse(String json) {
		progressBar = new ProgressBar();
		progressBar.Update(ProgressBar.ProgressLabel.Loading);
		progressBar.Update(0.0);
		ParseSuiteTypes(json);
		progressBar.Update(10.0);
		ParseBuildings(json);
		progressBar.Update(80.0);
		ParseSuites(json);
		progressBar.Update(90.0);
		ParseSites(json);
		progressBar.Update(100.0);
		ParseViewOrders(json);
		
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		if (obj.get("primaryViewOrderId") != null) {
			JSONString jsonString = obj.get("primaryViewOrderId").isString();
			String primaryViewOrderId = jsonString.stringValue();
			ViewOrder vo = viewOrders.get(primaryViewOrderId);
			if (vo != null) {
				switch (vo.getTargetObjectType())
				{
				case Suite:
					targetSuite = suites.get(vo.getTargetObjectId());
					targetSuite.setStatus(Suite.Status.Selected);
					if (vo.getProduct() == ViewOrder.ProductType.ExternalTour)
						targetSuite.setExternalLinkUrl(vo.getProductUrl());
					break;
					// TODO - should handle other viewOrder targets here
				}
			}
		}		
		
		for (Suite suite : getSuites())
			suite.CalcLineCoords();

		progressBar.Cleanup();
	}

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

	private void ParseSuites(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray suites = obj.get("suites").isArray();
		for (int index = 0; index < suites.size(); index++) {
			Suite suite = new Suite();
			suite.Parse(suites.get(index));
			this.suites.put(suite.getId(), suite);
		}
	}

	private void ParseBuildings(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray buildings = obj.get("buildings").isArray();
		for (int index = 0; index < buildings.size(); index++) {
			Building building = new Building();
			building.Parse(buildings.get(index));
			this.buildings.put(building.getId(), building);
		}
	}

	private void ParseSites(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray sites = obj.get("sites").isArray();
		for (int index = 0; index < sites.size(); index++) {
			Site site = new Site();
			site.Parse(sites.get(index));
			this.sites.put(site.getId(), site);
		}
	}

	private void ParseViewOrders(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray viewOrders = obj.get("viewOrders").isArray();
		for (int index = 0; index < viewOrders.size(); index++) {
			ViewOrder viewOrder = new ViewOrder();
			viewOrder.Parse(viewOrders.get(index));
			this.viewOrders.put(viewOrder.getId(), viewOrder);
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
