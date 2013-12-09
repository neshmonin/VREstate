package com.condox.vrestate.client.filter;


import com.condox.clientshared.abstractview.Log;
import com.condox.clientshared.document.SuiteType;
import com.condox.vrestate.client.view.GeoItems.SuiteGeoItem;
import com.google.gwt.event.logical.shared.ValueChangeEvent;
import com.google.gwt.event.logical.shared.ValueChangeHandler;
import com.google.gwt.json.client.JSONBoolean;
import com.google.gwt.json.client.JSONObject;
import com.google.gwt.json.client.JSONString;
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
	private I_FilterSectionContainer parentSection;

	private BedroomsSection() {
		super();
	}

	public static I_FilterSection CreateSectionPanel(I_FilterSectionContainer parentSection, 
			String sectionLabel,
			StackPanel stackPanel) {
		Log.write("BedroomsSection(" + sectionLabel + ")");
		// =====================================================
		boolean creating = false;
		for (SuiteType suite_type : parentSection.getActiveSuiteTypes().values())
			creating = creating || (suite_type.getBedrooms() >= 0);
		if (!creating)
			return null;
		// =====================================================
		instance = new BedroomsSection();
		instance.parentSection = parentSection;
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
				instance.Apply();
			}
		});
		instance.add(cbAny);

		cbStudio = new CheckBox("Studio");
		cbStudio.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				else {
					cbAny.setValue(!isAtLeastOneUnchecked(), true);
					instance.Apply();
				}
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
				else {
					cbAny.setValue(!isAtLeastOneUnchecked(), true);
					instance.Apply();
				}
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
						else {
							cbAny.setValue(!isAtLeastOneUnchecked(), true);
							instance.Apply();
						}
					}
				});
		instance.add(cbOneBedroomsDens);

		cbTwoBedrooms = new CheckBox("Two Bedroom");
		cbTwoBedrooms.addStyleDependentName("margined");
		cbTwoBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				else {
					cbAny.setValue(!isAtLeastOneUnchecked(), true);
					instance.Apply();
				}
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
						else {
							cbAny.setValue(!isAtLeastOneUnchecked(), true);
							instance.Apply();
						}
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
						else {
							cbAny.setValue(!isAtLeastOneUnchecked(), true);
							instance.Apply();
						}
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
						else {
							cbAny.setValue(!isAtLeastOneUnchecked(), true);
							instance.Apply();
						}
					}
				});
		instance.add(cbThreeBedroomsDens);

		cbFourBedrooms = new CheckBox("Four Bedroom");
		cbFourBedrooms.addStyleDependentName("margined");
		cbFourBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				else {
					cbAny.setValue(!isAtLeastOneUnchecked(), true);
					instance.Apply();
				}
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
						else {
							cbAny.setValue(!isAtLeastOneUnchecked(), true);
							instance.Apply();
						}
					}
				});
		instance.add(cbFourBedroomsDens);

		cbFiveBedrooms = new CheckBox("More");
		cbFiveBedrooms.addStyleDependentName("margined");
		cbFiveBedrooms.addValueChangeHandler(new ValueChangeHandler<Boolean>() {
			public void onValueChange(ValueChangeEvent<Boolean> event) {
				if (isAllBedroomsUnchecked())
					cbAny.setValue(true, true);
				else {
					cbAny.setValue(!isAtLeastOneUnchecked(), true);
					instance.Apply();
				}
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

		if (!cbStudio.getValue() && (cbStudio.isEnabled())) return true;
		if (!cbOneBedrooms.getValue() && (cbOneBedrooms.isEnabled())) return true;
		if (!cbOneBedroomsDens.getValue() && (cbOneBedroomsDens.isEnabled())) return true;
		if (!cbTwoBedrooms.getValue() && (cbTwoBedrooms.isEnabled())) return true;
		if (!cbTwoBedroomsDens.getValue() && (cbTwoBedroomsDens.isEnabled())) return true;
		if (!cbThreeBedrooms.getValue() && (cbThreeBedrooms.isEnabled())) return true;
		if (!cbThreeBedroomsDens.getValue() && (cbThreeBedroomsDens.isEnabled())) return true;
		if (!cbFourBedrooms.getValue() && (cbFourBedrooms.isEnabled())) return true;
		if (!cbFourBedroomsDens.getValue() && (cbFourBedroomsDens.isEnabled())) return true;
		if (!cbFiveBedrooms.getValue() && (cbFiveBedrooms.isEnabled())) return true;

		instance.isAny = true;
		return false;
	}

	@Override
	public int StateHash() {
		int hash = hashCode();  
		if (cbStudio!=null&&cbStudio.getValue()) hash += cbStudio.hashCode(); 
		if (cbOneBedrooms!=null&&cbOneBedrooms.getValue()) hash += cbOneBedrooms.hashCode(); 
		if (cbOneBedroomsDens!=null&&cbOneBedroomsDens.getValue()) hash += cbOneBedroomsDens.hashCode(); 
		if (cbTwoBedrooms!=null&&cbTwoBedrooms.getValue()) hash += cbTwoBedrooms.hashCode(); 
		if (cbTwoBedroomsDens!=null&&cbTwoBedroomsDens.getValue()) hash += cbTwoBedroomsDens.hashCode(); 
		if (cbThreeBedrooms!=null&&cbThreeBedrooms.getValue()) hash += cbThreeBedrooms.hashCode(); 
		if (cbThreeBedroomsDens!=null&&cbThreeBedroomsDens.getValue()) hash += cbThreeBedroomsDens.hashCode(); 
		if (cbFourBedrooms!=null&&cbFourBedrooms.getValue()) hash += cbFourBedrooms.hashCode(); 
		if (cbFourBedroomsDens!=null&&cbFourBedroomsDens.getValue()) hash += cbFourBedroomsDens.hashCode(); 
		if (cbFiveBedrooms!=null&&cbFiveBedrooms.getValue()) hash += cbFiveBedrooms.hashCode(); 
		
		return hash;
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
		for (SuiteType suite_type : getParentSectionContainer().getActiveSuiteTypes().values()) {
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
	public boolean isFilteredIn(SuiteGeoItem suiteGI) {
		if (isAny)
			return true;

		SuiteType type = suiteGI.suite.getSuiteType();
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
		if (isAny)
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance),"Bedrooms (any)");
		else
			instance.stackPanel.setStackText(instance.stackPanel.getWidgetIndex(instance),"Bedrooms");
		Filter.onChange();
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
		JSONObject result = new JSONObject();
		result.put("name", new JSONString(this.getClass().getName()));
		
		if (cbStudio != null) {
			boolean studio = cbStudio.getValue();
			result.put("studio", JSONBoolean.getInstance(studio));
		}
		
		if (cbOneBedrooms != null) {
			boolean one = cbOneBedrooms.getValue();
			result.put("one", JSONBoolean.getInstance(one));
		}
		
		if (cbOneBedroomsDens != null) {
			boolean one_dens = cbOneBedroomsDens.getValue();
			result.put("one_dens", JSONBoolean.getInstance(one_dens));
		}
		
		if (cbTwoBedrooms != null) {
			boolean two = cbTwoBedrooms.getValue();
			result.put("two", JSONBoolean.getInstance(two));
		}
		
		if (cbTwoBedroomsDens != null) {
			boolean two_dens = cbTwoBedroomsDens.getValue();
			result.put("two_dens", JSONBoolean.getInstance(two_dens));
		}
		
		if (cbThreeBedrooms != null) {
			boolean three = cbThreeBedrooms.getValue();
			result.put("three", JSONBoolean.getInstance(three));
		}
		
		if (cbThreeBedroomsDens != null) {
			boolean three_dens = cbThreeBedroomsDens.getValue();
			result.put("three_dens", JSONBoolean.getInstance(three_dens));
		}
		
		if (cbFourBedrooms != null) {
			boolean four = cbFourBedrooms.getValue();
			result.put("four", JSONBoolean.getInstance(four));
		}
		
		if (cbFourBedroomsDens != null) {
			boolean four_dens = cbFourBedroomsDens.getValue();
			result.put("four_dens", JSONBoolean.getInstance(four_dens));
		}
		
		if (cbFiveBedrooms != null) {
			boolean five = cbFiveBedrooms.getValue();
			result.put("five", JSONBoolean.getInstance(five));
		}
		
//		cbStudio.setVisible(studio);
//		cbOneBedrooms.setVisible(one);
//		cbOneBedroomsDens.setVisible(one_dens);
//		cbTwoBedrooms.setVisible(two);
//		cbTwoBedroomsDens.setVisible(two_dens);
//		cbThreeBedrooms.setVisible(three);
//		cbThreeBedroomsDens.setVisible(three_dens);
//		cbFourBedrooms.setVisible(four);
//		cbFourBedroomsDens.setVisible(four_dens);
//		cbFiveBedrooms.setVisible(five);
//		
//		cbStudio.setEnabled(studio);
//		cbOneBedrooms.setEnabled(one);
//		cbOneBedroomsDens.setEnabled(one_dens);
//		cbTwoBedrooms.setEnabled(two);
//		cbTwoBedroomsDens.setEnabled(two_dens);
//		cbThreeBedrooms.setEnabled(three);
//		cbThreeBedroomsDens.setEnabled(three_dens);
//		cbFourBedrooms.setEnabled(four);
//		cbFourBedroomsDens.setEnabled(four_dens);
//		cbFiveBedrooms.setEnabled(five);
//		isAny = true;
		
		return result;
	}

	@Override
	public void fromJSONObject(JSONObject json) {
		if (json == null) return;
		
		if (!json.containsKey("name")) return;
		if (json.get("name").isString() == null) return;
		String name = json.get("name").isString().stringValue();
		
		if (name.equals(getClass().getName())) {
			if ((json.containsKey("studio")) && (json.get("studio").isBoolean() != null))
				cbStudio.setValue(json.get("studio").isBoolean().booleanValue(), true);
			
			if ((json.containsKey("one")) && (json.get("one").isBoolean() != null))
				cbOneBedrooms.setValue(json.get("one").isBoolean().booleanValue(), true);

			if ((json.containsKey("one_dens")) && (json.get("one_dens").isBoolean() != null))
				cbOneBedroomsDens.setValue(json.get("one_dens").isBoolean().booleanValue(), true);

			if ((json.containsKey("two")) && (json.get("two").isBoolean() != null))
				cbTwoBedrooms.setValue(json.get("two").isBoolean().booleanValue(), true);
			
			if ((json.containsKey("two_dens")) && (json.get("two_dens").isBoolean() != null))
				cbTwoBedroomsDens.setValue(json.get("two_dens").isBoolean().booleanValue(), true);

			if ((json.containsKey("three")) && (json.get("three").isBoolean() != null))
				cbThreeBedrooms.setValue(json.get("three").isBoolean().booleanValue(), true);
			
			if ((json.containsKey("three_dens")) && (json.get("three_dens").isBoolean() != null))
				cbThreeBedroomsDens.setValue(json.get("three_dens").isBoolean().booleanValue(), true);

			if ((json.containsKey("four")) && (json.get("four").isBoolean() != null))
				cbFourBedrooms.setValue(json.get("four").isBoolean().booleanValue(), true);
			
			if ((json.containsKey("four_dens")) && (json.get("four_dens").isBoolean() != null))
				cbFourBedroomsDens.setValue(json.get("four_dens").isBoolean().booleanValue(), true);

			if ((json.containsKey("five")) && (json.get("five").isBoolean() != null))
				cbFiveBedrooms.setValue(json.get("five").isBoolean().booleanValue(), true);
		}
	}
}
