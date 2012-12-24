package com.condox.vrestate.client.document;

import java.util.ArrayList;

import com.condox.vrestate.client.Log;
import com.google.gwt.dom.client.FrameElement;
import com.google.gwt.json.client.JSONArray;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONParser;
import com.google.gwt.user.client.Timer;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.Frame;

public class Document implements IDocument {

	private static IDocument instance = new Document();

	private Document() {
	};

	public static IDocument get() {
		return instance;
	}

	private ArrayList<Site> sites = new ArrayList<Site>();
	private ArrayList<Building> buildings = new ArrayList<Building>();
	private ArrayList<Suite> suites = new ArrayList<Suite>();
	private ArrayList<SuiteType> suite_types = new ArrayList<SuiteType>();
	private boolean isReady = false;

	@Override
	public void Parse(String json) {
		isReady = false;
		ParseSuiteTypes(json);
		ParseSuites(json);
		ParseBuildings(json);
		ParseSites(json);
		//**************
//		Window.open("", "_blank", null);

//		test.schedule(10000);
		 isReady = true;
	}

	private Timer test = new Timer() {

		@Override
		public void run() {
			cancel();
			isReady = true;
		}
	};

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
		// TODO Auto-generated method stub
		return this.sites;
	}

	@Override
	public ArrayList<Building> getBuildings() {
		// TODO Auto-generated method stub
		return this.buildings;
	}

	@Override
	public ArrayList<Suite> getSuites() {
		// TODO Auto-generated method stub
		return this.suites;
	}
	
	@Override
	public ArrayList<SuiteType> getSuiteTypes() {
		// TODO Auto-generated method stub
		return this.suite_types;
	}

	@Override
	public boolean isReady() {
		return this.isReady;
	}

}
