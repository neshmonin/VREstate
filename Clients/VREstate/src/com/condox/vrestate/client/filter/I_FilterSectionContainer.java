package com.condox.vrestate.client.filter;


import java.util.Map;

import com.condox.clientshared.document.SuiteType;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;

public interface I_FilterSectionContainer extends I_FilterSection{

	public abstract Map<Integer, SuiteGeoItem> getActiveSuiteGeoItems();
	public abstract Map<Integer, SuiteType> getActiveSuiteTypes();
	public abstract void setActiveSuiteGeoItems(Map<Integer, SuiteGeoItem> suites);
}