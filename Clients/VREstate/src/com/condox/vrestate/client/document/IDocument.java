package com.condox.vrestate.client.document;

import java.util.Map;

public interface IDocument {

	boolean Parse(String json);
	Map<Integer, Site> getSites();
	Map<Integer, Building> getBuildings();
	Map<Integer, Suite> getSuites();
	Map<Integer, SuiteType> getSuiteTypes();
	Map<String, ViewOrder> getViewOrders();
}
