package com.condox.vrestate.client.filter;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.condox.vrestate.client.Log;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.Suite.Status;
import com.condox.vrestate.client.filter.PriceSection.PriceType;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.StackPanel;

public class OwnershipSection extends SectionContainer {

	StackPanel stackPanel = null;
	private String sectionLabel;

	private static OwnershipSection instance = null;
	private static CheckBox cbAny = null;
	private static CheckBox cbResale = null;
	private static CheckBox cbRent = null;
	private static CheckBox cbNotBuilt = null;
	private static boolean anyNotBuild = false;
	private static boolean anyResale = false;
	private static boolean anyRent = false;
	private static boolean simpleCase = false;
	private I_FilterSectionContainer parentSection;
	
	private OwnershipSection() {
		super();
	}

	public static I_FilterSection CreateSectionPanel(I_FilterSectionContainer parentSection, 
			String sectionLabel,
			StackPanel stackPanel) {
		// =====================================================
		for (SuiteGeoItem suiteGE : parentSection.getActiveSuiteGeoItems().values()) {
			Suite.Status status = suiteGE.suite.getStatus(); 
			anyNotBuild = anyNotBuild || status == Suite.Status.Available ||
										 status == Suite.Status.OnHold;
			anyResale = anyResale || status == Suite.Status.ResaleAvailable;
			anyRent = anyRent || status == Suite.Status.AvailableRent;
		}

		simpleCase = (!anyNotBuild && !anyResale) ||
					 (!anyNotBuild && !anyRent) ||
					 (!anyResale && !anyRent);
		
		instance = new OwnershipSection();
		instance.parentSection = parentSection;
		instance.stackPanel = stackPanel;
		if (simpleCase) {			
			instance.setActiveSuiteGeoItems(parentSection.getActiveSuiteGeoItems());
			instance.AssembleSections();
			instance.Init();
			instance.ResetAllSections();
			instance.Apply();
		}
		else {
			Log.write("OwnershipSection("+sectionLabel+")");
			instance.sectionLabel = sectionLabel;
			instance.setSpacing(5);
			stackPanel.add(instance, "Bathrooms", false);
			instance.setSize("100%", "150px");
	
			cbAny = new CheckBox("Any, or");
			cbAny.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.isAny = cbAny.getValue().booleanValue();
					if (instance.isAny) {
						cbResale.setValue(true, false);
						cbRent.setValue(true, false);
						cbNotBuilt.setValue(true, false);
					}
					instance.Parent2ActiveSuiteGeoItems();
					instance.AssembleSections();
					instance.Init();
					instance.ResetAllSections();
					instance.Apply();
				}
			});
			instance.add(cbAny);
	
			cbResale = new CheckBox("Condos for Resale");
			cbResale.addStyleDependentName("margined");
			cbResale.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					if (instance.isAllUnchecked())
						cbAny.setValue(true, true);

					cbAny.setValue(!instance.isAtLeastOneUnchecked(), false);
					instance.Parent2ActiveSuiteGeoItems();
					instance.AssembleSections();
					instance.Init();
					instance.ResetAllSections();
					instance.Apply();
				}
			});
			instance.add(cbResale);
	
			cbRent = new CheckBox("Condos for Rent");
			cbRent.addStyleDependentName("margined");
			cbRent.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					if (instance.isAllUnchecked())
						cbAny.setValue(true, true);

					cbAny.setValue(!instance.isAtLeastOneUnchecked(), false);
					instance.Parent2ActiveSuiteGeoItems();
					instance.AssembleSections();
					instance.Init();
					instance.ResetAllSections();
					instance.Apply();
				}
			});
			instance.add(cbRent);
	
			cbNotBuilt = new CheckBox("Not Built yet Condos");
			cbNotBuilt.addStyleDependentName("margined");
			cbNotBuilt.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					if (instance.isAllUnchecked())
						cbAny.setValue(true, true);

					cbAny.setValue(!instance.isAtLeastOneUnchecked(), false);
					instance.Parent2ActiveSuiteGeoItems();
					instance.AssembleSections();
					instance.Init();
					instance.ResetAllSections();
					instance.Apply();
				}
			});
			instance.add(cbNotBuilt);

			//cbAny.setValue(true, true);
		}
		
		return instance;
	}

	private void AssembleSections() {
		if (sections == null)
			sections = new ArrayList<I_FilterSection>();
		else {
			for (I_FilterSection section : sections)
				section.RemoveSection();
			sections.clear();
		}

		if (anyNotBuild || anyResale) {
			I_FilterSection purchasePriceSection = 
				PriceSection.CreateSectionPanel(this, "Purchase Price", stackPanel, PriceType.Ownership);
			if (purchasePriceSection != null) sections.add(purchasePriceSection);
		}
		
		if (anyRent) {
			I_FilterSection rentPriceSection = 
				PriceSection.CreateSectionPanel(this, "Rent Price", stackPanel, PriceType.Rent);
			if (rentPriceSection != null) sections.add(rentPriceSection);
		}
		
		I_FilterSection bedroomsSection = BedroomsSection.CreateSectionPanel(this, "Bedrooms", stackPanel);
		if (bedroomsSection != null) sections.add(bedroomsSection);
		I_FilterSection bathroomSection = BathroomSection.CreateSectionPanel(this, "Bathrooms", stackPanel);
		if (bathroomSection != null) sections.add(bathroomSection);
		I_FilterSection areaSection = AreaSection.CreateSectionPanel(this, "Area", stackPanel);
		if (areaSection != null) sections.add(areaSection);
		I_FilterSection balconySection = BalconySection.CreateSectionPanel(this, "Balconies", stackPanel);
		if (balconySection != null) sections.add(balconySection);
		
	}
	
	private String generateLabel() {
		return sectionLabel + (isAny ? " (any)" : "");
	}
	
	private boolean isAllUnchecked() {
		if (simpleCase) return false; 
		
		if (cbResale.getValue() && (cbResale.isEnabled()))
			return false;
		if (cbRent.getValue() && (cbRent.isEnabled()))
			return false;
		if (cbNotBuilt.getValue() && (cbNotBuilt.isEnabled()))
			return false;
		return true;
	}

	private boolean isAtLeastOneUnchecked() {
		if (simpleCase) return false; 
		
		isAny = false;
		if (!cbResale.getValue() && (cbResale.isEnabled()))
			return true;
		if (!cbRent.getValue() && (cbRent.isEnabled()))
			return true;
		if (!cbNotBuilt.getValue() && (cbNotBuilt.isEnabled()))
			return true;

		isAny = true;
		return false;
	}

	@Override
	public void Init() {
		if (!simpleCase) {
			cbResale.setVisible(anyResale);
			cbRent.setVisible(anyRent);
			cbNotBuilt.setVisible(anyNotBuild);
	
			cbResale.setEnabled(anyResale);
			cbRent.setEnabled(anyRent);
			cbNotBuilt.setEnabled(anyNotBuild);
		}
		
		super.Init();
	}
	
	private void ResetAllSections()	{ super.Reset(); }
	
	@Override
	public void Reset() {
		if (simpleCase){
			super.Reset();
			return;
		}
		
		if (!isAny)
			cbAny.setValue(true, true);
		else
			ResetAllSections();
	}
	
	public void Parent2ActiveSuiteGeoItems() {
		if (simpleCase) return;
		
		Map<Integer, SuiteGeoItem> filteredSuites = new HashMap<Integer, SuiteGeoItem>();
		Map<Integer, SuiteGeoItem> allSuites = getParentSectionContainer().getActiveSuiteGeoItems();
		for (SuiteGeoItem suiteGI: allSuites.values()) {
			Suite.Status status = suiteGI.suite.getStatus(); 
			if (anyResale && cbResale.getValue() && status == Suite.Status.ResaleAvailable) {
				filteredSuites.put(suiteGI.suite.getId(), suiteGI);
				continue;
			}
			if (anyNotBuild && cbNotBuilt.getValue() && (status == Suite.Status.Available ||
	   					   					  status == Suite.Status.OnHold)) {
				filteredSuites.put(suiteGI.suite.getId(), suiteGI);
				continue;
			}
			if (anyRent && cbRent.getValue() && status == Suite.Status.AvailableRent) {
				filteredSuites.put(suiteGI.suite.getId(), suiteGI);
				continue;
			}
		}

		setActiveSuiteGeoItems(filteredSuites);

		//Filter filter = Filter.get();
		//if(filter != null)
		//	Filter.get().Apply();
	}	
	
	@Override
	public boolean isFilteredIn(SuiteGeoItem suiteGI) {
		if (simpleCase)
			return super.isFilteredIn(suiteGI);
		else {
			if (isAny)
				return super.isFilteredIn(suiteGI);
	
			Status status = suiteGI.suite.getStatus(); 
	
			if (cbResale.isEnabled() && cbResale.getValue() && status == Status.ResaleAvailable)
				return super.isFilteredIn(suiteGI);
	
			if (cbRent.isEnabled() && cbRent.getValue() && status == Status.AvailableRent)
				return super.isFilteredIn(suiteGI);
	
			if (cbNotBuilt.isEnabled() && cbNotBuilt.getValue() && (status == Status.Available ||
					 						   						status == Status.OnHold))
				return super.isFilteredIn(suiteGI);
		}

		return false;
	}

	@Override
	public int StateHash() {
		int hash = super.StateHash() + hashCode();
		if (!simpleCase) {
			if (cbResale.getValue()) hash += cbResale.hashCode(); 
			if (cbRent.getValue()) hash += cbRent.hashCode(); 
			if (cbNotBuilt.getValue()) hash += cbNotBuilt.hashCode(); 
			if (cbAny.getValue()) hash += cbAny.hashCode();
		}
		
		return hash;
	}
	
	private boolean isAny = false;

	@Override
	public boolean isAny() {
		if (simpleCase) return super.isAny();
		if (!super.isAny())
			return false;

		return isAny;
	}

	@Override
	public void Apply() {
		if (!simpleCase)
			stackPanel.setStackText(stackPanel.getWidgetIndex(this), generateLabel());

		super.Apply();
		Filter.onChange();
	}

	@Override
	public void RemoveSection() {
		super.RemoveSection();
	}

	@Override
	public I_FilterSectionContainer getParentSectionContainer() {
		return parentSection;
	}
}
