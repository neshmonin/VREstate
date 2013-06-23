package com.condox.vrestate.shared;

import java.util.Map;

import com.google.gwt.http.client.Response;

public interface IDocument {

	boolean Parse(String json);

	Map<Integer, Site> getSites();

	Map<Integer, Building> getBuildings();

	Map<Integer, Suite> getSuites();

	Map<Integer, SuiteType> getSuiteTypes();

	Map<String, ViewOrder> getViewOrders();
	
	Map<Integer, Suite> onCheckChanges(Response response);
}
