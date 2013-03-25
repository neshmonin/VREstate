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
	private static CheckBox cbOneAndHalfBathroom = null;
	private static CheckBox cbTwoBathrooms = null;
	private static CheckBox cbTwoAndHalfBathrooms = null;
	private static CheckBox cbThreeBathrooms = null;
	private static CheckBox cbThreeAndHalfBathrooms = null;
	private static CheckBox cbFourBathrooms = null;
	private static CheckBox cbFourAndHalfBathrooms = null;
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

		cbAny = new MyCustomCheckBox("Any, or");
		cbAny.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				instance.isAny = cbAny.getValue().booleanValue();
				if (instance.isAny) {
					cbOneBathroom.setValue(true, false);
					cbOneAndHalfBathroom.setValue(true, false);
					cbTwoBathrooms.setValue(true, false);
					cbTwoAndHalfBathrooms.setValue(true, false);
					cbThreeBathrooms.setValue(true, false);
					cbThreeAndHalfBathrooms.setValue(true, false);
					cbFourBathrooms.setValue(true, false);
					cbFourAndHalfBathrooms.setValue(true, false);
					cbFiveBathrooms.setValue(true, false);
				}
				instance.UpdateCaption();
			}
		});
		instance.add(cbAny);

		cbOneBathroom = new CheckBox("1 Bathroom");
		cbOneBathroom.addStyleDependentName("margined");
		cbOneBathroom.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbOneBathroom);

		cbOneAndHalfBathroom = new CheckBox("1.5 Bathroom");
		cbOneAndHalfBathroom.addStyleDependentName("margined");
		cbOneAndHalfBathroom
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbOneAndHalfBathroom);

		cbTwoBathrooms = new CheckBox("2 Bathrooms");
		cbTwoBathrooms.addStyleDependentName("margined");
		cbTwoBathrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBathroomsUnchecked())
					cbAny.setValue(true, true);
				cbAny.setValue(!isAtLeastOneUnchecked(), true);
			}
		});
		instance.add(cbTwoBathrooms);

		cbTwoAndHalfBathrooms = new CheckBox("2.5 Bathrooms");
		cbTwoAndHalfBathrooms.addStyleDependentName("margined");
		cbTwoAndHalfBathrooms
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);
						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbTwoAndHalfBathrooms);

		cbThreeBathrooms = new CheckBox("3 Bathrooms");
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

		cbThreeAndHalfBathrooms = new CheckBox("3.5 Bathrooms");
		cbThreeAndHalfBathrooms.addStyleDependentName("margined");
		cbThreeAndHalfBathrooms
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);

						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbThreeAndHalfBathrooms);

		cbFourBathrooms = new CheckBox("4 Bathrooms");
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

		cbFourAndHalfBathrooms = new CheckBox("4.5 Bathrooms");
		cbFourAndHalfBathrooms.addStyleDependentName("margined");
		cbFourAndHalfBathrooms
				.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
					public void onValueChange(ValueChangeEvent<Boolean> event) {
						if (isAllBathroomsUnchecked())
							cbAny.setValue(true, true);

						cbAny.setValue(!isAtLeastOneUnchecked(), true);
					}
				});
		instance.add(cbFourAndHalfBathrooms);

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
		if (cbOneAndHalfBathroom.getValue() && (cbOneAndHalfBathroom.isEnabled()))
			return false;
		if (cbTwoBathrooms.getValue() && (cbTwoBathrooms.isEnabled()))
			return false;
		if (cbTwoAndHalfBathrooms.getValue() && (cbTwoAndHalfBathrooms.isEnabled()))
			return false;
		if (cbThreeBathrooms.getValue() && (cbThreeBathrooms.isEnabled()))
			return false;
		if (cbThreeAndHalfBathrooms.getValue() && (cbThreeAndHalfBathrooms.isEnabled()))
			return false;
		if (cbFourBathrooms.getValue() && (cbFourBathrooms.isEnabled()))
			return false;
		if (cbFourAndHalfBathrooms.getValue() && (cbFourAndHalfBathrooms.isEnabled()))
			return false;
		if (cbFiveBathrooms.getValue() && (cbFiveBathrooms.isEnabled()))
			return false;
		return true;
	}

	private static boolean isAtLeastOneUnchecked() {
		instance.isAny = false;
		if (!cbOneBathroom.getValue() && (cbOneBathroom.isEnabled()))
			return true;
		if (!cbOneAndHalfBathroom.getValue() && (cbOneAndHalfBathroom.isEnabled()))
			return true;
		if (!cbTwoBathrooms.getValue() && (cbTwoBathrooms.isEnabled()))
			return true;
		if (!cbTwoAndHalfBathrooms.getValue() && (cbTwoAndHalfBathrooms.isEnabled()))
			return true;
		if (!cbThreeBathrooms.getValue() && (cbThreeBathrooms.isEnabled()))
			return true;
		if (!cbThreeAndHalfBathrooms.getValue() && (cbThreeAndHalfBathrooms.isEnabled()))
			return true;
		if (!cbFourBathrooms.getValue() && (cbFourBathrooms.isEnabled()))
			return true;
		if (!cbFourAndHalfBathrooms.getValue() && (cbFourAndHalfBathrooms.isEnabled()))
			return true;
		if (!cbFiveBathrooms.getValue() && (cbFiveBathrooms.isEnabled()))
			return true;

		instance.isAny = true;
		return false;
	}

	@Override
	public void Init() {
		boolean one = false;
		boolean one_half = false;
		boolean two = false;
		boolean two_half = false;
		boolean three = false;
		boolean three_half = false;
		boolean four = false;
		boolean four_half = false;
		boolean five = false;
		for (SuiteType suite_type : Document.get().getSuiteTypes()) {
			double bathrooms = suite_type.getBathrooms(); 
			if (bathrooms == 0.0)
				break;
			else if (bathrooms == 1.0)
				one = true;
			else if (bathrooms == 1.5)
				one_half = true;
			else if (bathrooms == 2.0)
				two = true;
			else if (bathrooms == 2.5)
				two_half = true;
			else if (bathrooms == 3.0)
				three = true;
			else if (bathrooms == 3.5)
				three_half = true;
			else if (bathrooms == 4.0)
				four = true;
			else if (bathrooms == 4.5)
				four_half = true;
			else
				five = true;
		}
		cbOneBathroom.setVisible(one);
		cbOneAndHalfBathroom.setVisible(one_half);
		cbTwoBathrooms.setVisible(two);
		cbTwoAndHalfBathrooms.setVisible(two_half);
		cbThreeBathrooms.setVisible(three);
		cbThreeAndHalfBathrooms.setVisible(three_half);
		cbFourBathrooms.setVisible(four);
		cbFourAndHalfBathrooms.setVisible(four_half);
		cbFiveBathrooms.setVisible(five);

		cbOneBathroom.setEnabled(one);
		cbOneAndHalfBathroom.setEnabled(one_half);
		cbTwoBathrooms.setEnabled(two);
		cbTwoAndHalfBathrooms.setEnabled(two_half);
		cbThreeBathrooms.setEnabled(three);
		cbThreeAndHalfBathrooms.setEnabled(three_half);
		cbFourBathrooms.setEnabled(four);
		cbFourAndHalfBathrooms.setEnabled(four_half);
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
		double bathrooms = type.getBathrooms();
		if (cbOneBathroom.getValue() && bathrooms == 1.0) return true;
		else if (cbOneAndHalfBathroom.getValue() && bathrooms == 1.5) return true;
		else if (cbTwoBathrooms.getValue() && bathrooms == 2.0) return true;
		else if (cbTwoAndHalfBathrooms.getValue() && bathrooms == 2.5) return true;
		else if (cbThreeBathrooms.getValue() && bathrooms == 3.0) return true;
		else if (cbThreeAndHalfBathrooms.getValue() && bathrooms == 3.5) return true;
		else if (cbFourBathrooms.getValue() && bathrooms == 4.0) return true;
		else if (cbFourAndHalfBathrooms.getValue() && bathrooms == 4.5) return true;
		else if (cbFiveBathrooms.getValue() && bathrooms > 4.5) return true;
		
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
