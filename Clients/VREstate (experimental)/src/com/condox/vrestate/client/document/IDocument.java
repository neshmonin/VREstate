package com.condox.vrestate.client.document;

import java.util.ArrayList;

public interface IDocument {

	void Parse(String json);
	ArrayList<Site> getSites();
	ArrayList<Building> getBuildings();
	ArrayList<Suite> getSuites();
	ArrayList<SuiteType> getSuiteTypes();
	boolean isReady();
}
