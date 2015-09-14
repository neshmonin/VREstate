package com.condox.vrestate.client.filter;


import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.document.SuiteType;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.json.client.JSONBoolean;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
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
		stackPanel.add(instance, instance.generateTitle(true), false);
		instance.setSize("100%", "150px");

		rbDoNotCare = new RadioButton("new name", Filter.i18n.dontCare());
		rbDoNotCare.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.Apply();
			}});
		rbDoNotCare.setValue(true, true);
		instance.add(rbDoNotCare);
 
		if (nonePresent) {
			rbNone = new RadioButton("new name", Filter.i18n.none());
			rbNone.addStyleDependentName("margined");
			rbNone.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.Apply();
				}});
			instance.add(rbNone);
		}
 
		if (balconiesPresent) {
			rbBalconyYes = new RadioButton("new name", Filter.i18n.withBalcony());
			rbBalconyYes.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.Apply();
				}});
			rbBalconyYes.addStyleDependentName("margined");
			instance.add(rbBalconyYes);
		}

		if (terracesPresent) {
			rbTerraceYes = new RadioButton("new name", Filter.i18n.withTerrace());
			rbTerraceYes.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.Apply();
				}});
			rbTerraceYes.addStyleDependentName("margined");
			instance.add(rbTerraceYes);
		}

		if (balconiesPresent && terracesPresent) {
			rbBalconyOrTerrace = new RadioButton("new name", Filter.i18n.withBalconyOrTerrace());
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
		instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), 
					generateTitle(rbDoNotCare.getValue()));
		Filter.onChange();
	}

	private String generateTitle(boolean dontCare) {
		String retValue = "";
		if (terracesPresent && balconiesPresent)
			retValue = dontCare? Filter.i18n.balconies_terraces_any():Filter.i18n.balconies_terraces();
		else
		if (balconiesPresent)
			retValue = dontCare? Filter.i18n.balconies_any():Filter.i18n.balconies();
		else
		if (terracesPresent)
			retValue = dontCare? Filter.i18n.terraces_any():Filter.i18n.terraces();
		
		return retValue;
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
		JSONObject obj = new JSONObject();
		obj.put("name", new JSONString(this.getClass().getName()));
		
		saveRadioButton(rbBalconyOrTerrace, obj, "balcony_or_terrace");
		saveRadioButton(rbBalconyYes, obj, "balcony_yes");
		saveRadioButton(rbDoNotCare, obj, "do_not_care");
		saveRadioButton(rbNone, obj, "none");
		saveRadioButton(rbTerraceYes, obj, "terrace_yes");
		
		return obj;
	}
	
	private void saveRadioButton(RadioButton source, JSONObject obj, String key) {
		if (source == null) return;
		boolean value = source.getValue();
		obj.put(key, JSONBoolean.getInstance(value));
	}

	@Override
	public void fromJSONObject(JSONObject obj) {
		if (obj == null) return;
		
		if (!obj.containsKey("name")) return;
		if (obj.get("name").isString() == null) return;
		String name = obj.get("name").isString().stringValue();
		
		if (name.equals(getClass().getName())) {
			loadRadioButton(rbBalconyOrTerrace, obj, "balcony_or_terrace");
			loadRadioButton(rbBalconyYes, obj, "balcony_yes");
			loadRadioButton(rbDoNotCare, obj, "do_not_care");
			loadRadioButton(rbNone, obj, "none");
			loadRadioButton(rbTerraceYes, obj, "terrace_yes");
		}
	}
	
	private void loadRadioButton(RadioButton target, JSONObject obj, String key) {
		if (!obj.containsKey(key)) return;
		if (obj.get(key).isBoolean() == null) return;
		boolean value = obj.get(key).isBoolean().booleanValue();
		target.setValue(value, true);
	}
}
