package com.condox.vrestate.client.filter;

import com.condox.vrestate.client.document.Document;
import com.condox.vrestate.client.document.Suite;
import com.condox.vrestate.client.document.SuiteType;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.user.client.Window;
import com.google.gwt.user.client.ui.CheckBox;
import com.google.gwt.user.client.ui.StackPanel;
import com.google.gwt.user.client.ui.VerticalPanel;

public class BathroomSection extends VerticalPanel implements I_FilterSection {

	StackPanel stackPanel = null;

	private static BathroomSection instance = null;
	private static CheckBox cbAny = null;
	private static CheckBox cbOneBathroom = null;
	private static CheckBox cbOneBathroomDens = null;
	private static CheckBox cbTwoBathrooms = null;
	private static CheckBox cbTwoBathroomsDens = null;
	private static CheckBox cbThreeBathrooms = null;
	private static CheckBox cbThreeBathroomsDens = null;
	private static CheckBox cbFourBathrooms = null;
	private static CheckBox cbFourBathroomsDens = null;
	private static CheckBox cbFiveBathrooms = null;

	private BathroomSection() {
		super();
	}

	public static BathroomSection CreateSectionPanel(String sectionLabel,
			StackPanel stackPanel) {
		// =====================================================
		boolean creating = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes())
			creating = creating || (suite_type.getBathrooms() >= 0);
		if (!creating)
			return null;
		// =====================================================
		instance = new BathroomSection();
		instance.stackPanel = stackPanel;
		instance.setSpacing(5);
		stackPanel.add(instance, "Bathrooms", false);
		instance.setSize("100%", "150px");

		cbAny = new CheckBox("Any, or");
		cbAny.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.isAny = cbAny.getValue().booleanValue();
				if (instance.isAny) {
					cbOneBathroom.setValue(true, false);
					cbTwoBathrooms.setValue(true, false);
					cbThreeBathrooms.setValue(true, false);
					cbFourBathrooms.setValue(true, false);
					cbFiveBathrooms.setValue(true, false);
				}
				instance.UpdateCaption();
			}
		});
		instance.add(cbAny);

		cbOneBathroom = new CheckBox("One bathroom");
		cbOneBathroom.addStyleDependentName("margined");
		cbOneBathroom.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbOneBathroom);

		cbOneBathroomDens = new CheckBox("One bathroom + dens");
		cbOneBathroomDens.addStyleDependentName("margined");
		cbOneBathroomDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbOneBathroomDens);

		cbTwoBathrooms = new CheckBox("Two bathrooms");
		cbTwoBathrooms.addStyleDependentName("margined");
		cbTwoBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbTwoBathrooms);

		cbTwoBathroomsDens = new CheckBox("Two bathrooms + dens");
		cbTwoBathroomsDens.addStyleDependentName("margined");
		cbTwoBathroomsDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbTwoBathroomsDens);

		cbThreeBathrooms = new CheckBox("Three bathrooms");
		cbThreeBathrooms.addStyleDependentName("margined");
		cbThreeBathrooms
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);

						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbThreeBathrooms);

		cbThreeBathroomsDens = new CheckBox("Three bathrooms + dens");
		cbThreeBathroomsDens.addStyleDependentName("margined");
		cbThreeBathroomsDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);

						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbThreeBathroomsDens);

		cbFourBathrooms = new CheckBox("Four bathrooms");
		cbFourBathrooms.addStyleDependentName("margined");
		cbFourBathrooms
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);

						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbFourBathrooms);

		cbFourBathroomsDens = new CheckBox("Four bathrooms + dens");
		cbFourBathroomsDens.addStyleDependentName("margined");
		cbFourBathroomsDens
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);

						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbFourBathroomsDens);

		cbFiveBathrooms = new CheckBox("More");
		cbFiveBathrooms.addStyleDependentName("margined");
		cbFiveBathrooms
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
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
		if (cbOneBathroom.getValue() && (cbOneBathroom.isEnabled()))
			return false;
		if (cbOneBathroomDens.getValue() && (cbOneBathroomDens.isEnabled()))
			return false;
		if (cbTwoBathrooms.getValue() && (cbTwoBathrooms.isEnabled()))
			return false;
		if (cbTwoBathroomsDens.getValue() && (cbTwoBathroomsDens.isEnabled()))
			return false;
		if (cbThreeBathrooms.getValue() && (cbThreeBathrooms.isEnabled()))
			return false;
		if (cbThreeBathroomsDens.getValue()
				&& (cbThreeBathroomsDens.isEnabled()))
			return false;
		if (cbFourBathrooms.getValue() && (cbFourBathrooms.isEnabled()))
			return false;
		if (cbFourBathroomsDens.getValue() && (cbFourBathroomsDens.isEnabled()))
			return false;
		if (cbFiveBathrooms.getValue() && (cbFiveBathrooms.isEnabled()))
			return false;
		return true;
	}

	private static boolean isAtLeastOneUnchecked() {
		instance.isAny = false;
		if (!cbOneBathroom.getValue() && (cbOneBathroom.isEnabled()))
			return true;
		if (!cbOneBathroomDens.getValue() && (cbOneBathroomDens.isEnabled()))
			return true;
		if (!cbTwoBathrooms.getValue() && (cbTwoBathrooms.isEnabled()))
			return true;
		if (!cbTwoBathroomsDens.getValue() && (cbTwoBathroomsDens.isEnabled()))
			return true;
		if (!cbThreeBathrooms.getValue() && (cbThreeBathrooms.isEnabled()))
			return true;
		if (!cbThreeBathroomsDens.getValue()
				&& (cbThreeBathroomsDens.isEnabled()))
			return true;
		if (!cbFourBathrooms.getValue() && (cbFourBathrooms.isEnabled()))
			return true;
		if (!cbFourBathroomsDens.getValue()
				&& (cbFourBathroomsDens.isEnabled()))
			return true;
		if (!cbFiveBathrooms.getValue() && (cbFiveBathrooms.isEnabled()))
			return true;

		instance.isAny = true;
		return false;
	}

	@Override
	public void Init() {
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
			switch (suite_type.getBathrooms()) {
			case 0:
				break;
			case 1:
				if (suite_type.get__Bathrooms() == 0)
					one = true;
				else
					one_dens = true;
				break;
			case 2:
				if (suite_type.get__Bathrooms() == 0)
					two = true;
				else
					two_dens = true;
				break;
			case 3:
				if (suite_type.get__Bathrooms() == 0)
					three = true;
				else
					three_dens = true;
				break;
			case 4:
				if (suite_type.get__Bathrooms() == 0)
					four = true;
				else
					four_dens = true;
				break;
			default:
				five = true;
				break;
			}
		}
		cbOneBathroom.setVisible(one);
		cbOneBathroomDens.setVisible(one_dens);
		cbTwoBathrooms.setVisible(two);
		cbTwoBathroomsDens.setVisible(two_dens);
		cbThreeBathrooms.setVisible(three);
		cbThreeBathroomsDens.setVisible(three_dens);
		cbFourBathrooms.setVisible(four);
		cbFourBathroomsDens.setVisible(four_dens);
		cbFiveBathrooms.setVisible(five);

		cbOneBathroom.setEnabled(one);
		cbOneBathroomDens.setEnabled(one_dens);
		cbTwoBathrooms.setEnabled(two);
		cbTwoBathroomsDens.setEnabled(two_dens);
		cbThreeBathrooms.setEnabled(three);
		cbThreeBathroomsDens.setEnabled(three_dens);
		cbFourBathrooms.setEnabled(four);
		cbFourBathroomsDens.setEnabled(four_dens);
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
		int dens = type.getDens();
		if (cbOneBathroom.getValue() && bathrooms == 1)
			return true;
		else if (cbOneBathroomDens.getValue() && bathrooms == 1 && dens != 0)
			return true;
		else if (cbTwoBathrooms.getValue() && bathrooms == 2)
			return true;
		else if (cbTwoBathroomsDens.getValue() && bathrooms == 2 && dens != 0)
			return true;
		else if (cbThreeBathrooms.getValue() && bathrooms == 3)
			return true;
		else if (cbThreeBathroomsDens.getValue() && bathrooms == 3 && dens != 0)
			return true;
		else if (cbFourBathrooms.getValue() && bathrooms == 4)
			return true;
		else if (cbFourBathroomsDens.getValue() && bathrooms == 4 && dens != 0)
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

	@Override
	public void Apply() {
		instance.isChanged = false;
		if (Filter.initialized == true)
			Filter.get().onChanged();

	}

	private boolean isChanged = false;
	@Override
	public boolean isChanged() {
		// TODO Auto-generated method stub
		return isChanged;
	}

	private void UpdateCaption() {
		if (isAny)
			instance.stackPanel.setStackText(
					instance.stackPanel.getWidgetIndex(instance),
					"Bathrooms (any)");
		else
			instance.stackPanel.setStackText(
					instance.stackPanel.getWidgetIndex(instance), "Bathrooms");
		instance.isChanged = true;
		if (Filter.initialized == true)
			Filter.get().onChanged();
	}
}
