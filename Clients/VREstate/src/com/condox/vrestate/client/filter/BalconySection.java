package com.condox.vrestate.client.filter;


import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.document.SuiteType;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.user.client.ui.RadioButton;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class BalconySection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	private static BalconySection instance = null;
	private static RadioButton rbDoNotCare = null;
	private static RadioButton rbNone = null;
 	private static RadioButton rbBalconyYes = null;
	private static RadioButton rbTerraceYes = null;
	private static RadioButton rbBalconyOrTerrace = null;
	private I_FilterSectionContainer parentSection;
	
	private BalconySection(){
		super();
	}
	
	static boolean nonePresent = false;
	static boolean terracesPresent = false;
	static boolean balconiesPresent = false;
	public static I_FilterSection CreateSectionPanel(I_FilterSectionContainer parentSection, 
			String sectionLabel,
			StackPanel stackPanel) {
		Log.write("BalconySection(" + sectionLabel + ")");
		// =====================================================
		for (SuiteType suite_type : parentSection.getActiveSuiteTypes().values()) {
			nonePresent = nonePresent || (suite_type.getBalconies()== 0 && suite_type.getTerraces() == 0); 
			balconiesPresent = balconiesPresent || (suite_type.getBalconies()> 0);
			terracesPresent = terracesPresent || (suite_type.getTerraces() > 0);
		}
		if (!balconiesPresent && !terracesPresent)
			return null;
		// =====================================================
		instance = new BalconySection();
		instance.parentSection = parentSection;
		instance.stackPanel = stackPanel;  
		//instance.setSpacing(10);
		stackPanel.add(instance, generateTitle(), false);
		instance.setSize("100%", "150px");

		rbDoNotCare = new RadioButton("new name", "Don't Care");
		rbDoNotCare.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.Apply();
			}});
		rbDoNotCare.setValue(true, true);
		instance.add(rbDoNotCare);
 
		if (nonePresent) {
			rbNone = new RadioButton("new name", "None");
			rbNone.addStyleDependentName("margined");
			rbNone.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.Apply();
				}});
			instance.add(rbNone);
		}
 
		if (balconiesPresent) {
			rbBalconyYes = new RadioButton("new name", "With Balcony");
			rbBalconyYes.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.Apply();
				}});
			rbBalconyYes.addStyleDependentName("margined");
			instance.add(rbBalconyYes);
		}

		if (terracesPresent) {
			rbTerraceYes = new RadioButton("new name", "With Terrace");
			rbTerraceYes.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.Apply();
				}});
			rbTerraceYes.addStyleDependentName("margined");
			instance.add(rbTerraceYes);
		}

		if (balconiesPresent && terracesPresent) {
			rbBalconyOrTerrace = new RadioButton("new name", "With Balcony or Terrace");
			rbBalconyOrTerrace.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.Apply();
				}});
			rbBalconyOrTerrace.addStyleDependentName("margined");
			instance.add(rbBalconyOrTerrace);
		}


		return instance;
	}
	
	@Override
	public void Init() {
		rbDoNotCare.setValue(true);
	}

	@Override
	public void Reset() {
		rbDoNotCare.setValue(true,true);
	}

	@Override
	public int StateHash() {
		int hash = hashCode();
		
		if (rbNone!=null && rbNone.getValue()) hash += rbNone.hashCode();
		if (rbDoNotCare!=null&&rbDoNotCare.getValue()) hash += rbDoNotCare.hashCode();
		if (rbBalconyOrTerrace!=null&&rbBalconyOrTerrace.getValue()) hash += rbBalconyOrTerrace.hashCode();
		if (rbBalconyYes!=null&&rbBalconyYes.getValue()) hash += rbBalconyYes.hashCode();
		if (rbTerraceYes!=null&&rbTerraceYes.getValue()) hash += rbTerraceYes.hashCode();
		
		return hash;
	}
	
	@Override
	public boolean isFilteredIn(SuiteGeoItem suiteGI) {
		if (rbDoNotCare.getValue())
			return true;
		
		SuiteType type = suiteGI.suite.getSuiteType();
		if (rbNone != null && rbNone.getValue() &&
				(type.getTerraces() == 0 && type.getBalconies() == 0))
			return true;
		if (rbBalconyOrTerrace != null && rbBalconyOrTerrace.getValue() &&
				(type.getTerraces() > 0 || type.getBalconies() > 0))
			return true;
		if (rbBalconyYes != null && rbBalconyYes.getValue() &&
				type.getBalconies() > 0)
			return true;
		if (rbTerraceYes != null && rbTerraceYes.getValue() &&
				type.getTerraces() > 0)
			return true;
 
		return false;
	}

	@Override
	public boolean isAny() {
		return rbDoNotCare.getValue();
	}

	@Override
	public void Apply() {
		if (rbDoNotCare.getValue())
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), 
					generateTitle() + "(any)");
 		else
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), 
					generateTitle());
		Filter.onChange();
	}

	private static String generateTitle() {
		if (terracesPresent && balconiesPresent)
			return "Balconies/Terraces";
		if (balconiesPresent)
			return "Balconies";
		if (terracesPresent)
			return "Terraces";
		return "";		
	}

	@Override
	public void RemoveSection() {
		super.removeFromParent();
	}

	@Override
	public I_FilterSectionContainer getParentSectionContainer() {
		return parentSection;
	}

	@Override
	public JSONObject toJSONObject() {
		return null;
	}

	@Override
	public void fromJSONObject(JSONObject json) {
	}
}
