package com.condox.vrestate.client.filter;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class BathroomSection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	private static BathroomSection instance = null;
	private static CheckBox cbAny = null;
	private static CheckBox cbOneBathroom = null;
	private static CheckBox cbTwoBathrooms = null;
	private static CheckBox cbThreeBathrooms = null;
	private static CheckBox cbFourBathrooms = null;
	private static CheckBox cbFiveBathrooms = null;

	private BathroomSection(){ super();	}
	
	public static BathroomSection CreateSectionPanel(String sectionLabel, StackPanel stackPanel) {
		//=====================================================
		boolean creating = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes())
			creating = creating || (suite_type.getBathrooms() >= 0);
		if (!creating)
			return null;
		//=====================================================
		instance = new BathroomSection();
		instance.stackPanel = stackPanel;  
		instance.setSpacing(5);
		stackPanel.add(instance, "Bathrooms", false);
		instance.setSize("100%", "150px");

		cbAny = new CheckBox("Any, or...");
		cbAny.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (cbAny.getValue()) {
					cbOneBathroom.setValue(true, false);
					cbTwoBathrooms.setValue(true, false);
					cbThreeBathrooms.setValue(true, false);
					cbFourBathrooms.setValue(true, false);
					cbFiveBathrooms.setValue(true, false);
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Bathrooms (any)");
				} else
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Bathrooms");
				instance.isAny = cbAny.getValue() == true;
				instance.stackPanel.getParent();
			}
		});
		instance.add(cbAny);

		cbOneBathroom = new CheckBox(" . . . One bathroom");
		cbOneBathroom.addStyleDependentName("margined");
		cbOneBathroom.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbOneBathroom);

		cbTwoBathrooms = new CheckBox(" . . . Two bathrooms");
		cbTwoBathrooms.addStyleDependentName("margined");
		cbTwoBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbTwoBathrooms);

		cbThreeBathrooms = new CheckBox(" . . . Three bathrooms");
		cbThreeBathrooms.addStyleDependentName("margined");
		cbThreeBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbThreeBathrooms);

		cbFourBathrooms = new CheckBox(" . . . Four bathrooms");
		cbFourBathrooms.addStyleDependentName("margined");
		cbFourBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbFourBathrooms);

		cbFiveBathrooms = new CheckBox(" . . . More");
		cbFiveBathrooms.addStyleDependentName("margined");
		cbFiveBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbFiveBathrooms);
		
		return instance;
	}
	
	private static boolean isAllBathroomsUnchecked() {
		if(cbOneBathroom.getValue()&&(cbOneBathroom.isEnabled()))
			return false;
		if(cbTwoBathrooms.getValue()&&(cbTwoBathrooms.isEnabled()))
			return false;
		if(cbThreeBathrooms.getValue()&&(cbThreeBathrooms.isEnabled()))
			return false;
		if(cbFourBathrooms.getValue()&&(cbFourBathrooms.isEnabled()))
			return false;
		if(cbFiveBathrooms.getValue()&&(cbFiveBathrooms.isEnabled()))
			return false;
		return true;
	}

	private static boolean isAtLeastOneUnchecked() {
		instance.isAny = false;
		if(!cbOneBathroom.getValue()&&(cbOneBathroom.isEnabled()))
			return true;
		if(!cbTwoBathrooms.getValue()&&(cbTwoBathrooms.isEnabled()))
			return true;
		if(!cbThreeBathrooms.getValue()&&(cbThreeBathrooms.isEnabled()))
			return true;
		if(!cbFourBathrooms.getValue()&&(cbFourBathrooms.isEnabled()))
			return true;
		if(!cbFiveBathrooms.getValue()&&(cbFiveBathrooms.isEnabled()))
			return true;
		
		instance.isAny = true;
		return false;
	}

	
	@Override
	public void Init() {
		boolean one = false;
		boolean two = false;
		boolean three = false;
		boolean four = false;
		boolean five = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			if (suite_type.getBathrooms() >= 5)
				five = true;
			else
				switch (suite_type.getBathrooms()) {
				case 0:
				case 1:
					one = true;
					break;
				case 2:
					two = true;
					break;
				case 3:
					three = true;
					break;
				case 4:
					four = true;
					break;
				}
		}
		cbOneBathroom.setEnabled(one);
		cbTwoBathrooms.setEnabled(two);
		cbThreeBathrooms.setEnabled(three);
		cbFourBathrooms.setEnabled(four);
		cbFiveBathrooms.setEnabled(five);
		isAny = true;
	}

	@Override
	public void Reset() {
		cbAny.setValue(true, true);
	}

	@Override
	public boolean isFileredIn(Suite suite) {
		if (isAny)
			return true;
		
		SuiteType type = suite.getSuiteType();
		int bathrooms = type.getBathrooms();
		if (cbOneBathroom.getValue() && bathrooms == 0)
			return true;
		else if (cbOneBathroom.getValue() && bathrooms == 1)
			return true;
		else if (cbTwoBathrooms.getValue() && bathrooms == 2)
			return true;
		else if (cbThreeBathrooms.getValue() && bathrooms == 3)
			return true;
		else if (cbFourBathrooms.getValue() && bathrooms == 4)
			return true;
		else if (cbFiveBathrooms.getValue() && bathrooms > 4)
			return true;
		return false;
	}

	private boolean isAny = true;

	@Override
	public boolean isAny() {
		return isAny;
	}
}
