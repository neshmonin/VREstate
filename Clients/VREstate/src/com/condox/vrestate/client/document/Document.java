package com.condox.vrestate.client.document;

import java.util.ArrayList;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.view.ProgressBar;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;

public class Document implements IDocument {

	private static IDocument instance = new Document();
	public static ProgressBar progressBar;

	private Document() {
	};

	public static IDocument get() {
		return instance;
	}

	private ArrayList<Site> sites = new ArrayList<Site>();
	private ArrayList<Building> buildings = new ArrayList<Building>();
	private ArrayList<Suite> suites = new ArrayList<Suite>();
	private ArrayList<SuiteType> suite_types = new ArrayList<SuiteType>();

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
			this.suite_types.add(suite_type);
		}
	}

	private void ParseSuites(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray suites = obj.get("suites").isArray();
		for (int index = 0; index < suites.size(); index++) {
			Suite suite = new Suite();
			suite.Parse(suites.get(index));
			this.suites.add(suite);
		}
	}

	private void ParseBuildings(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray buildings = obj.get("buildings").isArray();
		for (int index = 0; index < buildings.size(); index++) {
			Building building = new Building();
			building.Parse(buildings.get(index));
			this.buildings.add(building);
		}
	}

	private void ParseSites(String json) {
		JSONObject obj = JSONParser.parseLenient(json).isObject();
		JSONArray sites = obj.get("sites").isArray();
		for (int index = 0; index < sites.size(); index++) {
			Site site = new Site();
			site.Parse(sites.get(index));
			this.sites.add(site);
		}
	}

	@Override
	public ArrayList<Site> getSites() {
		return this.sites;
	}

	@Override
	public ArrayList<Building> getBuildings() {
		return this.buildings;
	}

	@Override
	public ArrayList<Suite> getSuites() {
		return this.suites;
	}
	
	@Override
	public ArrayList<SuiteType> getSuiteTypes() {
		return this.suite_types;
	}

}
