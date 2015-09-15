package com.condox.vrestate.client.filter;

import com.condox.clientshared.document.I_JSON;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;

public interface I_FilterSection extends I_State, I_JSON {
	public void Init();
	public boolean Reset();
	public boolean isFilteredIn(SuiteGeoItem suiteGI);
	public boolean isAny();
	public void Apply();
	public I_FilterSectionContainer getParentSectionContainer();
	public void RemoveSection();
}
