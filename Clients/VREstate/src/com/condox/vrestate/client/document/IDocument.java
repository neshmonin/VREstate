package com.condox.vrestate.client.document;

import java.util.Collection;

public interface IDocument {

	void Parse(String json);
	Collection<Site> getSites();
	Collection<Building> getBuildings();
	Collection<Suite> getSuites();
	Collection<SuiteType> getSuiteTypes();
	Collection<ViewOrder> getViewOrders();
}
