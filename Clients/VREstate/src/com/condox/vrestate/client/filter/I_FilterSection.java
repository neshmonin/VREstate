package com.condox.vrestate.client.filter;

import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;

public interface I_FilterSection extends I_State {
	public void Init();
	public void Reset();
	public boolean isFilteredIn(SuiteGeoItem suiteGI);
	public boolean isAny();
	public void Apply();
	public I_FilterSectionContainer getParentSectionContainer();
	public void RemoveSection();
}
