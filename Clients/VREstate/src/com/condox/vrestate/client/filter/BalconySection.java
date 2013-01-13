package com.condox.vrestate.client.filter;

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
	private static RadioButton rbBalconyAny = null;
	private static RadioButton rbBalconyYes = null;
	private static RadioButton rbBalconyNo = null;
	
	private BalconySection(){
		super();
	}
	
	public static BalconySection CreateSectionPanel(String sectionLabel, StackPanel stackPanel) {
		instance = new BalconySection();
		instance.stackPanel = stackPanel;  
		instance.setSpacing(10);
		stackPanel.add(instance, "Balconies / Terrases", false);
		instance.setSize("100%", "150px");

		rbBalconyYes = new RadioButton("new name", "Yes");
		rbBalconyYes.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.isAny = rbBalconyAny.getValue() == true;
				if (rbBalconyAny.getValue())
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Balconies/Terrases(any)");
				else
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Balconies/Terrases");
			}});
		instance.add(rbBalconyYes);

		rbBalconyNo = new RadioButton("new name", "No");
		rbBalconyNo.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.isAny = rbBalconyAny.getValue() == true;
				if (rbBalconyAny.getValue())
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Balconies/Terrases(any)");
				else
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Balconies/Terrases");
			}});
		instance.add(rbBalconyNo);

		rbBalconyAny = new RadioButton("new name", "Any");
		rbBalconyAny.addValueChangeHandler(new ValueChangeHandler<Boolean>(){
			@Override
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.isAny = rbBalconyAny.getValue() == true;
				if (rbBalconyAny.getValue())
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Balconies/Terrases(any)");
				else
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Balconies/Terrases");
			}});
		rbBalconyAny.setValue(true, true);
		instance.add(rbBalconyAny);

		return instance;
	}
	
	@Override
	public void Init() {
		rbBalconyAny.setValue(true);
		isAny = true;
	}

	@Override
	public void Reset() {
		rbBalconyAny.setValue(true, true);
		isAny = true;
	}

	@Override
	public boolean isFileredIn(Suite suite) {
		if (isAny)
			return true;
		
		SuiteType type = suite.getSuiteType();
		boolean presented = type.getBalconies() > 0; 
		if (rbBalconyAny.getValue())
			return true;
		return (presented == rbBalconyYes.getValue());
	}

	private boolean isAny = true;

	@Override
	public boolean isAny() {
		return isAny;
	}
}
