package com.condox.clientshared.document;

import java.util.List;
import java.util.Map;

import com.condox.clientshared.communication.I_CheckChanges;

public interface IDocument extends  I_CheckChanges {

	boolean Parse(String json);

	Map<Integer, Site> getSites();

	Map<Integer, Building> getBuildings();

	Map<Integer, Suite> getSuites();

	Map<Integer, SuiteType> getSuiteTypes();

	Map<String, ViewOrder> getViewOrders();

	List<String> getStructures();
}
