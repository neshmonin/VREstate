package com.condox.vrestate.client.filter;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class BedroomsSection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	private static BedroomsSection instance = null;
	private static CheckBox cbAny = null;
	private static CheckBox cbStudio = null;
	private static CheckBox cbOneBedrooms = null;
	private static CheckBox cbTwoBedrooms = null;
	private static CheckBox cbThreeBedrooms = null;
	private static CheckBox cbFourBedrooms = null;
	private static CheckBox cbFiveBedrooms = null;
	
	private BedroomsSection(){super();}
	
	public static BedroomsSection CreateSectionPanel(String sectionLabel, StackPanel stackPanel) {
		//=====================================================
		boolean creating = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes())
			creating = creating || (suite_type.getBedrooms() >= 0);
		if (!creating)
			return null;
		//=====================================================
		instance = new BedroomsSection();
		instance.stackPanel = stackPanel;  
		stackPanel.add(instance, "Bedrooms", false);
		instance.setSize("100%", "150px");

		cbAny = new CheckBox("Any, or...");
		cbAny.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (cbAny.getValue()) {
					cbStudio.setValue(true, false);
					cbOneBedrooms.setValue(true, false);
					cbTwoBedrooms.setValue(true, false);
					cbThreeBedrooms.setValue(true, false);
					cbFourBedrooms.setValue(true, false);
					cbFiveBedrooms.setValue(true, false);
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Bedrooms (any)");
				} else
					instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance), "Bedrooms");
				instance.isAny = cbAny.getValue() == true; 
			}
		});
		instance.add(cbAny);

		cbStudio = new CheckBox(" . . . Studio");
		cbStudio.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		cbStudio.addStyleDependentName("margined");

		cbStudio.setEnabled(false);
		instance.add(cbStudio);

		cbOneBedrooms = new CheckBox(" . . . One bedroom");
		cbOneBedrooms.addStyleDependentName("margined");
		cbOneBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		cbOneBedrooms.setEnabled(false);
		instance.add(cbOneBedrooms);

		cbTwoBedrooms = new CheckBox(" . . . Two bedrooms");
		cbTwoBedrooms.addStyleDependentName("margined");
		cbTwoBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		cbTwoBedrooms.setEnabled(false);
		instance.add(cbTwoBedrooms);

		cbThreeBedrooms = new CheckBox(" . . . Three bedrooms");
		cbThreeBedrooms.addStyleDependentName("margined");
		cbThreeBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		cbThreeBedrooms.setEnabled(false);
		instance.add(cbThreeBedrooms);

		cbFourBedrooms = new CheckBox(" . . . Four bedrooms");
		cbFourBedrooms.addStyleDependentName("margined");
		cbFourBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		cbFourBedrooms.setEnabled(false);
		instance.add(cbFourBedrooms);

		cbFiveBedrooms = new CheckBox(" . . . More");
		cbFiveBedrooms.addStyleDependentName("margined");
		cbFiveBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);

				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		cbFiveBedrooms.setEnabled(false);
		instance.add(cbFiveBedrooms);
		
		return instance;
	}
	
	private static boolean isAllBedroomsUnchecked() {
		if(cbStudio.getValue()&&(cbStudio.isEnabled()))
			return false;
		if(cbOneBedrooms.getValue()&&(cbOneBedrooms.isEnabled()))
			return false;
		if(cbTwoBedrooms.getValue()&&(cbTwoBedrooms.isEnabled()))
			return false;
		if(cbThreeBedrooms.getValue()&&(cbThreeBedrooms.isEnabled()))
			return false;
		if(cbFourBedrooms.getValue()&&(cbFourBedrooms.isEnabled()))
			return false;
		if(cbFiveBedrooms.getValue()&&(cbFiveBedrooms.isEnabled()))
			return false;
		return true;
	}
	
	private static boolean isAtLeastOneUnchecked() {
		instance.isAny = false;
		if(!cbStudio.getValue()&&(cbStudio.isEnabled()))
			return true;
		if(!cbOneBedrooms.getValue()&&(cbOneBedrooms.isEnabled()))
			return true;
		if(!cbTwoBedrooms.getValue()&&(cbTwoBedrooms.isEnabled()))
			return true;
		if(!cbThreeBedrooms.getValue()&&(cbThreeBedrooms.isEnabled()))
			return true;
		if(!cbFourBedrooms.getValue()&&(cbFourBedrooms.isEnabled()))
			return true;
		if(!cbFiveBedrooms.getValue()&&(cbFiveBedrooms.isEnabled()))
			return true;
		
		instance.isAny = true;
		return false;
	}

	@Override
	public void Init() {
		boolean studio = false;
		boolean one = false;
		boolean two = false;
		boolean three = false;
		boolean four = false;
		boolean five = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			switch (suite_type.getBedrooms()) {
			case 0:
				studio = true;
				break;
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
			default:
				five = true;
				break;
			}
		}
		cbStudio.setEnabled(studio);
		cbOneBedrooms.setEnabled(one);
		cbTwoBedrooms.setEnabled(two);
		cbThreeBedrooms.setEnabled(three);
		cbFourBedrooms.setEnabled(four);
		cbFiveBedrooms.setEnabled(five);
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
		int bedrooms = type.getBedrooms();
		if (cbStudio.getValue() && bedrooms == 0)
			return true;
		else if (cbOneBedrooms.getValue() && bedrooms == 1)
			return true;
		else if (cbTwoBedrooms.getValue() && bedrooms == 2)
			return true;
		else if (cbThreeBedrooms.getValue() && bedrooms == 3)
			return true;
		else if (cbFourBedrooms.getValue() && bedrooms == 4)
			return true;
		else if (cbFiveBedrooms.getValue() && bedrooms > 4)
			return true;
		return false;
	}

	private boolean isAny = true;

	@Override
	public boolean isAny() {
		return isAny;
	}
}
