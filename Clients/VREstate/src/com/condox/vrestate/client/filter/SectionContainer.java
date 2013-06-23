package com.condox.vrestate.client.filter;


import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.SortedMap;
import java.util.TreeMap;

import com.condox.vrestate.shared.SuiteType;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.user.client.ui.VerticalPanel;

public abstract class SectionContainer extends VerticalPanel implements I_FilterSectionContainer {

	protected List<I_FilterSection> sections = null;
	protected SortedMap<Integer, SuiteGeoItem> activeSuites = new TreeMap<Integer, SuiteGeoItem>();;

	public SectionContainer() {
		super();
		activeSuites = new TreeMap<Integer, SuiteGeoItem>();;
	}

	@Override
	public void Init() {
		for (I_FilterSection section : sections)
			section.Init();
	}

	@Override
	public void Reset() {
		I_FilterSectionContainer parent = getParentSectionContainer(); 
		if (parent != null)
			setActiveSuiteGeoItems(parent.getActiveSuiteGeoItems());
		
		for (I_FilterSection section : sections)
			section.Reset();
	}
	
	@Override
	public int StateHash() {
		int hash = 0;
		for (I_FilterSection section : sections)
			hash += section.StateHash();
		
		return hash;
	}

	@Override
	public boolean isFilteredIn(SuiteGeoItem suiteGI) {
		for (I_FilterSection section : sections)
			if (!section.isFilteredIn(suiteGI))
				return false;

		getActiveSuiteGeoItems().put(suiteGI.getId(), suiteGI);
		return true;
	}

	@Override
	public boolean isAny() {
		for (I_FilterSection section : sections)
			if (!section.isAny())
				return false;
		return true;
	}

	@Override
	public void Apply() {
		for (I_FilterSection section : sections)
			section.Apply();
	}
	
	@Override
	public void RemoveSection() {
		for (I_FilterSection section : sections)
			section.RemoveSection();
	}

	@Override
	public void setActiveSuiteGeoItems(Map<Integer, SuiteGeoItem> activeSuites) {
		this.activeSuites.clear();
		this.activeSuites.putAll(activeSuites);
	}

	/* (non-Javadoc)
	 * @see com.condox.vrestate.client.filter.I_FilterSectionContainer#getActiveSuites()
	 */
	@Override
	public Map<Integer, SuiteGeoItem> getActiveSuiteGeoItems() {
		return activeSuites;
	}

	@Override
	public Map<Integer, SuiteType> getActiveSuiteTypes() {
		Map<Integer, SuiteType> suiteTypes = new HashMap<Integer, SuiteType>();
		for (SuiteGeoItem suiteGI : getActiveSuiteGeoItems().values()) {
			SuiteType type = suiteGI.suite.getSuiteType();
			if (type == null) continue;
			int typeId = type.getId();
			if (!suiteTypes.containsKey(typeId))
				suiteTypes.put(typeId, type);
		}			
			
		return suiteTypes;
	}
}