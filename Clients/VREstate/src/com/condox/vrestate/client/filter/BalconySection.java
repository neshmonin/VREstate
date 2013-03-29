package com.condox.vrestate.client.filter;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
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
	
	private BalconySection(){
		super();
	}
	
	static boolean nonePresent = false;
	static boolean terracesPresent = false;
	static boolean balconiesPresent = false;
	public static BalconySection CreateSectionPanel(String sectionLabel, StackPanel stackPanel) {
		// =====================================================
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			nonePresent = nonePresent || (suite_type.getBalconies()== 0 && suite_type.getTerraces() == 0); 
			balconiesPresent = balconiesPresent || (suite_type.getBalconies()> 0);
			terracesPresent = terracesPresent || (suite_type.getTerraces() > 0);
		}
		if (!balconiesPresent && !terracesPresent)
			return null;
		// =====================================================
		instance = new BalconySection();
		instance.stackPanel = stackPanel;  
		//instance.setSpacing(10);
		stackPanel.add(instance, generateTitle(), false);
		instance.setSize("100%", "150px");

		rbDoNotCare = new RadioButton("new name", "Don't Care");
		rbDoNotCare.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.UpdateCaption();
			}});
		rbDoNotCare.setValue(true, true);
		instance.add(rbDoNotCare);
 
		if (nonePresent) {
			rbNone = new RadioButton("new name", "None");
			rbNone.addStyleDependentName("margined");
			rbNone.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.UpdateCaption();
				}});
			instance.add(rbNone);
		}
 
		if (balconiesPresent) {
			rbBalconyYes = new RadioButton("new name", "With Balcony");
			rbBalconyYes.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.UpdateCaption();
				}});
			rbBalconyYes.addStyleDependentName("margined");
			instance.add(rbBalconyYes);
		}

		if (terracesPresent) {
			rbTerraceYes = new RadioButton("new name", "With Terrace");
			rbTerraceYes.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.UpdateCaption();
				}});
			rbTerraceYes.addStyleDependentName("margined");
			instance.add(rbTerraceYes);
		}

		if (balconiesPresent && terracesPresent) {
			rbBalconyOrTerrace = new RadioButton("new name", "With Balcony or Terrace");
			rbBalconyOrTerrace.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
				@Override
				public void onValueChange(ValueChangeEvent<Boolean> event) {
					instance.UpdateCaption();
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
	public boolean isFilteredIn(Suite suite) {
		if (rbDoNotCare.getValue())
			return true;
		
		SuiteType type = suite.getSuiteType();
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
		instance.isChanged = false;
		if (Filter.initialized == true)
			Filter.get().onChanged();
	}

	private boolean isChanged = false;
	@Override
	public boolean isChanged() {
		return isChanged;
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
	
	private void UpdateCaption() {
		if (rbDoNotCare.getValue())
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), 
					generateTitle() + "(any)");
 		else
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), 
					generateTitle());
		isChanged = true;
		if (Filter.initialized == true)
			Filter.get().onChanged();
	}
}
