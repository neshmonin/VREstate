package com.condox.vrestate.client.filter;

import com.condox.vrestate.client.Log;
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
	private static CheckBox cbOneBedroomsDens = null;
	private static CheckBox cbTwoBedrooms = null;
	private static CheckBox cbTwoBedroomsDens = null;
	private static CheckBox cbThreeBedrooms = null;
	private static CheckBox cbThreeBedroomsDens = null;
	private static CheckBox cbFourBedrooms = null;
	private static CheckBox cbFourBedroomsDens = null;
	private static CheckBox cbFiveBedrooms = null;

	private BedroomsSection() {
		super();
	}

	public static BedroomsSection CreateSectionPanel(String sectionLabel,
			StackPanel stackPanel) {
		// =====================================================
		boolean creating = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes())
			creating = creating || (suite_type.getBedrooms() >= 0);
		if (!creating)
			return null;
		// =====================================================
		instance = new BedroomsSection();
		instance.stackPanel = stackPanel;
		stackPanel.add(instance, "Bedrooms", false);
		instance.setSize("100%", "150px");

		cbAny = new MyCustomCheckBox("Any, or");
		cbAny.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.isAny = cbAny.getValue().booleanValue();
				if (instance.isAny) {
					cbStudio.setValue(true, false);
					cbOneBedrooms.setValue(true, false);
					cbOneBedroomsDens.setValue(true, false);
					cbTwoBedrooms.setValue(true, false);
					cbTwoBedroomsDens.setValue(true, false);
					cbThreeBedrooms.setValue(true, false);
					cbThreeBedroomsDens.setValue(true, false);
					cbFourBedrooms.setValue(true, false);
					cbFourBedroomsDens.setValue(true, false);
					cbFiveBedrooms.setValue(true, false);
				}
				instance.UpdateCaption();
			}
		});
		instance.add(cbAny);

		cbStudio = new CheckBox("Studio");
		cbStudio.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		cbStudio.addStyleDependentName("margined");
		instance.add(cbStudio);

		cbOneBedrooms = new CheckBox("One Bedroom");
		cbOneBedrooms.addStyleDependentName("margined");
		cbOneBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbOneBedrooms);

		cbOneBedroomsDens = new CheckBox("One Bedroom + Den");
		cbOneBedroomsDens.addStyleDependentName("margined");
		cbOneBedroomsDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBedroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbOneBedroomsDens);

		cbTwoBedrooms = new CheckBox("Two Bedroom");
		cbTwoBedrooms.addStyleDependentName("margined");
		cbTwoBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbTwoBedrooms);

		cbTwoBedroomsDens = new CheckBox("Two Bedroom + Den");
		cbTwoBedroomsDens.addStyleDependentName("margined");
		cbTwoBedroomsDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBedroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbTwoBedroomsDens);

		cbThreeBedrooms = new CheckBox("Three Bedroom");
		cbThreeBedrooms.addStyleDependentName("margined");
		cbThreeBedrooms
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBedroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbThreeBedrooms);

		cbThreeBedroomsDens = new CheckBox("Three Bedroom + Den");
		cbThreeBedroomsDens.addStyleDependentName("margined");
		cbThreeBedroomsDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBedroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbThreeBedroomsDens);

		cbFourBedrooms = new CheckBox("Four Bedroom");
		cbFourBedrooms.addStyleDependentName("margined");
		cbFourBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbFourBedrooms);

		cbFourBedroomsDens = new CheckBox("Four Bedroom + Den");
		cbFourBedroomsDens.addStyleDependentName("margined");
		cbFourBedroomsDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBedroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbFourBedroomsDens);

		cbFiveBedrooms = new CheckBox("More");
		cbFiveBedrooms.addStyleDependentName("margined");
		cbFiveBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbFiveBedrooms);

		return instance;
	}

	private static boolean isAllBedroomsUnchecked() {
		if (cbStudio.getValue() && (cbStudio.isEnabled()))
			return false;
		if (cbOneBedrooms.getValue() && (cbOneBedrooms.isEnabled()))
			return false;
		if (cbOneBedroomsDens.getValue() && (cbOneBedroomsDens.isEnabled()))
			return false;
		if (cbTwoBedrooms.getValue() && (cbTwoBedrooms.isEnabled()))
			return false;
		if (cbTwoBedroomsDens.getValue() && (cbTwoBedroomsDens.isEnabled()))
			return false;
		if (cbThreeBedrooms.getValue() && (cbThreeBedrooms.isEnabled()))
			return false;
		if (cbThreeBedroomsDens.getValue() && (cbThreeBedroomsDens.isEnabled()))
			return false;
		if (cbFourBedrooms.getValue() && (cbFourBedrooms.isEnabled()))
			return false;
		if (cbFourBedroomsDens.getValue() && (cbFourBedroomsDens.isEnabled()))
			return false;
		if (cbFiveBedrooms.getValue() && (cbFiveBedrooms.isEnabled()))
			return false;
		return true;
	}

	private static boolean isAtLeastOneUnchecked() {
		instance.isAny = false;

		if (!cbStudio.getValue() && (cbStudio.isEnabled()))
			return true;
		if (!cbOneBedrooms.getValue() && (cbOneBedrooms.isEnabled()))
			return true;
		if (!cbOneBedroomsDens.getValue() && (cbOneBedroomsDens.isEnabled()))
			return true;
		if (!cbTwoBedrooms.getValue() && (cbTwoBedrooms.isEnabled()))
			return true;
		if (!cbTwoBedroomsDens.getValue() && (cbTwoBedroomsDens.isEnabled()))
			return true;
		if (!cbThreeBedrooms.getValue() && (cbThreeBedrooms.isEnabled()))
			return true;
		if (!cbThreeBedroomsDens.getValue() && (cbThreeBedroomsDens.isEnabled()))
			return true;
		if (!cbFourBedrooms.getValue() && (cbFourBedrooms.isEnabled()))
			return true;
		if (!cbFourBedroomsDens.getValue() && (cbFourBedroomsDens.isEnabled()))
			return true;
		if (!cbFiveBedrooms.getValue() && (cbFiveBedrooms.isEnabled()))
			return true;

		instance.isAny = true;
		return false;
	}

	@Override
	public void Init() {
		boolean studio = false;
		boolean one = false;
		boolean one_dens = false;
		boolean two = false;
		boolean two_dens = false;
		boolean three = false;
		boolean three_dens = false;
		boolean four = false;
		boolean four_dens = false;
		boolean five = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			switch (suite_type.getBedrooms()) {
			case 0:
				studio = true;
				break;
			case 1:
				if (suite_type.getDens() == 0)
					one = true;
				else
					one_dens = true;
				break;
			case 2:
				if (suite_type.getDens() == 0)
					two = true;
				else
					two_dens = true;
				break;
			case 3:
				if (suite_type.getDens() == 0)
					three = true;
				else
					three_dens = true;
				break;
			case 4:
				if (suite_type.getDens() == 0)
					four = true;
				else
					four_dens = true;
				break;
			default:
				five = true;
				break;
			}
		}
		cbStudio.setVisible(studio);
		cbOneBedrooms.setVisible(one);
		cbOneBedroomsDens.setVisible(one_dens);
		cbTwoBedrooms.setVisible(two);
		cbTwoBedroomsDens.setVisible(two_dens);
		cbThreeBedrooms.setVisible(three);
		cbThreeBedroomsDens.setVisible(three_dens);
		cbFourBedrooms.setVisible(four);
		cbFourBedroomsDens.setVisible(four_dens);
		cbFiveBedrooms.setVisible(five);
		
		cbStudio.setEnabled(studio);
		cbOneBedrooms.setEnabled(one);
		cbOneBedroomsDens.setEnabled(one_dens);
		cbTwoBedrooms.setEnabled(two);
		cbTwoBedroomsDens.setEnabled(two_dens);
		cbThreeBedrooms.setEnabled(three);
		cbThreeBedroomsDens.setEnabled(three_dens);
		cbFourBedrooms.setEnabled(four);
		cbFourBedroomsDens.setEnabled(four_dens);
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
		if (type == null)
			Log.write("Filter->isFilteredIn: suite.getSuiteType() returned null");
		int bedrooms = type.getBedrooms();
		int dens = type.getDens();
		if (cbStudio.getValue() && bedrooms == 0 && dens == 0)
			return true;
		else if (cbOneBedrooms.getValue() && bedrooms == 1 && dens == 0)
			return true;
		else if (cbOneBedroomsDens.getValue() && bedrooms == 1 && dens != 0)
			return true;
		else if (cbTwoBedrooms.getValue() && bedrooms == 2 && dens == 0)
			return true;
		else if (cbTwoBedroomsDens.getValue() && bedrooms == 2 && dens != 0)
			return true;
		else if (cbThreeBedrooms.getValue() && bedrooms == 3 && dens == 0)
			return true;
		else if (cbThreeBedroomsDens.getValue() && bedrooms == 3 && dens != 0)
			return true;
		else if (cbFourBedrooms.getValue() && bedrooms == 4 && dens == 0)
			return true;
		else if (cbFourBedroomsDens.getValue() && bedrooms == 4 && dens != 0)
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
	
	private void UpdateCaption() {
		if (isAny)
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance),"Bedrooms (any)");
		else
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance),"Bedrooms");
		instance.isChanged = true;
		if (Filter.initialized == true)
			Filter.get().onChanged();
	}
}
